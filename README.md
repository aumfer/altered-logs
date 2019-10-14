# altered-logs

This fully-functional MVP shows a microservice-based logging platform in a minimal functional style.

The solution contains lambdas to capture and store in ElasticSearch
- CloudWatch Logs
- ALB and ELB logs (from S3)
- CodeBuild projects (from AWS Events)
- Autoscaling and ECS deployments (from AWS Events)
- Terraform state changes (from S3)

as well as a REST API with 2 redundant deployments of the same codebase:

One version hosted as a lambda behind an ALB

One version is:
- served via ASP.NET MVC Core
- deployed as a Docker image
- hosted in an ECS Cluster

Also included is a full CI/CD pipeline:
- executed via CodeBuild
- runs unit tests via dotnet test / vstest
- deploys via Terraform

```
echo '{"time":"3/12/2019 21:09:25.490", "log":{"name":"Hello, World!"}}' | dotnet run --project src/Altered.Logs -- altered.logs.logtoelasticsearch
```
