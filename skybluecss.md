Certainly Vignesh. Below is a sky blue–based global style.css that you can directly use in your Angular 17 project for a professional and aesthetically impressive look using Bootstrap 5. This CSS is suitable for dashboards, forms, cards, modals, and buttons — all styled with a clean and corporate-friendly visual language.

⸻

✅ Global style.css (Sky Blue Theme)

/* ===== Sky Blue Global Theme ===== */

/* Color Palette */
:root {
  --primary-sky: #00bfff;
  --primary-light: #e6f7ff;
  --primary-dark: #003366;
  --text-muted: #6c757d;
  --bg-soft: #f8f9fa;
  --card-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

/* Body */
body {
  background-color: var(--bg-soft);
  font-family: 'Segoe UI', Roboto, sans-serif;
  color: var(--primary-dark);
  margin: 0;
}

/* Headings */
h1, h2, h3, h4, h5 {
  color: var(--primary-dark);
  font-weight: 600;
}

/* Buttons */
.btn-primary {
  background-color: var(--primary-sky);
  border-color: var(--primary-sky);
  color: #fff;
}
.btn-primary:hover {
  background-color: #009acd;
  border-color: #009acd;
}

/* Cards */
.card {
  border: none;
  border-radius: 0.5rem;
  box-shadow: var(--card-shadow);
  background-color: #ffffff;
}

.card-header {
  background-color: var(--primary-light);
  color: var(--primary-dark);
  font-weight: 600;
  border-bottom: none;
}

/* Forms */
.form-control {
  border-radius: 0.5rem;
  border-color: #cceeff;
}
.form-control:focus {
  border-color: var(--primary-sky);
  box-shadow: 0 0 0 0.2rem rgba(0, 191, 255, 0.25);
}

/* Nav Tabs */
.nav-tabs .nav-link.active {
  background-color: var(--primary-light);
  border-color: var(--primary-sky) var(--primary-sky) #fff;
  color: var(--primary-dark);
}

.nav-tabs .nav-link {
  color: var(--text-muted);
}
.nav-tabs .nav-link:hover {
  color: var(--primary-sky);
}

/* Sidebar (Optional) */
.sidebar {
  background-color: var(--primary-dark);
  color: #ffffff;
  padding: 1rem;
}
.sidebar a {
  color: #e6f7ff;
  text-decoration: none;
  display: block;
  padding: 0.5rem 1rem;
  border-radius: 0.25rem;
}
.sidebar a:hover {
  background-color: var(--primary-sky);
  color: #ffffff;
}

/* Footer */
.footer {
  background-color: #ffffff;
  border-top: 1px solid #dee2e6;
  text-align: center;
  padding: 1rem;
  font-size: 0.9rem;
  color: var(--text-muted);
}

/* Modals */
.modal-header {
  background-color: var(--primary-light);
  color: var(--primary-dark);
  font-weight: 600;
  border-bottom: none;
}


⸻

📌 Usage Instructions:
	1.	Place this in your project’s src/styles.css.
	2.	Ensure Bootstrap 5 is installed:

npm install bootstrap


	3.	Then, import Bootstrap in angular.json:

"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
]



⸻

Would you like a dashboard homepage layout or a login page mockup using this style? I can generate a responsive Angular component structure and HTML for that as well.