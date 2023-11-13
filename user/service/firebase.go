package service

import (
	"cloud.google.com/go/firestore"
	"context"
	firebase "firebase.google.com/go/v4"
	"firebase.google.com/go/v4/auth"
	"github.com/leon123858/tsmc-meal-order/user/utils"
)

var AuthClient *auth.Client
var DBClient *firestore.Client

func InitFirebase() (err error) {
	var app *firebase.App
	app, err = firebase.NewApp(context.Background(), &firebase.Config{
		ProjectID: utils.GcpProjectId,
	})
	if err != nil {
		return
	}
	AuthClient, err = app.Auth(context.Background())
	if err != nil {
		return
	}
	DBClient, err = app.Firestore(context.Background())
	if err != nil {
		return
	}
	return nil
}
