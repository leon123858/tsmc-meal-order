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

// Login
//
//	@Summary		login api
//	@Description	login by JWT token
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			Authorization	header		string						true	"format:[Bearer JWT_Token]"
//	@Param			input			body		model.UserCoreInformation	true	"body"
//	@Success		200				{object}	model.StringResponse		"success"
//	@Router			/login [post]
func Login(c echo.Context) error {
	// get req
	req := new(model.UserCoreInformation)
	if err := c.Bind(req); err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	// validate request
	if err := c.Validate(req); err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	// get user from context
	user := c.Get("user").(*auth.Token)
	if user.UID != req.UID {
		return c.JSON(http.StatusForbidden, model.ErrorResponse("not map uid"))
	}
	// get user from firestore(db)
	userInfo, _ := service.DBClient.Collection("user").Doc(req.UID).Get(context.Background())
	if userInfo.Exists() {
		return c.JSON(http.StatusOK, model.SuccessResponse("login success"))
	}
	// publish event
	pubSubClient, err := service.NewPubSubInfo(service.PubsubClientWrapper{})
	if err != nil {
		return c.JSON(http.StatusInternalServerError, model.ErrorResponse(err.Error()))
	}
	if err := pubSubClient.Publish("user-create", model.UserCreateEvent{
		Type: "user-create",
		Data: model.UserCreateEventData{
			UserCoreInformation: model.UserCoreInformation{
				UID:   req.UID,
				Email: req.Email,
			},
			UserType: c.Get("userType").(model.UserType),
		},
	}); err != nil {
		return c.JSON(http.StatusInternalServerError, model.ErrorResponse(err.Error()))
	}

	return c.JSON(http.StatusOK, model.SuccessResponse("first login success"))
}

// CreateUser
//
//	@Summary		create user api (should be admin now)
//	@Description	create a user account, who can log in by username and password
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			input	body		model.UserCreateRequest	true	"body"
//	@Success		200		{object}	model.StringResponse	"data is uid (user id in this system)"
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
	if req.Type == "" {
		req.Type = model.Admin
	}
	// create user in firebase auth
	userInfo := (&auth.UserToCreate{}).Email(req.Email).Password(req.Password).DisplayName(req.Name)
	record, err := service.AuthClient.CreateUser(context.Background(), userInfo)
	if err != nil {
		return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
	}
	// set custom claim for admin
	err = service.AuthClient.SetCustomUserClaims(context.Background(), record.UID, map[string]interface{}{
		"type": req.Type,
	})
	if err != nil {
		return c.JSON(http.StatusInternalServerError, model.ErrorResponse(err.Error()))
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
//	@Param			uid	query		string	true	"uid"
//	@Success		200	{object}	model.UserInfoResponse
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
//	@Success		200		{object}	model.StringResponse			"data is uid (user id in this system)"
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
