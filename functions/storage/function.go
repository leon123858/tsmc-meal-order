package storage

import (
	"github.com/GoogleCloudPlatform/functions-framework-go/functions"
	"github.com/labstack/echo/v4"
	"github.com/labstack/echo/v4/middleware"
	customMiddleware "github.com/leon123858/tsmc-meal-order/functions/storage/middleware"
	"github.com/leon123858/tsmc-meal-order/functions/storage/model"
	"github.com/leon123858/tsmc-meal-order/functions/storage/service"
	"mime/multipart"
)

func init() {
	// backend service
	if err := service.InitFirebase(); err != nil {
		panic(err)
	}
	// http server
	e := echo.New()
	e.Use(middleware.CORS())
	e.Use(middleware.LoggerWithConfig(middleware.LoggerConfig{
		Format: "method=${method}, uri=${uri}, status=${status}\n",
	}))
	// api
	e.GET("/", introduce)
	e.POST("/api/storage/upload/image", uploadImage, customMiddleware.GenerateJwtFirebaseAuthHandler, customMiddleware.GenerateToAskUserInfoCheckAdminHandler)
	// start server
	e.Logger.Fatal(e.Start(":8080"))
	functions.HTTP("storage", e.ServeHTTP)
}

func introduce(c echo.Context) error {
	return c.String(200, "Here is storage service. please read README.md for more information.")
}

func uploadImage(c echo.Context) error {
	// Get the file from the request
	file, err := c.FormFile("image")
	if err != nil {
		return c.JSON(400, model.ErrorResponse(err.Error()))
	}

	// Open the uploaded file
	src, err := file.Open()
	if err != nil {
		return c.JSON(400, model.ErrorResponse(err.Error()))
	}
	defer func(src multipart.File) {
		err := src.Close()
		if err != nil {
			panic(err)
		}
	}(src)

	// check file type is image
	if file.Header.Get("Content-Type")[:6] != "image/" {
		return c.JSON(400, model.ErrorResponse("file type is not image"))
	}

	// image path
	path := "images/"
	// Upload to firebase storage
	err = service.GcsUploadFile(src, file.Filename, path)
	if err != nil {
		return c.JSON(500, model.ErrorResponse(err.Error()))
	}
	// let object be public
	url, err := service.GetFileGcsUrl(path, file.Filename)
	if err != nil {
		return c.JSON(500, model.ErrorResponse(err.Error()))
	}

	return c.JSON(200, model.SuccessResponse(url))
}
