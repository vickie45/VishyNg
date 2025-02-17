To implement this functionality in Angular, you can break it down into a few key steps:

	1.	Create a Service for App Data: This service will store and retrieve the selected drop-down value from localStorage and manage the application’s state.
	2.	Drop-down Component: In your component, bind the drop-down to the observable and update the local storage whenever the value changes.
	3.	Hit ID Generation API: Based on the selected value in the drop-down, conditionally hit the API for either preauth id or copyres id.
	4.	Save and Submit API: After the ID generation, based on the value stored in localStorage, pass the generated ID to the save and submit APIs.

Here’s a structured approach with sample code snippets:

1. App Data Service (app-data.service.ts)

This service will manage the application’s state in localStorage and provide observables to track changes.

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppDataService {
  private dropDownValueSubject = new BehaviorSubject<string>(localStorage.getItem('dropDownValue') || 'preauth');
  dropDownValue$ = this.dropDownValueSubject.asObservable();

  constructor() {}

  setDropDownValue(value: string) {
    localStorage.setItem('dropDownValue', value);
    this.dropDownValueSubject.next(value);
  }

  getDropDownValue(): string {
    return localStorage.getItem('dropDownValue') || 'preauth';
  }
}

2. Drop-down Component (dropdown.component.ts)

Bind the drop-down to the AppDataService and update the selected value to localStorage.

import { Component } from '@angular/core';
import { AppDataService } from './app-data.service';

@Component({
  selector: 'app-dropdown',
  templateUrl: './dropdown.component.html'
})
export class DropdownComponent {
  dropDownOptions = ['preauth', 'copyres'];

  constructor(private appDataService: AppDataService) {}

  onDropDownChange(value: string) {
    this.appDataService.setDropDownValue(value);
  }
}

In the template:

<select (change)="onDropDownChange($event.target.value)">
  <option *ngFor="let option of dropDownOptions" [value]="option">
    {{ option }}
  </option>
</select>

3. ID Generation API Service (id-generation.service.ts)

Hit the API based on the selected value from the drop-down.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppDataService } from './app-data.service';

@Injectable({
  providedIn: 'root'
})
export class IdGenerationService {
  constructor(private http: HttpClient, private appDataService: AppDataService) {}

  generateId(): Observable<any> {
    const selectedValue = this.appDataService.getDropDownValue();
    let apiEndpoint: string;

    if (selectedValue === 'preauth') {
      apiEndpoint = 'https://api.example.com/preauth-id';
    } else {
      apiEndpoint = 'https://api.example.com/copyres-id';
    }

    return this.http.get(apiEndpoint);
  }
}

4. Save and Submit API Service (save-submit.service.ts)

Submit the generated ID to the save and submit APIs.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppDataService } from './app-data.service';
import { IdGenerationService } from './id-generation.service';

@Injectable({
  providedIn: 'root'
})
export class SaveSubmitService {
  constructor(
    private http: HttpClient,
    private appDataService: AppDataService,
    private idGenerationService: IdGenerationService
  ) {}

  saveAndSubmit(): void {
    this.idGenerationService.generateId().subscribe((response) => {
      const selectedValue = this.appDataService.getDropDownValue();
      let saveApiEndpoint: string;
      let submitApiEndpoint: string;

      if (selectedValue === 'preauth') {
        saveApiEndpoint = 'https://api.example.com/save-preauth';
        submitApiEndpoint = 'https://api.example.com/submit-preauth';
      } else {
        saveApiEndpoint = 'https://api.example.com/save-copyres';
        submitApiEndpoint = 'https://api.example.com/submit-copyres';
      }

      // Now you can call the save and submit APIs
      this.http.post(saveApiEndpoint, { id: response.id }).subscribe();
      this.http.post(submitApiEndpoint, { id: response.id }).subscribe();
    });
  }
}

5. Component for Save and Submit (save-submit.component.ts)

This component will call the saveAndSubmit function when needed.

import { Component } from '@angular/core';
import { SaveSubmitService } from './save-submit.service';

@Component({
  selector: 'app-save-submit',
  templateUrl: './save-submit.component.html'
})
export class SaveSubmitComponent {
  constructor(private saveSubmitService: SaveSubmitService) {}

  onSaveSubmit() {
    this.saveSubmitService.saveAndSubmit();
  }
}

In the template:

<button (click)="onSaveSubmit()">Save and Submit</button>

Final Notes:

	•	The AppDataService keeps track of the drop-down value in the local storage, making it accessible across the app.
	•	The IdGenerationService hits the respective API based on the selected value and returns the generated ID.
	•	The SaveSubmitService uses the generated ID and makes the necessary save and submit API calls based on the selected drop-down value.

This structure ensures that you can dynamically change the ID source based on user selection and correctly submit the necessary ID to different endpoints.