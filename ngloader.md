✅ Angular Global Loader Using HttpInterceptor

To add a global loader that automatically shows for all HTTP requests, you can use Angular’s HttpInterceptor.

⸻

✅ 1. Create a Loader Service

Run the following command to generate a service:

ng generate service loader

📌 loader.service.ts:

import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$: Observable<boolean> = this.loadingSubject.asObservable();

  show(): void {
    this.loadingSubject.next(true);
  }

  hide(): void {
    this.loadingSubject.next(false);
  }
}



⸻

✅ 2. Create an HTTP Interceptor

Run the following command:

ng generate interceptor loader

📌 loader.interceptor.ts:

import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, finalize, tap } from 'rxjs';
import { LoaderService } from './loader.service';

@Injectable()
export class LoaderInterceptor implements HttpInterceptor {

  constructor(private loaderService: LoaderService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.loaderService.show();

    return next.handle(req).pipe(
      tap({
        next: (event) => {
          if (event instanceof HttpResponse) {
            this.loaderService.hide();
          }
        },
        error: (error: HttpErrorResponse) => {
          console.error('API Error:', error);
          this.loaderService.hide();
        }
      }),
      finalize(() => {
        this.loaderService.hide();
      })
    );
  }
}



⸻

✅ 3. Register the Interceptor in app.module.ts

Add the LoaderInterceptor to the provider’s array in app.module.ts:

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { LoaderInterceptor } from './loader.interceptor';
import { LoaderService } from './loader.service';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, HttpClientModule],
  providers: [
    LoaderService,
    { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }



⸻

✅ 4. Loader Component

Create a loader component to display the loading animation.

Run the following command:

ng generate component loader

📌 loader.component.ts:

import { Component } from '@angular/core';
import { LoaderService } from '../loader.service';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss']
})
export class LoaderComponent {
  loading$ = this.loaderService.loading$;

  constructor(private loaderService: LoaderService) {}
}

📌 loader.component.html:

<div *ngIf="loading$ | async" class="loader-overlay">
  <div class="loader"></div>
</div>

📌 loader.component.scss:

.loader-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
}

.loader {
  border: 6px solid #f3f3f3;
  border-top: 6px solid #3498db;
  border-radius: 50%;
  width: 50px;
  height: 50px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}



⸻

✅ 5. Add the Loader Component to app.component.html

To display the loader globally, add the LoaderComponent to your app.component.html:

<app-loader></app-loader>
<router-outlet></router-outlet>



⸻

✅ 6. Test the Global Loader

Now, whenever you make any API request using HttpClient, the loader will automatically appear and disappear once the response is received, or on error.

⸻

🚀 Key Benefits of Global Loader with Interceptor
	1.	Automatic Handling: The loader appears for all API requests without manual handling.
	2.	Cleaner Code: You don’t need to write loading = true/false in every component.
	3.	Error Handling: The loader disappears even if the request fails.
	4.	Reusability: The LoaderService and Interceptor can be used across the entire app.

⸻

✅ This solution ensures a consistent and reusable loading mechanism across your Angular 17 application. Let me know if you need further customization or enhancements! 🚀