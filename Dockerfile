FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY src/Altered.Logs/bin/Release/netcoreapp2.1/publish ./

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
ARG TF_VAR_repo_name
ARG TF_VAR_source_rev
ENV TF_VAR_repo_name=$TF_VAR_repo_name
ENV TF_VAR_source_rev=$TF_VAR_source_rev
WORKDIR /app
COPY --from=build-env /app .
ENV ASPNETCORE_URLS http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Altered.Logs.dll"]
