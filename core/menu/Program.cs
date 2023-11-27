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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.Configure<MenuDatabaseSettings>(
    builder.Configuration.GetSection("MenuDatabase")
);
builder.Services.AddSingleton<MenuService>();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.MapGet("/api/menu", async (MenuService _menuService, ILogger < Program> _logger) =>
{
    var menus = await _menuService.GetAsync();
    return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = menus });
})
.WithName("GetMenus")
.Produces<ApiResponse<IEnumerable<Menu>>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get all the menus."
});

app.MapGet("/api/menu/{menuId}", async (string menuId, MenuService _menuService) =>
{
    Menu? menu = await _menuService.GetAsync(menuId);
    if (menu != null)
        return Results.Ok(new ApiResponse<Menu> { Data = menu });
    else
        return Results.NotFound(ApiResponse.NotFound());
})
.WithName("GetMenu")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the menu with the specified ower id, if exist."
});

app.MapPost("/api/menu/", async ([FromBody] MenuCreateDto menuCreateDto, MenuService _menuService, ILogger<Program> _logger, IMapper _mapper, IValidator<MenuCreateDto> _validator) =>
{
    var validResult = await _validator.ValidateAsync(menuCreateDto);
    if (!validResult.IsValid)
    {
        string errorMsg = validResult.Errors.FirstOrDefault()!.ToString();
        return Results.BadRequest(ApiResponse.BadRequest(errorMsg));
    }

    Menu? oldMenu = await _menuService.GetAsync(menuCreateDto.Id);
    Menu menu = _mapper.Map<Menu>(menuCreateDto);
    if (oldMenu != null)
    {
        await _menuService.UpdateAsync(menu);
    }
    else
    {
        // todo: get real location of the user from UserService
        string tmpLocation = "Taipei";
        menu.Location = tmpLocation;
        await _menuService.CreateAsync(menu);
    }

    MenuDto menuDto = _mapper.Map<MenuDto>(menu);
    return Results.Ok(new ApiResponse<MenuDto>{ Data = menuDto });
})
.WithName("CreateMenu")
.Accepts<MenuCreateDto>("application/json")
.Produces<ApiResponse<MenuDto>>(StatusCodes.Status201Created)
.Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
.WithOpenApi(operation => new(operation)
{
    Summary = "Create the menu."
});

app.MapGet("/api/menu/{menuId}/foodItem/{itemIdx:int}", async (MenuService _menuService, IMapper _mapper, string menuId, int itemIdx) =>
{
    Menu? menu = await _menuService.GetAsync(menuId);
    if (menu != null && itemIdx < menu.FoodItems.Count)
    {
        FoodItemDto foodItemDto = _mapper.Map<FoodItemDto>(menu.FoodItems[itemIdx]);
        return Results.Ok(new ApiResponse<FoodItemDto> { Data = foodItemDto });
    }
    else
        return Results.NotFound(ApiResponse.NotFound());
})
.WithName("GetMenuItem")
.Produces<ApiResponse<FoodItemDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the i-th item of the menu with the specified id"
});

app.Run();