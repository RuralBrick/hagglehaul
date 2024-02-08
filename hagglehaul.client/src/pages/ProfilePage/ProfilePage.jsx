// src/pages/ProfilePage/ProfilePage.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import './ProfilePage.css';

function ProfilePage() {
    return (
        <div className="profile-page">
            <div className="container text-center">
                <h1>Welcome, Jordan</h1>
                <div className="profile-menu mt-5">
                    <Link to="/profile/activity" className="profile-menu-item">
                        <h3>Activity</h3>
                    </Link>
                    <Link to="/profile/wallet" className="profile-menu-item">
                        <h3>Wallet</h3>
                    </Link>
                    <Link to="/profile/messages" className="profile-menu-item">
                        <h3>Messages</h3>
                    </Link>
                    <Link to="/profile/settings" className="profile-menu-item">
                        <h3>Settings</h3>
                    </Link>
                </div>
            </div>
        </div>
    );
}

export default ProfilePage;
