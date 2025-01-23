Here’s an Angular implementation for date-based transaction search using a reactive form and HTTP API integration. Let me know if you’d like me to tailor it further to match the framework versions or your specific use case (e.g., Angular 17, .NET Core).

Component Code (transaction-search.component.ts)

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-transaction-search',
  templateUrl: './transaction-search.component.html',
  styleUrls: ['./transaction-search.component.css']
})
export class TransactionSearchComponent implements OnInit {
  transactionForm: FormGroup;
  transactions: any[] = [];
  loading: boolean = false;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.transactionForm = this.fb.group({
      startDate: [''],
      endDate: ['']
    });
  }

  ngOnInit(): void {}

  searchTransactions(): void {
    const { startDate, endDate } = this.transactionForm.value;

    if (!startDate || !endDate) {
      alert('Please select both start and end dates.');
      return;
    }

    this.loading = true;
    this.http
      .get<any[]>('https://api.example.com/transactions', {
        params: {
          startDate,
          endDate
        }
      })
      .subscribe({
        next: (data) => {
          this.transactions = data;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error fetching transactions:', error);
          this.loading = false;
        }
      });
  }
}

Template Code (transaction-search.component.html)

<div class="transaction-search">
  <form [formGroup]="transactionForm" (ngSubmit)="searchTransactions()">
    <div class="form-group">
      <label for="startDate">Start Date</label>
      <input
        type="date"
        id="startDate"
        formControlName="startDate"
        class="form-control"
      />
    </div>

    <div class="form-group">
      <label for="endDate">End Date</label>
      <input
        type="date"
        id="endDate"
        formControlName="endDate"
        class="form-control"
      />
    </div>

    <button type="submit" class="btn btn-primary" [disabled]="loading">
      Search
    </button>
  </form>

  <div *ngIf="loading" class="loading">Loading...</div>

  <div *ngIf="transactions.length > 0">
    <h3>Transactions:</h3>
    <table class="table table-striped">
      <thead>
        <tr>
          <th>ID</th>
          <th>Amount</th>
          <th>Date</th>
          <th>Description</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let transaction of transactions">
          <td>{{ transaction.id }}</td>
          <td>{{ transaction.amount }}</td>
          <td>{{ transaction.date | date }}</td>
          <td>{{ transaction.description }}</td>
        </tr>
      </tbody>
    </table>
  </div>

  <div *ngIf="!loading && transactions.length === 0">
    <p>No transactions found for the selected date range.</p>
  </div>
</div>

Key Features:

	1.	Reactive Form:
	•	Two date inputs: startDate and endDate.
	•	Validation to ensure both dates are selected before making the API call.
	2.	API Call:
	•	Sends a GET request to an example API endpoint with startDate and endDate as query parameters.
	•	Handles success and error responses.
	3.	Loading State:
	•	Displays a loading indicator while fetching data.
	4.	Dynamic Table Rendering:
	•	Displays transactions in a table format if available.

Notes:

	•	Replace https://api.example.com/transactions with your actual API endpoint.
	•	Ensure the API supports date filtering via startDate and endDate query parameters.
	•	Add appropriate CSS for styling (transaction-search.component.css).

Would you like me to assist in setting this up for your specific environment (e.g., Angular 17, .NET Core backend integration)?