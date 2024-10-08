Here’s a complete example of an Angular component that displays a modal when a submit button is clicked. This includes both the HTML template and the TypeScript code for the component.

### Step 1: Install Bootstrap
Make sure you have Bootstrap installed in your Angular project:

```bash
npm install bootstrap
```

Add Bootstrap to your `angular.json`:

```json
"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]
```

### Step 2: Create the Angular Component

1. **Generate a new component (if you haven’t already)**:
   ```bash
   ng generate component my-form
   ```

2. **Update the component's HTML template (`my-form.component.html`)**:

```html
<div class="container mt-5">
  <form (ngSubmit)="onSubmit()">
    <div class="mb-3">
      <label for="name" class="form-label">Name</label>
      <input type="text" id="name" class="form-control" required>
    </div>
    <div class="mb-3">
      <label for="email" class="form-label">Email</label>
      <input type="email" id="email" class="form-control" required>
    </div>
    <button type="submit" class="btn btn-primary">Submit</button>
  </form>
</div>

<!-- Modal -->
<div class="modal fade" id="submitModal" tabindex="-1" aria-labelledby="submitModalLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="submitModalLabel">Submission Successful</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        Your submission has been received successfully.
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>
```

3. **Update the component's TypeScript file (`my-form.component.ts`)**:

```typescript
import { Component } from '@angular/core';
import * as bootstrap from 'bootstrap';

@Component({
  selector: 'app-my-form',
  templateUrl: './my-form.component.html',
  styleUrls: ['./my-form.component.css']
})
export class MyFormComponent {
  onSubmit() {
    // Show the modal after submission
    const modalElement = document.getElementById('submitModal');
    const modal = new bootstrap.Modal(modalElement!);
    modal.show();
  }
}
```

### Step 3: Update Your App Component

If you want to use the form component in your main application, ensure you include it in your `app.component.html`:

```html
<app-my-form></app-my-form>
```

### Step 4: Run Your Application

Run your Angular application to see the modal in action:

```bash
ng serve
```

### Step 5: Test the Modal

1. Fill in the form fields (Name and Email).
2. Click the "Submit" button.
3. The modal should appear displaying the message "Your submission has been received successfully."

### Additional Notes

- You can add more form fields as needed.
- Customize the modal content according to your requirements.
- Make sure Bootstrap's JavaScript functionality is correctly integrated to allow the modal to function.

Let me know if you need any further modifications or additional features!