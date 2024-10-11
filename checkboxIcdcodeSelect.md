To achieve the functionality of displaying a list of checkboxes with `ICDCode` and `ICDDescription` as table headers, fetching the data from an API, and adding the selected item to another table outside the modal, you can follow these steps:

1. Fetch the `ICDCode` and `ICDDescription` from an API.
2. Display the data in a table with checkboxes.
3. Allow the user to select an item.
4. Display the selected item in another table outside of the modal.

Here is a sample Angular implementation:

### Step 1: Create the Component

#### HTML (icd-modal.component.html)

```html
<!-- Modal for displaying the ICD Codes with checkboxes -->
<div class="modal-body">
  <table class="table table-bordered">
    <thead>
      <tr>
        <th>Select</th>
        <th>ICD Code</th>
        <th>ICD Description</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let icd of icdList">
        <td>
          <input
            type="checkbox"
            [value]="icd"
            (change)="onCheckboxChange($event, icd)"
          />
        </td>
        <td>{{ icd.ICDCode }}</td>
        <td>{{ icd.ICDDescription }}</td>
      </tr>
    </tbody>
  </table>
</div>

<!-- Table outside the modal to display selected ICD Codes -->
<h3>Selected ICD Codes</h3>
<table class="table table-bordered">
  <thead>
    <tr>
      <th>ICD Code</th>
      <th>ICD Description</th>
    </tr>
  </thead>
  <tbody>
    <tr *ngFor="let selected of selectedIcdList">
      <td>{{ selected.ICDCode }}</td>
      <td>{{ selected.ICDDescription }}</td>
    </tr>
  </tbody>
</table>
```

#### TypeScript (icd-modal.component.ts)

```typescript
import { Component, OnInit } from '@angular/core';
import { IcdService } from './icd.service'; // Your service to fetch ICD data

@Component({
  selector: 'app-icd-modal',
  templateUrl: './icd-modal.component.html',
})
export class IcdModalComponent implements OnInit {
  icdList: any[] = []; // To store the ICD list fetched from the API
  selectedIcdList: any[] = []; // To store the selected ICD codes

  constructor(private icdService: IcdService) {}

  ngOnInit(): void {
    // Fetch the ICD data when the component loads
    this.fetchIcdCodes();
  }

  // Fetch the ICD codes from the API
  fetchIcdCodes() {
    this.icdService.getIcdCodes().subscribe(
      (response) => {
        this.icdList = response; // Assign the fetched data to icdList
      },
      (error) => {
        console.error('Error fetching ICD codes:', error);
      }
    );
  }

  // Handle checkbox selection
  onCheckboxChange(event: any, icd: any) {
    if (event.target.checked) {
      // Add the selected ICD code to the selectedIcdList
      this.selectedIcdList.push(icd);
    } else {
      // Remove the ICD code from the selectedIcdList if unchecked
      this.selectedIcdList = this.selectedIcdList.filter(
        (item) => item.ICDCode !== icd.ICDCode
      );
    }
  }
}
```

### Step 2: Create the Service

#### Service (icd.service.ts)

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class IcdService {
  private apiUrl = 'https://your-api-endpoint.com/icd-codes'; // Replace with your API URL

  constructor(private http: HttpClient) {}

  // Fetch ICD codes from API
  getIcdCodes(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
}
```

### Step 3: Add Necessary Imports in App Module

#### app.module.ts

```typescript
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { IcdModalComponent } from './icd-modal/icd-modal.component';
import { IcdService } from './icd-modal/icd.service';

@NgModule({
  declarations: [IcdModalComponent],
  imports: [
    HttpClientModule, // For API requests
    FormsModule, // For handling forms and checkbox changes
  ],
  providers: [IcdService],
  bootstrap: [IcdModalComponent],
})
export class AppModule {}
```

### Explanation:

1. **ICD Data Fetching**: The `IcdService` fetches data from an API (`getIcdCodes()`).
2. **Table with Checkboxes**: In `icd-modal.component.html`, a table displays `ICDCode` and `ICDDescription` with checkboxes for selection.
3. **Checkbox Handling**: When the user checks/unchecks a checkbox, the `onCheckboxChange` method updates the selected list (`selectedIcdList`).
4. **Selected Table**: The selected ICD codes are displayed in another table outside of the modal, reflecting the choices made in the modal.

This provides a dynamic interface where users can select ICD codes from a list and view the selected codes outside the modal in another table.