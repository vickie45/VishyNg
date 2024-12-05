Here’s an updated solution using Reactive Forms for managing the editable table.

Step 1: Angular Service to Fetch Data from API

This part remains the same as in the previous solution:

data.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private apiUrl = 'https://your-api-url.com/endpoint';

  constructor(private http: HttpClient) {}

  getData(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  saveData(data: any[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/save`, data);
  }

  submitData(data: any[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/submit`, data);
  }
}

Step 2: Component for Editable Table

The logic is updated to use a FormArray to manage rows.

editable-table.component.ts

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { DataService } from './data.service';

@Component({
  selector: 'app-editable-table',
  templateUrl: './editable-table.component.html',
  styleUrls: ['./editable-table.component.css'],
})
export class EditableTableComponent implements OnInit {
  form: FormGroup;

  constructor(private fb: FormBuilder, private dataService: DataService) {
    this.form = this.fb.group({
      rows: this.fb.array([]), // FormArray to manage table rows
    });
  }

  ngOnInit(): void {
    this.fetchData();
  }

  get rows(): FormArray {
    return this.form.get('rows') as FormArray;
  }

  fetchData(): void {
    this.dataService.getData().subscribe((data) => {
      data.forEach((item) => this.addRow(item));
    });
  }

  addRow(data: any = { id: null, name: '', email: '', phone: '' }): void {
    const row = this.fb.group({
      id: [data.id], // Use ID if data is pre-fetched
      name: [data.name, Validators.required],
      email: [data.email, [Validators.required, Validators.email]],
      phone: [data.phone, Validators.required],
      isEditing: [true], // Default state is editable for new rows
    });
    this.rows.push(row);
  }

  deleteRow(index: number): void {
    this.rows.removeAt(index);
  }

  toggleEdit(row: FormGroup): void {
    row.patchValue({ isEditing: !row.value.isEditing });
  }

  save(): void {
    const data = this.rows.value.map((row: any) => {
      delete row.isEditing; // Exclude non-API properties
      return row;
    });
    this.dataService.saveData(data).subscribe(() => {
      alert('Data saved successfully!');
    });
  }

  submit(): void {
    const data = this.rows.value.map((row: any) => {
      delete row.isEditing; // Exclude non-API properties
      return row;
    });
    this.dataService.submitData(data).subscribe(() => {
      alert('Data submitted successfully!');
    });
  }
}

Step 3: Template for Editable Table

The template uses FormArray for rows, enabling validation and dynamic row management.

editable-table.component.html

<div class="table-container">
  <button class="btn btn-primary add-new" (click)="addRow()">Add New</button>

  <form [formGroup]="form">
    <table class="table table-bordered mt-2">
      <thead>
        <tr>
          <th>#</th>
          <th>Name</th>
          <th>Email</th>
          <th>Phone</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody formArrayName="rows">
        <tr *ngFor="let row of rows.controls; let i = index" [formGroupName]="i">
          <td>{{ i + 1 }}</td>
          <td>
            <input
              *ngIf="row.value.isEditing"
              formControlName="name"
              type="text"
              class="form-control"
            />
            <span *ngIf="!row.value.isEditing">{{ row.value.name }}</span>
          </td>
          <td>
            <input
              *ngIf="row.value.isEditing"
              formControlName="email"
              type="email"
              class="form-control"
            />
            <span *ngIf="!row.value.isEditing">{{ row.value.email }}</span>
          </td>
          <td>
            <input
              *ngIf="row.value.isEditing"
              formControlName="phone"
              type="text"
              class="form-control"
            />
            <span *ngIf="!row.value.isEditing">{{ row.value.phone }}</span>
          </td>
          <td>
            <button
              *ngIf="!row.value.isEditing"
              class="btn btn-sm btn-warning"
              (click)="toggleEdit(row)"
            >
              Edit
            </button>
            <button
              *ngIf="row.value.isEditing"
              class="btn btn-sm btn-success"
              (click)="toggleEdit(row)"
            >
              Save
            </button>
            <button
              class="btn btn-sm btn-danger"
              (click)="deleteRow(i)"
            >
              Delete
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </form>

  <div class="action-buttons">
    <button class="btn btn-success" (click)="save()">Save</button>
    <button class="btn btn-primary" (click)="submit()">Submit</button>
  </div>
</div>

Step 4: Styling (Optional)

You can reuse the styling from the previous example or customize it further.

editable-table.component.css

.table-container {
  width: 80%;
  margin: auto;
}

.add-new {
  float: right;
}

.action-buttons {
  display: flex;
  justify-content: flex-end;
  margin-top: 10px;
}

Key Notes:

	1.	Validation: Add Validators for form controls to enforce rules (e.g., required fields, email format).
	2.	Reactive Forms Benefits:
	•	Centralized state management.
	•	Built-in validation support.
	•	Cleaner logic for dynamic row manipulation.
	3.	API Integration: Ensure saveData and submitData endpoints on the backend accept the correct structure.

Would you like help setting up error messages for validations?