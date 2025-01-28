Here’s how you can implement image-based CAPTCHA in Angular. We’ll use the canvas element to dynamically generate the CAPTCHA image.

Steps:

	1.	Use a canvas element to draw a CAPTCHA image with random text.
	2.	Validate the user’s input.
	3.	Reload the CAPTCHA if the user enters incorrect details.

Code Implementation:

HTML (captcha.component.html):

<div class="captcha-container">
  <form [formGroup]="captchaForm" (ngSubmit)="onSubmit()">
    <!-- Username Input -->
    <div>
      <label for="username">Username:</label>
      <input
        type="text"
        id="username"
        formControlName="username"
        placeholder="Enter username"
      />
    </div>

    <!-- Password Input -->
    <div>
      <label for="password">Password:</label>
      <input
        type="password"
        id="password"
        formControlName="password"
        placeholder="Enter password"
      />
    </div>

    <!-- CAPTCHA Display -->
    <div class="captcha-image">
      <canvas #captchaCanvas></canvas>
    </div>

    <!-- Input for CAPTCHA -->
    <div>
      <label for="captchaInput">Enter Captcha:</label>
      <input
        type="text"
        id="captchaInput"
        formControlName="captchaInput"
        placeholder="Enter the CAPTCHA shown above"
      />
      <div *ngIf="captchaForm.get('captchaInput')?.invalid && captchaForm.get('captchaInput')?.touched" class="error">
        Captcha is required.
      </div>
    </div>

    <!-- Validation Message -->
    <div *ngIf="isSubmitted">
      <p *ngIf="isCaptchaValid" class="success">Login Successful!</p>
      <p *ngIf="!isCaptchaValid" class="error">Invalid CAPTCHA, username, or password. Please try again!</p>
    </div>

    <!-- Submit Button -->
    <button type="submit" [disabled]="captchaForm.invalid">Login</button>
  </form>

  <!-- Reload CAPTCHA -->
  <button (click)="generateCaptcha()">Reload Captcha</button>
</div>

TypeScript (captcha.component.ts):

import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-captcha',
  templateUrl: './captcha.component.html',
  styleUrls: ['./captcha.component.css']
})
export class CaptchaComponent implements OnInit {
  @ViewChild('captchaCanvas', { static: true }) captchaCanvas!: ElementRef<HTMLCanvasElement>;
  captchaText: string = '';
  captchaForm: FormGroup;
  isSubmitted = false;
  isCaptchaValid = false;

  constructor(private fb: FormBuilder) {
    this.captchaForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      captchaInput: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.generateCaptcha();
  }

  // Generate CAPTCHA
  generateCaptcha(): void {
    const canvas = this.captchaCanvas.nativeElement;
    const ctx = canvas.getContext('2d')!;
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    this.captchaText = Array.from({ length: 6 }, () => characters[Math.floor(Math.random() * characters.length)]).join('');

    // Set canvas properties
    canvas.width = 200;
    canvas.height = 50;
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Draw background
    ctx.fillStyle = '#f4f4f4';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Draw random CAPTCHA text
    ctx.font = '30px Arial';
    ctx.fillStyle = '#333';
    ctx.fillText(this.captchaText, 50, 35);

    // Add random lines for distortion
    for (let i = 0; i < 5; i++) {
      ctx.strokeStyle = this.getRandomColor();
      ctx.beginPath();
      ctx.moveTo(Math.random() * canvas.width, Math.random() * canvas.height);
      ctx.lineTo(Math.random() * canvas.width, Math.random() * canvas.height);
      ctx.stroke();
    }

    // Add random dots for more distortion
    for (let i = 0; i < 30; i++) {
      ctx.fillStyle = this.getRandomColor();
      ctx.beginPath();
      ctx.arc(Math.random() * canvas.width, Math.random() * canvas.height, 1, 0, Math.PI * 2);
      ctx.fill();
    }

    this.captchaForm.get('captchaInput')?.reset();
    this.isSubmitted = false;
  }

  // Random color generator for distortion
  private getRandomColor(): string {
    const letters = '0123456789ABCDEF';
    return `#${Array.from({ length: 6 }, () => letters[Math.floor(Math.random() * letters.length)]).join('')}`;
  }

  // Handle form submission
  onSubmit(): void {
    this.isSubmitted = true;
    const userInput = this.captchaForm.get('captchaInput')?.value;
    const username = this.captchaForm.get('username')?.value;
    const password = this.captchaForm.get('password')?.value;

    // Validate CAPTCHA, username, and password
    this.isCaptchaValid = userInput === this.captchaText && username === 'testuser' && password === 'testpass';

    // Reload CAPTCHA if validation fails
    if (!this.isCaptchaValid) {
      this.generateCaptcha();
    }
  }
}

CSS (captcha.component.css):

.captcha-container {
  max-width: 400px;
  margin: 0 auto;
  font-family: Arial, sans-serif;
}

.captcha-image {
  margin: 10px 0;
  text-align: center;
}

canvas {
  border: 1px solid #ccc;
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

Features:

	1.	Dynamic CAPTCHA Image: A new CAPTCHA is drawn on a canvas each time the page loads or the user clicks “Reload Captcha.”
	2.	Distortion: Random lines and dots make the CAPTCHA harder to decipher by bots.
	3.	Validation: Checks username, password, and CAPTCHA input.
	4.	Reload on Failure: Automatically reloads a new CAPTCHA if the user enters incorrect data.

This implementation is simple yet effective. Let me know if you’d like to integrate external CAPTCHA libraries like Google reCAPTCHA!