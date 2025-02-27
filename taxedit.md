Here’s the Tax Details Page for hospitals, similar to the Bank Details Page. It includes:

✅ Tax Details Display Page
✅ Edit Button to Open the Tax Details Edit Page
✅ Edit Page with Reactive Forms
✅ Save & Submit Functionality

1️⃣ Create the Tax Details Components

Run the following commands:

ng generate component tax-details
ng generate component tax-details-edit
ng generate service tax

2️⃣ Define Routing for Navigation

Modify app-routing.module.ts:

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TaxDetailsComponent } from './tax-details/tax-details.component';
import { TaxDetailsEditComponent } from './tax-details-edit/tax-details-edit.component';

const routes: Routes = [
  { path: 'tax-details', component: TaxDetailsComponent },
  { path: 'tax-details/edit', component: TaxDetailsEditComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

3️⃣ Tax Details Display Page

Displays tax details. Clicking “Edit” navigates to the Edit Page.

📌 tax-details.component.ts

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TaxService } from '../services/tax.service';

@Component({
  selector: 'app-tax-details',
  templateUrl: './tax-details.component.html',
  styleUrls: ['./tax-details.component.css']
})
export class TaxDetailsComponent implements OnInit {
  taxDetails: any;

  constructor(private taxService: TaxService, private router: Router) {}

  ngOnInit(): void {
    this.loadTaxDetails();
  }

  loadTaxDetails() {
    this.taxService.getTaxDetails().subscribe(data => {
      this.taxDetails = data;
    });
  }

  editTaxDetails() {
    this.router.navigate(['/tax-details/edit']);
  }
}

📌 tax-details.component.html

<div class="container mt-4">
  <h2 class="mb-4">Tax Details</h2>

  <table class="table table-bordered">
    <tbody>
      <tr>
        <th>GST Number</th>
        <td>{{ taxDetails?.gstNumber }}</td>
      </tr>
      <tr>
        <th>PAN Number</th>
        <td>{{ taxDetails?.panNumber }}</td>
      </tr>
      <tr>
        <th>Tax Identification Number (TIN)</th>
        <td>{{ taxDetails?.tinNumber }}</td>
      </tr>
    </tbody>
  </table>

  <button class="btn btn-primary" (click)="editTaxDetails()">Edit</button>
</div>

4️⃣ Tax Details Edit Page

Allows hospitals to edit and save their tax details.

📌 tax-details-edit.component.ts

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TaxService } from '../services/tax.service';

@Component({
  selector: 'app-tax-details-edit',
  templateUrl: './tax-details-edit.component.html',
  styleUrls: ['./tax-details-edit.component.css']
})
export class TaxDetailsEditComponent implements OnInit {
  taxForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private taxService: TaxService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadTaxData();
  }

  initForm() {
    this.taxForm = this.fb.group({
      gstNumber: ['', [Validators.required, Validators.pattern('^\\d{2}[A-Z]{5}\\d{4}[A-Z]{1}[A-Z\\d]{1}[Z]{1}[A-Z\\d]{1}$')]],
      panNumber: ['', [Validators.required, Validators.pattern('^[A-Z]{5}[0-9]{4}[A-Z]{1}$')]],
      tinNumber: ['', Validators.required]
    });
  }

  loadTaxData() {
    this.taxService.getTaxDetails().subscribe(data => {
      this.taxForm.patchValue(data);
    });
  }

  saveTaxDetails() {
    if (this.taxForm.invalid) return;

    this.taxService.updateTaxDetails(this.taxForm.value).subscribe({
      next: () => {
        alert('Tax details updated successfully');
        this.router.navigate(['/tax-details']); // Redirect back
      },
      error: (err) => console.error('Update failed', err)
    });
  }
}

📌 tax-details-edit.component.html

<div class="container mt-4">
  <h2 class="mb-4">Edit Tax Details</h2>

  <form [formGroup]="taxForm" (ngSubmit)="saveTaxDetails()">
    <div class="mb-3">
      <label class="form-label">GST Number</label>
      <input type="text" class="form-control" formControlName="gstNumber">
      <div *ngIf="taxForm.get('gstNumber')?.invalid && taxForm.get('gstNumber')?.touched" class="text-danger">
        Invalid GST Number
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label">PAN Number</label>
      <input type="text" class="form-control" formControlName="panNumber">
      <div *ngIf="taxForm.get('panNumber')?.invalid && taxForm.get('panNumber')?.touched" class="text-danger">
        Invalid PAN Number
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label">Tax Identification Number (TIN)</label>
      <input type="text" class="form-control" formControlName="tinNumber">
    </div>

    <button type="submit" class="btn btn-success" [disabled]="taxForm.invalid">Save & Submit</button>
  </form>
</div>

5️⃣ Tax Service

Handles API calls for fetching and updating tax details.

📌 tax.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaxService {
  private apiUrl = 'https://api.example.com/tax-details';

  constructor(private http: HttpClient) {}

  getTaxDetails(): Observable<any> {
    return this.http.get(`${this.apiUrl}`);
  }

  updateTaxDetails(data: any): Observable<any> {
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

	1.	Tax Details Page (/tax-details)
	•	Displays tax details.
	•	Clicking “Edit” navigates to the Edit Page.
	2.	Tax Details Edit Page (/tax-details/edit)
	•	Prefills existing data for editing.
	•	User updates the details and clicks “Save & Submit”.
	•	Updates are sent to the API.
	•	Redirects back to tax details page after saving.

✅ Features

	•	Bootstrap 5 for UI
	•	Reactive Forms with Validation
	•	GST & PAN Number Validation
	•	Navigation Between Pages
	•	API Integration

Would you like real-time validation messages, confirmation modals, or additional features?