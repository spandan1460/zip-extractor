# Maersk FbM Kubernetes Cluster

Customer data file is [here](https://github.com/Maersk-Global/platform-data/blob/master/data/customers/oct.json)

## AD Groups / Access Control
The customer data file defines the AZure AD groups that map to administrative rights within the cluster, and the FluxCD repositories and namespaces.

Admiral can be accessed here [Admiral](https://admiralv2.hosting.maersk.com/products/oct-ecl)

## Admiral OCT-ECL Account

The following link is our OCT-ECL product in the Admiral application, once created we should not need to touch this, beyond administrating access.

[Admiral OCT-ECL](https://admiralv2.hosting.maersk.com/products/oct-ecl)

## Getting Started Documentation

* [Maersk Perpetual Getting Started Guide](https://perpetual.maersk-digital.net/docs/getting-started/access-your-cluster)

## KubeConfig Access

Use the Admiral AD for kubeconfig access

Methods:
* [Azure CLI](https://perpetual.maersk-digital.net/docs/getting-started/access-your-cluster#method-2-download-cluster-configuration-via-azure-cli)
* [Download Config](https://perpetual.maersk-digital.net/docs/getting-started/access-your-cluster#method-2-download-cluster-configuration-via-azure-cli)

Recommendation is to use the first method above

## Firewall Rules / Egress

The following is the egress IP list
https://perpetual.maersk-digital.net/iplist

## Retina

The empv3-tenant-data module needs to configured in Retina git repo

## Perpetual (AKS) Tickets / Support

In order to raise questions/support tickets use the following link.

https://github.com/Maersk-Global/platform-support/discussions

## Vault

Access to the user interface for Vault is here: https://vault.maersk-digital.net/

You need to be members of the OCT Developers Infrastructure Group which is in admiral as Team: OCT Infrastructure and Azure standard group cd5718bb-7816-4b6f-aaf0-07d1e343ebc7

Access to vault/aks/k8s can be granted by Matt Greenwood, Luis Ramos, Miguel Torres

## Cluster Instances in NAM

### DEV

Region: us-east-2
Cluster: oct-dev-app-eu2-01

### PROD

Region: us-east-2
Cluster: oct-prod-app-eu2-01

Region: us-west
Cluster: TO-BE-CREATED

## Cluster Instances in EU

Non presently TBD


# Tools Installation

(Instructions from Perpetual)[https://perpetual.maersk-digital.net/docs/getting-started/access-your-cluster/)

## OS/X Mac

### Azure CLI

```bash
brew update && brew install azure-cli
```

### Kubernetes CLI

Apple Silica M1/M2:
```bash
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/darwin/arm64/kubectl"
```

Intel
```bash
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/darwin/amd64/kubectl"
```

### Kubelogin

```
az login

```

### Vault CLI


Install:

```
brew tap hashicorp/tap
brew install hashicorp/tap/vault
```

## Windows Subsystem for Linux

Download the following script to install all pre-requisites:
[WSL_bash.sh](WSL_bash.sh)

Then execute: 
```bash
chmod u+x WSL_bash.sh
```
Then run the script: ./WSL_bash.sh


### Setup script perpetual-platform-kubeconfig.sh

The script resides here, download to a local file:

[perpetual-platform-kubeconfig.sh](https://github.com/Maersk-Global/platform-support/blob/master/scripts/perpetual-platform-kubeconfig.sh)

Then execute: 

```bash
chmod u+x perpetual-platform-kubeconfig.sh
```

### Local execution - kube-config generation/testing

```bash
az login
# Browser opens login to Azure
export VAULT_ADDR=https://vault.maersk-digital.net
vault login -method=oidc

# Check the dev instance
./perpetual-platform-kubeconfig.sh -c CLUSTER_NAME  # DEV: oct-dev-app-eu2-01 | PROD: oct-prod-app-eu2-01
ls kubeconfig*

# now move the contents of the kubeconfig into ~/.kube/config
# when you run this command you will need to login here with a code as presented to you in the CLI

kubectl get ns
```

__Login:__
Open browser to [https://microsoft.com/](https://microsoft.com/devicelogin)
type in the code provided, click continue, close the browser window once it informs you that you can

```bash
kubectl get ns
NAME                    STATUS   AGE
calico-system           Active   4d23h
capsule-system          Active   4d4h
cloudability            Active   4d4h
datadog                 Active   4d3h
default                 Active   4d23h
emp-kafka               Active   4d
external-dns            Active   4d4h
external-secrets        Active   4d4h
flux-system             Active   4d23h
gatekeeper-alerting     Active   4d3h
gatekeeper-system       Active   4d4h
goldilocks              Active   4d3h
infra-kyverno           Active   26h
ingress-nginx           Active   4d4h
keda                    Active   4d4h
kube-downscaler         Active   4d4h
kube-janitor            Active   4d4h
kube-node-lease         Active   4d23h
kube-public             Active   4d23h
kube-system             Active   4d23h
kured                   Active   4d4h
matts-namespace         Active   43m
oauth2-proxy            Active   4d4h
platform-jobs           Active   4d3h
platform-monitoring     Active   4d3h
serviceaccounts         Active   4d3h
tigera-operator         Active   4d23h
twistlock               Active   4d4h
vault-secrets-webhook   Active   4d4h
vpa                     Active   4d4h
```
 
# Egress IPs

Egress IPs are listed here: https://perpetual.maersk-digital.net/iplist

# Retina

The empv3-tenant-data module needs to configured in Retina git repo

# FluxCD

## k8s Namespaces

Namespaces cannot be created via kubectl, but instead via the product file reference above [oct.json](https://github.com/Maersk-Global/platform-data/blob/master/data/customers/oct.json)
Namespaces must be destroyed via the Perpetual team - they nuke things - so when you ask them to destroy the namespace, they don't do so gracefully.  

## Service Integration

FluxCD is the integration tool, each service that onboards needs at a mimimum to make these changes:

* Modify the [oct.json](https://github.com/Maersk-Global/platform-data/blob/master/data/customers/oct.json) to include the repository as a block, similar to the following:
```yaml
"repos": [
    {
      "branch": "main",
      "suspended": false,
      "enabled": true,
      "interval": "1h",
      "validation": "client",
      "prune": true,
      "path": "./flux-cd/flux-module-fbm",
      "source_ref": "flux-module-fbm",
      "url": "https://github.com/Maersk-Global/fbm-template",
      "namespace_name": "fbm-template",
      "set_default_var": true
    },
    {
    TODO add your repo here patterned off of fbm-template above
    } 
  ]
```

This change can be auto-merged once a PR is created.  The customer json file is checked to ensure the JSON is appropriate (valid) and does a variety of other checks - it will then auto-merge your PR once the PR passes the checks.

The one call-out here is, when your PR is merged, its possible another PR from another business unit blocks your changes from deploying as a result of an introduced problem.  For that you will need to reach out to the 

* Create a flux-cd deployment path in your repo as pointed to via the oct.json file reference (similar to the above oct.json path example).

See [./flux-cd/flux-module-fbm](./flux-cd/flux-module-fbm) as an example.


# FluxCD Examples

* [fluxcd.io](https://fluxcd.io)
* https://github.com/Maersk-Global/maury
* Platform-data example for FluxCD [thorin](https://github.com/Maersk-Global/platform-data/blob/master/data/customers/thorin.json)

