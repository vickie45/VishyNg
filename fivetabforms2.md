Let's dive deeper into the form structure, covering how to handle form controls, validations, and the form submission process across the five tabs: Case Details, Cost Details, Past Medical, Maternity, and Accidental. We'll use **Reactive Forms** to handle form state and validation efficiently in Angular.

### 1. **Case Details Form**

We'll begin with the **Case Details** form, adding relevant form controls and validation.

#### CaseDetailsComponent

```html
<form [formGroup]="caseDetailsForm" (ngSubmit)="onSubmit()">
  <div class="form-group">
    <label for="caseNumber">Case Number</label>
    <input
      id="caseNumber"
      formControlName="caseNumber"
      class="form-control"
      [ngClass]="{ 'is-invalid': caseDetailsForm.get('caseNumber').invalid && caseDetailsForm.get('caseNumber').touched }"
    />
    <div *ngIf="caseDetailsForm.get('caseNumber').invalid && caseDetailsForm.get('caseNumber').touched" class="invalid-feedback">
      Case Number is required.
    </div>
  </div>

  <div class="form-group">
    <label for="patientName">Patient Name</label>
    <input
      id="patientName"
      formControlName="patientName"
      class="form-control"
      [ngClass]="{ 'is-invalid': caseDetailsForm.get('patientName').invalid && caseDetailsForm.get('patientName').touched }"
    />
    <div *ngIf="caseDetailsForm.get('patientName').invalid && caseDetailsForm.get('patientName').touched" class="invalid-feedback">
      Patient Name is required.
    </div>
  </div>

  <!-- Add other form fields for case details -->

  <button type="button" class="btn btn-primary" (click)="save()">Save</button>
  <button type="submit" class="btn btn-success">Submit</button>
</form>
```

#### CaseDetailsComponent TypeScript

```typescript
export class CaseDetailsComponent implements OnInit {
  @Output() save = new EventEmitter<any>();
  @Output() submit = new EventEmitter<any>();

  caseDetailsForm: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.caseDetailsForm = this.fb.group({
      caseNumber: ['', Validators.required],
      patientName: ['', Validators.required],
      // Add more form controls with validators as required
    });
  }

  save() {
    if (this.caseDetailsForm.valid) {
      this.save.emit(this.caseDetailsForm.value);
    } else {
      this.caseDetailsForm.markAllAsTouched(); // Highlight invalid fields
    }
  }

  onSubmit() {
    if (this.caseDetailsForm.valid) {
      this.submit.emit(this.caseDetailsForm.value);
    } else {
      this.caseDetailsForm.markAllAsTouched(); // Highlight invalid fields
    }
  }
}
```

- **Explanation:**
  - `caseDetailsForm` is a reactive form with fields for case number and patient name.
  - The `save()` method validates the form before emitting the form data to the parent component. If the form is invalid, it highlights the invalid fields.
  - The `onSubmit()` method behaves similarly but is tied to the form's submission event, making it trigger when the Submit button is clicked.

### 2. **Cost Details Form**

The **Cost Details** form might capture billing information such as total cost and insurance coverage.

#### CostDetailsComponent HTML

```html
<form [formGroup]="costDetailsForm" (ngSubmit)="onSubmit()">
  <div class="form-group">
    <label for="totalCost">Total Cost</label>
    <input
      id="totalCost"
      formControlName="totalCost"
      class="form-control"
      [ngClass]="{ 'is-invalid': costDetailsForm.get('totalCost').invalid && costDetailsForm.get('totalCost').touched }"
    />
    <div *ngIf="costDetailsForm.get('totalCost').invalid && costDetailsForm.get('totalCost').touched" class="invalid-feedback">
      Total Cost is required.
    </div>
  </div>

  <div class="form-group">
    <label for="insuranceCoverage">Insurance Coverage</label>
    <input
      id="insuranceCoverage"
      formControlName="insuranceCoverage"
      class="form-control"
      [ngClass]="{ 'is-invalid': costDetailsForm.get('insuranceCoverage').invalid && costDetailsForm.get('insuranceCoverage').touched }"
    />
    <div *ngIf="costDetailsForm.get('insuranceCoverage').invalid && costDetailsForm.get('insuranceCoverage').touched" class="invalid-feedback">
      Insurance Coverage is required.
    </div>
  </div>

  <button type="button" class="btn btn-primary" (click)="save()">Save</button>
  <button type="submit" class="btn btn-success">Submit</button>
</form>
```

