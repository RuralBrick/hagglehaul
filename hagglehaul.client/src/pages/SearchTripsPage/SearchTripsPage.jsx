import React from 'react';
import { useNavigate } from 'react-router-dom';
import './SearchTripsPage.css';
import {Button, ButtonGroup, Col, Dropdown, DropdownButton, Row} from "react-bootstrap";
import TripCard from "@/components/TripCard/TripCard.jsx";
import AddressSearchBar from "@/components/AddressSearchBar/AddressSearchBar.jsx";

function SearchTripsPage() {
    let navigate = useNavigate();

    const goToTrips = () => {
        navigate('/trips');
    };
    
    const setCurrentCoords = (coords) => {
    }
    
    const setTargetCoords = (coords) => {
    }

    return (
        <div className="search-page-container">
            <h1>Search for Trips</h1>
            <div className="search-bar-container">
                {/* #1 current and target location */}
                <label>
                    <p>Please Enter Your Current Location: </p>
                    <AddressSearchBar setCoordinates={setCurrentCoords} />
                </label>
                <label>
                    <p>Please Enter Your Target Location: </p>
                    <AddressSearchBar setCoordinates={setTargetCoords} />
                </label>
                {/* #2 max current-to-start distance */}
                <label htmlFor="quantity">Max Current-To-Pickup Distance:</label>
                <input type="number" id="quantity" name="quantity" min="1" max="5"/>

                {/* #3 sort by dropdown --> */}
                <label htmlFor="sort_by">Sort By:</label>

                <select name="sort_by" id="sort_by">
                    <option value="#">sort option</option>
                    <option value="#">sort option</option>
                    <option value="#">sort option</option>
                    <option value="#">sort option</option>
                </select>

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
