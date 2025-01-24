Here’s how you can write an Angular solution directly in your application to remove any leftover modal-backdrop elements programmatically using Angular:

Angular Code (in app.component.ts)

import { Component, OnInit, Renderer2 } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private renderer: Renderer2) {}

  ngOnInit(): void {
    // Function to clean up leftover modal-backdrop elements
    const cleanupModalBackdrops = () => {
      const backdrops = document.querySelectorAll('.modal-backdrop');
      backdrops.forEach((backdrop) => {
        this.renderer.removeChild(document.body, backdrop);
      });

      // Reset body styles
      this.renderer.removeClass(document.body, 'modal-open');
      this.renderer.setStyle(document.body, 'padding-right', '0');
    };

    // Listen to Bootstrap's modal hidden event
    this.renderer.listen('document', 'hidden.bs.modal', () => {
      cleanupModalBackdrops();
    });

    // Fallback cleanup mechanism for any leftover backdrops
    setInterval(() => {
      cleanupModalBackdrops();
    }, 500);
  }
}

Explanation:

	1.	Renderer2 for DOM Manipulation:
	•	Renderer2 is used for safely manipulating the DOM in an Angular-friendly way, avoiding direct DOM access wherever possible.
	2.	Event Listener for hidden.bs.modal:
	•	The renderer.listen method listens for Bootstrap’s hidden.bs.modal event to trigger the cleanup process whenever a modal is hidden.
	3.	Cleanup Function:
	•	Removes .modal-backdrop elements and resets modal-open classes and padding from the body.
	4.	Fallback Mechanism:
	•	A setInterval periodically checks and removes leftover backdrops in case they persist due to timing issues or event failures.

Add Bootstrap to Your Angular Project

Ensure you have Bootstrap installed and set up in your Angular project:

	1.	Install Bootstrap:

npm install bootstrap


	2.	Add the CSS in angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]



Benefits:

	•	Fully Angular-friendly using Renderer2.
	•	Tied to Angular’s lifecycle (ngOnInit), ensuring proper initialization within the app.
	•	Compatible with Bootstrap 5’s modal behavior.

Let me know if you need further clarifications!