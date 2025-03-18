✅ File Upload in Angular and .NET Web API

⸻

🔥 1. .NET Web API – Backend Implementation

Step 1: Create a .NET Web API Project
	1.	Open Visual Studio → Create a new project → Select ASP.NET Core Web API.
	2.	Select .NET version and configure the project.

⸻

Step 2: Configure the Controller
	•	Create a new folder named Controllers (if it doesn’t already exist).
	•	Add a new controller called FileUploadController.cs.

📌 FileUploadController.cs

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace FileUploadExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { Message = "File uploaded successfully!", FileName = file.FileName });
        }
    }
}



⸻

Step 3: Configure Program.cs

Ensure you enable CORS and configure the file size limits.

📌 Program.cs

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();



⸻

🔥 2. Angular – Frontend Implementation

Step 1: Create Angular Project

ng new FileUploadApp
cd FileUploadApp
ng add @angular/material  # Optional for UI components
ng generate component FileUpload



⸻

Step 2: Install Angular HTTP Client

Make sure you have HttpClientModule installed.

📌 app.module.ts

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { FileUploadComponent } from './file-upload/file-upload.component';

@NgModule({
  declarations: [AppComponent, FileUploadComponent],
  imports: [BrowserModule, HttpClientModule],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }



⸻

Step 3: Create File Upload Service

📌 file-upload.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private apiUrl = 'https://localhost:5001/api/FileUpload/upload';  // API URL

  constructor(private http: HttpClient) { }

  uploadFile(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    const headers = new HttpHeaders();
    return this.http.post(this.apiUrl, formData, { headers });
  }
}



⸻

Step 4: Create the File Upload Component

📌 file-upload.component.ts

import { Component } from '@angular/core';
import { FileUploadService } from '../file-upload.service';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent {
  selectedFile: File | null = null;

  constructor(private fileUploadService: FileUploadService) {}

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  uploadFile() {
    if (this.selectedFile) {
      this.fileUploadService.uploadFile(this.selectedFile).subscribe(
        (response) => {
          console.log('File uploaded successfully', response);
          alert('File uploaded successfully');
        },
        (error) => {
          console.error('Error uploading file', error);
          alert('File upload failed');
        }
      );
    }
  }
}



⸻

Step 5: Add HTML Template

📌 file-upload.component.html

<div style="text-align: center; margin-top: 50px;">
  <h2>File Upload</h2>
  <input type="file" (change)="onFileSelected($event)" />
  <button (click)="uploadFile()" [disabled]="!selectedFile">Upload</button>
</div>



⸻

🔥 3. Running the Application

Backend
	•	Run the Web API using Visual Studio or the terminal:

dotnet run

Frontend
	•	Start the Angular app:

ng serve

	•	Navigate to http://localhost:4200 to see the file upload interface.

⸻

✅ Testing and Validation
	•	Ensure that the uploaded files are saved to the wwwroot/uploads folder on the server.
	•	You can add validation (e.g., file size, type) on both the frontend and backend.

⸻

🚀 Enhancements
	•	Add progress indicators for file upload status.
	•	Implement multiple file uploads.
	•	Include authentication and authorization for secure uploads.
	•	Implement error handling with custom messages.

⸻

🎯 Key Takeaway

You now have a complete Angular and .NET Web API file upload solution. Let me know if you need modifications, validations, or additional features!