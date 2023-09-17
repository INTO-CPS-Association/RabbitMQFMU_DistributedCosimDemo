FROM ubuntu:22.04

RUN apt-get update && apt-get install --fix-missing -y python3 python3-pip

RUN pip3 install pika

CMD tail -F anything