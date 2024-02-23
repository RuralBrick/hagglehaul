// DriverSearchTrip.jsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { BsSearch } from 'react-icons/bs';
import './DriverSearchTrip.css';

function DriverSearchTrip() {
    const navigate = useNavigate();

    const handleSearchClick = () => {
        navigate('/search-trips'); // Navigates to the SearchTripsPage
    };

    return (
        <button onClick={handleSearchClick} className="search-button">
            <BsSearch />
            Search Trips
        </button>
    );
}

export default DriverSearchTrip;
