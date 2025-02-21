Here’s a highly professional, aesthetically appealing “Sky Theme” CSS for your health claim management application, designed to override Bootstrap 5 while maintaining usability, accessibility, and a sleek, modern look.

Sky Theme (Global CSS for Bootstrap 5 Override)

/* Sky Theme - Bootstrap 5 Override */
/* Colors: Sky Blue, Soft Neutrals, Subtle Gradients */

:root {
    --sky-primary: #3A9AD9; /* Vibrant Sky Blue */
    --sky-secondary: #4FC1E9; /* Soft Azure */
    --sky-accent: #217DBB; /* Deeper Blue */
    --sky-light: #ECF7FF; /* Light Sky Tint */
    --sky-dark: #1C3D5A; /* Deep Navy */
    --sky-gray: #6C757D; /* Neutral Gray */
    --sky-bg: #F4F9FC; /* Light Background */
    --sky-text: #1A1A1A; /* Dark Text */
    --sky-success: #27AE60; /* Soft Green */
    --sky-danger: #E74C3C; /* Subtle Red */
    --sky-warning: #F39C12; /* Golden Yellow */
    --sky-info: #5DADE2; /* Light Blue Info */
}

/* General Body & Typography */
body {
    background-color: var(--sky-bg);
    color: var(--sky-text);
    font-family: 'Inter', 'Segoe UI', Arial, sans-serif;
    line-height: 1.6;
}

/* Override Bootstrap Container */
.container {
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    padding: 20px;
}

/* Buttons */
.btn {
    border-radius: 8px;
    padding: 10px 16px;
    font-weight: 500;
    transition: all 0.3s ease-in-out;
    border: none;
}

.btn-primary {
    background: linear-gradient(135deg, var(--sky-primary), var(--sky-secondary));
    color: white;
}

.btn-primary:hover {
    background: var(--sky-accent);
}

.btn-outline-primary {
    border: 2px solid var(--sky-primary);
    color: var(--sky-primary);
}

.btn-outline-primary:hover {
    background: var(--sky-primary);
    color: white;
}

/* Navbar */
.navbar {
    background: linear-gradient(180deg, var(--sky-primary) 0%, var(--sky-accent) 100%);
    padding: 12px 16px;
    box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
}

.navbar-brand {
    font-size: 1.5rem;
    font-weight: 600;
    color: white;
}

.navbar-nav .nav-link {
    color: white;
    font-weight: 500;
    transition: color 0.3s ease-in-out;
}

.navbar-nav .nav-link:hover {
    color: var(--sky-light);
}

/* Cards */
.card {
    border: none;
    border-radius: 12px;
    box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.08);
}

.card-header {
    background: var(--sky-light);
    font-weight: 600;
}

/* Forms */
.form-control {
    border-radius: 8px;
    border: 1px solid var(--sky-gray);
    padding: 10px;
    transition: border-color 0.3s ease-in-out;
}

.form-control:focus {
    border-color: var(--sky-primary);
    box-shadow: 0 0 6px rgba(58, 154, 217, 0.4);
}

/* Tables */
.table {
    background: white;
    border-radius: 8px;
    overflow: hidden;
}

.table thead {
    background: var(--sky-primary);
    color: white;
}

.table tbody tr:nth-child(even) {
    background: var(--sky-light);
}

/* Alerts */
.alert {
    border-radius: 8px;
}

.alert-primary {
    background: var(--sky-light);
    color: var(--sky-primary);
}

/* Footer */
.footer {
    background: var(--sky-dark);
    color: white;
    text-align: center;
    padding: 10px 0;
}

/* Responsive Tweaks */
@media (max-width: 768px) {
    .container {
        padding: 15px;
    }

    .navbar-brand {
        font-size: 1.3rem;
    }
}

Why This Works for a Health Claim Management App

	1.	Sky Theme Colors → Uses light blues, soft neutrals, and gradients for a clean and professional medical look.
	2.	Smooth UI Elements → Modern, rounded buttons, cards, and forms improve usability and accessibility.
	3.	Enhanced Readability → Proper font weights, color contrasts, and spacing ensure easy reading and interaction.
	4.	Professional Styling → Custom shadows, gradients, and hover effects create a polished, high-end UI.
	5.	Improved User Experience → Smooth transitions and responsive designs enhance functionality on all devices.

Would you like further refinements or additional UI elements?