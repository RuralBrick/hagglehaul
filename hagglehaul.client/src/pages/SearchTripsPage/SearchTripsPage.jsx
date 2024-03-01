import React, {useContext, useEffect, useState} from 'react';
import { useNavigate } from 'react-router-dom';
import './SearchTripsPage.css';
import {Accordion, Button, Col, Modal, Row, Spinner} from "react-bootstrap";
import TripCard from "@/components/TripCard/TripCard.jsx";
import AddressSearchBar from "@/components/AddressSearchBar/AddressSearchBar.jsx";
import {TokenContext} from "@/App.jsx";
import CustomDateFormatter from "@/utils/CustomDateFormatter.jsx";
import MetersToMiles from "@/utils/MetersToMiles.jsx";
import SecondsToMinutes from "@/utils/SecondsToMinutes.jsx";
import TripMapModal from "@/components/TripMapModal/TripMapModal.jsx";
import AddBidModal from "@/components/AddBidModal/AddBidModal.jsx";

function SearchTripsPage() {
    let navigate = useNavigate();
    const [current, setCurrent] = useState();
    const [target, setTarget] = useState();
    const [data, setData] = useState(null);
    const {token, role} = useContext(TokenContext);

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

    const [showMapModal, setShowMapModal] = useState(false);
    const [mapGeoJSON, setMapGeoJSON] = useState();
    
    const [showAddBidModal, setShowAddBidModal] = useState(false);
    const [addBidTripId, setAddBidTripId] = useState();
    
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
    }
    
    const searchTrips = async () => {
        setData(null);
        
        const tripMarketRequest = {}

        var invalidFilter = false;
        filters.map((filter, index) => {
            if (document.getElementById("filter-check-" + index).checked) {
                let filterValue = document.getElementById("filter-val-" + index).value;
                if (filterValue === "") {
                    setError("Please enter a value for the " + filter.name + " filter.");
                    invalidFilter = true;
                }
                filter.requisites.map((requisite) => {
                    if (!requisite) {
                        setError("You need to enter a location to use the " + filter.name + " filter.");
                        invalidFilter = true;
                    }
                });
                tripMarketRequest[filter.requestAttribute] = parseFloat(filterValue) * filter.conversionFactor;
            }
        });
        if (invalidFilter) return;
        
        if (current) {
            tripMarketRequest.currentLat = current[1];
            tripMarketRequest.currentLong = current[0];
        }
        if (target) {
            tripMarketRequest.targetLat = target[1];
            tripMarketRequest.targetLong = target[0];
        }
        
        if (document.getElementById("sort-by").value) {
            if (document.getElementById("then-by").value) {
                tripMarketRequest.sortMethods = [document.getElementById("sort-by").value, document.getElementById("then-by").value];
            }
            else {
                tripMarketRequest.sortMethods = [document.getElementById("sort-by").value];
            }
        }
        
        fetch('/api/Driver/tripMarket', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + token,
                },
                body: JSON.stringify(tripMarketRequest)
            }
        ).then(async (response) => {
            var data = await response.text();
            try {
                data = JSON.parse(data);
            } catch (err) {
                setError("Something went wrong (" + response.status + "). Please try again.");
                return;
            }
            if (!response.ok) {
                setError(data.error);
            } else {
                setData(data);
            }
        });
    }

    useEffect(() => {
        searchTrips();
    }, []);

    if (error) {
        return errorModal;
    }
    
    return (
        <>
            <TripMapModal show={showMapModal} setShow={setShowMapModal} mapGeoJSON={mapGeoJSON}/>
            <AddBidModal show={showAddBidModal} setShow={setShowAddBidModal} tripId={addBidTripId}/>
            <div className="search-page-container">
                <h1>Search for Trips</h1>
                <Accordion>
                    <Accordion.Item eventKey="0">
                        <Accordion.Header>Advanced Search</Accordion.Header>
                        <Accordion.Body>
                            {/* #1 current and target location */}
                            <label>
                                <p>Your Current Location: </p>
                                <AddressSearchBar setCoordinates={setCurrent} setAddressText={() => {}}/>
                            </label>
                            <label>
                                <p>Your Target Location: </p>
                                <AddressSearchBar setCoordinates={setTarget} setAddressText={() => {}}/>
                            </label>
                            {/* #2 filters */}
                            {
                                filters.map((filter, index) =>
                                    <p>
                                        <label htmlFor={"filter-check-" + index}>
                                            <input type="checkbox" id={"filter-check-" + index}/>{filter.name}:
                                        </label>
                                        <input type="text" id={"filter-val-" + index} name={filter.requestAttribute}
                                               inputMode="numeric"/>
                                    </p>
                                )
                            }

                            {/* #3 sort by --> */}
                            <label htmlFor="sort-by">Sort By:</label>
                            <select name="sortBy" id="sort-by">
                                <option></option>
                                {sortMethods.map((method, index) => <option
                                    value={method.requestAttribute}>{method.name}</option>)}
                            </select>

                            <label htmlFor="then-by">Then By:</label>
                            <select name="thenBy" id="then-by">
                                <option></option>
                                {sortMethods.map((method, index) => <option
                                    value={method.requestAttribute}>{method.name}</option>)}
                            </select>
                            <Button style={{backgroundColor: "#D96C06"}} onClick={searchTrips}>Search</Button>
                        </Accordion.Body>
                    </Accordion.Item>
                </Accordion>
                {
                    Array.isArray(data) && data.length > 0 ?
                        <div className="trips-page container mt-5">
                            <Row xs={1} md={2} lg={1}>
                                {data.map((trip) =>
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
                                        bidComponents={[...trip.tripBids.map((bid) =>
                                            <Row style={bid.yourBid ? {
                                                backgroundColor: "#fef0e3",
                                                borderRadius: "10px"
                                            } : {}}>
                                                <Col> {bid.driverName}
                                                    <br/> {bid.driverRating?.toFixed(2) + "\u2605 (" + bid.driverNumRatings + ")"}
                                                </Col>
                                                <Col style={{
                                                    display: 'flex',
                                                    justifyContent: 'right'
                                                }}> {"$" + (bid.centsAmount / 100)?.toFixed(2)} </Col>
                                            </Row>
                                        ),
                                            <Button style={{backgroundColor: "#D96C06"}} onClick={() => {
                                                setAddBidTripId(trip.tripId);
                                                setShowAddBidModal(true);
                                            }}>
                                                + Add Bid
                                            </Button>
                                        ]}
                                    />
                                )}
                            </Row>
                        </div>
                        :
                        <span style={{padding: "15px"}}>{
                            Array.isArray(data) ?
                                <h5>We've looked far and wide, but we couldn't find any trips. Try another
                                    search.</h5>
                                :
                                <Spinner animation="border" role="status"
                                         style={{color: "#D96C06", textAlign: "center"}}>
                                    <span className="visually-hidden">Loading...</span>
                                </Spinner>
                        }</span>
                }
                <button onClick={goToTrips} className="back-to-trips-btn">Back to Trips</button>
            </div>
        </>
    );
}

export default SearchTripsPage;
