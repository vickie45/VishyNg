Here’s the full implementation for your Angular project.

1. Creating the Parent Component (AlDetailsComponent)

Start by generating the parent component and the tab components.

ng generate component aldetails
ng generate component case-details-tab
ng generate component cost-details-tab
ng generate component past-medical-tab
ng generate component maternity-tab
ng generate component accidental-tab

2. aldetails.component.ts

This will be your parent component, fetching data and handling form submission.

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { InwardSaveService } from '../services/inward-save.service';

@Component({
  selector: 'app-aldetails',
  templateUrl: './aldetails.component.html',
  styleUrls: ['./aldetails.component.scss']
})
export class AlDetailsComponent implements OnInit {
  alDetailsForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private inwardSaveService: InwardSaveService
  ) {}

  ngOnInit(): void {
    this.createForm();
    this.fetchInwardSaveDetails();
  }

  createForm(): void {
    // Parent form to hold all tab forms
    this.alDetailsForm = this.fb.group({
      caseDetails: null,
      costDetails: null,
      pastMedical: null,
      maternity: null,
      accidental: null
    });
  }

  fetchInwardSaveDetails(): void {
    this.inwardSaveService.getDetails().subscribe((data) => {
      // Populate the form with data from API
      this.alDetailsForm.patchValue({
        caseDetails: data.caseDetails,
        costDetails: data.costDetails,
        pastMedical: data.pastMedical,
        maternity: data.maternity,
        accidental: data.accidental
      });
    });
  }

  onSubmit(): void {
    if (this.alDetailsForm.valid) {
      this.inwardSaveService.submitDetails(this.alDetailsForm.value).subscribe((response) => {
        console.log('Form Submitted:', response);
      });
    }
  }
}

3. aldetails.component.html

This will define how the parent component and its child components (tabs) are structured in the UI.

<div class="container">
  <form [formGroup]="alDetailsForm" (ngSubmit)="onSubmit()">
    <!-- Tab Components -->
    <app-case-details-tab [parentForm]="alDetailsForm"></app-case-details-tab>
    <app-cost-details-tab [parentForm]="alDetailsForm"></app-cost-details-tab>
    <app-past-medical-tab [parentForm]="alDetailsForm"></app-past-medical-tab>
    <app-maternity-tab [parentForm]="alDetailsForm"></app-maternity-tab>
    <app-accidental-tab [parentForm]="alDetailsForm"></app-accidental-tab>

    <!-- Submit Button -->
    <button type="submit" class="btn btn-primary">Submit All</button>
  </form>
</div>

4. Each Tab Component

Each tab component (like case-details-tab) will receive the parentForm and handle its specific form fields.

case-details-tab.component.ts:

import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-case-details-tab',
  templateUrl: './case-details-tab.component.html',
  styleUrls: ['./case-details-tab.component.scss']
})
export class CaseDetailsTabComponent implements OnInit {
  @Input() parentForm: FormGroup;

  ngOnInit(): void {
    // Initialize the caseDetails form group within the parent form
    if (this.parentForm && !this.parentForm.get('caseDetails')) {
      this.parentForm.addControl('caseDetails', new FormGroup({}));
    }
  }
}

case-details-tab.component.html:

<div [formGroup]="parentForm.get('caseDetails')">
  <label for="caseName">Case Name</label>
  <input formControlName="caseName" type="text" id="caseName" class="form-control" />
</div>

You will need to repeat this pattern for each of the other tabs (cost-details-tab, past-medical-tab, maternity-tab, and accidental-tab).

5. API Service (InwardSaveService)

The service will handle API interactions, such as fetching and submitting form data.

inward-save.service.ts:

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InwardSaveService {
  private apiUrl = 'https://api.example.com/inwardsave'; // Replace with your actual API endpoint

  constructor(private http: HttpClient) {}

  getDetails(): Observable<any> {
    return this.http.get(`${this.apiUrl}`);
  }

  submitDetails(details: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/submit`, details);
  }
}

6. App Module (app.module.ts)

Ensure that you import the ReactiveFormsModule in your app.module.ts and add your components and service.

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { AlDetailsComponent } from './aldetails/aldetails.component';
import { CaseDetailsTabComponent } from './case-details-tab/case-details-tab.component';
import { InwardSaveService } from './services/inward-save.service';

@NgModule({
  declarations: [
    AppComponent,
    AlDetailsComponent,
    CaseDetailsTabComponent,
    // Declare other tab components here
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [InwardSaveService],
  bootstrap: [AppComponent]
})
export class AppModule { }

7. Testing the Application

Now, you should be able to fetch the shared details from the API and submit the full form across the tabs with the single submit button in the AlDetailsComponent.

Let me know if you need further adjustments or specific features!