# Introduction
> Pavel's solution for Visma's recruitment assignment for Site Reliability Engineer

### Implemented
- .NET Core microservice `assignment-service` that calls into `dummy-pdf-or-png` service and return PDF or PNG back to the caller
- Handling transient HTTP errors with Polly.NET
- Validating PDF file for corruption and retrying
- Prometheus metrics
- Health checks & probes
- Tests with xUnit
- docker-compose for local dev/test
- Kubernetes all-in-one manifest for local dev/test on Docker Desktop
- AKS all-in-one manifest for deploying to Azure
- Azure DevOps CI/CD pipeline
- Terraform configuration for AKS, ACR infrastructure on Azure
- Documentation with VuePress

### Extras
- Docker-compose with Grafana, Loki and Prometheus for viewing logs and metrics

# Running locally
### docker-compose
From `/sre/.docker`, run `docker-compose up`

docker-compose will build and run everything for you

- Browse [http://localhost:3020/1](http://localhost:3020/1)

- To view Prometheus metrics and logs in Loki, 
navigate to [http://localhost:3050/](http://localhost:3050/) to open Grafana

### Docker Desktop with Kubernetes

1. Build both `dummy-pdf-or-png` and `assignment-service` containers locally so they are available

From `/sre/dummy-pdf-or-png`, run `docker build -t dummy-pdf-or-png .`

From `/sre/assignment-service`, run `docker build -t assignment-service .`

2. Deploy to local Kubernetes
  From `/sre`, run `kubectl apply -f ./.k8/docker-desktop-all-in-one.yaml`

3. Browse [http://localhost:3020/1](http://localhost:3020/1)
