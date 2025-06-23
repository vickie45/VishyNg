To handle a file upload sent from an Angular application to an Express.js proxy server, which then forwards the file to an ASP.NET Web API, follow this 3-step process:

⸻

🔹 1. Angular Frontend – Sending the File

Use FormData to upload the file to Express:

uploadFile(file: File) {
  const formData = new FormData();
  formData.append('file', file);

  return this.http.post('http://localhost:3000/api/upload', formData);
}

	•	Make sure HttpClientModule is configured.
	•	file is selected using an <input type="file"> element.

⸻

🔹 2. Express Proxy Server – Receiving & Forwarding

✅ Install Required Packages:

npm install express multer axios form-data

✅ server.js or uploadRouter.js:

const express = require('express');
const multer = require('multer');
const axios = require('axios');
const FormData = require('form-data');
const fs = require('fs');
const path = require('path');

const app = express();
const upload = multer({ dest: 'uploads/' }); // Temp storage

// Route to receive and forward file
app.post('/api/upload', upload.single('file'), async (req, res) => {
  try {
    const filePath = req.file.path;
    const fileStream = fs.createReadStream(filePath);

    const form = new FormData();
    form.append('file', fileStream, req.file.originalname);

    const response = await axios.post('http://localhost:5000/api/files/upload', form, {
      headers: {
        ...form.getHeaders()
      }
    });

    fs.unlinkSync(filePath); // Clean up temp file
    res.status(200).json(response.data);

  } catch (err) {
    console.error(err);
    res.status(500).send('File upload failed');
  }
});

app.listen(3000, () => console.log('Express server running on port 3000'));

✅ Notes:
	•	multer saves the incoming file to uploads/.
	•	The file is streamed to the ASP.NET Web API via axios and FormData.
	•	File is deleted afterward with fs.unlinkSync.

⸻

🔹 3. ASP.NET Web API – Receiving the File

✅ Controller Method:

[HttpPost]
[Route("api/files/upload")]
public async Task<IHttpActionResult> UploadFile()
{
    if (!Request.Content.IsMimeMultipartContent())
        return BadRequest("Unsupported media type");

    var provider = new MultipartMemoryStreamProvider();
    await Request.Content.ReadAsMultipartAsync(provider);

    foreach (var file in provider.Contents)
    {
        var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
        var buffer = await file.ReadAsByteArrayAsync();
        var filePath = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + filename);
        File.WriteAllBytes(filePath, buffer);
    }

    return Ok("File uploaded successfully");
}

✅ Ensure:
	•	System.Net.Http.Formatting is used.
	•	You have the right permissions on the target folder (~/UploadedFiles/).
	•	Enable CORS if requests are cross-domain.

⸻

✅ Summary

Layer	Tech	Role
Angular	HttpClient + FormData	Sends file to Express
Express.js	multer + axios + form-data	Receives file, forwards to ASP.NET
ASP.NET Web API	MultipartMemoryStreamProvider	Handles file and saves it


⸻

Would you like this architecture to include progress tracking or multiple file support as well?