Thank you for the clarification. If your Bootstrap 5 accordion opens but doesn’t close, here are the most common causes and their specific solutions:

⸻

1. Check if data-bs-parent is Causing Locking

If you are using data-bs-parent="#fileUploadAccordion" and there’s only one accordion item, Bootstrap doesn’t toggle the same panel again—it only ensures only one stays open.

Solution: Remove data-bs-parent if you’re using a single panel.

<div id="collapseUpload"
     class="accordion-collapse collapse show"
     aria-labelledby="headingUpload"
-    data-bs-parent="#fileUploadAccordion">
+>



⸻

2. Ensure Correct Button Attributes

Your toggle button must have:

<button
  class="accordion-button"
  type="button"
  data-bs-toggle="collapse"
  data-bs-target="#collapseUpload"
  aria-expanded="true"
  aria-controls="collapseUpload">
  File Upload
</button>

Common mistakes to avoid:
	•	Using wrong data-bs-target
	•	Forgetting data-bs-toggle="collapse"
	•	Using href="#collapseUpload" instead of data-bs-target

⸻

3. Manually Initialized Collapse via JavaScript?

If you’ve manually initialized with JS like this:

new bootstrap.Collapse(document.getElementById('collapseUpload'));

Then Bootstrap sets it as always expanded unless you manually call .toggle() or .hide().

Fix: Either avoid manual init, or ensure you toggle manually:

const panel = document.getElementById('collapseUpload');
const bsCollapse = bootstrap.Collapse.getInstance(panel) || new bootstrap.Collapse(panel);
bsCollapse.toggle();



⸻

4. Accordion Header or Parent Has pointer-events: none or disabled

Use dev tools to inspect the button. Sometimes styling or a parent tag disables pointer events.

Fix: Remove pointer-events: none; from CSS if applied unintentionally.

⸻

5. Another Element Inside the Button Prevents Collapse

If your button has complex inner elements (icons, spans, etc.), make sure they don’t prevent bubbling.

⸻

Sample Working Accordion (Single Item, Closes on Toggle)

<div class="accordion" id="fileUploadAccordion">
  <div class="accordion-item">
    <h2 class="accordion-header" id="headingUpload">
      <button class="accordion-button collapsed" type="button"
              data-bs-toggle="collapse" data-bs-target="#collapseUpload"
              aria-expanded="false" aria-controls="collapseUpload">
        File Upload
      </button>
    </h2>
    <div id="collapseUpload" class="accordion-collapse collapse"
         aria-labelledby="headingUpload">
      <div class="accordion-body">
        <input type="file" class="form-control">
      </div>
    </div>
  </div>
</div>

Note: collapsed class in button and collapse class in panel are required for toggle behavior.

⸻

If the issue still persists, kindly paste your accordion HTML block, and I will diagnose it directly for you. Would you like that?