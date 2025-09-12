Enterprise-level code recipes — Angular + ASP.NET Core full-stack (reusable patterns)

Below are production-ready, battle-tested recipes you can copy into projects with only minor changes (names, DTOs, claims, routes, config). Each recipe explains purpose, when to use it, gives compact, correct code snippets (Angular + .NET Core), configuration notes, tests to add, and common pitfalls. Use these as a template library in your company repo (/recipes), adapt to conventions (naming, DI container, logging).


---

1) JWT + Refresh Token Authentication (secure, stateless + refresh flow)

Purpose: Secure APIs, short-lived access tokens + long-lived refresh tokens to reduce re-login.

Key ideas

Access token ~ 5–15 minutes; refresh token ~ days/weeks, stored server-side (DB) or as rotating tokens.

Use HttpOnly, Secure cookies for refresh token where possible; for SPAs you can use secure cookie + access token in memory.

Always validate refresh token server-side; support rotation and revocation.


ASP.NET Core (minimal)

// Models
public record AuthRequest(string Username, string Password);
public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

// Controller
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwt;
    private readonly IUserService _users;
    public AuthController(IJwtService jwt, IUserService users) { _jwt=jwt; _users=users; }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthRequest req) {
        var user = await _users.ValidatePassword(req.Username, req.Password);
        if(user==null) return Unauthorized();
        var access = _jwt.CreateAccessToken(user);
        var refresh = await _jwt.CreateAndStoreRefreshToken(user.Id);
        // set HttpOnly cookie (example)
        Response.Cookies.Append("refreshToken", refresh.Token, new CookieOptions { HttpOnly=true, Secure=true, SameSite=SameSiteMode.Strict, Expires=refresh.ExpiresAt });
        return Ok(new AuthResponse(access.Token, refresh.Token, access.ExpiresAt));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh() {
        if(!Request.Cookies.TryGetValue("refreshToken", out var token)) return Unauthorized();
        var result = await _jwt.RotateRefreshToken(token, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
        if(result==null) return Unauthorized();
        Response.Cookies.Append("refreshToken", result.NewRefresh.Token, new CookieOptions { HttpOnly=true, Secure=true, Expires=result.NewRefresh.ExpiresAt });
        return Ok(new { accessToken = result.NewAccessToken, expiresAt = result.NewAccessExpiresAt });
    }
}

Notes:

Implement IJwtService to create/signed JWTs (using System.IdentityModel.Tokens.Jwt) and store refresh tokens in DB with userId, createdByIp, revoked flag, replacedBy token, expiry.

Revoke on logout/compromise.


Angular: Auth service + interceptor + guard

// auth.service.ts (simplified)
@Injectable({providedIn:'root'})
export class AuthService {
  private accessToken = new BehaviorSubject<string | null>(null);
  constructor(private http: HttpClient) {}
  login(username:string, password:string){ return this.http.post('/api/auth/login', {username,password}).pipe(tap((r:any) => this.accessToken.next(r.accessToken))); }
  logout(){ return this.http.post('/api/auth/logout', {}).pipe(tap(()=>this.accessToken.next(null))); }
  getAccessToken(){ return this.accessToken.value; }
  refresh() { return this.http.post('/api/auth/refresh', {}); } // cookie carries refresh token
}

// interceptor
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private auth:AuthService, private injector:Injector) {}
  intercept(req: HttpRequest<any>, next: HttpHandler){
    const token = this.auth.getAccessToken();
    let modified = req;
    if(token) modified = req.clone({ setHeaders: { Authorization: `Bearer ${token}` }});
    return next.handle(modified).pipe(catchError(err => {
      if(err.status === 401){
        // attempt refresh synchronously (simple pattern)
        return this.auth.refresh().pipe(
          switchMap((r:any) => {
            this.auth.login; // or set token from response
            const t = r.accessToken;
            const retry = req.clone({ setHeaders: { Authorization: `Bearer ${t}` }});
            return next.handle(retry);
          }),
          catchError(_ => { /* navigate to login */ return throwError(()=>_); })
        );
      }
      return throwError(()=>err);
    }));
  }
}

