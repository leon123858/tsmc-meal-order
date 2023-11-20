package service

import (
	"encoding/json"
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
