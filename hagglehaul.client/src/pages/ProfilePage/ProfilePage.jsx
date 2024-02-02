// src/pages/Profile/ProfilePage.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import './ProfilePage.css';
function ProfilePage() {
    return (
        <div className="profile-page">
            <div className="container text-center">
                <h1>Welcome, Jordan</h1>
                <div className="profile-menu mt-5">
                    <Link to="/activity" className="profile-menu-item">
                        <h3>Activity</h3>
                    </Link>
                    <Link to="/wallet" className="profile-menu-item">
                        <h3>Wallet</h3>
                    </Link>
                    <Link to="/messages" className="profile-menu-item">
                        <h3>Messages</h3>
                    </Link>
                    <Link to="/settings" className="profile-menu-item">
                        <h3>Settings</h3>
                    </Link>
                </div>
            </div>
        </div>
    );
}

export default ProfilePage;
