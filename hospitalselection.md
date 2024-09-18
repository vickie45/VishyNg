To implement this in Angular 17, you can follow these steps:

### 1. **Create a Hospital List Component**  
This component will display a list of hospitals in a hoverable table, allowing the user to select one.

```html
<!-- hospital-list.component.html -->
<div class="table-responsive">
  <table class="table table-hover">
    <thead>
      <tr>
        <th>Hospital Name</th>
        <th>Location</th>
        <th>Specialization</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let hospital of hospitals" (click)="selectHospital(hospital)" [class.selected]="hospital === selectedHospital">
        <td>{{ hospital.name }}</td>
        <td>{{ hospital.location }}</td>
        <td>{{ hospital.specialization }}</td>
      </tr>
    </tbody>
  </table>
</div>

<button class="btn btn-primary" [disabled]="!selectedHospital" (click)="submitSelection()">Submit</button>
```

### 2. **Hospital List Component Logic**  
The component will handle selecting a hospital and submitting the form.

```typescript
// hospital-list.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-hospital-list',
  templateUrl: './hospital-list.component.html',
  styleUrls: ['./hospital-list.component.css']
})
export class HospitalListComponent {
  hospitals = [
    { name: 'Hospital A', location: 'City A', specialization: 'Cardiology' },
    { name: 'Hospital B', location: 'City B', specialization: 'Neurology' },
    { name: 'Hospital C', location: 'City C', specialization: 'Orthopedics' }
  ];

  selectedHospital: any = null;

  constructor(private router: Router) {}

  selectHospital(hospital: any) {
    this.selectedHospital = hospital;
  }

  submitSelection() {
    if (this.selectedHospital) {
      this.router.navigate(['/hospital-dashboard', this.selectedHospital.name]);
    }
  }
}
```

### 3. **Hospital Dashboard Route**  
Define a route that navigates to the hospital dashboard when a hospital is selected.

```typescript
// app-routing.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HospitalListComponent } from './hospital-list/hospital-list.component';
import { HospitalDashboardComponent } from './hospital-dashboard/hospital-dashboard.component';

const routes: Routes = [
  { path: 'hospital-list', component: HospitalListComponent },
  { path: 'hospital-dashboard/:hospitalName', component: HospitalDashboardComponent },
  { path: '', redirectTo: '/hospital-list', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
```

### 4. **Hospital Dashboard Component**  
This component will display the selected hospital's dashboard.

```typescript
// hospital-dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-hospital-dashboard',
  templateUrl: './hospital-dashboard.component.html',
  styleUrls: ['./hospital-dashboard.component.css']
})
export class HospitalDashboardComponent implements OnInit {
  hospitalName: string = '';

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.hospitalName = this.route.snapshot.paramMap.get('hospitalName') || '';
  }
}
```

```html
<!-- hospital-dashboard.component.html -->
<h1>{{ hospitalName }} Dashboard</h1>
<p>Welcome to the dashboard of {{ hospitalName }}.</p>
```

### 5. **CSS for Selected Row**  
Optionally, you can add some CSS to highlight the selected hospital row.

```css
/* hospital-list.component.css */
tr.selected {
  background-color: #007bff;
  color: white;
}
```

### Explanation:
- **Table Row Selection**: When a hospital is clicked, it becomes the `selectedHospital`.
- **Submit Button**: The "Submit" button is only enabled when a hospital is selected, and it navigates to the hospital's dashboard upon click.
- **Routing**: The selected hospital’s name is passed as a route parameter, which is used to display the hospital's dashboard.

Let me know if you'd like any additional functionality!