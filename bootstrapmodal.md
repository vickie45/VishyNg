To handle a Bootstrap 5 modal popup in Angular, you can use either:
	•	Manual DOM-based control using Bootstrap’s JavaScript APIs (Modal class)
	•	Or, use third-party libraries like ng-bootstrap (optional)

Below is the manual Bootstrap 5 integration (without external libraries), which gives you full control.

⸻

1. Setup Bootstrap 5 in Angular

Ensure Bootstrap CSS & JS are added in your angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]



⸻

2. Create Modal HTML in Your Component

<!-- modal.component.html -->
<!-- Trigger button -->
<button class="btn btn-primary" (click)="openModal()">Open Modal</button>

<!-- Modal -->
<div class="modal fade" id="myModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">Angular Bootstrap Modal</h5>
        <button type="button" class="btn-close" (click)="closeModal()" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        This is a Bootstrap 5 modal popup.
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" (click)="closeModal()">Close</button>
      </div>
    </div>
  </div>
</div>



⸻

3. Use Bootstrap Modal API in Component Code

// modal.component.ts
import { Component, ElementRef, ViewChild } from '@angular/core';

declare var bootstrap: any; // Bootstrap JS API

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html'
})
export class ModalComponent {
  @ViewChild('myModal', { static: true }) modalRef!: ElementRef;
  private modal: any;

  ngOnInit() {
    this.modal = new bootstrap.Modal(this.modalRef.nativeElement);
  }

  openModal() {
    this.modal.show();
  }

  closeModal() {
    this.modal.hide();
  }
}

To make #myModal accessible in @ViewChild, update HTML:

<div #myModal class="modal fade" id="myModal" ...>



⸻

Alternative: Use ng-bootstrap (Optional)

If you prefer Angular components for modals, you can use ng-bootstrap, but it requires installation.

⸻

Would you like a version using ng-bootstrap as well?