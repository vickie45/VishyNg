To add a header to an API request in Angular, you typically use Angular’s HttpClient service from the @angular/common/http module.

Here is a step-by-step and practical example:

⸻

✅ Step 1: Import HttpClient and HttpHeaders

Make sure HttpClientModule is imported in your root or feature module:

// app.module.ts
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  imports: [
    HttpClientModule,
    // other imports
  ]
})
export class AppModule {}


⸻

✅ Step 2: Use HttpHeaders in Your Service

// my-api.service.ts
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MyApiService {

  constructor(private http: HttpClient) {}

  getData() {
    const url = 'https://api.example.com/data';

    const headers = new HttpHeaders({
      'Authorization': 'Bearer your-token-here',
      'Custom-Header': 'custom-value'
    });

    return this.http.get(url, { headers });
  }

  postData(body: any) {
    const url = 'https://api.example.com/post';

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer your-token-here'
    });

    return this.http.post(url, body, { headers });
  }
}


⸻

✅ Step 3 (Optional): Set Headers Interceptor-Wide

If you want to add headers (like tokens) to every request, use an HTTP interceptor:

// auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authToken = 'your-token-here';

    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${authToken}`
      }
    });

    return next.handle(cloned);
  }
}

Then register it:

// app.module.ts
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';

@NgModule({
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ]
})
export class AppModule {}


⸻

Let me know if your header is dynamic (like JWT after login), or if you need help sending headers with FormData, PUT, etc.