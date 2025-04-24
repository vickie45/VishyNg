To create a fully reusable, pluggable Bootstrap 5 modal in Angular 17—with customizable buttons, navigation, and functionality—you can follow this clean and flexible design:

⸻

Goal:
	•	Reusability across components.
	•	Plugable buttons (label + action).
	•	Dynamic title, message/body.
	•	Optional router navigation on button click.

⸻

1. Reusable Modal Component

modal.component.ts

import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

declare var bootstrap: any;

export interface ModalButton {
  label: string;
  class?: string; // Bootstrap button class
  action?: () => void;
  navigateTo?: string; // optional router navigation
  dismiss?: boolean; // whether to auto close modal
}

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
})
export class ModalComponent {
  @Input() modalId = 'appModal';
  @Input() title: string = 'Alert';
  @Input() body: string = '';
  @Input() buttons: ModalButton[] = [];

  modalInstance: any;

  constructor(private router: Router) {}

  open() {
    const modalElement = document.getElementById(this.modalId);
    if (modalElement) {
      this.modalInstance = new bootstrap.Modal(modalElement);
      this.modalInstance.show();
    }
  }

  close() {
    if (this.modalInstance) {
      this.modalInstance.hide();
    }
  }

  onButtonClick(btn: ModalButton) {
    if (btn.action) btn.action();
    if (btn.navigateTo) this.router.navigate([btn.navigateTo]);
    if (btn.dismiss !== false) this.close(); // dismiss unless explicitly set false
  }
}



⸻

modal.component.html

<div class="modal fade" id="{{ modalId }}" tabindex="-1">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">

      <div class="modal-header">
        <h5 class="modal-title">{{ title }}</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" (click)="close()"></button>
      </div>

      <div class="modal-body">
        <p>{{ body }}</p>
      </div>

      <div class="modal-footer">
        <button
          *ngFor="let btn of buttons"
          [ngClass]="btn.class || 'btn btn-primary'"
          (click)="onButtonClick(btn)">
          {{ btn.label }}
        </button>
      </div>

    </div>
  </div>
</div>



⸻

2. Example Usage in a Host Component

example.component.ts

import { Component, ViewChild } from '@angular/core';
import { ModalComponent, ModalButton } from '../shared/modal/modal.component';

@Component({
  selector: 'app-example',
  templateUrl: './example.component.html',
})
export class ExampleComponent {
  @ViewChild('myModal') myModal!: ModalComponent;

  openCustomModal() {
    this.myModal.title = 'Delete Confirmation';
    this.myModal.body = 'Are you sure you want to delete this item?';

    this.myModal.buttons = [
      {
        label: 'Cancel',
        class: 'btn btn-secondary',
        dismiss: true
      },
      {
        label: 'Delete',
        class: 'btn btn-danger',
        action: () => {
          console.log('Deleted!');
        },
      },
      {
        label: 'Go to Dashboard',
        class: 'btn btn-success',
        navigateTo: '/dashboard'
      }
    ];

    this.myModal.open();
  }
}



⸻

example.component.html

<app-modal #myModal></app-modal>

<button class="btn btn-warning" (click)="openCustomModal()">Launch Custom Modal</button>



⸻

3. Optional: Global Modal Service (Advanced)

If you want to open modals from services or anywhere, I can help you with a modal service pattern as well.

Would you like that included?