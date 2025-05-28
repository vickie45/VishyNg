Here is the complete Angular 17 solution combining everything:

✅ Paginated ICD Code List Modal
✅ Max 5 unique selections
✅ Persistent selection across modal open/close, tabs, reloads
✅ Uses shared AppDataService with BehaviorSubject + localStorage
✅ Selected codes displayed in a table with delete option
✅ All written in clean, modular, scalable format

⸻

📁 File Structure

src/
├── app/
│   ├── app-data.service.ts      # Shared app state (BehaviorSubject + localStorage)
│   ├── icd-selector/
│   │   ├── icd-selector.component.ts
│   │   └── icd-selector.component.html


⸻

🧠 1. AppDataService – Global ICD State

app-data.service.ts

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface IcdCode {
  code: string;
  description: string;
}

const STORAGE_KEY = 'selected_icd_codes';

@Injectable({
  providedIn: 'root'
})
export class AppDataService {
  private _selectedIcdList$: BehaviorSubject<IcdCode[]>;

  constructor() {
    const saved = localStorage.getItem(STORAGE_KEY);
    const parsed: IcdCode[] = saved ? JSON.parse(saved) : [];
    this._selectedIcdList$ = new BehaviorSubject<IcdCode[]>(parsed);
  }

  get selectedIcdList$() {
    return this._selectedIcdList$.asObservable();
  }

  get selectedIcdList(): IcdCode[] {
    return this._selectedIcdList$.getValue();
  }

  setSelectedIcdList(list: IcdCode[]) {
    this._selectedIcdList$.next(list);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(list));
  }

  removeIcdCode(code: string) {
    const updated = this.selectedIcdList.filter(c => c.code !== code);
    this.setSelectedIcdList(updated);
  }
}


⸻

🧱 2. icd-selector.component.ts – Main Logic

icd-selector.component.ts

import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppDataService, IcdCode } from '../app-data.service';

@Component({
  selector: 'app-icd-selector',
  templateUrl: './icd-selector.component.html',
})
export class IcdSelectorComponent implements OnInit {
  allIcdCodes: IcdCode[] = [];
  page = 1;
  pageSize = 10;

  modalSelectedIcdList: IcdCode[] = [];
  selectedIcdList: IcdCode[] = [];

  constructor(private modalService: NgbModal, private appData: AppDataService) {}

  ngOnInit() {
    // Dummy data for ICD codes
    for (let i = 1; i <= 100; i++) {
      this.allIcdCodes.push({
        code: `ICD${i}`,
        description: `ICD Description ${i}`
      });
    }

    // Subscribe to global selected ICD list
    this.appData.selectedIcdList$.subscribe(list => {
      this.selectedIcdList = list;
    });
  }

  openModal(content: any) {
    this.modalSelectedIcdList = [...this.appData.selectedIcdList];
    this.modalService.open(content, { size: 'lg', scrollable: true });
  }

  isChecked(code: IcdCode): boolean {
    return this.modalSelectedIcdList.some(c => c.code === code.code);
  }

  toggleSelection(code: IcdCode) {
    const index = this.modalSelectedIcdList.findIndex(c => c.code === code.code);
    if (index >= 0) {
      this.modalSelectedIcdList.splice(index, 1);
    } else if (this.modalSelectedIcdList.length < 5) {
      this.modalSelectedIcdList.push(code);
    }
  }

  saveSelection(modal: any) {
    this.appData.setSelectedIcdList([...this.modalSelectedIcdList]);
    modal.close();
  }

  deleteCode(code: IcdCode) {
    this.appData.removeIcdCode(code.code);
  }
}


⸻

📄 3. icd-selector.component.html – UI Layout

icd-selector.component.html

<!-- Button to open modal -->
<button class="btn btn-outline-primary mb-3" (click)="openModal(icdModal)">
  Search ICD Codes
</button>

<!-- Modal Template -->
<ng-template #icdModal let-modal>
  <div class="modal-header">
    <h5 class="modal-title">Select up to 5 ICD Codes</h5>
    <button type="button" class="btn-close" aria-label="Close" (click)="modal.dismiss()"></button>
  </div>

  <div class="modal-body">
    <ngb-pagination
      [collectionSize]="allIcdCodes.length"
      [(page)]="page"
      [pageSize]="pageSize"
      [boundaryLinks]="true"
    ></ngb-pagination>

    <div *ngFor="let code of allIcdCodes.slice((page - 1) * pageSize, page * pageSize)">
      <div class="form-check">
        <input
          type="checkbox"
          class="form-check-input"
          [id]="code.code"
          [checked]="isChecked(code)"
          [disabled]="!isChecked(code) && modalSelectedIcdList.length >= 5"
          (change)="toggleSelection(code)"
        />
        <label class="form-check-label" [for]="code.code">
          {{ code.code }} - {{ code.description }}
        </label>
      </div>
    </div>
  </div>

  <div class="modal-footer">
    <button class="btn btn-primary" (click)="saveSelection(modal)">Save</button>
  </div>
</ng-template>

<!-- Selected ICD Codes Table -->
<div class="mt-4">
  <h5>Selected ICD Codes:</h5>
  <table class="table table-bordered table-hover" *ngIf="selectedIcdList.length > 0">
    <thead>
      <tr>
        <th>Code</th>
        <th>Description</th>
        <th>Action</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let code of selectedIcdList">
        <td>{{ code.code }}</td>
        <td>{{ code.description }}</td>
        <td>
          <button class="btn btn-sm btn-danger" (click)="deleteCode(code)">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>

  <p *ngIf="selectedIcdList.length === 0" class="text-muted">No ICD codes selected.</p>
</div>


⸻

📦 Install Required Dependencies

Make sure you have ng-bootstrap installed and configured:

ng add @ng-bootstrap/ng-bootstrap

In your app.module.ts:

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [...],
  imports: [
    ...,
    NgbModule
  ]
})
export class AppModule {}


⸻

✅ Final Result

Feature	Status
Paginated ICD code modal	✅
Max 5 unique selections	✅
Table view with delete	✅
Reopen modal = old state	✅
Persist after reload	✅
Shared globally	✅


⸻

Would you like the ICD categories tabbed view inside the modal next? Or full unit tests and reusable ICD modal component?