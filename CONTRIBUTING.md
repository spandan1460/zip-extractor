# Introduction

This document provides a guide to the collaboration process for a project hosted on GitHub, using the GitHub Flow workflow. This document also outlines the release process, which ensures a structured approach to deploying new features and bug fixes.

[Here](https://my.maerskgroup.com/:v:/r/personal/rafael_ferreira_maersk_com/Documents/Recordings/Monthly%20OCT%20tech%20session-20230816_160234-Meeting%20Recording.mp4?csf=1&web=1&e=m6eSBl&nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJTdHJlYW1XZWJBcHAiLCJyZWZlcnJhbFZpZXciOiJTaGFyZURpYWxvZy1MaW5rIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXcifX0%3D) you can watch the OCT demo video about git which also talks about collaboration workflow.

## Code Quality

- Make sure to integrate automated testing (e.g., unit tests, integration tests) into your CI/CD pipeline to ensure code quality.
- SonarQube should be enabled on the branch for monitoring test metrics.
- SonarWay should be enabled to ensure refactored and new code hits an 80% test coverage.
- Refer to [FBM-Template](https://github.com/Maersk-Global/fbm-template/tree/main#pipeline-gates) for a more in-depth look into this.

## Collaboration Workflow

### Main Branch as the Source of Truth

The main branch is considered the only source of the project. It should always reflect the latest production-ready code.

### Feature Branches

- For every new development task or user story, create a dedicated feature branch. Feature branches allow to work on isolated changes without affecting the main branch.

- Feature branch names must follow this naming convention:

  - `prefix/JIRA-TICKET/small-description` for example:

    ```
    bugfix/TIGER-33/fix-request-bug
    ```

  - available prefixes are: feature, bugfix, hotfix and breaking

- The naming convention should be enforced by a GitHub action such as on [FBM-Template](https://github.com/Maersk-Global/fbm-template/blob/main/.github/workflows/branch-enforce-naming.yml).

### Pull Requests (PRs)

- When development is complete (which should include unit and integration tests) on a feature branch, initiate a Pull Request (PR) to propose merging new changes into the main branch.

- The PR title should also follow this naming convention:

  - `prefix: Description [JIRA-TICKET]` for example:

    ```
    bugfix: Fix bug on HTTP request [TIGER-33]
    ```

  - available prefixes are: feature, bugfix, hotfix and breaking

- The naming convention should be enforced by a GitHub action such as on [FBM-Template](https://github.com/Maersk-Global/fbm-template/blob/main/.github/workflows/pull-request-enforce-naming.yml).

- Your repository should have the PR template from [FBM-Template](https://github.com/Maersk-Global/fbm-template/blob/main/.github/pull_request_template.md) configured ([Creating a pull request template for your repository](https://docs.github.com/en/communities/using-templates-to-encourage-useful-issues-and-pull-requests/creating-a-pull-request-template-for-your-repository)). Provide a detailed description of the changes made, including any relevant context, and reference related issues or user stories.

- Assign at least two team members as reviewers for the PR.

### Testing in Sandbox Environment

- Create a test plan that outlines the specific test cases to be executed.
- Before proposing PR for code review, deploy the feature branch to sandbox environment for comprehensive testing.
- Perform manual testing, including exploratory testing, to verify the changes.

### Building (CI)

- Set up a continuous integration (CI) pipeline to automatically run checks and tests on every Pull Request.
- Ensure that all automated checks and tests pass successfully. This includes linting, code style checks, and any project-specific criteria.

## Code Reviews

Here is a guideline on how someone should comment on a code review [Conventional Comments](https://conventionalcomments.org/):

- Be open to constructive criticism and understand that initial PR rejections are common, aimed at maintaining codebase quality.
- Defend your decisions when necessary, but always keep discussions civil and respectful, despite possible frustrations.
- The feature branch should be merged into main only after it has received at least two approved code reviews from team members.

## Merging and Branch Deletion

- Once the Pull Request is approved and all checks/tests pass, merge the feature branch into the main branch.

- Preferably, configure GitHub to automatically delete the feature branch after successful merge to keep the repository clean and organized.

## Release Process

### Tagging and Versioning

You should follow the guidelines from [FBM-Template](https://github.com/Maersk-Global/fbm-template/tree/main#versioning) for tagging and versioning.

### Deployment

- Deploy the new tagged release using the deployment GitHub action ([FBM-Template example](https://github.com/Maersk-Global/fbm-template/blob/main/.github/workflows/deploy.yml)).

- Ensure that you always deploy a version that is tagged and never a branch that is not tagged.

## Useful Documentation

For further information and detailed instructions, refer to the following documentation:

- [GitHub Flow](https://guides.github.com/introduction/flow/)
- [Creating a Pull Request](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request)
- [Configuring Branch Protection Rules](https://docs.github.com/en/github/administering-a-repository/defining-the-mergeability-of-pull-requests/about-protected-branches)
- [Semantic Versioning (SemVer)](https://semver.org/)
- [Creating Releases on GitHub](https://docs.github.com/en/repositories/releasing-projects-on-github/managing-releases-in-a-repository)
