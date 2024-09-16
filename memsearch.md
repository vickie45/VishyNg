GET /api/MemberSearchApi/GetMemberSearchDetails?EmployeeId=123&InsuredName=JohnDoe&PatientRegistrationNo=456&PolicyName=XYZ&PolicyNo=789&PolicyType=Health&ProviderBasicDetailId=1011&UHID=1234

To fetch the list of members from this API in Angular 17, you need to:

1. Create a service in Angular to call the Web API.
2. Use `HttpClient` to make a GET request to the API.
3. Handle the query parameters dynamically based on user input.

Here’s how to implement it:

### 1. Install `HttpClientModule` in your Angular App
Make sure you have `HttpClientModule` imported in your `AppModule`.

```typescript
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    // components
  ],
  imports: [
    HttpClientModule, // Add this
    // other modules
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

### 2. Create a Service to Call the API

First, generate a new service using Angular CLI:

```bash
ng generate service member-search
```

Then, modify the `member-search.service.ts` file to fetch the member search details:

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

interface MemberSearchViewModel {
  employeeId: string;
  insuredName: string;
  patientRegistrationNo: string;
  policyName: string;
  policyNo: string;
  policyType: string;
  providerBasicDetailId: number;
  uhid: string;
}

@Injectable({
  providedIn: 'root'
})
export class MemberSearchService {

  private apiUrl = 'https://your-api-domain/api/MemberSearchApi/GetMemberSearchDetails'; // Change this to your actual API URL

  constructor(private http: HttpClient) { }

  searchMembers(params: any): Observable<MemberSearchViewModel[]> {
    // Prepare the query parameters
    let httpParams = new HttpParams()
      .set('EmployeeId', params.employeeId || '')
      .set('InsuredName', params.insuredName || '')
      .set('PatientRegistrationNo', params.patientRegistrationNo || '')
      .set('PolicyName', params.policyName || '')
      .set('PolicyNo', params.policyNo || '')
      .set('PolicyType', params.policyType || '')
      .set('ProviderBasicDetailId', params.providerBasicDetailId || '')
      .set('UHID', params.uhid || '');

    return this.http.get<MemberSearchViewModel[]>(this.apiUrl, { params: httpParams });
  }
}
```

### 3. Use the Service in a Component

Now, let’s use this service in a component to search for members and display the results. First, create the component:

```bash
ng generate component member-search
```

Modify the `member-search.component.ts` file:

```typescript
import { Component } from '@angular/core';
import { MemberSearchService } from './member-search.service';

@Component({
  selector: 'app-member-search',
  templateUrl: './member-search.component.html'
})
export class MemberSearchComponent {

  memberSearchParams = {
    employeeId: '',
    insuredName: '',
    patientRegistrationNo: '',
    policyName: '',
    policyNo: '',
    policyType: '',
    providerBasicDetailId: null,
    uhid: ''
  };

  members: any[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private memberSearchService: MemberSearchService) { }

  onSearch(): void {
    this.isLoading = true;
    this.memberSearchService.searchMembers(this.memberSearchParams).subscribe(
      (data) => {
        this.members = data;
        this.isLoading = false;
      },
      (error) => {
        this.errorMessage = 'Error occurred while fetching members.';
        this.isLoading = false;
      }
    );
  }
}
```

### 4. Create the Template (HTML)

Modify the `member-search.component.html` file to provide a form for inputting the search parameters and display the search results:

```html
<div>
  <h2>Member Search</h2>
  <form (submit)="onSearch()">
    <div>
      <label>Employee ID:</label>
      <input [(ngModel)]="memberSearchParams.employeeId" name="employeeId" />
    </div>
    <div>
      <label>Insured Name:</label>
      <input [(ngModel)]="memberSearchParams.insuredName" name="insuredName" />
    </div>
    <div>
      <label>Patient Registration No:</label>
      <input [(ngModel)]="memberSearchParams.patientRegistrationNo" name="patientRegistrationNo" />
    </div>
    <div>
      <label>Policy Name:</label>
      <input [(ngModel)]="memberSearchParams.policyName" name="policyName" />
    </div>
    <div>
      <label>Policy No:</label>
      <input [(ngModel)]="memberSearchParams.policyNo" name="policyNo" />
    </div>
    <div>
      <label>Policy Type:</label>
      <input [(ngModel)]="memberSearchParams.policyType" name="policyType" />
    </div>
    <div>
      <label>Provider Basic Detail ID:</label>
      <input type="number" [(ngModel)]="memberSearchParams.providerBasicDetailId" name="providerBasicDetailId" />
    </div>
    <div>
      <label>UHID:</label>
      <input [(ngModel)]="memberSearchParams.uhid" name="uhid" />
    </div>
    <button type="submit">Search</button>
  </form>

  <div *ngIf="isLoading">Loading...</div>
  <div *ngIf="errorMessage">{{ errorMessage }}</div>

  <div *ngIf="members.length > 0">
    <h3>Search Results</h3>
    <ul>
      <li *ngFor="let member of members">
        {{ member.employeeId }} - {{ member.insuredName }} - {{ member.policyNo }}
      </li>
    </ul>
  </div>
</div>
```

### 5. Import `FormsModule`

Ensure that `FormsModule` is imported in the `AppModule` for two-way binding in the template:

```typescript
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    // components
  ],
  imports: [
    FormsModule, // Add this
    // other modules
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

### Summary

- You’ve created an Angular service to handle the API call.
- You’ve implemented a component that collects search parameters, fetches the results, and displays them.
