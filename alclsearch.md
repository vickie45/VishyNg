Here’s the Bootstrap 5-based implementation of your AlclSearchComponent with modular structure, API-driven data, and dynamic UI interactions.

Features

✔ Search & Display Data: Members, Health Card No, Policy, Registration No, Employee ID, Relationship.
✔ Expandable Toggle (+ / -): Expands to show Preauth and Claims.
✔ Tabs (IPD & OPD): Under Preauth and Claims.
✔ Details Table: Shows Intimation No (clickable), AL No, CL No, Status, Time Generated.
✔ Bootstrap 5 Modal: Opens on Intimation No click with a form.
✔ API-Driven Dropdowns: Selection options fetched from API.
✔ Conditional Navigation: Redirects based on form selection.

1. alcl-search.component.ts

import { Component, OnInit } from '@angular/core';
import { AlclSearchService } from './alcl-search.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntimationModalComponent } from './intimation-modal/intimation-modal.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-alcl-search',
  templateUrl: './alcl-search.component.html',
  styleUrls: ['./alcl-search.component.scss']
})
export class AlclSearchComponent implements OnInit {
  searchResults: any[] = [];
  expandedRow: number | null = null;

  constructor(
    private searchService: AlclSearchService,
    private modalService: NgbModal,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchSearchResults();
  }

  fetchSearchResults() {
    this.searchService.getSearchResults().subscribe(data => {
      this.searchResults = data;
    });
  }

  toggleExpand(index: number) {
    this.expandedRow = this.expandedRow === index ? null : index;
  }

  openIntimationModal(intimationNo: string) {
    const modalRef = this.modalService.open(IntimationModalComponent, { size: 'lg' });
    modalRef.componentInstance.intimationNo = intimationNo;

    modalRef.result.then((result) => {
      if (result?.navigateTo) {
        this.router.navigate([result.navigateTo]);
      }
    }).catch(() => {});
  }
}

2. alcl-search.component.html

<div class="container mt-3">
  <h4 class="mb-3">ALCL Search Results</h4>
  
  <table class="table table-striped table-bordered">
    <thead class="table-dark">
      <tr>
        <th>#</th>
        <th>Member</th>
        <th>Health Card No</th>
        <th>Policy</th>
        <th>Registration No</th>
        <th>Employee ID</th>
        <th>Relationship</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let result of searchResults; let i = index">
        <td>{{ i + 1 }}</td>
        <td>{{ result.member }}</td>
        <td>{{ result.healthCardNo }}</td>
        <td>{{ result.policy }}</td>
        <td>{{ result.registrationNo }}</td>
        <td>{{ result.employeeId }}</td>
        <td>{{ result.relationship }}</td>
        <td>
          <button class="btn btn-outline-primary btn-sm" (click)="toggleExpand(i)">
            {{ expandedRow === i ? '−' : '+' }}
          </button>
        </td>
      </tr>

      <tr *ngIf="expandedRow === i">
        <td colspan="8">
          <ul class="nav nav-pills mb-3">
            <li class="nav-item">
              <a class="nav-link" [class.active]="result.activeTab === 'preauth'"
                (click)="result.activeTab = 'preauth'">Preauth</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" [class.active]="result.activeTab === 'claims'"
                (click)="result.activeTab = 'claims'">Claims</a>
            </li>
          </ul>

          <div *ngIf="result.activeTab">
            <ul class="nav nav-tabs">
              <li class="nav-item">
                <a class="nav-link" [class.active]="result.activeSubTab === 'ipd'"
                  (click)="result.activeSubTab = 'ipd'">IPD</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" [class.active]="result.activeSubTab === 'opd'"
                  (click)="result.activeSubTab = 'opd'">OPD</a>
              </li>
            </ul>

            <table class="table mt-2" *ngIf="result.activeSubTab">
              <thead>
                <tr>
                  <th>Intimation No</th>
                  <th>AL No</th>
                  <th>CL No</th>
                  <th>Status</th>
                  <th>Time Generated</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let item of result[result.activeSubTab]">
                  <td>
                    <a href="#" (click)="openIntimationModal(item.intimationNo)">
                      {{ item.intimationNo }}
                    </a>
                  </td>
                  <td>{{ item.alNo }}</td>
                  <td>{{ item.clNo }}</td>
                  <td>{{ item.status }}</td>
                  <td>{{ item.timeGenerated }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </td>
      </tr>
    </tbody>
  </table>
</div>

3. intimation-modal.component.ts

import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AlclSearchService } from '../alcl-search.service';

@Component({
  selector: 'app-intimation-modal',
  templateUrl: './intimation-modal.component.html',
  styleUrls: ['./intimation-modal.component.scss']
})
export class IntimationModalComponent implements OnInit {
  @Input() intimationNo!: string;
  intimationForm!: FormGroup;
  options: any[] = [];

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private searchService: AlclSearchService
  ) {}

  ngOnInit(): void {
    this.intimationForm = this.fb.group({
      amount: [''],
      selection: ['']
    });

    this.fetchOptions();
  }

  fetchOptions() {
    this.searchService.getSelectionOptions().subscribe(data => {
      this.options = data;
    });
  }

  submit() {
    const selectedValue = this.intimationForm.value.selection;
    let navigateTo = selectedValue === 'Option1' ? '/component1' : '/component2';
    this.activeModal.close({ navigateTo });
  }
}

4. intimation-modal.component.html

<div class="modal-header">
  <h5 class="modal-title">Intimation Details</h5>
  <button type="button" class="btn-close" (click)="activeModal.dismiss()"></button>
</div>

<div class="modal-body">
  <form [formGroup]="intimationForm">
    <div class="mb-3">
      <label class="form-label">Amount:</label>
      <input type="number" formControlName="amount" class="form-control" />
    </div>

    <div class="mb-3">
      <label class="form-label">Selection:</label>
      <select formControlName="selection" class="form-select">
        <option *ngFor="let option of options" [value]="option.value">
          {{ option.label }}
        </option>
      </select>
    </div>
  </form>
</div>

<div class="modal-footer">
  <button type="button" class="btn btn-primary" (click)="submit()">Submit</button>
</div>

Final Thoughts

✅ Uses Bootstrap 5 (No Material UI)
✅ Clean, modular, and API-driven
✅ Follows Angular best practices

Let me know if you need any refinements!