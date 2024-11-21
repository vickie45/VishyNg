Here’s a guide to create an AldetailsComponent in Angular 17 with Bootstrap 5, including tabs for Case Details, Cost Details, Past Medical History, and conditional tabs for Maternity and Injury Accident based on checkbox selections.

Steps:

	1.	Create the Component:
Use the Angular CLI to generate the AldetailsComponent.

ng generate component aldetails


	2.	Setup Bootstrap:
Ensure Bootstrap 5 is added to your project. Install it via npm if not already done:

npm install bootstrap

Add Bootstrap’s CSS to your angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
]


	3.	Template for aldetails.component.html:
Use Bootstrap 5 tabs to organize the components.

<div class="container mt-4">
  <div class="form-check mb-3">
    <input 
      class="form-check-input" 
      type="checkbox" 
      id="maternityCheckbox" 
      [(ngModel)]="showMaternity" 
    />
    <label class="form-check-label" for="maternityCheckbox">Maternity</label>
  </div>

  <div class="form-check mb-3">
    <input 
      class="form-check-input" 
      type="checkbox" 
      id="injuryCheckbox" 
      [(ngModel)]="showInjury" 
    />
    <label class="form-check-label" for="injuryCheckbox">Injury Accident</label>
  </div>

  <ul class="nav nav-tabs" role="tablist">
    <li class="nav-item" role="presentation">
      <button 
        class="nav-link active" 
        id="case-tab" 
        data-bs-toggle="tab" 
        data-bs-target="#caseDetails" 
        type="button" 
        role="tab" 
        aria-controls="caseDetails" 
        aria-selected="true">
        Case Details
      </button>
    </li>
    <li class="nav-item" role="presentation">
      <button 
        class="nav-link" 
        id="cost-tab" 
        data-bs-toggle="tab" 
        data-bs-target="#costDetails" 
        type="button" 
        role="tab" 
        aria-controls="costDetails" 
        aria-selected="false">
        Cost Details
      </button>
    </li>
    <li class="nav-item" role="presentation">
      <button 
        class="nav-link" 
        id="past-tab" 
        data-bs-toggle="tab" 
        data-bs-target="#pastMedical" 
        type="button" 
        role="tab" 
        aria-controls="pastMedical" 
        aria-selected="false">
        Past Medical History
      </button>
    </li>
    <li 
      *ngIf="showMaternity" 
      class="nav-item" 
      role="presentation">
      <button 
        class="nav-link" 
        id="maternity-tab" 
        data-bs-toggle="tab" 
        data-bs-target="#maternityDetails" 
        type="button" 
        role="tab" 
        aria-controls="maternityDetails" 
        aria-selected="false">
        Maternity
      </button>
    </li>
    <li 
      *ngIf="showInjury" 
      class="nav-item" 
      role="presentation">
      <button 
        class="nav-link" 
        id="injury-tab" 
        data-bs-toggle="tab" 
        data-bs-target="#injuryDetails" 
        type="button" 
        role="tab" 
        aria-controls="injuryDetails" 
        aria-selected="false">
        Injury Accident
      </button>
    </li>
  </ul>

  <div class="tab-content mt-3">
    <div class="tab-pane fade show active" id="caseDetails" role="tabpanel" aria-labelledby="case-tab">
      <app-case-details></app-case-details>
    </div>
    <div class="tab-pane fade" id="costDetails" role="tabpanel" aria-labelledby="cost-tab">
      <app-cost-details></app-cost-details>
    </div>
    <div class="tab-pane fade" id="pastMedical" role="tabpanel" aria-labelledby="past-tab">
      <app-past-medical></app-past-medical>
    </div>
    <div *ngIf="showMaternity" class="tab-pane fade" id="maternityDetails" role="tabpanel" aria-labelledby="maternity-tab">
      <app-maternity></app-maternity>
    </div>
    <div *ngIf="showInjury" class="tab-pane fade" id="injuryDetails" role="tabpanel" aria-labelledby="injury-tab">
      <app-injury></app-injury>
    </div>
  </div>
</div>


	4.	Component Logic (aldetails.component.ts):
Manage the checkbox states and conditional tab visibility.

import { Component } from '@angular/core';

@Component({
  selector: 'app-aldetails',
  templateUrl: './aldetails.component.html',
  styleUrls: ['./aldetails.component.css'],
})
export class AldetailsComponent {
  showMaternity: boolean = false;
  showInjury: boolean = false;
}


	5.	Generate Child Components:
Use Angular CLI to generate the individual components.

ng generate component case-details
ng generate component cost-details
ng generate component past-medical
ng generate component maternity
ng generate component injury


	6.	Test the Application:
Serve the app and verify functionality.

ng serve



Explanation:

	•	Bootstrap Tabs: The nav-tabs and tab-content classes structure the tabs.
	•	Dynamic Tabs: *ngIf directives control the visibility of Maternity and Injury Accident tabs based on checkbox selections.
	•	Two-way Binding: [(ngModel)] binds the checkboxes to component properties.

Let me know if you need help with additional styling or functionality!

//shared file upload :

To add a File Upload Component as a shared component within the AldetailsComponent, follow these steps:

1. Generate File Upload Component

Use Angular CLI to generate the FileUploadComponent:

ng generate component shared/file-upload

2. File Upload Component Code

file-upload.component.html:

<div class="file-upload-container">
  <label for="fileInput" class="form-label">Upload File</label>
  <input 
    type="file" 
    id="fileInput" 
    class="form-control" 
    (change)="onFileSelected($event)" 
  />
  <div *ngIf="uploadedFile" class="mt-3">
    <strong>Uploaded File:</strong> {{ uploadedFile.name }} ({{ uploadedFile.size | number }} bytes)
  </div>
</div>

file-upload.component.ts:

import { Component } from '@angular/core';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css'],
})
export class FileUploadComponent {
  uploadedFile: File | null = null;

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.uploadedFile = input.files[0];
      console.log('File selected:', this.uploadedFile);
    }
  }
}

file-upload.component.css:

.file-upload-container {
  max-width: 400px;
}

3. Add File Upload to AldetailsComponent

Update aldetails.component.html:

Add the FileUploadComponent to the AldetailsComponent template, placing it below the tabs.

<div class="container mt-4">
  <!-- Existing code for tabs and components -->

  <div class="mt-5">
    <h5>File Upload</h5>
    <app-file-upload></app-file-upload>
  </div>
</div>

4. Export File Upload Component as Shared

To make the FileUploadComponent reusable in other parts of the app, export it via a shared module.

Create Shared Module:

Generate a shared module if it doesn’t already exist:

ng generate module shared

Update shared.module.ts:

Add FileUploadComponent to the declarations and exports:

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FileUploadComponent } from './file-upload/file-upload.component';

@NgModule({
  declarations: [FileUploadComponent],
  imports: [CommonModule],
  exports: [FileUploadComponent],
})
export class SharedModule {}

Import SharedModule in AppModule or Feature Module:

import { SharedModule } from './shared/shared.module';

@NgModule({
  imports: [
    SharedModule,
    // other imports
  ],
})
export class AppModule {}

5. Test the Integration

Run the application:

ng serve

Verify the following:

	1.	The File Upload component is displayed below the tabs.
	2.	Users can upload files, and the selected file’s details (name and size) are displayed.

Example UI Flow:

	•	Tabs: Allow navigation between the sections (Case Details, Cost Details, etc.).
	•	File Upload Section: Appears as a shared component under the tabs, ensuring reusability.

Let me know if you need further customizations!

