ARG VARIANT="20.04"
FROM ubuntu:"$VARIANT"
LABEL maintainer="Jesse N. <jesse@keplerdev.com" \
      org.opencontainers.image.source="https://github.com/jessenich/docker-ubuntu"

ENV DEBIAN_FRONTEND=noninteractive \
    VARIANT=$VARIANT

RUN apt-get update && \
    apt-get install -y \
        zsh \
        zshdb \
        zsh-doc \
        ubuntu-server

CMD "/bin/bash -c"