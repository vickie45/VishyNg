Here’s the Infrastructure Details Display Page with an Edit Button, which navigates to an Infrastructure Edit Page for updating details. It includes:

✅ Infrastructure Display Page
✅ Edit Button to Navigate to Edit Page
✅ Edit Page with Reactive Forms
✅ Save & Submit Functionality

1️⃣ Create the Infrastructure Components

Run the following commands:

ng generate component infrastructure-details
ng generate component infrastructure-edit
ng generate service infrastructure

2️⃣ Define Routing for Navigation

Modify app-routing.module.ts:

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InfrastructureDetailsComponent } from './infrastructure-details/infrastructure-details.component';
import { InfrastructureEditComponent } from './infrastructure-edit/infrastructure-edit.component';

const routes: Routes = [
  { path: 'infrastructure', component: InfrastructureDetailsComponent },
  { path: 'infrastructure/edit', component: InfrastructureEditComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

3️⃣ Infrastructure Details Display Page

This page fetches infrastructure details and displays them in a table. Clicking “Edit” navigates to the Edit Page.

📌 infrastructure-details.component.ts

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { InfrastructureService } from '../services/infrastructure.service';

@Component({
  selector: 'app-infrastructure-details',
  templateUrl: './infrastructure-details.component.html',
  styleUrls: ['./infrastructure-details.component.css']
})
export class InfrastructureDetailsComponent implements OnInit {
  infrastructure: any;

  constructor(private infraService: InfrastructureService, private router: Router) {}

  ngOnInit(): void {
    this.loadInfrastructureDetails();
  }

  loadInfrastructureDetails() {
    this.infraService.getInfrastructure().subscribe(data => {
      this.infrastructure = data;
    });
  }

  editInfrastructure() {
    this.router.navigate(['/infrastructure/edit']);
  }
}

📌 infrastructure-details.component.html

<div class="container mt-4">
  <h2 class="mb-4">Infrastructure Details</h2>

  <table class="table table-bordered">
    <tbody>
      <tr>
        <th>Building Name</th>
        <td>{{ infrastructure?.buildingName }}</td>
      </tr>
      <tr>
        <th>Floors</th>
        <td>{{ infrastructure?.floors }}</td>
      </tr>
      <tr>
        <th>Total Rooms</th>
        <td>{{ infrastructure?.totalRooms }}</td>
      </tr>
      <tr>
        <th>ICU Beds</th>
        <td>{{ infrastructure?.icuBeds }}</td>
      </tr>
      <tr>
        <th>Operation Theaters</th>
        <td>{{ infrastructure?.operationTheaters }}</td>
      </tr>
    </tbody>
  </table>

  <button class="btn btn-primary" (click)="editInfrastructure()">Edit</button>
</div>

4️⃣ Infrastructure Edit Page

This page retrieves existing details, allows editing, and saves the updated information.

📌 infrastructure-edit.component.ts

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { InfrastructureService } from '../services/infrastructure.service';

@Component({
  selector: 'app-infrastructure-edit',
  templateUrl: './infrastructure-edit.component.html',
  styleUrls: ['./infrastructure-edit.component.css']
})
export class InfrastructureEditComponent implements OnInit {
  infraForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private infraService: InfrastructureService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadInfrastructureData();
  }

  initForm() {
    this.infraForm = this.fb.group({
      buildingName: ['', Validators.required],
      floors: ['', [Validators.required, Validators.min(1)]],
      totalRooms: ['', [Validators.required, Validators.min(1)]],
      icuBeds: ['', [Validators.required, Validators.min(1)]],
      operationTheaters: ['', [Validators.required, Validators.min(1)]]
    });
  }

  loadInfrastructureData() {
    this.infraService.getInfrastructure().subscribe(data => {
      this.infraForm.patchValue(data);
    });
  }

  saveInfrastructure() {
    if (this.infraForm.invalid) return;

    this.infraService.updateInfrastructure(this.infraForm.value).subscribe({
      next: () => {
        alert('Infrastructure details updated successfully');
        this.router.navigate(['/infrastructure']); // Redirect back
      },
      error: (err) => console.error('Update failed', err)
    });
  }
}

📌 infrastructure-edit.component.html

<div class="container mt-4">
  <h2 class="mb-4">Edit Infrastructure Details</h2>

  <form [formGroup]="infraForm" (ngSubmit)="saveInfrastructure()">
    <div class="mb-3">
      <label class="form-label">Building Name</label>
      <input type="text" class="form-control" formControlName="buildingName">
    </div>

    <div class="mb-3">
      <label class="form-label">Floors</label>
      <input type="number" class="form-control" formControlName="floors">
    </div>

    <div class="mb-3">
      <label class="form-label">Total Rooms</label>
      <input type="number" class="form-control" formControlName="totalRooms">
    </div>

    <div class="mb-3">
      <label class="form-label">ICU Beds</label>
      <input type="number" class="form-control" formControlName="icuBeds">
    </div>

    <div class="mb-3">
      <label class="form-label">Operation Theaters</label>
      <input type="number" class="form-control" formControlName="operationTheaters">
    </div>

    <button type="submit" class="btn btn-success" [disabled]="infraForm.invalid">Save & Submit</button>
  </form>
</div>

5️⃣ Infrastructure Service

Handles API calls for retrieving and updating infrastructure details.

📌 infrastructure.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InfrastructureService {
  private apiUrl = 'https://api.example.com/infrastructure';

  constructor(private http: HttpClient) {}

  getInfrastructure(): Observable<any> {
    return this.http.get(`${this.apiUrl}`);
  }

  updateInfrastructure(data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}`, data);
  }
}

6️⃣ Register HttpClientModule

Import HttpClientModule in app.module.ts:

import { HttpClientModule } from '@angular/common/http';

@NgModule({
  imports: [HttpClientModule],
})
export class AppModule { }

🛠️ How It Works

	1.	Infrastructure Details Page (/infrastructure)
	•	Displays hospital infrastructure details.
	•	Clicking “Edit” navigates to the Edit Page.
	2.	Infrastructure Edit Page (/infrastructure/edit)
	•	Prefills data for editing.
	•	User updates the details and clicks “Save & Submit”.
	•	Updates are sent to the API.
	•	Redirects back to details page after a successful update.

✅ Features

	•	Bootstrap 5 for UI
	•	Reactive Forms with Validation
	•	Edit Page with Save & Submit
	•	Navigation Between Pages
	•	API Integration

Would you like any enhancements, such as real-time validation messages or a success popup?