package model

type Response struct {
	Result  bool        `json:"result"`  // true: success, false: error
	Message string      `json:"message"` // error message
	Data    interface{} `json:"data"`    // response data
}

func SuccessResponse(data interface{}) Response {
	return Response{
		Result:  true,
		Message: "success",
		Data:    data,
	}
}

func ErrorResponse(message string) Response {
	return Response{
		Result:  false,
		Message: message,
		Data:    nil,
	}
}

type UserData struct {
	Email    string `json:"email"`
	Name     string `json:"name"`
	Place    string `json:"place"`
	UID      string `json:"uid"`
	UserType string `json:"userType"`
}

// UserResponse is the response from user service
type UserResponse struct {
	Response,
	Data UserData
}
