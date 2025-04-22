Absolutely. Below is the enhanced Reactive Form version with:
	•	Add disease
	•	Edit percentage (auto-adjust others)
	•	Remove disease
	•	Total percentage validation

⸻

Full Angular Component Code with Remove Functionality

⸻

Component HTML

<form [formGroup]="diseaseForm">
  <div formArrayName="diseases">
    <div *ngFor="let disease of diseaseArray.controls; let i = index" [formGroupName]="i" class="disease-entry">
      <input formControlName="name" placeholder="Disease Code" />

      <input type="number"
             formControlName="percentage"
             (change)="onPercentageChange(i)" />

      <button type="button" (click)="removeDisease(i)" *ngIf="diseaseArray.length > 1">Remove</button>
    </div>
  </div>

  <button type="button" (click)="addDisease()">Add Disease</button>

  <div style="margin-top: 10px;">
    Total: {{ getTotalPercentage() }}%
    <div *ngIf="!isTotalValid()" style="color: red;">
      Total contribution must equal 100%
    </div>
  </div>
</form>



⸻

Component TypeScript

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';

@Component({
  selector: 'app-disease-contribution',
  templateUrl: './disease-contribution.component.html',
})
export class DiseaseContributionComponent {
  diseaseForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.diseaseForm = this.fb.group({
      diseases: this.fb.array([])
    });

    this.addDisease(); // Initialize with one disease by default
  }

  get diseaseArray(): FormArray {
    return this.diseaseForm.get('diseases') as FormArray;
  }

  addDisease(): void {
    const newDisease = this.fb.group({
      name: [''],
      percentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]]
    });

    this.diseaseArray.push(newDisease);
    this.recalculateEqualShare();
  }

  removeDisease(index: number): void {
    this.diseaseArray.removeAt(index);
    this.recalculateEqualShare();
  }

  recalculateEqualShare(): void {
    const count = this.diseaseArray.length;
    if (count === 0) return;

    const equalShare = +(100 / count).toFixed(2);

    this.diseaseArray.controls.forEach(control => {
      control.get('percentage')?.setValue(equalShare, { emitEvent: false });
    });

    this.adjustRounding();
  }

  onPercentageChange(changedIndex: number): void {
    const changedControl = this.diseaseArray.at(changedIndex);
    const fixed = changedControl.get('percentage')?.value ?? 0;

    const others = this.diseaseArray.controls.filter((_, idx) => idx !== changedIndex);
    const totalOthersOld = others.reduce((sum, ctrl) => sum + (ctrl.get('percentage')?.value ?? 0), 0);
    const remaining = 100 - fixed;

    if (others.length === 0 || totalOthersOld === 0) return;

    others.forEach(ctrl => {
      const oldVal = ctrl.get('percentage')?.value ?? 0;
      const proportion = oldVal / totalOthersOld;
      ctrl.get('percentage')?.setValue(+(proportion * remaining).toFixed(2), { emitEvent: false });
    });

    this.adjustRounding();
  }

  adjustRounding(): void {
    const total = this.getTotalPercentage();
    const diff = +(100 - total).toFixed(2);
    if (this.diseaseArray.length > 0 && diff !== 0) {
      const last = this.diseaseArray.at(this.diseaseArray.length - 1);
      const current = last.get('percentage')?.value ?? 0;
      last.get('percentage')?.setValue(+(current + diff).toFixed(2), { emitEvent: false });
    }
  }

  getTotalPercentage(): number {
    return +this.diseaseArray.controls.reduce((sum, ctrl) => {
      return sum + (ctrl.get('percentage')?.value ?? 0);
    }, 0).toFixed(2);
  }

  isTotalValid(): boolean {
    return this.getTotalPercentage() === 100;
  }
}



⸻

Styling Suggestion (Optional)

.disease-entry {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 8px;
}



⸻

Next Steps You Can Add
	•	Disable “Add” button if total diseases = 10.
	•	Confirm dialog before deleting.
	•	Show % in slider + number field combo.
	•	Emit changes via @Output() for parent components.

Would you like a working StackBlitz demo link or a save-to-file version?