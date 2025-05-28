Understood. You had CaseDetails, PastMedical, and CosDetails as separate page-level components earlier, and now you want to refactor them to become child components inside a parent ALDetailsComponent (or a similar container component for all details).

Here is a practical guide to do that in Angular:

⸻

Step 1: Create the Parent Component

If you don’t have the parent container component (say ALDetailsComponent), create it:

ng generate component ALDetails


⸻

Step 2: Modify Child Components to be Embedded

Make sure CaseDetails, PastMedical, and CosDetails are ready to be used as child components.

They should:
	•	Have @Input() decorators for data they receive from the parent (if needed).
	•	Have @Output() events if they need to notify the parent about changes/actions.

Example in case-details.component.ts:

import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-case-details',
  templateUrl: './case-details.component.html',
  styleUrls: ['./case-details.component.css']
})
export class CaseDetailsComponent {
  @Input() caseData: any;
  @Output() caseUpdated = new EventEmitter<any>();

  // Methods to emit changes to parent
  updateCase(data: any) {
    this.caseUpdated.emit(data);
  }
}


⸻

Step 3: Include Child Components in Parent Template

In al-details.component.html, embed child components like this:

<app-case-details [caseData]="caseDetails" (caseUpdated)="onCaseUpdated($event)"></app-case-details>
<app-past-medical [medicalData]="pastMedicalDetails" (medicalUpdated)="onMedicalUpdated($event)"></app-past-medical>
<app-cos-details [cosData]="cosDetails" (cosUpdated)="onCosUpdated($event)"></app-cos-details>


⸻

Step 4: Manage Data in Parent Component Class

In al-details.component.ts, define the data properties and handlers:

export class ALDetailsComponent {
  caseDetails: any = {};       // Data for case details
  pastMedicalDetails: any = {}; // Data for past medical
  cosDetails: any = {};        // Data for cos details

  // Handlers for child events
  onCaseUpdated(updatedData: any) {
    this.caseDetails = updatedData;
    // Possibly call service or update the backend
  }

  onMedicalUpdated(updatedData: any) {
    this.pastMedicalDetails = updatedData;
  }

  onCosUpdated(updatedData: any) {
    this.cosDetails = updatedData;
  }
}


⸻

Step 5: Adjust Routing (if any)

If earlier CaseDetails, PastMedical, and CosDetails had their own routes/pages, remove or disable those routes and replace navigation to the parent component, which now contains all three child components.

⸻

Summary
	•	Convert individual pages into child components.
	•	Use @Input() and @Output() to pass data and events.
	•	Parent component manages the state and acts as a container.
	•	Adjust routing accordingly.

⸻

If you want, I can also help you with a sample full code example or how to manage form validation and submission across child components inside the parent form. Would you like that?