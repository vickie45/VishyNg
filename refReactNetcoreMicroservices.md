Great — I’ll give you a practical, step-by-step master tutorial that covers the real parts you’ll use on the project: a .NET 6 microservice (producer + consumer), connecting to PostgreSQL with EF Core, wiring Kafka (Confluent client), a small React frontend (Vite + React) showing how to call the service, Docker + docker-compose for local dev, unit tests, and a sample Azure DevOps pipeline YAML to build, test and push images. I’ll also include recommended patterns for batch jobs and tips for using Copilot and enterprise templates efficiently.

I ran quick documentation checks while assembling the tutorial so the code and best practices align with official guidance and clients. Key references used: .NET docs, Confluent .NET Kafka client docs, PostgreSQL docs, and Azure DevOps pipeline docs. 


---

Tutorial: Mastering the Stack — React + .NET 6 microservices + Kafka + PostgreSQL + Azure DevOps

> Estimated scope: follow these modules in order. Each module is self-contained so you can start implementing small pieces immediately and expand. Treat this as a practical playbook to survive and excel on your project.




---

Module 0 — Assumptions & local setup checklist

You already installed IDE, .NET 6 and Node.js. Make sure:

.NET SDK 6.x installed and dotnet --info shows it. (Official docs for .NET 6). 

Install Docker Desktop (for local Kafka, Zookeeper, PostgreSQL).

Install PostgreSQL client tools (psql) or use pgAdmin. Official docs: PostgreSQL. 

Install Kafka locally via Docker (we use confluentinc/cp-kafka in docker-compose in this tutorial). Official Kafka docs useful when you scale. 

Familiarize quickly with Azure DevOps YAML pipelines (we’ll include a skeleton). 



---

Module 1 — Project skeletons

We’ll create two projects:

1. Employee.Api — a .NET 6 microservice (ASP.NET Core Web API) — exposes REST endpoints and produces events to Kafka.


2. Employee.Worker — .NET background service that consumes Kafka events, processes batches, writes to PostgreSQL.


3. web-frontend — React (Vite) UI that uses a component library and calls Employee.Api.



Create root directory:

mkdir employee-system
cd employee-system
dotnet new sln -n EmployeeSystem


---

Module 2 — Docker compose (local dev)

Create docker-compose.yml to run PostgreSQL, Zookeeper, Kafka (Confluent), and schema-registry (optional).

version: '3.8'
services:
  zookeeper:
    image: confluentinc/cp-zookeeper:7.4.0
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181
      ZOOKEEPER_TICK_TIME: 2000
    ports:
      - "2181:2181"

  kafka:
    image: confluentinc/cp-kafka:7.4.0
    depends_on: ["zookeeper"]
    environment:
      KAFKA_BROKER_ID: 1
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:9092,OUTSIDE://localhost:29092
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,OUTSIDE:PLAINTEXT
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
    ports:
      - "9092:9092"
      - "29092:29092"

  postgres:
    image: postgres:15
    environment:
      POSTGRES_USER: devuser
      POSTGRES_PASSWORD: devpass
      POSTGRES_DB: employee
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:

Run:

docker compose up -d

Notes: using Confluent images works well for local dev. Use official Kafka docs if you need cluster details. 


---

Module 3 — .NET 6 API (producer)

Create the API project and solution entry:

dotnet new webapi -n Employee.Api
dotnet sln add Employee.Api/Employee.Api.csproj

Install packages:

cd Employee.Api
dotnet add package Confluent.Kafka
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Swashbuckle.AspNetCore

Domain model (simple)

Create Models/EmployeeClaim.cs:

public class EmployeeClaim
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EmployeeId { get; set; }
    public string ClaimType { get; set; } // e.g., 'termination-pay'
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

EF Core DbContext

Data/AppDbContext.cs:

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) {}
    public DbSet<EmployeeClaim> EmployeeClaims { get; set; }
}

Register in Program.cs:

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Pg")));

