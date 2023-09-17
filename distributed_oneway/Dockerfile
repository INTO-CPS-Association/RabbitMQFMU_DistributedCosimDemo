FROM ubuntu:22.04

RUN DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install --fix-missing -y openjdk-11-jdk 

# Fixes https://github.com/INTO-CPS-Association/fmu-rabbitmq/issues/57
RUN echo "deb http://security.ubuntu.com/ubuntu focal-security main" | tee --append /etc/apt/sources.list

RUN apt-get update && apt-get install -y libicu66 libssl1.1 

CMD tail -F anything
