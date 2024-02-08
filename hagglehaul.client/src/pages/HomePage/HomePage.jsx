// src/pages/HomePage/HomePage.jsx
import React from 'react';
import './HomePage.css';

function HomePage() {
    return (
        <div className="home-page">
            <h2>Confirmed Trips</h2>
            {/* ... Add confirmed trips list here ... */}

            <h2 className="mt-4">Trips in Bidding</h2>
            {/* ... Add trips in bidding list here ... */}
        </div>
    );
}

export default HomePage;
