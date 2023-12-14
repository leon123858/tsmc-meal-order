package service

import (
	"firebase.google.com/go/v4/auth"
	"github.com/labstack/echo/v4"
	"github.com/stretchr/testify/assert"
	"net/http"
	"net/http/httptest"
	"os"
	"testing"
)

func setEnv() {
	// env
	if os.Setenv("FIRESTORE_EMULATOR_HOST", "127.0.0.1:8081") != nil {
		return
	}
	if os.Setenv("FIREBASE_AUTH_EMULATOR_HOST", "127.0.0.1:9099") != nil {
		return
	}
	if os.Setenv("PUBSUB_EMULATOR_HOST", "127.0.0.1:8085") != nil {
		return
	}
}

func TestGenerateJwtAuthHandler(t *testing.T) {
	setEnv()
	// Setup
	err := InitFirebase()
	if err != nil {
		t.Fatal(err)
	}
	e := echo.New()
	req := httptest.NewRequest(http.MethodGet, "/", nil)
	rec := httptest.NewRecorder()
	c := e.NewContext(req, rec)
	c.SetPath("/api/user")
	c.Request().Header.Set("Authorization", "Bearer "+"wrong token")

	// Assertions
	if assert.NoError(t, GenerateJwtAuthHandler(func(c echo.Context) error {
		return nil
	})(c)) {
		assert.Equal(t, http.StatusForbidden, rec.Code)
	}
}

func TestGenerateJwtGetUserTypeHandler(t *testing.T) {
	setEnv()
	// Setup
	err := InitFirebase()
	if err != nil {
		t.Fatal(err)
	}
	e := echo.New()
	req := httptest.NewRequest(http.MethodGet, "/", nil)
	rec := httptest.NewRecorder()
	c := e.NewContext(req, rec)
	c.SetPath("/api/user")

	c.Set("user", &auth.Token{
		UID: "4bCkldMFxoh5kP9byf7GUFsiF2t2",
	})

	// Assertions
	if assert.NoError(t, GenerateJwtGetUserTypeHandler(func(c echo.Context) error {
		return nil
	})(c)) {
		assert.Equal(t, http.StatusInternalServerError, rec.Code)
	}
}
