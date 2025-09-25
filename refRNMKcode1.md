#!/bin/bash

# Employee System - Complete Project Setup
# This script creates the entire project structure with all files

echo "Creating Employee System project structure..."

# Create root directory structure
mkdir -p employee-system
cd employee-system

# Create solution
dotnet new sln -n EmployeeSystem

# Create project directories
mkdir -p Employee.Api/Controllers
mkdir -p Employee.Api/Models
mkdir -p Employee.Api/Data
mkdir -p Employee.Worker/Services
mkdir -p Employee.Tests
mkdir -p web-frontend/src

echo "✅ Directory structure created"

# Create docker-compose.yml
cat > docker-compose.yml << 'EOF'
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

  employee-api:
    build:
      context: .
      dockerfile: Employee.Api/Dockerfile
    ports:
      - "5000:80"
    depends_on: ["postgres", "kafka"]
    environment:
      - ConnectionStrings__Pg=Host=postgres;Port=5432;Database=employee;Username=devuser;Password=devpass
      - Kafka__BootstrapServers=kafka:9092

  employee-worker:
    build:
      context: .
      dockerfile: Employee.Worker/Dockerfile
    depends_on: ["postgres", "kafka"]
    environment:
      - ConnectionStrings__Pg=Host=postgres;Port=5432;Database=employee;Username=devuser;Password=devpass
      - Kafka__BootstrapServers=kafka:9092

volumes:
  pgdata:
EOF

echo "✅ docker-compose.yml created"

# Navigate to create API project
cd Employee.Api

# Create API project
dotnet new webapi --no-https

# Add packages
dotnet add package Confluent.Kafka
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore

# Create Models
cat > Models/EmployeeClaim.cs << 'EOF'
using System.Text.Json.Serialization;

namespace Employee.Api.Models;

public class EmployeeClaim
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EmployeeId { get; set; } = string.Empty;
    public string ClaimType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsProcessed { get; set; } = false;
}
EOF

# Create DbContext
cat > Data/AppDbContext.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using Employee.Api.Models;

namespace Employee.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<EmployeeClaim> EmployeeClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeClaim>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ClaimType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });
    }
}
EOF

# Create Controller
cat > Controllers/ClaimsController.cs << 'EOF'
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Confluent.Kafka;
using Employee.Api.Data;
using Employee.Api.Models;

namespace Employee.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<ClaimsController> _logger;

    public ClaimsController(AppDbContext db, IProducer<Null, string> producer, ILogger<ClaimsController> logger)
    {
        _db = db;
        _producer = producer;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeClaim claim)
    {
        try
        {
            if (string.IsNullOrEmpty(claim.EmployeeId) || claim.Amount <= 0)
            {
                return BadRequest("EmployeeId and Amount are required");
            }

            // Save to database first
            _db.EmployeeClaims.Add(claim);
            await _db.SaveChangesAsync();

            // Then publish event
            var payload = JsonSerializer.Serialize(claim);
            _producer.Produce("employee-claims", new Message<Null, string> { Value = payload },
                deliveryReport =>
                {
                    if (deliveryReport.Error.IsError)
                    {
                        _logger.LogError("Failed to deliver message: {error}", deliveryReport.Error.Reason);
                    }
                    else
                    {
                        _logger.LogInformation("Message delivered to: {topicPartitionOffset}", deliveryReport.TopicPartitionOffset);
                    }
                });

            return CreatedAtAction(nameof(GetById), new { id = claim.Id }, claim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating claim");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var claim = await _db.EmployeeClaims.FindAsync(id);
        if (claim == null) return NotFound();
        return Ok(claim);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var claims = await _db.EmployeeClaims
            .OrderByDescending(c => c.CreatedAt)
            .Take(100)
            .ToListAsync();
        return Ok(claims);
    }
}
EOF

# Update Program.cs
cat > Program.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using Employee.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Pg")));

// Add Kafka Producer
builder.Services.AddSingleton<IProducer<Null, string>>(serviceProvider =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
    };
    return new ProducerBuilder<Null, string>(config).Build();
});

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
EOF

# Create appsettings files
cat > appsettings.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
EOF

cat > appsettings.Development.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "ConnectionStrings": {
    "Pg": "Host=localhost;Port=5432;Database=employee;Username=devuser;Password=devpass"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  }
}
EOF

# Create Dockerfile for API
cat > Dockerfile << 'EOF'
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Employee.Api/Employee.Api.csproj", "Employee.Api/"]
RUN dotnet restore "Employee.Api/Employee.Api.csproj"
COPY . .
WORKDIR "/src/Employee.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Employee.Api.dll"]
EOF

