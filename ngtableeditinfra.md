Here’s a step-by-step guide to creating an Infrastructure Details Page in Angular 17 using Bootstrap 5. The page will have a table with editable fields when clicking the “Edit” button.

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

Step 3: Update HTML Template (infrastructure-details.component.html)

<div class="container mt-4">
  <h2 class="mb-3">Infrastructure Details</h2>

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
      <tr *ngFor="let item of infrastructureList; let i = index">
        <!-- Editable Text Fields -->
        <td>
          <input type="text" class="form-control" [(ngModel)]="item.facility" [disabled]="!item.editing">
        </td>
        <td>
          <input type="number" class="form-control" [(ngModel)]="item.strengthCount" [disabled]="!item.editing">
        </td>

        <!-- Editable Dropdown -->
        <td>
          <select class="form-select" [(ngModel)]="item.classification" [disabled]="!item.editing">
            <option *ngFor="let option of classificationOptions" [value]="option">{{ option }}</option>
          </select>
        </td>

        <!-- Checkbox -->
        <td class="text-center">
          <input type="checkbox" class="form-check-input" [(ngModel)]="item.available" [disabled]="!item.editing">
        </td>

        <!-- Editable Text Field -->
        <td>
          <input type="text" class="form-control" [(ngModel)]="item.comment" [disabled]="!item.editing">
        </td>

        <!-- Action Buttons -->
        <td>
          <button class="btn btn-sm btn-primary me-2" (click)="editRow(i)" *ngIf="!item.editing">Edit</button>
          <button class="btn btn-sm btn-success me-2" (click)="saveRow(i)" *ngIf="item.editing">Save</button>
          <button class="btn btn-sm btn-danger" (click)="deleteRow(i)">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>

  <!-- Add New Row -->
  <button class="btn btn-success mt-3" (click)="addRow()">Add New Row</button>
</div>



⸻

Step 4: Update Component Logic (infrastructure-details.component.ts)

import { Component } from '@angular/core';

interface Infrastructure {
  facility: string;
  strengthCount: number;
  classification: string;
  available: boolean;
  comment: string;
  editing: boolean;
}

@Component({
  selector: 'app-infrastructure-details',
  templateUrl: './infrastructure-details.component.html',
  styleUrls: ['./infrastructure-details.component.scss']
})
export class InfrastructureDetailsComponent {
  classificationOptions: string[] = ['Class A', 'Class B', 'Class C'];

  infrastructureList: Infrastructure[] = [
    { facility: 'Library', strengthCount: 100, classification: 'Class A', available: true, comment: 'Spacious', editing: false },
    { facility: 'Lab', strengthCount: 50, classification: 'Class B', available: false, comment: 'Well equipped', editing: false }
  ];

  editRow(index: number): void {
    this.infrastructureList[index].editing = true;
  }

  saveRow(index: number): void {
    this.infrastructureList[index].editing = false;
  }

  deleteRow(index: number): void {
    this.infrastructureList.splice(index, 1);
  }

  addRow(): void {
    this.infrastructureList.push({
      facility: '',
      strengthCount: 0,
      classification: 'Class A',
      available: false,
      comment: '',
      editing: true
    });
  }
}



⸻

Step 5: Enable Two-Way Binding (app.module.ts)

To use [(ngModel)], import FormsModule:

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { InfrastructureDetailsComponent } from './infrastructure-details/infrastructure-details.component';

@NgModule({
  declarations: [
    AppComponent,
    InfrastructureDetailsComponent
  ],
  imports: [
    BrowserModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }



⸻

Step 6: Run the Application

Start the app:

ng serve

Navigate to http://localhost:4200/ and see the editable table in action.

⸻

Features Implemented

✅ Editable text fields
✅ Editable dropdown for classification
✅ Checkbox for availability
✅ Add, Edit, Save, and Delete functionality

This follows Angular 17 best practices and Bootstrap 5 styling, ensuring a clean and user-friendly table. Let me know if you need enhancements!