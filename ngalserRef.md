Certainly. Below is the complete implementation of the AlService and AppDataService with full support for:

Getting master data for dropdowns across 5 tabs

Handling API selection between preauthId, copyAPI (enhancement/query response)

Maintaining app state in localStorage

Hitting save and submit APIs using the selected ID (preauthId or enhancement/query response ID)



---

✅ Final Full Code Setup


---

1. ✅ AppDataService: App State + Local Storage

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface AlAppState {
  preauthId: number | null;
  enhancementId: number | null;
  queryResponseId: number | null;
}

const LOCAL_KEY = 'alAppState';

@Injectable({
  providedIn: 'root'
})
export class AppDataService {
  private _state: AlAppState = JSON.parse(localStorage.getItem(LOCAL_KEY) || '{}') || {
    preauthId: null,
    enhancementId: null,
    queryResponseId: null
  };

  private appStateSubject = new BehaviorSubject<AlAppState>(this._state);
  public appState$ = this.appStateSubject.asObservable();

  get appState(): AlAppState {
    return this._state;
  }

  updateState(newState: Partial<AlAppState>) {
    this._state = { ...this._state, ...newState };
    this.appStateSubject.next(this._state);
    localStorage.setItem(LOCAL_KEY, JSON.stringify(this._state));
  }

  clearState() {
    this._state = { preauthId: null, enhancementId: null, queryResponseId: null };
    this.appStateSubject.next(this._state);
    localStorage.removeItem(LOCAL_KEY);
  }
}


---

2. ✅ AlService: All Master Data + Save/Submit + Preauth/Copy Logic

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { AppDataService } from './app-data.service';

@Injectable({
  providedIn: 'root'
})
export class AlService {
  private baseUrl = '/api/al';

  constructor(
    private http: HttpClient,
    private appDataService: AppDataService
  ) {}

  // ------------------- Master Dropdowns -------------------
  // Tab 1: Case Details
  getGenders() { return this.http.get(`${this.baseUrl}/masters/genders`); }
  getPatientTypes() { return this.http.get(`${this.baseUrl}/masters/patient-types`); }
  getSpecialities() { return this.http.get(`${this.baseUrl}/masters/specialities`); }
  getTreatingDoctors() { return this.http.get(`${this.baseUrl}/masters/doctors`); }

  // Tab 2: Cost Details
  getProcedureTypes() { return this.http.get(`${this.baseUrl}/masters/procedure-types`); }
  getTariffCodes() { return this.http.get(`${this.baseUrl}/masters/tariffs`); }
  getCostHeads() { return this.http.get(`${this.baseUrl}/masters/cost-heads`); }

  // Tab 3: Past Medical History
  getDiseases() { return this.http.get(`${this.baseUrl}/masters/diseases`); }
  getICD10Codes() { return this.http.get(`${this.baseUrl}/masters/icd-codes`); }

  // Tab 4: Maternity
  getMaternityStatus() { return this.http.get(`${this.baseUrl}/masters/maternity-status`); }
  getGravidaParityOptions() { return this.http.get(`${this.baseUrl}/masters/gp-options`); }

  // Tab 5: Injury/Accident
  getInjuryTypes() { return this.http.get(`${this.baseUrl}/masters/injury-types`); }
  getAccidentLocations() { return this.http.get(`${this.baseUrl}/masters/accident-locations`); }

  // ------------------- Preauth/Copy API -------------------
  getPreauthId(uhid: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/preauth-id/${uhid}`);
  }

  copyFromPrevious(preauthId: number, type: 'ENH' | 'QR'): Observable<any> {
    return this.http.get(`${this.baseUrl}/copy/${preauthId}?type=${type}`);
  }

  // ------------------- Save & Submit -------------------
  saveAL(payload: any): Observable<any> {
    const state = this.appDataService.appState;
    const id = state.preauthId || state.enhancementId || state.queryResponseId;

    return this.http.post(`${this.baseUrl}/save`, { ...payload, id });
  }

  submitAL(payload: any): Observable<any> {
    const state = this.appDataService.appState;
    const id = state.preauthId || state.enhancementId || state.queryResponseId;

    return this.http.post(`${this.baseUrl}/submit`, { ...payload, id });
  }

  // ------------------- Load All Dropdowns Together -------------------
  getAllDropdowns(): Observable<any> {
    return forkJoin({
      genders: this.getGenders(),
      patientTypes: this.getPatientTypes(),
      specialities: this.getSpecialities(),
      doctors: this.getTreatingDoctors(),
      procedureTypes: this.getProcedureTypes(),
      tariffCodes: this.getTariffCodes(),
      costHeads: this.getCostHeads(),
      diseases: this.getDiseases(),
      icd10Codes: this.getICD10Codes(),
      maternityStatus: this.getMaternityStatus(),
      gravidaParityOptions: this.getGravidaParityOptions(),
      injuryTypes: this.getInjuryTypes(),
      accidentLocations: this.getAccidentLocations()
    });
  }
}


---

3. ✅ Component Usage Example (Controller Logic)

ngOnInit() {
  const uhid = 'UHID123';

  // Try Preauth
  this.alService.getPreauthId(uhid).subscribe(res => {
    if (res?.preauthId) {
      this.appDataService.updateState({ preauthId: res.preauthId });
    } else {
      // Try Enhancement
      this.alService.copyFromPrevious(Number(uhid), 'ENH').subscribe(copyRes => {
        if (copyRes?.enhancementId) {
          this.appDataService.updateState({ enhancementId: copyRes.enhancementId });
        } else if (copyRes?.queryResponseId) {
          this.appDataService.updateState({ queryResponseId: copyRes.queryResponseId });
        }
      });
    }
  });

  // Fetch all dropdowns for form
  this.alService.getAllDropdowns().subscribe(data => {
    this.genderOptions = data.genders;
    this.specialityOptions = data.specialities;
    // etc...
  });
}


---

🔁 Reset on Logout

logout() {
  this.appDataService.clearState();
}


---

✅ Summary

You asked to:

Fetch IDs from preauth or copy APIs ✅

Use whichever response is not null ✅

Store in localStorage with observable app state ✅

Hit save/submit using the selected ID ✅

Provide all dropdown master APIs ✅


All of this is now wrapped into a complete Angular service and app state setup.
Would you like me to generate a real backend stub or mock server JSON .

