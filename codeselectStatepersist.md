To make the ICD code selection persistent across tabs (like when it’s used under a CaseDetails tab), we’ll implement a global AppDataService that uses BehaviorSubject and localStorage for state persistence.

⸻

Step-by-Step: Global State Management with Persistence

⸻

1. AppDataService

app-data.service.ts

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

const ICD_KEY = 'selected_icd_codes';

@Injectable({ providedIn: 'root' })
export class AppDataService {
  private icdSubject = new BehaviorSubject<any[]>(this.loadFromStorage());

  icd$ = this.icdSubject.asObservable();

  private loadFromStorage(): any[] {
    const data = localStorage.getItem(ICD_KEY);
    return data ? JSON.parse(data) : [];
  }

  private saveToStorage(icds: any[]) {
    localStorage.setItem(ICD_KEY, JSON.stringify(icds));
  }

  setSelectedIcds(icds: any[]) {
    this.icdSubject.next(icds);
    this.saveToStorage(icds);
  }

  getSelectedIcds(): any[] {
    return this.icdSubject.getValue();
  }

  removeIcd(code: string) {
    const updated = this.getSelectedIcds().filter(i => i.code !== code);
    this.setSelectedIcds(updated);
  }

  clearIcds() {
    this.icdSubject.next([]);
    localStorage.removeItem(ICD_KEY);
  }
}


⸻

2. Use in icd.component.ts

import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IcdModalComponent } from './icd-modal/icd-modal.component';
import { AppDataService } from '../shared/app-data.service';

@Component({
  selector: 'app-icd',
  templateUrl: './icd.component.html'
})
export class IcdComponent implements OnInit {
  selectedCodes: any[] = [];

  constructor(private modalService: NgbModal, private appData: AppDataService) {}

  ngOnInit() {
    this.appData.icd$.subscribe(data => this.selectedCodes = data);
  }

  openIcdModal() {
    const modalRef = this.modalService.open(IcdModalComponent, { size: 'lg' });
    modalRef.componentInstance.preSelected = this.selectedCodes.map(i => i.code);

    modalRef.result.then((result) => {
      if (result) {
        this.appData.setSelectedIcds(result);
      }
    }).catch(() => {});
  }

  removeCode(code: string) {
    this.appData.removeIcd(code);
  }
}


⸻

3. Use AppDataService in Modal Too

icd-modal.component.ts

import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IcdService } from '../icd.service';
import { AppDataService } from '../../shared/app-data.service';

@Component({
  selector: 'app-icd-modal',
  templateUrl: './icd-modal.component.html'
})
export class IcdModalComponent implements OnInit {
  @Input() preSelected: string[] = [];

  icdCodes: any[] = [];
  selectedMap = new Map<string, any>();
  searchTerm = '';
  page = 1;
  pageSize = 10;
  totalItems = 0;

  constructor(
    public activeModal: NgbActiveModal,
    private icdService: IcdService,
    private appData: AppDataService
  ) {}

  ngOnInit() {
    const saved = this.appData.getSelectedIcds();
    for (const icd of saved) {
      this.selectedMap.set(icd.code, icd);
    }
    this.fetchCodes();
  }

  fetchCodes() {
    this.icdService.getIcdCodes(this.searchTerm, this.page, this.pageSize).subscribe(res => {
      this.icdCodes = res.items;
      this.totalItems = res.total;
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
    const selectedList = Array.from(this.selectedMap.values());
    this.activeModal.close(selectedList); // handled in parent to save in AppDataService
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


⸻

4. Summary Table in Any Tab

Wherever you include the ICD summary (like in CaseDetails tab):

<app-icd></app-icd>

It will stay in sync across tabs due to BehaviorSubject and localStorage.

⸻

5. Optional Improvements
	•	Add expiry time to the stored data.
	•	Create a base <app-state-table> to reuse this pattern for diagnosis, allergies, etc.
	•	Emit event to parent CaseDetailsComponent if needed using Output() or shared service.

⸻

Would you like me to export this as a ready-to-run Angular project with routing, navigation, and Bootstrap 5 styles integrated?