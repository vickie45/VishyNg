 Here's a complete and consistent structure for OPD (Outpatient Department) that matches your previous AL and CL service designs.


---

✅ OPD – Full Service & App State Pattern

This setup will:

Provide GET APIs for all master data dropdowns across OPD tabs.

Get IDs via getOpdId() or copyFromPrevious().

Store the resolved ID in a central OpdAppState service using localStorage.

Use this ID for calling saveOPD() and submitOPD().



---

✅ 1. OpdDataService.ts: App State + Local Storage

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface OpdAppState {
  opdId: number | null;
  enhancementId: number | null;
  queryResponseId: number | null;
}

const LOCAL_KEY = 'opdAppState';

@Injectable({
  providedIn: 'root'
})
export class OpdDataService {
  private _state: OpdAppState = JSON.parse(localStorage.getItem(LOCAL_KEY) || '{}') || {
    opdId: null,
    enhancementId: null,
    queryResponseId: null
  };

  private appStateSubject = new BehaviorSubject<OpdAppState>(this._state);
  public appState$ = this.appStateSubject.asObservable();

  get appState(): OpdAppState {
    return this._state;
  }

  updateState(newState: Partial<OpdAppState>) {
    this._state = { ...this._state, ...newState };
    this.appStateSubject.next(this._state);
    localStorage.setItem(LOCAL_KEY, JSON.stringify(this._state));
  }

  clearState() {
    this._state = { opdId: null, enhancementId: null, queryResponseId: null };
    this.appStateSubject.next(this._state);
    localStorage.removeItem(LOCAL_KEY);
  }
}


---

✅ 2. OpdService.ts: Master Dropdowns, ID Resolution, Save & Submit

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { OpdDataService } from './opd-data.service';

@Injectable({
  providedIn: 'root'
})
export class OpdService {
  private baseUrl = '/api/opd';

  constructor(
    private http: HttpClient,
    private opdDataService: OpdDataService
  ) {}

  // ------------------- Master Dropdowns -------------------
  getVisitReasons() { return this.http.get(`${this.baseUrl}/masters/visit-reasons`); }
  getDoctors() { return this.http.get(`${this.baseUrl}/masters/doctors`); }
  getDepartments() { return this.http.get(`${this.baseUrl}/masters/departments`); }
  getConsultationTypes() { return this.http.get(`${this.baseUrl}/masters/consultation-types`); }
  getSymptoms() { return this.http.get(`${this.baseUrl}/masters/symptoms`); }

  // ------------------- ID Resolution APIs -------------------
  getOpdId(uhid: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/opd-id/${uhid}`);
  }

  copyFromPrevious(preauthId: number, type: 'ENH' | 'QR'): Observable<any> {
    return this.http.get(`${this.baseUrl}/copy/${preauthId}?type=${type}`);
  }

  // ------------------- Save & Submit -------------------
  saveOPD(payload: any): Observable<any> {
    const state = this.opdDataService.appState;
    const id = state.opdId || state.enhancementId || state.queryResponseId;

    return this.http.post(`${this.baseUrl}/save`, { ...payload, id });
  }

  submitOPD(payload: any): Observable<any> {
    const state = this.opdDataService.appState;
    const id = state.opdId || state.enhancementId || state.queryResponseId;

    return this.http.post(`${this.baseUrl}/submit`, { ...payload, id });
  }

  // ------------------- All Dropdowns Together -------------------
  getAllDropdowns(): Observable<any> {
    return forkJoin({
      visitReasons: this.getVisitReasons(),
      doctors: this.getDoctors(),
      departments: this.getDepartments(),
      consultationTypes: this.getConsultationTypes(),
      symptoms: this.getSymptoms()
    });
  }
}


---

✅ 3. Component Usage

ngOnInit() {
  const uhid = 'UHID123';

  // Try OPD ID
  this.opdService.getOpdId(uhid).subscribe(res => {
    if (res?.opdId) {
      this.opdDataService.updateState({ opdId: res.opdId });
    } else {
      // Fallback to Copy
      this.opdService.copyFromPrevious(Number(uhid), 'ENH').subscribe(copyRes => {
        if (copyRes?.enhancementId) {
          this.opdDataService.updateState({ enhancementId: copyRes.enhancementId });
        } else if (copyRes?.queryResponseId) {
          this.opdDataService.updateState({ queryResponseId: copyRes.queryResponseId });
        }
      });
    }
  });

  // Dropdowns for OPD form
  this.opdService.getAllDropdowns().subscribe(data => {
    this.visitReasons = data.visitReasons;
    this.consultationTypes = data.consultationTypes;
    // ...
  });
}


---

✅ Summary Table (AL, CL, OPD Pattern)

Feature	OPD	CL	AL

All tab dropdowns (GET)	✅	✅	✅
getId() + copyAPI()	✅	✅	✅
App state via BehaviorSubject	✅	✅	✅
Persist state in localStorage	✅	✅	✅
save() and submit() with ID	✅	✅	✅



---

🔄 Would you like:

A shared base class for all 3 services to avoid duplication?

A factory service to load the correct service (AL/CL/OPD) dynamically?

A UI-ready payload generator per module (AL/CL/OPD)?


Let me know if you want all three combined into a scalable architecture.

