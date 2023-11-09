package controller

import (
	"github.com/labstack/echo/v4"
	"github.com/leon123858/tsmc-meal-order/user/model/user_model"
	"net/http"
)

// HelloWorld
//
//	@Summary		Hello World
//	@Description	Hello World
//	@Tags			Hello World
//	@Accept			json
//	@Produce		json
//	@Success		200	{string}	string	"Hello, World!"
//	@Router			/ [get]
func HelloWorld(c echo.Context) error {
	return c.String(http.StatusOK, "Hello, World!")
}

// AdminRegister
//
//	@Summary		Admin Register
//	@Description	create an admin account, who can login by username and password
//	@Tags			admin
//	@Accept			json
//	@Produce		json
//	@Param			input	body		user_model.UserCreateRequest	true	"body"
//	@Success		200		{string}	string							"Hello, World!"
//	@Router			/user/register/admin [post]
func AdminRegister(c echo.Context) error {
	req := new(user_model.UserCreateRequest)
	if err := c.Bind(req); err != nil {
		return err
	}
	return c.String(http.StatusOK, "Hello, World!")
}
