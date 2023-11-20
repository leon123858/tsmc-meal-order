package service

import (
	"context"
	"firebase.google.com/go/v4/auth"
	"github.com/labstack/echo/v4"
	"github.com/leon123858/tsmc-meal-order/user/model"
	"net/http"
)

func GenerateJwtAuthHandler(next echo.HandlerFunc) echo.HandlerFunc {
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
		user, err := AuthClient.VerifyIDToken(context.Background(), token)
		if err != nil {
			return c.JSON(http.StatusForbidden, model.ErrorResponse(err.Error()))
		}
		// set user to context
		c.Set("user", user)
		return next(c)
	}
}

func GenerateJwtGetUserTypeHandler(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) error {
		// get user from context
		user := c.Get("user").(*auth.Token)
		userType := user.Claims["type"]
		if userType == nil {
			userType = ""
		}
		// set to normal directly when user type is invalid
		typeOfUserType, ok := model.Parse2UserType(userType.(string))
		if !ok {
			typeOfUserType = model.Normal
			if err := AuthClient.SetCustomUserClaims(context.Background(), user.UID, map[string]interface{}{
				"type": typeOfUserType,
			}); err != nil {
				return c.JSON(http.StatusInternalServerError, model.ErrorResponse(err.Error()))
			}
		}
		c.Set("userType", typeOfUserType)
		return next(c)
	}
}
