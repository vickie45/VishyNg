Here’s an implementation using Bootstrap 5 with Angular 17 for the desired functionality:

HTML Structure

<div class="container mt-4">
  <!-- Form Section -->
  <div class="card mb-3">
    <div class="card-header">Upload Document</div>
    <div class="card-body">
      <form [formGroup]="uploadForm">
        <div class="mb-3">
          <label for="documentType" class="form-label">Select Document Type</label>
          <select
            id="documentType"
            class="form-select"
            formControlName="selectedDocument"
          >
            <option *ngFor="let document of documents" [value]="document.name">
              {{ document.name }}
            </option>
          </select>
        </div>

        <div class="mb-3">
          <label for="fileUpload" class="form-label">Upload File</label>
          <input
            type="file"
            id="fileUpload"
            class="form-control"
            (change)="onFileSelect($event)"
            accept=".pdf,.jpg,.jpeg,.png"
          />
        </div>

        <div class="form-check mb-3">
          <input
            class="form-check-input"
            type="checkbox"
            id="isOriginal"
            formControlName="isOriginal"
          />
          <label class="form-check-label" for="isOriginal">Is Original?</label>
        </div>

        <button
          class="btn btn-primary"
          [disabled]="!file || !uploadForm.value.selectedDocument"
          (click)="uploadFile()"
        >
          Upload
        </button>
      </form>
    </div>
  </div>

  <!-- Uploaded Documents Section -->
  <div class="card">
    <div class="card-header">Uploaded Documents</div>
    <div class="card-body">
      <table class="table table-bordered">
        <thead>
          <tr>
            <th>Document Name</th>
            <th>Is Original</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let doc of uploadedDocuments">
            <td>{{ doc.name }}</td>
            <td>{{ doc.isOriginal ? 'Yes' : 'No' }}</td>
            <td>
              <button
                class="btn btn-danger btn-sm"
                (click)="confirmDelete(doc)"
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <!-- Delete Confirmation Modal -->
  <div
    class="modal fade"
    id="deleteModal"
    tabindex="-1"
    aria-labelledby="deleteModalLabel"
    aria-hidden="true"
  >
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="deleteModalLabel">Delete Confirmation</h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body">
          Are you sure you want to delete this document?
        </div>
        <div class="modal-footer">
          <button
            type="button"
            class="btn btn-secondary"
            data-bs-dismiss="modal"
          >
            Cancel
          </button>
          <button type="button" class="btn btn-danger" (click)="deleteConfirmed()">
            Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</div>

Component Logic

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

declare var bootstrap: any;

@Component({
  selector: 'app-document-upload',
  templateUrl: './document-upload.component.html',
  styleUrls: ['./document-upload.component.css'],
})
export class DocumentUploadComponent implements OnInit {
  uploadForm!: FormGroup;
  documents: any[] = [];
  file: File | null = null;

  uploadedDocuments: any[] = [];
  documentToDelete: any = null; // Holds the document to delete

  constructor(private fb: FormBuilder, private http: HttpClient) {}

  ngOnInit(): void {
    // Initialize form
    this.uploadForm = this.fb.group({
      selectedDocument: [null, Validators.required],
      isOriginal: [false],
    });

    // Fetch document types from API
    this.http.get<any[]>('api/documents').subscribe((data) => {
      this.documents = data;
    });
  }

  onFileSelect(event: any): void {
    this.file = event.target.files[0];
  }

  uploadFile(): void {
    if (this.file && this.uploadForm.valid) {
      const formData = new FormData();
      formData.append('file', this.file);
      formData.append('documentType', this.uploadForm.value.selectedDocument);
      formData.append('isOriginal', this.uploadForm.value.isOriginal);

      this.http.post('api/upload', formData).subscribe((response: any) => {
        // Add to the uploaded documents table
        this.uploadedDocuments.push({
          name: this.uploadForm.value.selectedDocument,
          isOriginal: this.uploadForm.value.isOriginal,
        });

        // Reset the form and file input
        this.uploadForm.reset();
        this.file = null;
      });
    }
  }

  confirmDelete(document: any): void {
    this.documentToDelete = document;

    // Show delete confirmation modal
    const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    deleteModal.show();
  }

  deleteConfirmed(): void {
    if (this.documentToDelete) {
      this.http
        .delete(`api/delete/${this.documentToDelete.name}`)
        .subscribe(() => {
          this.uploadedDocuments = this.uploadedDocuments.filter(
            (d) => d.name !== this.documentToDelete.name
          );
          this.documentToDelete = null;
        });
    }
  }
}

Backend API

	1.	GET /api/documents: Fetches document types.

[
  { "id": 1, "name": "Passport" },
  { "id": 2, "name": "Driving License" }
]


	2.	POST /api/upload: Accepts the file and metadata.

{
  "file": "<binary data>",
  "documentType": "Passport",
  "isOriginal": true
}


	3.	DELETE /api/delete/:documentName: Deletes the document.

Bootstrap Modal Logic

The delete confirmation modal uses Bootstrap 5 Modals. Ensure you include the Bootstrap 5 JavaScript and CSS in your index.html:

<!-- Bootstrap 5 CSS -->
<link
  href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha3/dist/css/bootstrap.min.css"
  rel="stylesheet"
/>
<!-- Bootstrap 5 JS -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha3/dist/js/bootstrap.bundle.min.js"></script>

Final Features

	1.	The user selects a document type, uploads a file, and checks if it’s original.
	2.	Uploaded documents appear in a table with “Yes/No” for the “Is Original” column.
	3.	Clicking the delete button shows a confirmation modal before deleting the document.
	4.	The system interacts with APIs for document fetching, uploading, and deletion.