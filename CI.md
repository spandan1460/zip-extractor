### Pipeline Components
Pipelines are processes for moving software from development to production in an automated fashion -hands off the wheel, because humans inject latency in the process & create mistakes when automation is not applied with rigor as steps/tests and validation is forgotten about or neglected. 

Pipeline gates in place to prevent either environmental issues, or implementation issues from making it  out past a developers laptop without first scrutinizing the implementation at various levels.

The following sections are considered pipeline gates - there's an example in [./.github/actions/build-test-scan.yml](./.github/workflows/build-test-scan.yml) that shows the entire lifecycle of build, test, dockerize, sonarqube, publish artifact to Harbor.

### Pipeline Gates

Static code analysis

* [SonarQube](https://sonarcloud.io/) - Analyzes for code smells, verifies that code coverage is maintained. Example see: [.github/workflows/sonarqube.yml](.github/workflows/build-test-scan.yml)

* [CodeQL](https://codeql.github.com/) - To identify vulnerabilities and errors in code, CodeQL analysis is used. This can be configured using [codeql actions](https://github.com/github/codeql-action). Vulnerability testing (security) scans are currently run through github actions. Configure these actions in the workflows of your repository. Example see: [this action](.github/workflows/build-test-scan.yml)

* [BlackDuck](https://www.synopsys.com/software-integrity/security-testing/software-composition-analysis.html) - Provides open source detection and vulnerability analysis. 

Local Testing { xUnit, nUnit, etc.  } - which also runs in CI (github actions)

* **Note:** None of the testing approaches listed below require external service integration, micro-services should be capable of verifying the functionality deployed against an external contract without the invocation of the external service (this especially applies to integration testing).

* **Unit** - testing method by which individual units of source code—sets of one or more computer program modules together with associated control data, usage procedures, and operating procedures—are tested to determine whether they are fit for use.

* **Functional** - tests a particular functional area - in aggregate whether a collection of classes are suitable for the particular task / purpose. Greater than method (unit) level testing, functional testing seeks to determine the application feature works as intended.

* **Integration** - tests from the outside in. Development of a micro-service would standup the micro-service locally using mock persistence storage responses and hit the API end points validating the full fledged contract as bespoke tests.

*PR Naming convention*

* **Linting of PR title** - verifies that the title of the PR provides the following levels of information

  * feature/bugfix/chore/docs/fix/performance/test, etc.

  * Jira Ticket

  * A description of the reason for the PR
 
* Install the following: [PR Lint Action - GitHub Marketplace](https://github.com/marketplace/actions/pr-lint-action)
  ** alternatively you can use [this action](https://github.com/Maersk-Global/fbm-template/blob/main/.github/workflows/pull-request-enforce-naming.yml) to make the verification

Examples:

```bugfix(concurrency): Resolves a bugfix in the Help system - JIRA-9999```

*Branch Naming Convention*

You should setup a gate to verify branch naming conventions using [this action](https://github.com/Maersk-Global/fbm-template/blob/main/.github/workflows/branch-enforce-naming.yml)


__Remote Testing of the deployed service__

* (SANDBOX, STAGING, PROD) **E2E & Smoke Testing** - blackbox testing - hitting the service from an external client, assuming a specific response with a given input to verify the system as a whole is functional. Useful for verifying the code-as-deployed functions once released to a particular environment. This allows the pipeline to verify the code-as-deployed does not need to be rolled back.

* **(STAGING) Performance Testing** - verifies the service is capable of achieving particular a latency per request, that at a given RPS level the response time of the service is maintained. Helps verify overtime that new functionality does not lower the expected RPS rate or latency expectations of the service. 

Each of these gates plays a role in verifying the software as written performs the function it intended in away that is free of defects and adhere’s to standards set by the engineering team or organization. The idea with these tools is they’re continuously applied to the repo once changes are made, be it locally (see local testing above) or during the build / pipeline process.

## Continuous Integration (CI) - Github Actions Pipeline Phases (Happy Path)

1. CI system builds artifact using the exact same assembly instructions used in development inclusive of testing as described in the [Local Development Lifecycle](./LOCAL_DEV.md):

  * BlackDuck
  * unit tests
  * functional tests
  * integration tests
  * Sonarqube

2. **Gate:** The testing performed in (Local Development Lifecycle) is run
3. An artifact is generated, and versioning strategy applies a version number to the docker container/immutable artifact
4. Revision control tagging occurs on the main branch to indicating the current revision of the generated
5. The generated artifact is published to a repository that can be pulled down for deployment
6. The artifact is deployed to development
7. **Gate:** Smoke testing executes against the deployed development deploy
8. Upon successful smoke testing, the deployment occurs in sandbox (perf environment)
9. **Gate:** Automation testing for performance - checking latency, and other API level metrics to ensure the build does not degrade the performance of the service prior to production deployment.

Depending on the maturity of the code base, if the gates succeeded you can automate the deployment to production.  But that depends heavily upon the maturity of the code/tests/monitoring & roll back procedures in place to determine if the implementation functions as expected.  If there is no automation to detect failures, or no / minimal unit/functional/integration tests automation should stop after development until the required tests exist to promote the build beyond development.

The subsequent sections present a template that developers can utilize to integrate the mentioned components into their repositories and follow standards that enhance the quality of our code, our deployment processes, and, in turn, the features we progressively introduce.

### Testing

Let's begin with the premise that you already have certain tests in place, including unit, functional, and integration tests, with their outcomes being reported as NUnit results, or alternatively, using Jest tests for JavaScript or pytests for Python. However, if you currently lack any tests, there's no need for concern. Starting from your initial baseline is a valuable initial step toward progress, and you can introduce SonarQube even without tests in place, as it can detect code issues such as code smells (linting issues also recognized by SpotBugs).

### Maersk Github Action Templates

There's a variety of github action plugins available in the Maersk library available here [github-actions-common](https://github.com/Maersk-Global/github-actions-commons)
There's also a FbM common actions repository here [fbm-actions](https://github.com/Maersk-Global/fbm-actions)

## Versioning

Once we’ve built a binary artifact and all of the gates have passed, we need to version everything using the same version.  Nuget refers to this as SemVer.

**Major.Minor.Patch**

1. MAJOR version when you make incompatible API changes
2. MINOR version when you add functionality in a backward compatible manner
3. PATCH version when you make backward compatible bug fixes

Additional labels for pre-release and build metadata are available as extensions to the MAJOR.MINOR.PATCH format.

Semantic versioning is the process of applying that verion using a strategy based on the type of change being performed.    

You can read more about semver here: [Semantic Versioning 2.0.0](https://semver.org/) and [C# Versioning Guide](https://learn.microsoft.com/en-us/dotnet/csharp/versioning)

### Github Applying Semantic Version

You can incorporate the semantic versioning by applying the following example: 

```
on: 
  push:
    branches:
      - master

jobs:
  release-on-push:
    runs-on: windows-latest
    env:
      GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    steps:
      - uses: rymndhng/release-on-push-action@master
        with:
          bump_version_scheme: minor
```

#### Can I skip creation of a release?

There are several approaches:

1. Put [norelease] in the commit title.
2. If the commit has an attached PR, add the label norelease to the PR.
3. Set the action's bump_version_scheme to norelease to disable this behavior by default

#### How do I change the bump version scheme using Pull Requests?

If the PR has the label release:major, release:minor, or release:patch, this will override bump_version_scheme.

This repository's pull requests are an example of this in action. For example, [#19](https://github.com/rymndhng/release-on-push-action/pull/19).

Only one of these labels should be present on a PR. If there are multiple, the behavior is undefined.

Further documentation on the semantic versioning tool is available here: [Tag/Release on Push Action - GitHub Marketplace](https://github.com/marketplace/actions/tag-release-on-push-action)

### Places to Apply Semantic Versioning

There’s multiple places to apply the semantic version:

  * Github pointer to current version (codified version of the current latest version of main/master).
  * Github repository tag - when the assembly is created, we want to tag the repository (main/master) with the semantic 
    version created.
  * The binary artifact we’re publishing
    * DLL assembly
    * A docker container creating the DLL
    * The helm chart referencing the dockerized container

