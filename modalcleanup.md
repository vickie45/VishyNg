In an Angular project, if the screen remains greyed out and non-clickable after closing a Bootstrap modal, you can address this issue by ensuring proper modal cleanup and integration with Angular. Here’s how to fix it:

Fixing Modal Issues in Angular

1. Ensure Proper Modal Initialization and Cleanup

Use the Bootstrap modal lifecycle events (shown.bs.modal and hidden.bs.modal) to handle the modal behavior:

import { Component, AfterViewInit } from '@angular/core';

declare var bootstrap: any; // To access Bootstrap's JavaScript functions

@Component({
  selector: 'app-modal-example',
  templateUrl: './modal-example.component.html',
  styleUrls: ['./modal-example.component.css']
})
export class ModalExampleComponent implements AfterViewInit {
  private modalInstance: any;

  ngAfterViewInit(): void {
    const modalElement = document.getElementById('myModal');
    if (modalElement) {
      this.modalInstance = new bootstrap.Modal(modalElement);

      // Listen to modal hide event
      modalElement.addEventListener('hidden.bs.modal', () => {
        document.body.classList.remove('modal-open'); // Ensure body class is removed
        const backdrop = document.querySelector('.modal-backdrop');
        if (backdrop) {
          backdrop.remove(); // Remove the backdrop manually if necessary
        }
      });
    }
  }

  openModal(): void {
    this.modalInstance.show();
  }

  closeModal(): void {
    this.modalInstance.hide();
  }
}

2. Manually Remove Backdrop

If the modal’s backdrop is not automatically removed, you can manually handle it in the hidden.bs.modal event as shown above.

Alternatively, in your component template, you can ensure the cleanup by binding the hidden.bs.modal event in the modal tag:

<div
  class="modal fade"
  id="myModal"
  tabindex="-1"
  aria-labelledby="exampleModalLabel"
  aria-hidden="true"
  (hidden.bs.modal)="onModalHidden()"
>
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="exampleModalLabel">Modal Title</h5>
        <button
          type="button"
          class="btn-close"
          data-bs-dismiss="modal"
          aria-label="Close"
        ></button>
      </div>
      <div class="modal-body">
        Modal content goes here.
      </div>
    </div>
  </div>
</div>

In your TypeScript file:

onModalHidden(): void {
  const backdrop = document.querySelector('.modal-backdrop');
  if (backdrop) {
    backdrop.remove(); // Remove the backdrop
  }
}

3. Verify Bootstrap Version and Dependencies

Ensure the correct version of Bootstrap is installed and imported. If using Bootstrap 5, you need to import both CSS and JavaScript. In your angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]

Run npm install bootstrap if it’s not already installed.

4. Use Angular’s Bootstrap Integration Libraries (Optional)

If you’re using NG Bootstrap or ngx-bootstrap, they handle modals natively and cleanly integrate with Angular. For example, with NG Bootstrap:

npm install @ng-bootstrap/ng-bootstrap

In your component:

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-modal-example',
  templateUrl: './modal-example.component.html',
  styleUrls: ['./modal-example.component.css'],
})
export class ModalExampleComponent {
  constructor(private modalService: NgbModal) {}

  open(content: any): void {
    this.modalService.open(content, { backdrop: 'static', size: 'lg' });
  }
}

And in the template:

<button class="btn btn-primary" (click)="open(myModal)">Open Modal</button>

<ng-template #myModal let-modal>
  <div class="modal-header">
    <h4 class="modal-title">Modal Title</h4>
    <button type="button" class="btn-close" aria-label="Close" (click)="modal.dismiss('Cross click')"></button>
  </div>
  <div class="modal-body">
    Modal content goes here.
  </div>
  <div class="modal-footer">
    <button type="button" class="btn btn-secondary" (click)="modal.close('Save click')">Close</button>
  </div>
</ng-template>

Summary

	1.	Use hidden.bs.modal events to clean up any leftover .modal-backdrop.
	2.	Ensure you have included Bootstrap’s CSS and JavaScript correctly.
	3.	Consider using Angular-specific libraries like NG Bootstrap for better integration and modal management.

Let me know if you’d like further help debugging this!