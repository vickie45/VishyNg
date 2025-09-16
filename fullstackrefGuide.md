Angular + .NET Core + SQL Server — Enterprise Reference Guide

> A production-ready, practical reference for building enterprise-grade full‑stack applications using Angular (frontend), ASP.NET Core Web API (backend), and SQL Server (database). Includes folder structures, configuration, code examples, security, testing, CI/CD, monitoring, and best practices.




---

Table of Contents

1. Overview & Architecture


2. Enterprise folder structures


3. Development environment & tooling


4. Angular (Frontend)

Project setup (Angular 17)

App architecture & modules

Services & HttpClient patterns

Authentication & Authorization (JWT + refresh)

Interceptors, error handling, global state

Forms (Reactive), validation, dynamic forms

File upload, previews, chunked upload

Lazy loading, route guards

Accessibility & i18n

Unit & E2E testing (Jest + Cypress)

Build & deployment (AOT, optimization)



5. ASP.NET Core Web API (Backend)

Project setup (recommended .NET 8)

Clean architecture (Presentation / Application / Domain / Infrastructure)

Configuration, secrets, environment-based settings

Authentication (JWT + refresh tokens) and Authorization (policies, roles)

DTOs, AutoMapper, validation (FluentValidation)

EF Core patterns, migrations, performance

Services, dependency injection, background jobs

Versioning, API documentation (Swagger/OpenAPI)

Error handling, logging, structured logs

Health checks, metrics, telemetry



6. SQL Server

Schema design, indexing, constraints

Stored procedures, parameterization, execution patterns

Migrations strategy vs manual scripts

Backup, recovery, maintenance, performance tuning

Security: least privilege, encryption, row-level security



7. Cross-cutting concerns

Security checklist

Performance & caching strategies

Observability: logging, tracing, metrics

CI/CD pipelines (Azure DevOps/GitHub Actions)

Containerization & deployment (Docker, Kubernetes)

Secrets management & key rotation



8. Example end-to-end: User Registration + Login + Profile + File Upload

Angular code snippets

.NET WebAPI controllers, services, EF entities

SQL scripts



9. Checklists: Pre-release & Production readiness


10. Appendix: useful code snippets & templates




---

1. Overview & Architecture

A typical enterprise stack splits responsibilities:

Frontend (Angular): UI, client-side validation, routing, progressive enhancement.

Backend (ASP.NET Core Web API): business logic, data access, auth, integration.

Database (SQL Server): persistent storage, complex queries, reporting.


Architecture goals:

Separation of concerns (clean architecture)

Testability and automation

Security by default

Observability and maintainability


Recommended high-level architecture: Client ⇄ API Gateway / Backend API(s) ⇄ SQL Server


---

2. Enterprise folder structures

Angular (monorepo-friendly)

/apps
  /portal-app (angular project)
/libs
  /ui (shared components)
  /data (shared services & models)
  /utils
  /auth
/tools
/angular.json
/package.json
/tsconfig.base.json
/README.md

ASP.NET Core (Clean Architecture)

/src
  /MyApp.Api            -> Web / Controllers
  /MyApp.Application    -> DTOs, Interfaces, UseCases
  /MyApp.Domain         -> Entities, ValueObjects, Domain Services
  /MyApp.Infrastructure -> EF Core, Repositories, External Integrations
/tests
  /MyApp.UnitTests
  /MyApp.IntegrationTests
/docker-compose.yml
/README.md


---

3. Development environment & tooling

Recommended tools:

Node LTS (for Angular), npm or pnpm

Angular CLI ^17

.NET SDK (recommended latest LTS or current stable: .NET 8)

SQL Server Developer / Docker image

IDEs: VSCode (frontend) + Visual Studio / Rider (backend)

Docker, Helm (for k8s), Git

Linters & formatters: ESLint, Prettier, EditorConfig

Code analyzers: SonarQube/Cloud, Roslyn analyzers


