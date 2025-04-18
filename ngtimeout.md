Certainly. Here’s how to add a warning modal (e.g., 1 minute before logout) in your Angular 17 app when the user is idle. This gives the user a chance to stay logged in.

⸻

Final Flow
	1.	Track user activity.
	2.	Start countdown after inactivity.
	3.	At timeout - warningTime, show warning modal.
	4.	If user responds, reset timer.
	5.	If no response, logout.

⸻

1. Update IdleService

We’ll emit warning and logout events.

idle.service.ts

import { Injectable, NgZone, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, merge, fromEvent, timer, Subscription } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class IdleService {
  private idleTimeoutMs = 15 * 60 * 1000; // 15 minutes
  private warningBeforeMs = 1 * 60 * 1000; // 1 minute
  private activityEvents = ['mousemove', 'keydown', 'scroll', 'touchstart'];

  private warningSubject = new Subject<void>();
  private logoutSubject = new Subject<void>();

  warning$ = this.warningSubject.asObservable();
  logout$ = this.logoutSubject.asObservable();

  private subscription: Subscription | null = null;
  private warningTimer: Subscription | null = null;

  private router = inject(Router);
  private ngZone = inject(NgZone);

  startWatching(): void {
    this.ngZone.runOutsideAngular(() => {
      const activity$ = merge(
        ...this.activityEvents.map(e => fromEvent(document, e))
      );

      this.subscription = activity$
        .pipe(
          tap(() => {
            this.cancelTimers();
          }),
          switchMap(() => {
            const timeToWarning = this.idleTimeoutMs - this.warningBeforeMs;
            this.setWarningTimer(timeToWarning);
            return timer(this.idleTimeoutMs);
          })
        )
        .subscribe(() => {
          this.ngZone.run(() => this.logout());
        });
    });
  }

  private setWarningTimer(timeToWarning: number) {
    this.warningTimer = timer(timeToWarning).subscribe(() => {
      this.ngZone.run(() => this.warningSubject.next());
    });
  }

  cancelTimers() {
    this.warningTimer?.unsubscribe();
  }

  stayActive() {
    this.cancelTimers();
  }

  stopWatching(): void {
    this.subscription?.unsubscribe();
    this.cancelTimers();
  }

  logout(): void {
    this.logoutSubject.next();
    localStorage.clear();
    sessionStorage.clear();
    this.router.navigate(['/login']);
  }
}



⸻

2. Create Warning Modal

session-timeout.component.ts

import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-session-timeout',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="overlay">
      <div class="modal">
        <p>You have been inactive. Do you want to stay logged in?</p>
        <button (click)="stay.emit()">Stay Logged In</button>
        <button (click)="logout.emit()">Logout</button>
      </div>
    </div>
  `,
  styles: [`
    .overlay {
      position: fixed; top: 0; left: 0; width: 100%; height: 100%;
      background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center;
    }
    .modal {
      background: white; padding: 20px; border-radius: 8px;
      display: flex; flex-direction: column; gap: 10px;
    }
  `]
})
export class SessionTimeoutComponent {
  @Output() stay = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();
}



⸻

3. Use it in AppComponent

app.component.ts

import { Component } from '@angular/core';
import { IdleService } from './idle.service';
import { SessionTimeoutComponent } from './session-timeout.component';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NgIf, SessionTimeoutComponent],
  template: `
    <router-outlet></router-outlet>
    <app-session-timeout 
      *ngIf="showWarning" 
      (stay)="onStayActive()" 
      (logout)="onLogout()">
    </app-session-timeout>
  `
})
export class AppComponent {
  showWarning = false;

  constructor(private idle: IdleService) {
    this.idle.startWatching();

    this.idle.warning$.subscribe(() => {
      this.showWarning = true;
    });

    this.idle.logout$.subscribe(() => {
      this.showWarning = false;
    });
  }

  onStayActive() {
    this.showWarning = false;
    this.idle.stayActive(); // cancel logout and reset timer
  }

  onLogout() {
    this.idle.logout();
  }
}



⸻

Summary
	•	Idle tracking starts automatically.
	•	After 14 minutes (15 - 1), a modal appears.
	•	If user clicks “Stay”, the timer resets.
	•	If user ignores or clicks “Logout”, session ends.

⸻

Would you like this integrated with JWT token expiration and refresh too?