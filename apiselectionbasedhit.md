To achieve this, you can create a function in Angular that checks the user selection and hits the respective API based on the condition. After fetching data from the API, navigate to the alldetails case details page.

Here’s a sample approach in Angular:

	1.	Define Enum or Constants for Case Types: Define constants or enums to represent each case type for readability.
	2.	Service Function for API Calls: Implement a function in your service to handle API calls based on the type selected.
	3.	Component Logic: In your component, create a function to determine the selected case type, call the respective API, and navigate to the details page after the data is retrieved.

Here’s an example code structure:

// case-types.enum.ts
export enum CaseType {
  Enhancement = 'Enhancement',
  Freshcase = 'Freshcase',
  Revision = 'Revision',
  Additional = 'Additional'
}

// case.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CaseType } from './case-types.enum';

@Injectable({
  providedIn: 'root',
})
export class CaseService {
  constructor(private http: HttpClient) {}

  getCaseData(caseType: CaseType): Observable<any> {
    let apiUrl = '';

    switch (caseType) {
      case CaseType.Enhancement:
        apiUrl = '/api/enhancement';
        break;
      case CaseType.Freshcase:
        apiUrl = '/api/freshcase';
        break;
      case CaseType.Revision:
        apiUrl = '/api/revision';
        break;
      case CaseType.Additional:
        apiUrl = '/api/additional';
        break;
    }

    return this.http.get(apiUrl);
  }
}

// your-component.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CaseService } from './case.service';
import { CaseType } from './case-types.enum';

@Component({
  selector: 'app-your-component',
  templateUrl: './your-component.component.html',
})
export class YourComponent {
  selectedCaseType: CaseType;

  constructor(private caseService: CaseService, private router: Router) {}

  onSelectionChange(caseType: CaseType) {
    this.selectedCaseType = caseType;
  }

  fetchDataAndNavigate() {
    if (this.selectedCaseType) {
      this.caseService.getCaseData(this.selectedCaseType).subscribe(
        (data) => {
          // Navigate to case details page with the necessary data if needed
          this.router.navigate(['/alldetails'], {
            queryParams: { caseType: this.selectedCaseType },
          });
        },
        (error) => {
          console.error('Error fetching data:', error);
        }
      );
    } else {
      console.warn('No case type selected');
    }
  }
}

Explanation

	1.	Enum CaseType: Helps to maintain readability and avoids hardcoding case types.
	2.	Service Method getCaseData: Switches between APIs based on CaseType and makes a GET request to fetch data.
	3.	Component Logic: Based on the user selection (onSelectionChange), it hits the corresponding API and navigates to /alldetails after data retrieval.

This structure ensures your app makes the correct API request based on the user’s selection and navigates to the required page with the relevant data.