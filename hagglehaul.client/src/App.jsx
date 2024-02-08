import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import LoginRegPage from './pages/LoginRegPage/LoginRegPage';
import TripsPage from './pages/TripsPage/TripsPage';
import ProfilePage from './pages/ProfilePage/ProfilePage';
import ActivityPage from './pages/ProfilePage/ActivityPage/ActivityPage';
import WalletPage from './pages/ProfilePage/WalletPage/WalletPage';
import MessagesPage from './pages/ProfilePage/MessagesPage/MessagesPage';
import SettingsPage from './pages/ProfilePage/SettingsPage/SettingsPage';
import Signin from './Signin';  
import Signup from './Signup';  
import './App.css';

function App() {

    const [token, setToken] = useState();

    if (!token) {
        return <LoginRegPage setToken={setToken} />
    }

    return (
        <Router>
            <div>
                <nav className="navbar navbar-expand-lg navbar-light bg-light fixed-top">
                    <div className="container-fluid">
                        <Link className="navbar-brand" to="/" style={{ fontFamily: 'Inika' }}>HaggleHaul</Link>
                        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNavAltMarkup" aria-controls="navbarNavAltMarkup" aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <div className="collapse navbar-collapse" id="navbarNavAltMarkup">
                            <div className="navbar-nav ms-auto">
                                <Link className="nav-link" to="/trips">My Trips</Link>
                                <Link className="nav-link" to="/profile">Profile</Link>
                                {/* ... other navigation links */}
                            </div>
                        </div>
                    </div>
                </nav>

                <main className="pt-5">
                    <Routes>
                        <Route path="/" element={<TripsPage />} />
                        <Route path="/trips" element={<TripsPage />} />
                        <Route path="/profile" element={<ProfilePage />} />
                        <Route path="/profile/activity" element={<ActivityPage />} />
                        <Route path="/profile/wallet" element={<WalletPage />} />
                        <Route path="/profile/messages" element={<MessagesPage />} />
                        <Route path="/profile/settings" element={<SettingsPage />} />
                        <Route path="/signin" element={<Signin />} /> 
                        <Route path="/signup" element={<Signup />} /> 
          
                        {/* ... other routes */}
                    </Routes>
                </main>
            </div>
        </Router>
    );
}

export default App;