Tests: unit-test refresh flow and rotation, simulate revoked token.


---

2) Centralized Error Handling & Problem Details

Purpose: Consistent JSON error shape, automatic logging, correct HTTP codes.

ASP.NET Core: Middleware

public class ErrorHandlingMiddleware {
  private readonly RequestDelegate _next; private readonly ILogger _log;
  public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> log) { _next=next; _log=log; }
  public async Task Invoke(HttpContext ctx) {
    try { await _next(ctx); }
    catch(Exception ex) {
      _log.LogError(ex, "Unhandled");
      ctx.Response.ContentType = "application/problem+json";
      ctx.Response.StatusCode = ex is ValidationException ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
      var problem = new { type = "about:blank", title = ex.Message, status = ctx.Response.StatusCode, detail = ex is ValidationException v? v.Errors : null };
      await ctx.Response.WriteAsJsonAsync(problem);
    }
  }
}
// Register in Program.cs
app.UseMiddleware<ErrorHandlingMiddleware>();

Angular: Global HTTP error handler to map problem+json to user-friendly UI messages.


---

3) Layered Architecture: Controllers → Services → Repositories → EF Core

Purpose: Separation of concerns, testability, clean DI.

Pattern (C#)

Controllers do validation (ModelState) and return DTOs.

Services contain business logic & orchestrate repositories.

Repositories handle EF queries and are interface-backed for mocking.

UnitOfWork to commit transactions across repos if needed.


public interface IRepository<T> where T:class { Task<T?> GetAsync(int id); IQueryable<T> Query(); /* Add/Update/Delete */ }
public class EfRepository<T> : IRepository<T> where T: class {
  protected readonly DbContext _db;
  public EfRepository(DbContext db){_db=db;}
  public Task<T?> GetAsync(int id) => _db.Set<T>().FindAsync(id).AsTask();
  public IQueryable<T> Query() => _db.Set<T>();
}

Notes: Keep services focused; controllers thin. Use DTOs (see next).


---

4) DTOs, AutoMapper, and Request/Response Models

Purpose: Avoid over-posting, versioning-friendly, stable contracts.

// DTO
public class PatientDto { public int Id {get;set;} public string FullName {get;set;} }

// AutoMapper profile
public class MappingProfile : Profile {
  public MappingProfile() { CreateMap<Patient, PatientDto>(); CreateMap<CreatePatientRequest, Patient>(); }
}

Angular: Mirror DTOs in TS interfaces. Use codegen (NSwag/Swashbuckle) to generate client models where possible.


---

5) Pagination, Sorting & Filtering (server-driven)

Purpose: Standardize list endpoints.

Pattern (C#)

public class PagedResult<T> { public int Page {get;set;} public int PageSize {get;set;} public int Total {get;set;} public IEnumerable<T> Items {get;set;} }
public async Task<PagedResult<TDto>> GetPagedAsync<T, TDto>(IQueryable<T> q, int page, int size, IMapper m) {
  var total = await q.CountAsync();
  var items = await q.Skip((page-1)*size).Take(size).ToListAsync();
  return new PagedResult<TDto>{ Page=page, PageSize=size, Total=total, Items = items.Select(i=>m.Map<TDto>(i)).ToList() };
}

Angular: Standard PagedService to request /api/resource?page=1&pageSize=20&sort=-created and an abstract PagedListComponent for reuse.


---

6) File Uploads (chunked + virus-scan friendly)

Purpose: Large files, resumable/chunked uploads, metadata.

Recipe

Use signed upload URLs (S3/Azure Blob) for direct client → cloud.

For server-side flow (if required): accept multipart, stream to disk/cloud, validate mime/type/size, scan with antivirus (clamAV) asynchronously.


