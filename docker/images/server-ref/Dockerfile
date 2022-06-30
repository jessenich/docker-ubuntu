ARG VARIANT="21.04"
FROM ubuntu:"$VARIANT" as serverref

LABEL maintainer="cykuryx@outlook.com" \
      org.opencontainers.image.source="https://github.com/jessenich/docker-ubuntu"

ENV DEBIAN_NONINTERACTIVE true

RUN apt-get update && \
    apt-get install -y --no-install-recommends ubuntu-server

FROM serverref

RUN unminimize && \
    apt-get update && \
    apt-get install -y --no-install-recommends man-db
