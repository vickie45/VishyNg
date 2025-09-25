# React Frontend Setup Script
# Run this after the main setup script

cd employee-system

# Create React app with Vite
npm create vite@latest web-frontend -- --template react --yes
cd web-frontend

# Install additional dependencies
npm install axios

# Create the main App component
cat > src/App.jsx << 'EOF'
import { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

function App() {
  const [claims, setClaims] = useState([]);
  const [employeeId, setEmployeeId] = useState('');
  const [amount, setAmount] = useState('');
  const [claimType, setClaimType] = useState('termination-pay');
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const API_BASE = 'http://localhost:5000/api';

  useEffect(() => {
    fetchClaims();
  }, []);

  const fetchClaims = async () => {
    try {
      const response = await axios.get(`${API_BASE}/claims`);
      setClaims(response.data);
    } catch (error) {
      console.error('Error fetching claims:', error);
      setMessage('Failed to fetch claims');
    }
  };

  const createClaim = async (e) => {
    e.preventDefault();
    if (!employeeId || !amount) {
      setMessage('Employee ID and Amount are required');
      return;
    }

    setLoading(true);
    try {
      const claim = {
        employeeId,
        claimType,
        amount: parseFloat(amount)
      };

      await axios.post(`${API_BASE}/claims`, claim);
      setMessage('Claim created successfully!');
      setEmployeeId('');
      setAmount('');
      fetchClaims(); // Refresh the list
    } catch (error) {
      console.error('Error creating claim:', error);
      setMessage('Failed to create claim');
    }
    setLoading(false);
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Employee Claims System</h1>
      </header>

      <main className="main-content">
        <div className="form-section">
          <h2>Create New Claim</h2>
          <form onSubmit={createClaim} className="claim-form">
            <div className="form-group">
              <label htmlFor="employeeId">Employee ID:</label>
              <input
                type="text"
                id="employeeId"
                value={employeeId}
                onChange={(e) => setEmployeeId(e.target.value)}
                placeholder="Enter employee ID"
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="claimType">Claim Type:</label>
              <select
                id="claimType"
                value={claimType}
                onChange={(e) => setClaimType(e.target.value)}
              >
                <option value="termination-pay">Termination Pay</option>
                <option value="overtime">Overtime</option>
                <option value="expenses">Expenses</option>
                <option value="bonus">Bonus</option>
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="amount">Amount ($):</label>
              <input
                type="number"
                id="amount"
                step="0.01"
                min="0"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                placeholder="Enter amount"
                required
              />
            </div>

            <button type="submit" disabled={loading}>
              {loading ? 'Creating...' : 'Create Claim'}
            </button>
          </form>

          {message && (
            <div className={`message ${message.includes('successfully') ? 'success' : 'error'}`}>
              {message}
            </div>
          )}
        </div>

        <div className="claims-section">
          <h2>Recent Claims</h2>
          <div className="claims-list">
            {claims.length === 0 ? (
              <p>No claims found</p>
            ) : (
              claims.map((claim) => (
                <div key={claim.id} className="claim-card">
                  <div className="claim-header">
                    <span className="employee-id">ID: {claim.employeeId}</span>
                    <span className={`status ${claim.isProcessed ? 'processed' : 'pending'}`}>
                      {claim.isProcessed ? 'Processed' : 'Pending'}
                    </span>
                  </div>
                  <div className="claim-details">
                    <p><strong>Type:</strong> {claim.claimType}</p>
                    <p><strong>Amount:</strong> ${claim.amount.toFixed(2)}</p>
                    <p><strong>Created:</strong> {new Date(claim.createdAt).toLocaleString()}</p>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

export default App;
EOF

# Create updated CSS
cat > src/App.css << 'EOF'
.App {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
}

.App-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 30px;
  border-radius: 10px;
  margin-bottom: 30px;
  text-align: center;
}

.App-header h1 {
  margin: 0;
  font-size: 2.5rem;
  font-weight: 300;
}

.main-content {
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 30px;
  align-items: start;
}

.form-section {
  background: white;
  padding: 25px;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.form-section h2 {
  margin-top: 0;
  color: #333;
  font-size: 1.5rem;
  margin-bottom: 20px;
}

.claim-form {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 5px;
  font-weight: 500;
  color: #555;
}

.form-group input,
.form-group select {
  padding: 12px;
  border: 2px solid #e1e1e1;
  border-radius: 6px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #667eea;
}

button[type="submit"] {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 12px 24px;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: transform 0.2s ease;
}

button[type="submit"]:hover:not(:disabled) {
  transform: translateY(-1px);
}

button[type="submit"]:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.message {
  margin-top: 15px;
  padding: 12px;
  border-radius: 6px;
  font-weight: 500;
}

.message.success {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.message.error {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.claims-section {
  background: white;
  padding: 25px;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.claims-section h2 {
  margin-top: 0;
  color: #333;
  font-size: 1.5rem;
  margin-bottom: 20px;
}

.claims-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.claim-card {
  background: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  padding: 15px;
  transition: transform 0.2s ease;
}

.claim-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.claim-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.employee-id {
  font-weight: 600;
  color: #495057;
}

.status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  text-transform: uppercase;
}

.status.processed {
  background-color: #d4edda;
  color: #155724;
}

.status.pending {
  background-color: #fff3cd;
  color: #856404;
}

.claim-details {
  color: #6c757d;
}

.claim-details p {
  margin: 5px 0;
}

.claim-details strong {
  color: #495057;
}

@media (max-width: 768px) {
  .main-content {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  
  .App {
    padding: 10px;
  }
  
  .App-header h1 {
    font-size: 2rem;
  }
}
EOF

# Update package.json to add proxy for development
cat > package.json << 'EOF'
{
  "name": "web-frontend",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "lint": "eslint . --ext js,jsx --report-unused-disable-directives --max-warnings 0",
    "preview": "vite preview"
  },
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "axios": "^1.6.0"
  },
  "devDependencies": {
    "@types/react": "^18.2.37",
    "@types/react-dom": "^18.2.15",
    "@vitejs/plugin-react": "^4.1.0",
    "eslint": "^8.53.0",
    "eslint-plugin-react": "^7.33.2",
    "eslint-plugin-react-hooks": "^4.6.0",
    "eslint-plugin-react-refresh": "^0.4.4",
    "vite": "^5.0.0"
  }
}
EOF

# Update Vite config for proxy
cat > vite.config.js << 'EOF'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
EOF

# Update the App.jsx to use relative URLs (since we have proxy)
sed -i "s|const API_BASE = 'http://localhost:5000/api';|const API_BASE = '/api';|g" src/App.jsx 2>/dev/null || \
perl -i -pe "s|const API_BASE = 'http://localhost:5000/api';|const API_BASE = '/api';|g" src/App.jsx

# Create a README for the frontend
cat > README.md << 'EOF'
# Employee Claims Frontend

A React application for managing employee claims.

## Features

- Create new employee claims
- View recent claims
- Real-time status updates
- Responsive design

## Development

```bash
npm install
npm run dev
```

The app will be available at http://localhost:5173

## Building

```bash
npm run build
```

## API Integration

The frontend connects to the Employee.Api service running on port 5000. During development, Vite proxy is used to handle CORS.
EOF

cd .. # Back to employee-system root

echo "✅ React frontend created"