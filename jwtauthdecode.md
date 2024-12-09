JWT (JSON Web Token) authentication is a widely used method for handling secure data exchange between clients and servers. Here’s how you can implement JWT authentication and decoding in Angular:

1. Install Required Packages

Install the @auth0/angular-jwt package to handle JWTs in Angular:

npm install @auth0/angular-jwt

2. Configure JWT Interceptor

Use the JwtModule to automatically attach tokens to HTTP requests.

app.module.ts

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';

import { AppComponent } from './app.component';

export function tokenGetter() {
  return localStorage.getItem('access_token'); // Fetch the token from storage
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ['api.example.com'], // Whitelist the API domains
        disallowedRoutes: ['api.example.com/auth/login'], // Avoid attaching token to these routes
      },
    }),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

3. Handle Login and Token Storage

auth.service.ts

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://api.example.com/auth'; // Replace with your API URL

  constructor(private http: HttpClient) {}

  login(credentials: { username: string; password: string }) {
    return this.http.post<{ access_token: string }>(`${this.apiUrl}/login`, credentials);
  }

  saveToken(token: string) {
    localStorage.setItem('access_token', token);
  }

  logout() {
    localStorage.removeItem('access_token');
  }

  getToken() {
    return localStorage.getItem('access_token');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token; // Check if token exists
  }
}

4. Decode JWT

You can decode a JWT using the @auth0/angular-jwt library.

jwt-decoder.service.ts

import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root',
})
export class JwtDecoderService {
  constructor(private jwtHelper: JwtHelperService) {}

  decodeToken(token: string): any {
    return this.jwtHelper.decodeToken(token); // Decodes the JWT payload
  }

  isTokenExpired(token: string): boolean {
    return this.jwtHelper.isTokenExpired(token); // Checks if token is expired
  }

  getTokenExpirationDate(token: string): Date | null {
    return this.jwtHelper.getTokenExpirationDate(token); // Returns the expiration date
  }
}

5. Protect Routes with Guards

Use an Angular Guard to protect routes based on token authentication.

auth.guard.ts

import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    }
    this.router.navigate(['/login']);
    return false;
  }
}

6. Use Interceptors to Attach Tokens

Add an HTTP interceptor to attach the JWT token to API requests.

auth.interceptor.ts

import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = localStorage.getItem('access_token');
    if (token) {
      const cloned = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`),
      });
      return next.handle(cloned);
    }
    return next.handle(req);
  }
}

app.module.ts

providers: [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
];

7. Full Flow in a Component

login.component.ts

import { Component } from '@angular/core';
import { AuthService } from './auth.service';
import { JwtDecoderService } from './jwt-decoder.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})
export class LoginComponent {
  credentials = { username: '', password: '' };

  constructor(
    private authService: AuthService,
    private jwtDecoder: JwtDecoderService,
    private router: Router
  ) {}

  login() {
    this.authService.login(this.credentials).subscribe((response) => {
      this.authService.saveToken(response.access_token);

      const decodedToken = this.jwtDecoder.decodeToken(response.access_token);
      console.log('Decoded Token:', decodedToken);

      this.router.navigate(['/dashboard']);
    });
  }
}

8. Decode Token in Dashboard

dashboard.component.ts

import { Component, OnInit } from '@angular/core';
import { JwtDecoderService } from './jwt-decoder.service';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  user: any;

  constructor(
    private jwtDecoder: JwtDecoderService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    const token = this.authService.getToken();
    if (token) {
      this.user = this.jwtDecoder.decodeToken(token);
    }
  }
}

Final Notes

	1.	Ensure your backend implements secure JWT token generation and validation.
	2.	Use HTTPS to avoid token interception.
	3.	Use refresh tokens for longer sessions.