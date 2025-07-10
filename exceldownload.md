Here is a complete working pipeline to send an Excel file from ASP.NET Web API to an Express.js proxy, store it temporarily on the server, and then allow Angular 17 frontend to download it via the Express.js endpoint.


---

✅ OVERVIEW:

1. ASP.NET Web API: Generates and sends Excel file as binary stream.


2. Express.js: Acts as proxy — receives, saves, and forwards it.


3. Angular 17: Triggers download via the Express proxy.




---

🔹 STEP 1: ASP.NET Web API (C#)

[HttpGet]
[Route("api/report/download")]
public HttpResponseMessage DownloadExcel()
{
    var fileBytes = System.IO.File.ReadAllBytes("path/to/sample.xlsx");
    var result = new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new ByteArrayContent(fileBytes)
    };

    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
    {
        FileName = "report.xlsx"
    };
    result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

    return result;
}

> You can dynamically generate Excel using libraries like ClosedXML or EPPlus.




---

🔹 STEP 2: Express.js Proxy Server (Node.js)

// server.js (Express)
const express = require('express');
const axios = require('axios');
const fs = require('fs');
const path = require('path');
const app = express();
const PORT = 3000;

app.get('/proxy/report/download', async (req, res) => {
  try {
    const apiUrl = 'http://localhost:5000/api/report/download'; // ASP.NET Web API URL
    const response = await axios.get(apiUrl, {
      responseType: 'stream'
    });

    const filePath = path.join(__dirname, 'report.xlsx');
    const writer = fs.createWriteStream(filePath);

    response.data.pipe(writer);

    writer.on('finish', () => {
      res.download(filePath, 'report.xlsx', (err) => {
        if (!err) {
          // Optional: Clean up file
          fs.unlinkSync(filePath);
        }
      });
    });

    writer.on('error', (err) => {
      res.status(500).send('Error writing file');
    });
  } catch (err) {
    console.error(err.message);
    res.status(500).send('Failed to download Excel');
  }
});

app.listen(PORT, () => {
  console.log(`Proxy server running on http://localhost:${PORT}`);
});


---

🔹 STEP 3: Angular 17 Frontend

✅ Angular Service (download.service.ts)

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DownloadService {
  constructor(private http: HttpClient) {}

  downloadExcel() {
    return this.http.get('http://localhost:3000/proxy/report/download', {
      responseType: 'blob',
      observe: 'response'
    });
  }
}

✅ Angular Component

import { Component } from '@angular/core';
import { DownloadService } from './download.service';

@Component({
  selector: 'app-excel-download',
  template: `<button (click)="download()">Download Excel</button>`
})
export class ExcelDownloadComponent {
  constructor(private downloadService: DownloadService) {}

  download() {
    this.downloadService.downloadExcel().subscribe((res) => {
      const blob = new Blob([res.body!], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      });
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'report.xlsx';
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    });
  }
}


---

✅ CORS & PROXY SETUP

Ensure:

CORS is allowed in .NET Web API if Express.js is hosted on a different port.

Angular should call Express (http://localhost:3000) — not directly call .NET API.

You can use a proxy config in Angular during development (proxy.conf.json) if needed.



---

✅ Example proxy.conf.json

{
  "/proxy": {
    "target": "http://localhost:3000",
    "secure": false,
    "changeOrigin": true
  }
}

> Add "proxyConfig": "proxy.conf.json" to your angular.json.




---

Would you like to store the Excel permanently in proxy for later download, or is this just for immediate forwarding use?

