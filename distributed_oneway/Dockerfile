FROM ubuntu:latest

RUN apt-get update && apt-get install -y openjdk-11-jdk wget tzdata

RUN DEBIAN_FRONTEND=noninteractive && apt-get install -y tzdata

CMD tail -F anything
