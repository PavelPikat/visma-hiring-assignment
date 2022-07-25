# Hosting on Azure Cloud from Zero to Hero

#### Prerequisites
- [Az CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli)
- [Terraform](https://www.terraform.io/downloads)
- Azure Cloud account (you can also [try it for free](https://azure.microsoft.com/en-us/free/))

#### 1. Create Service Principal for Azure DevOps pipelines

Login to your Azure account

`az login`

Create Service Principal with Contributor role in a subscription of your choice

`az ad sp create-for-rbac --display-name "e-conomic assignment" --role="Contributor" --scopes="/subscriptions/SUBSCRIPTION_ID"`

In Azure DevOps UI, create new Service connection called `e-conomic assignment` using `appId` and `password` from the newly created SP

#### 2. Create Storage Account for Terraform state
Create resource group
`az group create -l westeurope -n terraform-state-rg`

Create Azure Storage Account (since SA names are globally unique, change the index number)

`az storage account create -n sreterraformstate001 -g terraform-state-rg -l westeurope --sku Standard_LRS`

Create blob container

`az storage container create -n tfstate --account-name sreterraformstate001 --resource-group terraform-state-rg`


