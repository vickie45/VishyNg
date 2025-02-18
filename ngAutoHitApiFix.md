The issue you’re describing occurs because the subscription to the observable in the save and submit methods is being triggered immediately upon page load, instead of being delayed until the buttons are clicked. To fix this, we need to ensure that the API calls are only triggered when the user explicitly interacts with the UI (via the button clicks).

To fix this, we can take the following steps:

1. Use BehaviorSubject in AppDataService to store the state and trigger the API call only on button click.

The observable in the AppDataService should be subscribed to only when the button is clicked. This way, it doesn’t automatically trigger on page load.

2. Avoid auto-triggering API calls on initialization.

Ensure that API calls (for both save and submit actions) are not being triggered directly upon component load. We need to subscribe to the observable only inside the respective button click handlers.

Solution:

Here’s how you can modify the code to prevent the issue of automatically hitting the API on page load:

1. AppDataService (No changes here)

Ensure that the BehaviorSubject stores the selected value correctly.

2. SaveSubmitService - Fixing Immediate API Calls

Ensure that the API call for save and submit only happens on button clicks. You can achieve this by subscribing to the observable inside the button click method, not at component initialization.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppDataService } from './app-data.service';
import { IdGenerationService } from './id-generation.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SaveSubmitService {
  constructor(
    private http: HttpClient,
    private appDataService: AppDataService,
    private idGenerationService: IdGenerationService
  ) {}

  // This method is responsible for calling the save and submit API on button click
  saveAndSubmit(): Observable<any> {
    return new Observable((observer) => {
      this.idGenerationService.generateId().subscribe((response) => {
        const selectedValue = this.appDataService.getDropDownValue();
        let saveApiEndpoint: string;
        let submitApiEndpoint: string;

        // Define API endpoints based on selected drop-down value
        if (selectedValue === 'preauth') {
          saveApiEndpoint = 'https://api.example.com/save-preauth';
          submitApiEndpoint = 'https://api.example.com/submit-preauth';
        } else {
          saveApiEndpoint = 'https://api.example.com/save-copyres';
          submitApiEndpoint = 'https://api.example.com/submit-copyres';
        }

        // Make the save API call
        this.http.post(saveApiEndpoint, { id: response.id }).subscribe(
          (saveResponse) => {
            // Make the submit API call after save
            this.http.post(submitApiEndpoint, { id: response.id }).subscribe(
              (submitResponse) => {
                observer.next(submitResponse);
                observer.complete();
              },
              (submitError) => observer.error(submitError)
            );
          },
          (saveError) => observer.error(saveError)
        );
      });
    });
  }
}

3. Component for Save and Submit (save-submit.component.ts)

Ensure that the API is triggered only on button click and not on page load.

import { Component } from '@angular/core';
import { SaveSubmitService } from './save-submit.service';

@Component({
  selector: 'app-save-submit',
  templateUrl: './save-submit.component.html'
})
export class SaveSubmitComponent {
  constructor(private saveSubmitService: SaveSubmitService) {}

  // Trigger save and submit logic only when the button is clicked
  onSaveSubmit() {
    this.saveSubmitService.saveAndSubmit().subscribe(
      (response) => {
        console.log('Save and Submit successful', response);
      },
      (error) => {
        console.error('Error in Save and Submit', error);
      }
    );
  }
}

4. Template for Save and Submit Button (save-submit.component.html)

Ensure the button triggers the method on click.

<button (click)="onSaveSubmit()">Save and Submit</button>

Key Fixes:

	1.	Observable Subscription on Click: The subscription to the observable (idGenerationService.generateId()) now happens only inside the onSaveSubmit() method, which is bound to the button click event. This ensures the API calls are triggered only when the user clicks the button and not on page load.
	2.	Avoiding Unintended API Calls on Load: Since the observable is only subscribed to in response to user interaction (button click), the save and submit APIs will not be hit on page load.

Additional Notes:

	•	If you’re using BehaviorSubject to store the drop-down value, the observable can still emit new values, but you should ensure that the saveAndSubmit() method only reacts to user input and not automatically on initialization.
	•	The use of new Observable() in saveAndSubmit() ensures that the flow of operations, such as making save and submit API calls sequentially, happens after the user triggers it.

With these changes, the save and submit logic will only execute when the user clicks the buttons, as intended.