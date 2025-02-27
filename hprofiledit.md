Here’s a Hospital Profile Edit Page for a Hospital Claim Management System using Angular 17 and Bootstrap 5. The page includes:

	•	Reactive Forms for validation
	•	Bootstrap 5 UI for styling
	•	HTTP request simulation for fetching and updating hospital details
	•	Image upload functionality

1. Install Dependencies

Make sure you have Angular 17 and Bootstrap 5 installed. If not, install Bootstrap:

npm install bootstrap

Then, import Bootstrap in angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
]

2. Create the Hospital Edit Component

Run the following command to generate the component:

ng generate component hospital-profile-edit

3. Component Code

hospital-profile-edit.component.ts

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HospitalService } from '../services/hospital.service';

@Component({
  selector: 'app-hospital-profile-edit',
  templateUrl: './hospital-profile-edit.component.html',
  styleUrls: ['./hospital-profile-edit.component.css']
})
export class HospitalProfileEditComponent implements OnInit {
  hospitalForm!: FormGroup;
  hospitalId: number = 1; // Example hospital ID
  selectedImage: File | null = null;
  previewImage: string | ArrayBuffer | null = null;
  
  constructor(private fb: FormBuilder, private hospitalService: HospitalService) {}

  ngOnInit(): void {
    this.initForm();
    this.loadHospitalData();
  }

  initForm() {
    this.hospitalForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      address: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zip: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
      image: [null]
    });
  }

  loadHospitalData() {
    this.hospitalService.getHospitalById(this.hospitalId).subscribe(data => {
      this.hospitalForm.patchValue(data);
      this.previewImage = data.imageUrl; // Set preview image
    });
  }

  onFileSelect(event: any) {
    if (event.target.files && event.target.files[0]) {
      this.selectedImage = event.target.files[0];

      const reader = new FileReader();
      reader.onload = (e) => this.previewImage = reader.result;
      reader.readAsDataURL(this.selectedImage);
    }
  }

  updateHospital() {
    if (this.hospitalForm.invalid) return;

    const formData = new FormData();
    formData.append('name', this.hospitalForm.value.name);
    formData.append('email', this.hospitalForm.value.email);
    formData.append('phone', this.hospitalForm.value.phone);
    formData.append('address', this.hospitalForm.value.address);
    formData.append('city', this.hospitalForm.value.city);
    formData.append('state', this.hospitalForm.value.state);
    formData.append('zip', this.hospitalForm.value.zip);
    if (this.selectedImage) {
      formData.append('image', this.selectedImage);
    }

    this.hospitalService.updateHospital(this.hospitalId, formData).subscribe({
      next: () => alert('Hospital details updated successfully'),
      error: (err) => console.error('Update failed', err)
    });
  }
}

4. HTML UI with Bootstrap 5

hospital-profile-edit.component.html

<div class="container mt-4">
  <h2 class="mb-4">Edit Hospital Profile</h2>
  <form [formGroup]="hospitalForm" (ngSubmit)="updateHospital()">
    
    <!-- Image Upload -->
    <div class="mb-3 text-center">
      <img [src]="previewImage || 'assets/default-hospital.png'" class="rounded-circle" width="150" height="150" />
      <input type="file" class="form-control mt-2" (change)="onFileSelect($event)">
    </div>

    <div class="row">
      <div class="col-md-6">
        <div class="mb-3">
          <label class="form-label">Hospital Name</label>
          <input type="text" class="form-control" formControlName="name">
          <div *ngIf="hospitalForm.controls.name.touched && hospitalForm.controls.name.invalid" class="text-danger">
            Name is required (Min 3 characters)
          </div>
        </div>
      </div>

      <div class="col-md-6">
        <div class="mb-3">
          <label class="form-label">Email</label>
          <input type="email" class="form-control" formControlName="email">
          <div *ngIf="hospitalForm.controls.email.touched && hospitalForm.controls.email.invalid" class="text-danger">
            Valid email is required
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-md-6">
        <div class="mb-3">
          <label class="form-label">Phone</label>
          <input type="text" class="form-control" formControlName="phone">
          <div *ngIf="hospitalForm.controls.phone.touched && hospitalForm.controls.phone.invalid" class="text-danger">
            Valid 10-digit phone number required
          </div>
        </div>
      </div>

      <div class="col-md-6">
        <div class="mb-3">
          <label class="form-label">Zip Code</label>
          <input type="text" class="form-control" formControlName="zip">
          <div *ngIf="hospitalForm.controls.zip.touched && hospitalForm.controls.zip.invalid" class="text-danger">
            Valid 6-digit zip code required
          </div>
        </div>
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label">Address</label>
      <textarea class="form-control" rows="3" formControlName="address"></textarea>
    </div>

    <div class="row">
      <div class="col-md-6">
        <div class="mb-3">
          <label class="form-label">City</label>
          <input type="text" class="form-control" formControlName="city">
        </div>
      </div>
      <div class="col-md-6">
        <div class="mb-3">
          <label class="form-label">State</label>
          <input type="text" class="form-control" formControlName="state">
        </div>
      </div>
    </div>

    <button type="submit" class="btn btn-primary" [disabled]="hospitalForm.invalid">Update</button>
  </form>
</div>

5. Service for API Calls

hospital.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HospitalService {
  private apiUrl = 'https://api.example.com/hospitals';

  constructor(private http: HttpClient) {}

  getHospitalById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  updateHospital(id: number, data: FormData): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, data);
  }
}

6. Register the Service

Import HttpClientModule in app.module.ts:

import { HttpClientModule } from '@angular/common/http';

@NgModule({
  imports: [HttpClientModule],
})
export class AppModule { }

Conclusion

This Hospital Profile Edit Page is fully functional with:
✅ Reactive Forms & Validation
✅ Bootstrap 5 UI
✅ Image Upload Preview
✅ API Integration with HttpClient

Would you like any enhancements, such as adding multi-file upload or role-based access control?