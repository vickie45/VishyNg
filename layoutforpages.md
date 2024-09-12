To ensure the login component does not have a header while all other pages do, you can manage the layout using Angular's `ngIf` directive based on the current route. Here's how you can achieve this:

### Steps:

1. **Create a Layout Component with the Header:**
   - Create a `LayoutComponent` that includes your header and footer. This component will wrap all other components except the login component.

2. **Use Angular Router to Check the Route:**
   - You can check the current route in the `app.component.ts` to determine whether to display the header or not.

### Example Code:

#### `app.component.html`
```html
<div *ngIf="showHeader">
  <app-header></app-header>
</div>

<router-outlet></router-outlet>
```

#### `app.component.ts`
```typescript
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {
  showHeader = true;

  constructor(private router: Router) {
    // Listen to route changes to toggle header visibility
    this.router.events.subscribe(() => {
      this.showHeader = this.router.url !== '/login'; // Hide header for login route
    });
  }
}
```

#### `app-routing.module.ts`
```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { LayoutComponent } from './layout/layout.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', component: LayoutComponent, children: [
    { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
    { path: 'profile', loadChildren: () => import('./profile/profile.module').then(m => m.ProfileModule) }
  ] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
```

#### `layout.component.html`
```html
<app-header></app-header>
<router-outlet></router-outlet>
<app-footer></app-footer>
```

### Explanation:
- **`app.component.ts`**: The `showHeader` flag controls whether the header is visible based on the current route. If the user is on the login page (`/login`), the header is hidden.
- **`app-routing.module.ts`**: Routes are configured so that the login page doesn't use the `LayoutComponent`. Other routes are wrapped inside the `LayoutComponent`, which includes the header and footer.

With this setup, the login page will not show the header, while all other pages will.