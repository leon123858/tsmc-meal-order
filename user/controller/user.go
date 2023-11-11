package controller

import (
	"cloud.google.com/go/firestore"
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
//	@Param			input	body		model.UserCreateRequest	true	"body"
//	@Success		200		{object} model.StringResponse "data is uid (user id in this system)"
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
		UserCoreInformation: model.UserCoreInformation{
			UID:   record.UID,
			Email: req.Email,
		},
		UserGeneralInformation: model.UserGeneralInformation{
			Name:  req.Name,
			Place: "",
		},
		UserType: req.Type,
	})
	if err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}

	return c.JSON(http.StatusOK, model.SuccessResponse(record.UID))
}

// GetUser
//
//	@Summary		get user api
//	@Description	get user information by uid
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			uid	query	string	true	"uid"
//	@Success		200		{object} model.UserInfoResponse
//	@Router			/get [get]
func GetUser(c echo.Context) error {
	uid := c.QueryParam("uid")
	if uid == "" {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse("uid is required"))
	}
	user, err := service.DBClient.Collection("user").Doc(uid).Get(context.Background())
	if err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	if user.Exists() == false {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse("user not found"))
	}
	return c.JSON(http.StatusOK, model.SuccessResponse(user.Data()))
}

// UpdateUser
//
//	@Summary		update user api
//	@Description	update user information by uid
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			input	body		model.UserGeneralUpdateRequest	true	"body"
//	@Success		200		{object} model.StringResponse "data is uid (user id in this system)"
//	@Router			/update [post]
func UpdateUser(c echo.Context) error {
	req := new(model.UserGeneralUpdateRequest)
	if err := c.Bind(req); err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	// validate request
	if err := c.Validate(req); err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	// update user in firestore(db)
	_, err := service.DBClient.Collection("user").Doc(req.UID).Update(context.Background(), []firestore.Update{
		{Path: "place", Value: req.Place},
		{Path: "name", Value: req.Name},
	})
	if err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}

	return c.JSON(http.StatusOK, model.SuccessResponse(req.UID))
}
