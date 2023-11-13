package main

import (
	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/labstack/echo/v4/middleware"
	"github.com/leon123858/tsmc-meal-order/user/controller"
	"github.com/leon123858/tsmc-meal-order/user/docs"
	"github.com/leon123858/tsmc-meal-order/user/service"
	"github.com/leon123858/tsmc-meal-order/user/utils"
	echoSwagger "github.com/swaggo/echo-swagger"
	"os"
)

func init() {
	// init firebase
	if err := service.InitFirebase(); err != nil {
		panic(err)
	}
}

//	@title			meal order user API
//	@version		3.0
//	@description	this the user service for meal order system

//	@contact.name	Leon Lin

//	@license.name	Apache 2.0
//	@license.url	http://www.apache.org/licenses/LICENSE-2.0.html

// @schemes http https
// @host		127.0.0.1:8080
// @BasePath	/api/user
func main() {
	e := echo.New()
	e.Use(middleware.CORS())
	e.Use(middleware.LoggerWithConfig(middleware.LoggerConfig{
		Format: "method=${method}, uri=${uri}, status=${status}\n",
	}))

	e.Validator = &utils.CustomValidator{Validator: validator.New()}

	if os.Getenv("APP_ENV") == "production" {
		e.Debug = false
		docs.SwaggerInfo.Host = utils.GcpRunUrlHost
	}
	e.GET("/swagger/*", echoSwagger.WrapHandler)

	apiGroup := e.Group("/api/user")

	apiGroup.POST("/create", controller.CreateUser)
	apiGroup.GET("/get", controller.GetUser)
	apiGroup.POST("/update", controller.UpdateUser)
	apiGroup.POST("/login", controller.Login)

	apiGroup.POST("/sync/:event", controller.SyncEventMessage)

	e.Logger.Fatal(e.Start(":8080"))
}
