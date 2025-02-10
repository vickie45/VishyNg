Here’s how you can implement file upload and handling in Angular 17, where the Angular app sends the file to an Express.js proxy server, which then forwards it to an ASP.NET Web API.

1. Angular 17 - Upload File to Express Proxy

Install Dependencies

You’ll need HttpClient for HTTP requests.

ng add @angular/common

Create File Upload Service

Create a service file-upload.service.ts to handle file uploads.

import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FileUploadService {
  private baseUrl = 'http://localhost:3000/api/upload'; // Proxy Server URL

  constructor(private http: HttpClient) {}

  uploadFile(file: File): Observable<HttpEvent<any>> {
    const formData = new FormData();
    formData.append('file', file);

    const req = new HttpRequest('POST', this.baseUrl, formData, {
      reportProgress: true,
      responseType: 'json',
    });

    return this.http.request(req);
  }
}

Create File Upload Component

Generate a new component:

ng generate component file-upload

Update file-upload.component.html:

<input type="file" (change)="onFileSelected($event)">
<button (click)="upload()">Upload</button>
<p *ngIf="progress > 0">Progress: {{ progress }}%</p>

Update file-upload.component.ts:

import { Component } from '@angular/core';
import { FileUploadService } from '../services/file-upload.service';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css'],
})
export class FileUploadComponent {
  selectedFile?: File;
  progress = 0;

  constructor(private fileUploadService: FileUploadService) {}

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
  }

  upload(): void {
    if (!this.selectedFile) {
      alert('Please select a file first!');
      return;
    }

    this.fileUploadService.uploadFile(this.selectedFile).subscribe((event) => {
      if (event.type === HttpEventType.UploadProgress && event.total) {
        this.progress = Math.round((100 * event.loaded) / event.total);
      } else if (event.type === HttpEventType.Response) {
        console.log('File uploaded successfully!', event.body);
        this.progress = 0;
      }
    });
  }
}

2. Express.js Proxy Server (Node.js)

Install Dependencies

npm init -y
npm install express multer axios cors

Create server.js

const express = require('express');
const multer = require('multer');
const cors = require('cors');
const axios = require('axios');

const app = express();
const port = 3000;

app.use(cors());
app.use(express.json());

const upload = multer({ dest: 'uploads/' });

app.post('/api/upload', upload.single('file'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).send({ message: 'No file uploaded' });
    }

    // Forward file to ASP.NET Web API
    const formData = new FormData();
    formData.append('file', req.file);

    const response = await axios.post('http://localhost:5000/api/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    res.json(response.data);
  } catch (error) {
    console.error('Error uploading file:', error);
    res.status(500).json({ error: 'File upload failed' });
  }
});

app.listen(port, () => {
  console.log(`Server running on http://localhost:${port}`);
});

3. ASP.NET Web API (C#)

Create API Controller (FilesController.cs)

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var filePath = Path.Combine("Uploads", file.FileName);

        Directory.CreateDirectory("Uploads");

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { Message = "File uploaded successfully!", FilePath = filePath });
    }
}

Configure Startup.cs or Program.cs (if using .NET 6+)

Add CORS to allow requests from Angular and Express.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseRouting();
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.Run();

4. Run the Setup

Start ASP.NET Web API

dotnet run

Runs on http://localhost:5000

Start Express Proxy

node server.js

Runs on http://localhost:3000

Run Angular App

ng serve

Runs on http://localhost:4200

Workflow

	1.	User selects a file in Angular.
	2.	Angular sends the file to http://localhost:3000/api/upload (Express server).
	3.	Express processes and forwards the file to http://localhost:5000/api/files/upload (ASP.NET API).
	4.	ASP.NET saves the file and responds.
	5.	Express sends the response back to Angular.

This setup ensures Angular doesn’t directly interact with ASP.NET API, keeping it secure with an Express proxy. Let me know if you need modifications!