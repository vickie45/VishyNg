Here’s a sample validation function in TypeScript/JavaScript for an Angular form that validates all fields and displays the errors when the user clicks the “Save” button. This assumes you’re using Angular’s Reactive Forms with FormBuilder and FormGroup.

Validation Function

validateForm(formGroup: FormGroup): void {
  // Trigger validation for all form controls
  Object.keys(formGroup.controls).forEach((field) => {
    const control = formGroup.get(field);
    if (control instanceof FormControl) {
      control.markAsTouched({ onlySelf: true });
    } else if (control instanceof FormGroup) {
      // Recursive validation for nested form groups
      this.validateForm(control);
    }
  });
}

Usage in Save Button

Call this validation function when the “Save” button is clicked. Here’s an example:

onSave(): void {
  if (this.form.invalid) {
    this.validateForm(this.form); // Call the validation function
    console.error('Form is invalid:', this.form.errors); // Log validation errors if needed
    return; // Stop further processing if the form is invalid
  }

  // Proceed with save logic if the form is valid
  console.log('Form is valid. Proceeding with save...');
}

Example Form with Validation

Assume you have a form defined like this:

form: FormGroup;

constructor(private fb: FormBuilder) {
  this.form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });
}

Displaying Validation Errors in the Template

You can show validation errors in the HTML like this:

<form [formGroup]="form">
  <div>
    <label for="name">Name</label>
    <input id="name" formControlName="name" />
    <div *ngIf="form.get('name')?.touched && form.get('name')?.invalid">
      <small *ngIf="form.get('name')?.errors?.['required']">Name is required.</small>
      <small *ngIf="form.get('name')?.errors?.['minlength']">
        Name must be at least 3 characters long.
      </small>
    </div>
  </div>

  <div>
    <label for="email">Email</label>
    <input id="email" formControlName="email" />
    <div *ngIf="form.get('email')?.touched && form.get('email')?.invalid">
      <small *ngIf="form.get('email')?.errors?.['required']">Email is required.</small>
      <small *ngIf="form.get('email')?.errors?.['email']">Invalid email format.</small>
    </div>
  </div>

  <div>
    <label for="password">Password</label>
    <input id="password" formControlName="password" type="password" />
    <div *ngIf="form.get('password')?.touched && form.get('password')?.invalid">
      <small *ngIf="form.get('password')?.errors?.['required']">Password is required.</small>
      <small *ngIf="form.get('password')?.errors?.['minlength']">
        Password must be at least 6 characters long.
      </small>
    </div>
  </div>

  <button type="button" (click)="onSave()">Save</button>
</form>

Key Points

	1.	Mark Controls as Touched: The markAsTouched method ensures Angular applies the validation styles and displays error messages for untouched fields.
	2.	Recursive Validation: The function also supports nested FormGroups.
	3.	Error Display: The template dynamically displays error messages for each field.