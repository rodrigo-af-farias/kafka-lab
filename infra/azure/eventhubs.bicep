/*
//para recriar o event hub na azure quando precisar:
az deployment group create `
  --resource-group kafka-lab-rg `
  --template-file .\infra\azure\eventhubs.bicep
  
//Depois da recriação, atualizar no Key Vault o valor de eventhubs-connection-string, a connection string será nova.
*/


param location string = 'centralus'
param namespaceName string = 'kafka-lab-eh-prd'
param eventHubName string = 'orders'

resource eventHubsNamespace 'Microsoft.EventHub/namespaces@2024-01-01' = {
  name: namespaceName
  location: location

  sku: {
    name: 'Standard'
    tier: 'Standard'
    capacity: 1
  }

  properties: {
    isAutoInflateEnabled: false
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: false
    zoneRedundant: true
  }
}

resource ordersEventHub 'Microsoft.EventHub/namespaces/eventhubs@2024-01-01' = {
  parent: eventHubsNamespace
  name: eventHubName

  properties: {
    partitionCount: 3
    messageRetentionInDays: 1

    retentionDescription: {
      cleanupPolicy: 'Delete'
      retentionTimeInHours: 24
    }
  }
}

resource appAuthorizationRule 'Microsoft.EventHub/namespaces/authorizationRules@2024-01-01' = {
  parent: eventHubsNamespace
  name: 'kafka-lab-app'

  properties: {
    rights: [
      'Listen'
      'Send'
    ]
  }
}

output namespaceName string = eventHubsNamespace.name
output eventHubName string = ordersEventHub.name