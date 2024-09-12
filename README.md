# VishyNg
To implement a login and authentication system with CAPTCHA and encryption of username and password using an Express proxy server, an Angular app, and WCF services, follow these steps:

### 1. **Set Up the Angular Frontend** (Angular 17 + Bootstrap 5)

#### Install Necessary Packages:
```bash
npm install @angular/forms @angular/http crypto-js
```

#### Add a Login Component with CAPTCHA

```typescript
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as CryptoJS from 'crypto-js'; // For encryption
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})
export class LoginComponent {
  loginForm: FormGroup;
  captchaCode: string = ''; // Placeholder for CAPTCHA

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      captcha: ['', [Validators.required]],
    });

    this.generateCaptcha();
  }

  generateCaptcha() {
    this.captchaCode = Math.random().toString(36).substring(2, 8); // Simple CAPTCHA generation
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const { username, password, captcha } = this.loginForm.value;

      // Encrypt the username and password
      const encryptedUsername = CryptoJS.AES.encrypt(username, 'secretKey').toString();
      const encryptedPassword = CryptoJS.AES.encrypt(password, 'secretKey').toString();

      if (captcha !== this.captchaCode) {
        alert('Invalid CAPTCHA');
        return;
      }

      // Send encrypted credentials to the backend
      this.http.post('http://localhost:3000/api/login', { username: encryptedUsername, password: encryptedPassword })
        .subscribe(response => {
          console.log('Login successful', response);
        }, error => {
          console.error('Login failed', error);
        });
    }
  }
}
```

#### Add Login HTML

```html
<form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
  <div>
    <label for="username">Username:</label>
    <input id="username" formControlName="username" type="text" />
  </div>
  
  <div>
    <label for="password">Password:</label>
    <input id="password" formControlName="password" type="password" />
  </div>

  <div>
    <label for="captcha">Captcha: {{ captchaCode }}</label>
    <input id="captcha" formControlName="captcha" type="text" />
  </div>

  <button type="submit">Login</button>
</form>
```

### 2. **Create the Express Proxy Server**

This will handle the login request and forward it to the WCF service.

#### Install Required Dependencies:
```bash
npm install express body-parser axios cors
```

#### Create the Express Server:

```javascript
const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const axios = require('axios');
const app = express();

app.use(cors());
app.use(bodyParser.json());

app.post('/api/login', (req, res) => {
  const { username, password } = req.body;

  // Call the WCF service
  axios.post('http://your-wcf-service-url/Login', { username, password })
    .then(wcfResponse => {
      if (wcfResponse.data.success) {
        res.status(200).json({ message: 'Login successful' });
      } else {
        res.status(401).json({ message: 'Invalid credentials' });
      }
    })
    .catch(error => {
      console.error('Error connecting to WCF service:', error);
      res.status(500).json({ message: 'Internal server error' });
    });
});

app.listen(3000, () => {
  console.log('Express server is running on port 3000');
});
```

### 3. **WCF Service for Login Authentication**

The WCF service will validate the encrypted username and password received from the proxy server.

#### Define the WCF Contract:

```csharp
[ServiceContract]
public interface IAuthService
{
    [OperationContract]
    LoginResponse Login(LoginRequest request);
}

[DataContract]
public class LoginRequest
{
    [DataMember]
    public string Username { get; set; }

    [DataMember]
    public string Password { get; set; }
}

[DataContract]
public class LoginResponse
{
    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public string Message { get; set; }
}
```

#### Implement the WCF Service:

```csharp
public class AuthService : IAuthService
{
    public LoginResponse Login(LoginRequest request)
    {
        // Decrypt the username and password
        string decryptedUsername = Decrypt(request.Username);
        string decryptedPassword = Decrypt(request.Password);

        // Validate the credentials (use your own validation logic)
        if (decryptedUsername == "admin" && decryptedPassword == "password123")
        {
            return new LoginResponse { Success = true, Message = "Login successful" };
        }
        else
        {
            return new LoginResponse { Success = false, Message = "Invalid credentials" };
        }
    }

    private string Decrypt(string encryptedText)
    {
        // Decrypt logic (AES decryption)
        byte[] bytes = Convert.FromBase64String(encryptedText);
        // Perform decryption and return the result
        return Encoding.UTF8.GetString(bytes); // Simplified, use proper decryption
    }
}
```

### 4. **Connecting Everything**

- **Angular app** sends an encrypted username and password to **Express**.
- **Express** forwards this request to the **WCF service**.
- **WCF** decrypts the credentials and validates them.
- The response flows back from **WCF** to **Express** and finally to the **Angular app**.

### 5. **Running the Project**

1. Start your WCF service and ensure it is accessible via HTTP.
2. Run the Express proxy server:
   ```bash
   node server.js
   ```
3. Serve your Angular application:
   ```bash
   ng serve
   ```

With these steps, you will have a login system using an Angular app, an Express proxy server, and WCF, with CAPTCHA and encryption for enhanced security.
