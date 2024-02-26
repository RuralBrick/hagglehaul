import React, { useState } from 'react';
import './DriverTripsPage.css';
import TripCard from "@/components/TripCard/TripCard.jsx";
import { DropdownButton, ButtonGroup, Dropdown, Button, Row, Col } from "react-bootstrap";
import AddTripModal from "@/components/AddTripModal/AddTripModal.jsx";

function DriverTripsPage() {
    const [showAddTrip, setShowAddTrip] = useState(false);

    return (
        <>
            <div className="trips-page container mt-5">
                <br />
                <div className="trips-header mb-4">
                    <h2>Confirmed Trips</h2>
                </div>
                <br />
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
                        actionComponent={<Button style={{ backgroundColor: "#D96C06" }}>Go somewhere</Button>}
                        attributes={[["Column 1", "Some longer information is here :)"], ["Column 2", "Information 2"], ["$39.99", "10.2 miles"]]}
                        bidComponents={[
                            <Row>
                                <Col>Some left aligned components</Col>
                                <Col className={"text-end"}>Some right aligned components</Col>
                            </Row>,
                            <Row>
                                <Col>Some left aligned components</Col>
                                <Col className={"text-end"}>Some right aligned components</Col>
                            </Row>,
                            <Button variant="light">+ Add Bid</Button>]}
                    />
                </Row>
            </div>
        </>
    );
}

export default DriverTripsPage;