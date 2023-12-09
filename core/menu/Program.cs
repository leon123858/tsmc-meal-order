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
using menu.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.Configure<MenuDatabaseConfig>(builder.Configuration.GetSection("MenuDatabase"));
builder.Services.Configure<WebConfig>(builder.Configuration.GetSection("WebApi"));
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<IUserClient ,UserClient>();
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

app.MapGet("/api/menu", async (MenuService _menuService, ILogger < Program> _logger) =>
{
    var menus = await _menuService.GetAllAsync();
    return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = menus });
})
.WithName("GetAllMenus")
.Produces<ApiResponse<IEnumerable<Menu>>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get all the menus."
});

app.MapGet("/api/menu/{menuId}", async (string menuId, MenuService _menuService, ILogger<Program> _logger) =>
{
    try
    {
        Menu? menu = await _menuService.GetMenuAsync(menuId);
        return Results.Ok(new ApiResponse<Menu> { Data = menu! });
    }
    catch(MenuNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("GetMenuById")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the menu with the specified ower id, if exist."
});

app.MapGet("/api/menu/user/{userId}", async (string userId, MenuService _menuService, IUserClient _userClient, ILogger<Program> _logger) =>
{
    try
    {
        var user = await _userClient.GetUserAsync(userId);
        var menus = await _menuService.GetMenusByLocationAsync(user!.Place);
        return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = menus });
    }
    catch (UserNotFoundException e)
    {
        _logger.LogError(e.Message);
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("GetMenuForUser")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get all the menus with the same location as the specified user id, if exist."
});

app.MapPost("/api/menu/", async ([FromBody] MenuCreateDTO menuCreateDto, MenuService _menuService, IUserClient _userClient, ILogger<Program> _logger, IMapper _mapper, IValidator<MenuCreateDTO> _validator) =>
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
        var user = await _userClient.GetUserAsync(menuCreateDto.Id);
        menu.Location = user!.Place;

        Menu? oldMenu = await _menuService.GetMenuAsync(menuCreateDto.Id);
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
    Summary = "Create the menu. Note that the tags of each food item cannot be more than 4"
});

app.MapGet("/api/menu/{menuId}/foodItem/{itemIdx:int}", async (string menuId, int itemIdx, MenuService _menuService, ILogger<Program> _logger, IMapper _mapper) =>
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
.WithName("GetMenuItem")
.Produces<ApiResponse<FoodItemDTO>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the i-th item of the menu with the specified id"
});

app.MapPost("/api/menu/foodItem", async (MenuService _menuService, ILogger<Program> _logger) =>
{
    var menus = await _menuService.GetAllAsync();
    foreach(var menu in menus)
    {
        foreach (var item in menu.FoodItems)
        {
            item.Count = item.CountLimit;
        }

        await _menuService.UpdateMenuAsync(menu);
    }

    return Results.Ok(new ApiResponse<object>());
})
.WithName("ResetFoodItemCount")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Reset the count of all foodItems to the given limit"
});

app.MapPost("/api/menu/{menuId}/foodItem/{itemIdx:int}/{decreaseCount:int}", async (string menuId, int itemIdx, int decreaseCount, MenuService _menuService, ILogger < Program > _logger) =>
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
.WithName("DecreaseFoodItemCount")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Decrease the count of the given food item by x, which can be a negative number."
});

app.Run();