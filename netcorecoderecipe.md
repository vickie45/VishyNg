Got it — you’re asking for “.NET Core code recipes” — essentially small, production-ready snippets/templates you can reuse in day-to-day development.
I’ll give you a library of code recipes arranged by topic. Each recipe includes:

Use case (why you’d use it)

Code snippet (ready to copy)

Key points / best practices



---

1️⃣ Web API Controller – CRUD Template

Use case: A standard controller for CRUD operations in an ASP.NET Core Web API project.

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        return await _context.Products.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id) return BadRequest();
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

✅ Key points:

Use async EF Core methods (ToListAsync, FindAsync).

Return correct HTTP codes.

Use CreatedAtAction for POST.



---

2️⃣ Dependency Injection – Service Registration

Use case: Register a custom service in Program.cs or Startup.cs.

// Service Interface
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

// Implementation
public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // send email logic here
        await Task.CompletedTask;
    }
}

// In Program.cs
builder.Services.AddScoped<IEmailService, EmailService>();

✅ Key points:

Scoped for web requests, Singleton for config-based services.

Inject into controllers via constructor.



---

3️⃣ Configuration & Options Pattern

Use case: Bind appsettings.json sections to strongly typed objects.

// appsettings.json
{
  "SmtpSettings": {
    "Host": "smtp.mailtrap.io",
    "Port": 2525,
    "Username": "user",
    "Password": "pass"
  }
}

// Model
public class SmtpSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

// Program.cs
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Controller
public class TestController : ControllerBase
{
    private readonly SmtpSettings _smtp;

    public TestController(IOptions<SmtpSettings> smtpOptions)
    {
        _smtp = smtpOptions.Value;
    }
}

✅ Key points:

Use IOptions<T> for runtime read.

Use IOptionsMonitor<T> for refreshable settings.



---

4️⃣ Middleware – Custom Logging

Use case: Write a middleware to log requests and responses.

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
        await _next(context);
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
}

// Program.cs
app.UseMiddleware<LoggingMiddleware>();

✅ Key points:

Middleware runs per-request.

Always call _next(context) to pass control to the next component.



---

5️⃣ Global Exception Handling

Use case: Handle all unhandled exceptions globally.

// Program.cs
app.UseExceptionHandler("/error");

// Error Controller
[ApiController]
public class ErrorController : ControllerBase
{
    [Route("error")]
    public IActionResult HandleError() =>
        Problem("An unexpected error occurred");
}

✅ Key points:

Use UseExceptionHandler for production.

Use DeveloperExceptionPage for dev environment.



---

6️⃣ Paging & Filtering with EF Core

Use case: Paginate large datasets.

[HttpGet("paged")]
public async Task<ActionResult<IEnumerable<Product>>> GetPaged(int page = 1, int pageSize = 10)
{
    return await _context.Products
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}

✅ Key points:

Always index columns used for filtering.

Consider returning metadata (total count).



---

7️⃣ Background Task with Hosted Service

Use case: Run background jobs (e.g., cleanup, reminders).

public class MyBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Background task running...");
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

// Program.cs
builder.Services.AddHostedService<MyBackgroundService>();

✅ Key points:

Use BackgroundService for infinite loops.

Handle CancellationToken properly.



---

8️⃣ Action Filters for Cross-Cutting Concerns

Use case: Add logic before/after action execution.

public class LogActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine($"Action: {context.ActionDescriptor.DisplayName}");
    }
}

// Apply globally in Program.cs
builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogActionFilter>();
});

✅ Key points:

Use for validation, logging, caching.

Can be applied per action or globally.



---

9️⃣ JWT Authentication Setup

Use case: Secure your API with JWT tokens.

// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

app.UseAuthentication();
app.UseAuthorization();

✅ Key points:

Always store secrets securely.

Use refresh tokens for long-lived sessions.



---

🔟 Repository Pattern with EF Core

Use case: Abstract data layer.

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAll();
    Task<Product> GetById(int id);
}

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAll() =>
        await _context.Products.ToListAsync();

    public async Task<Product> GetById(int id) =>
        await _context.Products.FindAsync(id);
}

// Program.cs
builder.Services.AddScoped<IProductRepository, ProductRepository>();

✅ Key points:

Keeps controllers thin.

Makes testing easier.



---

📚 Next Steps

I can also provide you:

Advanced recipes (CQRS, MediatR, caching, SignalR, gRPC)

Full project templates (clean architecture, multi-layered solutions)

