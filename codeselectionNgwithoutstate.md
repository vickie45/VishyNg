Here’s a complete implementation approach to design an ICD code selection UI in Angular 17 using Bootstrap 5 and @ng-bootstrap/ng-bootstrap, covering all edge cases and behaviors.

⸻

Step-by-Step Implementation

1. UI Overview
	•	Main Page: Shows selected ICD codes in a table with delete buttons.
	•	Search Button: Triggers modal to search and select ICD codes.
	•	Modal:
	•	Search box.
	•	Paginated checkbox list.
	•	“Save” and “Cancel” buttons.

⸻

2. Folder Structure

icd/
├── icd.component.ts
├── icd.component.html
├── icd.component.css
├── icd.service.ts
└── icd-modal/
    ├── icd-modal.component.ts
    ├── icd-modal.component.html
    └── icd-modal.component.css


⸻

3. ICD Service (Mocked)

// icd.service.ts
import { Injectable } from '@angular/core';
import { of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class IcdService {
  private icdCodes = Array.from({ length: 100 }).map((_, i) => ({
    code: `ICD-${i + 1}`,
    description: `ICD Description ${i + 1}`,
  }));

  getIcdCodes(search: string = '', page: number = 1, pageSize: number = 10) {
    const filtered = this.icdCodes.filter(i =>
      i.code.toLowerCase().includes(search.toLowerCase()) ||
      i.description.toLowerCase().includes(search.toLowerCase())
    );
    const start = (page - 1) * pageSize;
    return of({
      items: filtered.slice(start, start + pageSize),
      total: filtered.length
    });
  }
}


⸻

4. Main Component

icd.component.ts

import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IcdModalComponent } from './icd-modal/icd-modal.component';

@Component({
  selector: 'app-icd',
  templateUrl: './icd.component.html',
})
export class IcdComponent {
  selectedCodes: any[] = [];

  constructor(private modalService: NgbModal) {}

  openIcdModal() {
    const modalRef = this.modalService.open(IcdModalComponent, { size: 'lg' });
    modalRef.componentInstance.preSelected = this.selectedCodes.map(c => c.code);

    modalRef.result.then((result) => {
      if (result) {
        this.selectedCodes = result;
      }
    }).catch(() => {});
  }

  removeCode(code: string) {
    this.selectedCodes = this.selectedCodes.filter(c => c.code !== code);
  }
}

icd.component.html

<button class="btn btn-primary" (click)="openIcdModal()">
  ICD Search 🔍
</button>

<table class="table mt-3" *ngIf="selectedCodes.length">
  <thead>
    <tr>
      <th>Code</th>
      <th>Description</th>
      <th>Action</th>
    </tr>
  </thead>
  <tbody>
    <tr *ngFor="let icd of selectedCodes">
      <td>{{ icd.code }}</td>
      <td>{{ icd.description }}</td>
      <td>
        <button class="btn btn-sm btn-danger" (click)="removeCode(icd.code)">Delete</button>
      </td>
    </tr>
  </tbody>
</table>


⸻

5. Modal Component

icd-modal.component.ts

import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IcdService } from '../icd.service';

@Component({
  selector: 'app-icd-modal',
  templateUrl: './icd-modal.component.html',
})
export class IcdModalComponent implements OnInit {
  @Input() preSelected: string[] = [];

  icdCodes: any[] = [];
  selectedMap = new Map<string, any>();
  searchTerm = '';
  page = 1;
  pageSize = 10;
  totalItems = 0;

  constructor(public activeModal: NgbActiveModal, private icdService: IcdService) {}

  ngOnInit() {
    this.fetchCodes();
  }

  fetchCodes() {
    this.icdService.getIcdCodes(this.searchTerm, this.page, this.pageSize).subscribe(res => {
      this.icdCodes = res.items;
      this.totalItems = res.total;

      // Restore selections
      for (const code of this.preSelected) {
        const match = res.items.find(i => i.code === code);
        if (match) {
          this.selectedMap.set(code, match);
        }
      }
    });
  }

  toggleSelection(code: string, icd: any, event: any) {
    if (event.target.checked) {
      this.selectedMap.set(code, icd);
    } else {
      this.selectedMap.delete(code);
    }
  }

  isChecked(code: string): boolean {
    return this.selectedMap.has(code);
  }

  onSave() {
    this.activeModal.close(Array.from(this.selectedMap.values()));
  }

  onSearchChange() {
    this.page = 1;
    this.fetchCodes();
  }

  onPageChange(page: number) {
    this.page = page;
    this.fetchCodes();
  }
}

icd-modal.component.html

<div class="modal-header">
  <h5 class="modal-title">Select ICD Codes</h5>
  <button type="button" class="btn-close" (click)="activeModal.dismiss()"></button>
</div>
<div class="modal-body">
  <input type="text" class="form-control mb-3" placeholder="Search ICD..." [(ngModel)]="searchTerm" (input)="onSearchChange()"/>

  <div *ngFor="let icd of icdCodes" class="form-check">
    <input type="checkbox" class="form-check-input"
           [checked]="isChecked(icd.code)"
           (change)="toggleSelection(icd.code, icd, $event)">
    <label class="form-check-label">
      {{ icd.code }} - {{ icd.description }}
    </label>
  </div>

  <ngb-pagination [collectionSize]="totalItems"
                  [(page)]="page"
                  [pageSize]="pageSize"
                  (pageChange)="onPageChange($event)">
  </ngb-pagination>
</div>
<div class="modal-footer">
  <button class="btn btn-secondary" (click)="activeModal.dismiss()">Cancel</button>
  <button class="btn btn-primary" (click)="onSave()">Save</button>
</div>


⸻

Edge Cases Handled
	•	Prevents duplicate entries.
	•	Preserves selected codes across paginated views.
	•	Search resets to page 1.
	•	Selections only committed on Save.
	•	Clean deselection and deletion.

⸻

Would you like this wrapped in a GitHub-ready Angular project structure with routing and service injection? I can also include test cases and best practices for optimization.