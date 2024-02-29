import React, {useEffect, useState} from 'react';
import './DriverTripsPage.css';
import TripCard from "@/components/TripCard/TripCard.jsx";
import {DropdownButton, ButtonGroup, Dropdown, Button, Row, Col, Modal, Placeholder} from "react-bootstrap";
import AddTripModal from "@/components/AddTripModal/AddTripModal.jsx";
import {TokenContext} from "@/App.jsx";
import TripMapModal from "@/components/TripMapModal/TripMapModal.jsx";
import CustomDateFormatter from "@/utils/CustomDateFormatter.jsx";
import MetersToMiles from "@/utils/MetersToMiles.jsx";
import SecondsToMinutes from "@/utils/SecondsToMinutes.jsx";
import ModifyBidModal from "@/components/ModifyBidModal/ModifyBidModal.jsx";
import WithdrawBidModal from "@/components/WithdrawBidModal/WithdrawBidModal.jsx";
import DriverLaunchModal from "@/components/DriverLaunchModal/DriverLaunchModal.jsx";
import DriverSearchTrip from "@/components/DriverSearchTrip/DriverSearchTrip.jsx";

function DriverTripsPage() {
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
    
    const [showMapModal, setShowMapModal] = useState(false);
    const [mapGeoJSON, setMapGeoJSON] = useState();
    
    const [showModifyBidModal, setShowModifyBidModal] = useState(false);
    const [modifyBidTripId, setModifyBidTripId] = useState();
    
    const [showWithdrawBidModal, setShowWithdrawBidModal] = useState(false);
    const [withdrawBidTripId, setWithdrawBidTripId] = useState();
    
    const [showDriverLaunchModal, setShowDriverLaunchModal] = useState(false);
    const [driverLaunchModalData, setDriverLaunchModalData] = useState({riderPhone: "", riderEmail: "", geoJSON: {}});
    
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
        fetch('/api/Driver/dashboard', {
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
            {showMapModal ? <TripMapModal show={showMapModal} setShow={setShowMapModal} mapGeoJSON={mapGeoJSON} /> : null}
            {showInfoModal ? infoModal : null}
            {showModifyBidModal ? <ModifyBidModal show={showModifyBidModal} setShow={setShowModifyBidModal} setError={setError} tripId={modifyBidTripId} /> : null}
            {showWithdrawBidModal ? <WithdrawBidModal show={showWithdrawBidModal} setShow={setShowWithdrawBidModal} setError={setError} tripId={withdrawBidTripId} /> : null}
            {showDriverLaunchModal ? <DriverLaunchModal show={showDriverLaunchModal} setShow={setShowDriverLaunchModal} {...driverLaunchModalData} /> : null}

            <div className="trips-page container mt-5">
                <br/>
                <div className="trips-header d-flex justify-content-between align-items-center mb-4">
                    <div style={{flex: 1}}></div>
                    <h2 style={{flex: 1, textAlign: 'center'}}>Confirmed Trips</h2>
                    <div style={{flex: 1, textAlign: 'right'}}>
                        <DriverSearchTrip/>
                    </div>
                </div>
                <br/>
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
                                    onClickImg={() => {
                                        setMapGeoJSON(JSON.parse(trip.geoJson));
                                        setShowMapModal(true);
                                    }}
                                    title={trip.tripName}
                                    actionComponent={<Button variant="light" onClick={() => {
                                        setInfoModalData([["Rider Email:", trip.riderEmail], ["Rider Phone:", trip.riderPhone],
                                            ["Pickup Address:", trip.pickupAddress], ["Destination Address:", trip.destinationAddress]]);
                                        setShowInfoModal(true)
                                    }}>
                                        Show More
                                    </Button>}
                                    attributes={[[CustomDateFormatter(trip.startTime), MetersToMiles(trip.distance) + " miles - " + SecondsToMinutes(trip.duration) + " minutes"],
                                        [trip.riderName, trip.riderRating?.toFixed(2) + "\u2605 (" + trip.riderNumRating + ")"],
                                        ["$" + (trip.cost / 100)?.toFixed(2), ""]]}
                                    bidComponents={trip.showRatingPrompt ? [
                                        <Row>
                                            <Col style={{display: 'flex', justifyContent: 'center', fontSize: "1.5em"}}>
                                                Rate Rider: {Array(1, 2, 3, 4, 5).map((x) =>
                                                <span style={{cursor: "pointer"}}>&#x2606;</span>
                                            )}
                                            </Col>
                                        </Row>] : [
                                        <Button style={{width: "100%"}} variant="primary" onClick={() => {
                                            setDriverLaunchModalData({riderPhone: trip.riderPhone, riderEmail: trip.riderEmail, geoJSON: JSON.parse(trip.geoJson)});
                                            setShowDriverLaunchModal(true);
                                        }}>Launch Trip</Button>
                                    ]}
                                />
                            )}
                        </Row>
                }
                <div className="trips-header mt-4 mb-4">
                    <h2>My bids</h2>
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
                                    onClickImg={() => {
                                        setMapGeoJSON(JSON.parse(trip.geoJson));
                                        setShowMapModal(true);
                                    }}
                                    title={trip.tripName}
                                    actionComponent={""}
                                    attributes={[[CustomDateFormatter(trip.startTime), MetersToMiles(trip.distance) + " miles - " + SecondsToMinutes(trip.duration) + " minutes"],
                                        ["Pickup Address:", trip.pickupAddress], ["Destination Address:", trip.destinationAddress]]}
                                    bidComponents={trip.bids.map((bid) =>
                                        <>
                                            <Row style={bid.yourBid ? {backgroundColor: "#fef0e3", borderRadius: "10px"} : {}}>
                                                <Col> {bid.driverName} {bid.yourBid ? "(me)" : null} <br/> {bid.driverRating?.toFixed(2) + "\u2605 (" + bid.driverNumRating + ")"} </Col>
                                                <Col style={{display: 'flex', justifyContent: 'right'}}> {"$" + (bid.cost / 100)?.toFixed(2)} </Col>
                                            </Row>
                                            {bid.yourBid ? 
                                                <Row>
                                                    <DropdownButton as={ButtonGroup} title="Options"
                                                        id="bg-nested-dropdown"
                                                        variant="light">
                                                            <Dropdown.Item onClick={() => {setModifyBidTripId(trip.tripID); setShowModifyBidModal(true)}}>
                                                                Change Bid Amount
                                                            </Dropdown.Item>
                                                            <Dropdown.Item onClick={() => {setWithdrawBidTripId(trip.tripID); setShowWithdrawBidModal(true)}}>
                                                                Withdraw Bid
                                                            </Dropdown.Item>
                                                    </DropdownButton>
                                                </Row>
                                                :
                                                null
                                            }
                                        </>
                                    )}
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
                                onClickImg={() => {
                                    setMapGeoJSON(JSON.parse(trip.geoJson));
                                    setShowMapModal(true);
                                }}
                                title={trip.tripName}
                                actionComponent={""}
                                attributes={[[CustomDateFormatter(trip.startTime), MetersToMiles(trip.distance) + " miles - " + SecondsToMinutes(trip.duration) + " minutes"],
                                    [trip.riderName, trip.riderRating?.toFixed(2) + "\u2605 (" + trip.riderNumRating + ")"],
                                    ["$" + (trip.cost / 100)?.toFixed(2), ""]]}
                                bidComponents={[]}
                            />
                            )}
                        </Row>
                }
            </div>
        </>
    );
}

export default DriverTripsPage;