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
