# syntax=docker/dockerfile:1

FROM node:18
# install node, fireabse tools and JDK
RUN apt-get update && apt-get upgrade -y
RUN apt-get install -y python3-launchpadlib
RUN apt-get install -y software-properties-common
RUN add-apt-repository ppa:longsleep/golang-backports
RUN apt-get update
RUN apt-get install -y golang-go openjdk-17-jdk
RUN npm install -g firebase-tools