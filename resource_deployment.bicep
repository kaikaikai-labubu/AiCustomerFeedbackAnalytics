param projectPrefix string
param aiFoundryName string = projectPrefix
param aiProjectName string = '${aiFoundryName}-proj'
param llmModelDeploymentName string = '${projectPrefix}-llm-deploy'
param embeddingModelDeploymentName string = '${projectPrefix}-embedding-deploy'
param aiSearchName string = '${projectPrefix}-aisearch'
param location string = resourceGroup().location

/*
  An AI Foundry resources is a variant of a CognitiveServices/account resource type
*/ 
resource aiFoundry 'Microsoft.CognitiveServices/accounts@2026-09-01' = {
  name: aiFoundryName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  sku: {
    name: 'S0'
  }
  kind: 'AIServices'
  properties: {
    // required to work in AI Foundry
    allowProjectManagement: true

    // Defines developer API endpoint subdomain
    customSubDomainName: aiFoundryName

    disableLocalAuth: false
  }
}

/*
  Developer APIs are exposed via a project, which groups in- and outputs that relate to one use case, including files.
  Its advisable to create one project right away, so development teams can directly get started.
  Projects may be granted individual RBAC permissions and identities on top of what account provides.
*/
resource aiProject 'Microsoft.CognitiveServices/accounts/projects@2026-09-01' = {
  name: aiProjectName
  parent: aiFoundry
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {}
}

/*
  Deploying a model to the AI Foundry is done through a deployment resource.
  In this example, we are deploying the 'gpt-5-mini' model, 
  which is a variant of GPT-5 optimized for lower latency and 
  cost while still providing strong performance for many use cases.
*/
resource llmModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2026-09-01'= {
  parent: aiFoundry
  name: llmModelDeploymentName
  sku : {
    capacity: 10
    name: 'GlobalStandard'
  }
  properties: {
    model:{
      name: 'gpt-5-mini'
      format: 'OpenAI'
      version: '2025-08-07'
    }
  }
}

// https://ai.azure.com/catalog/models/text-embedding-3-small
resource embeddingModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2026-09-01'= {
  parent: aiFoundry
  name: embeddingModelDeploymentName
  dependsOn: [
    llmModelDeployment
  ]
  sku : {
    capacity: 1
    name: 'GlobalStandard'
  }
  properties: {
    model:{
      name: 'text-embedding-3-small' 
      format: 'OpenAI'
      version: '1'
    }
  }
}

resource searchService 'Microsoft.Search/searchServices@2025-09-01' = {
  name: aiSearchName
  location: location
  sku: {
    name: 'standard'
  }
  properties: {
    replicaCount: 1
    partitionCount: 1
    hostingMode: 'default'
    disableLocalAuth: true
  }
}

/*
  OUTPUTS
*/
output project_endpoint string = 'https://${aiFoundry.properties.customSubDomainName}.services.ai.azure.com/api/projects/${aiProject.name}'
output llm_model_deployment_name string = llmModelDeployment.name
output embedding_model_deployment_name string = embeddingModelDeployment.name
output AI_SEARCH_SERVICE_ENDPOINT string = 'https://${searchService.name}.search.windows.net'
output AI_SEARCH_SERVICE_DEPLOYMENT_NAME string = searchService.name