ASP.NET Core: Use IFormFile streaming and RequestSizeLimit. Angular: Use chunked upload service with Content-Range header and resume support.


---

7) Caching Strategy (distributed)

Purpose: Reduce DB load; consistent across multiple instances.

Pattern

Use IMemoryCache for single-instance ephemeral caches.

Use Redis for distributed caching and locks.

Cache invalidation: time-based + event-driven (publish cache invalidation messages).


ASP.NET Core snippet

// ISomeService with Redis cache
public async Task<MyDto> GetItem(int id) {
  var key = $"item:{id}";
  var cached = await _distributedCache.GetStringAsync(key);
  if(cached != null) return JsonConvert.DeserializeObject<MyDto>(cached);
  var item = await _repo.GetAsync(id);
  await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(item), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow=TimeSpan.FromMinutes(5)});
  return item;
}

Angular: Use in-memory caching service (BehaviorSubject) and caching headers (Cache-Control, ETag) support.


---

8) Background Jobs & Scheduling

Purpose: Offload heavy tasks (report generation, emails, file processing).

Options: Hangfire (easy), Quartz.NET, or Azure Functions/AWS Lambda/Worker Service.

Hangfire quick-start

// Program.cs
builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<IMyService>(s => s.ProcessQueue(), Cron.Hourly);

Notes: Use durable queues for retries, poison queue handling.


---

9) Health Checks, Metrics, and Observability

Purpose: Readiness, liveness, telemetry, and alerting.

Use Microsoft.Diagnostics.HealthChecks and push metrics to Prometheus (via exporter) or Application Insights.

Add /health/live and /health/ready.

Centralized structured logging with Serilog; ship logs to ELK/Datadog.


ASP.NET Core

services.AddHealthChecks().AddDbContextCheck<AppDbContext>("db").AddUrlGroup(new Uri("https://external-service/health"), name:"external");
app.MapHealthChecks("/health/ready", new HealthCheckOptions { /* map result to json */ });


---

10) API Versioning & OpenAPI

Purpose: Evolve contracts safely.

Use Microsoft.AspNetCore.Mvc.Versioning and route versioning (api/v{version:apiVersion}/resource).

Keep backward-compatible behaviors; deprecate with headers.


Swagger/NSwag: Expose per-version Swagger docs and generated typed clients for Angular (nswag generates Angular services and DTOs).


---

11) Rate Limiting & Throttling

Purpose: Protect from abuse.

Use middleware-based rate limiting (ASP.NET Core RateLimiting package) with policies by IP, endpoint, or user.

Consider distributed counters via Redis for multi-instance.



---

12) Feature Flags

Purpose: Gradual rollouts, A/B testing.

Use Microsoft.FeatureManagement or LaunchDarkly.

Implement server-side feature checks; keep flags in config store/DB and cache.



---

13) Localization & Globalization

Purpose: Support multiple locales and currency/date formats.

Use .resx resources on server, Angular @angular/localize (i18n) or ngx-translate for runtime translation.

Provide locale negotiation middleware.



---

14) Secure-by-default (OWASP basics)

Checklist

Use HTTPS everywhere; HSTS.

Set security headers (CSP, X-Frame-Options).

Validate inputs (server-side).

Parameterized queries / EF Core to avoid SQL injection.

Limit file types & sizes; scan uploads.

Enforce CORS policy to trusted origins only.

Store secrets in vaults (Azure Key Vault, AWS Secrets Manager) — not in source.

Protect against XSS: encode output, use Angular built-in sanitization.

Rotate keys/tokens and set short lifetimes.



---

15) CI/CD + Infrastructure as Code

Essentials

Build pipelines: dotnet restore && dotnet build && dotnet test && dotnet publish.

Linting & formatting (ESLint/Prettier for Angular).

Use container images (multi-stage Dockerfiles) and push to registry.

