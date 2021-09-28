THIS_DIR := $(dir $(realpath $(lastword $(MAKEFILE_LIST))))

GET-DOCKER-TAG-ARGS-SCRIPT = source $(this_dir)makefile-support-functions.sh -g $(ghcr_library) -d $(dockerio_library) -l $(latest_tag) -s $(semver_tag); get-docker-tag-args -i $(1)