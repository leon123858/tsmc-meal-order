package service

import (
	"cloud.google.com/go/storage"
	"context"
	firebase "firebase.google.com/go/v4"
	"firebase.google.com/go/v4/auth"
	"fmt"
	"github.com/leon123858/tsmc-meal-order/functions/storage/utils"
	"io"
	"log"
	"mime/multipart"
	"time"
)

var AuthClient *auth.Client
var GcsBucket *storage.BucketHandle

func InitFirebase() (err error) {
	var app *firebase.App
	app, err = firebase.NewApp(context.Background(), &firebase.Config{
		ProjectID:     utils.ProjectID,
		StorageBucket: utils.StorageBucket,
	})
	if err != nil {
		return
	}
	AuthClient, err = app.Auth(context.Background())
	if err != nil {
		return
	}
	client, err := app.Storage(context.Background())
	if err != nil {
		log.Fatalln(err)
	}
	GcsBucket, err = client.DefaultBucket()
	if err != nil {
		log.Fatalln(err)
	}
	return nil
}

func GcsUploadFile(file multipart.File, name, path string) error {
	ctx := context.Background()

	ctx, cancel := context.WithTimeout(ctx, time.Second*20)
	defer cancel()

	// Upload an object with storage.Writer.
	wc := GcsBucket.Object(path + name).NewWriter(ctx)
	if _, err := io.Copy(wc, file); err != nil {
		return fmt.Errorf("io.Copy: %v", err)
	}
	if err := wc.Close(); err != nil {
		return fmt.Errorf("Writer.Close: %v", err)
	}

	return nil
}

func GetFileGcsUrl(path, name string) (string, error) {
	obj := GcsBucket.Object("images/" + name)
	err := obj.ACL().Set(context.Background(), storage.AllUsers, storage.RoleReader)
	if err != nil {
		return "", err
	}
	return utils.StorageBucketURL + path + name, nil
}