Deploy via Kubernetes (Helm charts) or cloud app service; use blue/green or canary deployments.

Migrations applied during deploy via dotnet ef database update in a controlled step.



---

16) Testing Pyramid

What to include

Unit tests: services & controllers (mock repos).

Integration tests: use WebApplicationFactory/in-memory DB (SQLite in-memory or testcontainers).

Contract tests: verify API contracts (Pact).

E2E tests: Cypress or Playwright for Angular UI flows.

Static analysis: SonarCloud/ESLint, .NET analyzers.


Example unit test (xUnit + Moq)

[Fact]
public async Task GetPatient_ReturnsDto() {
  var repo = new Mock<IRepository<Patient>>();
  repo.Setup(r=>r.GetAsync(1)).ReturnsAsync(new Patient{Id=1,FirstName="V"});
  var svc = new PatientService(repo.Object, /*mapper*/);
  var dto = await svc.Get(1);
  Assert.Equal(1, dto.Id);
}


---

17) Angular Architecture Recipes

Project structure (recommended)

src/
  app/
    core/        // singletons: http, auth, interceptors, services used app-wide
    features/    // feature modules (lazy loaded)
      patients/
        components/
        services/
        patients-routing.module.ts
    shared/      // reusable pipes, components, models
    store/       // NgRx modules (if used)
    app-routing.module.ts

Lazy-loaded feature module example

@NgModule({
  imports: [CommonModule, RouterModule.forChild([{path:'', component: PatientsListComponent}]), SharedModule],
  declarations:[PatientsListComponent]
})
export class PatientsModule {}

HTTP Interceptor pattern (retries, error mapping)

Add exponential backoff for retries on 429/5xx.

Central place for attaching auth header and logging.


State management

For complex apps use NgRx (actions, reducers, effects) or keep with services + BehaviorSubjects for smaller domains.



---

18) Logging & Correlation IDs

Purpose: Trace requests across services.

Generate a correlation ID at middleware entry (or accept X-Correlation-ID header), attach it to logs and responses.

Add same ID in Angular XHR headers for tracing.


Middleware (C#)

app.Use(async (ctx,next) => {
  var id = ctx.Request.Headers.ContainsKey("X-Correlation-ID") ? ctx.Request.Headers["X-Correlation-ID"].ToString() : Guid.NewGuid().ToString();
  using(LogContext.PushProperty("CorrelationId", id)) {
    ctx.Response.Headers.Add("X-Correlation-ID", id);
    await next();
  }
});


---

19) Database Migrations & Seed Data

Pattern

Use EF Core migrations with scripted release pipelines.

Maintain idempotent seed scripts for initial data.

Keep production migrations reviewed and small.



---

20) Boilerplate: Minimal reusable code snippets

Generic ApiResponse wrapper

public class ApiResponse<T> {
  public bool Success {get;set;} = true;
  public T? Data {get;set;}
  public IEnumerable<string>? Errors {get;set;}
}

Angular service base

export abstract class ApiService {
  protected base = '/api';
  constructor(protected http:HttpClient) {}
  protected get<T>(path:string, params?:HttpParams){ return this.http.get<ApiResponse<T>>(`${this.base}${path}`, { params }); }
}


---

Final notes & recommended repo layout

Create a git repo enterprise-recipes with:

dotnet/ folder: template projects, middleware, common libs (Auth, Jwt, ErrorHandling, Repositories).

angular/ folder: starter app, shared libs (interceptor, auth, services, models), sample feature modules, Cypress tests.

docs/ with recipes explained, code snippets, and a checklist for production readiness.



---

If you'd like, I can:

generate a starter repo scaffold (file list + sample files) for both Angular 17 + ASP.NET Core (with Dockerfile & GitHub Actions), or

produce a detailed implementation of one recipe (e.g., full JWT + refresh token implementation with DB schema, unit tests, Angular login flow and interceptor) ready to paste into your project.


Which of the two do you want me to produce now?

