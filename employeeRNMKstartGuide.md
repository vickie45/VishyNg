# Employee System - Complete Project Setup

## 🚀 Quick Start Guide

### Prerequisites
- ✅ .NET 6 SDK
- ✅ Node.js 18+
- ✅ Docker Desktop
- ✅ PostgreSQL client (optional)

### 1. Setup the Project

```bash
# Run the main setup script
./setup.sh

# This creates:
# ├── Employee.Api/          # REST API service
# ├── Employee.Worker/       # Kafka consumer service  
# ├── Employee.Tests/        # Unit tests
# ├── web-frontend/          # React app
# └── docker-compose.yml     # Local development stack
```

### 2. Start Infrastructure

```bash
cd employee-system

# Start PostgreSQL, Kafka, Zookeeper
docker compose up -d postgres kafka zookeeper

# Wait ~30 seconds for services to be ready
docker compose logs kafka  # Check if ready
```

### 3. Run the Services

```bash
# Terminal 1: API Service
dotnet run --project Employee.Api
# → Available at http://localhost:5000
# → Swagger UI at http://localhost:5000/swagger

# Terminal 2: Worker Service  
dotnet run --project Employee.Worker
# → Consumes Kafka messages and processes claims

# Terminal 3: Frontend
cd web-frontend
npm install
npm run dev
# → Available at http://localhost:5173
```

### 4. Test the System

1. **Open the React app**: http://localhost:5173
2. **Create a claim** using the form
3. **Check the API**: http://localhost:5000/swagger
4. **Monitor the worker** logs for message processing

## 🏗️ Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React App     │    │   Employee.Api  │    │Employee.Worker  │
│  (Frontend)     │────│   (Producer)    │    │  (Consumer)     │
│                 │    │                 │    │                 │
│ - Create claims │    │ - REST API      │    │ - Process events│
│ - View claims   │    │ - Kafka publish │    │ - Batch jobs    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │                       │
                                ▼                       ▼
                       ┌─────────────────┐    ┌─────────────────┐
                       │   PostgreSQL    │    │      Kafka      │
                       │   (Database)    │    │   (Message Bus) │
                       └─────────────────┘    └─────────────────┘
```

## 📁 Project Structure

```
employee-system/
├── Employee.Api/                    # Web API Service
│   ├── Controllers/ClaimsController.cs
│   ├── Models/EmployeeClaim.cs
│   ├── Data/AppDbContext.cs
│   └── Program.cs
├── Employee.Worker/                 # Background Service
│   ├── Services/ClaimConsumerService.cs
│   └── Program.cs
├── Employee.Tests/                  # Unit Tests
│   └── ClaimsControllerTests.cs
├── web-frontend/                    # React Frontend
│   ├── src/App.jsx
│   ├── src/App.css
│   └── package.json
├── docker-compose.yml              # Local infrastructure
├── azure-pipelines.yml            # CI/CD pipeline
└── EmployeeSystem.sln             # Solution file
```

## 🔧 Development Workflow

### Making Changes

1. **API Changes**: Modify `Employee.Api/Controllers/ClaimsController.cs`
2. **Worker Logic**: Update `Employee.Worker/Services/ClaimConsumerService.cs`
3. **Frontend**: Edit `web-frontend/src/App.jsx`
4. **Database**: Add EF migrations: `dotnet ef migrations add NewMigration -p Employee.Api`

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Frontend tests (if you add them)
cd web-frontend
npm test
```

### Docker Development

```bash
# Build all services
docker compose build

# Run entire stack in containers
docker compose up

# View logs
docker compose logs -f employee-api
docker compose logs -f employee-worker
```

## 🚀 Deployment

### Azure DevOps Pipeline

The included `azure-pipelines.yml` provides:

- ✅ **Build** - Compile .NET projects and React app
- ✅ **Test** - Run unit tests with coverage
- ✅ **Package** - Create Docker images  
- ✅ **Deploy** - Push to Azure Container Registry
- ✅ **Environment** - Deploy to dev/prod environments

### Manual Deployment

```bash
# Build production images
docker build -t employee-api -f Employee.Api/Dockerfile .
docker build -t employee-worker -f Employee.Worker/Dockerfile .

# Tag and push to registry
docker tag employee-api yourregistry.azurecr.io/employee-api:latest
docker push yourregistry.azurecr.io/employee-api:latest
```

## 🔍 Monitoring & Troubleshooting

### Common Issues

**Kafka Connection Issues:**
```bash
# Check if Kafka is ready
docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092
```

**Database Connection:**
```bash
# Connect to PostgreSQL
docker compose exec postgres psql -U devuser -d employee
```

**View Application Logs:**
```bash
# API logs
docker compose logs -f employee-api

# Worker logs  
docker compose logs -f employee-worker
```

### Health Checks

- **API Health**: http://localhost:5000/api/claims
- **Database**: Connect with connection string from appsettings
- **Kafka Topics**: Should see `employee-claims` topic created automatically

## 📊 Key Patterns & Best Practices

### 1. Event-Driven Architecture
- API persists data then publishes events
- Worker consumes events for async processing
- Eventual consistency between services

### 2. Database-First Approach
- Single source of truth in PostgreSQL
- Events are derived from database changes
- Consider outbox pattern for reliability

### 3. Idempotent Consumers  
- Worker can safely reprocess messages
- Use database constraints to prevent duplicates

### 4. Error Handling
- Retry logic with exponential backoff
- Dead letter queues for poison messages
- Structured logging for observability

## 🎯 Next Steps Checklist

### Week 1: Foundation
- [ ] Get the system running locally
- [ ] Create a simple claim via the UI
- [ ] Verify end-to-end flow (API → Kafka → Worker)
- [ ] Run tests successfully

### Week 2: Development
- [ ] Add a new claim field (e.g., description)
- [ ] Implement input validation
- [ ] Add error handling to the consumer
- [ ] Create additional unit tests

### Week 3: Enhancement
- [ ] Add authentication/authorization
- [ ] Implement the outbox pattern
- [ ] Add health check endpoints
- [ ] Set up monitoring/logging

### Week 4: Production Ready
- [ ] Configure CI/CD pipeline
- [ ] Add integration tests
- [ ] Performance testing
- [ ] Deploy to staging environment

## 📚 Learning Resources

- **Kafka**: [Confluent .NET Client Docs](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)
- **EF Core**: [Microsoft EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- **Azure DevOps**: [Pipeline YAML Schema](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema)
- **React**: [React Documentation](https://reactjs.org/docs/getting-started.html)

## 🆘 Getting Help

1. **Check the logs** first (API, Worker, Kafka, PostgreSQL)
2. **Verify service connectivity** (can API reach Kafka/DB?)
3. **Review configuration** (connection strings, Kafka bootstrap servers)
4. **Test individual components** (API endpoints, database queries)

---

**Happy coding! 🎉**

*This system provides a solid foundation for building event-driven microservices with .NET, React, and Kafka.*