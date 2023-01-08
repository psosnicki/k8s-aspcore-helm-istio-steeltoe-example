#start minikube cluster
minikube start
#install istio demo profile
istioctl install --set profile=demo -y
#enable auto injecting sidecar into pods in namespace default
kubectl label namespace default istio-injection=enabled --overwrite
#install kiali, prometheus, jaeger 
kubectl apply -f https://raw.githubusercontent.com/istio/istio/release-1.16/samples/addons/kiali.yaml
kubectl apply -f https://raw.githubusercontent.com/istio/istio/release-1.16/samples/addons/prometheus.yaml
kubectl apply -f https://raw.githubusercontent.com/istio/istio/release-1.16/samples/addons/jaeger.yaml

#provide terminal to docker engine inside minikube
minikube -p minikube docker-env --shell powershell | Invoke-Expression

#build aspnet-core-web-api docker image inside minikube
docker build -t aspnet-core-web-api:latest .\src\WebApi

#update helm dependencies
helm dependency update .\k8s

#install helm package
helm install web-api .\k8s --wait

Write-Host "Visit http://127.0.0.1/test"
Write-Host "Refresh page to see that requests are forwarded to different pods (round robin)"

#enable access to application running within minikube (istio-ingressgateway)
minikube tunnel
