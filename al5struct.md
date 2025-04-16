Here is a clean Angular implementation for a “All Details Form” with five tabs (Case, Cost, Maternity, Injury, Others), where each tab is a separate component, includes a file upload component at the bottom, and has Save and Submit buttons.

⸻

1. Project Structure

src/
  app/
    all-details/
      all-details.component.ts/html/css
      case-tab/
        case-tab.component.ts/html/css
      cost-tab/
        cost-tab.component.ts/html/css
      maternity-tab/
        maternity-tab.component.ts/html/css
      injury-tab/
        injury-tab.component.ts/html/css
      others-tab/
        others-tab.component.ts/html/css
      shared/
        file-upload.component.ts/html/css



⸻

2. all-details.component.html

<ul class="nav nav-tabs">
  <li class="nav-item" *ngFor="let tab of tabs; let i = index">
    <a class="nav-link" [class.active]="activeTab === i" (click)="activeTab = i">{{ tab }}</a>
  </li>
</ul>

<div class="tab-content mt-3">
  <app-case-tab *ngIf="activeTab === 0"></app-case-tab>
  <app-cost-tab *ngIf="activeTab === 1"></app-cost-tab>
  <app-maternity-tab *ngIf="activeTab === 2"></app-maternity-tab>
  <app-injury-tab *ngIf="activeTab === 3"></app-injury-tab>
  <app-others-tab *ngIf="activeTab === 4"></app-others-tab>
</div>



⸻

3. all-details.component.ts

import { Component } from '@angular/core';

@Component({
  selector: 'app-all-details',
  templateUrl: './all-details.component.html',
  styleUrls: ['./all-details.component.css']
})
export class AllDetailsComponent {
  tabs = ['Case', 'Cost', 'Maternity', 'Injury', 'Others'];
  activeTab = 0;
}



⸻

4. Example: case-tab.component.html

(Other tabs will have similar structure.)

<form (ngSubmit)="submit()" #caseForm="ngForm">
  <!-- Add form fields specific to 'Case' -->
  <div class="mb-3">
    <label>Case Title</label>
    <input type="text" class="form-control" name="caseTitle" [(ngModel)]="form.caseTitle">
  </div>

  <!-- Shared File Upload Component -->
  <app-file-upload (fileSelected)="handleFile($event)"></app-file-upload>

  <div class="d-flex justify-content-end gap-2 mt-3">
    <button type="button" class="btn btn-secondary" (click)="save()">Save</button>
    <button type="submit" class="btn btn-primary">Submit</button>
  </div>
</form>



⸻

5. case-tab.component.ts

import { Component } from '@angular/core';

@Component({
  selector: 'app-case-tab',
  templateUrl: './case-tab.component.html',
  styleUrls: ['./case-tab.component.css']
})
export class CaseTabComponent {
  form = {
    caseTitle: '',
    file: null
  };

  handleFile(file: File) {
    this.form.file = file;
  }

  save() {
    console.log('Case Saved:', this.form);
    // Optional: Call API to save as draft
  }

  submit() {
    console.log('Case Submitted:', this.form);
    // Optional: Call API to submit
  }
}



⸻

6. shared/file-upload.component.html

<div class="mb-3">
  <label class="form-label">Upload File</label>
  <input type="file" class="form-control" (change)="onFileChange($event)">
</div>



⸻

7. file-upload.component.ts

import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent {
  @Output() fileSelected = new EventEmitter<File>();

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.fileSelected.emit(file);
    }
  }
}



⸻

8. Notes
	•	You can replicate the case-tab component structure for cost-tab, maternity-tab, injury-tab, and others-tab.
	•	Use Angular services to manage the shared form state or perform API integration.
	•	Add file type and size validation if required.

Would you like me to generate all the tab components’ boilerplate code as well?