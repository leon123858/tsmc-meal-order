# syntax=docker/dockerfile:1

##
## Basic code initialization
##
FROM golang:1.21 AS init-stage

WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

COPY . ./

##
## Run the tests in the container
##

FROM node:18 AS run-test-stage
WORKDIR /app

COPY . ./
# install node, fireabse tools and JDK
RUN apt-get update && apt-get upgrade -y
RUN apt-get install -y python3-launchpadlib
RUN apt-get install -y software-properties-common
RUN add-apt-repository ppa:longsleep/golang-backports
RUN apt-get update
RUN apt-get install -y golang-go openjdk-17-jdk
RUN npm install -g firebase-tools
# go operation
COPY go.mod go.sum ./
RUN go mod download
# run test
RUN make ci

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