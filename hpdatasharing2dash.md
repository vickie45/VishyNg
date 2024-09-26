To implement sharing of API data from the **HospitalSelectComponent** to the **DashboardComponent** for further computation, you can follow these steps. This involves fetching the hospital data from an API in **HospitalSelectComponent** and using a shared service to pass the selected hospital's details to **DashboardComponent**.

Here’s the detailed implementation:

### 1. **Create a Shared Service**

The shared service will manage the communication between the two components and store the selected hospital details.

#### `hospital.service.ts`:
```typescript
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HospitalService {
  // BehaviorSubject to store selected hospital details
  private selectedHospitalSource = new BehaviorSubject<any>(null);
  selectedHospital$ = this.selectedHospitalSource.asObservable();

  // Method to update the selected hospital
  selectHospital(hospital: any) {
    this.selectedHospitalSource.next(hospital);
  }
}
```

### 2. **Fetching Data in HospitalSelectComponent**

In **HospitalSelectComponent**, you will fetch the list of hospitals from an API and allow the user to select one. When a selection is made, the selected hospital's data is passed to the service.

#### `hospital-select.component.ts`:
```typescript
import { Component, OnInit } from '@angular/core';
import { HospitalService } from './hospital.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-hospital-select',
  template: `
    <select (change)="onSelectHospital($event.target.value)">
      <option *ngFor="let hospital of hospitals" [value]="hospital">{{ hospital.name }}</option>
    </select>
  `
})
export class HospitalSelectComponent implements OnInit {
  hospitals: any[] = [];  // To store the list of hospitals

  constructor(private hospitalService: HospitalService, private http: HttpClient) {}

  ngOnInit() {
    this.getHospitalsFromAPI();
  }

  // Fetch the hospital data from API
  getHospitalsFromAPI() {
    this.http.get<any[]>('https://api.example.com/hospitals')  // Replace with your actual API
      .subscribe(data => {
        this.hospitals = data;
      });
  }

  // Handle hospital selection
  onSelectHospital(hospitalName: string) {
    const selectedHospital = this.hospitals.find(hospital => hospital.name === hospitalName);
    this.hospitalService.selectHospital(selectedHospital);  // Pass selected hospital to service
  }
}
```

### 3. **Display and Use Data in DashboardComponent**

In **DashboardComponent**, you will subscribe to the shared service to get the selected hospital details. You can then use this data for further computation or display.

#### `dashboard.component.ts`:
```typescript
import { Component, OnInit } from '@angular/core';
import { HospitalService } from './hospital.service';

@Component({
  selector: 'app-dashboard',
  template: `
    <div *ngIf="selectedHospital">
      <h2>Hospital Details</h2>
      <p><strong>Name:</strong> {{ selectedHospital.name }}</p>
      <p><strong>Location:</strong> {{ selectedHospital.location }}</p>
      <p><strong>Contact:</strong> {{ selectedHospital.contact }}</p>
      <!-- Further computations or logic can be added here -->
    </div>
  `
})
export class DashboardComponent implements OnInit {
  selectedHospital: any;

  constructor(private hospitalService: HospitalService) {}

  ngOnInit() {
    // Subscribe to the selected hospital data from the service
    this.hospitalService.selectedHospital$.subscribe(hospital => {
      this.selectedHospital = hospital;
    });
  }
}
```

### 4. **API Response Structure**

Ensure the API response contains the necessary fields like `name`, `location`, `contact`, etc. For example, the hospital data from the API might look like this:

```json
[
  {
    "name": "Hospital A",
    "location": "City A",
    "contact": "1234567890"
  },
  {
    "name": "Hospital B",
    "location": "City B",
    "contact": "0987654321"
  }
]
```

### 5. **Routing Setup**

The components should be routed under the layout component using Angular Router. Ensure the routes are defined correctly in your routing module.

#### `app-routing.module.ts`:
```typescript
const routes: Routes = [
  {
    path: '',
    component: LayoutPageComponent,
    children: [
      { path: 'hospital-select', component: HospitalSelectComponent },
      { path: 'dashboard', component: DashboardComponent }
    ]
  }
];
```

### 6. **HTTPClient Module**

Ensure you have imported `HttpClientModule` in your app module to use the `HttpClient` service for API calls.

#### `app.module.ts`:
```typescript
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    // Components
  ],
  imports: [
    // Other modules
    HttpClientModule  // Add HttpClientModule here
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

### How it works:
- **HospitalSelectComponent** fetches hospital data from an API and allows the user to select one.
- Upon selecting a hospital, the selected hospital’s data is passed to the `HospitalService`.
- **DashboardComponent** subscribes to the `HospitalService` to get the selected hospital's details and displays the information, or performs further computations.

This approach ensures that the API data is efficiently shared between the routed components without involving the layout component.

Let me know if you need further clarification!