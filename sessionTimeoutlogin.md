To implement session timeout in Angular 17, especially for a logged-in user, you can follow a structured approach using RxJS timers, Angular services, and interceptors. Here’s a practical, production-ready implementation:

⸻

Objectives
	•	Auto-logout user after X minutes of inactivity (e.g., 15 mins).
	•	Reset the timer on user activity (mouse, keyboard, etc.).
	•	Optional: Show a warning before logout.

⸻

Step-by-Step Implementation

1. Session Timeout Service

// session-timeout.service.ts
import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';
import { fromEvent, merge, Subscription, timer } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SessionTimeoutService {
  private userActivityEvents$: Subscription | null = null;
  private timerSubscription: Subscription | null = null;
  private readonly TIMEOUT = 15 * 60 * 1000; // 15 minutes in milliseconds

  constructor(private router: Router, private ngZone: NgZone) {}

  startMonitoring() {
    this.ngZone.runOutsideAngular(() => {
      const activityEvents = merge(
        fromEvent(window, 'mousemove'),
        fromEvent(window, 'keydown'),
        fromEvent(window, 'click'),
        fromEvent(window, 'scroll')
      );

      this.userActivityEvents$ = activityEvents.subscribe(() => {
        this.resetTimer();
      });

      this.resetTimer(); // Start initially
    });
  }

  stopMonitoring() {
    this.userActivityEvents$?.unsubscribe();
    this.timerSubscription?.unsubscribe();
  }

  private resetTimer() {
    this.timerSubscription?.unsubscribe();

    this.timerSubscription = timer(this.TIMEOUT).subscribe(() => {
      this.ngZone.run(() => {
        alert('Session expired due to inactivity.');
        this.logout();
      });
    });
  }

  private logout() {
    // Clear tokens or session info from storage
    localStorage.removeItem('authToken');
    // Redirect to login
    this.router.navigate(['/login']);
  }
}



⸻

2. Call the Service in AppComponent

// app.component.ts
import { Component, OnInit } from '@angular/core';
import { SessionTimeoutService } from './services/session-timeout.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  constructor(private sessionTimeoutService: SessionTimeoutService) {}

  ngOnInit() {
    const token = localStorage.getItem('authToken');
    if (token) {
      this.sessionTimeoutService.startMonitoring();
    }
  }
}



⸻

3. Token Handling in Auth Interceptor (Optional)

To ensure session expiration works in sync with token validity:

// auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('authToken');
    if (token) {
      const cloned = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      return next.handle(cloned);
    }
    return next.handle(req);
  }
}



⸻

4. Optional Enhancements
	•	Show a modal warning before actual logout.
	•	Reset session on manual logout or token refresh.
	•	Store last active timestamp in localStorage for multi-tab sync.

⸻

Would You Like?
	•	A version with modal countdown before logout?
	•	Support for multi-tab session handling?
	•	Integration with .NET backend for token expiration check?

Let me know, and I’ll help you customize it accordingly.