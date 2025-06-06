Here is a detailed Requirement Specification Document for your Provider Portal – Health Claim Management System modernized using Angular 17 + Bootstrap 5, integrating with a backend built on ASP.NET Web API + WCF services.

⸻

📝 Requirement Specification Document

Project Name: Provider Portal – Health Claim Management System
Frontend Tech Stack: Angular 17, Bootstrap 5
Backend Integration: ASP.NET Web API + WCF Services
Legacy System: Migrating from .NET 4.5 ASP.NET MVC + jQuery + Bootstrap 4

⸻

1. 📌 Objective

To develop a modern, responsive, and secure provider portal that enables hospitals in tie-up with insurance companies to manage health claims efficiently. The system will allow user authentication, hospital selection, patient and claim management, AL (Authorization Letter) and CL (Claim Letter) processing, communication with insurance systems, reporting, document management, and administrative tools.

⸻

2. 👥 User Roles & Access

2.1 User Types
	•	Admin
	•	Hospital Admin
	•	Hospital User
	•	Insurance Coordinator
	•	Read-Only Auditor

2.2 Role-Based Permissions

Each module and sub-feature will support permission flags:
	•	View / Add / Edit / Delete
	•	Submit / Approve / Respond
	•	Upload / Download documents

⸻

3. 🔐 Authentication & Session
	•	Login with captcha (image/text-based)
	•	Session expiration with idle timeout
	•	JWT or token-based authentication (if backend allows)
	•	Audit log for all login attempts and activity

⸻

4. 🏥 Post-Login: Hospital Selection
	•	If a user is mapped to multiple hospitals:
	•	Display hospital list with search
	•	Allow user to select one and proceed
	•	Save selected hospital context across sessions until logout

⸻

5. 🧭 Dashboard

5.1 Layout
	•	Cards for major modules
	•	Top Navbar:
	•	Navigation links
	•	Notification bell
	•	Profile dropdown (change password, logout)
	•	Right Panel (Tabbed Container):
	•	Transactions Tab: Filter/search by patient name, date, claim status
	•	Reports & Updates Tab: Notification feed, system messages

5.2 Card Modules

Each card links to a full-page module:
	•	Patient Search
	•	Claim Search
	•	Reports
	•	Infrastructure
	•	Payments
	•	Communication
	•	MoU & Tariff
	•	Profile

⸻

6. 📂 Patient Search
	•	Search by UHID, name, phone number
	•	Show demographics, visit history, current claim(s)
	•	Option to initiate AL directly if patient is not in claim system

⸻

7. 📁 Claim Search
	•	Search by Intimation Number or UHID
	•	View:
	•	Claim status
	•	Timeline
	•	Uploaded documents
	•	Linked AL/CL
	•	Action Options:
	•	Add information
	•	Create revision
	•	Respond to query
	•	Submit fresh CL from approved AL

⸻

8. 📝 AL & CL Workflow

8.1 Common Tabs for Both
	1.	Case Details
	2.	Cost Details
	3.	Past Medical History
	4.	Maternity Info (optional tab if checkbox enabled)
	5.	Injury/Accident Info (optional tab if checkbox enabled)

8.2 Document Management Tab
	•	Select document type from dropdown
	•	Upload one or more files
	•	Delete option
	•	Preview (if file type supported)

8.3 Actions & Statuses
	•	Draft → Submitted → Approved / Rejected / Queried
	•	Revisions allowed
	•	Fresh AL → CL conversion if allowed by insurer
	•	Track response logs from insurance system

⸻

9. 💬 Communication
	•	Messaging module between hospital and insurance system
	•	Threaded message view per claim
	•	Attach documents to messages
	•	Notification alerts in navbar for new messages

⸻

10. 💳 Payments
	•	Claim settlement view per hospital
	•	Filters by date, status (paid, pending, disputed)
	•	Export to Excel
	•	Reconcile with bank/payment reference
	•	Raise disputes if mismatch

⸻

11. 🧾 Reports
	•	Generate reports:
	•	Daily/Monthly Claims
	•	Turnaround Time (TAT)
	•	AL → CL ratio
	•	Rejection reasons
	•	Query rate
	•	Export formats: PDF, Excel
	•	Visual dashboards (optional)

⸻

12. 🏗 Infrastructure
	•	Capture hospital infrastructure details
	•	Bed types
	•	Facilities
	•	Specialties
	•	Authorized users can edit/update
	•	Document proof upload (e.g., NABH certification)

⸻

13. 📜 MoU & Tariff
	•	View tariff list
	•	Upload MoU copies
	•	Add/edit tariffs by department and procedure
	•	Validity dates and status

⸻

14. 👤 Profile Module
	•	View hospital profile
	•	Update basic details
	•	Manage users under hospital (if admin)
	•	Change password, email, contact

⸻

15. 🔔 Notifications
	•	Shown in navbar
	•	Types: Claim Status Change, New Query, Approval, New Message
	•	Mark as read/unread
	•	Settings for notification preferences

⸻

16. 🧰 Technical Requirements

16.1 Angular 17
	•	Standalone components, signals (optional), lazy loading
	•	Routing + guards
	•	Reactive forms with dynamic validation
	•	State management using RxJS (or signals if appropriate)

16.2 Bootstrap 5
	•	Grid layout for responsive UI
	•	Cards, tabs, buttons, forms styled using BS5 classes

16.3 Backend Communication
	•	HttpClient for calling:
	•	ASP.NET Web API (REST)
	•	WCF Services (via REST wrapper or SOAP/XML)

16.4 File Upload
	•	Upload with progress bar
	•	File size/type validation
	•	Multi-file support

⸻

17. 📅 Audit & Logging
	•	Track all user actions:
	•	Login, logout, edits, uploads, deletions
	•	Log by user, timestamp, IP
	•	Export audit logs for inspection

⸻

18. 🛡 Security Considerations
	•	Input sanitization
	•	Output encoding
	•	CORS setup
	•	HTTPS only
	•	JWT or token/session-based auth
	•	Backend rate limiting (optional)

⸻

19. 🚀 Optional Enhancements
	•	Angular PWA (for mobile-friendly offline support)
	•	Dark mode / theme switch
	•	Chatbot assistant for claim FAQs
	•	AI-driven rejection pattern analysis (future roadmap)

⸻