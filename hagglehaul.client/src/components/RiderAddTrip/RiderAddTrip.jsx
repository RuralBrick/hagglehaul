import React, { useState } from 'react';
import PropTypes from 'prop-types'
import 'bootstrap/dist/css/bootstrap.min.css';
import './RiderAddTrip.css';
import AddressSearchBar from "@/components/AddressSearchBar/AddressSearchBar.jsx";
import PassengerDropdown from "@/components/PassengerDropdown/PassengerDropdown.jsx"; // Ensure this is correctly imported
import DatePickerComponent from '@/components/DatePicker/DatePicker.jsx'; 


async function addTrip(tripData) {
    return fetch('/api/Rider/addTrip', {
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
    const [date, setDate] = useState(new Date()); // State for selected date and time
    
    const handleTripSubmit = async e => {

        e.preventDefault();

        // Validate origin, destination, and passengers
        if (!origin || !destination || !passengers || !date) {
            setStatusMessage("Please fill in all fields.");
            return;
        }

        const result = await addTrip ({
            origin,
            destination,
            date,
            passengers // Include the number of passengers in the trip data
        });

        if (!result) {
            setStatusMessage(errorAddingTripMessage);
            return;
        }

        else {
            setStatusMessage(successAddingTripMessage);
            setOrigin("");
            setDestination("");
            setPassengers("");
            return;
        }

    }

    return (
        <div className="map-wrapper mt-3">
            <p className="trip-status-message">{statusMessage}</p>
            <form onSubmit={() =>{handleTripSubmit}}>
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
                        <p>Select Date and Time:</p>
                        <DatePickerComponent selectedDate={date} onChange={setDate} />
                    </label>
                </div>

                
                <br/>
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
