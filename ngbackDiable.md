Disabling the back button in an Angular application can be done using different approaches depending on your requirements. Below are professional, industry-standard solutions:

Approach 1: Intercept Location Change & Replace State

This prevents users from navigating back by modifying the browser’s history state.

Implementation (Using LocationStrategy)

Modify your AppComponent or a service handling navigation:

import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { Router, NavigationStart } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  constructor(private location: Location, private router: Router) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        this.location.replaceState('/'); // Replace history state to disable back
      }
    });
  }
}

🔹 How It Works:

	•	Replaces the browser history entry each time navigation happens.
	•	The back button won’t take the user to the previous page as the history state is modified.

Approach 2: Using CanDeactivate Guard

If you want to prevent users from leaving certain routes, you can implement CanDeactivate.

Step 1: Create a Route Guard

import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';

export interface CanComponentDeactivate {
  canDeactivate: () => Observable<boolean> | Promise<boolean> | boolean;
}

@Injectable({
  providedIn: 'root',
})
export class CanDeactivateGuard implements CanDeactivate<CanComponentDeactivate> {
  canDeactivate(component: CanComponentDeactivate): Observable<boolean> | Promise<boolean> | boolean {
    return component.canDeactivate ? component.canDeactivate() : true;
  }
}

Step 2: Implement in a Component

import { Component } from '@angular/core';
import { CanComponentDeactivate } from '../guards/can-deactivate.guard';

@Component({
  selector: 'app-secure-page',
  templateUrl: './secure-page.component.html',
  styleUrls: ['./secure-page.component.scss'],
})
export class SecurePageComponent implements CanComponentDeactivate {
  canDeactivate(): boolean {
    return confirm('Are you sure you want to leave this page?');
  }
}

Step 3: Apply to Routes

import { Routes } from '@angular/router';
import { SecurePageComponent } from './secure-page/secure-page.component';
import { CanDeactivateGuard } from './guards/can-deactivate.guard';

const routes: Routes = [
  { path: 'secure', component: SecurePageComponent, canDeactivate: [CanDeactivateGuard] },
];

🔹 How It Works:

	•	Before navigating away, the user gets a confirmation dialog.
	•	If they choose Cancel, the navigation is blocked.

Approach 3: Use JavaScript onpopstate

This is a lower-level solution that prevents the back button globally.

Implementation in AppComponent

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  ngOnInit() {
    history.pushState(null, '', location.href);
    window.onpopstate = () => {
      history.pushState(null, '', location.href);
    };
  }
}

🔹 How It Works:

	•	Every time the user tries to go back, pushState re-adds the current URL.
	•	The back button won’t function.

Which Solution to Choose?

Use Case	Best Approach
Prevent back navigation globally	Approach 1 (Replace State)
Prevent leaving a route (with confirmation)	Approach 2 (CanDeactivate Guard)
Completely block back navigation using JS	Approach 3 (onpopstate)

Would you like to customize any approach further?