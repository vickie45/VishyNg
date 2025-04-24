Here is a practical, step-by-step Angular 17 implementation to handle multiple file uploads with document type selection, validation, and API integration for upload and delete.

⸻

Step 1: Component Template (HTML)

<div class="container mt-4">

  <label for="docType">Select Document Type</label>
  <select [(ngModel)]="selectedDocType" class="form-select mb-3">
    <option *ngFor="let doc of documentTypes" [value]="doc.type">
      {{ doc.label }}
    </option>
  </select>

  <input type="file" multiple (change)="onFileSelect($event)" class="form-control" />

  <div *ngIf="selectedFiles.length > 0" class="mt-3">
    <h5>Files to Upload</h5>
    <ul class="list-group">
      <li *ngFor="let file of selectedFiles; let i = index" class="list-group-item d-flex justify-content-between">
        {{ file.name }}
        <button class="btn btn-danger btn-sm" (click)="removeFile(i)">Delete</button>
      </li>
    </ul>
  </div>

  <button class="btn btn-primary mt-3" (click)="uploadFiles()">Upload</button>

  <div *ngIf="uploadedFiles.length > 0" class="mt-4">
    <h5>Uploaded Files</h5>
    <ul class="list-group">
      <li *ngFor="let file of uploadedFiles; let i = index" class="list-group-item d-flex justify-content-between">
        {{ file.name }}
        <button class="btn btn-outline-danger btn-sm" (click)="deleteUploadedFile(file, i)">Remove</button>
      </li>
    </ul>
  </div>

</div>



⸻

Step 2: Component Class (TS)

import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface DocumentType {
  type: string;
  label: string;
}

interface UploadedFile {
  name: string;
  id: string; // server-generated ID or file identifier
}

@Component({
  selector: 'app-multi-upload',
  templateUrl: './multi-upload.component.html',
})
export class MultiUploadComponent implements OnInit {

  documentTypes: DocumentType[] = [];
  selectedDocType: string = 'Preauth Form';
  selectedFiles: File[] = [];
  uploadedFiles: UploadedFile[] = [];

  allowedFileTypes = ['application/pdf', 'image/jpeg', 'image/png'];
  maxFileSize = 5 * 1024 * 1024; // 5MB

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchDocumentTypes();
  }

  fetchDocumentTypes() {
    this.http.get<DocumentType[]>('/api/document-types').subscribe(data => {
      this.documentTypes = data;
      if (!this.selectedDocType && data.length > 0) {
        this.selectedDocType = data[0].type || 'Preauth Form';
      }
    });
  }

  onFileSelect(event: any) {
    const files: FileList = event.target.files;
    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      if (this.validateFile(file)) {
        this.selectedFiles.push(file);
      }
    }
  }

  validateFile(file: File): boolean {
    if (file.size > this.maxFileSize) {
      alert(`${file.name} exceeds max file size`);
      return false;
    }
    if (!this.allowedFileTypes.includes(file.type)) {
      alert(`${file.name} has invalid file type`);
      return false;
    }
    return true;
  }

  removeFile(index: number) {
    this.selectedFiles.splice(index, 1);
  }

  uploadFiles() {
    const formData = new FormData();
    this.selectedFiles.forEach(file => formData.append('files', file));
    formData.append('documentType', this.selectedDocType);

    this.http.post<UploadedFile[]>('/api/upload', formData).subscribe({
      next: (uploaded) => {
        this.uploadedFiles = [...this.uploadedFiles, ...uploaded];
        this.selectedFiles = [];
      },
      error: (err) => {
        alert('Upload failed');
        console.error(err);
      }
    });
  }

  deleteUploadedFile(file: UploadedFile, index: number) {
    this.http.delete(`/api/delete/${file.id}`).subscribe({
      next: () => {
        this.uploadedFiles.splice(index, 1);
      },
      error: (err) => {
        alert('Failed to delete');
        console.error(err);
      }
    });
  }
}



⸻

Step 3: Backend API Assumptions
	•	GET /api/document-types: Returns list of document types like:

[{ "type": "PAN", "label": "PAN Card" }, { "type": "AADHAAR", "label": "Aadhaar" }]


	•	POST /api/upload: Accepts FormData with files and document type. Returns uploaded file metadata:

[{ "name": "file1.pdf", "id": "123" }, { "name": "file2.jpg", "id": "124" }]


	•	DELETE /api/delete/{id}: Deletes uploaded file with the given ID.

⸻

Extras (Optional Enhancements)
	•	Show upload progress with HttpClient reportProgress and observe: 'events'.
	•	Drag-and-drop support.
	•	Retry failed uploads.
	•	Confirmation dialog for deletes.

⸻

Shall I help you add this to your current Angular project structure with Bootstrap 5 layout integration?