set -ev

bash build.sh BuildContainers

echo "$DOCKER_PASSWORD" | docker login -u "$DOCKER_USERNAME" --password-stdin
docker push deadsith/lightest-api
docker push deadsith/lightest-identity
