package middleware

import (
	"context"
	"encoding/json"
	"fmt"
	"github.com/labstack/echo/v4"
	"github.com/leon123858/tsmc-meal-order/functions/storage/model"
	"github.com/leon123858/tsmc-meal-order/functions/storage/service"
	"github.com/leon123858/tsmc-meal-order/functions/storage/utils"
	"io"
	"net/http"
)

func GenerateJwtFirebaseAuthHandler(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) (err error) {
		// get token from header
		token := c.Request().Header.Get("Authorization")
		if token == "" {
			return c.JSON(http.StatusForbidden, model.ErrorResponse("token is required"))
		}
		// replace Bearer
		if len(token) < 7 || token[:7] != "Bearer " {
			return c.JSON(http.StatusForbidden, model.ErrorResponse("invalid token"))
		}
		token = token[7:]
		// verify token
		user, err := service.AuthClient.VerifyIDToken(context.Background(), token)
		if err != nil {
			return c.JSON(http.StatusForbidden, model.ErrorResponse(err.Error()))
		}
		// set user to context
		c.Set("userId", user.UID)
		return next(c)
	}
}

func GenerateToAskUserInfoCheckAdminHandler(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) (err error) {
		// get user id from context
		userId := c.Get("userId").(string)
		// get user info by http request
		response, err := http.Get(utils.UserServiceHost + "/api/user/get?uid=" + userId)
		if err != nil {
			fmt.Println("Error making GET request:", err)
			return c.JSON(http.StatusInternalServerError, model.ErrorResponse(err.Error()))
		}
		defer func(Body io.ReadCloser) {
			err := Body.Close()
			if err != nil {
				panic(err)
			}
		}(response.Body)

		// Read the response body
		body, err := io.ReadAll(response.Body)
		if err != nil {
			fmt.Println("Error reading response body:", err)
			return c.JSON(http.StatusInternalServerError, model.ErrorResponse(err.Error()))
		}

		// map response body to map<string, string>

		userInfo := model.UserResponse{}
		err = json.Unmarshal(body, &userInfo)
		if err != nil {
			fmt.Println("Error unmarshal response body:", err)
			return c.JSON(http.StatusInternalServerError, model.ErrorResponse(err.Error()))
		}

		// check user type
		if userInfo.Data.UserType != "admin" {
			return c.JSON(http.StatusForbidden, model.ErrorResponse("user is not admin"))
		}

		return next(c)
	}
}
