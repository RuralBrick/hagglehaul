import React, {useEffect, useState} from 'react';
import { useNavigate } from 'react-router-dom';
import './SearchTripsPage.css';
import {Button, ButtonGroup, Col, Dropdown, DropdownButton, Row} from "react-bootstrap";
import TripCard from "@/components/TripCard/TripCard.jsx";
import AddressSearchBar from "@/components/AddressSearchBar/AddressSearchBar.jsx";

function SearchTripsPage() {
    let navigate = useNavigate();
    const [current, setCurrent] = useState();
    const [target, setTarget] = useState();
    
    const filters = [
        {
            name: "Current Location Search Radius (mi)",
            requestAttribute: "maxCurrentToStartDistance",
            requisites: [current],
            conversionFactor: 1/70 // approx WGS84 factor for Southern California
        },
        {
            name: "Target Location Search Radius (mi)",
            requestAttribute: "maxEndToTargetDistance",
            requisites: [target],
            conversionFactor: 1/70
        },
        {
            name: "Max Euclidean Distance (mi)",
            requestAttribute: "maxEuclideanDistance",
            requisites: [],
            conversionFactor: 1/70
        },
        {
            name: "Max Route Distance (mi)",
            requestAttribute: "maxRouteDistance",
            requisites: [],
            conversionFactor: 1609.34
        },
        {
            name: "Min Lowest Bid ($)",
            requestAttribute: "minCurrentMinBid",
            requisites: [],
            conversionFactor: 100
        }
    ]
    
    const sortMethods = [
        {
            name: "Euclidean Distance",
            requestAttribute: "euclideanDistance",
            requisites: []
        },
        {
            name: "Route Distance",
            requestAttribute: "routeDistance",
            requisites: []
        },
        {
            name: "Driving Time",
            requestAttribute: "routeDuration",
            requisites: []
        },
        {
            name: "Closest to Current",
            requestAttribute: "currentToStartDistance",
            requisites: [current]
        },
        {
            name: "Closest to Target",
            requestAttribute: "endToTargetDistance",
            requisites: [target]
        },
        {
            name: "Lowest Bid",
            requestAttribute: "currentMinBid",
            requisites: []
        },
        {
            name: "Earliest Start Time",
            requestAttribute: "startTime",
            requisites: []
        }
    ]

    const goToTrips = () => {
        navigate('/trips');
    };
    
    const searchTrips = async () => {
        filters.map((filter, index) => {
            let filterChecked = document.getElementById("filter-check-" + index).checked;
            let filterValue = document.getElementById("filter-val-" + index).value;
            console.log("Filter " + filter.requestAttribute + " checked: " + filterChecked + ", value: " + filterValue);
        });
        
        console.log("Sort by: " + document.getElementById("sort-by").value);
        console.log("Then by: " + document.getElementById("then-by").value);
    };

    useEffect(() => {
        searchTrips();
    }, []);
    
    return (
        <div className="search-page-container">
            <h1>Search for Trips</h1>
            <div className="search-bar-container">
                {/* #1 current and target location */}
                <label>
                    <p>Please Enter Your Current Location: </p>
                    <AddressSearchBar setCoordinates={setCurrent} />
                </label>
                <label>
                    <p>Please Enter Your Target Location: </p>
                    <AddressSearchBar setCoordinates={setTarget} />
                </label>
                {/* #2 filters */}
                {
                    filters.map((filter, index) => 
                        <p>
                            <label htmlFor={"filter-check-" + index}>
                                <input type="checkbox" id={"filter-check-" + index}/>{filter.name}:
                            </label>
                            <input type="text" id={"filter-val-" + index} name={filter.requestAttribute} inputmode="numeric" />
                        </p>
                    )
                }
                
                {/* #3 sort by --> */}
                <label htmlFor="sort-by">Sort By:</label>

                <select name="sortBy" id="sort-by">
                    <option></option>
                    {sortMethods.map((method, index) => <option value={method.requestAttribute}>{method.name}</option>)}
                </select>
                
                <label htmlFor="then-by">Then By:</label>
                
                <select name="thenBy" id="then-by">
                    <option></option>
                    {sortMethods.map((method, index) => <option value={method.requestAttribute}>{method.name}</option>)}
                </select>
                
                <Button style={{backgroundColor: "#D96C06"}} onClick={searchTrips}>Search</Button>

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
