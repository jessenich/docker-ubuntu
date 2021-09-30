export SHELL = /bin/bash

export ROOT_DIR := $(dir $(realpath $(lastword $(MAKEFILE_LIST))))
export DOCKER_DIR := $(realpath $(ROOT_DIR)docker)
export IMAGES_DIR := $(realpath $(DOCKER_DIR)images)
export BUILD_DIR := $(realpath $(ROOT_DIR)build)
export BUILD_SCRIPTS_DIR := $(realpath $(BUILD_DIR)scripts)

export DOKCERIO_USERNAME2 = jessenich91
export DOKCERIO_USERNAME2 = jessenich
export GHCR_USERNAME = jessenich

export DOCKERIO_PAT1 = $(shell cat $(DOCKER_DIR)/secrets/dockerhub-jessenich91-pat.secret-file.txt)
export DOCKERIO_PAT2 = $(shell cat $(DOCKER_DIR)/secrets/dockerhub-jessenich-pat.secret-file.txt)
export GHCR_PAT = $(shell cat $(DOCKER_DIR)/secrets/github-jessenich-pat.secret-file.txt)

export DOCKERIO_REGISTRY = docker.io
export GHCR_REGISTRY = ghcr.io
export DOCKERIO_LIBRARY := $(DOCKERIO-REGISTRY)/jessenich91
export GHCR_LIBRARY := $(GHCR-REGISTRY)/jessenich

export IMAGE_LABELS = $(shell \
cat <<EOF
--label "maintainer=Jesse N. <jesse@keplerdev.com>" \
--label "org.opencontainers.image.source=https://github.com/jessenich/docker-syslog-ng"
EOF
)

export SEMVER_TAG = v1.0.0
export LATEST_TAG = latest
export PLATFORMS_STRING = linux/amd64,linux/arm64/v8,linux/arm/v7

$(info importing $(DOCKER_DIR)server-ref/server-ref.mk...)
include $(DOCKER_DIR)/server-ref/server-ref.mk

$(info Importing $(BUILD_DIR)...)
include $(BUILD_DIR)/build/mk

LAST_TAG = $(echo "$(shell git) | grep -oP 'refs/tags/\K(.+)')"

## 1 - Major, Minor, Patch, or Prerelease.
## 2 - Increment by how many?
## 3 - Force override any existing images or tags
INCREMENT_CMD = dotnet script $(BUILD_SCRIPTS_DIR)bump_semver.csx -- --increment $(2:-"1") --$(1:-"patch") $(3:="false")

.PHONY: init bump-semver-patch

init:
	@dotnet script $(BUILD_SCRIPTS_DIR)bump_semver.csx -- --ini

bump-semver-path:
	$(shell $(call INCREMENT_CMD,patch,1,false))
	
