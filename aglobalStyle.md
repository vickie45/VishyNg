/* === Global Styles === */
body {
  font-family: 'Roboto', sans-serif !important;
  background-color: #f3f9fc !important; /* Light blue background */
  color: #333 !important;
  margin: 0 !important;
  padding: 0 !important;
  line-height: 1.6 !important;
  overflow-x: hidden !important;
}

h1, h2, h3, h4, h5, h6 {
  font-weight: 500 !important;
  color: #007ea7 !important; /* Cyan headers */
}

a {
  text-decoration: none !important;
  color: #007ea7 !important;
}

a:hover {
  color: #005b73 !important;
  text-decoration: underline !important;
}

/* === Navbar === */
.navbar {
  background-color: #00bcd4 !important; /* Cyan navbar */
  padding: 1rem !important;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1) !important;
}

.navbar-brand,
.nav-link {
  color: #fff !important;
  font-size: 1rem !important;
}

.navbar-brand:hover,
.nav-link:hover {
  color: #e1f5fe !important;
}

.navbar-toggler {
  border: none !important;
}

.navbar-toggler-icon {
  background-color: #fff !important;
}

/* === Buttons === */
.btn-cyan {
  background-color: #00bcd4 !important;
  color: #fff !important;
  border: none !important;
  font-size: 1rem !important;
  padding: 0.5rem 1.5rem !important;
  border-radius: 0.3rem !important;
  transition: background-color 0.3s ease !important;
}

.btn-cyan:hover {
  background-color: #007ea7 !important;
  color: #fff !important;
}

/* === Containers === */
.container-cyan {
  background-color: #e1f5fe !important; /* Light cyan container */
  padding: 2rem !important;
  border-radius: 0.5rem !important;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1) !important;
  margin-bottom: 2rem !important;
}

/* === Cards === */
.card-custom {
  background-color: #f5f5f5 !important; /* Light grey */
  border: none !important;
  border-radius: 0.5rem !important;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1) !important;
  padding: 1.5rem !important;
}

.card-header {
  background-color: #00bcd4 !important; /* Cyan card header */
  color: #fff !important;
  border-radius: 0.5rem 0.5rem 0 0 !important;
  font-weight: 500 !important;
}

/* === Forms === */
.form-control {
  border: 1px solid #ccc !important;
  border-radius: 0.25rem !important;
  padding: 0.5rem 1rem !important;
}

.form-control:focus {
  border-color: #00bcd4 !important;
  box-shadow: 0 0 5px rgba(0, 188, 212, 0.5) !important;
}

label {
  font-weight: 500 !important;
  color: #007ea7 !important;
}

/* === Tables === */
.table-responsive {
  overflow-x: auto !important;
  -webkit-overflow-scrolling: touch !important; /* Smooth scrolling */
}

.table {
  margin-bottom: 0 !important;
  border-collapse: collapse !important;
}

.table th {
  background-color: #00bcd4 !important;
  color: #fff !important;
  text-align: center !important;
}

.table td {
  border: 1px solid #ddd !important;
  text-align: center !important;
  padding: 0.75rem !important;
}

.table-hover tbody tr:hover {
  background-color: #e1f5fe !important; /* Light cyan row hover */
}

/* === Accordion === */
.accordion-button {
  background-color: #00bcd4 !important; /* Cyan accordion header */
  color: #fff !important;
  font-weight: 500 !important;
  border: none !important;
}

.accordion-button:not(.collapsed) {
  background-color: #007ea7 !important; /* Deeper cyan when expanded */
}

.accordion-body {
  background-color: #f5f5f5 !important; /* Light grey accordion body */
  color: #333 !important;
}

/* === Footer === */
footer {
  background-color: #007ea7 !important; /* Deep cyan footer */
  color: #fff !important;
  padding: 1rem !important;
  text-align: center !important;
}

footer a {
  color: #e1f5fe !important;
  text-decoration: none !important;
}

footer a:hover {
  text-decoration: underline !important;
}

/* === Media Queries for Responsiveness === */
@media (max-width: 768px) {
  .navbar-brand {
    font-size: 1.2rem !important;
  }

  .btn-cyan {
    font-size: 0.9rem !important;
    padding: 0.4rem 1rem !important;
  }

  .form-control {
    font-size: 0.9rem !important;
  }

  .card-custom {
    padding: 1rem !important;
  }
}