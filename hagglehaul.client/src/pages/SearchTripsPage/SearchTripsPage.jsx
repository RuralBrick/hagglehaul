import React from 'react';
import { useNavigate } from 'react-router-dom';
import { BsSearch } from 'react-icons/bs';
import './SearchTripsPage.css';
import {Button, ButtonGroup, Col, Dropdown, DropdownButton, Row} from "react-bootstrap";
import TripCard from "@/components/TripCard/TripCard.jsx";

function SearchTripsPage() {
    let navigate = useNavigate();

    const goToTrips = () => {
        navigate('/trips');
    };

    return (
        <div className="search-page-container">
            <h1>Search for Trips</h1>
            <div className="search-bar-container">
                <input
                    type="text"
                    placeholder="Search trips..."
                    className="search-input"
                />
            </div>
            <div className="trips-page container mt-5">
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
            <button onClick={goToTrips} className="back-to-trips-btn">Back to Trips</button>
        </div>
    );
}

export default SearchTripsPage;
