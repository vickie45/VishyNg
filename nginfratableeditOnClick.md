You want the table to become editable when clicking the “Edit” button, allowing users to modify Strength Count, Availability (checkbox), and Comment (textarea). Once the user clicks “Save”, it should return to a read-only state.

⸻

✅ Implementation Details

✔ Click “Edit” → The row fields become editable.
✔ Click “Save” → The row returns to read-only mode.
✔ Facility remains read-only and only changes when clicked separately.

⸻

Updated Bootstrap 5 Table with Edit/Save Button

<div class="container mt-4">
  <h2 class="mb-3">Infrastructure Details</h2>

  <form [formGroup]="infrastructureForm">
    <div class="table-responsive">
      <table class="table table-bordered table-striped align-middle">
        <thead class="table-dark">
          <tr>
            <th style="min-width: 200px;">Facility</th>
            <th class="w-15">Strength Count</th>
            <th class="w-20">Classification</th>
            <th class="text-center w-10">Available</th>
            <th class="w-25">Comment</th>
            <th class="w-15">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let item of infrastructures.controls; let i = index" [formGroup]="item">
            <!-- Facility (Read-only, Editable Separately) -->
            <td>
              <div (click)="editFacility(i)" *ngIf="!editingFacility[i]" class="form-control-plaintext text-wrap">
                {{ item.get('facility')?.value || 'Click to edit' }}
              </div>
              <input *ngIf="editingFacility[i]" type="text" class="form-control w-100" formControlName="facility" 
                     (blur)="saveFacility(i)" autofocus>
            </td>

            <!-- Strength Count -->
            <td>
              <input *ngIf="editingRow[i]" type="number" class="form-control w-100" formControlName="strengthCount">
              <div *ngIf="!editingRow[i]" class="form-control-plaintext">{{ item.get('strengthCount')?.value }}</div>
            </td>

            <!-- Classification Dropdown (Read-only) -->
            <td>
              <select class="form-select w-100" formControlName="classification" disabled>
                <option *ngFor="let option of classificationOptions" [value]="option">{{ option }}</option>
              </select>
            </td>

            <!-- Available Checkbox -->
            <td class="text-center">
              <input *ngIf="editingRow[i]" type="checkbox" class="form-check-input" formControlName="available">
              <span *ngIf="!editingRow[i]">
                {{ item.get('available')?.value ? '✔' : '✘' }}
              </span>
            </td>

            <!-- Comment Field as Textarea -->
            <td>
              <textarea *ngIf="editingRow[i]" class="form-control w-100" formControlName="comment" rows="2"></textarea>
              <div *ngIf="!editingRow[i]" class="form-control-plaintext text-wrap">
                {{ item.get('comment')?.value }}
              </div>
            </td>

            <!-- Action Buttons -->
            <td>
              <button *ngIf="!editingRow[i]" type="button" class="btn btn-sm btn-warning" (click)="editRow(i)">
                <i class="bi bi-pencil"></i> Edit
              </button>
              <button *ngIf="editingRow[i]" type="button" class="btn btn-sm btn-success" (click)="saveRow(i)">
                <i class="bi bi-check-lg"></i> Save
              </button>
              <button type="button" class="btn btn-sm btn-danger" (click)="deleteRow(i)">
                <i class="bi bi-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </form>

  <!-- Add Row & Save All Button -->
  <div class="mt-3 d-flex gap-2">
    <button class="btn btn-success" (click)="addRow()">
      <i class="bi bi-plus-lg"></i> Add New Row
    </button>
    <button class="btn btn-primary" (click)="save()">
      <i class="bi bi-save"></i> Save Data
    </button>
  </div>
</div>



⸻

🛠 TypeScript Logic (infrastructure-details.component.ts)

import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-infrastructure-details',
  templateUrl: './infrastructure-details.component.html',
  styleUrls: ['./infrastructure-details.component.scss']
})
export class InfrastructureDetailsComponent {
  infrastructureForm: FormGroup;
  classificationOptions: string[] = ['Class A', 'Class B', 'Class C'];
  
  editingRow: boolean[] = []; // Tracks row edit state
  editingFacility: boolean[] = []; // Tracks facility field edit state

  constructor(private fb: FormBuilder) {
    this.infrastructureForm = this.fb.group({
      infrastructures: this.fb.array<FormGroup>([])
    });

    this.addRow({ facility: 'Library', strengthCount: 100, classification: 'Class A', available: true, comment: '' });
    this.addRow({ facility: 'Lab', strengthCount: 50, classification: 'Class B', available: false, comment: '' });
  }

  get infrastructures(): FormArray<FormGroup> {
    return this.infrastructureForm.get('infrastructures') as FormArray<FormGroup>;
  }

  addRow(defaults: { facility: string; strengthCount: number; classification: string; available: boolean; comment: string }) {
    const row: FormGroup = this.fb.group({
      facility: new FormControl(defaults.facility),
      strengthCount: new FormControl(defaults.strengthCount),
      classification: new FormControl(defaults.classification),
      available: new FormControl(defaults.available),
      comment: new FormControl(defaults.comment)
    });

    this.infrastructures.push(row);
    this.editingRow.push(false); // Initially not editable
    this.editingFacility.push(false); // Facility starts as non-editable
  }

  deleteRow(index: number): void {
    this.infrastructures.removeAt(index);
    this.editingRow.splice(index, 1);
    this.editingFacility.splice(index, 1);
  }

  editRow(index: number): void {
    this.editingRow[index] = true;
  }

  saveRow(index: number): void {
    this.editingRow[index] = false;
  }

  editFacility(index: number): void {
    this.editingFacility[index] = true;
  }

  saveFacility(index: number): void {
    this.editingFacility[index] = false;
  }

  save(): void {
    console.log('Form Data:', this.infrastructureForm.value);
  }
}



⸻

🔹 Features of This Implementation

✅ Click “Edit” → Strength, Availability, and Comment become editable.
✅ Click “Save” → The row fields switch back to read-only mode.
✅ Facility is edited separately by clicking on it.
✅ Table stays neat & readable when not editing.
✅ Fully responsive with Bootstrap 5 styling.

⸻

📌 Expected User Flow
	1.	The user clicks “Edit”, and the row becomes editable.
	2.	The user modifies Strength Count, Availability, and Comments.
	3.	Clicking “Save” locks the row in a non-editable state.
	4.	The Facility field is edited separately (by clicking on it).

This ensures a clean UX while keeping the table structured. Let me know if you need tweaks! 🚀