#### CostDetailsComponent TypeScript

```typescript
export class CostDetailsComponent implements OnInit {
  @Output() save = new EventEmitter<any>();
  @Output() submit = new EventEmitter<any>();

  costDetailsForm: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.costDetailsForm = this.fb.group({
      totalCost: ['', Validators.required],
      insuranceCoverage: ['', Validators.required],
      // Add other form controls as needed
    });
  }

  save() {
    if (this.costDetailsForm.valid) {
      this.save.emit(this.costDetailsForm.value);
    } else {
      this.costDetailsForm.markAllAsTouched(); // Highlight invalid fields
    }
  }

  onSubmit() {
    if (this.costDetailsForm.valid) {
      this.submit.emit(this.costDetailsForm.value);
    } else {
      this.costDetailsForm.markAllAsTouched(); // Highlight invalid fields
    }
  }
}
```

- **Explanation:** Similar to the `CaseDetailsComponent`, `CostDetailsComponent` manages form controls for billing information. Validation ensures the fields are filled before allowing save or submit.

### 3. **Past Medical Form**

The **Past Medical** form could collect past medical history, including diagnoses and previous treatments.

#### PastMedicalComponent HTML

```html
<form [formGroup]="pastMedicalForm" (ngSubmit)="onSubmit()">
  <div class="form-group">
    <label for="diagnosis">Diagnosis</label>
    <input
      id="diagnosis"
      formControlName="diagnosis"
      class="form-control"
      [ngClass]="{ 'is-invalid': pastMedicalForm.get('diagnosis').invalid && pastMedicalForm.get('diagnosis').touched }"
    />
    <div *ngIf="pastMedicalForm.get('diagnosis').invalid && pastMedicalForm.get('diagnosis').touched" class="invalid-feedback">
      Diagnosis is required.
    </div>
  </div>

  <div class="form-group">
    <label for="treatment">Previous Treatment</label>
    <textarea
      id="treatment"
      formControlName="treatment"
      class="form-control"
      [ngClass]="{ 'is-invalid': pastMedicalForm.get('treatment').invalid && pastMedicalForm.get('treatment').touched }"
    ></textarea>
    <div *ngIf="pastMedicalForm.get('treatment').invalid && pastMedicalForm.get('treatment').touched" class="invalid-feedback">
      Treatment information is required.
    </div>
  </div>

  <button type="button" class="btn btn-primary" (click)="save()">Save</button>
  <button type="submit" class="btn btn-success">Submit</button>
</form>
```

#### PastMedicalComponent TypeScript

```typescript
export class PastMedicalComponent implements OnInit {
  @Output() save = new EventEmitter<any>();
  @Output() submit = new EventEmitter<any>();

  pastMedicalForm: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.pastMedicalForm = this.fb.group({
      diagnosis: ['', Validators.required],
      treatment: ['', Validators.required],
    });
  }

  save() {
    if (this.pastMedicalForm.valid) {
      this.save.emit(this.pastMedicalForm.value);
    } else {
      this.pastMedicalForm.markAllAsTouched(); // Highlight invalid fields
    }
  }

  onSubmit() {
    if (this.pastMedicalForm.valid) {
      this.submit.emit(this.pastMedicalForm.value);
    } else {
      this.pastMedicalForm.markAllAsTouched(); // Highlight invalid fields
    }
  }
}
```

### 4. **Maternity Form**

The **Maternity** form includes optional fields and radio buttons, reflecting whether the patient has maternity history.

#### MaternityComponent HTML

```html
<form [formGroup]="maternityForm" (ngSubmit)="onSubmit()">
  <div class="form-group">
    <label>Has Maternity History?</label>
    <div>
      <input type="radio" formControlName="maternityHistory" [value]="true" /> Yes
      <input type="radio" formControlName="maternityHistory" [value]="false" /> No
    </div>
  </div>

  <div *ngIf="maternityForm.get('maternityHistory').value">
    <div class="form-group">
      <label for="lastChildBirthDate">Last Childbirth Date</label>
      <input
        id="lastChildBirthDate"
        formControlName="lastChildBirthDate"
        class="form-control"
      />
    </div>
    <div class="form-group">
  <label for="complications">Complications (if any)</label>
  <textarea
    id="complications"
    formControlName="complications"
    class="form-control"
  ></textarea>
</div>
>
    <div class="form-group">
      <label for="complications">Complications (if any)</label>
      <textarea
        id="complications"
        formControlName="complications"
        class="form-control"
      ></textarea>
    </div>
  </div>

  <button type="button" class="btn btn-primary" (click)="save()">Save</button>
  <button type="submit" class="btn btn-success">Submit</button>
</form>
```

