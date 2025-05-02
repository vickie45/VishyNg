Here’s how to disable negative values in a form control in Angular using Reactive Forms with both validation and keyboard restriction:

⸻

1. Custom Validator to Prevent Negative Values

Step 1: Create the Validator

// validators/no-negative.validator.ts
import { AbstractControl, ValidationErrors } from '@angular/forms';

export function noNegativeValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  return value < 0 ? { negativeValue: true } : null;
}



⸻

2. Set Up the Form

Step 2: Component Code

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { noNegativeValidator } from './validators/no-negative.validator';

@Component({
  selector: 'app-amount-form',
  templateUrl: './amount-form.component.html'
})
export class AmountFormComponent {
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      amount: [0, [Validators.required, noNegativeValidator]]
    });
  }

  preventNegative(event: KeyboardEvent) {
    if (event.key === '-' || event.key === 'e') {
      event.preventDefault();
    }
  }
}



⸻

3. Template Code

Step 3: HTML Template

<form [formGroup]="form">
  <label for="amount">Amount</label>
  <input type="number"
         id="amount"
         formControlName="amount"
         min="0"
         (keypress)="preventNegative($event)" />

  <div *ngIf="form.get('amount')?.hasError('negativeValue') && form.get('amount')?.touched">
    Negative values are not allowed.
  </div>
</form>



⸻

Key Points
	•	min="0" prevents down-arrow from going below 0.
	•	(keypress) blocks manual typing of - or e.
	•	Custom validator ensures values are validated even if entered via copy-paste or browser autofill.

⸻

Would you like a version using Template-driven forms or Angular Material as well?