Got it! Here’s the revised implementation using Bootstrap 5 instead of Angular Material.

1. Install Bootstrap 5 in Angular

If you haven’t installed Bootstrap 5 yet, run:

npm install bootstrap

Then, add Bootstrap to angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]

2. Modify the Service for API Calls

communication.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CommunicationService {
  private apiUrl = 'https://api.example.com/communication'; // Replace with actual API

  constructor(private http: HttpClient) {}

  getCommunications(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  addCommunication(data: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }

  updateCommunication(id: number, data: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, data);
  }

  deleteCommunication(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

3. Communication Component

communication.component.ts

import { Component, OnInit } from '@angular/core';
import { CommunicationService } from '../services/communication.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-communication',
  templateUrl: './communication.component.html',
  styleUrls: ['./communication.component.css']
})
export class CommunicationComponent implements OnInit {
  communicationData: any[] = [];
  commForm!: FormGroup;
  isEdit = false;
  selectedId: number | null = null;

  constructor(private commService: CommunicationService, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.loadCommunications();
    this.commForm = this.fb.group({
      name: ['', Validators.required],
      contact: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  loadCommunications() {
    this.commService.getCommunications().subscribe(data => {
      this.communicationData = data;
    });
  }

  openModal(isEdit: boolean, communication?: any) {
    this.isEdit = isEdit;
    this.selectedId = isEdit ? communication.id : null;

    if (isEdit) {
      this.commForm.patchValue({
        name: communication.name,
        contact: communication.contact,
        email: communication.email,
      });
    } else {
      this.commForm.reset();
    }

    let modal = new bootstrap.Modal(document.getElementById('communicationModal')!);
    modal.show();
  }

  submitForm() {
    if (this.commForm.valid) {
      if (this.isEdit && this.selectedId !== null) {
        this.commService.updateCommunication(this.selectedId, this.commForm.value).subscribe(() => {
          this.loadCommunications();
          this.closeModal();
        });
      } else {
        this.commService.addCommunication(this.commForm.value).subscribe(() => {
          this.loadCommunications();
          this.closeModal();
        });
      }
    }
  }

  deleteCommunication(id: number) {
    if (confirm('Are you sure you want to delete this record?')) {
      this.commService.deleteCommunication(id).subscribe(() => {
        this.loadCommunications();
      });
    }
  }

  closeModal() {
    let modalElement = document.getElementById('communicationModal');
    let modalInstance = bootstrap.Modal.getInstance(modalElement!);
    modalInstance?.hide();
  }
}

4. Communication Component Template

communication.component.html

<div class="container mt-4">
  <div class="d-flex justify-content-between align-items-center mb-3">
    <h3>Communication Details</h3>
    <button class="btn btn-primary" (click)="openModal(false)">➕ Add Communication</button>
  </div>

  <table class="table table-bordered">
    <thead class="table-dark">
      <tr>
        <th>ID</th>
        <th>Name</th>
        <th>Contact</th>
        <th>Email</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let comm of communicationData">
        <td>{{ comm.id }}</td>
        <td>{{ comm.name }}</td>
        <td>{{ comm.contact }}</td>
        <td>{{ comm.email }}</td>
        <td>
          <button class="btn btn-warning btn-sm me-2" (click)="openModal(true, comm)">✏️ Edit</button>
          <button class="btn btn-danger btn-sm" (click)="deleteCommunication(comm.id)">🗑️ Delete</button>
        </td>
      </tr>
    </tbody>
  </table>
</div>

<!-- Bootstrap Modal -->
<div class="modal fade" id="communicationModal" tabindex="-1">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">{{ isEdit ? 'Edit Communication' : 'Add Communication' }}</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
      </div>
      <div class="modal-body">
        <form [formGroup]="commForm">
          <div class="mb-3">
            <label class="form-label">Name</label>
            <input type="text" class="form-control" formControlName="name">
          </div>
          <div class="mb-3">
            <label class="form-label">Contact</label>
            <input type="text" class="form-control" formControlName="contact">
          </div>
          <div class="mb-3">
            <label class="form-label">Email</label>
            <input type="email" class="form-control" formControlName="email">
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
        <button type="button" class="btn btn-primary" (click)="submitForm()" [disabled]="commForm.invalid">
          {{ isEdit ? 'Update' : 'Add' }}
        </button>
      </div>
    </div>
  </div>
</div>

5. Bootstrap 5 Modal Behavior

Bootstrap 5 modals require JavaScript to manage their instances dynamically. Ensure the modal opens and closes properly by controlling it in TypeScript.

6. Explanation of Features

✔ Fetches communication details and displays them in a Bootstrap-styled table.
✔ Add a new record using a modal with a reactive form.
✔ Edit an existing record by clicking the “Edit” button, filling the modal form with current values, and submitting the update.
✔ Delete a record by clicking the “Delete” button (with confirmation).
✔ Bootstrap 5-based UI for a modern, responsive look.

Would you like any more enhancements, such as pagination or search functionality?