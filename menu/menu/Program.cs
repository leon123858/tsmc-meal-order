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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/menu", (ILogger<Program> _logger) =>
{
    APIResponse response = new();
    response.Result = MenuList.menuList;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    _logger.Log(LogLevel.Information, "Getting all menus.");
    return Results.Ok(response);
})
.WithName("GetMenus")
.Produces<APIResponse>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get all the menus."
});

app.MapGet("/api/menu/{id:int}", (int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };
    Menu? menu = MenuList.menuList.FirstOrDefault(m => m.Id == id);

    if (menu != null)
    {
        response.Result = menu;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessages.Add("Invalid id.");
        return Results.NotFound(response);
    }
})
.WithName("GetMenu")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the menu with the specified id, if exist."
});

app.MapPost("/api/menu/", async ([FromBody] MenuCreateDto menuCreateDto, ILogger<Program> _logger, IMapper _mapper, IValidator<MenuCreateDto> _validator) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validResult = await _validator.ValidateAsync(menuCreateDto);
    if (!validResult.IsValid)
    {
        response.ErrorMessages.Add(validResult.Errors.FirstOrDefault()!.ToString());
        return Results.BadRequest(response);
    }

    Menu menu = _mapper.Map<Menu>(menuCreateDto);
    menu.Id = MenuList.generateNewId();
    MenuList.menuList.Add(menu);
    MenuDto menuDto = _mapper.Map<MenuDto>(menu);

    response.Result = menuDto;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;

    _logger.Log(LogLevel.Information, "Generate a new menu with id: {menu.Id}", menu.Id);

    return Results.Ok(response);
})
.WithName("CreateMenu")
.Accepts<MenuCreateDto>("application/json")
.Produces<APIResponse>(StatusCodes.Status201Created)
.Produces<APIResponse>(StatusCodes.Status400BadRequest)
.WithOpenApi(operation => new(operation)
{
    Summary = "Create the menu."
});

app.MapPut("/api/menu/", async ([FromBody] MenuUpdateDto menuUpdateDto, ILogger<Program> _logger, IMapper _mapper, IValidator<MenuUpdateDto> _validator) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validResult = await _validator.ValidateAsync(menuUpdateDto);
    if (!validResult.IsValid)
    {
        response.ErrorMessages.Add(validResult.Errors.FirstOrDefault()!.ToString());
        return Results.BadRequest(response);
    }

    Menu? menu = MenuList.menuList.FirstOrDefault(m => m.Id == menuUpdateDto.Id);
    if (menu == null)
    {
        response.StatusCode = HttpStatusCode.NotFound;
        response.ErrorMessages.Add("Menu id not found.");
        return Results.NotFound(response);
    }
    menu.Name = menuUpdateDto.Name;
    menu.FoodItems = menuUpdateDto.FoodItems;

    MenuDto menuDto = _mapper.Map<MenuDto>(menu);
    response.Result = menuDto;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    _logger.Log(LogLevel.Information, "Update a menu with id: {menu.Id}", menu.Id);

    return Results.Ok(response);
})
.WithName("UpdateMenu")
.Accepts<MenuUpdateDto>("application/json")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status400BadRequest)
.Produces<APIResponse>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Update the menu, if exist."
});

app.MapDelete("/api/menu/{id:int}", (int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    Menu? menu = MenuList.menuList.FirstOrDefault(m => m.Id == id);
    if (menu != null)
    {
        MenuList.menuList.Remove(menu);
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessages.Add("Invalid id.");
        return Results.NotFound(response);
    }
})
.WithName("DeleteMenu")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound)
.WithOpenApi(operation => new(operation)
{
    Summary = "Delete the menu with the specified id, if exist."
});

app.Run();