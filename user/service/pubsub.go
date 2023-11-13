package service

import (
	"cloud.google.com/go/pubsub"
	"context"
	"encoding/json"
	"github.com/leon123858/tsmc-meal-order/user/utils"
)

type PubsubClientWrapper struct {
	ProjectID string
	Client    *pubsub.Client
}

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

func (req *PubSubMessage) BindPubSubMessageData(obj interface{}) error {
	err := json.Unmarshal([]byte(string(req.Message.Data)), obj)
	if err != nil {
		return err
	}
	return nil
}

func NewPubSubInfo(info PubsubClientWrapper) (*PubsubClientWrapper, error) {
	if info.ProjectID == "" {
		info.ProjectID = utils.GcpProjectId
	}
	client, err := pubsub.NewClient(context.Background(), info.ProjectID)
	if err != nil {
		return nil, err
	}
	return &PubsubClientWrapper{
		ProjectID: info.ProjectID,
		Client:    client,
	}, nil
}

func (info *PubsubClientWrapper) Publish(topicID string, data interface{}) error {
	topic := info.Client.Topic(topicID)
	defer topic.Stop()
	dataByte, err := json.Marshal(data)
	if err != nil {
		return err
	}
	result := topic.Publish(context.Background(), &pubsub.Message{
		Data: dataByte,
	})
	_, err = result.Get(context.Background())
	if err != nil {
		return err
	}
	return nil
}