Security-focused recipes (rate limiting, audit logging, 2FA)



---

Would you like me to extend this into an “enterprise-grade .NET Core cookbook” (with ~25–30 recipes covering everything: caching, email, queues, testing, CI/CD)? (It would be like a ready reference book.)

Perfect 🙂 — let’s build you an Enterprise-Grade .NET Core Cookbook.
I’ll structure it as a reference guide with ~30 recipes, grouped by layers (API, Data, Security, Cross-cutting, Background/Integration, Testing/Deployment).
Each recipe will be self-contained, production-ready, and follow best practices.

Here’s Part 1 (10 recipes).
If you like the format, I’ll keep adding parts until we reach 25–30 recipes.


---

🥘 Enterprise .NET Core Cookbook (Part 1)


---

📂 A. API Layer

1️⃣ Global Exception Handling (Enterprise Style)

Use case: Centralize error handling & logging.

// Middleware
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "An unexpected error occurred"
            }));
        }
    }
}

// Program.cs
app.UseMiddleware<ErrorHandlingMiddleware>();

✅ Why:

Centralizes error handling.

Logs stack traces safely.

Returns friendly JSON to client.



---

2️⃣ API Versioning

Use case: Support multiple API versions seamlessly.

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Controller
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ProductsController : ControllerBase { /* … */ }

✅ Why:

Backward compatibility.

Allows incremental migrations.



---

3️⃣ FluentValidation for Model Validation

public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Length(2, 50);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// Program.cs
builder.Services.AddControllers().AddFluentValidation(fv =>
    fv.RegisterValidatorsFromAssemblyContaining<ProductValidator>());

✅ Why:

Keeps validation logic out of controllers.

Reusable & testable.



---

4️⃣ Rate Limiting

// Add package: Microsoft.AspNetCore.RateLimiting
builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter("fixed", options =>
{
    options.PermitLimit = 10;
    options.Window = TimeSpan.FromSeconds(30);
}));

app.UseRateLimiter();

// Apply to endpoint
app.MapGet("/api/products", () => "Hello").RequireRateLimiting("fixed");

✅ Why:

Protects APIs from abuse.

Enterprise security standard.



---

5️⃣ Health Checks

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("Default"));

app.MapHealthChecks("/health");

✅ Why:

Used by Kubernetes, Azure, AWS for liveness checks.

Easy monitoring.



---


---

📂 B. Data Layer

6️⃣ Generic Repository Pattern

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById(int id);
    Task Add(T entity);
    Task Delete(int id);
}

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    public Repository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<T>> GetAll() => await _context.Set<T>().ToListAsync();
    public async Task<T> GetById(int id) => await _context.Set<T>().FindAsync(id);
    public async Task Add(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
    }
    public async Task Delete(int id)
    {
        var entity = await GetById(id);
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}

✅ Why:

Keeps EF Core isolated.

Simplifies testing.



---

7️⃣ Unit of Work Pattern

public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<Customer> Customers { get; }
    Task<int> CompleteAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IRepository<Product> Products { get; }
    public IRepository<Customer> Customers { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new Repository<Product>(context);
        Customers = new Repository<Customer>(context);
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}

✅ Why:

Handles transactions.

Central point for saving changes.



---

8️⃣ EF Core Query Filters (Multi-Tenancy)

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .HasQueryFilter(p => p.TenantId == _tenantId);
}

✅ Why:

Automatically filter data by tenant/user.

Security built-in at DB layer.



---


---

📂 C. Security Layer

9️⃣ JWT + Refresh Tokens

Key parts:

Generate access token (short-lived).

Generate refresh token (stored securely).

Exchange refresh token for new access token.


var tokenHandler = new JwtSecurityTokenHandler();
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
    Expires = DateTime.UtcNow.AddMinutes(15),
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
};
var token = tokenHandler.CreateToken(tokenDescriptor);
string jwt = tokenHandler.WriteToken(token);

✅ Why:

Short-lived access tokens improve security.

Refresh tokens allow silent re-authentication.



---


---

📂 D. Cross-Cutting Concerns

🔟 Caching with IMemoryCache

public class ProductService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public ProductService(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<Product>> GetProductsCached()
    {
        return await _cache.GetOrCreateAsync("products_cache", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _context.Products.ToListAsync();
        });
    }
}

// Program.cs
builder.Services.AddMemoryCache();

✅ Why:

Reduces DB calls.

Very simple to implement.



---

🔹 Coming up in Part 2

Distributed Caching with Redis

