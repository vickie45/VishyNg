Here's how you can write the Angular code for searching members based on either a `UHID` (Unique Health Identifier) or, if `UHID` is not provided, based on a combination of `policyNo` and `employeeId` along with other optional search fields like `patientRegistrationNo`, `policyName`, or `insuredName`.

### 1. Angular Service for Member Search

#### `member.service.ts`

This service is responsible for making HTTP requests to the backend API.

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  private apiUrl = 'http://localhost:3000/api/members';  // Adjust with your backend URL

  constructor(private http: HttpClient) { }

  searchMembers(uhid: string, policyNo?: string, employeeId?: string, patientRegistrationNo?: string, policyName?: string, insuredName?: string): Observable<any[]> {
    let params = new HttpParams();

    if (uhid) {
      // Search by UHID
      params = params.set('uhid', uhid);
    } else {
      // Search by other fields if UHID is not provided
      if (policyNo) params = params.set('policyNo', policyNo);
      if (employeeId) params = params.set('employeeId', employeeId);
      if (patientRegistrationNo) params = params.set('patientRegistrationNo', patientRegistrationNo);
      if (policyName) params = params.set('policyName', policyName);
      if (insuredName) params = params.set('insuredName', insuredName);
    }

    return this.http.get<any[]>(this.apiUrl, { params });
  }
}
```

### 2. Angular Component for Member Search

#### `member-search.component.ts`

The component contains the logic for the search form and the result display. It ensures that if a `UHID` is provided, it is prioritized, otherwise, it falls back to other search criteria.

```typescript
import { Component } from '@angular/core';
import { MemberService } from './member.service';

@Component({
  selector: 'app-member-search',
  templateUrl: './member-search.component.html',
  styleUrls: ['./member-search.component.css']
})
export class MemberSearchComponent {

  // Form input fields
  uhid: string = '';
  policyNo: string = '';
  employeeId: string = '';
  patientRegistrationNo: string = '';
  policyName: string = '';
  insuredName: string = '';

  // Result and error handling
  members: any[] = [];
  errorMessage: string = '';

  constructor(private memberService: MemberService) { }

  onSearch() {
    // Clear previous results and errors
    this.members = [];
    this.errorMessage = '';

    // Validate that either UHID or a valid combination of other fields is provided
    if (!this.uhid && (!this.policyNo || !this.employeeId) && (!this.patientRegistrationNo && !this.policyName && !this.insuredName)) {
      this.errorMessage = 'Please provide UHID or Policy No and Employee ID, or other search criteria.';
      return;
    }

    // Perform the search using the service
    this.memberService.searchMembers(
      this.uhid,
      this.policyNo,
      this.employeeId,
      this.patientRegistrationNo,
      this.policyName,
      this.insuredName
    ).subscribe({
      next: (data) => {
        if (data.length > 0) {
          this.members = data;
        } else {
          this.errorMessage = 'No members found matching the criteria.';
        }
      },
      error: (err) => {
        this.errorMessage = 'Error fetching member data.';
      }
    });
  }
}
```

### Explanation of Logic

1. **Form Validation**: 
   - If `UHID` is provided, it searches using only `UHID`.
   - If `UHID` is not provided, it requires both `policyNo` and `employeeId` as mandatory fields for the search. The other fields (`patientRegistrationNo`, `policyName`, `insuredName`) are optional and can be used as further filters.
   - If neither of these conditions are met, an error message is shown to the user.

2. **Search Request**:
   - The `searchMembers` function in the service is called with the relevant form data.
   - The result is displayed in the template, and if no data is found, an error message is shown.

### 3. HTML Template for the Search Form

#### `member-search.component.html`

This is the form where the user can input search criteria. Depending on whether they provide `UHID` or the other search fields, the search is conducted accordingly.

```html
<div class="container">
  <h2>Member Search</h2>

  <form (ngSubmit)="onSearch()">
    <!-- UHID Input -->
    <div class="mb-3">
      <label for="uhid" class="form-label">UHID</label>
      <input type="text" class="form-control" id="uhid" [(ngModel)]="uhid" name="uhid" />
    </div>

    <div class="mb-3">
      <label for="policyNo" class="form-label">Policy No</label>
      <input type="text" class="form-control" id="policyNo" [(ngModel)]="policyNo" name="policyNo" />
    </div>

    <div class="mb-3">
      <label for="employeeId" class="form-label">Employee ID</label>
      <input type="text" class="form-control" id="employeeId" [(ngModel)]="employeeId" name="employeeId" />
    </div>

    <div class="mb-3">
      <label for="patientRegistrationNo" class="form-label">Patient Registration No</label>
      <input type="text" class="form-control" id="patientRegistrationNo" [(ngModel)]="patientRegistrationNo" name="patientRegistrationNo" />
    </div>

    <div class="mb-3">
      <label for="policyName" class="form-label">Policy Name</label>
      <input type="text" class="form-control" id="policyName" [(ngModel)]="policyName" name="policyName" />
    </div>

    <div class="mb-3">
      <label for="insuredName" class="form-label">Insured Name</label>
      <input type="text" class="form-control" id="insuredName" [(ngModel)]="insuredName" name="insuredName" />
    </div>

    <button type="submit" class="btn btn-primary">Search</button>
  </form>

  <!-- Display Error Messages -->
  <div *ngIf="errorMessage" class="alert alert-danger mt-3">
    {{ errorMessage }}
  </div>

  <!-- Display Search Results -->
  <div *ngIf="members.length > 0">
    <h3>Search Results</h3>
    <ul class="list-group">
      <li *ngFor="let member of members" class="list-group-item">
        <!-- Assuming each member has a 'name' and 'id' -->
        Name: {{ member.name }}, ID: {{ member.id }}
      </li>
    </ul>
  </div>
</div>
```

### Explanation of HTML Template

1. **Form Fields**: The form has inputs for `UHID`, `policyNo`, `employeeId`, `patientRegistrationNo`, `policyName`, and `insuredName`. These fields are bound to the component variables using `[(ngModel)]`.
2. **Form Submission**: The form submits to the `onSearch` method when the "Search" button is clicked.
3. **Error Messages**: If there is a validation issue or no results are found, appropriate error messages are displayed.
4. **Search Results**: The members are displayed in a list if any results are found.

### Summary

- If a user provides `UHID`, the search will only use `UHID` to retrieve the data.
- If `UHID` is not provided, the search requires both `policyNo` and `employeeId`, with additional optional filters for `patientRegistrationNo`, `policyName`, and `insuredName`.
- Results and errors are displayed dynamically based on the search outcome.

This structure allows for flexibility in searching either by a single `UHID` or by other search criteria as required.