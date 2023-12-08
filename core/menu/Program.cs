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

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

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

app.MapGet("/api/menu/{menuId}", async (string menuId, MenuService _menuService) =>
{
    Menu? menu = await _menuService.GetByIdAsync(menuId);
    if (menu != null)
        return Results.Ok(new ApiResponse<Menu> { Data = menu });
    
    return Results.NotFound(ApiResponse.NotFound());
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
    var user = await _userClient.GetUserAsync(userId);
    if (user != null)
    {
        var menus = await _menuService.GetByLocationAsync(user.Place);
        if (menus != null)
            return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = menus });
    }

    return Results.NotFound(ApiResponse.NotFound());
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
  
    var user = await _userClient.GetUserAsync(menuCreateDto.Id);
    if (user == null)
    {
        return Results.NotFound(ApiResponse.NotFound());
    }

    Menu? oldMenu = await _menuService.GetByIdAsync(menuCreateDto.Id);
    Menu menu = _mapper.Map<Menu>(menuCreateDto);
    menu.Location = user.Place;
    MenuDTO menuDto = _mapper.Map<MenuDTO>(menu);
    if (oldMenu != null)
    {
        await _menuService.UpdateAsync(menu);
    }
    else
    {
        await _menuService.CreateAsync(menu);
    }

    return Results.Ok(new ApiResponse<MenuDTO>{ Data = menuDto });
})
.WithName("CreateMenu")
.Accepts<MenuCreateDTO>("application/json")
.Produces<ApiResponse<MenuDTO>>(StatusCodes.Status201Created)
.Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Create the menu."
});

app.MapGet("/api/menu/{menuId}/foodItem/{itemIdx:int}", async (MenuService _menuService, IMapper _mapper, string menuId, int itemIdx) =>
{
    Menu? menu = await _menuService.GetByIdAsync(menuId);
    if (menu != null && itemIdx < menu.FoodItems.Count)
    {
        FoodItemDTO foodItemDto = _mapper.Map<FoodItemDTO>(menu.FoodItems[itemIdx]);
        return Results.Ok(new ApiResponse<FoodItemDTO> { Data = foodItemDto });
    }
    else
        return Results.NotFound(ApiResponse.NotFound());
})
.WithName("GetMenuItem")
.Produces<ApiResponse<FoodItemDTO>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the i-th item of the menu with the specified id"
});

app.Run();