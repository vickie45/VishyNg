✅ Angular 17: Modal Popup for Displaying Form Submission Response

Here’s how you can create a Bootstrap 5 modal popup to display the server or form submission response dynamically.

⸻

🚀 1. Install Bootstrap 5 (if not already installed)

If you haven’t installed Bootstrap yet, run the following command:

npm install bootstrap

Add the Bootstrap CSS in angular.json:

"styles": [
  "src/styles.css",
  "node_modules/bootstrap/dist/css/bootstrap.min.css"
]



⸻

✅ 2. Create a Modal Service

To keep the modal functionality reusable and clean, create a service to handle modal interactions.

Command:

ng generate service modal

src/app/modal.service.ts:

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private modalData = new BehaviorSubject<{ title: string, message: string, type: string } | null>(null);
  modalData$ = this.modalData.asObservable();

  open(title: string, message: string, type: 'success' | 'error' | 'info' | 'warning') {
    this.modalData.next({ title, message, type });
  }

  close() {
    this.modalData.next(null);
  }
}



⸻

✅ 3. Create a Modal Component

Generate a reusable modal component.

Command:

ng generate component modal

src/app/modal/modal.component.ts:

import { Component } from '@angular/core';
import { ModalService } from '../modal.service';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css']
})
export class ModalComponent {
  constructor(public modalService: ModalService) {}

  closeModal(): void {
    this.modalService.close();
  }
}



⸻

✅ 4. Create the Modal Template

Add the modal structure with dynamic content.

src/app/modal/modal.component.html:

<div class="modal fade show d-block" *ngIf="modalService.modalData$ | async as modalData" tabindex="-1" aria-labelledby="modalLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header bg-{{modalData.type}}">
        <h5 class="modal-title text-white">{{ modalData.title }}</h5>
        <button type="button" class="btn-close" aria-label="Close" (click)="closeModal()"></button>
      </div>
      <div class="modal-body">
        <p>{{ modalData.message }}</p>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" (click)="closeModal()">Close</button>
      </div>
    </div>
  </div>
</div>



⸻

✅ 5. Add Modal Styling

src/app/modal/modal.component.css:

.modal.show {
  display: block;
  background: rgba(0, 0, 0, 0.5);
}

.modal-header.bg-success {
  background-color: #28a745 !important;
}

.modal-header.bg-error {
  background-color: #dc3545 !important;
}

.modal-header.bg-warning {
  background-color: #ffc107 !important;
}

.modal-header.bg-info {
  background-color: #17a2b8 !important;
}



⸻

✅ 6. Add Modal to app.component.html:

Include the ModalComponent in the app.component.html so that it can be displayed globally.

<app-modal></app-modal>
<router-outlet></router-outlet>



⸻

✅ 7. Modify the Form to Trigger the Modal

Use the modal service to display form submission responses.

src/app/app.component.ts:

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ModalService } from './modal.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  myForm!: FormGroup;
  submitted = false;

  constructor(private fb: FormBuilder, private modalService: ModalService) {}

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
      this.modalService.open(
        'Validation Error',
        'Please fill out all required fields correctly.',
        'warning'
      );
      return;
    }

    // Simulate server response
    setTimeout(() => {
      const isSuccess = Math.random() > 0.5; // Simulate success or failure
      if (isSuccess) {
        this.modalService.open(
          'Form Submitted',
          'Your form was submitted successfully!',
          'success'
        );
      } else {
        this.modalService.open(
          'Submission Failed',
          'There was an error submitting the form. Try again later.',
          'error'
        );
      }

      this.myForm.reset();
      this.submitted = false;
    }, 1000);
  }
}



⸻

✅ 8. Update Form Template

Use the same form template, which triggers the modal upon submission.

src/app/app.component.html:

<div class="container mt-5">
  <h2 class="mb-4">Reactive Form with Modal Popup</h2>
  
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

🚀 Explanation:
	1.	Modal Service: Handles opening and closing modals with dynamic content.
	2.	Modal Component: Displays the modal with dynamic messages and colors based on response types.
	3.	Form Submission:
	•	On form submission:
	•	If invalid: shows validation error in the modal.
	•	If valid: simulates server response with success or error messages.
	4.	Styling: Includes Bootstrap 5 styling with different colors for success, error, warning, and info messages.

⸻

✅ This solution follows industrial standards, providing a reusable, modular modal popup service that can be used across your Angular app. Let me know if you need further customization or additional features! 🚀