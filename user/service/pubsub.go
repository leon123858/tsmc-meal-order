package service

import "encoding/json"

// PubSubMessage is the payload of a Pub/Sub event.
// See the documentation for more details:
// https://cloud.google.com/pubsub/docs/reference/rest/v1/PubsubMessage
type PubSubMessage struct {
	Message struct {
		Data []byte `json:"data,omitempty"`
		ID   string `json:"id"`
	} `json:"message"`
	Subscription string `json:"subscription"`
}

func (req *PubSubMessage) BindPubSubMessageData(obj *interface{}) error {
	err := json.Unmarshal(req.Message.Data, obj)
	if err != nil {
		return err
	}
	return nil
}
