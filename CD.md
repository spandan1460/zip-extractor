## Containerization

- Setup dockerfile - example Dockerfile for containerizing .NET 8 application and standing it up:
[Dockerfile](./Dockerfile)
- Make container runnable - example entrypoint.sh script for running the .NET 7 application:
[entrypoint.sh](./entrypoint.sh)
- Setup [publish](https://github.com/Maersk-Global/fbm-template/blob/main/.github/workflows/build-test-scan.yml) action - This workflow will perform build, test, scan, dockerize and publish.
- Setup [deploy](https://github.com/Maersk-Global/fbm-template/blob/main/.github/workflows/deploy.yml) action - This workflow will push the image and deploy.

### Azure Docker Registry Deployment

Maersk uses FluxCD for deployment - see documentation [here](https://perpetual.maersk-digital.net/docs/kubernetes/gitops/)

Confluence Version is available here:
https://maersk-tools.atlassian.net/wiki/spaces/ECLPLAT/pages/182262761387/CI+CD+Pipelines+Best+Practices+Templates#Dockerizing-C#-.NET-Applications
