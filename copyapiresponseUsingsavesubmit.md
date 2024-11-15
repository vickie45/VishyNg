It sounds like you want to determine which ID to use when making the save and submit API requests in the casedetails component, based on the presence of a copyapiresponse object in the appstate$ of the appdata service. Here’s a potential approach:

	1.	Check if copyapiresponse exists:
	•	If copyapiresponse exists in appstate$, use the id values from the copyapiresponse object for the API request.
	2.	Fallback to preauthinward ID:
	•	If copyapiresponse is absent, use the id from preauthinward in appstate$.

Here’s how you could structure this in the casedetails component:

import { AppdataService } from './appdata.service';

@Component({
  // component metadata
})
export class CasedetailsComponent {

  constructor(private appdataService: AppdataService) {}

  saveAndSubmit() {
    // Subscribe to appstate$ to get the current state
    this.appdataService.appstate$.subscribe(appState => {
      let id;

      // Check if copyapiresponse exists and has an ID
      if (appState.copyapiresponse && appState.copyapiresponse.id) {
        id = appState.copyapiresponse.id;
      } else if (appState.preauthinward && appState.preauthinward.id) {
        // Fallback to preauthinward ID
        id = appState.preauthinward.id;
      }

      // Now that we have the ID, call save and submit API
      this.callSaveAndSubmitApi(id);
    });
  }

  private callSaveAndSubmitApi(id: string) {
    // API call logic here
    console.log('Calling save and submit API with ID:', id);
    // this.apiService.saveAndSubmit(id).subscribe(...);
  }
}

In this code:

	•	We subscribe to appstate$ to access appState.
	•	If copyapiresponse is present, we use its id; otherwise, we fall back to the preauthinward ID.
	•	The callSaveAndSubmitApi method is then used to make the API call with the selected id.