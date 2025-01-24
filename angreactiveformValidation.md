Below is an Angular example for validating a reactive form. It ensures that API calls are made only when the form is valid. If the form is invalid, a toast message is displayed to indicate validation errors.

Steps:

	1.	Create a reactive form with required fields.
	2.	Add validation rules for each form field.
	3.	Display validation errors.
	4.	Show a toast message if the form is invalid.

app.component.ts

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  myForm: FormGroup;

  constructor(private fb: FormBuilder, private toastr: ToastrService) {
    this.myForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  submitForm() {
    if (this.myForm.valid) {
      // Make the API call
      console.log('Form Data:', this.myForm.value);
      this.toastr.success('Form is valid! API called successfully.');
    } else {
      this.toastr.error('Validation errors exist. Please correct them.');
      this.myForm.markAllAsTouched(); // Mark all fields as touched to show validation errors
    }
  }

  // Helper method to check field validity
  isFieldInvalid(field: string): boolean {
    return this.myForm.get(field)?.invalid && this.myForm.get(field)?.touched;
  }
}

app.component.html

<div class="container mt-4">
  <form [formGroup]="myForm" (ngSubmit)="submitForm()">
    <div class="mb-3">
      <label for="username" class="form-label">Username</label>
      <input
        id="username"
        type="text"
        class="form-control"
        formControlName="username"
        [class.is-invalid]="isFieldInvalid('username')"
      />
      <div *ngIf="isFieldInvalid('username')" class="invalid-feedback">
        Username is required and should be at least 3 characters long.
      </div>
    </div>

    <div class="mb-3">
      <label for="email" class="form-label">Email</label>
      <input
        id="email"
        type="email"
        class="form-control"
        formControlName="email"
        [class.is-invalid]="isFieldInvalid('email')"
      />
      <div *ngIf="isFieldInvalid('email')" class="invalid-feedback">
        Please enter a valid email address.
      </div>
    </div>

    <div class="mb-3">
      <label for="password" class="form-label">Password</label>
      <input
        id="password"
        type="password"
        class="form-control"
        formControlName="password"
        [class.is-invalid]="isFieldInvalid('password')"
      />
      <div *ngIf="isFieldInvalid('password')" class="invalid-feedback">
        Password is required and should be at least 6 characters long.
      </div>
    </div>

    <button type="submit" class="btn btn-primary">Submit</button>
  </form>
</div>

app.module.ts

Install and configure ngx-toastr for toast messages:

npm install ngx-toastr --save
npm install @angular/animations --save

Import the required modules:

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot() // Toastr configuration
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}

Key Points:

	•	Validation: Fields are validated using Angular’s Validators.
	•	Error Display: Validation errors are shown dynamically below the inputs.
	•	Toastr Feedback: Toasts provide feedback for valid or invalid form submission.
	•	Mark All as Touched: Ensures all validation messages appear on form submission.

This approach is modular, user-friendly, and aligns with best practices.