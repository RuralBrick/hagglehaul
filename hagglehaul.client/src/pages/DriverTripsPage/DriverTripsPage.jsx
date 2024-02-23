    import React, { useState } from 'react';
    import './DriverTripsPage.css';
    import TripCard from "@/components/TripCard/TripCard.jsx";
    import { DropdownButton, ButtonGroup, Dropdown, Button, Row, Col } from "react-bootstrap";
    import AddTripModal from "@/components/AddTripModal/AddTripModal.jsx";
    import DriverSearchTrip from "@/components/DriverSearchTrip/DriverSearchTrip.jsx"
    
    function DriverTripsPage() {
        const [showAddTrip, setShowAddTrip] = useState(false);
    
        return (
            <>
                <div className="trips-page container mt-5">
                    <br/>
                    {/* Header row with title and search bar */}
                    <div className="trips-header d-flex justify-content-between align-items-center mb-4">
                        <div style={{flex: 1}}></div>
                        {/* Invisible spacer */}
                        <h2 style={{flex: 1, textAlign: 'center'}}>Confirmed Trips</h2>
                        <div style={{flex: 1, textAlign: 'right'}}>
                            <DriverSearchTrip/>
                        </div>
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

    export default DriverTripsPage;