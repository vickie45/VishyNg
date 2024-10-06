Here’s an implementation using Angular 17 with Bootstrap 5 for a search button that triggers a modal with a list of diagnosis codes, where selected codes are displayed in a table with delete options:

### 1. **HTML Structure (Template)**
```html
<div class="container">
  <!-- Search Button -->
  <button class="btn btn-primary mt-3" (click)="openModal()">Search Diagnosis Codes</button>

  <!-- Selected Diagnosis Codes Table -->
  <table class="table table-bordered mt-3" *ngIf="selectedDiagnosisCodes.length > 0">
    <thead>
      <tr>
        <th>Diagnosis Code</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let code of selectedDiagnosisCodes; let i = index">
        <td>{{ code }}</td>
        <td>
          <button class="btn btn-danger" (click)="removeCode(i)">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>

  <!-- Modal -->
  <div class="modal fade" id="diagnosisModal" tabindex="-1" aria-labelledby="diagnosisModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="diagnosisModalLabel">Select Diagnosis Codes</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <div *ngFor="let code of diagnosisCodes">
            <div class="form-check">
              <input class="form-check-input" type="checkbox" [id]="code" [(ngModel)]="selectedCodes[code]">
              <label class="form-check-label" [for]="code">{{ code }}</label>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
          <button type="button" class="btn btn-primary" (click)="saveSelectedCodes()">Save</button>
        </div>
      </div>
    </div>
  </div>
</div>
```

### 2. **TypeScript (Component)**
```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-diagnosis-selector',
  templateUrl: './diagnosis-selector.component.html',
})
export class DiagnosisSelectorComponent {
  diagnosisCodes: string[] = ['Code A', 'Code B', 'Code C', 'Code D'];
  selectedCodes: { [key: string]: boolean } = {};
  selectedDiagnosisCodes: string[] = [];

  openModal() {
    const modal = new bootstrap.Modal(document.getElementById('diagnosisModal')!);
    modal.show();
  }

  saveSelectedCodes() {
    this.selectedDiagnosisCodes = Object.keys(this.selectedCodes).filter(code => this.selectedCodes[code]);
    const modal = bootstrap.Modal.getInstance(document.getElementById('diagnosisModal')!);
    modal?.hide();
  }

  removeCode(index: number) {
    this.selectedDiagnosisCodes.splice(index, 1);
  }
}
```

### 3. **Styles (CSS)**
You don’t need specific styles for this example as Bootstrap provides sufficient styling. However, if you want to customize the appearance, you can add styles here:

```css
.table-bordered {
  border: 1px solid #dee2e6;
}

.table-bordered th,
.table-bordered td {
  border: 1px solid #dee2e6;
}
```

### 4. **Explanation**
- **Search Button**: Triggers the opening of a Bootstrap modal using the `openModal()` method.
- **Modal**: Contains a list of checkboxes for selecting diagnosis codes.
- **Save Button**: When clicked, the selected diagnosis codes are saved and displayed in a table.
- **Table**: Displays the selected diagnosis codes with a delete button next to each code.
- **Delete Button**: Removes the diagnosis code from the list when clicked.

### 5. **Bootstrap Modal Setup**
Make sure to include Bootstrap 5 in your Angular project. You can install it via `npm`:

```bash
npm install bootstrap
```

Add the Bootstrap CSS in your `angular.json`:

```json
"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]
```

This setup should give you the functionality you're looking for with Angular 17 and Bootstrap 5.