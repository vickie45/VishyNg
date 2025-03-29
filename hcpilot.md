✅ Building an Industrial-Standard Health Claim Application in .NET 4.5

⸻

💡 Technology Stack

We’ll use the following technologies:
	•	Backend: ASP.NET MVC 4.5, C#, Web API
	•	Database: SQL Server 2019
	•	ORM: Entity Framework 6 (EF6)
	•	Authentication: JWT with token-based authentication
	•	Frontend: Angular 17 + Bootstrap 5 (for modern UI)
	•	Caching: Redis (optional) for enhanced performance
	•	Deployment: IIS (Internet Information Services)

⸻

📚 1. Solution Architecture

💡 Project Structure

/HealthClaimApp  
 ├── BatchJob            → For background jobs and processing  
 ├── Common              → Shared utilities, constants, and helpers  
 ├── Service             → Business logic layer  
 ├── UI                  → Angular 17 frontend  
 ├── UnitTest            → Unit tests (NUnit or MSTest)  
 ├── Database            → SQL Server database schema  
 ├── API                 → Web API (RESTful endpoints)  
 ├── Models              → Entity models  
 ├── Services            → Business services  
 ├── DAL (Data Access)   → Database layer with EF6  



⸻

🚀 2. Setting Up the Project

(a) Create a New .NET 4.5 MVC Project
	1.	Open Visual Studio → New Project → ASP.NET Web Application (.NET 4.5)
	2.	Choose MVC template → Click Create
	3.	Install NuGet Packages:

Install-Package EntityFramework -Version 6.2.0  
Install-Package Microsoft.AspNet.WebApi  
Install-Package Newtonsoft.Json  
Install-Package System.IdentityModel.Tokens.Jwt  

✅ Explanation:
	•	EntityFramework: ORM for data access
	•	WebApi: RESTful services
	•	Newtonsoft.Json: JSON serialization
	•	Jwt: Token-based authentication

⸻

🔥 3. Database Design (SQL Server)

Create a SQL Server database called HealthClaimDB.

Tables:

✅ User Table

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(150) UNIQUE,
    PasswordHash NVARCHAR(500),
    Role NVARCHAR(50), -- (Admin, User)
    CreatedAt DATETIME DEFAULT GETDATE()
);

✅ Claims Table

CREATE TABLE Claims (
    ClaimId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    ClaimAmount DECIMAL(18,2),
    ClaimType NVARCHAR(100),  -- (Health, Accident, etc.)
    ClaimDate DATETIME,
    Status NVARCHAR(50) DEFAULT 'Pending',
    ApprovedAmount DECIMAL(18,2) NULL,
    Description NVARCHAR(500)
);

✅ Tokens Table (For JWT Revocation)

CREATE TABLE Tokens (
    TokenId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    Token NVARCHAR(500),
    Expiry DATETIME
);



⸻

🔧 4. Data Access Layer (DAL) with EF6

Add EF models and configure the context.

✅ Install EF

Install-Package EntityFramework

✅ Entity Models

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; }

    [Required, MaxLength(100)]
    public string LastName { get; set; }

    [Required, MaxLength(150)]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public string Role { get; set; }  // Admin or User
}

public class Claim
{
    [Key]
    public int ClaimId { get; set; }

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public decimal ClaimAmount { get; set; }

    [MaxLength(100)]
    public string ClaimType { get; set; }

    public DateTime ClaimDate { get; set; }
    public string Status { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string Description { get; set; }
}

✅ DbContext Configuration

using System.Data.Entity;

public class HealthClaimContext : DbContext
{
    public HealthClaimContext() : base("HealthClaimDB") { }

    public DbSet<User> Users { get; set; }
    public DbSet<Claim> Claims { get; set; }
}



⸻

🔥 5. Web API for RESTful Endpoints

Create a Web API controller for managing claims.

✅ User Authentication API

[RoutePrefix("api/auth")]
public class AuthController : ApiController
{
    private readonly HealthClaimContext _db = new HealthClaimContext();

    [HttpPost]
    [Route("register")]
    public IHttpActionResult Register(User user)
    {
        if (_db.Users.Any(x => x.Email == user.Email))
            return BadRequest("User already exists.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        _db.Users.Add(user);
        _db.SaveChanges();
        return Ok("User registered successfully.");
    }

    [HttpPost]
    [Route("login")]
    public IHttpActionResult Login(string email, string password)
    {
        var user = _db.Users.FirstOrDefault(x => x.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Unauthorized();

        var token = JwtHelper.GenerateToken(user);
        return Ok(new { Token = token });
    }
}

✅ JWT Helper

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class JwtHelper
{
    public static string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your_Secret_Key_Here"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "healthclaim.com",
            audience: "healthclaim.com",
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

✅ Claim CRUD API

[RoutePrefix("api/claims")]
public class ClaimsController : ApiController
{
    private readonly HealthClaimContext _db = new HealthClaimContext();

    [HttpGet]
    [Route("{userId}")]
    public IHttpActionResult GetUserClaims(int userId)
    {
        var claims = _db.Claims.Where(c => c.UserId == userId).ToList();
        return Ok(claims);
    }

    [HttpPost]
    [Route("create")]
    public IHttpActionResult CreateClaim(Claim claim)
    {
        _db.Claims.Add(claim);
        _db.SaveChanges();
        return Ok("Claim submitted successfully.");
    }

    [HttpPut]
    [Route("update")]
    public IHttpActionResult UpdateClaim(Claim claim)
    {
        _db.Entry(claim).State = System.Data.Entity.EntityState.Modified;
        _db.SaveChanges();
        return Ok("Claim updated successfully.");
    }
}



⸻

💻 6. Angular 17 Frontend with Bootstrap 5

✅ Install Angular CLI

npm install -g @angular/cli  
ng new health-claim-ui  
cd health-claim-ui  
ng add @angular/material

✅ Create Components

ng generate component login  
ng generate component register  
ng generate component claims  
ng generate service api

✅ Angular API Service

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private apiUrl = 'https://localhost:44356/api';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/login`, { email, password });
  }
}



⸻

🚀 7. Deployment on IIS
	1.	Publish the .NET Project
	2.	Configure IIS with the web application path
	3.	Set SQL Server connection string
	4.	Deploy Angular using ng build --prod

⸻

✅ Final Outcome

You’ll have a full-fledged, industrial-standard health claim management system with:
	•	User authentication
	•	JWT-based security
	•	Efficient API with EF6
	•	Modern Angular 17 UI
	•	SQL Server backend

🔥 Let me know if you need more features, optimizations, or documentation!