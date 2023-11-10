all:

start:
	make format
	make swagger
	export GOOGLE_APPLICATION_CREDENTIALS="./key.json" && go run main.go

swagger:
	swag init
	swag fmt ./...
	open http://localhost:8080/swagger/index.html

format:
	go mod tidy
	go fmt ./...