// Kafka producer config
builder.Services.AddSingleton(sp =>
{
    var config = new Confluent.Kafka.ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    return new Confluent.Kafka.ProducerBuilder<Null, string>(config).Build();
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

Add configuration in appsettings.Development.json:

{
  "ConnectionStrings": {
    "Pg": "Host=localhost;Port=5432;Database=employee;Username=devuser;Password=devpass"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  }
}

Controller: create claim and publish event

Controllers/ClaimsController.cs:

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IProducer<Null, string> _producer;

    public ClaimsController(AppDbContext db, IProducer<Null, string> producer)
    {
        _db = db;
        _producer = producer;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeClaim claim)
    {
        _db.EmployeeClaims.Add(claim);
        await _db.SaveChangesAsync();

        var payload = JsonSerializer.Serialize(claim);
        // fire-and-forget — but log result; you can also await delivery reports
        _producer.Produce("employee-claims", new Message<Null, string>{ Value = payload },
            r => {
                // Logging in callback
                Console.WriteLine($"Delivered: {r.TopicPartitionOffset}");
            });

        return CreatedAtAction(nameof(GetById), new { id = claim.Id }, claim);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var e = await _db.EmployeeClaims.FindAsync(id);
        if (e == null) return NotFound();
        return Ok(e);
    }
}

Explanation / patterns

Producer is a singleton created with Confluent.Kafka.ProducerBuilder (Confluent .NET client is the recommended client). 

We persist the claim to Postgres first (single source of truth) and then publish an event — common pattern for eventual consistency. Consider transactional outbox for stronger guarantees in production.



---

Module 4 — .NET Background worker (consumer + batch processing)

Create worker project:

cd ..
dotnet new worker -n Employee.Worker
dotnet sln add Employee.Worker/Employee.Worker.csproj
dotnet add Employee.Worker package Confluent.Kafka
dotnet add Employee.Worker package Microsoft.EntityFrameworkCore
dotnet add Employee.Worker package Npgsql.EntityFrameworkCore.PostgreSQL

Implement a BackgroundService that consumes Kafka and processes claims.

Worker/ClaimConsumerService.cs:

public class ClaimConsumerService : BackgroundService
{
    private readonly ILogger<ClaimConsumerService> _logger;
    private readonly ConsumerConfig _config;
    private readonly IServiceScopeFactory _scopeFactory;

    public ClaimConsumerService(ILogger<ClaimConsumerService> logger, IConfiguration cfg, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _config = new ConsumerConfig
        {
            BootstrapServers = cfg["Kafka:BootstrapServers"],
            GroupId = "employee-claims-processor",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        consumer.Subscribe("employee-claims");
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var payload = consumeResult.Message.Value;
                _logger.LogInformation("Consumed: {msg}", payload);

                // Process in scoped DI (for EF DbContext)
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var claim = JsonSerializer.Deserialize<EmployeeClaim>(payload);
                // Example processing: mark as processed, run business logic, etc.
                // e.g., insert into processed table or call external batch job.
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            consumer.Close();
        }
    }
}

Batch jobs

For scheduled or large batches, use a separate worker or Azure WebJobs/Azure Functions Timer trigger. For in-process scheduled batches, implement IHostedService / BackgroundService and run periodic tasks with Delay or a scheduling library (e.g., Quartz.NET) when needed.



---

Module 5 — EF Core Migrations & PostgreSQL tips

Install EF tools:

dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design

Create migration:

dotnet ef migrations add InitialCreate -p Employee.Api -s Employee.Api
dotnet ef database update -p Employee.Api -s Employee.Api

Notes:

Use Npgsql provider for PostgreSQL: Npgsql.EntityFrameworkCore.PostgreSQL. Official docs: PostgreSQL + EF Core guidance. 

Use connection pooling and set sensible command timeouts in production.



---

Module 6 — Dockerizing services

Example Dockerfile for API:

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Employee.Api/Employee.Api.csproj", "Employee.Api/"]
RUN dotnet restore "Employee.Api/Employee.Api.csproj"
COPY . .
WORKDIR "/src/Employee.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Employee.Api.dll"]

Dockerfile for worker similar but use dotnet Employee.Worker.dll.

Add docker-compose services to run API and Worker against local Kafka/Postgres in development (connect via service names).


---

Module 7 — React frontend (Vite + React) calling API

Create Vite app:

npm create vite@latest web-frontend --template react
cd web-frontend
npm install

If "Lift UI" is an in-house component library in your project, use that. I couldn't find an official public "Lift UI" package — use your project’s internal library or choose a modern component library like MUI/Chakra (if you need public alternative). (I checked for a public Lift UI; no authoritative package was found.) 

Example src/App.jsx (simple claim form):

import { useState } from 'react';

function App(){
  const [employeeId, setEmployeeId] = useState('');
  const [amount, setAmount] = useState('');

  async function submit(e){
    e.preventDefault();
    const payload = {
      employeeId,
      claimType: 'termination-pay',
      amount: parseFloat(amount)
    };
    const res = await fetch('http://localhost:5000/api/claims', {
      method: 'POST',
      headers: {'Content-Type':'application/json'},
      body: JSON.stringify(payload)
    });
    if(res.ok){
      alert('Claim created');
    } else {
      alert('Failed');
    }
  }

  return (
    <div style={{padding:20}}>
      <h2>Create Claim</h2>
      <form onSubmit={submit}>
        <input placeholder="Employee ID" value={employeeId} onChange={e=>setEmployeeId(e.target.value)} />
        <input placeholder="Amount" value={amount} onChange={e=>setAmount(e.target.value)} />
        <button type="submit">Create</button>
      </form>
    </div>
  );
}

export default App;

Run npm run dev. Use CORS in API or proxy in Vite for dev.


---

Module 8 — Unit tests & integration tests

Use xUnit with Moq for .NET:

dotnet new xunit -n Employee.Tests
dotnet add Employee.Tests/ package Moq
dotnet sln add Employee.Tests/Employee.Tests.csproj

Example unit test for controller: mock the DbContext using in-memory provider or use repository pattern and mock repository. For integration tests, use WebApplicationFactory<T> to spin up in-memory server.

Tip: Confluent.Kafka producer/consumer behavior is best tested by abstracting the producer as an interface and mocking it in unit tests, or using a local test instance of Kafka for integration tests.


