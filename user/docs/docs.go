// Package docs Code generated by swaggo/swag. DO NOT EDIT
package docs

import "github.com/swaggo/swag"

const docTemplate = `{
    "schemes": {{ marshal .Schemes }},
    "swagger": "2.0",
    "info": {
        "description": "{{escape .Description}}",
        "title": "{{.Title}}",
        "contact": {
            "name": "Leon Lin"
        },
        "license": {
            "name": "Apache 2.0",
            "url": "http://www.apache.org/licenses/LICENSE-2.0.html"
        },
        "version": "{{.Version}}"
    },
    "host": "{{.Host}}",
    "basePath": "{{.BasePath}}",
    "paths": {
        "/create": {
            "post": {
                "description": "create a user account, who can log in by username and password",
                "consumes": [
                    "application/json"
                ],
                "produces": [
                    "application/json"
                ],
                "tags": [
                    "user"
                ],
                "summary": "create user api (should be admin now)",
                "parameters": [
                    {
                        "description": "body",
                        "name": "input",
                        "in": "body",
                        "required": true,
                        "schema": {
                            "$ref": "#/definitions/model.UserCreateRequest"
                        }
                    }
                ],
                "responses": {
                    "200": {
                        "description": "data is uid (user id in this system)",
                        "schema": {
                            "$ref": "#/definitions/model.StringResponse"
                        }
                    }
                }
            }
        },
        "/get": {
            "get": {
                "description": "get user information by uid",
                "consumes": [
                    "application/json"
                ],
                "produces": [
                    "application/json"
                ],
                "tags": [
                    "user"
                ],
                "summary": "get user api",
                "parameters": [
                    {
                        "type": "string",
                        "description": "uid",
                        "name": "uid",
                        "in": "query",
                        "required": true
                    }
                ],
                "responses": {
                    "200": {
                        "description": "OK",
                        "schema": {
                            "$ref": "#/definitions/model.UserInfoResponse"
                        }
                    }
                }
            }
        },
        "/login": {
            "post": {
                "description": "login by JWT token",
                "consumes": [
                    "application/json"
                ],
                "produces": [
                    "application/json"
                ],
                "tags": [
                    "user"
                ],
                "summary": "login api",
                "parameters": [
                    {
                        "type": "string",
                        "description": "JWT token",
                        "name": "Authorization",
                        "in": "header",
                        "required": true
                    },
                    {
                        "description": "body",
                        "name": "input",
                        "in": "body",
                        "required": true,
                        "schema": {
                            "$ref": "#/definitions/model.UserCoreInformation"
                        }
                    }
                ],
                "responses": {
                    "200": {
                        "description": "success",
                        "schema": {
                            "$ref": "#/definitions/model.StringResponse"
                        }
                    }
                }
            }
        },
        "/sync/:event": {
            "post": {
                "description": "同步 event, 由 infra handle 傳入 cloud run, 若非 200 回傳, 會觸發重試",
                "consumes": [
                    "application/json"
                ],
                "produces": [
                    "application/json"
                ],
                "tags": [
                    "event"
                ],
                "summary": "同步 event",
                "parameters": [
                    {
                        "description": "body",
                        "name": "input",
                        "in": "body",
                        "required": true,
                        "schema": {
                            "$ref": "#/definitions/service.PubSubMessage"
                        }
                    }
                ],
                "responses": {
                    "200": {
                        "description": "success",
                        "schema": {
                            "type": "string"
                        }
                    }
                }
            }
        },
        "/update": {
            "post": {
                "description": "update user information by uid",
                "consumes": [
                    "application/json"
                ],
                "produces": [
                    "application/json"
                ],
                "tags": [
                    "user"
                ],
                "summary": "update user api",
                "parameters": [
                    {
                        "description": "body",
                        "name": "input",
                        "in": "body",
                        "required": true,
                        "schema": {
                            "$ref": "#/definitions/model.UserGeneralUpdateRequest"
                        }
                    }
                ],
                "responses": {
                    "200": {
                        "description": "data is uid (user id in this system)",
                        "schema": {
                            "$ref": "#/definitions/model.StringResponse"
                        }
                    }
                }
            }
        }
    },
    "definitions": {
        "model.StringResponse": {
            "type": "object",
            "properties": {
                "data": {
                    "type": "string"
                },
                "message": {
                    "description": "error message",
                    "type": "string"
                },
                "result": {
                    "description": "true: success, false: error",
                    "type": "boolean"
                }
            }
        },
        "model.UserCoreInformation": {
            "type": "object",
            "properties": {
                "email": {
                    "description": "email",
                    "type": "string"
                },
                "uid": {
                    "description": "user id (uid in this system)",
                    "type": "string"
                }
            }
        },
        "model.UserCreateRequest": {
            "type": "object",
            "required": [
                "email",
                "name",
                "password"
            ],
            "properties": {
                "email": {
                    "description": "email",
                    "type": "string"
                },
                "name": {
                    "description": "name",
                    "type": "string"
                },
                "password": {
                    "description": "password",
                    "type": "string"
                },
                "type": {
                    "description": "使用者類型 (一般使用者, 管理者)",
                    "enum": [
                        "normal",
                        "admin",
                        ""
                    ],
                    "allOf": [
                        {
                            "$ref": "#/definitions/model.UserType"
                        }
                    ]
                }
            }
        },
        "model.UserGeneralUpdateRequest": {
            "type": "object",
            "required": [
                "uid"
            ],
            "properties": {
                "name": {
                    "description": "name",
                    "type": "string"
                },
                "place": {
                    "description": "取餐地點",
                    "type": "string"
                },
                "uid": {
                    "description": "user id (uid in this system)",
                    "type": "string"
                }
            }
        },
        "model.UserInfoResponse": {
            "type": "object",
            "properties": {
                "data": {
                    "$ref": "#/definitions/model.UserInformation"
                },
                "message": {
                    "description": "error message",
                    "type": "string"
                },
                "result": {
                    "description": "true: success, false: error",
                    "type": "boolean"
                }
            }
        },
        "model.UserInformation": {
            "type": "object",
            "properties": {
                "email": {
                    "description": "email",
                    "type": "string"
                },
                "name": {
                    "description": "name",
                    "type": "string"
                },
                "place": {
                    "description": "取餐地點",
                    "type": "string"
                },
                "uid": {
                    "description": "user id (uid in this system)",
                    "type": "string"
                },
                "userType": {
                    "description": "使用者類型 (一般使用者, 管理者)",
                    "allOf": [
                        {
                            "$ref": "#/definitions/model.UserType"
                        }
                    ]
                }
            }
        },
        "model.UserType": {
            "type": "string",
            "enum": [
                "normal",
                "admin"
            ],
            "x-enum-comments": {
                "Normal": "一般使用者"
            },
            "x-enum-varnames": [
                "Normal",
                "Admin"
            ]
        },
        "service.PubSubMessage": {
            "type": "object",
            "properties": {
                "message": {
                    "type": "object",
                    "properties": {
                        "data": {
                            "type": "array",
                            "items": {
                                "type": "integer"
                            }
                        },
                        "id": {
                            "type": "string"
                        }
                    }
                },
                "subscription": {
                    "type": "string"
                }
            }
        }
    }
}`

// SwaggerInfo holds exported Swagger Info so clients can modify it
var SwaggerInfo = &swag.Spec{
	Version:          "3.0",
	Host:             "127.0.0.1:8080",
	BasePath:         "/api/user",
	Schemes:          []string{"http", "https"},
	Title:            "meal order user API",
	Description:      "this the user service for meal order system",
	InfoInstanceName: "swagger",
	SwaggerTemplate:  docTemplate,
	LeftDelim:        "{{",
	RightDelim:       "}}",
}

func init() {
	swag.Register(SwaggerInfo.InstanceName(), SwaggerInfo)
}
