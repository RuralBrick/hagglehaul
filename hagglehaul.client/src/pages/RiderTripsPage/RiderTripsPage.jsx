import React, { useState } from 'react';
import './RiderTripsPage.css';
import TripCard from "@/components/TripCard/TripCard.jsx";
import { DropdownButton, ButtonGroup, Dropdown, Button, Row, Col } from "react-bootstrap";
import AddTripModal from "@/components/AddTripModal/AddTripModal.jsx";

function RiderTripsPage() {
    const [showAddTrip, setShowAddTrip] = useState(false);

    return (
        <>
            <AddTripModal show={showAddTrip} setShow={setShowAddTrip} />
            <div className="trips-page container mt-5">
                <div className="trips-header mb-4">
                    <h2>Confirmed Trips</h2>
                    <Button onClick={() => {
                        setShowAddTrip(true)
                    }} className="btn-add-trip">+ Add Trip</Button>
                </div>
                <Row xs={1} md={2} lg={1}>
                    <TripCard
                        image="https://placeholder.co/600x400.png"
                        title="Disneyland Park"
                        actionComponent={<DropdownButton as={ButtonGroup} title="Options" id="bg-nested-dropdown"
                            variant="light">
                            <Dropdown.Item>Cancel Trip</Dropdown.Item>
                            <Dropdown.Item>Show More</Dropdown.Item>
                        </DropdownButton>}
                        attributes={[["Mon, Feb 19, 08:00 AM", "12 miles - 36 minutes"], ["Tim Pajim", "4.5\u2605 (130)"], ["$39.99", ""]]}
                        bidComponents={[
                            <Row>
                                <Col
                                     style={{ display: 'flex', justifyContent: 'center' }}> <span style={{ fontSize: "1.5em" }}>Rate Driver: &#x2606;&#x2606;&#x2606;&#x2606;&#x2606; </span>
                                </Col>
                            </Row>]}
                    />
                </Row>
                <div className="trips-header mt-4 mb-4">
                    <h2>Trips in Bidding</h2>
                </div>
                <Row xs={1} md={2} lg={1}>
                    <TripCard
                        image="https://placeholder.co/600x400.png"
                        title="Disneyland Park"
                        actionComponent={<Button style={{ backgroundColor: "#D96C06" }}>Cancel Trip</Button>}
                        attributes={[["Mon, Feb 19, 08:00 AM", "12 miles - 36 minutes"], ["Pickup:", "1234 Virginia Ave."], ["Destination:", "4567 Disney Rd."]]}
                        bidComponents={[
                            <Row>
                                <Col> Tim Pajim <br /> 4.5&#9733; (130) </Col>
                                <Col> <Button onClick={() => { setShowAddTrip(true) }} className="btn-add-trip"> $Price </Button> </Col>
                            </Row>,
                            <Row>
                                <Col> Heavy but FREAKY <br /> 2.2&#9733; (34565)</Col>
                                <Col> <Button onClick={() => { setShowAddTrip(true) }} className="btn-add-trip"> $Price </Button> </Col>
                            </Row>]}
                    />
                </Row>
                <div className="trips-header mt-4 mb-4">
                    <h2>Archived Trips</h2>
                </div>
                <Row xs={1} md={2} lg={1}>
                    <TripCard
                        image="https://placeholder.co/600x400.png"
                        title="Disneyland Park"
                        actionComponent={""}
                        attributes={[["Mon, Feb 19, 08:00 AM", "12 miles - 36 minutes"], ["Tim Pajim", "4.5\u2605 (130)"], ["$39.99", ""]]}
                        bidComponents={[]}
                    />
                </Row>
            </div>
        </>
    );
}

export default RiderTripsPage;