using AutoMapper;
using menu;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using menu.Models;
using menu.Models.DTO;
using FluentValidation;
using System.Net;
using Microsoft.AspNetCore.Http;
using core;
using core.Model;
using menu.Services;
using menu.Clients;
using menu.Config;
using Microsoft.AspNetCore.Cors;
using menu.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.Configure<MenuDatabaseConfig>(builder.Configuration.GetSection("MenuDatabase"));
builder.Services.Configure<WebConfig>(builder.Configuration.GetSection("WebApi"));
builder.Services.AddSingleton<RealMenuService>();
builder.Services.AddSingleton<TempMenuService>();
builder.Services.AddSingleton<IUserClient ,UserClient>();
builder.Services.AddSingleton<IRecClient, RecClient>();
builder.Services.AddAuthentication();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
// }

app.MapGet("/api/menu", async (TempMenuService _menuService, ILogger < Program> _logger) =>
{
    var menus = await _menuService.GetAllMenuAsync();
    return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = menus });
})
.WithName("GetAllTempMenus")
.Produces<ApiResponse<IEnumerable<Menu>>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get all the temporary menus."
});

app.MapGet("/api/menu/{menuId}", async (string menuId, RealMenuService _menuService, IUserClient _userClient, IMapper _mapper, ILogger<Program> _logger) =>
{
    try
    {
        var user = _mapper.Map<User>(await _userClient.GetUserAsync(menuId));
        if (user!.UserType != "admin")
            return Results.BadRequest(ApiResponse.BadRequest("Not an admin user."));

        var menu = await _menuService.GetMenuAsync(menuId);
        return Results.Ok(new ApiResponse<Menu> { Data = menu! });
    }
    catch (UserNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
    catch(MenuNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("GetRealMenuForAdmin")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "For admin user, get his / her real menu."
});

app.MapGet("/api/menu/user/{userId}", async (string userId, TempMenuService _menuService, IUserClient _userClient, IMapper _mapper, ILogger<Program> _logger) =>
{
    try
    {
        var user = _mapper.Map<User>(await _userClient.GetUserAsync(userId));
        var menus = await _menuService.GetMenusByLocationAsync(user!.Place);
        return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = menus });
    }
    catch (UserNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("GetTempMenuForUser")
.Produces<ApiResponse<IEnumerable<Menu>>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "For normal user, get all the temporary menus with the same location."
});

app.MapPost("/api/menu/", async ([FromBody] MenuCreateDTO menuCreateDto, RealMenuService _menuService, IUserClient _userClient, ILogger<Program> _logger, IMapper _mapper, IValidator<MenuCreateDTO> _validator) =>
{
    var validResult = await _validator.ValidateAsync(menuCreateDto);
    if (!validResult.IsValid)
    {
        string errorMsg = validResult.Errors.FirstOrDefault()!.ToString();
        return Results.BadRequest(ApiResponse.BadRequest(errorMsg));
    }

    Menu menu = _mapper.Map<Menu>(menuCreateDto);
    foreach (var foodItem in menu.FoodItems)
    {
        foodItem.Count = foodItem.CountLimit;
    }

    try
    {
        var user = _mapper.Map<User>(await _userClient.GetUserAsync(menu.Id));
        menu.Location = user!.Place;

        Menu? oldMenu = await _menuService.GetMenuAsync(menu.Id);
        await _menuService.UpdateMenuAsync(menu);
    }
    catch (UserNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
    catch (MenuNotFoundException)
    {
        await _menuService.CreateMenuAsync(menu);
    }
    
    MenuDTO menuDto = _mapper.Map<MenuDTO>(menu);
    return Results.Ok(new ApiResponse<MenuDTO>{ Data = menuDto });
})
.WithName("CreateMenu")
.Accepts<MenuCreateDTO>("application/json")
.Produces<ApiResponse<MenuDTO>>(StatusCodes.Status201Created)
.Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Create a real menu if non-existent, update it otherwise"
});

app.MapGet("/api/menu/{menuId}/foodItem/{itemIdx:int}", async (string menuId, int itemIdx, TempMenuService _menuService, ILogger<Program> _logger, IMapper _mapper) =>
{
    try
    {
        Menu? menu = await _menuService.GetMenuAsync(menuId);
        FoodItem? foodItem = _menuService.GetFoodItem(menu, itemIdx);
        FoodItemDTO foodItemDto = _mapper.Map<FoodItemDTO>(foodItem);

        return Results.Ok(new ApiResponse<FoodItemDTO> { Data = foodItemDto });
    }
    catch(MenuNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
    catch(FoodItemNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("GetTempMenuItem")
.Produces<ApiResponse<FoodItemDTO>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the i-th item of the temporary menu with the specified id"
});

app.MapPost("/api/menu/sync", async (RealMenuService _realMenu, TempMenuService _tempMenu, IRecClient _recClient, IMapper _mapper, ILogger<Program> _logger) =>
{
    var menus = await _realMenu.GetAllMenuAsync();
    foreach(var menu in menus)
    {
        try
        {
            await _tempMenu.GetMenuAsync(menu.Id);
            await _tempMenu.UpdateMenuAsync(menu);
        }
        catch (MenuNotFoundException)
        {
            await _tempMenu.CreateMenuAsync(menu);
        }

        await _recClient.SyncRecMenuAsync(_mapper.Map<MenuDTO>(menu));
    }

    return Results.Ok(new ApiResponse<object>());
})
.WithName("SyncTempMenu")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Sync the data of the temp menu, from the real menu"
});

app.MapPost("/api/menu/{menuId}/foodItem/{itemIdx:int}/{decreaseCount:int}", async (string menuId, int itemIdx, int decreaseCount, TempMenuService _menuService, ILogger < Program > _logger) =>
{
    try
    {
        Menu? menu = await _menuService.GetMenuAsync(menuId);
        FoodItem? foodItem = _menuService.GetFoodItem(menu, itemIdx);
        foodItem!.Count -= decreaseCount;
        
        if (foodItem.Count < 0)
        {
            return Results.BadRequest(ApiResponse.BadRequest("food item count should not be negative"));
        }
        else if(foodItem.Count > foodItem.CountLimit)
        {
            return Results.BadRequest(ApiResponse.BadRequest("food item count exceed the limit."));
        }

        menu!.FoodItems[itemIdx] = foodItem;
        await _menuService.UpdateMenuAsync(menu);

        return Results.Ok(new ApiResponse<object>());
    }
    catch(MenuNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
    catch (FoodItemNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("DecreaseTempFoodItemCount")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Decrease the count of the given food item by x, which can be a negative number, on the temporary menu."
});

app.MapGet("/api/menu/recommend/{user_input}", async (string user_input, TempMenuService _menuService, IRecClient _recClient, ILogger<Program> _logger, IMapper _mapper) =>
{
    try
    {
        var recResult = await _recClient.GetRecAsync(user_input);
        var recFoodItems = new RecResultDTO();
        foreach(var recItem in recResult!)
        {
            Console.WriteLine(recItem.MenuId);
            Menu? menu = await _menuService.GetMenuAsync(recItem.MenuId);
            FoodItem? foodItem = _menuService.GetFoodItem(menu, recItem.Index);
            recFoodItems.FoodItems.Add(_mapper.Map<FoodItemDTO>(foodItem));
        }

        return Results.Ok(new ApiResponse<RecResultDTO> { Data = recFoodItems });
    }
    catch (MenuNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
    catch (FoodItemNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("GetRecommendTempMenuItem")
.Produces<ApiResponse<RecResultDTO>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Given the user input, get the food items sorted by a Language Model."
});

app.Run();