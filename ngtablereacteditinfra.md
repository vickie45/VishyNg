Here’s how to implement the Infrastructure Details Page using Reactive Forms in Angular 17 with Bootstrap 5.

⸻

Step 1: Setup Angular 17 Project

If you haven’t created an Angular 17 project yet, do so with:

ng new infrastructure-details --style=scss --routing=true
cd infrastructure-details
ng add @angular-eslint/schematics  # Optional for linting

Then, install Bootstrap 5:

npm install bootstrap

Include Bootstrap in angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.scss"
]



⸻

Step 2: Generate Component

Run:

ng generate component infrastructure-details



⸻

Step 3: Import Reactive Forms in app.module.ts

Modify app.module.ts to include ReactiveFormsModule:

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { InfrastructureDetailsComponent } from './infrastructure-details/infrastructure-details.component';

@NgModule({
  declarations: [
    AppComponent,
    InfrastructureDetailsComponent
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }



⸻

Step 4: Update Component Logic (infrastructure-details.component.ts)

import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-infrastructure-details',
  templateUrl: './infrastructure-details.component.html',
  styleUrls: ['./infrastructure-details.component.scss']
})
export class InfrastructureDetailsComponent {
  infrastructureForm: FormGroup;
  classificationOptions: string[] = ['Class A', 'Class B', 'Class C'];

  constructor(private fb: FormBuilder) {
    this.infrastructureForm = this.fb.group({
      infrastructures: this.fb.array([])
    });

    // Add initial rows
    this.addRow({ facility: 'Library', strengthCount: 100, classification: 'Class A', available: true, comment: '' });
    this.addRow({ facility: 'Lab', strengthCount: 50, classification: 'Class B', available: false, comment: '' });
  }

  get infrastructures(): FormArray {
    return this.infrastructureForm.get('infrastructures') as FormArray;
  }

  addRow(defaults = { facility: '', strengthCount: 0, classification: 'Class A', available: false, comment: '' }) {
    const row = this.fb.group({
      facility: [defaults.facility],
      strengthCount: [defaults.strengthCount],
      classification: [defaults.classification],
      available: [defaults.available],
      comment: [defaults.comment]
    });

    this.infrastructures.push(row);
  }

  deleteRow(index: number): void {
    this.infrastructures.removeAt(index);
  }

  save(): void {
    console.log('Form Data:', this.infrastructureForm.value);
  }
}



⸻

Step 5: Update HTML Template (infrastructure-details.component.html)

<div class="container mt-4">
  <h2 class="mb-3">Infrastructure Details</h2>

  <form [formGroup]="infrastructureForm">
    <table class="table table-bordered table-striped">
      <thead class="table-dark">
        <tr>
          <th>Facility</th>
          <th>Strength Count</th>
          <th>Classification</th>
          <th>Available</th>
          <th>Comment</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let item of infrastructures.controls; let i = index" [formGroup]="item">
          <!-- Text Inputs -->
          <td>
            <input type="text" class="form-control" formControlName="facility">
          </td>
          <td>
            <input type="number" class="form-control" formControlName="strengthCount">
          </td>

          <!-- Dropdown -->
          <td>
            <select class="form-select" formControlName="classification">
              <option *ngFor="let option of classificationOptions" [value]="option">{{ option }}</option>
            </select>
          </td>

          <!-- Checkbox -->
          <td class="text-center">
            <input type="checkbox" class="form-check-input" formControlName="available">
          </td>

          <!-- Comment Field -->
          <td>
            <input type="text" class="form-control" formControlName="comment">
          </td>

          <!-- Action Buttons -->
          <td>
            <button class="btn btn-sm btn-danger" (click)="deleteRow(i)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
  </form>

  <!-- Add New Row -->
  <button class="btn btn-success mt-3" (click)="addRow()">Add New Row</button>
  <button class="btn btn-primary mt-3 ms-2" (click)="save()">Save Data</button>
</div>



⸻

Step 6: Run the Application

Start the app:

ng serve

Navigate to http://localhost:4200/ to test the editable infrastructure details table.

⸻

Features Implemented

✅ Uses Reactive Forms for better validation and scalability
✅ Editable text fields
✅ Editable dropdown for classification
✅ Checkbox for availability
✅ Add & Delete rows dynamically
✅ Save Data logs values to console

This follows Angular 17 best practices with Bootstrap 5 styling. Let me know if you need modifications!