FROM ubuntu:latest

RUN apt-get update && apt-get install -y openjdk-11-jdk wget tzdata

RUN DEBIAN_FRONTEND=noninteractive && apt-get install -y tzdata

RUN wget http://archive.ubuntu.com/ubuntu/pool/main/i/icu/libicu66_66.1-2ubuntu2_amd64.deb && dpkg -i libicu66_66.1-2ubuntu2_amd64.deb

RUN wget http://nz2.archive.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2.17_amd64.deb && dpkg -i libssl1.1_1.1.1f-1ubuntu2.17_amd64.deb

CMD tail -F anything
