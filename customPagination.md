If you want to implement pagination without Angular Material, you can build a custom pagination component with plain HTML and CSS. Here’s a step-by-step guide:

Step 1: Create the Transactions Component

If you don’t already have a component to display transactions, create one:

ng generate component transactions

Step 2: HTML Template for Transactions List and Pagination Controls

Define the structure of the table or list for transactions in the component template, and add pagination controls below it.

<!-- transactions.component.html -->
<div class="transaction-list">
  <table>
    <thead>
      <tr>
        <th>Date</th>
        <th>Card Number</th>
        <th>Amount</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let transaction of paginatedTransactions">
        <td>{{ transaction.date | date }}</td>
        <td>{{ transaction.cardNumber }}</td>
        <td>{{ transaction.amount | currency }}</td>
      </tr>
    </tbody>
  </table>

  <!-- Pagination Controls -->
  <div class="pagination">
    <button (click)="goToPage(1)" [disabled]="currentPage === 1">First</button>
    <button (click)="goToPage(currentPage - 1)" [disabled]="currentPage === 1">Previous</button>

    <span>Page {{ currentPage }} of {{ totalPages }}</span>

    <button (click)="goToPage(currentPage + 1)" [disabled]="currentPage === totalPages">Next</button>
    <button (click)="goToPage(totalPages)" [disabled]="currentPage === totalPages">Last</button>
  </div>
</div>

Step 3: Add Pagination Logic in the Component

Define the logic for pagination in your component’s TypeScript file.

// transactions.component.ts
import { Component, OnInit } from '@angular/core';
import { TransactionService } from '../services/transaction.service'; // Service to fetch transactions

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']
})
export class TransactionsComponent implements OnInit {

  paginatedTransactions: any[] = [];
  totalTransactions: number = 0;
  pageSize: number = 10;
  currentPage: number = 1;
  totalPages: number = 1;

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    this.fetchTransactions(this.currentPage);
  }

  // Fetch transactions with pagination
  fetchTransactions(page: number): void {
    this.transactionService.getTransactions(page, this.pageSize).subscribe(response => {
      this.paginatedTransactions = response.transactions;
      this.totalTransactions = response.total; // Assuming the API response contains the total count
      this.totalPages = Math.ceil(this.totalTransactions / this.pageSize);
    });
  }

  // Go to a specific page
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return; // Prevent invalid pages
    this.currentPage = page;
    this.fetchTransactions(this.currentPage);
  }
}

Step 4: Transaction Service with Pagination Support

The service should handle the API call for fetching paginated transactions.

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

  getTransactions(page: number, size: number): Observable<any> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('size', size.toString());

    return this.http.get<any>(this.apiUrl, { params });
  }
}

Step 5: Backend Support for Pagination (in .NET Core)

Modify your API to accept page and size parameters and return paginated results.

[HttpGet("transactions")]
public async Task<IActionResult> GetTransactions(int page = 1, int size = 10)
{
    var totalTransactions = await _dbContext.Transactions.CountAsync();
    var transactions = await _dbContext.Transactions
        .OrderByDescending(t => t.TransactionDate)
        .Skip((page - 1) * size)
        .Take(size)
        .ToListAsync();

    return Ok(new { total = totalTransactions, transactions });
}

Step 6: Basic CSS for Pagination Controls

Add some basic CSS to style the pagination controls.

/* transactions.component.css */
.transaction-list {
  margin: 20px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

table, th, td {
  border: 1px solid #ddd;
  padding: 8px;
}

th {
  background-color: #f2f2f2;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 10px;
}

.pagination button {
  margin: 0 5px;
  padding: 5px 10px;
  border: none;
  background-color: #007bff;
  color: white;
  cursor: pointer;
  transition: background-color 0.3s;
}

.pagination button[disabled] {
  background-color: #ccc;
  cursor: not-allowed;
}

.pagination button:not([disabled]):hover {
  background-color: #0056b3;
}

Summary

This custom pagination setup provides the functionality to fetch and display transactions in a paginated way without relying on Angular Material. You can further enhance it with features like displaying a dynamic range of pages or allowing the user to select the page size.