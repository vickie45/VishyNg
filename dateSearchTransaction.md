To fetch and display transactions based on a date selection and search input using Angular 17 and Bootstrap 5, you can follow these steps:

Step 1: Set Up the Angular Component

If you don’t already have a component for transactions, create one:

ng generate component transactions

Step 2: HTML Template with Date Picker and Search Input

In the component’s HTML template, use Bootstrap 5’s date picker and input fields. The date picker can be handled by the input field of type date, and Bootstrap classes can be used for styling.

<!-- transactions.component.html -->
<div class="container mt-4">
  <!-- Date and Search Input Fields -->
  <div class="row mb-3">
    <div class="col-md-4">
      <input type="date" class="form-control" [(ngModel)]="selectedDate" placeholder="Select Date">
    </div>
    <div class="col-md-4">
      <input type="text" class="form-control" [(ngModel)]="cardSearchTerm" placeholder="Search by Card Number or Name">
    </div>
    <div class="col-md-4">
      <button class="btn btn-primary w-100" (click)="searchTransactions()">Search</button>
    </div>
  </div>

  <!-- Transactions Table -->
  <table class="table table-bordered table-striped">
    <thead>
      <tr>
        <th>Date</th>
        <th>Card Number</th>
        <th>Amount</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let transaction of transactions">
        <td>{{ transaction.date | date }}</td>
        <td>{{ transaction.cardNumber }}</td>
        <td>{{ transaction.amount | currency }}</td>
      </tr>
    </tbody>
  </table>
</div>

Step 3: Define Variables and Search Logic in the Component

In your component’s TypeScript file, define the variables and create a method to fetch the transactions based on the selected date and search term.

// transactions.component.ts
import { Component, OnInit } from '@angular/core';
import { TransactionService } from '../services/transaction.service'; // Assuming a service for fetching transactions

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']
})
export class TransactionsComponent implements OnInit {

  transactions: any[] = [];
  selectedDate: string | null = null;
  cardSearchTerm: string = '';

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    // Optionally load initial transactions here, e.g., the latest transactions
    this.searchTransactions();
  }

  // Search transactions based on date and search term
  searchTransactions(): void {
    const params: any = {
      date: this.selectedDate,
      card: this.cardSearchTerm
    };
    this.transactionService.getFilteredTransactions(params).subscribe(response => {
      this.transactions = response.transactions;
    });
  }
}

Step 4: Transaction Service with Filtering Support

In the service, make an API call that accepts the date and card search term as query parameters.

// transaction.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  private apiUrl = 'https://api.yourdomain.com/transactions';

  constructor(private http: HttpClient) {}

  getFilteredTransactions(params: any): Observable<any> {
    let queryParams = new HttpParams();
    if (params.date) {
      queryParams = queryParams.set('date', params.date);
    }
    if (params.card) {
      queryParams = queryParams.set('card', params.card);
    }

    return this.http.get<any>(this.apiUrl, { params: queryParams });
  }
}

Step 5: Backend Support for Filtering (in .NET Core)

Modify your API to accept date and card parameters and filter based on them.

[HttpGet("transactions")]
public async Task<IActionResult> GetTransactions(DateTime? date, string card)
{
    var query = _dbContext.Transactions.AsQueryable();

    if (date.HasValue)
    {
        query = query.Where(t => t.TransactionDate.Date == date.Value.Date);
    }

    if (!string.IsNullOrEmpty(card))
    {
        query = query.Where(t => t.CardNumber.Contains(card) || t.CardHolderName.Contains(card));
    }

    var transactions = await query.ToListAsync();

    return Ok(new { transactions });
}

Step 6: CSS Styling (Optional)

You can add additional styling for better layout using Bootstrap 5 utility classes. Here’s some optional styling you might include:

/* transactions.component.css */

.container {
  max-width: 800px;
}

.table {
  margin-top: 20px;
}

.btn-primary {
  background-color: #007bff;
  border-color: #007bff;
}

.btn-primary:hover {
  background-color: #0056b3;
  border-color: #0056b3;
}

Summary

This approach uses Angular 17 and Bootstrap 5 to build a simple transactions search interface based on a selected date and card search term. The service and backend API support the filtering, while Bootstrap is used for styling and layout. You could expand this by adding additional search fields, error handling, or pagination if necessary.