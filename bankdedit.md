Here’s a Bank Details Page where hospitals can fill in their bank details. It includes:

✅ Bank Details Display Page
✅ Edit Button to Open the Bank Details Edit Page
✅ Edit Page with Reactive Forms
✅ Save & Submit Functionality

1️⃣ Create the Bank Details Components

Run the following commands:

ng generate component bank-details
ng generate component bank-details-edit
ng generate service bank

2️⃣ Define Routing for Navigation

Modify app-routing.module.ts:

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BankDetailsComponent } from './bank-details/bank-details.component';
import { BankDetailsEditComponent } from './bank-details-edit/bank-details-edit.component';

const routes: Routes = [
  { path: 'bank-details', component: BankDetailsComponent },
  { path: 'bank-details/edit', component: BankDetailsEditComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

3️⃣ Bank Details Display Page

Displays bank details. Clicking “Edit” navigates to the Edit Page.

📌 bank-details.component.ts

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BankService } from '../services/bank.service';

@Component({
  selector: 'app-bank-details',
  templateUrl: './bank-details.component.html',
  styleUrls: ['./bank-details.component.css']
})
export class BankDetailsComponent implements OnInit {
  bankDetails: any;

  constructor(private bankService: BankService, private router: Router) {}

  ngOnInit(): void {
    this.loadBankDetails();
  }

  loadBankDetails() {
    this.bankService.getBankDetails().subscribe(data => {
      this.bankDetails = data;
    });
  }

  editBankDetails() {
    this.router.navigate(['/bank-details/edit']);
  }
}

📌 bank-details.component.html

<div class="container mt-4">
  <h2 class="mb-4">Bank Details</h2>

  <table class="table table-bordered">
    <tbody>
      <tr>
        <th>Bank Name</th>
        <td>{{ bankDetails?.bankName }}</td>
      </tr>
      <tr>
        <th>Branch</th>
        <td>{{ bankDetails?.branch }}</td>
      </tr>
      <tr>
        <th>Account Number</th>
        <td>{{ bankDetails?.accountNumber }}</td>
      </tr>
      <tr>
        <th>IFSC Code</th>
        <td>{{ bankDetails?.ifscCode }}</td>
      </tr>
      <tr>
        <th>Account Holder Name</th>
        <td>{{ bankDetails?.accountHolderName }}</td>
      </tr>
    </tbody>
  </table>

  <button class="btn btn-primary" (click)="editBankDetails()">Edit</button>
</div>

4️⃣ Bank Details Edit Page

Allows hospitals to edit and save their bank details.

📌 bank-details-edit.component.ts

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BankService } from '../services/bank.service';

@Component({
  selector: 'app-bank-details-edit',
  templateUrl: './bank-details-edit.component.html',
  styleUrls: ['./bank-details-edit.component.css']
})
export class BankDetailsEditComponent implements OnInit {
  bankForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private bankService: BankService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadBankData();
  }

  initForm() {
    this.bankForm = this.fb.group({
      bankName: ['', Validators.required],
      branch: ['', Validators.required],
      accountNumber: ['', [Validators.required, Validators.pattern('^[0-9]{9,18}$')]],
      ifscCode: ['', [Validators.required, Validators.pattern('^[A-Z]{4}0[A-Z0-9]{6}$')]],
      accountHolderName: ['', Validators.required]
    });
  }

  loadBankData() {
    this.bankService.getBankDetails().subscribe(data => {
      this.bankForm.patchValue(data);
    });
  }

  saveBankDetails() {
    if (this.bankForm.invalid) return;

    this.bankService.updateBankDetails(this.bankForm.value).subscribe({
      next: () => {
        alert('Bank details updated successfully');
        this.router.navigate(['/bank-details']); // Redirect back
      },
      error: (err) => console.error('Update failed', err)
    });
  }
}

📌 bank-details-edit.component.html

<div class="container mt-4">
  <h2 class="mb-4">Edit Bank Details</h2>

  <form [formGroup]="bankForm" (ngSubmit)="saveBankDetails()">
    <div class="mb-3">
      <label class="form-label">Bank Name</label>
      <input type="text" class="form-control" formControlName="bankName">
    </div>

    <div class="mb-3">
      <label class="form-label">Branch</label>
      <input type="text" class="form-control" formControlName="branch">
    </div>

    <div class="mb-3">
      <label class="form-label">Account Number</label>
      <input type="text" class="form-control" formControlName="accountNumber">
    </div>

    <div class="mb-3">
      <label class="form-label">IFSC Code</label>
      <input type="text" class="form-control" formControlName="ifscCode">
    </div>

    <div class="mb-3">
      <label class="form-label">Account Holder Name</label>
      <input type="text" class="form-control" formControlName="accountHolderName">
    </div>

    <button type="submit" class="btn btn-success" [disabled]="bankForm.invalid">Save & Submit</button>
  </form>
</div>

5️⃣ Bank Service

Handles API calls for fetching and updating bank details.

📌 bank.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  private apiUrl = 'https://api.example.com/bank-details';

  constructor(private http: HttpClient) {}

  getBankDetails(): Observable<any> {
    return this.http.get(`${this.apiUrl}`);
  }

  updateBankDetails(data: any): Observable<any> {
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

	1.	Bank Details Page (/bank-details)
	•	Displays bank details.
	•	Clicking “Edit” navigates to the Edit Page.
	2.	Bank Details Edit Page (/bank-details/edit)
	•	Prefills existing data for editing.
	•	User updates the details and clicks “Save & Submit”.
	•	Updates are sent to the API.
	•	Redirects back to bank details page after saving.

✅ Features

	•	Bootstrap 5 for UI
	•	Reactive Forms with Validation
	•	Bank Account Number & IFSC Code Validation
	•	Navigation Between Pages
	•	API Integration

Would you like real-time validation messages, success popups, or additional security checks?