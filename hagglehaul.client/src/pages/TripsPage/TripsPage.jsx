import React, {useState} from 'react';
import { Link } from 'react-router-dom'; // Import Link from react-router-dom
import './TripsPage.css';
import AddTripModal from "@/components/AddTripModal/AddTripModal.jsx";
import {Button} from "react-bootstrap";

function TripsPage() {
    const [showAddTrip, setShowAddTrip] = useState(false);
    return (
        <>
            <AddTripModal show={showAddTrip} setShow={setShowAddTrip} />
            <div className="trips-page container mt-5">
                <div className="trips-header mb-4">
                    <h2>Confirmed Trips</h2>
                    <Button onClick={() => {setShowAddTrip(true)}} className="btn-add-trip">+ Add Trip</Button>
                </div>
                <div className="card mb-3">
                    <div className="card-body">
                        <h5 className="card-title">Riverside Gaming Lab</h5>
                        <h6 className="card-subtitle mb-2 text-muted">3633 Market St, Riverside, CA 92501</h6>
                        <p className="card-text">1/28/2024, 5:22 PM - $23.78</p>
                    </div>
                </div>

                <h2 className="mb-4">Trips in Bidding</h2>
                <div className="card mb-3">
                    <div className="card-body">
                        <h5 className="card-title">Disneyland Park</h5>
                        <h6 className="card-subtitle mb-2 text-muted">Anaheim, CA 92802</h6>
                        <div className="card-text">
                            <div className="row">
                                <div className="col-sm">
                                    <p>Driver: Will Smith</p>
                                    <p>Rating: 4.83</p>
                                </div>
                                <div className="col-sm">
                                    <p>Time: 2/03/2024, 6:32 AM</p>
                                    <p>Price: $76.93</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                {/* ... Add more cards for each trip */}
            </div>
        </>

    );
}

export default TripsPage;
