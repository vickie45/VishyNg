To create a tabbed card with a “Transactions” tab that has “OPD” and “IPD” subtabs, including a search function to filter by “Date of Admission” and “Patient Name” along with tables for each subtab that display transaction details fetched from an API, you can use the following Angular and Bootstrap setup:

1. HTML Structure (Angular Template)

Here’s an example Angular component template:

<div class="card">
  <div class="card-header">
    <ul class="nav nav-tabs card-header-tabs">
      <li class="nav-item">
        <a
          class="nav-link"
          [class.active]="activeTab === 'transactions'"
          (click)="selectTab('transactions')"
          >Transactions</a
        >
      </li>
    </ul>
  </div>

  <div class="card-body">
    <!-- Transactions Tab -->
    <div *ngIf="activeTab === 'transactions'">
      <!-- Search Filters -->
      <div class="mb-3 row">
        <div class="col-md-4">
          <label for="admissionDate" class="form-label">Date of Admission</label>
          <input
            type="date"
            id="admissionDate"
            [(ngModel)]="filters.admissionDate"
            (change)="fetchTransactions()"
            class="form-control"
          />
        </div>
        <div class="col-md-4">
          <label for="patientName" class="form-label">Patient Name</label>
          <input
            type="text"
            id="patientName"
            [(ngModel)]="filters.patientName"
            (input)="fetchTransactions()"
            class="form-control"
            placeholder="Enter patient name"
          />
        </div>
      </div>

      <!-- Subtabs for OPD and IPD -->
      <ul class="nav nav-pills mb-3">
        <li class="nav-item">
          <a
            class="nav-link"
            [class.active]="activeSubTab === 'opd'"
            (click)="selectSubTab('opd')"
            >OPD</a
          >
        </li>
        <li class="nav-item">
          <a
            class="nav-link"
            [class.active]="activeSubTab === 'ipd'"
            (click)="selectSubTab('ipd')"
            >IPD</a
          >
        </li>
      </ul>

      <!-- Table for OPD -->
      <div *ngIf="activeSubTab === 'opd'">
        <table class="table table-bordered">
          <thead>
            <tr>
              <th>Transaction ID</th>
              <th>Patient Name</th>
              <th>Date of Admission</th>
              <th>Amount</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let transaction of opdTransactions">
              <td>{{ transaction.id }}</td>
              <td>{{ transaction.patientName }}</td>
              <td>{{ transaction.dateOfAdmission | date }}</td>
              <td>{{ transaction.amount | currency }}</td>
              <td>{{ transaction.status }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Table for IPD -->
      <div *ngIf="activeSubTab === 'ipd'">
        <table class="table table-bordered">
          <thead>
            <tr>
              <th>Transaction ID</th>
              <th>Patient Name</th>
              <th>Date of Admission</th>
              <th>Amount</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let transaction of ipdTransactions">
              <td>{{ transaction.id }}</td>
              <td>{{ transaction.patientName }}</td>
              <td>{{ transaction.dateOfAdmission | date }}</td>
              <td>{{ transaction.amount | currency }}</td>
              <td>{{ transaction.status }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</div>

2. Component Logic (TypeScript)

The logic in the TypeScript file manages the tab switching, search filters, and API fetching.

import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-transaction-tabs',
  templateUrl: './transaction-tabs.component.html',
  styleUrls: ['./transaction-tabs.component.css'],
})
export class TransactionTabsComponent implements OnInit {
  activeTab: string = 'transactions';
  activeSubTab: string = 'opd';
  opdTransactions: any[] = [];
  ipdTransactions: any[] = [];
  filters = {
    admissionDate: '',
    patientName: '',
  };

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchTransactions();
  }

  selectTab(tab: string): void {
    this.activeTab = tab;
  }

  selectSubTab(subTab: string): void {
    this.activeSubTab = subTab;
    this.fetchTransactions();
  }

  fetchTransactions(): void {
    const params = {
      dateOfAdmission: this.filters.admissionDate,
      patientName: this.filters.patientName,
    };

    if (this.activeSubTab === 'opd') {
      this.http.get<any[]>('https://api.example.com/opd-transactions', { params })
        .subscribe(data => {
          this.opdTransactions = data;
        });
    } else if (this.activeSubTab === 'ipd') {
      this.http.get<any[]>('https://api.example.com/ipd-transactions', { params })
        .subscribe(data => {
          this.ipdTransactions = data;
        });
    }
  }
}

3. Styling (CSS)

Basic styling for a clean layout:

.card {
  margin-top: 20px;
}
.table {
  margin-top: 10px;
}
.nav-tabs .nav-link.active,
.nav-pills .nav-link.active {
  color: #fff;
  background-color: #007bff;
}

Explanation

	•	Tabs and Subtabs: Transactions is the main tab with OPD and IPD subtabs.
	•	Search Functionality: The search inputs allow filtering by “Date of Admission” and “Patient Name” and trigger fetchTransactions() on change.
	•	Dynamic API Fetching: fetchTransactions() checks the active subtab to call the corresponding API endpoint and updates either opdTransactions or ipdTransactions.
	•	Tables: Each subtab displays a table with transaction details returned by the API.

This setup will provide a fully functional, responsive UI for transaction management.