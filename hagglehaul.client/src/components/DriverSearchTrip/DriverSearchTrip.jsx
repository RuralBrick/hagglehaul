// DriverSearchTrip.jsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import './DriverSearchTrip.css';

function DriverSearchTrip() {
    const navigate = useNavigate();

    const handleSearchClick = () => {
        navigate('/search-trips'); // Navigates to the SearchTripsPage
    };

    return (
        <button onClick={handleSearchClick} className="search-button">
            Search Trips
        </button>
    );
}

export default DriverSearchTrip;
