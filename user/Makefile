all:

start:
	make format
	make swagger
	 make emulator & \
		export FIRESTORE_EMULATOR_HOST="127.0.0.1:8081" && \
		export FIREBASE_AUTH_EMULATOR_HOST="127.0.0.1:9099" &&  \
		export PUBSUB_EMULATOR_HOST="127.0.0.1:8085" &&  \
		go run main.go

swagger:
	@echo "can install swag by: go install github.com/swaggo/swag/cmd/swag@latest"
	swag init
	swag fmt ./...
	open http://localhost:8080/swagger/index.html

emulator:
	firebase emulators:start  --only firestore,auth,pubsub

format:
	go mod tidy
	go fmt ./...

trySync:
	node ./trigger-sync.js

testCover:
	go clean -testcache
	go test ./... -p=1 -cover=true
