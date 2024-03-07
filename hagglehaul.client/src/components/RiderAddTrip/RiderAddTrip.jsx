import React, { useContext, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import './RiderAddTrip.css';
import AddressSearchBar from "@/components/AddressSearchBar/AddressSearchBar.jsx";
import PassengerDropdown from "@/components/PassengerDropdown/PassengerDropdown.jsx"; // Ensure this is correctly imported
import DatePickerComponent from '@/components/DatePicker/DatePicker.jsx';
import { TokenContext } from "@/App.jsx";
import { Spinner } from "react-bootstrap";

const errorAddingTripMessage = "Something went wrong. Please try again.";

Date.prototype.addHours = function(h) {
    this.setTime(this.getTime() + (h*60*60*1000));
    return this;
}

function RiderAddTrip() {
    const { token, role } = useContext(TokenContext);
    const [origin, setOrigin] = useState();
    const [destination, setDestination] = useState();
    const [originText, setOriginText] = useState();
    const [destinationText, setDestinationText] = useState();
    const [passengers, setPassengers] = useState(""); // State for tracking the number of passengers
    const [statusMessage, setStatusMessage] = useState("");
    const [date, setDate] = useState(new Date().addHours(168)); // State for selected date and time
    const [waiting, setWaiting] = useState(false);

    // Function to add a trip
    async function addTrip(tripData) {
        return await fetch('/api/Rider/trip', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            },
            body: JSON.stringify(tripData)
        })
            .then(async (response) => {
                if (!response.ok) {
                    try {
                        return JSON.parse(await response.text());
                    } catch (err) {
                        return { error: "Something went wrong." };
                    }
                }
                return { success: "success" };
            });
    }

    // Function to handle trip submission
    const handleTripSubmit = async e => {
        e.preventDefault();
        setWaiting(true);

        if (role !== "rider") {
            setStatusMessage("You are not authorized to add a trip.");
            return;
        }
        // Validate origin, destination, and passengers
        if (!origin || !originText || !destination || !destinationText || !passengers || !date) {
            setStatusMessage("Please fill in all fields.");
            return;
        }

        const result = await addTrip ({
            name: destinationText.summary,
            startTime: date.toISOString(),
            pickupLong: origin[0],
            pickupLat: origin[1],
            destinationLong: destination[0],
            destinationLat: destination[1],
            pickupAddress: originText.address,
            destinationAddress: destinationText.address,
            partySize: passengers
        });

        if (result.success) {
            window.location.href = "/";
        }
        else {
            setStatusMessage(result?.error ? result.error : errorAddingTripMessage);
        }
        setWaiting(false);
    }

    return (
        <div className="map-wrapper mt-3">
            <p className="trip-status-message">{statusMessage}</p>
            <form onSubmit={handleTripSubmit}>
                <div>
                    <label>
                        <p>Please Enter Your Origin: </p>
                        <AddressSearchBar setCoordinates={setOrigin} setAddressText={setOriginText}/>
                    </label>
                </div>
                <div>
                    <label>
                        <p>Please Enter Your Destination: </p>
                        <AddressSearchBar setCoordinates={setDestination} setAddressText={setDestinationText}/>
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
                {waiting ?
                    <Spinner animation="border" role="status" style={{ color: "#D96C06" }}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <button type="submit">Submit</button>
                }
            </form>
        </div>
    );

}

export default RiderAddTrip;