Editor config and shared linting allow consistent code style across teams.


---

4. Angular (Frontend)

Project setup (Angular 17)

npm create @angular/workspace@latest my-org --interactive
cd my-org
# or: ng new portal-app --routing --style=scss

Install recommended deps:

npm i @ngrx/store @ngrx/effects @ngrx/router-store
npm i rxjs-interop
npm i @ng-bootstrap/ng-bootstrap
npm i @ngx-translate/core @ngx-translate/http-loader

Use NgModules sparingly; prefer standalone components where appropriate in Angular 17.

App architecture & modules

CoreModule: singletons (AuthService, HttpService)

SharedModule: shared components, pipes

Feature modules: lazy-loaded

Routing module per feature


Example: app-routing.module.ts

import { Routes } from '@angular/router';
export const routes: Routes = [
  { path: '', loadChildren: () => import('./features/home/home.routes') },
  { path: 'auth', loadChildren: () => import('./features/auth/auth.routes') },
  { path: '**', redirectTo: '' }
];

Services & HttpClient patterns

Use typed HttpClient with generics

Centralize API base URL and version


api.service.ts

@Injectable({providedIn:'root'})
export class ApiService {
  constructor(private http: HttpClient) {}
  get<T>(url: string, params?: HttpParams) { return this.http.get<T>(url, { params }); }
  post<T, R>(url: string, body: T) { return this.http.post<R>(url, body); }
}

Authentication & Authorization (JWT + refresh)

Store access token in memory or secure storage (avoid localStorage for access tokens if possible)

Use HTTP-only cookies for refresh token (recommended) or secure storage mechanism

Implement refresh token flow via interceptor to retry original request


auth.interceptor.ts (simplified)

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService) {}
  intercept(req, next) {
    const token = this.auth.getAccessToken();
    let cloned = req;
    if (token) cloned = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
    return next.handle(cloned).pipe(
      catchError(err => {
        if (err.status === 401) {
          return this.auth.refreshToken().pipe(
            switchMap(() => next.handle(req))
          );
        }
        return throwError(() => err);
      })
    );
  }
}

Interceptors, error handling, global state

Interceptors: attach tokens, log requests, capture metrics, global error mapping

Global error handler to show friendly messages

Use RxJS operators (retry, exponentialBackoff) for transient faults


Forms (Reactive), validation, dynamic forms

Use ReactiveForms with strong typing

Extract custom validators into reusable functions


Example: reactive form

this.form = this.fb.group({
  name: ['', [Validators.required, Validators.maxLength(100)]],
  email: ['', [Validators.required, Validators.email]]
});

Dynamic forms: build FormGroup from metadata coming from backend when needed.

File upload, previews, chunked upload

Client: preview images via URL.createObjectURL for images

Use chunked upload for large files; resume capability recommended

Use Content-Range / custom multipart endpoints


simple file upload

const fd = new FormData();
fd.append('file', file, file.name);
this.api.post('/files', fd);

Lazy loading, route guards

Protect routes with AuthGuard and RoleGuard

Use preloading strategy for frequently used modules


Accessibility & i18n

Semantic HTML, ARIA labels, keyboard navigation

Use @ngx-translate for runtime translations or Angular i18n for compile-time


Unit & E2E testing

Unit tests: Jest with @angular-builders/jest for speed

E2E: Cypress for real browser flows

Keep tests deterministic; mock external calls


Build & deployment

Use AOT, build optimizer, differential loading not needed post-modern browsers

Generate source maps for staging; disable in production or upload to Sentry

Environment files: environment.prod.ts holds endpoints and flags



---

5. ASP.NET Core Web API (Backend)

> Recommended: use .NET 8 or latest stable. Examples use C# 12 syntax where applicable.



Project setup (Clean Architecture)

dotnet new sln -n MyApp
cd MyApp
dotnet new webapi -o src/MyApp.Api
dotnet new classlib -o src/MyApp.Domain
dotnet new classlib -o src/MyApp.Application
dotnet new classlib -o src/MyApp.Infrastructure
# add projects to solution and set references

