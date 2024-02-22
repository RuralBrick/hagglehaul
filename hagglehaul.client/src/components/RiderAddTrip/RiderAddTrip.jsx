import React, { useState } from 'react';
import PropTypes from 'prop-types'
import 'bootstrap/dist/css/bootstrap.min.css';
import './RiderAddTrip.css';
import AddressSearchBar from "@/components/AddressSearchBar/AddressSearchBar.jsx";
import DatePickerComponent from '@/components/DatePicker/DatePicker.jsx'; 


async function addTrip(addressData) {
    return fetch('/api/Rider/addTrip', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(addressData)
    })
        .then(data => data.json())
}

const errorAddingTripMessage = "Something went wrong. Please try again.";
const successAddingTripMessage = "Trip submitted. Thank you!";

function RiderAddTrip() {
    const [origin, setOrigin] = useState();
    const [destination, setDestination] = useState();
    const [statusMessage, setStatusMessage] = useState("");
    const [date, setDate] = useState(new Date()); // State for selected date and time

    const handleTripSubmit = async e => {

        e.preventDefault();

        if (!origin || !destination  || !date) return;

        const result = await addTrip ({
            origin,
            destination,
            date,
        });

        if (!result) {
            setStatusMessage(errorAddingTripMessage);
            return;
        }

        else {
            setStatusMessage(successAddingTripMessage);
            setOrigin("");
            setDestination("");
            return;
        }

    }

    return (
        <div className="map-wrapper mt-3">
            <p className="trip-status-message">{statusMessage}</p>
            <form onSubmit={() =>{}}>
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
                    <button type="submit" onClick={handleTripSubmit} className="custom-button">Submit</button>
                </div>  
            </form>
        </div>

    );

}

export default RiderAddTrip;
