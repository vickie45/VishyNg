To highlight an active tab in an Angular application, you can use property binding with ngClass or ngStyle, or use Angular’s router to detect active routes if you’re using routing. Here’s a clean and practical implementation using ngClass.

⸻

1. CSS (for Highlighting Active Tab)

.tab {
  padding: 10px 20px;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  color: #555;
  transition: 0.3s;
}

.tab:hover {
  color: #000;
}

.active-tab {
  border-bottom: 2px solid #007bff; /* Bootstrap blue */
  color: #007bff;
  font-weight: bold;
}



⸻

2. Angular Component Code

HTML (tab UI)

<div class="tab-container">
  <div
    class="tab"
    [ngClass]="{ 'active-tab': activeTab === 'home' }"
    (click)="setActiveTab('home')"
  >
    Home
  </div>
  <div
    class="tab"
    [ngClass]="{ 'active-tab': activeTab === 'profile' }"
    (click)="setActiveTab('profile')"
  >
    Profile
  </div>
  <div
    class="tab"
    [ngClass]="{ 'active-tab': activeTab === 'settings' }"
    (click)="setActiveTab('settings')"
  >
    Settings
  </div>
</div>

<!-- You can conditionally show tab content -->
<div class="tab-content">
  <div *ngIf="activeTab === 'home'">Welcome to Home</div>
  <div *ngIf="activeTab === 'profile'">User Profile Info</div>
  <div *ngIf="activeTab === 'settings'">Change your settings here</div>
</div>



⸻

TypeScript (component logic)

import { Component } from '@angular/core';

@Component({
  selector: 'app-tab-example',
  templateUrl: './tab-example.component.html',
  styleUrls: ['./tab-example.component.css']
})
export class TabExampleComponent {
  activeTab: string = 'home';

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}



⸻

This structure gives you a clean way to handle tab state manually. If you’re using Angular routing and want to highlight the tab based on the current route, let me know—I can share that version too.