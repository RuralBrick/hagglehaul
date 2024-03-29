import React, { useState } from 'react';
import Cookies from 'js-cookie';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import LoginRegPage from './pages/LoginRegPage/LoginRegPage';
import RiderTripsPage from './pages/RiderTripsPage/RiderTripsPage';
import DriverTripsPage from './pages/DriverTripsPage/DriverTripsPage';
import ProfilePage from './pages/ProfilePage/ProfilePage';
import ActivityPage from './pages/ProfilePage/ActivityPage/ActivityPage';
import WalletPage from './pages/ProfilePage/WalletPage/WalletPage';
import MessagesPage from './pages/ProfilePage/MessagesPage/MessagesPage';
import SettingsPage from './pages/ProfilePage/SettingsPage/SettingsPage';
import RiderAddTrip from './components/RiderAddTrip/RiderAddTrip';
import SearchTripsPage from './pages/SearchTripsPage/SearchTripsPage';
import './App.css';

export const TokenContext = React.createContext({token: null, role: null});
function App() {
    const [token, setToken] = useState(Cookies.get('token'));
    const [role, setRole] = useState(Cookies.get('role'));

    const fetchRole = async (token) => {
        const m_results = await fetch('/api/Authentication/role', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            }
        })
            .then(data => data.text());
        Cookies.set('role', m_results, { expires: 0.125, secure: true });
        setRole(m_results);
    }

    async function setTokenWithCookie(token)
    {
        await fetchRole(token);
        Cookies.set('token', token, { expires: 0.125, secure: true });
        setToken(token);
    }
    
    function removeTokenWithCookie()
    {
        Cookies.remove('token');
        setToken(null);
        Cookies.remove('role');
        setRole(null);
    }
    
    if (!token) {
        return <LoginRegPage setToken={setTokenWithCookie} />
    }

    return (
        <Router>
            <div>
                <TokenContext.Provider value={{token: token, role: role}}>
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
                                    <button className="nav-link" onClick={removeTokenWithCookie}>Sign Out</button>
                                    {/* ... other navigation links */}
                                </div>
                            </div>
                        </div>
                    </nav>
                    <main className="pt-2 hh-app-flex">
                        <Routes>
                            <Route path="/" element={role === "rider" ? <RiderTripsPage /> : <DriverTripsPage />} />
                            <Route path="/trips" element={role === "rider" ? <RiderTripsPage /> : <DriverTripsPage />} />
                            <Route path="/search-trips" element={<SearchTripsPage />} />
                            <Route path="/rider-add-trip" element={<RiderAddTrip />} />
                            <Route path="/profile" element={<ProfilePage />} />
                            <Route path="/profile/activity" element={<ActivityPage />} />
                            <Route path="/profile/wallet" element={<WalletPage />} />
                            <Route path="/profile/messages" element={<MessagesPage />} />
                            <Route path="/profile/settings" element={<SettingsPage />} />
                            {/* ... other routes */}
                        </Routes>
                    </main>
                </TokenContext.Provider>
            </div>
        </Router>
    );
}

export default App;
