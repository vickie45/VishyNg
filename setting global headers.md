To set API call headers for the entire application in Angular, you should use an HTTP interceptor. This is the cleanest and most scalable way to append headers like authorization tokens or custom headers to all outgoing HTTP requests.

⸻

Step-by-Step: Setting API Call Headers Globally in Angular

1. Create an HTTP Interceptor

ng generate service interceptors/auth

2. Implement the Interceptor Logic

auth.interceptor.ts

import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('authToken'); // or get it from a service

    const clonedRequest = req.clone({
      setHeaders: {
        'Authorization': `Bearer ${token}`,
        'Custom-Header': 'MyHeaderValue' // optional: any other headers
      }
    });

    return next.handle(clonedRequest);
  }
}

3. Register the Interceptor in App Module

app.module.ts

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';

@NgModule({
  // ...
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ]
})
export class AppModule { }



⸻

Best Practices
	•	Use multiple interceptors if you need separation of concerns (e.g., error handling, auth).
	•	Don’t hardcode tokens; fetch them from a secure service.
	•	Handle token refresh in another interceptor if needed.

⸻

Let me know if you need the same concept implemented in .NET Core Web API, or want to add conditional logic (like only set headers for certain domains).