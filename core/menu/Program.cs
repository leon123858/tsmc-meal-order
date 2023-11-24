using AutoMapper;
using menu;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using menu.Models;
using menu.Models.Data;
using menu.Models.DTO;
using FluentValidation;
using System.Net;
using Microsoft.AspNetCore.Http;
using core;
using core.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.MapGet("/api/menu", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting all menus.");
    return Results.Ok(new ApiResponse<IEnumerable<Menu>> { Data = MenuList.menuList });
})
.WithName("GetMenus")
.Produces<ApiResponse<IEnumerable<Menu>>>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get all the menus."
});

app.MapGet("/api/menu/{menuId:Guid}", (Guid menuId) =>
{
    // todo : refactor to mongo db
    Menu? menu = MenuList.menuList.FirstOrDefault(m => m.Id == menuId);

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

app.MapPost("/api/menu/", async ([FromBody] MenuCreateDto menuCreateDto, ILogger<Program> _logger, IMapper _mapper, IValidator<MenuCreateDto> _validator) =>
{
    var validResult = await _validator.ValidateAsync(menuCreateDto);
    if (!validResult.IsValid)
    {
        string errorMsg = validResult.Errors.FirstOrDefault()!.ToString();
        return Results.BadRequest(ApiResponse.BadRequest(errorMsg));
    }

    // todo: get location of the user from UserService

    Menu menu = _mapper.Map<Menu>(menuCreateDto);
    MenuDto menuDto = _mapper.Map<MenuDto>(menu);

    // todo : refactor to mongo db
    MenuList.menuList.Add(menu);


    _logger.Log(LogLevel.Information, "Create a new menu with id: {menu.Id}", menu.Id);
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

app.MapPut("/api/menu/", async ([FromBody] MenuUpdateDto menuUpdateDto, ILogger<Program> _logger, IMapper _mapper, IValidator<MenuUpdateDto> _validator) =>
{
    var validResult = await _validator.ValidateAsync(menuUpdateDto);
    if (!validResult.IsValid)
    {
        string errorMsg = validResult.Errors.FirstOrDefault()!.ToString();
        return Results.BadRequest(ApiResponse.BadRequest(errorMsg));
    }
    
    Menu newMenu = _mapper.Map<Menu>(menuUpdateDto);

    // todo : refactor to mongo db
    Menu? menu = MenuList.menuList.FirstOrDefault(m => m.Id == newMenu.Id);
    if (menu == null)
        return Results.NotFound(ApiResponse.NotFound());

    menu.Name = newMenu.Name;
    menu.FoodItems = newMenu.FoodItems;
    MenuDto menuDto = _mapper.Map<MenuDto>(menu);

    _logger.Log(LogLevel.Information, "Update a menu with id: {menu.Id}", menu.Id);
    return Results.Ok(new ApiResponse<MenuDto> { Data = menuDto });
})
.WithName("UpdateMenu")
.Accepts<MenuUpdateDto>("application/json")
.Produces<ApiResponse<MenuDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Update the menu, if exist."
});

app.MapDelete("/api/menu/{menuId:Guid}", (Guid menuId) =>
{
    // todo : refactor to mongo db
    Menu? menu = MenuList.menuList.FirstOrDefault(m => m.Id == menuId);

    if (menu != null)
    {
        MenuList.menuList.Remove(menu);
        return Results.Ok(new ApiResponse<object>());
    }
    else
    {
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("DeleteMenu")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Delete the menu with the specified id, if exist."
});

app.MapGet("/api/menu/{menuId:Guid}/foodItem/{itemIdx:int}", (IMapper _mapper, Guid menuId, int itemIdx) =>
{
    // todo : refactor to mongo db
    Menu? menu = MenuList.menuList.FirstOrDefault(m => m.Id == menuId); // for now, the menuId is the same as the owerId
    if (menu != null && itemIdx < menu.FoodItems.Count)
    {
        FoodItemDto foodItemDto = _mapper.Map<FoodItemDto>(menu.FoodItems[itemIdx]);
        return Results.Ok(new ApiResponse<FoodItemDto> { Data = foodItemDto });
    }
    else
    {
        return Results.NotFound(ApiResponse.NotFound());
    }
})
.WithName("GetMenuItem")
.Produces<ApiResponse<FoodItemDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the i-th item of the menu with the specified id"
});

app.Run();