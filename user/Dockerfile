# syntax=docker/dockerfile:1

##
## Run the tests in the container
##

FROM leon1234858/firebase-simulate-go:amd AS run-test-stage
WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

COPY . ./

RUN make ci

##
## Basic code initialization
##
FROM golang:1.21 AS init-stage

WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

COPY . ./

##
## Build the application from source
##

FROM init-stage AS build-stage

WORKDIR /app

RUN CGO_ENABLED=0 GOOS=linux go build -o /user

##
## Deploy the application binary into a lean image
##

FROM gcr.io/distroless/base-debian11 AS build-release-stage

WORKDIR /

COPY --from=build-stage /user /user
ENV APP_ENV=production

EXPOSE 8080

ENTRYPOINT ["/user"]