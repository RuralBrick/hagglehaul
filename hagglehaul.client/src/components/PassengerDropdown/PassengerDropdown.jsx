import React from 'react';
import './PassengerDropdown.css'; // Make sure this is correctly referenced

// Component for selecting the number of passengers
function PassengerDropdown({ selectedPassengers, onChange }) {
    return (
        <div className="passenger-dropdown">
            <label htmlFor="passenger-select">Number of Passengers:</label>
            <select
                id="passenger-select"
                value={selectedPassengers}
                onChange={e => onChange(e.target.value)}
                className="form-control"
            >
                <option value="">Select passengers</option>
                {[...Array(10).keys()].map(num => (
                    <option key={num + 1} value={num + 1}>
                        {num + 1}
                    </option>
                ))}
            </select>
        </div>
    );
}

export default PassengerDropdown;
