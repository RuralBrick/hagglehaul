import React from 'react';
import { useNavigate } from 'react-router-dom';
import './DriverSearchTrip.css';

// Component for searching trips as a driver
function DriverSearchTrip() {
    const navigate = useNavigate();

    // Function to handle search button click
    const handleSearchClick = () => {
        navigate('/search-trips'); // Navigates to the SearchTripsPage
    };

    // Render search button component
    return (
        <button onClick={handleSearchClick} className="search-button">
            Search Trips
        </button>
    );
}

export default DriverSearchTrip;
