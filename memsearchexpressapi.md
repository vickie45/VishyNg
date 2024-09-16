To create a member search API using Express.js, you need to:

1. Set up an Express application.
2. Create a route that handles the GET request for member search.
3. Use query parameters to filter members.

Here’s a step-by-step guide to implement a simple member search API.

### 1. Install Required Dependencies

First, set up a basic Express application. You will need `express` and optionally `cors` to handle cross-origin requests if you're making calls from a frontend like Angular.

```bash
mkdir member-search-api
cd member-search-api
npm init -y
npm install express cors
```

### 2. Create the `app.js` File

In your project directory, create an `app.js` file and add the following code:

```javascript
const express = require('express');
const cors = require('cors');

const app = express();
const port = 3000;

// Allow CORS
app.use(cors());

// Dummy data for members
const members = [
  { employeeId: '001', insuredName: 'John Doe', patientRegistrationNo: '1234', policyName: 'HealthPlus', policyNo: 'H001', policyType: 'Health', providerBasicDetailId: 1001, uhid: 'UHID001' },
  { employeeId: '002', insuredName: 'Jane Smith', patientRegistrationNo: '1235', policyName: 'LifeCare', policyNo: 'L002', policyType: 'Life', providerBasicDetailId: 1002, uhid: 'UHID002' },
  { employeeId: '003', insuredName: 'Jack Johnson', patientRegistrationNo: '1236', policyName: 'HealthShield', policyNo: 'H003', policyType: 'Health', providerBasicDetailId: 1003, uhid: 'UHID003' },
  // Add more dummy members as needed
];

// API route to get member search details
app.get('/api/members', (req, res) => {
  const {
    employeeId,
    insuredName,
    patientRegistrationNo,
    policyName,
    policyNo,
    policyType,
    providerBasicDetailId,
    uhid
  } = req.query;

  // Filter members based on the search query parameters
  const filteredMembers = members.filter(member => {
    return (
      (!employeeId || member.employeeId === employeeId) &&
      (!insuredName || member.insuredName.toLowerCase().includes(insuredName.toLowerCase())) &&
      (!patientRegistrationNo || member.patientRegistrationNo === patientRegistrationNo) &&
      (!policyName || member.policyName.toLowerCase().includes(policyName.toLowerCase())) &&
      (!policyNo || member.policyNo === policyNo) &&
      (!policyType || member.policyType.toLowerCase() === policyType.toLowerCase()) &&
      (!providerBasicDetailId || member.providerBasicDetailId == providerBasicDetailId) &&
      (!uhid || member.uhid === uhid)
    );
  });

  res.json(filteredMembers);
});

// Start the server
app.listen(port, () => {
  console.log(`Member Search API running on port ${port}`);
});
```

### 3. Explanation of the Code

- **Data**: There’s a dummy `members` array that represents the members’ data. In a real application, you would fetch this data from a database.
  
- **CORS**: `cors` middleware is used to allow requests from different origins (e.g., an Angular frontend).

- **Route**: The `/api/members` route listens for a GET request, reads the query parameters (`employeeId`, `insuredName`, etc.), and filters the `members` array accordingly. If no filter is applied, it returns all members.

- **Filtering**: Each query parameter is optional. If the parameter is present, the filter checks if the member data matches the given parameter.

### 4. Test the API

To test the API, you can use `Postman` or a browser. Run the server:

```bash
node app.js
```

Then, make a request to the API, such as:

```
http://localhost:3000/api/members?insuredName=John
```

This will return the members whose `insuredName` contains "John".

You can also combine multiple query parameters:

```
http://localhost:3000/api/members?policyType=Health&uhid=UHID001
```

### 5. Connecting to a Database (Optional)

If you want to connect to a database, replace the dummy `members` array with actual database queries using a library like `mongoose` for MongoDB or `pg` for PostgreSQL. Here's an example with `pg`:

1. Install the `pg` package:

   ```bash
   npm install pg
   ```

2. Replace the member filtering with a database query:

   ```javascript
   const { Pool } = require('pg');
   const pool = new Pool({
     user: 'youruser',
     host: 'localhost',
     database: 'yourdb',
     password: 'yourpassword',
     port: 5432,
   });

   app.get('/api/members', async (req, res) => {
     try {
       const { rows } = await pool.query('SELECT * FROM members WHERE insuredName = $1', [req.query.insuredName]);
       res.json(rows);
     } catch (err) {
       res.status(500).json({ error: err.message });
     }
   });
   ```

This would query the `members` table in a PostgreSQL database.

### 6. Final Thoughts

You now have a fully working member search API in Express.js. You can expand it by connecting to a database or adding authentication.