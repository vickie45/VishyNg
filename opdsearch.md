Here’s how to implement the Search Popup using Reactive Forms in Angular, with the functionality to close the modal upon clicking the Search button and display the fetched data in a table.

Step 1: Set Up the Reactive Forms Module

Ensure your application has the ReactiveFormsModule imported.

In app.module.ts:

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, ReactiveFormsModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

Step 2: HTML Template

The modal popup is bound to a reactive form, and the table dynamically displays the fetched data.

<!-- Modal -->
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
        <form [formGroup]="searchForm">
          <!-- Start Date -->
          <div class="mb-3">
            <label for="startDate" class="form-label">Start Date</label>
            <input
              type="date"
              id="startDate"
              class="form-control"
              formControlName="startDate"
              required
            />
          </div>
          <!-- End Date -->
          <div class="mb-3">
            <label for="endDate" class="form-label">End Date</label>
            <input
              type="date"
              id="endDate"
              class="form-control"
              formControlName="endDate"
              required
            />
          </div>
          <!-- Search Category -->
          <div class="mb-3">
            <label for="category" class="form-label">Search Category</label>
            <select
              id="category"
              class="form-select"
              formControlName="category"
              required
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
        <button
          type="button"
          class="btn btn-primary"
          [disabled]="!searchForm.valid"
          (click)="onSearch()"
        >
          Search
        </button>
      </div>
    </div>
  </div>
</div>

<!-- Table to Display Search Results -->
<div class="container mt-4">
  <h3>Search Results</h3>
  <table class="table table-striped">
    <thead>
      <tr>
        <th>#</th>
        <th>Start Date</th>
        <th>End Date</th>
        <th>Category</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let result of searchResults; index as i">
        <td>{{ i + 1 }}</td>
        <td>{{ result.startDate }}</td>
        <td>{{ result.endDate }}</td>
        <td>{{ result.category }}</td>
      </tr>
    </tbody>
  </table>
</div>

Step 3: TypeScript Component

Use Reactive Forms to handle form input, simulate data fetching, and update the search results.

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

declare var bootstrap: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  searchForm: FormGroup;
  categories: string[] = ['Category 1', 'Category 2', 'Category 3'];
  searchResults: Array<{ startDate: string; endDate: string; category: string }> = [];

  constructor(private fb: FormBuilder) {
    // Initialize the reactive form
    this.searchForm = this.fb.group({
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      category: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    // Show the modal on page load
    const modalElement = document.getElementById('searchModal');
    const modalInstance = new bootstrap.Modal(modalElement!);
    modalInstance.show();
  }

  onSearch(): void {
    // Check if the form is valid
    if (this.searchForm.valid) {
      // Fetch form values
      const formData = this.searchForm.value;

      // Simulate data fetching (you can replace this with an API call)
      this.searchResults = [
        {
          startDate: formData.startDate,
          endDate: formData.endDate,
          category: formData.category,
        },
      ];

      // Close the modal
      const modalElement = document.getElementById('searchModal');
      const modalInstance = bootstrap.Modal.getInstance(modalElement!);
      modalInstance.hide();
    } else {
      alert('Please fill all the required fields.');
    }
  }
}

Step 4: Explanation of Code

	1.	Reactive Form:
	•	The FormGroup instance searchForm manages the form’s input values and validation.
	2.	Validation:
	•	Each form control (startDate, endDate, category) is required. The search button is disabled until the form is valid.
	3.	Data Fetching:
	•	The onSearch() method simulates fetching data (you can replace it with an API call). The fetched data is stored in searchResults and displayed in the table.
	4.	Closing the Modal:
	•	The modal is closed programmatically using bootstrap.Modal.getInstance(modalElement!).hide().
	5.	Dynamic Table:
	•	The <table> element dynamically renders searchResults using *ngFor.

Sample Output

	1.	On Page Load:
The modal appears automatically with the fields for Start Date, End Date, and Search Category.
	2.	Validation:
	•	If any field is missing, the search button remains disabled.
	•	Upon submission, an alert prompts the user to fill in the required fields if validation fails.
	3.	Data Fetching and Display:
After clicking Search, the modal closes, and the table is populated with the search results.

Enhancements:

	•	Replace the simulated data with a backend API call using Angular’s HttpClient.
	•	Add pagination or filtering to the table if the data set is large.

Let me know if you’d like help with those!