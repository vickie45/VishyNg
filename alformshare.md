To unify all individual Reactive Forms from the tabs (Case, Cost, Maternity, Injury, Others) into a single object to send to Save and Submit APIs, we can use FormGroup composition via @Input() or a shared service.

Here’s the cleanest and scalable approach using shared parent FormGroup, which enables central control while keeping tab components modular.

⸻

Goal

Unify all tab forms under a single parent FormGroup in all-details.component.ts, and pass sub-forms to child tab components via @Input().

⸻

Step-by-Step Implementation

1. Parent: all-details.component.ts

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-all-details',
  templateUrl: './all-details.component.html',
})
export class AllDetailsComponent implements OnInit {
  allDetailsForm!: FormGroup;
  tabs = ['Case', 'Cost', 'Maternity', 'Injury', 'Others'];
  activeTab = 0;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.allDetailsForm = this.fb.group({
      case: this.fb.group({}),
      cost: this.fb.group({}),
      maternity: this.fb.group({}),
      injury: this.fb.group({}),
      others: this.fb.group({})
    });
  }

  saveAll(): void {
    console.log('Saving:', this.allDetailsForm.value);
    // Call save API here
  }

  submitAll(): void {
    if (this.allDetailsForm.valid) {
      console.log('Submitting:', this.allDetailsForm.value);
      // Call submit API here
    } else {
      console.log('Form Invalid');
      this.allDetailsForm.markAllAsTouched();
    }
  }
}



⸻

2. Parent Template: all-details.component.html

<ul class="nav nav-tabs">
  <li class="nav-item" *ngFor="let tab of tabs; let i = index">
    <a class="nav-link" [class.active]="activeTab === i" (click)="activeTab = i">{{ tab }}</a>
  </li>
</ul>

<div class="tab-content mt-3">
  <app-case-tab *ngIf="activeTab === 0" [form]="allDetailsForm.get('case')"></app-case-tab>
  <app-cost-tab *ngIf="activeTab === 1" [form]="allDetailsForm.get('cost')"></app-cost-tab>
  <app-maternity-tab *ngIf="activeTab === 2" [form]="allDetailsForm.get('maternity')"></app-maternity-tab>
  <app-injury-tab *ngIf="activeTab === 3" [form]="allDetailsForm.get('injury')"></app-injury-tab>
  <app-others-tab *ngIf="activeTab === 4" [form]="allDetailsForm.get('others')"></app-others-tab>
</div>

<div class="mt-4 text-end">
  <button class="btn btn-secondary me-2" (click)="saveAll()">Save All</button>
  <button class="btn btn-primary" (click)="submitAll()">Submit All</button>
</div>



⸻

3. Child Tab Component (e.g., case-tab.component.ts)

import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-case-tab',
  templateUrl: './case-tab.component.html'
})
export class CaseTabComponent implements OnInit {
  @Input() form!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.form.addControl('caseTitle', this.fb.control(''));
    this.form.addControl('description', this.fb.control(''));
    this.form.addControl('file', this.fb.control(null));
  }

  handleFile(file: File) {
    this.form.get('file')?.setValue(file);
  }
}



⸻

4. case-tab.component.html

<div [formGroup]="form">
  <div class="mb-3">
    <label>Case Title</label>
    <input formControlName="caseTitle" class="form-control">
  </div>
  <div class="mb-3">
    <label>Description</label>
    <textarea formControlName="description" class="form-control"></textarea>
  </div>
  <app-file-upload (fileSelected)="handleFile($event)"></app-file-upload>
</div>



⸻

5. File Upload (Reusable)

@Output() fileSelected = new EventEmitter<File>();
onFileChange(event: any) {
  const file = event.target.files[0];
  if (file) this.fileSelected.emit(file);
}



⸻

Advantages of This Approach
	•	Centralized control from parent
	•	Clean separation of concerns
	•	File uploads and form data for each tab stay together
	•	Easily extendable
	•	Reusable for draft save and final submit APIs

⸻

Would you like a working StackBlitz link or sample JSON payload for the unified form?