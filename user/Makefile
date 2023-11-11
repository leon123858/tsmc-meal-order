all:

start:
	make format
	make swagger
	 make emulator & \
		export FIRESTORE_EMULATOR_HOST="127.0.0.1:8081" && \
		export FIREBASE_AUTH_EMULATOR_HOST="127.0.0.1:9099" &&  \
		go run main.go

swagger:
	swag init
	swag fmt ./...
	open http://localhost:8080/swagger/index.html

emulator:
	firebase emulators:start  --only firestore,auth

format:
	go mod tidy
	go fmt ./...

trySync:
	node ./trigger-sync.js