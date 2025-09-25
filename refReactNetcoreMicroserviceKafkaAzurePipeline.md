# azure-pipelines.yml
# Multi-stage pipeline for Employee System

trigger:
  - main
  - develop

variables:
  buildConfiguration: 'Release'
  vmImageName: 'ubuntu-latest'
  
  # Container Registry variables (replace with your values)
  dockerRegistryServiceConnection: 'YOUR_ACR_SERVICE_CONNECTION'
  imageRepository: 'employee-system'
  containerRegistry: 'yourregistry.azurecr.io'
  dockerfilePath: '$(Build.SourcesDirectory)'
  tag: '$(Build.BuildId)'

stages:
- stage: Build
  displayName: Build and Test
  jobs:
  - job: Build
    displayName: Build
    pool:
      vmImage: $(vmImageName)
    
    steps:
    - task: UseDotNet@2
      displayName: 'Use .NET 6'
      inputs:
        packageType: 'sdk'
        version: '6.x'
    
    - task: DotNetCoreCLI@2
      displayName: 'Restore packages'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build solution'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration) --no-restore'
    
    - task: DotNetCoreCLI@2
      displayName: 'Run tests'
      inputs:
        command: 'test'
        projects: '**/*Tests.csproj'
        arguments: '--configuration $(buildConfiguration) --no-build --collect:"XPlat Code Coverage"'
    
    - task: PublishCodeCoverageResults@1
      displayName: 'Publish code coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
    
    - task: DotNetCoreCLI@2
      displayName: 'Publish API'
      inputs:
        command: 'publish'
        publishWebProjects: false
        projects: 'Employee.Api/Employee.Api.csproj'
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/api'
        zipAfterPublish: true
    
    - task: DotNetCoreCLI@2
      displayName: 'Publish Worker'
      inputs:
        command: 'publish'
        publishWebProjects: false
        projects: 'Employee.Worker/Employee.Worker.csproj'
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/worker'
        zipAfterPublish: true
    
    # Frontend build
    - task: NodeTool@0
      displayName: 'Use Node 18'
      inputs:
        versionSpec: '18'
    
    - script: |
        cd web-frontend
        npm ci
        npm run build
      displayName: 'Build React app'
    
    - task: CopyFiles@2
      displayName: 'Copy React build'
      inputs:
        SourceFolder: 'web-frontend/dist'
        Contents: '**'
        TargetFolder: '$(Build.ArtifactStagingDirectory)/web'
    
    - publish: $(Build.ArtifactStagingDirectory)
      artifact: drop
      displayName: 'Publish artifacts'

- stage: BuildContainers
  displayName: Build and Push Container Images
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: BuildContainers
    displayName: Build Containers
    pool:
      vmImage: $(vmImageName)
    
    steps:
    - task: Docker@2
      displayName: 'Build and push API image'
      inputs:
        containerRegistry: '$(dockerRegistryServiceConnection)'
        repository: '$(imageRepository)-api'
        command: 'buildAndPush'
        Dockerfile: 'Employee.Api/Dockerfile'
        buildContext: '$(Build.SourcesDirectory)'
        tags: |
          $(tag)
          latest
    
    - task: Docker@2
      displayName: 'Build and push Worker image'
      inputs:
        containerRegistry: '$(dockerRegistryServiceConnection)'
        repository: '$(imageRepository)-worker'
        command: 'buildAndPush'
        Dockerfile: 'Employee.Worker/Dockerfile'
        buildContext: '$(Build.SourcesDirectory)'
        tags: |
          $(tag)
          latest

- stage: Deploy
  displayName: Deploy to Development
  dependsOn: BuildContainers
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
  jobs:
  - deployment: DeployToDev
    displayName: Deploy to Development
    pool:
      vmImage: $(vmImageName)
    environment: 'development'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebAppContainer@1
            displayName: 'Deploy API to Azure Web App'
            inputs:
              azureSubscription: 'YOUR_AZURE_SERVICE_CONNECTION'
              appName: 'employee-api-dev'
              containers: '$(containerRegistry)/$(imageRepository)-api:$(tag)'
          
          # Add additional deployment steps as needed

- stage: DeployProduction
  displayName: Deploy to Production
  dependsOn: BuildContainers
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployToProd
    displayName: Deploy to Production
    pool:
      vmImage: $(vmImageName)
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebAppContainer@1
            displayName: 'Deploy API to Azure Web App'
            inputs:
              azureSubscription: 'YOUR_AZURE_SERVICE_CONNECTION'
              appName: 'employee-api-prod'
              containers: '$(containerRegistry)/$(imageRepository)-api:$(tag)'