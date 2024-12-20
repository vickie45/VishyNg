Here’s the detailed step-by-step guide to implement a modal popup named “Search Popup” in Angular 17 with Bootstrap 5. The modal will contain fields for Start Date, End Date, and Search Category, and it will automatically appear when the page loads.

Step 1: Install Bootstrap 5

First, make sure Bootstrap 5 is installed in your project. You can do this by either using a CDN or installing it via npm.

Install via npm

Run the following command:

npm install bootstrap

Then, include Bootstrap in your angular.json file under the styles and scripts sections:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]

Step 2: Create the Angular Component

If you haven’t already created a component, use the Angular CLI to create one:

ng generate component app

Step 3: HTML Template (app.component.html)

Add the modal markup. This uses Bootstrap’s modal structure, along with Angular’s [(ngModel)] binding for input fields.

<div class="modal fade" id="searchModal" tabindex="-1" aria-labelledby="searchModalLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <!-- Modal Header -->
      <div class="modal-header">
        <h5 class="modal-title" id="searchModalLabel">Search Popup</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <!-- Modal Body -->
      <div class="modal-body">
        <form>
          <!-- Start Date -->
          <div class="mb-3">
            <label for="startDate" class="form-label">Start Date</label>
            <input
              type="date"
              class="form-control"
              id="startDate"
              [(ngModel)]="startDate"
              name="startDate"
            />
          </div>
          <!-- End Date -->
          <div class="mb-3">
            <label for="endDate" class="form-label">End Date</label>
            <input
              type="date"
              class="form-control"
              id="endDate"
              [(ngModel)]="endDate"
              name="endDate"
            />
          </div>
          <!-- Search Category -->
          <div class="mb-3">
            <label for="searchCategory" class="form-label">Search Category</label>
            <select
              class="form-select"
              id="searchCategory"
              [(ngModel)]="searchCategory"
              name="searchCategory"
            >
              <option value="" disabled>Select a category</option>
              <option *ngFor="let category of categories" [value]="category">
                {{ category }}
              </option>
            </select>
          </div>
        </form>
      </div>
      <!-- Modal Footer -->
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        <button type="button" class="btn btn-primary" (click)="search()">Search</button>
      </div>
    </div>
  </div>
</div>

Step 4: TypeScript Component (app.component.ts)

In the TypeScript file, define the logic to show the modal on page load and handle the form submission.

import { Component, OnInit } from '@angular/core';

declare var bootstrap: any; // Declare Bootstrap globally

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  startDate: string = '';
  endDate: string = '';
  searchCategory: string = '';
  categories: string[] = ['Category 1', 'Category 2', 'Category 3'];

  ngOnInit(): void {
    // Automatically show the modal when the page loads
    const modalElement = document.getElementById('searchModal');
    const modalInstance = new bootstrap.Modal(modalElement!); // Initialize Bootstrap modal
    modalInstance.show(); // Show modal
  }

  search(): void {
    // Perform the search operation or log the values
    console.log('Start Date:', this.startDate);
    console.log('End Date:', this.endDate);
    console.log('Search Category:', this.searchCategory);
    alert(
      `Search initiated with Start Date: ${this.startDate}, End Date: ${this.endDate}, and Category: ${this.searchCategory}`
    );
  }
}

Step 5: Configure the Angular Module (app.module.ts)

Import the required modules (FormsModule for ngModel binding).

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, FormsModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

Step 6: Styling (Optional)

You can add custom styles to adjust the appearance of the modal or its fields. Place these in the app.component.css file.

.modal-title {
  font-weight: bold;
}

.modal-footer .btn {
  min-width: 100px;
}

Step 7: Run the Application

Run your Angular application using:

ng serve

Navigate to http://localhost:4200/. On page load, the modal popup will automatically appear with the fields for Start Date, End Date, and Search Category.

When you click the Search button, the entered data will be logged to the console or displayed in an alert (depending on the search() method implementation).

Additional Notes:

	1.	Bootstrap Dependencies: Ensure that the Bootstrap JavaScript library is correctly loaded. Without this, the modal will not function as expected.
	2.	Dynamic Categories: You can fetch the categories dynamically from a backend API if needed.
	3.	Angular Forms: For more advanced validation, you can use Angular’s Reactive Forms instead of ngModel.

Would you like me to assist with further enhancements, such as form validation or adding animations?