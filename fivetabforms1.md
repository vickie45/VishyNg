I'll walk you through a solution using Angular to handle multiple tabs with forms, the logic to save and submit the data to an Express server, and how to communicate with a WCF services API via the Express backend.

### 1. **Setting Up the HTML for the Tabs**
```html
<div class="container">
  <ul class="nav nav-tabs">
    <li class="nav-item">
      <a class="nav-link" [class.active]="activeTab === 'caseDetails'" (click)="selectTab('caseDetails')">Case Details</a>
    </li>
    <li class="nav-item">
      <a class="nav-link" [class.active]="activeTab === 'costDetails'" (click)="selectTab('costDetails')">Cost Details</a>
    </li>
    <li class="nav-item">
      <a class="nav-link" [class.active]="activeTab === 'pastMedical'" (click)="selectTab('pastMedical')">Past Medical</a>
    </li>
    <li class="nav-item">
      <a class="nav-link" [class.active]="activeTab === 'maternity'" (click)="selectTab('maternity')">Maternity</a>
    </li>
    <li class="nav-item">
      <a class="nav-link" [class.active]="activeTab === 'accidental'" (click)="selectTab('accidental')">Accidental</a>
    </li>
  </ul>

  <div [ngSwitch]="activeTab">
    <div *ngSwitchCase="'caseDetails'">
      <app-case-details (save)="onSave($event)" (submit)="onSubmit($event)"></app-case-details>
    </div>
    <div *ngSwitchCase="'costDetails'">
      <app-cost-details (save)="onSave($event)" (submit)="onSubmit($event)"></app-cost-details>
    </div>
    <div *ngSwitchCase="'pastMedical'">
      <app-past-medical (save)="onSave($event)" (submit)="onSubmit($event)"></app-past-medical>
    </div>
    <div *ngSwitchCase="'maternity'">
      <app-maternity (save)="onSave($event)" (submit)="onSubmit($event)"></app-maternity>
    </div>
    <div *ngSwitchCase="'accidental'">
      <app-accidental (save)="onSave($event)" (submit)="onSubmit($event)"></app-accidental>
    </div>
  </div>
</div>
```

- **Explanation:**
  - The `nav` element creates the tabs. Clicking on a tab sets the `activeTab` variable.
  - Each tab's component is rendered dynamically using Angular's `ngSwitch`.
  - Each component (`app-case-details`, `app-cost-details`, etc.) emits `save` and `submit` events that are handled by the parent component.

### 2. **Component Logic**

In the parent component, manage which tab is active and handle the save and submit events.

```typescript
export class ParentComponent {
  activeTab: string = 'caseDetails';

  selectTab(tab: string) {
    this.activeTab = tab;
  }

  onSave(data: any) {
    // Save data to backend (Express)
    this.http.post('/api/save', data).subscribe(response => {
      console.log('Data saved:', response);
    });
  }

  onSubmit(data: any) {
    // Submit data to backend (Express) and forward it to WCF service
    this.http.post('/api/submit', data).subscribe(response => {
      console.log('Data submitted:', response);
    });
  }
}
```

- **Explanation:**
  - `selectTab` changes the `activeTab` variable, which controls which tab is shown.
  - `onSave` sends the form data to the Express backend via a `POST` request to `/api/save`.
  - `onSubmit` sends the form data to `/api/submit`, which will forward the data to the WCF services API.

### 3. **Sample Tab Component (Case Details)**

Create each tab component with a form and save/submit buttons.

```html
<form [formGroup]="caseDetailsForm">
  <div class="form-group">
    <label for="caseNumber">Case Number</label>
    <input id="caseNumber" formControlName="caseNumber" class="form-control">
  </div>
  <!-- Other form fields for case details -->
  
  <button (click)="save()" class="btn btn-primary">Save</button>
  <button (click)="submit()" class="btn btn-success">Submit</button>
</form>
```

```typescript
export class CaseDetailsComponent {
  @Output() save = new EventEmitter<any>();
  @Output() submit = new EventEmitter<any>();

  caseDetailsForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.caseDetailsForm = this.fb.group({
      caseNumber: ['', Validators.required],
      // Other form controls
    });
  }

  save() {
    this.save.emit(this.caseDetailsForm.value);
  }

  submit() {
    this.submit.emit(this.caseDetailsForm.value);
  }
}
```

- **Explanation:**
  - `save()` and `submit()` methods emit the form data to the parent component.
  - You can follow a similar pattern for the other tab components (Cost Details, Past Medical, Maternity, Accidental).

### 4. **Backend (Express)**
Create routes in your Express server to handle save and submit actions. The submit route will communicate with the WCF services API.

```javascript
const express = require('express');
const axios = require('axios'); // For making requests to WCF
const app = express();

app.use(express.json());

app.post('/api/save', (req, res) => {
  const data = req.body;
  // Save logic here (e.g., save to a database)
  res.json({ message: 'Data saved successfully' });
});

app.post('/api/submit', async (req, res) => {
  const data = req.body;
  try {
    // Forward the data to WCF services API
    const wcfResponse = await axios.post('https://your-wcf-service-url', data);
    res.json({ message: 'Data submitted successfully', wcfResponse: wcfResponse.data });
  } catch (error) {
    res.status(500).json({ message: 'Failed to submit data to WCF', error });
  }
});

app.listen(3000, () => {
  console.log('Server running on port 3000');
});
```

- **Explanation:**
  - The `/api/save` route stores the data.
  - The `/api/submit` route forwards the data to the WCF services API using `axios`.

### 5. **WCF Service (Sample Contract)**
Make sure your WCF service has the proper contract to handle incoming data.

```csharp
[ServiceContract]
public interface IYourService
{
    [OperationContract]
    void ProcessFormData(FormData data);
}

[DataContract]
public class FormData
{
    [DataMember]
    public string CaseNumber { get; set; }
    // Other fields
}
```

- **Explanation:**
  - The WCF service should have a contract that matches the data structure sent from the Express server.
  - This data can be processed and stored as per your business logic.

### Conclusion
- **Frontend:** Angular manages form state in each tab, with save and submit functionality that communicates with the backend via events.
- **Backend:** Express handles saving and submitting the data, forwarding it to the WCF services API.
- **WCF:** The WCF service processes the submitted data as per the contract defined.

Let me know if you need further clarification or adjustments!