# altered-logs

This fully-functional MVP shows a microservice-based logging platform in a minimal functional style.

The solution contains lambdas to capture
- ALB and ELB logs (from S3)
- CodeBuild projects (from AWS Events)
- Autoscaling and ECS deployments (from AWS Events)
- Terraform state changes (from S3)

as well as a REST API (that crudely proxies Kibana)

```
echo '{"@t":"3/12/2019 21:09:25.490", "log":{"name":"Hello, World!"}}' | dotnet run --project src/Altered.Logs -- altered.logs.logtoelasticsearch
```
