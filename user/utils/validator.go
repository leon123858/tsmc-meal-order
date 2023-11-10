package utils

import "github.com/go-playground/validator/v10"

// CustomValidator 是一個用於在 Echo 中自定義驗證器的結構
type CustomValidator struct {
	Validator *validator.Validate
}

// Validate 實現了 echo.Validator 介面中的 Validate 方法
func (cv *CustomValidator) Validate(i interface{}) error {
	return cv.Validator.Struct(i)
}
