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
                        attributes={[["Mon, Feb 19, 08:00 AM", "12 miles - 36 minutes"], ["Jim Patim", "4.2\u2605 (122)"], ["$39.99", ""]]}
                        bidComponents={[
                            <Row>
                                <Col style={{ display: 'flex', justifyContent: 'center', fontSize: "1.5em" }}>
                                    Rate Rider: &#x2606;&#x2606;&#x2606;&#x2606;&#x2606;
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
                        actionComponent={<DropdownButton as={ButtonGroup} title="Options" id="bg-nested-dropdown"
                            variant="light">
                            <Dropdown.Item>Modify Bid</Dropdown.Item>
                            <Dropdown.Item>Withdraw Bid</Dropdown.Item>
                        </DropdownButton>}
                        attributes={[["Mon, Feb 19, 08:00 AM", "12 miles - 36 minutes"], ["Pickup:", "1234 Virginia Ave."], ["Destination:", "4567 Disney Rd."]]}
                        bidComponents={[
                            <Row>
                                <Col> Tim Pajim (me) <br /> 4.5&#9733; (130) </Col>
                                <Col style={{ display: 'flex', justifyContent: 'right' }}> $Price </Col>
                            </Row>,
                            <Row>
                                <Col> Heavy but FREAKY <br /> 2.2&#9733; (34565)</Col>
                                <Col style={{ display: 'flex', justifyContent: 'right' }}> $Price </Col>
                            </Row>]}
                    />
                    <div className="trips-header mt-4 mb-4">
                        <h2>Archived Trips</h2>
                    </div>
                    <Row xs={1} md={2} lg={1}>
                        <TripCard
                            image="https://placeholder.co/600x400.png"
                            title="Disneyland Park"
                            actionComponent={""}
                            attributes={[["Mon, Feb 19, 08:00 AM", "12 miles - 36 minutes"], ["Jim Patim", "4.2\u2605 (122)"], ["$39.99", ""]]}
                            bidComponents={[]}
                        />
                    </Row>
                </Row>
            </div>
        </>
    );
}

export default DriverTripsPage;