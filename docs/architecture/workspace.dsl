workspace "FEATv1" "Find Education and Training - Hosting internal AI Search" {

    model {
        properties {
            "structurizr.groupSeparator" "/"
        }

        group "DfE" {

            findASystem = softwareSystem "Find Education and Training" {
                description "Serverless container platform"
                tags "Microsoft Azure - Container Apps Environments"
            
                webapp = container "Website" {
                    description "Proof of concept site to demonstrate a front-end for finding information"
                    technology "dotnet 9.0"
                    tags "Microsoft Azure - Website Staging"
                }
                api = container "API" {
                    description "FEAT API that interrogates Azure AI Search"
                    technology "dotnet 9.0"
                    tags "Microsoft Azure - API Management Services"
                    webapp -> this
                }
                ingestion = container "Ingestion Job" {
                    technology "dotnet 9.0"
                    tags "Microsoft Azure - Data Shares"
                }
                
            }
        }
        
        
        group "Azure AI" {
            ai = softwareSystem "Microsoft AI Services" {
                tags "Microsoft Azure - Cognitive Services"
                aiSearch = container "Microsoft AI Search" {
                    technology "Microsoft AI"
                    tags "Microsoft Azure - Cognitive Search"
                    description "Vector search database"
                    api -> this "Queries" "HTTPS"
                    ingestion -> this "Writes to" "HTTPS"
                }
                
                openAI = container "OpenAI LLM" {
                    technology "Microsoft AI"
                    tags "Microsoft Azure - Azure OpenAI"
                    description "Embeddings generator"
                    api -> this "Queries" "HTTPS"
                    ingestion -> this "Queries" "HTTPS"
                }
            }
        }
        
        group "Find an Apprenticeship" {
            faa = softwareSystem "Find an Apprenticeship" {
                tags "Microsoft Azure - API Management Services"
                
                faaApi = container "Display Advert API" {
                    tags "Microsoft Azure - API Management Services"
                    description "Display Advert API"
                    ingestion -> this "Queries" "HTTPS"
                }
            }
            
        }
        
        service = deploymentEnvironment "Service" {

            internet = deploymentNode "Public Internet" {
                tags "Microsoft Azure - Entra Internet Access"
                gateway = infrastructureNode "Shared DfE WAF" {
                    technology "Azure Front Door and Firewalls"
                    description "Automatically distributes and secures incoming application traffic"
                    tags "Microsoft Azure - Firewalls"
                }
                
                
            }
                
            deploymentNode "Microsoft Azure" {
                tags "Microsoft Azure - Azure A"
                    
                deploymentNode "UK South"  {
                    tags "Microsoft Azure - Region Management"
                    
                    bastion = infrastructureNode "Bastion" {
                        description "Remote SSH for connecting to the virtual network"
                        tags "Microsoft Azure - Bastions"
                    }
                    
                    vnet = deploymentNode "FEAT Vnet" {
                        tags "Microsoft Azure - Virtual Networks"
                        
                        bastion -> this "Allows secured access to"
                        
                        
                        aiSearchInstance = containerInstance aiSearch {
                        
                        }
                        
                        
                        cache = infrastructureNode "Distributed Cache" {
                            description "Azure Managed Redis"
                            tags "Microsoft Azure - Cache Redis"
                        }
                        
                         
                        staging = infrastructureNode "Azure Blob Storage" {
                            description "Staging location for CSV file imports and data storage"
                            tags "Microsoft Azure - Storage Accounts"
                        }
                        
                        
                        db = infrastructureNode "Relational DB" {
                            description "Data store for information to be surfaced to users that isn't required to be searchable"
                            tags "Microsoft Azure - SQL Server"
                            cache -> this "Backing store"
                        }
                        
                        
                        webContainer = deploymentNode "Auto-Scaling ASP" {
                            tags "Microsoft Azure - App Service Plans"
                            
                            webAppService = deploymentNode "Web - App Service" {
                                tags "Microsoft Azure - App Services"
                                webApplicationInstance = containerInstance webapp {
                                    gateway -> this "Fowards requests to" "HTTPS"
                                }
                            }
                            
                            apiAppService = deploymentNode "API - App Service" {
                                tags "Microsoft Azure - App Services"
                                apiInstance = containerInstance api {
                                    this -> cache "Saves semantic results and geolocation lookups"
                                    cache -> this "Fetches semantic results and geolocation lookups"
                                    db -> this "Fetches additional information for display"
                                }
                                
                                gateway -> this "Provides secured access"
                            }
                            
                            
                        }
                        
                        igestionContainer = deploymentNode "Container Environment" {
                            tags "Microsoft Azure - Container Apps Environments"
                            
                            ingestionAppService = deploymentNode "Auto-scaling Container" {
                                tags "Microsoft Azure - Worker Container App"
                                ingestionInstance = containerInstance ingestion {
                                    this -> cache "Saves semantic results and geolocation lookups"
                                    cache -> this "Fetches semantic results and geolocation lookups"
                                    this -> db "Saves additional information for display"
                                    staging -> this "Loads data from"
                                    
                                }
                            }
                            
                        }
                        
                    }
                    
                }

                deploymentNode "West Europe" {
                    tags "Microsoft Azure - Region Management"
                    
                    dfeCOE = deploymentNode "DfE AI CoE" {
                        tags "Microsoft Azure - Virtual Networks"
                        
                        openAIInstance = containerInstance openAI {

                        }
                    }
                    
                    faaDeployment = deploymentNode "Apprenticeship Service" {
                        tags "Microsoft Azure - Virtual Networks"
                        
                        faaApiInstance = containerInstance faaApi {
                            
                            
                        }

                    }

                }
            }
            
        }
        
    }

    views {
        
        deployment * Service {
            title "Service View"
            include *
        }
        
        themes https://raw.githubusercontent.com/structurizr/themes/refs/heads/master/microsoft-azure-2024.07.15/icons.json
        // themes https://static.structurizr.com/themes/microsoft-azure-2023.01.24/theme.json
    }

}