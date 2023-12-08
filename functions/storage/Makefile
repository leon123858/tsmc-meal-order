all:

start:
	go mod tidy
	export FUNCTION_TARGET=storage
	go run cmd/main.go

deploy:
	gcloud functions deploy storage --gen2 --region=asia-east1 --trigger-http --runtime=go121