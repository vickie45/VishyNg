Here’s an example of how you can implement a Captcha in Angular to display and validate a basic captcha. This uses a text-based captcha for simplicity.

Steps:

	1.	Generate a random captcha string.
	2.	Validate the user’s input.
	3.	Show appropriate messages on success or failure.

Code Implementation:

HTML (captcha.component.html):

<div class="captcha-container">
  <form [formGroup]="captchaForm" (ngSubmit)="onSubmit()">
    <!-- Captcha Display -->
    <div class="captcha-display">
      <p>{{ captcha }}</p>
    </div>

    <!-- Input Field -->
    <div>
      <label for="captchaInput">Enter Captcha:</label>
      <input
        type="text"
        id="captchaInput"
        formControlName="captchaInput"
        placeholder="Enter captcha"
      />
      <div *ngIf="captchaForm.get('captchaInput')?.invalid && captchaForm.get('captchaInput')?.touched" class="error">
        Captcha is required.
      </div>
    </div>

    <!-- Validation Message -->
    <div *ngIf="isSubmitted">
      <p *ngIf="isCaptchaValid" class="success">Captcha Validated Successfully!</p>
      <p *ngIf="!isCaptchaValid" class="error">Invalid Captcha. Please try again!</p>
    </div>

    <!-- Submit Button -->
    <button type="submit" [disabled]="captchaForm.invalid">Validate Captcha</button>
  </form>

  <!-- Refresh Captcha -->
  <button (click)="generateCaptcha()">Refresh Captcha</button>
</div>

TypeScript (captcha.component.ts):

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-captcha',
  templateUrl: './captcha.component.html',
  styleUrls: ['./captcha.component.css']
})
export class CaptchaComponent implements OnInit {
  captcha: string = '';
  captchaForm: FormGroup;
  isSubmitted = false;
  isCaptchaValid = false;

  constructor(private fb: FormBuilder) {
    this.captchaForm = this.fb.group({
      captchaInput: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.generateCaptcha();
  }

  // Generate a random captcha
  generateCaptcha(): void {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    this.captcha = Array.from({ length: 6 }, () => characters[Math.floor(Math.random() * characters.length)]).join('');
    this.isSubmitted = false; // Reset validation messages on refresh
    this.captchaForm.reset(); // Reset input field
  }

  // Handle form submission
  onSubmit(): void {
    this.isSubmitted = true;
    const userInput = this.captchaForm.get('captchaInput')?.value;
    this.isCaptchaValid = userInput === this.captcha;
  }
}

CSS (captcha.component.css):

.captcha-container {
  max-width: 400px;
  margin: 0 auto;
  font-family: Arial, sans-serif;
}

.captcha-display {
  font-size: 24px;
  font-weight: bold;
  text-align: center;
  letter-spacing: 3px;
  background-color: #f4f4f4;
  border: 1px solid #ccc;
  margin-bottom: 10px;
  padding: 10px;
}

input {
  width: 100%;
  padding: 8px;
  margin: 10px 0;
  border: 1px solid #ccc;
  border-radius: 4px;
}

button {
  padding: 10px 15px;
  border: none;
  background-color: #007bff;
  color: white;
  cursor: pointer;
  border-radius: 4px;
}

button[disabled] {
  background-color: #cccccc;
}

.success {
  color: green;
}

.error {
  color: red;
}

Key Features:

	1.	Captcha Generation: A random alphanumeric captcha is generated.
	2.	Form Validation: The captcha field is validated to ensure it’s not empty.
	3.	User Feedback: Displays success or error messages after form submission.
	4.	Refresh Option: Allows refreshing the captcha.

You can further enhance this by:

	•	Adding server-side validation.
	•	Using image-based captchas (e.g., Google reCAPTCHA). Let me know if you’d like help with those!