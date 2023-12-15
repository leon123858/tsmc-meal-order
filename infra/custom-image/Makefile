all:

arm:
	docker build -t leon1234858/firebase-simulate-go:arm -f ./Dockerfile .
amd:
	docker buildx build --platform linux/amd64 -t leon1234858/firebase-simulate-go:amd -f ./Dockerfile .
push:
	docker push leon1234858/firebase-simulate-go:amd