Background Jobs (Hangfire)

Outbox Pattern (Reliable Messaging)

CQRS with MediatR

SignalR (real-time notifications)

Unit Testing, Integration Testing, TestContainers

Deployment recipes (Docker, CI/CD pipeline)

Secure secrets with Azure Key Vault / AWS Secrets Manager

Rate limiting advanced scenarios

Advanced logging with Serilog + ELK



---

Would you like me to continue with Part 2 (another ~10 recipes) right away? (It’ll include advanced enterprise patterns like CQRS, Hangfire, Redis, SignalR, and testing.)

🥘 Enterprise .NET Core Cookbook — Part 2 (10 advanced recipes)

Great — here are 10 production-ready recipes covering distributed caching, background jobs, reliable messaging, CQRS, real-time, logging, email, file storage, integration tests, and container + CI. Each recipe includes: use case, code snippet / config, and key points.


---

1️⃣ Distributed Cache — Redis (StackExchange.Redis + IDistributedCache)

Use case: Share cache across multiple instances (Kubernetes, scaled web farm).

// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyApp:";
});

// Usage (in a service)
public class ProductCacheService
{
    private readonly IDistributedCache _cache;
    public ProductCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<IEnumerable<Product>> GetOrSetProductsAsync(Func<Task<IEnumerable<Product>>> factory)
    {
        var cached = await _cache.GetStringAsync("products:list");
        if (cached != null) return JsonSerializer.Deserialize<IEnumerable<Product>>(cached);

        var products = await factory();
        await _cache.SetStringAsync("products:list", JsonSerializer.Serialize(products),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
        return products;
    }
}

Key points:

Use IDistributedCache for portability.

Avoid caching large objects inline; store IDs or compress.

Use TTL and eviction strategy to avoid stale data.



---

2️⃣ Background Jobs — Hangfire (persistent jobs)

Use case: Run scheduled and fire-and-forget jobs reliably with dashboard.

// Program.cs
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHangfireServer();
app.UseHangfireDashboard("/hangfire");

// Enqueue job
BackgroundJob.Enqueue<IMyJob>(job => job.RunAsync(123));

public interface IMyJob { Task RunAsync(int id); }
public class MyJob : IMyJob
{
    public Task RunAsync(int id) { /* do work */ return Task.CompletedTask; }
}

Key points:

Jobs persist to DB and survive restarts.

Use Hangfire Dashboard with restricted access.

For high throughput use multiple workers and queues.



---

3️⃣ Outbox Pattern (EF Core + Background Publisher)

Use case: Guarantee event delivery (DB + message bus) — avoid lost messages due to partial failures.

// Outbox entity
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Dispatched { get; set; }
}

// During transaction: add domain changes + outbox message
using var tx = await _context.Database.BeginTransactionAsync();
_context.Add(order);
_context.Add(new OutboxMessage { Type="OrderCreated", Payload = JsonSerializer.Serialize(evt), CreatedAt = DateTime.UtcNow });
await _context.SaveChangesAsync();
await tx.CommitAsync();

// Background worker scans Outbox (e.g., Hangfire or BackgroundService)
var pending = _context.OutboxMessages.Where(x => !x.Dispatched).Take(50).ToList();
foreach(var msg in pending) {
    // publish to broker (e.g., Kafka/RabbitMQ)
    Publish(msg);
    msg.Dispatched = true;
}
await _context.SaveChangesAsync();

Key points:

Ensures atomic DB + message persistence.

Use batching and retries for delivery.

Consider idempotency on consumers.



---

4️⃣ CQRS + MediatR (commands, queries, handlers)

Use case: Separate read/write, centralize cross-cutting via pipeline behaviors.

// Install MediatR
// Command
public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;

// Handler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly AppDbContext _db;
    public CreateProductHandler(AppDbContext db) => _db = db;
    public async Task<int> Handle(CreateProductCommand req, CancellationToken ct)
    {
        var p = new Product { Name = req.Name, Price = req.Price };
        _db.Products.Add(p);
        await _db.SaveChangesAsync(ct);
        return p.Id;
    }
}

// Program.cs
builder.Services.AddMediatR(typeof(CreateProductHandler).Assembly);

Key points:

Use pipeline behaviors for logging, validation (FluentValidation), and transaction handling.

Keep queries optimized and use read model projections if necessary.



---

5️⃣ Real-time Notifications — SignalR

Use case: Push realtime updates to clients (dashboard, notifications).

