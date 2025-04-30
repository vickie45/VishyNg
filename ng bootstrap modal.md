Here’s how to handle a Bootstrap 5 modal popup in Angular using ng-bootstrap, which is an Angular-native Bootstrap implementation—no manual DOM control needed.

⸻

1. Install ng-bootstrap

ng add @ng-bootstrap/ng-bootstrap

This will automatically update angular.json and install dependencies.

⸻

2. Import NgbModule in Your App Module

// app.module.ts
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { ModalComponent } from './modal/modal.component';

@NgModule({
  declarations: [AppComponent, ModalComponent],
  imports: [BrowserModule, NgbModule],
  bootstrap: [AppComponent]
})
export class AppModule {}



⸻

3. Create the Modal Component

a. modal-content.component.ts

This is the content shown inside the modal.

// modal-content.component.ts
import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-modal-content',
  template: `
    <div class="modal-header">
      <h5 class="modal-title">Hello from Modal</h5>
      <button type="button" class="btn-close" aria-label="Close" (click)="activeModal.dismiss()"></button>
    </div>
    <div class="modal-body">
      <p>This is the modal content.</p>
    </div>
    <div class="modal-footer">
      <button type="button" class="btn btn-outline-dark" (click)="activeModal.close('Close click')">Close</button>
    </div>
  `
})
export class ModalContentComponent {
  constructor(public activeModal: NgbActiveModal) {}
}

b. modal-launcher.component.ts

This is the component with a button to open the modal.

// modal-launcher.component.ts
import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalContentComponent } from './modal-content.component';

@Component({
  selector: 'app-modal-launcher',
  template: `
    <button class="btn btn-primary" (click)="open()">Open Modal</button>
  `
})
export class ModalLauncherComponent {
  constructor(private modalService: NgbModal) {}

  open() {
    this.modalService.open(ModalContentComponent, {
      centered: true,
      backdrop: 'static'
    });
  }
}



⸻

4. Register Both Components in AppModule

Make sure to add ModalContentComponent as an entry component (only needed for older versions before Angular 13):

@NgModule({
  declarations: [
    AppComponent,
    ModalLauncherComponent,
    ModalContentComponent
  ],
  imports: [BrowserModule, NgbModule],
  bootstrap: [AppComponent]
})
export class AppModule {}



⸻

Summary:

Method	Modal Control	Bootstrap JS Needed?	Angular-native?
Manual (Bootstrap API)	Manual DOM control via @ViewChild	Yes	No
ng-bootstrap	Declarative and TypeScript-friendly	No	Yes

Would you like this in a working StackBlitz template or zip file format?