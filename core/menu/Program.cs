using AutoMapper;
using menu;
using Microsoft.AspNetCore.Mvc;
using menu.Models;
using menu.Models.DTO;
using FluentValidation;
using core;
using core.Model;
using menu.Services;
using menu.Clients;
using menu.Config;
using menu.Exceptions;
using menu.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.Configure<MenuDatabaseConfig>(builder.Configuration.GetSection("MenuDatabase"));
builder.Services.Configure<WebConfig>(builder.Configuration.GetSection("WebApi"));
builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddSingleton<IMenuRepository, MenuRepository>();
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

app.MapGet("/api/menu", async (IMenuService _menuService, ILogger < Program> _logger) =>
{
    bool isTempMenu = true;
    var menus = await _menuService.GetAllMenuAsync(isTempMenu);
    return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = menus });
})
.WithName("GetAllTempMenus")
.Produces<ApiResponse<IEnumerable<Menu>>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get all the temporary menus."
});

app.MapGet("/api/menu/{menuId}", async (string menuId, IMenuService _menuService, IUserClient _userClient, IMapper _mapper, ILogger<Program> _logger) =>
{
    try
    {
        var user = _mapper.Map<User>(await _userClient.GetUserAsync(menuId));
        if (user!.UserType != "admin")
            return Results.BadRequest(ApiResponse.BadRequest("Not an admin user."));

        bool isTempMenu = false;
        var menu = await _menuService.GetMenuAsync(menuId, isTempMenu);
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
.WithName("GetMenuForAdmin")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "For admin user, get his / her menu."
});

app.MapGet("/api/menu/user/{userId}", async (string userId, IMenuService _menuService, IUserClient _userClient, IMapper _mapper, ILogger<Program> _logger) =>
{
    try
    {
        int countLimit = 100;
        bool isTempMenu = true;
        var user = _mapper.Map<User>(await _userClient.GetUserAsync(userId));
        var menus = await _menuService.GetMenusByLocationAsync(user!.Place, countLimit, isTempMenu);
        return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = menus });
    }
    catch (UserNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("GetTempMenuForUserByLocation")
.Produces<ApiResponse<IEnumerable<Menu>>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "For normal user, get the temporary menus with the same location, but no more than 100 items for performance issues."
});

app.MapPost("/api/menu/", async ([FromBody] MenuCreateDTO menuCreateDto, IMenuService _menuService, IUserClient _userClient, ILogger<Program> _logger, IMapper _mapper, IValidator<MenuCreateDTO> _validator) =>
{
    var validResult = await _validator.ValidateAsync(menuCreateDto);
    if (!validResult.IsValid)
    {
        string errorMsg = validResult.Errors.FirstOrDefault()!.ToString();
        return Results.BadRequest(ApiResponse.BadRequest(errorMsg));
    }

    bool isTempMenu = false;
    Menu menu = _mapper.Map<Menu>(menuCreateDto);
    foreach (var foodItem in menu.FoodItems)
    {
        foodItem.Count = foodItem.CountLimit;
    }

    try
    {
        var user = _mapper.Map<User>(await _userClient.GetUserAsync(menu.Id));
        menu.Location = user!.Place;

        Menu? oldMenu = await _menuService.GetMenuAsync(menu.Id, isTempMenu);
        await _menuService.UpdateMenuAsync(menu, isTempMenu);
    }
    catch (UserNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
    catch (MenuNotFoundException)
    {
        await _menuService.CreateMenuAsync(menu, isTempMenu);
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
    Summary = "Create a menu if non-existent, update it otherwise"
});

app.MapGet("/api/menu/{menuId}/foodItem/{itemIdx:int}", async (string menuId, int itemIdx, IMenuService _menuService, ILogger<Program> _logger, IMapper _mapper) =>
{
    try
    {
        bool isTempMenu = true;
        Menu? menu = await _menuService.GetMenuAsync(menuId, isTempMenu);
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

app.MapPost("/api/menu/sync", async (IMenuService _menuService, IRecClient _recClient, IMapper _mapper, ILogger<Program> _logger) =>
{
    bool isTempMenu = false;
    var menus = await _menuService.GetAllMenuAsync(isTempMenu);

    isTempMenu = true;
    foreach (var menu in menus)
    {
        try
        {
            await _menuService.GetMenuAsync(menu.Id, isTempMenu);
            await _menuService.UpdateMenuAsync(menu, isTempMenu);
        }
        catch (MenuNotFoundException)
        {
            await _menuService.CreateMenuAsync(menu, isTempMenu);
        }
    }

    return Results.Ok(new ApiResponse<object>());
})
.WithName("SyncTempMenu")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Sync the data of the temporary menu, from the menu"
});

app.MapPost("/api/menu/{menuId}/foodItem/{itemIdx:int}/{decreaseCount:int}", async (string menuId, int itemIdx, int decreaseCount, IMenuService _menuService, IValidator<FoodItem> _validator, ILogger < Program > _logger) =>
{
    try
    {
        bool isTempMenu = true;
        Menu? menu = await _menuService.GetMenuAsync(menuId, isTempMenu);
        FoodItem? foodItem = _menuService.GetFoodItem(menu, itemIdx);
        foodItem!.Count -= decreaseCount;

        var validResult = await _validator.ValidateAsync(foodItem);
        if (!validResult.IsValid)
        {
            string errorMsg = validResult.Errors.FirstOrDefault()!.ToString();
            return Results.BadRequest(ApiResponse.BadRequest(errorMsg));
        }

        menu!.FoodItems[itemIdx] = foodItem;
        await _menuService.UpdateMenuAsync(menu, isTempMenu);

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

app.MapGet("/api/menu/recommend/{userId}/{userInput}", async (string userId, string userInput, IMenuService _menuService, IUserClient _userClient, IRecClient _recClient, ILogger<Program> _logger, IMapper _mapper) =>
{
    bool isTempMenu = true;
    var recResult = await _recClient.GetRecAsync(userInput);
    var recFoodItems = new RecResultDTO();
    int countLimit = 100;

    try
    {
        var user = _mapper.Map<User>(await _userClient.GetUserAsync(userId));
        foreach (var recItem in recResult!)
        {
            if (recFoodItems.FoodItems.Count >= countLimit)
                break;

            try
            {
                Menu? menu = await _menuService.GetMenuAsync(recItem.MenuId, isTempMenu);
                if (menu!.Location != user.Place)
                    continue;

                FoodItem? foodItem = _menuService.GetFoodItem(menu, recItem.Index);
                recFoodItems.FoodItems.Add(_mapper.Map<FoodItemDTO>(foodItem));
            }
            catch (MenuNotFoundException e)
            {
                _logger.LogError(e.Message);
                continue;
            }
            catch (FoodItemNotFoundException e)
            {
                _logger.LogError(e.Message);
                continue;
            }
        }
    }
    catch (UserNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }

    return Results.Ok(new ApiResponse<RecResultDTO> { Data = recFoodItems });
})
.WithName("GetRecTempMenuItemForUserByLocation")
.Produces<ApiResponse<RecResultDTO>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the recommended food items within the same locaiton as the user, but no more than 100 items for performance issues."
});

app.Run();