// src/App.jsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import TripsPage from './pages/TripsPage/TripsPage'; // Corrected import path
import ProfilePage from './pages/ProfilePage/ProfilePage'; // Corrected import path
import './App.css';

function App() {
    return (
        <Router>
            <div>
                <nav className="navbar navbar-expand-lg navbar-light bg-light">
                    <Link className="navbar-brand" to="/">HaggleHaul</Link>
                    <div>
                        <ul className="navbar-nav">
                            <li className="nav-item">
                                <Link className="nav-link" to="/trips">My Trips</Link>
                            </li>
                            <li className="nav-item">
                                <Link className="nav-link" to="/profile">Profile</Link>
                            </li>
                            {/* ... other navigation links */}
                        </ul>
                    </div>
                </nav>

                <Routes>
                    <Route path="/" element={<TripsPage />} />
                    <Route path="/trips" element={<TripsPage />} />
                    <Route path="/profile" element={<ProfilePage />} />
                    {/* ... other routes */}
                </Routes>
            </div>
        </Router>
    );
}

export default App;