#### MaternityComponent TypeScript

```typescript
export class MaternityComponent implements OnInit {
  @Output() save = new EventEmitter<any>();
  @Output() submit = new EventEmitter<any>();

  maternityForm: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.maternityForm = this.fb.group({
      maternityHistory: [null, Validators.required],
      lastChildBirthDate: [''],
      complications: [''],
    });

    this.maternityForm.get('maternityHistory').valueChanges.subscribe((value) => {
      if (value === true) {
        this.maternityForm.get('lastChildBirthDate').setValidators([Validators.required]);
      } else {
        this.maternityForm.get('lastChildBirthDate').clearValidators();
      }
      this.maternityForm.get('lastChildBirthDate').updateValueAndValidity();
    });
  }

  save() {
    if (this.maternityForm.valid) {
      this.save.emit(this.maternityForm.value);
    } else {
      this.maternityForm.markAllAsTouched();
    }
  }

  onSubmit() {
    if (this.maternityForm.valid) {
      this.submit.emit(this.maternityForm.value);
    } else {
      this.maternityForm.markAllAsTouched();
    }
  }
}
```

### 5. **Accidental Form**

The **Accidental** form is similar to the Maternity form, using radio buttons to capture optional fields.

#### AccidentalComponent HTML

```html
<form [formGroup]="accidentalForm" (ngSubmit)="onSubmit()">
  <div class="form-group">
    <label>Has Accidental History?</label>
    <div>
      <input type="radio" formControlName="accidentalHistory" [value]="true" /> Yes
      <input type="radio" formControlName="accidentalHistory" [value]="false" /> No
    </div>
  </div>

  <div *ngIf="accidentalForm.get('accidentalHistory').value">
    <div class="form-group">
      <label for="accidentDate">Accident Date</label>
      <input
        id="accidentDate"
        formControlName="accidentDate"
        class="form-control"
      />
    </div>
    <div class="form-group">
      <label for="injuries">Injuries (if any)</label>
      <textarea
        id="injuries"
        formControlName="injuries"
        class="form-control"
      ></textarea>
    </div>
  </div>

  <button type="button" class="btn btn-primary" (click)="save()">Save</button>
  <button type="submit" class="btn btn-success">Submit</button>
</form>
```

#### AccidentalComponent TypeScript

```typescript
export class AccidentalComponent implements OnInit {
  @Output() save = new EventEmitter<any>();
  @Output() submit = new EventEmitter<any>();

  accidentalForm: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.accidentalForm = this.fb.group({
      accidentalHistory: [null, Validators.required],
      accidentDate: [''],
      injuries: [''],
    });

    this.accidentalForm.get('accidentalHistory').valueChanges.subscribe((value) => {
      if (value === true) {
        this.accidentalForm.get('accidentDate').setValidators([Validators.required]);
      } else {
        this.accidentalForm.get('accidentDate').clearValidators();
      }
      this.accidentalForm.get('accidentDate').updateValueAndValidity();
    });
  }

  save() {
    if (this.accidentalForm.valid) {
      this.save.emit(this.accidentalForm.value);
    } else {
      this.accidentalForm.markAllAsTouched();
    }
  }

  onSubmit() {
    if (this.accidentalForm.valid) {
      this.submit.emit(this.accidentalForm.value);
    } else {
      this.accidentalForm.markAllAsTouched();
    }
  }
}
```

### Summary

Each form component:
- Uses **Reactive Forms** for validation and control.
- Handles **optional fields** dynamically using radio buttons for `Maternity` and `Accidental` tabs.
- Emphasizes **save** and **submit** functionality with proper validation.

The forms are structured to provide clean separation between form logic and parent components, making the data flow intuitive while ensuring that all fields are validated before submission. Let me know if you need further details or adjustments!