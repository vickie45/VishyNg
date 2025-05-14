Here is the enhanced Angular 17 file upload component with:
	•	Validations (file type, size, duplicate prevention)
	•	Error handling
	•	Multiple file selection
	•	API upload simulation (replace with real HttpClient)
	•	User feedback messages

⸻

1. TypeScript: file-upload.component.ts

import { Component } from '@angular/core';

interface DocumentType {
  label: string;
  value: string;
}

interface UploadFile {
  file: File;
  docType: string;
  isOriginal: boolean;
  error?: string;
}

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent {
  documentTypes: DocumentType[] = [
    { label: 'Preauth Form', value: 'preauth' },
    { label: 'ID Proof', value: 'idproof' },
    { label: 'Bills', value: 'bills' }
  ];

  selectedDocType: string = '';
  selectedFiles: UploadFile[] = [];
  uploadedFiles: UploadFile[] = [];
  errorMessages: string[] = [];
  maxFileSizeMB: number = 5;
  allowedTypes = ['image/jpeg', 'image/png', 'application/pdf'];

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files;
    this.errorMessages = [];

    if (!files || !this.selectedDocType) return;

    Array.from(files).forEach(file => {
      const duplicate = this.selectedFiles.some(f => f.file.name === file.name && f.docType === this.selectedDocType);

      if (duplicate) {
        this.errorMessages.push(`Duplicate file "${file.name}" for selected document type.`);
        return;
      }

      if (!this.allowedTypes.includes(file.type)) {
        this.errorMessages.push(`Invalid file type for "${file.name}". Allowed: JPG, PNG, PDF.`);
        return;
      }

      const sizeMB = file.size / (1024 * 1024);
      if (sizeMB > this.maxFileSizeMB) {
        this.errorMessages.push(`"${file.name}" exceeds the size limit of ${this.maxFileSizeMB}MB.`);
        return;
      }

      this.selectedFiles.push({
        file,
        docType: this.selectedDocType,
        isOriginal: true
      });
    });

    input.value = '';
  }

  removeFromSelected(index: number): void {
    this.selectedFiles.splice(index, 1);
  }

  async uploadAll(): Promise<void> {
    this.errorMessages = [];

    if (this.selectedFiles.length === 0) {
      this.errorMessages.push('No files to upload.');
      return;
    }

    try {
      for (const item of this.selectedFiles) {
        const formData = new FormData();
        formData.append('file', item.file);
        formData.append('docType', item.docType);
        formData.append('isOriginal', String(item.isOriginal));

        // Replace this with HttpClient API call
        await new Promise(resolve => setTimeout(resolve, 500));

        this.uploadedFiles.push(item);
      }

      this.selectedFiles = [];
    } catch (err) {
      this.errorMessages.push('Failed to upload files. Please try again.');
    }
  }

  deleteUploaded(index: number): void {
    this.uploadedFiles.splice(index, 1);
  }
}


⸻

2. HTML Template: file-upload.component.html

<div class="upload-container">
  <h3>Upload Documents</h3>

  <!-- Select Document Type -->
  <label for="docType">Select Document Type:</label>
  <select [(ngModel)]="selectedDocType" id="docType" required>
    <option value="" disabled selected>Select type</option>
    <option *ngFor="let type of documentTypes" [value]="type.value">{{ type.label }}</option>
  </select>

  <!-- File Input -->
  <input type="file" (change)="onFileSelected($event)" [disabled]="!selectedDocType" multiple />

  <!-- Error Messages -->
  <div class="error" *ngFor="let msg of errorMessages">
    {{ msg }}
  </div>

  <!-- Selected Files -->
  <h4>Selected Documents to Upload</h4>
  <div *ngIf="selectedFiles.length === 0">No files selected.</div>
  <ul>
    <li *ngFor="let file of selectedFiles; let i = index">
      <strong>{{ file.file.name }}</strong> ({{ file.docType }})
      <label>
        Is Original:
        <input type="checkbox" [(ngModel)]="file.isOriginal" />
      </label>
      <button (click)="removeFromSelected(i)">Delete</button>
    </li>
  </ul>

  <!-- Upload Button -->
  <button (click)="uploadAll()" [disabled]="selectedFiles.length === 0">Upload All</button>

  <hr />

  <!-- Uploaded Files -->
  <h4>Uploaded Documents</h4>
  <div *ngIf="uploadedFiles.length === 0">No documents uploaded.</div>
  <ul>
    <li *ngFor="let file of uploadedFiles; let i = index">
      <strong>{{ file.file.name }}</strong> ({{ file.docType }}) - Original: {{ file.isOriginal ? 'Yes' : 'No' }}
      <button (click)="deleteUploaded(i)">Delete</button>
    </li>
  </ul>
</div>


⸻

3. CSS (optional): file-upload.component.scss

.upload-container {
  max-width: 600px;
  margin: auto;
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 6px;
  background-color: #fafafa;
}

input[type='file'],
select {
  margin: 10px 0;
  display: block;
}

.error {
  color: red;
  margin-top: 5px;
}

ul {
  list-style: none;
  padding: 0;
}

li {
  padding: 8px 0;
  border-bottom: 1px solid #ccc;
}


⸻

Next Steps
	•	Use Angular HttpClient to integrate the real upload API.
	•	Add progress indicators if needed.
	•	Store document metadata (name, type, size, upload time) in the backend if required.

Would you like the real ASP.NET Core Web API backend now?