Program.cs (minimal hosting)

var builder = WebApplication.CreateBuilder(args);
// Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add DB, DI, etc.
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

Dependency injection & separation

Define repository/service interfaces in Application or Domain, implement in Infrastructure.

Use AddScoped, AddSingleton and AddTransient appropriately.


Authentication (JWT + refresh tokens)

Use Identity or a lightweight custom user store depending on needs. For enterprise, integrating with IdentityServer/Keycloak/AD often preferred.


JWT setup

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters { /*validate issuer, audience, signing key*/ };
  });

Refresh tokens: store refresh tokens hashed in DB, rotate on use, revoke on logout.

DTOs, AutoMapper, validation

Use DTOs for all external contracts. Avoid exposing EF entities.

Use AutoMapper for mapping DTO ⇄ Entity.

Use FluentValidation for request validation.


Controller sample

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
  private readonly IAuthService _auth;
  public AuthController(IAuthService auth) { _auth = auth; }
  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginRequest req) {
    var res = await _auth.LoginAsync(req.Email, req.Password);
    if (!res.Succeeded) return Unauthorized();
    return Ok(res);
  }
}

EF Core patterns, migrations, performance

Use DbContext per logical bounded-context. Keep contexts small.

Use explicit transactions for multi-step operations.

For read-heavy endpoints, use optimized SQL queries or read replicas.

Use AsNoTracking() for read-only queries.


Sample repository pattern (simple)

public interface IUserRepository { Task<User> GetByIdAsync(Guid id); }
public class UserRepository : IUserRepository {
  private readonly AppDbContext _db;
  public UserRepository(AppDbContext db) { _db = db; }
  public Task<User> GetByIdAsync(Guid id) => _db.Users.FirstOrDefaultAsync(u => u.Id == id);
}

Background jobs

Use IHostedService for long-running background workers or Hangfire/Quartz for scheduled tasks.


Versioning & Swagger

Use URL versioning api/v1/... and Swagger document per version.


Error handling & logging

Global ExceptionMiddleware to return consistent error shape

Use structured logging (Serilog) with enrichers for request id, user id


Exception middleware pattern

app.UseMiddleware<ExceptionHandlingMiddleware>();

Health checks & telemetry

Add AddHealthChecks() and expose endpoints for readiness & liveness

Use OpenTelemetry for traces and metrics



---

6. SQL Server

Schema design

Normalize up to a sensible point; denormalize for performance when needed

Use surrogate GUIDs or BIGINT IDs depending on scale

Enforce constraints, use proper datatypes


Index strategy

Covering indexes for frequent queries

Avoid over-indexing; monitor index usage


Stored procedures

Use parameterized stored procedures for heavy data operations

Keep SPs deterministic and side-effect conscious


Example: create user SP

CREATE PROCEDURE dbo.CreateUser
  @Id UNIQUEIDENTIFIER,
  @Email NVARCHAR(256),
  @PasswordHash VARBINARY(MAX)
AS
BEGIN
  SET NOCOUNT ON;
  INSERT INTO dbo.Users (Id, Email, PasswordHash) VALUES (@Id, @Email, @PasswordHash);
END

Migrations strategy

Use EF Core migrations for schema changes where feasible.

For complex changes, use manual scripts reviewed by DBAs; store in git and run via CI.


Security

Use contained database users with least privilege

Use Transparent Data Encryption (TDE) for at-rest encryption if required

Use Always Encrypted for sensitive columns



---

7. Cross-cutting concerns

Security checklist (essentials)

Validate and sanitize all inputs

Use parameterized queries / ORM parameters

Rate limit sensitive endpoints

Use CSP, X-Frame-Options, secure cookies, HSTS

Regular dependency scanning and vulnerability management


Caching & performance

Use Redis for distributed caching and session store

Use CDN for static frontend assets

