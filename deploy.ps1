$resourceGroup = "rg-ai-feedback"
$location = "australiaeast"

az group create `
--name $resourceGroup `
--location $location

az deployment group create `
--resource-group $resourceGroup `
--template-file resource_deployment.bicep `
--parameters projectPrefix=ai-customer-feedback

# Get your deployment outputs
az deployment group show `
--resource-group aiagent-course-rg `
--name resource_deployment `
--query properties.outputs