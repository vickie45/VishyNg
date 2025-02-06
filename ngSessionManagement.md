To manage the logged-in user’s session in Angular 17, you typically use services, local storage/session storage, and JWT tokens for authentication and session management. Here’s a detailed breakdown of how to implement this:

1. Setting Up Authentication

First, you need to authenticate users by validating their credentials (username/password) and issuing a JWT token from the backend (usually a REST API).

Once the user is authenticated, you’ll store the JWT token in the local storage or session storage of the browser for the session duration. You will also need to keep track of the logged-in status, which you can manage via a service.

2. Create an Auth Service

You can create an authentication service that handles user login, logout, and the maintenance of the authentication state.

Example auth.service.ts:

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://your-api.com'; // Your API URL
  private currentUserSubject: BehaviorSubject<any>;
  public currentUser: Observable<any>;

  constructor(private http: HttpClient, private router: Router) {
    this.currentUserSubject = new BehaviorSubject<any>(JSON.parse(localStorage.getItem('currentUser') || '{}'));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  // Method to login and store JWT in local storage
  login(username: string, password: string) {
    return this.http.post<any>(`${this.apiUrl}/login`, { username, password })
      .subscribe(response => {
        if (response && response.token) {
          localStorage.setItem('currentUser', JSON.stringify({ token: response.token }));
          this.currentUserSubject.next({ token: response.token });
          this.router.navigate(['/dashboard']); // Navigate to dashboard
        }
      });
  }

  // Method to logout and clear the session
  logout() {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  // Method to check if the user is logged in
  get isLoggedIn(): boolean {
    return !!this.currentUserSubject.value?.token;
  }
}

3. Login Component

In your login component, you will use the AuthService to handle login actions.

Example login.component.ts:

import { Component } from '@angular/core';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';

  constructor(private authService: AuthService) {}

  onLogin() {
    this.authService.login(this.username, this.password);
  }
}

4. Session Persistence with Local Storage

Local Storage allows you to persist the JWT token between page reloads. It’s key to remember that local storage is persistent even when the browser is closed, whereas session storage is cleared when the browser or tab is closed.

	•	Store Token: After a successful login, store the JWT token in localStorage.

localStorage.setItem('currentUser', JSON.stringify({ token: response.token }));

	•	Retrieve Token: On page load or application start, retrieve the stored token to maintain the session.

const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
if (currentUser?.token) {
  // User is authenticated, continue with session
}

	•	Clear Token: On logout, clear the token to end the session.

localStorage.removeItem('currentUser');

5. Securing Routes (Route Guards)

To prevent unauthorized users from accessing restricted routes, you can use Angular route guards. These guards will check if the user is logged in (i.e., has a valid token).

Example auth.guard.ts:

import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    
    if (this.authService.isLoggedIn) {
      return true;
    } else {
      this.router.navigate(['/login']);
      return false;
    }
  }
}

In your routing module, apply this guard to the routes that need protection.

Example app-routing.module.ts:

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './auth.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

6. API Request Interception with JWT

You can use an HTTP interceptor to automatically add the JWT token to the headers of all outgoing HTTP requests to secure your API.

Example auth.interceptor.ts:

import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const currentUser = this.authService.currentUserValue;
    
    if (currentUser && currentUser.token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      });
    }
    
    return next.handle(req);
  }
}

Don’t forget to provide the interceptor in your module.

Example app.module.ts:

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';

@NgModule({
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
})
export class AppModule { }

7. Logout & Session Expiry

You can handle session expiry by monitoring the token’s validity (e.g., by checking the expiration date) or by forcing the user to log out after a certain period of inactivity.

You can clear the session manually or automatically when the token expires by using a combination of a session timeout timer and token validation on API requests.

This setup ensures your users’ sessions are managed properly in Angular 17, with secure authentication, session persistence, route protection, and automatic token handling for API requests.