---

Module 9 — Azure DevOps pipeline (CI/CD) — basic YAML

This skeleton builds both projects, runs tests, builds docker images and pushes to Azure Container Registry (ACR) (adjust for your registry).

azure-pipelines.yml:

trigger:
  - main

variables:
  imageName: 'employee-api'

stages:
- stage: Build
  jobs:
  - job: Build
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - task: UseDotNet@2
      inputs:
        packageType: 'sdk'
        version: '6.x'
    - script: dotnet restore
    - script: dotnet build --no-restore -c Release
    - script: dotnet test --no-build -c Release
    - task: Docker@2
      inputs:
        containerRegistry: 'YOUR_ACR_SERVICE_CONNECTION'
        repository: '$(imageName)'
        command: 'buildAndPush'
        Dockerfile: 'Employee.Api/Dockerfile'
        tags: |
          $(Build.BuildId)

Notes:

Use YAML templates and library variables for secrets (service connections) — Azure DevOps docs show how to structure templates and secure pipeline variables. 

For multi-service deployments, make multi-stage pipelines: build -> push -> deploy.



---

Module 10 — Operational considerations & best practices

1. Transactions & Outbox

When you update DB and publish to Kafka, consider the outbox pattern or transactional outbox to prevent lost events. For high reliability, avoid naive DB-then-produce without safeguards.



2. Idempotency & consumer design

Consumers should be idempotent — reprocessing must not cause duplicates.



3. Schema management

Use JSON schema or Avro for messages if you have contract evolution needs. Consider schema registry (Confluent) for governance.



4. Monitoring & logging

Push logs to centralized system (ELK, Azure Monitor). Monitor Kafka lag, consumer group offsets.



5. Retries & DLQ

Implement retry/backoff in consumers; route poison messages to Dead Letter Queue (DLQ) topics.



6. Testing local Kafka

For CI, you can spin up ephemeral Kafka via docker-compose in a build agent or use Testcontainers in integration tests.



7. Batch processing

For long-running batch jobs either: implement dedicated workers with BackgroundService or use cloud-native scheduled functions (Azure Functions Timer) or Azure Batch, depending on SLA.





---

Module 11 — Copilot & enterprise templates — practical tips

Use templates as scaffolds: your org’s templates are gold. Use them to generate controllers, DTOs, and tests. Copilot becomes most useful when you paste your project’s template comments / XML docstrings — it will generate code that follows existing conventions.

Treat Copilot suggestions as drafts: always review — especially for security (SQL injection, secrets), concurrency, and error handling.

Unit tests from Copilot: Copilot can help draft tests; again, review edge cases and mocking.

Document snippets: maintain a personal snippets file (or VS Code snippets) matching your org’s templates.



---

Module 12 — Learning roadmap (30 / 60 / 90 day)

Follow this schedule in parallel with work:

First 2 weeks (stabilize)

Read existing code and templates.

Deploy local docker-compose; run API + worker.

Implement a trivial bug or small feature and deliver it.


Next 30 days (build fluency)

Add a Kafka consumer flow in worker and a test.

Create EF migrations and learn deployment config.

Add a React page that calls an API endpoint.


60–90 days (ownership)

Implement outbox pattern or reliable delivery.

Harden consumer (retries, DLQ).

Create CI/CD pipeline (build/test/push) and run it end-to-end.



---

Module 13 — Ready-to-copy checklist for first ticket

1. Run docker compose up -d.


2. dotnet ef database update (run migrations).


3. Run dotnet run --project Employee.Api and dotnet run --project Employee.Worker.


4. Run npm run dev in web-frontend.


5. Create a small fix/feature and open a PR using your org’s PR template.


6. Add unit tests; run them locally.


7. Push a branch and trigger CI (Azure DevOps); observe pipeline run.




---

References & docs (most load-bearing links)

.NET official docs (get started, runtime, SDK). 

Confluent .NET Kafka client (producer/consumer API). 

PostgreSQL official docs (install, query, advanced topics). 

Azure DevOps pipelines YAML guide. 

Apache Kafka docs (brokers/clients/streams). 



---

Quick patterns cheat-sheet (copy into your personal doc)

Persist first → publish event (use outbox if possible).

Consumer idempotency + DLQ.

BackgroundService for batch; Quartz.NET for scheduling.

Abstract Kafka client behind an interface for testability.

Docker-compose for local dependencies; keep images lightweight in CI.

Use YAML templates & secure service connections in Azure DevOps.



---

Final practical files I recommend you create right now

~/notes/employee-system/README.md — your personal doc: endpoints, topic names, DB schema, conventions.

~/notes/employee-system/snippets/ — copy common code snippets and template headers.

~/notes/employee-system/pipeline-templates/azure-pipelines.yml — your working pipeline template.



---

If you want, I’ll now:

1. Generate the complete repository scaffolding (all files above populated) as a zipped project you can run locally (I can produce the files and explain how to run them), or



2. Produce a compact one-page cheat sheet and a 90-day checklist you can print/share.




Tell me which you prefer and I’ll produce it immediately. (I’ll generate files and step-by-step run commands in this chat when you pick an option.)

