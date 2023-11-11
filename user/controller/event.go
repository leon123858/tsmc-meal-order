package controller

import (
	"fmt"
	"github.com/labstack/echo/v4"
	"github.com/leon123858/tsmc-meal-order/user/service"
	"net/http"
)

// SyncEventMessage 同步 event
// @Summary 同步 event
// @Description 同步 event, 由 infra handle 傳入 cloud run, 若非 200 回傳, 會觸發重試
// @Tags event
// @Accept json
// @Produce json
// @Param input body service.PubSubMessage true "body"
// @Success 200 {object} string "success"
// @Router /sync [post]
func SyncEventMessage(c echo.Context) error {
	req := new(service.PubSubMessage)
	if err := c.Bind(req); err != nil {
		return c.String(http.StatusBadRequest, err.Error())
	}
	event := make(map[string]interface{})
	if err := req.BindPubSubMessageData(&event); err != nil {
		return c.String(http.StatusBadRequest, err.Error())
	}
	// print event
	fmt.Printf("%+v\n", event)
	return c.String(http.StatusOK, "success")
}
