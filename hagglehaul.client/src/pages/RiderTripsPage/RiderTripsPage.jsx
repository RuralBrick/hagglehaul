import React, {useEffect, useState} from 'react';
import './RiderTripsPage.css';
import TripCard from "@/components/TripCard/TripCard.jsx";
import {DropdownButton, ButtonGroup, Dropdown, Button, Row, Col, Modal, Placeholder} from "react-bootstrap";
import AddTripModal from "@/components/AddTripModal/AddTripModal.jsx";
import {TokenContext} from "@/App.jsx";
import CustomDateFormatter from "@/utils/CustomDateFormatter.jsx";
import MetersToMiles from "@/utils/MetersToMiles.jsx";
import SecondsToMinutes from "@/utils/SecondsToMinutes.jsx";
import CancellationModal from "@/components/CancellationModal/CancellationModal.jsx";
import SelectBidModal from "@/components/SelectBidModal/SelectBidModal.jsx";

function RiderTripsPage() {
    const [showAddTrip, setShowAddTrip] = useState(false);
    const [data, setData] = useState();
    const {token, role} = React.useContext(TokenContext);
    
    // Modals
    const [showInfoModal, setShowInfoModal] = useState(false);
    const [infoModalData, setInfoModalData] = useState([]);
    const infoModal = (<Modal show={showInfoModal} onHide={() => {setShowInfoModal(false)}} centered>
        <Modal.Header closeButton>Trip Detail</Modal.Header>
        <Modal.Body>
            {infoModalData.map((item) =>
                <Row style={{marginBottom: '10px'}}>
                    <Col><span style={{fontSize: "1.1em"}}>{item[0]}</span></Col>
                    <Col>{item[1]}</Col>
                </Row>
            )}
        </Modal.Body>
        <Modal.Footer>
            <Button style={{backgroundColor: "#D96C06"}} onClick={() => {setShowInfoModal(false)}}>Close</Button>
        </Modal.Footer>
    </Modal>);
    
    const [showCancellationModal, setShowCancellationModal] = useState(false);
    const [cancellationId, setCancellationId] = useState();
    
    const [showSelectBidModal, setShowSelectBidModal] = useState(false);
    const [selectBidData, setSelectBidData] = useState();

    const [error, setError] = useState();
    const errorModal = (
        <Modal show={true} onHide={() => { window.location.reload()}} centered>
            <Modal.Header closeButton>Error</Modal.Header>
            <Modal.Body>
                An error occurred: <br />
                {error}
            </Modal.Body>
            <Modal.Footer>
                <Button style={{backgroundColor: "#D96C06"}} onClick={() => { window.location.reload()}}>Reload</Button>
            </Modal.Footer>
        </Modal>);

    // Initial fetch
    useEffect(() => {
        fetch('/api/Rider/dashboard', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            }
        }).then(async (response) => {
                var data = await response.text();
                try {
                     data = JSON.parse(data);
                } catch (err) {
                    setError("Something went wrong (" + response.status + "). Please try again.");
                    return;
                }
                if (!response.ok) {
                    setError(data.error);
                    return;
                } else {
                    setData(data);
                }
            }
        );
    }, []);
    
    if (error) {
        return errorModal;
    }
    
    if (!data) {
        return (
            <div className="trips-page container mt-5">
                <Row xs={1} md={2} lg={1}>
                    {Array(3).fill().map(() =>                     
                        <TripCard
                        image=""
                        title={<Placeholder animation="wave"><Placeholder xs={8} /></Placeholder>}
                        actionComponent={<Placeholder animation="glow"><Placeholder.Button style={{backgroundColor: "darkgray"}} xs={4} /></Placeholder>}
                        attributes={Array.from(Array(3), _ => Array(2).fill(<Placeholder animation="wave"><Placeholder xs={8} /></Placeholder>))}
                        bidComponents={[]}
                    />)}
                </Row>
            </div>
        )
    }

    return (
        <>
            {showInfoModal ? infoModal : null}
            {showCancellationModal ? <CancellationModal show={showCancellationModal} setShow={setShowCancellationModal} cancellationId={cancellationId} setError={setError} /> : null}
            {showSelectBidModal ? <SelectBidModal show={showSelectBidModal} setShow={setShowSelectBidModal} setError={setError} selectBidData={selectBidData} /> : null}
            
            <AddTripModal show={showAddTrip} setShow={setShowAddTrip} />
            <div className="trips-page container mt-5">
                <div className="trips-header mb-4">
                    <h2>Confirmed Trips</h2>
                    <Button onClick={() => {
                        setShowAddTrip(true)
                    }} className="btn-add-trip">+ Add Trip</Button>
                </div>
                {
                    (!data?.confirmedTrips || data.confirmedTrips.length === 0) ?
                        <div>
                            <h5>No confirmed trips so far</h5>
                        </div>
                        :
                        <Row xs={1} md={2} lg={1}>
                            {data.confirmedTrips.map((trip) =>
                                <TripCard
                                    image={"data:image/png;base64," + trip.thumbnail}
                                    title={trip.tripName}
                                    actionComponent={<DropdownButton as={ButtonGroup} title="Options" id="bg-nested-dropdown"
                                                                     variant="light">
                                        <Dropdown.Item onClick={() => {
                                            setCancellationId(trip.tripID);
                                            setShowCancellationModal(true);
                                        }}>
                                            Cancel Trip
                                        </Dropdown.Item>
                                        <Dropdown.Item onClick={() => {
                                            setInfoModalData([["Driver Email:", trip.driverEmail], ["Driver Phone:", trip.driverPhone],
                                            ["Driver Vehicle:", trip.driverCarModel], ["Pickup Address:", trip.pickupAddress],
                                            ["Destination Address:", trip.destinationAddress]]);
                                            setShowInfoModal(true)
                                        }}>
                                            Show More
                                        </Dropdown.Item>
                                    </DropdownButton>}
                                    attributes={[[CustomDateFormatter(trip.startTime), MetersToMiles(trip.distance) + " miles - " + SecondsToMinutes(trip.duration) + " minutes"], 
                                        [trip.driverName, trip.driverRating?.toFixed(2) + "\u2605 (" + trip.driverNumRating + ")"], 
                                        ["$" + (trip.cost / 100)?.toFixed(2), ""]]}
                                    bidComponents={trip.showRatingPrompt ? [
                                        <Row>
                                            <Col style={{ display: 'flex', justifyContent: 'center', fontSize: "1.5em" }}>
                                                Rate Driver: {Array(1,2,3,4,5).map((x) =>
                                                <span style={{cursor: "pointer"}}>&#x2606;</span>
                                            )}
                                            </Col>
                                        </Row>] : []}
                                />
                            )}
                        </Row>
                }

                <div className="trips-header mt-4 mb-4">
                    <h2>Trips in Bidding</h2>
                </div>
                {
                    (!data?.tripsInBidding || data.tripsInBidding.length === 0) ?
                        <div>
                            <h5>No trips in bidding so far</h5>
                        </div>
                        :
                        <Row xs={1} md={2} lg={1}>
                            {data.tripsInBidding.map((trip) =>
                                <TripCard
                                    image={"data:image/png;base64," + trip.thumbnail}
                                    title={trip.tripName}
                                    actionComponent={<Button style={{ backgroundColor: "#D96C06" }} onClick={() => {
                                        setCancellationId(trip.tripID);
                                        setShowCancellationModal(true);
                                    }}>
                                        Cancel Trip
                                    </Button>}
                                    attributes={[[CustomDateFormatter(trip.startTime), MetersToMiles(trip.distance) + " miles - " + SecondsToMinutes(trip.duration) + " minutes"], 
                                        ["Pickup:", trip.pickupAddress], ["Destination:", trip.destinationAddress]]}
                                    bidComponents={
                                        trip.bids.length === 0 ? 
                                            [<p style={{ display: 'flex', justifyContent: 'center'}}>No bids yet, hang tight!</p>]
                                        :
                                            trip.bids.map((bid) =>
                                                <Row>
                                                    <Col> {bid.driverName} <br /> {bid.driverRating?.toFixed(2) + "\u2605 (" + bid.driverNumRating + ")"} </Col>
                                                    <Col> <Button className="btn-add-trip" onClick={() => {
                                                        setSelectBidData({tripId: trip.tripID, bidId: bid.bidId});
                                                        setShowSelectBidModal(true);
                                                    }}>
                                                        ${(bid.cost / 100)?.toFixed(2)}
                                                    </Button></Col>
                                                </Row>
                                            )
                                    }
                                />
                            )}
                        </Row>
                }

                <div className="trips-header mt-4 mb-4">
                    <h2>Archived Trips</h2>
                </div>
                {
                    (!data?.archivedTrips || data.archivedTrips.length === 0) ?
                        <div>
                            <h5>No archived trips so far</h5>
                        </div>
                        :
                        <Row xs={1} md={2} lg={1}>
                            {data.archivedTrips.map((trip) =>
                                <TripCard
                                    image={"data:image/png;base64," + trip.thumbnail}
                                    title={trip.tripName}
                                    actionComponent={""}
                                    attributes={trip.hasDriver ? 
                                        [[CustomDateFormatter(trip.startTime), MetersToMiles(trip.distance) + " miles - " + SecondsToMinutes(trip.duration) + " minutes"],
                                        [trip.driverName, trip.driverRating?.toFixed(2) + "\u2605 (" + trip.driverNumRating + ")"],
                                        ["$" + (trip.cost / 100)?.toFixed(2), ""]]
                                        :
                                        [[CustomDateFormatter(trip.startTime), MetersToMiles(trip.distance) + " miles - " + SecondsToMinutes(trip.duration) + " minutes"], 
                                        ["", ""], ["No driver found", ""]]
                                    }
                                    bidComponents={[]}
                                />
                            )}
                        </Row>
                }
            </div>
        </>
    );
}

export default RiderTripsPage;