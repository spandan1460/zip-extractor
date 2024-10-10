#!/usr/bin/env bash
#Make sure the clock is set right
sudo hwclock --hctosys
#Download vault
wget -O- https://apt.releases.hashicorp.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg
echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/hashicorp.list
sudo apt update && sudo apt install vault
#Make sure we have kubectl and kubelogin
cd /usr/bin/
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
sudo az aks install-cli
sudo kubectl version --client
sudo kubelogin --version
#Install jq package
sudo apt -y install jq
#Add environment variables to make vault work and browser open, if you have chrome
cd ~
echo "" >> .bashrc
echo "export VAULT_ADDR=https://vault.maersk-digital.net" >> .bashrc
echo "export BROWSER='/mnt/c/Program Files (x86)/Google/Chrome/Application/chrome.exe'" >> .bashrc
source .bashrc
export VAULT_ADDR=https://vault.maersk-digital.net
vault login -method=oidc
