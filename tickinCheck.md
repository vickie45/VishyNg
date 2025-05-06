Understood. If everything appears correct — type="checkbox", Bootstrap 5 classes, proper markup — yet the checkbox still shows a circle highlight instead of a tick, the most likely causes are:

⸻

Root Cause: appearance: none or Custom Theme Conflicts

One or more of the following might be in effect:
	1.	Your global CSS or a component library is overriding Bootstrap styles, especially with:

input[type="checkbox"] {
  appearance: none;
  border-radius: 50%;
  /* other custom styles */
}


	2.	You’re using a custom theme or UI library (like Tailwind, Materialize, or MDBootstrap) which could override Bootstrap’s checkbox rendering.
	3.	Browser defaults are being suppressed (e.g., by a reset.css or normalize.css with unintended effects).

⸻

✅ Definitive Fix (Enforce Bootstrap 5 Checkbox Style)

Add the following reset CSS to bring back the proper tick behavior:

input[type="checkbox"].form-check-input {
  appearance: auto !important;
  -webkit-appearance: checkbox !important;
  -moz-appearance: checkbox !important;
  border-radius: 0.25em !important; /* square shape */
}

This enforces native checkbox appearance and resets any circle-style overrides.

⸻

✅ Test Case — Paste This Minimal HTML

Try this in a blank HTML file to isolate the problem:

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Bootstrap 5 Checkbox Test</title>
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
  <style>
    input[type="checkbox"].form-check-input {
      appearance: auto !important;
      -webkit-appearance: checkbox !important;
      -moz-appearance: checkbox !important;
      border-radius: 0.25em !important;
    }
  </style>
</head>
<body>
  <div class="container mt-5">
    <div class="form-check">
      <input class="form-check-input" type="checkbox" id="check1">
      <label class="form-check-label" for="check1">
        This is a checkbox
      </label>
    </div>
  </div>
</body>
</html>

If this shows a proper tick mark, then your original project definitely has an overriding CSS issue.

⸻

Would you like to paste your project’s checkbox HTML and any related CSS here for direct inspection?