// src/pages/TripsPage/TripsPage.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import React, {useState} from 'react';
import './TripsPage.css';
import TripCard from "@/components/TripCard/TripCard.jsx";
import {DropdownButton, ButtonGroup, Dropdown, Button, Row, Col} from "react-bootstrap";
import AddTripModal from "@/components/AddTripModal/AddTripModal.jsx";

function TripsPage() {
    const [showAddTrip, setShowAddTrip] = useState(false);

    return (

        <>
            <AddTripModal show={showAddTrip} setShow={setShowAddTrip}/>
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
                        actionComponent={<Button style={{backgroundColor: "#D96C06"}}>Go somewhere</Button>}
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
                    <TripCard
                        image="https://placeholder.co/600x400.png"
                        title="Staples Center"
                        actionComponent={<Button style={{backgroundColor: "#D96C06"}}>Go somewhere</Button>}
                        attributes={[["Column 1", "Information 1"], ["Column 2", "Information 2"], ["Column 3", "Information 3"]]}
                        bidComponents={[]}
                    />
                    <TripCard
                        image="https://placeholder.co/600x400.png"
                        title="UCLA Jules Stein Eye Institute"
                        actionComponent={<DropdownButton as={ButtonGroup} title="Options" id="bg-nested-dropdown"
                                                         variant="light">
                            <Dropdown.Item>Cancel Trip</Dropdown.Item>
                            <Dropdown.Item>Do Something</Dropdown.Item>
                        </DropdownButton>}
                        attributes={[["Column 1", "Information 1"], ["Column 2", "Information 2"], ["Column 3", "Information 3"]]}
                        bidComponents={[]}
                    />
                </Row>
                <div className="trips-header mt-4 mb-4">
                    <h2>Trips in Bidding</h2>
                </div>
                <Row xs={1} md={2} lg={1}>
                    <TripCard
                        image="https://placeholder.co/600x400.png"
                        title="Disneyland Park"
                        actionComponent={<Button style={{backgroundColor: "#D96C06"}}>Go somewhere</Button>}
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

export default TripsPage;
