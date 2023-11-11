package controller

import (
	"context"
	"firebase.google.com/go/v4/auth"
	"github.com/labstack/echo/v4"
	"github.com/leon123858/tsmc-meal-order/user/model"
	"github.com/leon123858/tsmc-meal-order/user/service"
	"net/http"
)

// CreateUser
//
//	@Summary		create user api (should be admin now)
//	@Description	create an user account, who can log in by username and password
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			input	body		user_model.UserCreateRequest	true	"body"
//	@Success		200		{object} user_model.UserCreateResponse
//	@Router			/create [post]
func CreateUser(c echo.Context) error {
	req := new(model.UserCreateRequest)
	if err := c.Bind(req); err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	// validate request
	if err := c.Validate(req); err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	if req.Type == model.Normal {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse("normal user can't be created by api now!"))
	}
	// create user in firebase auth
	userInfo := (&auth.UserToCreate{}).Email(req.Email).Password(req.Password).DisplayName(req.Name)
	record, err := service.AuthClient.CreateUser(context.Background(), userInfo)
	if err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	// create user in firestore(db)
	_, err = service.DBClient.Collection("user").Doc(record.UID).Set(context.Background(), model.UserInformation{
		UID:      record.UID,
		Email:    req.Email,
		Name:     req.Name,
		Place:    "",
		UserType: req.Type,
	})
	if err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}

	return c.JSON(http.StatusOK, model.SuccessResponse(record.UID))
}
