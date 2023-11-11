package model

type UserType string

const (
	Normal UserType = "normal" // 一般使用者
	Admin  UserType = "admin"  //
)

// UserCreateRequest 創建使用 email 登入的用戶
type UserCreateRequest struct {
	Email    string   `json:"email" validate:"required"`              // email
	Password string   ` json:"password" validate:"required"`          // password
	Name     string   `json:"name" validate:"required"`               // name
	Type     UserType ` json:"type" validate:"oneof=normal admin ''"` // 使用者類型 (一般使用者, 管理者)
}

// UserInformation 用戶存在 user 資料庫的資訊
type UserInformation struct {
	UserCoreInformation
	UserGeneralInformation
	UserType UserType `json:"userType" firestore:"userType"` // 使用者類型 (一般使用者, 管理者)
}

type UserCoreInformation struct {
	UID   string `json:"uid" firestore:"uid"`     // user id (uid in this system)
	Email string `json:"email" firestore:"email"` // email
}

type UserGeneralInformation struct {
	Name  string `json:"name" firestore:"name"`   // name
	Place string `json:"place" firestore:"place"` // 取餐地點
}

type UserGeneralUpdateRequest struct {
	UID string `json:"uid" validate:"required"` // user id (uid in this system)
	UserGeneralInformation
}

type StringResponse struct {
	Response
	Data string `json:"data"`
}

type UserInfoResponse struct {
	Response
	Data UserInformation `json:"data"`
}