// Hub
public class NotificationsHub : Hub
{
    public async Task SendNotification(string userId, string message)
        => await Clients.User(userId).SendAsync("ReceiveNotification", message);
}

// Program.cs
builder.Services.AddSignalR();
app.MapHub<NotificationsHub>("/hubs/notifications");

Client (JS)

const conn = new signalR.HubConnectionBuilder().withUrl("/hubs/notifications").build();
conn.on("ReceiveNotification", msg => console.log(msg));
await conn.start();

Key points:

Use backplane (Redis) for multi-server scenarios.

Authenticate hub connections; check authorization in Hub methods.



---

6️⃣ Structured Logging — Serilog + Seq / File / ELK

Use case: Rich structured logs for observability (correlation IDs, JSON output).

// Program.cs (top)
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq(builder.Configuration["Seq:Url"])
    .CreateLogger();

builder.Host.UseSerilog();

Usage:

public class MyController : ControllerBase
{
    private readonly ILogger<MyController> _logger;
    public MyController(ILogger<MyController> logger) => _logger = logger;
    public IActionResult Get() { _logger.LogInformation("Fetching items {@Filter}", new { q = "abc" }); return Ok(); }
}

Key points:

Include request and correlation IDs.

Avoid logging PII.

Use sinks compatible with your observability stack (Seq, ELK).



---

7️⃣ Email — SendGrid (recommended for scale) + fallback SMTP

Use case: Transactional email (password reset, notifications).

// Service using SendGrid
public class EmailService : IEmailService
{
    private readonly IConfiguration _cfg;
    public EmailService(IConfiguration cfg) => _cfg = cfg;

    public async Task SendAsync(string to, string subject, string html)
    {
        var client = new SendGrid.SendGridClient(_cfg["SendGrid:ApiKey"]);
        var msg = new SendGrid.Helpers.Mail.SendGridMessage()
        {
            From = new SendGrid.Helpers.Mail.EmailAddress("no-reply@myapp.com", "MyApp"),
            Subject = subject,
            HtmlContent = html
        };
        msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(to));
        await client.SendEmailAsync(msg);
    }
}

Key points:

Use templates and substitution fields for consistency.

Monitor bounce/complaint webhooks.

Keep API key in secrets manager.



---

8️⃣ File Upload to Azure Blob Storage

Use case: Store large files, serve via CDN.

// Program.cs - register client
builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlob")));

// Service
public class BlobFileService
{
    private readonly BlobContainerClient _container;
    public BlobFileService(BlobServiceClient client)
    {
        _container = client.GetBlobContainerClient("uploads");
        _container.CreateIfNotExists();
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        var blob = _container.GetBlobClient(fileName);
        await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "application/octet-stream" });
        return blob.Uri.ToString();
    }
}

Key points:

Use SAS tokens or CDN for secure access.

Validate uploads (size/type) and scan for viruses if needed.

Consider multipart/resumable uploads for large files.



---

9️⃣ Integration Testing — WebApplicationFactory (in-memory server)

Use case: End-to-end tests against real pipeline (controllers, middleware, DI).

public class CustomWebAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace DB context with InMemory or Testcontainers DB
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            services.Remove(descriptor);
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        });
    }
}

// Test example (xUnit)
public class ProductsApiTests : IClassFixture<CustomWebAppFactory<Program>>
{
    private readonly HttpClient _client;
    public ProductsApiTests(CustomWebAppFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetProducts_ReturnsOk() {
        var res = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}

Key points:

Use Testcontainers for realistic DB tests.

Keep tests fast; isolate network/third-party calls with mocks.



---

🔟 Docker + GitHub Actions (build, test, push)

Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MyApp.csproj", "./"]
RUN dotnet restore "./MyApp.csproj"
COPY . .
RUN dotnet publish "MyApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyApp.dll"]

GitHub Actions (ci.yml)

name: CI
on: [push, pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v3
        with: dotnet-version: '7.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore --configuration Release
      - run: dotnet test --no-build --configuration Release
      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          push: true
          tags: user/myapp:${{ github.sha }}

Key points:

Use multi-stage Docker builds for small images.

Run tests in CI before publishing images.

Store registry credentials in GitHub Secrets.



---

If this format looks good I’ll continue with Part 3 covering: observability (distributed tracing), advanced security (OWASP hardening, CSP), API gateways (Ocelot), gRPC, Elasticsearch + Kibana, TestContainers, plus a few complete example projects (clean architecture + CI/CD).

Would you like Part 3 next?

