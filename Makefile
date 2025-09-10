PROJECT_NAME ?= signalbooster
IMAGE_OWNER ?= nickspghetti/${PROJECT_NAME}
REGISTRY_URL = ghcr.io/${IMAGE_OWNER}
IMAGE_NAME ?= ${PROJECT_NAME}
RELEASE_TAG ?= "v0"
IMAGE_VERSION ?= local
IMAGE_FULL_NAME = "${REGISTRY_URL}/${IMAGE_NAME}"
DOCKER_BUILDX_FLAG_PLATFORM ?= linux/amd64
DOCKER_BUILDX_FLAG_OUTPUT ?= docker

# Build docker image by spec
# Parameters:
#	$1 - Docker build target (production, development)
#	$2 - Docker image tag (local, local-dev)
#	$3 - Dockerfile path
define build_image_by_spec
	@docker buildx create --use
	@docker buildx build \
		--target $1 \
		--tag $2 \
		--pull \
		--file $3 \
		--output=type=${DOCKER_BUILDX_FLAG_OUTPUT} \
		--platform ${DOCKER_BUILDX_FLAG_PLATFORM} \
		.
endef

.DEFAULT_GOAL := build-cli

help:			## Show help menu
	@grep -E '^[a-zA-Z_\%-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

start:		## Start api environment
	docker-compose up --remove-orphans --build

down:		## Stop api environment
	docker-compose down --remove-orphans

remove-cli:		    ## Remove previously built cli Docker image
	@docker rmi ${IMAGE_FULL_NAME}:${IMAGE_VERSION}

remove-api:		    ## Remove previously built api Docker image
	@docker rmi ${IMAGE_FULL_NAME}-api:${IMAGE_VERSION}

build-cli: 			## Build cli production Docker image
	@$(call build_image_by_spec,production,${IMAGE_FULL_NAME}:${IMAGE_VERSION},SignalBooster/SignalBoosterCLI/Dockerfile)

build-api: 			## Build api production Docker image
	@$(call build_image_by_spec,production,${IMAGE_FULL_NAME}-api:${IMAGE_VERSION},SignalBooster/VendorApi/Dockerfile)

test: 			## Run tests against the image
	@container-structure-test test --image ${IMAGE_FULL_NAME}:${IMAGE_VERSION} --config tests.yaml

which-image:	## Image identifier helper
	@echo ${IMAGE_FULL_NAME}:${IMAGE_VERSION}

run:			## Run the proxy container
	@docker run --rm ${IMAGE_FULL_NAME}:${IMAGE_VERSION}

shell-cli:			## Shell into the proxy container
	@docker run -it --rm --entrypoint /bin/bash ${IMAGE_FULL_NAME}:${IMAGE_VERSION}

shell-api:			## Shell into the API container
	@docker run -it --rm --entrypoint /bin/bash ${IMAGE_FULL_NAME}-api:${IMAGE_VERSION}

pull:			## Pull image and mark as local
	docker pull ${IMAGE_FULL_NAME}:${IMAGE_VERSION}
	docker tag ${IMAGE_FULL_NAME}:${IMAGE_VERSION} ${IMAGE_FULL_NAME}:local

push:			## Push image to artifact registry
	@docker tag ${IMAGE_FULL_NAME}:local ${IMAGE_FULL_NAME}:${IMAGE_VERSION}
	@docker push ${IMAGE_FULL_NAME}:${IMAGE_VERSION}

release:		## Release Image to Registry
	@$(call build_image_by_spec,production,${IMAGE_FULL_NAME}:${RELEASE_TAG})