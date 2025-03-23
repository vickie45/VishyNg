Here’s an Angular 17 solution to display toast messages for validation errors and successful form submission using Bootstrap 5.

⸻

✅ 1. Install Bootstrap 5 (if not already installed)

Run the following command to add Bootstrap 5 to your Angular project:

npm install bootstrap

Add the Bootstrap CSS in angular.json:

"styles": [
  "src/styles.css",
  "node_modules/bootstrap/dist/css/bootstrap.min.css"
]



⸻

✅ 2. Create a Toast Service

Create a service for handling toast messages. This makes it reusable across the app.

Command:

ng generate service toast

src/app/toast.service.ts:

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toastMessages = new BehaviorSubject<{ message: string, type: string }[]>([]);
  messages$ = this.toastMessages.asObservable();

  show(message: string, type: 'success' | 'danger' | 'warning' | 'info') {
    const messages = this.toastMessages.getValue();
    this.toastMessages.next([...messages, { message, type }]);

    // Auto-remove toast after 3 seconds
    setTimeout(() => {
      this.toastMessages.next(this.toastMessages.getValue().slice(1));
    }, 3000);
  }
}



⸻

✅ 3. Create the Toast Component

This component will display the toast messages dynamically.

Command:

ng generate component toast

src/app/toast/toast.component.ts:

import { Component } from '@angular/core';
import { ToastService } from '../toast.service';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.css']
})
export class ToastComponent {
  constructor(public toastService: ToastService) {}
}

src/app/toast/toast.component.html:

<div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 1050;">
  <div *ngFor="let toast of toastService.messages$ | async" 
       class="toast show align-items-center text-white bg-{{toast.type}} border-0" 
       role="alert" 
       aria-live="assertive" 
       aria-atomic="true">
    <div class="d-flex">
      <div class="toast-body">
        {{ toast.message }}
      </div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" 
              (click)="toastService.show('', 'info')" aria-label="Close"></button>
    </div>
  </div>
</div>

src/app/toast/toast.component.css:

.toast-container {
  max-width: 350px;
}
.toast {
  opacity: 1 !important;
  transition: opacity 0.5s;
}



⸻

✅ 4. Add Toast Component to app.component.html:

Add the ToastComponent to the app.component.html to ensure the messages are displayed globally.

<app-toast></app-toast>
<router-outlet></router-outlet>



⸻

✅ 5. Create the Reactive Form with Validation

Create a reactive form with validation.

src/app/app.component.ts:

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  myForm!: FormGroup;
  submitted = false;

  constructor(private fb: FormBuilder, private toastService: ToastService) {}

  ngOnInit(): void {
    this.myForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.myForm.invalid) {
      this.toastService.show('Please fill all required fields correctly!', 'danger');
      return;
    }

    // Simulate saving logic
    this.toastService.show('Form saved successfully!', 'success');
    console.log('Form Data:', this.myForm.value);
    
    this.myForm.reset();
    this.submitted = false;
  }
}



⸻

✅ 6. Add the Form in HTML

src/app/app.component.html:

<div class="container mt-5">
  <h2 class="mb-4">Reactive Form with Toast Messages</h2>
  
  <form [formGroup]="myForm" (ngSubmit)="onSubmit()" novalidate>
    <div class="mb-3">
      <label class="form-label">Name</label>
      <input type="text" formControlName="name" class="form-control" [class.is-invalid]="submitted && myForm.controls['name'].invalid">
      <div class="invalid-feedback" *ngIf="submitted && myForm.controls['name'].invalid">
        Name is required and must be at least 3 characters long.
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label">Email</label>
      <input type="email" formControlName="email" class="form-control" [class.is-invalid]="submitted && myForm.controls['email'].invalid">
      <div class="invalid-feedback" *ngIf="submitted && myForm.controls['email'].invalid">
        Enter a valid email.
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label">Password</label>
      <input type="password" formControlName="password" class="form-control" [class.is-invalid]="submitted && myForm.controls['password'].invalid">
      <div class="invalid-feedback" *ngIf="submitted && myForm.controls['password'].invalid">
        Password is required (minimum 6 characters).
      </div>
    </div>

    <button type="submit" class="btn btn-primary">Submit</button>
  </form>
</div>



⸻

✅ 7. Final Touch: Add CSS

src/app/app.component.css:

.container {
  max-width: 600px;
}
form .form-control.is-invalid {
  border-color: #dc3545;
}



⸻

🚀 Explanation:
	1.	Toast Service: Manages toast messages globally.
	2.	Toast Component: Displays messages with Bootstrap styles.
	3.	Form Handling:
	•	On form submission:
	•	Shows error toast if validation fails.
	•	Shows success toast if saved successfully.
	•	Resets the form on success.
	4.	Bootstrap Styling: Displays attractive toast notifications with success, danger, info, and warning types.

⸻

✅ This solution follows industrial standards with a reusable ToastService and modular component-based architecture, making it easy to integrate into larger Angular applications. Let me know if you need further customization or additional features!