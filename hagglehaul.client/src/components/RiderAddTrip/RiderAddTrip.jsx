import React, { useState } from 'react';
import PropTypes from 'prop-types'
import 'bootstrap/dist/css/bootstrap.min.css';
import './RiderAddTrip.css';
import AddressSearchBar from "@/components/AddressSearchBar/AddressSearchBar.jsx";
import PassengerDropdown from "@/components/PassengerDropdown/PassengerDropdown.jsx"; // Ensure this is correctly imported

async function addTrip(tripData) {
    return fetch('api/Rider/addTrip', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(tripData)
    })
        .then(data => data.json())
}

const errorAddingTripMessage = "Something went wrong. Please try again.";
const successAddingTripMessage = "Trip submitted. Thank you!";

function RiderAddTrip() {
    const [origin, setOrigin] = useState();
    const [destination, setDestination] = useState();
    const [passengers, setPassengers] = useState(""); // State for tracking the number of passengers
    const [statusMessage, setStatusMessage] = useState("");

    const handleTripSubmit = async e => {
        e.preventDefault();

        // Validate origin, destination, and passengers
        if (!origin || !destination || !passengers) {
            setStatusMessage("Please fill in all fields.");
            return;
        }

        const result = await addTrip({
            origin,
            destination,
            passengers // Include the number of passengers in the trip data
        });

        if (!result) {
            setStatusMessage(errorAddingTripMessage);
            return;
        } else {
            setStatusMessage(successAddingTripMessage);
            setOrigin("");
            setDestination("");
            setPassengers(""); // Reset passengers state
            return;
        }
    }

    return (
        <div className="map-wrapper mt-3">
            <p className="trip-status-message">{statusMessage}</p>
            <form onSubmit={handleTripSubmit}> {/* Updated to use handleTripSubmit on form submission */}
                <div>
                    <label>
                        <p>Please Enter Your Origin: </p>
                        <AddressSearchBar setCoordinates={setOrigin}/>
                    </label>
                </div>
                <div>
                    <label>
                        <p>Please Enter Your Destination: </p>
                        <AddressSearchBar setCoordinates={setDestination}/>
                    </label>
                </div>
                <div>
                    <label>
                        <PassengerDropdown selectedPassengers={passengers} onChange={setPassengers}/>
                    </label>
                </div>
                <br/>
                <button type="submit" className="custom-button">Submit</button>
            </form>
        </div>
    );
}

export default RiderAddTrip;
