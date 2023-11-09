package main

import (
	"github.com/labstack/echo/v4"
	"github.com/labstack/echo/v4/middleware"
	"github.com/leon123858/tsmc-meal-order/user/controller"
	_ "github.com/leon123858/tsmc-meal-order/user/docs"
	echoSwagger "github.com/swaggo/echo-swagger"
)

//	@title			meal order user API
//	@version		1.0
//	@description	this the user service for meal order system

//	@contact.name	Leon Lin

//	@license.name	Apache 2.0
//	@license.url	http://www.apache.org/licenses/LICENSE-2.0.html

// @host		127.0.0.1:8080
// @BasePath	/api
func main() {
	e := echo.New()
	e.Use(middleware.CORS())
	e.Use(middleware.Logger())

	e.GET("/swagger/*", echoSwagger.WrapHandler)

	apiGroup := e.Group("/api")
	apiGroup.GET("/", controller.HelloWorld)

	userGroup := apiGroup.Group("/user")
	userGroup.POST("/register/admin", controller.AdminRegister)

	e.Logger.Fatal(e.Start(":8080"))
}
