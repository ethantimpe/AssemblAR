[![AssemblAR](docs/img/logo.png)](https://github.com/ethantimpe/AssemblAR)

Update image:
```
docker build . -t ghcr.io/ethantimpe/assemblar-server:latest

docker push ghcr.io/ethantimpe/assemblar-server:latest
```

Deployment needs access to GitHub Container Registry
```
kubectl create secret docker-registry ghcr-secret \
  --docker-server=ghcr.io \
  --docker-username=${GITHUB_USERNAME} \
  --docker-password=${GITHUB_TOKEN}
```

Stand up:
```
helm upgrade --install assemblar-1753648313 helm/ -f helm/values.yaml -n assemblar
```

Expose dashboard:
```
kubectl -n kubernetes-dashboard port-forward svc/kubernetes-dashboard-kong-proxy 8443:443
```