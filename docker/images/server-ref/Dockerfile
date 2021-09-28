ARG VARIANT="21.04"
FROM ubuntu:"$VARIANT"
LABEL maintainer="Jesse N. <jesse@keplerdev.com" \
      org.opencontainers.image.source="https://github.com/jessenich/docker-ubuntu"
      
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && \
    apt-get install -y ubuntu-server

CMD "/bin/bash -c"