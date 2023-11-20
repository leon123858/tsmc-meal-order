package controller

import (
	"context"
	"github.com/labstack/echo/v4"
	"github.com/leon123858/tsmc-meal-order/user/model"
	"github.com/leon123858/tsmc-meal-order/user/service"
	"net/http"
)

// SyncEventMessage 同步 event
//
//	@Summary		同步 event
//	@Description	同步 event, 由 infra handle 傳入 cloud run, 若非 200 回傳, 會觸發重試
//	@Tags			event
//	@Accept			json
//	@Produce		json
//	@Param			input	body		service.PubSubMessage	true	"body"
//	@Success		200		{object}	string					"success"
//	@Router			/sync/:event [post]
func SyncEventMessage(c echo.Context) error {
	req := new(service.PubSubMessage)
	if err := c.Bind(req); err != nil {
		println(err.Error())
		return c.String(http.StatusBadRequest, err.Error())
	}
	eventType := c.Param("event")
	switch eventType {
	case "user-create-event":
		event := new(model.UserCreateEvent)
		if err := req.BindPubSubMessageData(event); err != nil {
			println(err.Error())
			return c.String(http.StatusBadRequest, err.Error())
		}
		// create user in firestore(db)
		_, err := service.DBClient.Collection("user").Doc(event.Data.UID).Set(context.Background(), model.UserInformation{
			UserCoreInformation: model.UserCoreInformation{
				UID:   event.Data.UID,
				Email: event.Data.Email,
			},
			UserGeneralInformation: model.UserGeneralInformation{
				Name:  "",
				Place: "",
			},
			UserType: event.Data.UserType,
		})
		if err != nil {
			println(err.Error())
			return c.JSON(http.StatusBadRequest, model.ErrorResponse(err.Error()))
		}
	default:
		println("invalid subscription id" + req.Subscription)
		return c.String(http.StatusBadRequest, "invalid subscription id")
	}
	return c.String(http.StatusOK, "success")
}
