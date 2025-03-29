✅ Building a UI Builder Application in Angular + Express.js

⸻

💡 Tech Stack
	•	Frontend: Angular 17 + Bootstrap 5 (for modern UI)
	•	Backend: Express.js (Node.js server)
	•	Database: MongoDB (for saving UI templates)
	•	Deployment: IIS / NGINX or Docker
	•	Authentication: JWT token-based authentication
	•	Code Generation: Dynamic HTML, CSS, and Angular component generation

⸻

🚀 1. Project Structure

Here’s the architecture for the UI Builder application:

/UI-Builder  
 ├── backend              → Express.js server (Node.js)  
 │     ├── models         → MongoDB schemas  
 │     ├── routes         → REST API endpoints  
 │     ├── services       → Business logic  
 │     ├── utils          → Helper functions  
 │     └── server.js      → Main Express server  
 │  
 ├── frontend             → Angular 17  
 │     ├── src            
 │         ├── app        
 │         │     ├── components → UI builder components  
 │         │     ├── services   → API services  
 │         │     ├── models     → Angular models  
 │         │     ├── styles     → Global styles  
 │         │     ├── templates  → Generated templates  
 │         └── index.html  
 │  
 ├── Dockerfile           → For containerization  
 ├── package.json         → Dependencies  
 ├── README.md            → Documentation  



⸻

🔥 2. Setting Up the Backend (Express.js)

✅ (a) Install Dependencies

Create the backend folder and initialize the project.

# Create backend folder
mkdir backend
cd backend

# Initialize Node.js
npm init -y

# Install dependencies
npm install express mongoose cors body-parser jsonwebtoken dotenv
npm install nodemon --save-dev

✅ Packages Explained:
	•	express: Backend framework
	•	mongoose: MongoDB ORM
	•	cors: Cross-origin resource sharing
	•	jsonwebtoken: JWT authentication
	•	dotenv: Environment variables
	•	nodemon: Auto-restart the server during development

⸻

✅ (b) Create server.js

const express = require("express");
const mongoose = require("mongoose");
const cors = require("cors");
const bodyParser = require("body-parser");
const dotenv = require("dotenv");

dotenv.config();
const app = express();

// Middleware
app.use(cors());
app.use(bodyParser.json());

// MongoDB connection
mongoose.connect(process.env.MONGO_URI, { useNewUrlParser: true, useUnifiedTopology: true })
    .then(() => console.log("MongoDB connected"))
    .catch((err) => console.error(err));

// Routes
const componentRoutes = require("./routes/components");
app.use("/api/components", componentRoutes);

// Start server
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));



⸻

✅ (c) MongoDB Model (UI Component Schema)

Create models/Component.js:

const mongoose = require("mongoose");

const componentSchema = new mongoose.Schema({
    name: String,
    html: String,
    css: String,
    ts: String,
    createdAt: { type: Date, default: Date.now }
});

module.exports = mongoose.model("Component", componentSchema);



⸻

✅ (d) API Routes

Create routes/components.js:

const express = require("express");
const router = express.Router();
const Component = require("../models/Component");

// Create new component
router.post("/", async (req, res) => {
    const { name, html, css, ts } = req.body;
    const newComponent = new Component({ name, html, css, ts });

    try {
        await newComponent.save();
        res.status(201).send("Component saved successfully");
    } catch (error) {
        res.status(400).send(error.message);
    }
});

// Get all components
router.get("/", async (req, res) => {
    try {
        const components = await Component.find();
        res.json(components);
    } catch (error) {
        res.status(500).send(error.message);
    }
});

// Get component by ID
router.get("/:id", async (req, res) => {
    try {
        const component = await Component.findById(req.params.id);
        res.json(component);
    } catch (error) {
        res.status(500).send(error.message);
    }
});

// Delete component
router.delete("/:id", async (req, res) => {
    try {
        await Component.findByIdAndDelete(req.params.id);
        res.send("Component deleted successfully");
    } catch (error) {
        res.status(500).send(error.message);
    }
});

module.exports = router;



⸻

🔥 3. Setting Up the Frontend (Angular 17)

✅ (a) Create Angular App

# Go back to the root folder
cd ..

# Create Angular project
ng new frontend
cd frontend

# Install Bootstrap
npm install bootstrap

✅ Add Bootstrap to angular.json:

"styles": [
    "src/styles.css",
    "./node_modules/bootstrap/dist/css/bootstrap.min.css"
]



⸻

✅ (b) Generate Components and Services

ng generate component builder  
ng generate component preview  
ng generate service api  



⸻

✅ (c) Angular Service for Backend API

Create src/app/services/api.service.ts:

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private apiUrl = 'http://localhost:3000/api/components';

  constructor(private http: HttpClient) {}

  getComponents(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  createComponent(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  deleteComponent(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}



⸻

✅ (d) UI Builder Component

Create src/app/builder/builder.component.ts:

import { Component } from '@angular/core';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-builder',
  templateUrl: './builder.component.html',
  styleUrls: ['./builder.component.css']
})
export class BuilderComponent {
  htmlCode = '';
  cssCode = '';
  tsCode = '';
  componentName = '';

  constructor(private api: ApiService) {}

  saveComponent() {
    const component = {
      name: this.componentName,
      html: this.htmlCode,
      css: this.cssCode,
      ts: this.tsCode
    };

    this.api.createComponent(component).subscribe(() => {
      alert('Component saved successfully');
    });
  }
}

✅ Template:

<div class="container">
  <h2>UI Builder</h2>

  <input type="text" [(ngModel)]="componentName" placeholder="Component Name" class="form-control mb-3">

  <textarea [(ngModel)]="htmlCode" rows="5" class="form-control" placeholder="HTML"></textarea>
  <textarea [(ngModel)]="cssCode" rows="5" class="form-control" placeholder="CSS"></textarea>
  <textarea [(ngModel)]="tsCode" rows="5" class="form-control" placeholder="TypeScript"></textarea>

  <button (click)="saveComponent()" class="btn btn-success mt-3">Save Component</button>
</div>



⸻

🔥 4. Preview Component

Add a preview component to render the saved UI component dynamically.

✅ Dynamic Rendering Logic

<div [innerHTML]="htmlCode" class="preview"></div>
<style [innerHTML]="cssCode"></style>



⸻

🚀 5. Running the Application

✅ Start MongoDB

mongod --dbpath /path/to/your/data/db

✅ Start Backend

cd backend  
nodemon server.js  

✅ Start Angular Frontend

cd frontend  
ng serve  

✅ Access the app at:
	•	Frontend: http://localhost:4200
	•	Backend API: http://localhost:3000

⸻

🔥 6. Deployment Options
	•	Docker: Deploy both the backend and frontend in Docker containers
	•	IIS / NGINX: Serve the Angular app using NGINX and the Express server separately

⸻

✅ Final Outcome
	•	Angular-based UI Builder
	•	Store and preview reusable UI components
	•	Save templates in MongoDB
	•	Dynamically render UI components

🔥 Let me know if you need any additional features or refinements! 🚀