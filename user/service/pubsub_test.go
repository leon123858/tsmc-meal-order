package service

import (
	"cloud.google.com/go/pubsub"
	"context"
	"encoding/json"
	"fmt"
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestPubSubMessage_BindPubSubMessageData(t *testing.T) {
	type Message struct {
		Data []byte `json:"data,omitempty"`
		ID   string `json:"id"`
	}
	type fields struct {
		Message
		Subscription string
	}
	type args struct {
		obj interface{}
	}
	tests := []struct {
		name   string
		fields fields
		args   args
		final  string
	}{
		{
			name: "success",
			args: args{
				obj: new(struct {
					Data struct {
						UID      string `json:"uid"`
						Email    string `json:"email"`
						UserType string `json:"user_type"`
					} `json:"data"`
				}),
			},
			fields: fields{
				Message: Message{
					Data: []byte(`{"data":{"uid":"123","email":"test@gg.cc","user_type":"user"}}`),
					ID:   "123",
				},
				Subscription: "user-create-event",
			},
			final: "{\"data\":{\"uid\":\"123\",\"email\":\"test@gg.cc\",\"user_type\":\"user\"}}",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			req := &PubSubMessage{
				Message:      tt.fields.Message,
				Subscription: tt.fields.Subscription,
			}
			if err := req.BindPubSubMessageData(tt.args.obj); err != nil {
				t.Errorf("BindPubSubMessageData() error = %v", err)
			}
			// object to string
			dataByte, err := json.Marshal(tt.args.obj)
			if err != nil {
				t.Errorf("Marshal() error = %v", err)
			}
			if string(dataByte) != tt.final {
				t.Errorf("BindPubSubMessageData() final = %v, want %v", string(dataByte), tt.final)
			}
		})
	}
}

func TestPubSubMessage_BindPubSubMessageData1(t *testing.T) {
	type fields struct {
		Message struct {
			Data []byte `json:"data,omitempty"`
			ID   string `json:"id"`
		}
		Subscription string
	}
	type args struct {
		obj interface{}
	}
	tests := []struct {
		name    string
		fields  fields
		args    args
		wantErr assert.ErrorAssertionFunc
	}{
		{
			name: "error",
			args: args{
				obj: new(struct {
					Data struct {
						UID      string `json:"uid"`
						Email    string `json:"email"`
						UserType string `json:"user_type"`
					} `json:"data"`
				}),
			},
			fields: fields{
				Message: struct {
					Data []byte `json:"data,omitempty"`
					ID   string `json:"id"`
				}{
					Data: []byte(`{"data":{"uid":"123","email":"`),
					ID:   "123",
				},
				Subscription: "user-create-event",
			},
			wantErr: assert.Error,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			req := &PubSubMessage{
				Message:      tt.fields.Message,
				Subscription: tt.fields.Subscription,
			}
			tt.wantErr(t, req.BindPubSubMessageData(tt.args.obj), fmt.Sprintf("BindPubSubMessageData(%v)", tt.args.obj))
		})
	}
}

func TestNewPubSubInfo(t *testing.T) {
	setEnv()
	if InitFirebase() != nil {
		return
	}
	_, e := NewPubSubInfo(PubsubClientWrapper{})
	if e != nil {
		t.Error(e)
	}
}

func TestPubsubClientWrapper_Publish(t *testing.T) {
	setEnv()
	if InitFirebase() != nil {
		t.Error("InitFirebase() error")
	}
	ctx := context.Background()
	client, err := pubsub.NewClient(ctx, "tsmc-meal-order")
	if err != nil {
		t.Error(err)
	}
	wrapper := PubsubClientWrapper{
		ProjectID: "tsmc-meal-order",
		Client:    client,
	}
	_, err = client.CreateTopic(ctx, "user-create-topic")
	if err != nil {
		if err.Error() != "rpc error: code = AlreadyExists desc = Topic already exists" {
			t.Error(err)
		}
	}
	if wrapper.Publish("user-create-topic", map[string]string{
		"test": "test",
	}) != nil {
		t.Error(err)
	}
}
