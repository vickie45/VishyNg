To highlight empty fields with a reddish-pink border when the user clicks “Save,” you can use Angular’s reactive forms along with conditional CSS styling.

Steps:
	1.	Create a form with validation.
	2.	Apply CSS classes dynamically when fields are empty.
	3.	Show validation messages if necessary.

⸻

Implementation:

1. app.component.ts

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  myForm: FormGroup;
  
  constructor(private fb: FormBuilder) {
    this.myForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  save() {
    if (this.myForm.invalid) {
      this.myForm.markAllAsTouched(); // Mark all fields as touched to trigger validation
    } else {
      alert('Form submitted successfully!');
    }
  }
}



⸻

2. app.component.html

<div class="container">
  <form [formGroup]="myForm">
    <div class="form-group">
      <label>Name:</label>
      <input 
        type="text" 
        class="form-control" 
        [class.invalid]="myForm.controls['name'].invalid && myForm.controls['name'].touched"
        formControlName="name">
    </div>
    
    <div class="form-group">
      <label>Email:</label>
      <input 
        type="email" 
        class="form-control" 
        [class.invalid]="myForm.controls['email'].invalid && myForm.controls['email'].touched"
        formControlName="email">
    </div>
    
    <button type="button" (click)="save()" class="btn btn-primary">Save</button>
  </form>
</div>



⸻

3. app.component.css

.form-control.invalid {
  border: 2px solid #ff4d6d; /* Reddish pink */
  background-color: #ffe6eb; /* Light pinkish background */
}



⸻

Explanation:
	•	formControlName binds the input fields to Angular’s reactive form.
	•	When the user clicks “Save,” markAllAsTouched() ensures validation errors appear.
	•	The [class.invalid] directive dynamically applies the invalid class if the field is invalid and has been touched.
	•	The .invalid CSS class highlights the empty fields with a reddish-pink border.

This approach ensures a clean, user-friendly way to highlight empty fields when submitting a form.