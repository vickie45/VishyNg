🚀 Guide: Integrating Phi-2 in VS Code and an Angular Application

This guide will help you run Phi-2 locally, integrate it into VS Code, and use it in an Angular app for AI-assisted coding.

1️⃣ Install & Run Phi-2 Locally

Option 1: Using Ollama (Recommended)

	1.	Install Ollama (if not already installed):

curl -fsSL https://ollama.com/install.sh | sh

For Windows, download and install from Ollama’s website.

	2.	Download & Run Phi-2 Model:

ollama pull phi2
ollama run phi2


	3.	Test the Model via API:
Open another terminal and run:

curl -X POST http://localhost:11434/api/generate -d '{
  "model": "phi2",
  "messages": [{"role": "user", "content": "Write a Python function for Fibonacci"}]
}'

It should return AI-generated code.

2️⃣ Integrating Phi-2 in VS Code

You can integrate Phi-2 into VS Code for code suggestions & chat-based AI assistance.

A. Using the REST API in VS Code

	1.	Install the REST Client Extension in VS Code
	•	Open VS Code → Extensions (Ctrl + Shift + X)
	•	Search for “REST Client” → Install it
	2.	Create a Request File (phi2-api.http)
Inside your project, create a file phi2-api.http and add:

POST http://localhost:11434/api/generate
Content-Type: application/json

{
  "model": "phi2",
  "messages": [{"role": "user", "content": "Suggest a C# function for sorting an array"}]
}

	3.	Run the API Request
	•	Click the “Send Request” option that appears in VS Code.
	•	You should receive AI-generated code.

B. Automating Code Completion in VS Code

You can create a VS Code extension or Python script that sends the current code to Phi-2 and gets AI-assisted suggestions.

3️⃣ Using Phi-2 in an Angular Application

Now, let’s integrate Phi-2 into an Angular 17 app to provide AI-generated code suggestions.

A. Create an Angular App (If Not Already Created)

ng new phi2-angular-app --standalone
cd phi2-angular-app

B. Install Required Dependencies

npm install axios

C. Create a Service to Call Phi-2 API

Create a service src/app/services/phi2.service.ts:

import { Injectable } from '@angular/core';
import axios from 'axios';

@Injectable({
  providedIn: 'root'
})
export class Phi2Service {
  private apiUrl = 'http://localhost:11434/api/generate';

  async getAIResponse(prompt: string): Promise<string> {
    try {
      const response = await axios.post(this.apiUrl, {
        model: 'phi2',
        messages: [{ role: 'user', content: prompt }]
      });

      return response.data.response || 'No response from AI';
    } catch (error) {
      console.error('Error fetching AI response:', error);
      return 'Error communicating with AI';
    }
  }
}

D. Create a Component to Display AI Suggestions

Run:

ng generate component chat

Then, modify src/app/chat/chat.component.ts:

import { Component } from '@angular/core';
import { Phi2Service } from '../services/phi2.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent {
  userInput: string = '';
  aiResponse: string = '';

  constructor(private phi2Service: Phi2Service) {}

  async sendPrompt() {
    this.aiResponse = 'Thinking...';
    this.aiResponse = await this.phi2Service.getAIResponse(this.userInput);
  }
}

E. Update the Chat UI (HTML)

Modify src/app/chat/chat.component.html:

<div class="chat-container">
  <h2>AI Code Assistant</h2>
  <textarea [(ngModel)]="userInput" placeholder="Ask the AI..."></textarea>
  <button (click)="sendPrompt()">Ask AI</button>
  <pre>{{ aiResponse }}</pre>
</div>

F. Add Styling (CSS)

Modify src/app/chat/chat.component.css:

.chat-container {
  max-width: 600px;
  margin: auto;
  padding: 20px;
  border: 1px solid #ccc;
  border-radius: 10px;
  background: #f8f9fa;
}

textarea {
  width: 100%;
  height: 80px;
  margin-bottom: 10px;
}

button {
  width: 100%;
  padding: 10px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

G. Display the Chat Component

Modify src/app/app.component.html:

<app-chat></app-chat>

🚀 4️⃣ Run the Angular App & Test

	1.	Start the Phi-2 model (if not already running):

ollama run phi2


	2.	Start the Angular app:

ng serve --open


	3.	Now, ask coding questions in the chat UI, and Phi-2 will respond with AI-generated suggestions.

✅ Summary

	•	Phi-2 runs locally using Ollama (or LM Studio/GPT4All).
	•	VS Code integration allows AI-powered coding via API calls.
	•	Angular integration provides a UI for AI-assisted programming.

Would you like me to help extend this to generate full project templates dynamically? 🚀