#!/usr/bin/env bash

set -eo pipefail

usage() {
    echo "Usage: $0 [-a true] [-c cluster] [-f file] [-k true] [-v true]"
    echo ""
    echo "Flags:"
    echo "-c cluster-name       Get specific cluster"
    echo "-a                    Get all clusters"
    echo "-k                    Convert kubeconfig to kubelogin (requires kubelogin is installed)"
    echo "-f file-name          Output to specific file"
    echo "-v                    Verbose output"
    echo ""
    echo "Examples:"
    echo "Get specific cluster: $0 -c core-dev-west-1"
    echo "Get all clusters: $0 -a"
    echo ""
    echo "Requires a valid token for Vault, so make sure you've logged in with:"
    echo "vault login -method=oidc"
    exit 0;
}

# use this for getopts when sourcing
OPTIND=1
# check for config input flag
while getopts ":a c: f: k v h" o; do
  case "${o}" in
    a)
        ALL_CLUSTERS=true
        ;;
    c)
        CLUSTER=${OPTARG}
        ;;
    f)
        KUBECONFIG_FILE=${OPTARG}
        ;;
    k)
        KUBELOGIN=true
        ;;
    v)
        VERBOSE=true
        ;;
    *)
        usage
        ;;
  esac
done

# Set default environment
if [ -z "${CLUSTER}" ] && [ -z "${ALL_CLUSTERS}" ]; then
    usage
    echo "You need to either specify a specific cluster or all clusters"
    exit 1
fi

if [ ! -z "${VERBOSE}" ]; then
  set -x
fi

if [ -z "${KUBECONFIG_FILE}" ]; then
  KUBECONFIG_FILE=kubeconfig-$(date "+%Y-%m-%d")
fi

if [ ! -z "${KUBELOGIN}" ]; then
  if ! command -v kubelogin &> /dev/null ; then
    echo "Install kubelogin first: https://github.com/Azure/kubelogin"
    exit 1
  fi
fi

VAULT_ADDR=https://vault.maersk-digital.net
export VAULT_ADDR

# If ~/.vault-token file exists, source VAULT_TOKEN with the content
if [ -f ~/.vault-token ]; then
    VAULT_TOKEN=$(cat ~/.vault-token)
    export VAULT_TOKEN
    if ! vault token lookup > /dev/null 2>&1 ; then
      echo "Vault token expired or for the wrong instance."
      echo "Run 'vault login --method=oidc'";
      exit 1
    fi
else
    echo "Error, expecting file ~/.vault-token"
    echo "Run vault login --method=oidc"
    exit 1
fi

get_cluster() {
  # get rid of pesky slashes
  CLUSTER=$(echo $1|sed s#/##)

  KUBECONFIG=$(vault kv get -field=kubeconfig shared/all/kubernetes/${CLUSTER})

  echo "$KUBECONFIG" >> ./$KUBECONFIG_FILE
}

if [ ! -z "${ALL_CLUSTERS}" ]; then
  # Get all clusters and loop through them
  vault kv list -format=json shared/all/kubernetes/ | jq -c -r '.[]' | while read c; do
    get_cluster $c
  done
else
  get_cluster ${CLUSTER}
fi

if [ ! -z "${KUBELOGIN}" ]; then
  KUBECONFIG=$KUBECONFIG_FILE kubelogin convert-kubeconfig
fi

echo "A local file $KUBECONFIG_FILE has been created! To start using it you have 2 options:"
echo ""
echo "Option 1:"
echo "Use it as environment variable to kubectl"
echo "KUBECONFIG=$KUBECONFIG_FILE kubectl <command>"
echo ""
echo "Option 2:"
echo "Merge $KUBECONFIG_FILE with current ~/.kube/config file"
echo "KUBECONFIG=$KUBECONFIG_FILE:~/.kube/config kubectl config view --flatten > ~/.kube/config.NEW"
echo ""
echo "Now verify that everything is as expected in the NEW file and then you can replace the old:"
echo "mv ~/.kube/config.NEW ~/.kube/config"
