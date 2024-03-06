// Import necessary modules and components from React and react-bootstrap
import React, { useEffect, useRef } from 'react';
import { Modal, Button } from 'react-bootstrap';
import './TripMapModal.css'; // Import CSS file for TripMapModal styling
import { GeoJSON, MapContainer, Marker, Popup, TileLayer, useMap } from 'react-leaflet'; // Import components from react-leaflet
import LeafletMapRoute from "@/components/LeafletMapRoute/LeafletMapRoute.jsx"; // Import custom LeafletMapRoute component

// Functional component: TripMapModal
const TripMapModal = ({ show, setShow, mapGeoJSON }) => {
    return (
        <Modal
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            className="trip-map-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>
                <Modal.Title id="contained-modal-title-vcenter">
                    Route Preview
                </Modal.Title>
            </Modal.Header>
            <Modal.Body >
                {/* Map container for displaying route preview */}
                <MapContainer className="hh-map-frame" scrollWheelZoom={true}>
                    {/* Tile layer for map */}
                    <TileLayer
                        attribution='&copy; <a href="https://www.stadiamaps.com/" target="_blank">Stadia Maps</a> &copy; <a href="https://www.stamen.com/" target="_blank">Stamen Design</a> &copy; <a href="https://openmaptiles.org/" target="_blank">OpenMapTiles</a> &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                        url="https://tiles.stadiamaps.com/tiles/stamen_toner_lite/{z}/{x}/{y}{r}.png"
                    />
                    {/* Custom LeafletMapRoute component */}
                    <LeafletMapRoute mapGeoJSON={mapGeoJSON} />
                </MapContainer>
            </Modal.Body>
        </Modal>
    );
}

export default TripMapModal; // Export the TripMapModal component
