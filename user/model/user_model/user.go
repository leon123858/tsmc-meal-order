package user_model

type UserCreateRequest struct {
	ID       string `query:"id" json:"id" validate:"required"`
	Password string `query:"password" json:"password" validate:"required"`
	Email    string `query:"email" json:"email" validate:"required"`
	Name     string `query:"name" json:"name" validate:"required"`
}