# Go back and add to solution
cd ..
dotnet sln add Employee.Api/Employee.Api.csproj

echo "✅ Employee.Api project created"

# Create Worker project
cd Employee.Worker
dotnet new worker

# Add packages to worker
dotnet add package Confluent.Kafka
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.Extensions.Hosting

# Create the consumer service
cat > Services/ClaimConsumerService.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using System.Text.Json;

namespace Employee.Worker.Services;

public class EmployeeClaim
{
    public Guid Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string ClaimType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsProcessed { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<EmployeeClaim> EmployeeClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeClaim>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ClaimType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });
    }
}

public class ClaimConsumerService : BackgroundService
{
    private readonly ILogger<ClaimConsumerService> _logger;
    private readonly ConsumerConfig _config;
    private readonly IServiceScopeFactory _scopeFactory;

    public ClaimConsumerService(ILogger<ClaimConsumerService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = "employee-claims-processor",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        consumer.Subscribe("employee-claims");
        
        _logger.LogInformation("Claim consumer started. Waiting for messages...");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    var payload = consumeResult.Message.Value;
                    _logger.LogInformation("Consumed message: {message}", payload);

                    // Process in scoped DI context
                    using var scope = _scopeFactory.CreateScope();
                    await ProcessClaim(scope, payload);

                    // Commit offset after successful processing
                    consumer.Commit(consumeResult);
                    
                    _logger.LogInformation("Message processed successfully");
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    // In production, consider sending to DLQ
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer cancelled");
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessClaim(IServiceScope scope, string payload)
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var claim = JsonSerializer.Deserialize<EmployeeClaim>(payload);
        
        if (claim == null)
        {
            _logger.LogWarning("Failed to deserialize claim");
            return;
        }

        // Example processing: mark as processed
        var existingClaim = await db.EmployeeClaims.FindAsync(claim.Id);
        if (existingClaim != null)
        {
            existingClaim.IsProcessed = true;
            await db.SaveChangesAsync();
            _logger.LogInformation("Claim {claimId} marked as processed", claim.Id);
        }
        else
        {
            _logger.LogWarning("Claim {claimId} not found in database", claim.Id);
        }

        // Simulate some processing time
        await Task.Delay(100);
    }
}
EOF

# Update Program.cs for worker
cat > Program.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using Employee.Worker.Services;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    // Add EF Core
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(context.Configuration.GetConnectionString("Pg")));

    // Add the consumer service
    services.AddHostedService<ClaimConsumerService>();
});

var host = builder.Build();

// Ensure database is created
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

host.Run();
EOF

# Create appsettings for worker
cat > appsettings.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
EOF

cat > appsettings.Development.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "Pg": "Host=localhost;Port=5432;Database=employee;Username=devuser;Password=devpass"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  }
}
EOF

# Create Dockerfile for Worker
cat > Dockerfile << 'EOF'
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Employee.Worker/Employee.Worker.csproj", "Employee.Worker/"]
RUN dotnet restore "Employee.Worker/Employee.Worker.csproj"
COPY . .
WORKDIR "/src/Employee.Worker"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Employee.Worker.dll"]
EOF

# Add to solution
cd ..
dotnet sln add Employee.Worker/Employee.Worker.csproj

echo "✅ Employee.Worker project created"

# Create test project
cd Employee.Tests
dotnet new xunit
dotnet add package Moq
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add reference ../Employee.Api/Employee.Api.csproj

# Create a sample test
cat > ClaimsControllerTests.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Employee.Api;
using Employee.Api.Models;
using Xunit;

namespace Employee.Tests;

public class ClaimsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ClaimsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnCreated_WhenValidClaim()
    {
        // Arrange
        var client = _factory.CreateClient();
        var claim = new EmployeeClaim
        {
            EmployeeId = "EMP001",
            ClaimType = "termination-pay",
            Amount = 1000.00m
        };

        var json = JsonSerializer.Serialize(claim);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/claims", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }
}
EOF

cd ..
dotnet sln add Employee.Tests/Employee.Tests.csproj

echo "✅ Employee.Tests project created"

echo ""
echo "🎉 Complete Employee System created!"
echo ""
echo "Next steps:"
echo "1. Run: cd employee-system"
echo "2. Run: docker compose up -d postgres kafka zookeeper"
echo "3. Run: dotnet run --project Employee.Api"
echo "4. Run: dotnet run --project Employee.Worker (in another terminal)"
echo "5. Run: dotnet test Employee.Tests"
echo ""
echo "API will be available at: http://localhost:5000"
echo "Swagger UI at: http://localhost:5000/swagger"