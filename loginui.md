To design a responsive login page similar to the one in the image you shared using Bootstrap 5, here's a step-by-step guide.

### 1. **HTML Structure:**

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        body, html {
            height: 100%;
            margin: 0;
        }

        .bg-img {
            background-image: url('your-image-path.jpg'); /* Replace with your image path */
            background-position: center;
            background-size: cover;
            background-repeat: no-repeat;
            height: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .login-form {
            background-color: rgba(255, 255, 255, 0.8); /* Semi-transparent white */
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
        }

        .login-btn {
            background-color: #ff1493; /* Similar to pink login button */
            color: white;
            font-weight: bold;
        }
        
        .contact-links {
            text-align: center;
            margin-top: 20px;
            color: #ff1493;
        }

        .contact-links a {
            color: #ff1493;
            margin: 0 10px;
            text-decoration: none;
        }

        .contact-links a:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <div class="bg-img">
        <div class="login-form text-center">
            <img src="reliance-logo.png" alt="Reliance Logo" class="mb-4" style="width: 150px;">
            <form>
                <div class="mb-3">
                    <input type="text" class="form-control" placeholder="Username" required>
                </div>
                <div class="mb-3">
                    <input type="password" class="form-control" placeholder="Password" required>
                </div>
                <div class="mb-3">
                    <input type="text" class="form-control" placeholder="Captcha text" required>
                </div>
                <button type="submit" class="btn login-btn btn-block w-100">LOGIN</button>
            </form>
            <div class="mt-3">
                <a href="#" class="text-muted">Forgot Password?</a>
            </div>
            <div class="contact-links">
                <a href="#">Contact Us</a> |
                <a href="#">Feedback</a>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
```

### 2. **Explanation:**

- **Background Image:**
  - The image is set in `.bg-img` as the background. Replace `'your-image-path.jpg'` with the path to your actual image.
  
- **Login Form:**
  - The form is centered both vertically and horizontally using Flexbox inside the `.bg-img` div.
  - The form itself is wrapped in a `div.login-form` with a semi-transparent white background and some padding for spacing.
  
- **Form Controls:**
  - Bootstrap’s form controls are used to create the input fields.
  - The button is styled to resemble the pink button in your image by setting the background color to `#ff1493`.

- **Links:**
  - The "Contact Us" and "Feedback" links are styled similarly to those in the image, centered and spaced evenly.

### 3. **Responsive Design:**

This layout is already responsive due to Bootstrap's grid and flexbox layout. The background image will cover the full screen, and the login form will resize itself appropriately for different screen sizes. You can adjust further for mobile if needed with media queries.

You can replace the logo and background image paths with the actual paths from your project’s assets folder.

Let me know if you need further customization or any adjustments!