Perfect. Here’s how to create tabs using <a> tags in Angular with no routerLink or TypeScript logic — only HTML and CSS.

We’ll simulate tabs using anchor <a> tags with href="#id" and style the active tab using the :target CSS selector.

⸻

✅ Pure Angular Template with <a> Tags and CSS (No Router, No TS)

1. HTML (in your component template)

<div class="tabs-container">
  <!-- Tab Headers -->
  <div class="tabs">
    <a href="#tab-home" class="tab">Home</a>
    <a href="#tab-profile" class="tab">Profile</a>
    <a href="#tab-settings" class="tab">Settings</a>
  </div>

  <!-- Tab Content -->
  <div class="tab-content">
    <div id="tab-home" class="content">Welcome to Home Tab</div>
    <div id="tab-profile" class="content">This is your Profile</div>
    <div id="tab-settings" class="content">Adjust your Settings</div>
  </div>
</div>



⸻

2. CSS (in your component’s .css file)

.tabs {
  display: flex;
  border-bottom: 2px solid #ccc;
  margin-bottom: 10px;
}

.tab {
  padding: 10px 20px;
  text-decoration: none;
  color: #555;
  border-bottom: 2px solid transparent;
  transition: 0.3s;
}

.tab:hover {
  color: #000;
}

/* Show only targeted content */
.tab-content .content {
  display: none;
}

/* Show content when it is targeted */
.content:target {
  display: block;
}

/* Highlight active tab when its content is targeted */
a[href="#tab-home"]:focus,
#tab-home:target ~ .tabs a[href="#tab-home"] {
  color: #007bff;
  border-bottom: 2px solid #007bff;
  font-weight: bold;
}
a[href="#tab-profile"]:focus,
#tab-profile:target ~ .tabs a[href="#tab-profile"] {
  color: #007bff;
  border-bottom: 2px solid #007bff;
  font-weight: bold;
}
a[href="#tab-settings"]:focus,
#tab-settings:target ~ .tabs a[href="#tab-settings"] {
  color: #007bff;
  border-bottom: 2px solid #007bff;
  font-weight: bold;
}



⸻

Optional Enhancement (Default Tab)

Browsers don’t apply :target unless a hash exists in the URL. To show the default tab (like “Home”) initially, you can add this at the end of your CSS:

/* Default display if no target */
body:not(:has(:target)) #tab-home {
  display: block;
}

	Note: :has() has limited support, mostly in modern Chromium browsers only. If compatibility is critical, JavaScript would be required for default tab selection.

⸻

Would you like me to convert this into a reusable Angular component with inputs (e.g., for dynamic tab generation)?