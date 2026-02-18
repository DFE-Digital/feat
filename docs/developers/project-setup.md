# Project Setup

This describes how to set up and run the solution locally in a development environment.

The solution consists of three runnable projects:

- `feat.api` – .NET API  
- `feat.web` – .NET Website
- `feat.ingestion` – Ingestion Service

## Prerequisites

Ensure the following are installed:

- .NET SDK (version matching the solution)
- Visual Studio 2022 or later (or Rider / VS Code)
- Access to the relevant Azure resources
- Access to the Find and Use an API service (GOV.UK)

## Azure Resources

You may need access to the following Azure resources for connection strings and keys:

### s265d01-coursedb

For database connection string.

`Settings → Connection strings`

### s265d01-search

For Azure AI Search keys.

`Settings → Keys`

# Configuration

## API

### appsettings.Development.json

```json
"ConnectionStrings": {
  "Courses": ""
  ...
}
```

Ideally, use a local SQL Server/SQL Express instance and connect to that, otherwise get the connection string from `s265d01-coursedb`.

### User Secrets

`feat.api` uses .NET user secrets.

```bash
cd feat.api
dotnet user-secrets init
```

Set the following:

```json
{
  "Azure": {
    "OpenAIKey": "",
    "AISearchKey": ""
  }
}
```

#### Where to get these

- **OpenAIKey**  
  From the Find and Use an API service (GOV.UK)

- **AISearchKey**  
  From `s265d01-search → Settings → Keys`  
  You may use:
  - The Admin Key, or
  - A dedicated Query Key


## Website

No changes required to run locally.

## Ingestion Service

### appsettings.Development.json

```json
"Azure": {
  "AiSearchAdminKey": ""
  ...
}
"ConnectionStrings": {
  "Ingestion": ""
  ...
}
```

Use the same connection string as outlined for the API (ideally a local DB). The admin key should come from `s265d01-search`.

### User Secrets

`feat.ingestion` uses .NET user secrets.

```bash
cd feat.ingestion
dotnet user-secrets init
```

Set the following:

```json
{
  "Ingestion": {
    "ApprenticeshipApiKey": ""
  }
}
```

#### Where to get this

- **ApprenticeshipApiKey**  
  Request it via https://developer.apprenticeships.education.gov.uk

# Running the Solution Locally

- Run `feat.api` and `feat.web` together
- Run `feat.ingestion` independently when required

## Running the API

API base URL:

```
https://localhost:7151/api/
```

## Running the Website

Website URL:

```
https://localhost:7239/
```

## Running the Ingestion Service

### Arguments

You must provide at least one data source:

| Argument | Data Source |
|----------|------------|
| FAC      | Find A Course |
| FAA      | Find An Apprenticeship |
| DU       | Discover Uni |

### What the Ingestion Service Does

When executed, the ingestion service will:

- Apply database migrations
- Update Azure AI Search index fields
- Upsert data into the database
- Push searchable content into Azure AI Search

This service can be run safely multiple times.

# Caching

The easiest way to set this up is to spin up a local Docker instance of Redis and connect to it. In the `infrastructure/docker` folder of the solution is a `docker-compose-redis.yml` file.

Ensure Redis is running before running the ingestion service.

# API Requests

Scalar's interactive document UI can be found here:

```
https://localhost:7151/scalar/v1
```

This contains all requests and responses for the FEAT API.

Documentation of the Find An Apprenticeship API used by the ingestion service can be found here:
- https://developer.apprenticeships.education.gov.uk/Documentation/display-advert-api-v2#/operations/get-vacancy
- https://developer.apprenticeships.education.gov.uk/Documentation/display-advert-api-v2#/operations/get-vacancy-vacancyreference