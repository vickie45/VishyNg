To display data from a GET API in a reactive form in Angular that is initially readonly and becomes editable when the user clicks an “Edit” button, you can follow these steps:

	1.	Fetch data from API: Use HttpClient to fetch data from the API on component initialization.
	2.	Populate Reactive Form: Bind the fetched data to a reactive form.
	3.	Control the Form State: Use the readonly mode initially, then enable form controls when “Edit” is clicked.

Here’s a basic implementation:

Component (TypeScript)

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profileForm: FormGroup;
  isEditable = false;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    // Initialize the form with controls in readonly mode
    this.profileForm = this.fb.group({
      name: [{ value: '', disabled: true }],
      email: [{ value: '', disabled: true }],
      phone: [{ value: '', disabled: true }],
      // Add more fields as needed
    });
  }

  ngOnInit() {
    // Fetch data from API on component load
    this.http.get<any>('https://api.example.com/profile').subscribe(data => {
      this.profileForm.patchValue(data);
    });
  }

  // Toggle edit mode
  onEdit() {
    this.isEditable = !this.isEditable;
    if (this.isEditable) {
      this.profileForm.enable();
    } else {
      this.profileForm.disable();
    }
  }
}

Component Template (HTML)

<div [formGroup]="profileForm">
  <label for="name">Name</label>
  <input id="name" formControlName="name" [readonly]="!isEditable" />

  <label for="email">Email</label>
  <input id="email" formControlName="email" [readonly]="!isEditable" />

  <label for="phone">Phone</label>
  <input id="phone" formControlName="phone" [readonly]="!isEditable" />

  <!-- Add more fields as needed -->

  <button (click)="onEdit()">
    {{ isEditable ? 'Save' : 'Edit' }}
  </button>
</div>

Explanation

	1.	Form Initialization: The form fields are initialized with disabled: true so they are readonly.
	2.	API Data Binding: patchValue() fills the form with the API response data.
	3.	Toggle Edit Mode: onEdit() toggles the isEditable flag and enables/disables the form controls.

This setup ensures the form remains readonly until “Edit” is clicked, allowing users to edit data only when desired.