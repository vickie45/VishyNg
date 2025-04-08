To implement a search functionality with results shown in a popover, we’ll use Angular and Bootstrap 5. The user types into a search input, and matching results appear in a popover-style dropdown. The user can then select an item from the list.

Here’s a complete example:

⸻

Step 1: Setup

Make sure you have Bootstrap 5 integrated into your Angular project. If not, add it in angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]



⸻

Step 2: Component HTML

<div class="position-relative">
  <input
    type="text"
    class="form-control"
    [(ngModel)]="searchText"
    (input)="onSearch()"
    placeholder="Search..."
    (focus)="showPopover = true"
    (blur)="onBlur()"
  />

  <div
    class="popover bs-popover-bottom show"
    role="tooltip"
    *ngIf="showPopover && filteredItems.length > 0"
    style="position: absolute; top: 100%; left: 0; z-index: 1000; width: 100%;"
  >
    <div class="popover-body p-0">
      <ul class="list-group list-group-flush m-0">
        <li
          *ngFor="let item of filteredItems"
          class="list-group-item list-group-item-action"
          (mousedown)="selectItem(item)"
        >
          {{ item }}
        </li>
      </ul>
    </div>
  </div>
</div>



⸻

Step 3: Component TypeScript

import { Component } from '@angular/core';

@Component({
  selector: 'app-search-popover',
  templateUrl: './search-popover.component.html',
})
export class SearchPopoverComponent {
  searchText = '';
  showPopover = false;

  items = ['Apple', 'Banana', 'Mango', 'Orange', 'Grapes'];
  filteredItems: string[] = [];

  onSearch() {
    const term = this.searchText.toLowerCase();
    this.filteredItems = this.items.filter(item =>
      item.toLowerCase().includes(term)
    );
  }

  selectItem(item: string) {
    this.searchText = item;
    this.showPopover = false;
  }

  onBlur() {
    setTimeout(() => {
      this.showPopover = false;
    }, 200); // Delay to allow click event to register
  }
}



⸻

Step 4: Add FormsModule

In your app.module.ts, import FormsModule if not already added:

import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    FormsModule,
    // other modules
  ]
})
export class AppModule {}



⸻

Optional Improvements
	•	Add keyboard navigation (arrow keys + Enter).
	•	Show “No results found” if the array is empty.
	•	Make it reusable as a directive or component with @Input() and @Output() bindings.

⸻

Would you like a reusable Angular component version of this with proper input/output support and styling for production use?