Client-side caching headers (ETag/Last-Modified)


Observability

Use Serilog + Seq/ELK for logs

Use OpenTelemetry to export traces to Jaeger/Tempo

Instrument metrics (Prometheus)


CI/CD

Build steps: lint → unit tests → build → integration tests → publish artifact → deploy to staging → smoke tests → deploy to production

Use Infrastructure as Code (Terraform/ARM/Bicep)

Blue/green or canary deployments for minimal downtime


Containerization

Multi-stage Docker builds

Keep images small, use distroless or minimal base images



---

8. Example: End-to-end (User Registration, Login, Profile, File Upload)

> This section gives a condensed end-to-end example.



SQL: Users table

CREATE TABLE dbo.Users (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  Email NVARCHAR(256) NOT NULL UNIQUE,
  PasswordHash VARBINARY(MAX) NOT NULL,
  DisplayName NVARCHAR(200),
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

EF Entity (Domain)

public class User {
  public Guid Id {get; set;} = Guid.NewGuid();
  public string Email {get; set;} = null!;
  public byte[] PasswordHash {get; set;} = null!;
  public string? DisplayName {get; set;}
}

Application DTOs

public record RegisterRequest(string Email, string Password, string? DisplayName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string AccessToken, string RefreshToken);

Controller (Auth)

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase {
  private readonly IAuthService _auth;
  public AuthController(IAuthService auth) { _auth = auth; }

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterRequest req) {
    var res = await _auth.RegisterAsync(req);
    if (!res.Succeeded) return BadRequest(res.Errors);
    return Ok();
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginRequest req) {
    var res = await _auth.LoginAsync(req);
    if (!res.Succeeded) return Unauthorized();
    return Ok(new AuthResponse(res.AccessToken, res.RefreshToken));
  }
}

Angular: AuthService (simplified)

@Injectable({providedIn: 'root'})
export class AuthService {
  private token?: string;
  constructor(private api: ApiService) {}
  login(email: string, password: string) {
    return this.api.post<LoginReq, AuthResp>('/api/v1/auth/login', { email, password }).pipe(
      tap(res => { this.token = res.accessToken; /* store refresh cookie handled server side */ })
    );
  }
  getAccessToken() { return this.token; }
}

File upload (controller)

[HttpPost("me/avatar")]
[Authorize]
public async Task<IActionResult> UploadAvatar(IFormFile file) {
  if (file == null) return BadRequest("file missing");
  var path = await _fileService.StoreUserFileAsync(UserId, file);
  return Ok(new { url = path });
}

Angular file upload (component)

onFileSelected(e: Event) {
  const input = e.target as HTMLInputElement;
  const file = input.files?.[0];
  if (!file) return;
  const fd = new FormData();
  fd.append('file', file);
  this.api.post<FormData, UploadResp>('/api/v1/users/me/avatar', fd).subscribe(r => console.log(r));
}


---

9. Checklists: Pre-release & Production readiness

Pre-release checklist

Security scan: SAST, dependency scanners

Unit, integration, E2E tests passing

Load test on key endpoints

Backups and rollback steps defined

Monitoring & alerts configured


Production checklist

TLS enforced, secure cookies

Secrets in vault

Autoscaling and resource limits

Disaster recovery runbook

Audit logging enabled



---

10. Appendix: useful snippets & templates

Dockerfile examples for Angular and .NET

GitHub Actions pipeline template

SQL scripts for migrations and seeding

Terraform templates for Azure / AWS



---

Final notes

This guide is a practical starting point and should be adapted to your organization's policies and scale. For complex enterprise needs (SSO, multi-tenant data isolation, compliance), integrate with specialized identity providers (Keycloak, Azure AD), database specialists, and security teams.


---

If you want, I can convert this into a printable PDF, a multi-file repo skeleton, or expand any section (e.g., detailed CI pipelines, Docker + Kubernetes manifests, or a complete end-to-end sample project).

