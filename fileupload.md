To implement this functionality in Angular, you can follow these steps:

Requirements

	1.	Fetch the document names list from an API.
	2.	Allow users to select documents for upload.
	3.	Create a stepper above to indicate the steps: Document Selection → File Upload → Submission.
	4.	Upload selected documents.
	5.	Submit the data to an API.

Here’s how you can do it:

Step 1: Create Angular Components

	1.	Generate the component for your document upload module.

ng generate component document-upload


	2.	Install Angular Material (for the stepper).

ng add @angular/material



Step 2: HTML Structure

Create the UI structure with a stepper, file upload, and submit button.

<mat-horizontal-stepper [linear]="true" #stepper>
  <!-- Step 1: Document Selection -->
  <mat-step [stepControl]="documentForm">
    <form [formGroup]="documentForm">
      <ng-template matStepLabel>Select Documents</ng-template>
      <mat-list>
        <mat-list-item *ngFor="let document of documents">
          <mat-checkbox
            [checked]="document.selected"
            (change)="onDocumentSelect(document, $event)"
          >
            {{ document.name }}
          </mat-checkbox>
        </mat-list-item>
      </mat-list>
      <button mat-button matStepperNext [disabled]="!selectedDocuments.length">
        Next
      </button>
    </form>
  </mat-step>

  <!-- Step 2: File Upload -->
  <mat-step [stepControl]="uploadForm">
    <form [formGroup]="uploadForm">
      <ng-template matStepLabel>Upload Files</ng-template>
      <div *ngFor="let document of selectedDocuments">
        <label>{{ document.name }}</label>
        <input
          type="file"
          (change)="onFileChange(document, $event)"
          required
        />
      </div>
      <button mat-button matStepperPrevious>Back</button>
      <button mat-button matStepperNext [disabled]="!isAllFilesUploaded()">
        Next
      </button>
    </form>
  </mat-step>

  <!-- Step 3: Submit -->
  <mat-step>
    <ng-template matStepLabel>Submit</ng-template>
    <p>Review your selections and uploaded files.</p>
    <button mat-button matStepperPrevious>Back</button>
    <button mat-button (click)="submit()">Submit</button>
  </mat-step>
</mat-horizontal-stepper>

Step 3: Component Logic

Write the TypeScript logic for fetching documents, handling selection, file upload, and submission.

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-document-upload',
  templateUrl: './document-upload.component.html',
  styleUrls: ['./document-upload.component.css'],
})
export class DocumentUploadComponent implements OnInit {
  documentForm!: FormGroup;
  uploadForm!: FormGroup;

  documents: any[] = [];
  selectedDocuments: any[] = [];
  uploadedFiles: { [key: string]: File } = {};

  constructor(private fb: FormBuilder, private http: HttpClient) {}

  ngOnInit(): void {
    // Initialize forms
    this.documentForm = this.fb.group({});
    this.uploadForm = this.fb.group({});

    // Fetch documents list
    this.http.get<any[]>('api/documents').subscribe((data) => {
      this.documents = data.map((doc) => ({ ...doc, selected: false }));
    });
  }

  onDocumentSelect(document: any, event: any) {
    document.selected = event.checked;
    if (event.checked) {
      this.selectedDocuments.push(document);
    } else {
      this.selectedDocuments = this.selectedDocuments.filter(
        (d) => d.id !== document.id
      );
    }
  }

  onFileChange(document: any, event: any) {
    const file = event.target.files[0];
    if (file) {
      this.uploadedFiles[document.id] = file;
    }
  }

  isAllFilesUploaded() {
    return this.selectedDocuments.every((doc) => this.uploadedFiles[doc.id]);
  }

  submit() {
    const formData = new FormData();

    this.selectedDocuments.forEach((doc) => {
      formData.append('files', this.uploadedFiles[doc.id]);
    });

    this.http.post('api/upload', formData).subscribe({
      next: (res) => alert('Files uploaded successfully'),
      error: (err) => alert('Error uploading files'),
    });
  }
}

Step 4: API Example

	•	GET /api/documents: Returns the list of documents.

[
  { "id": 1, "name": "Passport" },
  { "id": 2, "name": "Driving License" },
  { "id": 3, "name": "Utility Bill" }
]


	•	POST /api/upload: Accepts a FormData object with files.

Step 5: Style the UI

Use Angular Material styles or custom styles to enhance the appearance of the stepper and forms.

This implementation ensures a user-friendly interface for selecting documents, uploading them, and submitting them to the API.