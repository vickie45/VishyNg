Excellent. Let’s create a reusable directive to disable the back button on any component by just applying it to the component’s HTML. This keeps your code clean, modular, and easy to apply.

⸻

Step-by-Step: Reusable Back Button Blocker Directive

1. Create the Directive

Run this command or create a new file manually:

ng generate directive directives/block-back-button

Then edit block-back-button.directive.ts:

import { Directive, HostListener, OnInit } from '@angular/core';

@Directive({
  selector: '[appBlockBackButton]'
})
export class BlockBackButtonDirective implements OnInit {

  ngOnInit(): void {
    history.pushState(null, '', location.href);
  }

  @HostListener('window:popstate', ['$event'])
  onPopState(event: any) {
    history.pushState(null, '', location.href); // Pushes state again, preventing back
  }
}



⸻

2. Register the Directive

Ensure it’s added to your module:

@NgModule({
  declarations: [
    BlockBackButtonDirective
    // ... other components
  ],
  exports: [
    BlockBackButtonDirective
  ]
})
export class SharedModule {} // Or AppModule if no SharedModule



⸻

3. Use It in Any Component

In your form or dashboard page component HTML:

<div appBlockBackButton>
  <!-- Your form or dashboard content -->
</div>



⸻

Optional: Show Confirm Before Exit (canDeactivate Guard)

You can still combine this directive with a canDeactivate guard to confirm when the user tries to navigate away.

Let me know if you’d like the directive to include a confirmation dialog as well (e.g. “Do you really want to go back?”)—we can extend it easily.

⸻

This setup is declarative, clean, and highly reusable. Would you like me to turn this into a full demo project with Angular routing and example form page too?