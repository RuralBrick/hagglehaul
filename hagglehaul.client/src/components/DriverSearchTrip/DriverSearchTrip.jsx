import React, { useState } from 'react';
import { BsSearch } from 'react-icons/bs'; // Make sure this import is at the top of your file
import './DriverSearchTrip.css';

function DriverSearchTrip({ onSearch }) {
    const [searchTerm, setSearchTerm] = useState('');

    const handleSearch = (event) => {
        setSearchTerm(event.target.value);
        if (onSearch) {
            onSearch(event.target.value);
        }
    };

    return (
        <div className="driver-search-trip">
            <BsSearch className="search-icon" /> {/* Ensure this line is included */}
            <input
                type="text"
                placeholder="Search trips..."
                value={searchTerm}
                onChange={handleSearch}
                className="search-input"
            />
        </div>
    );
}

export default DriverSearchTrip;
