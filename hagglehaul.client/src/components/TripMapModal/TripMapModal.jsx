import React, {useEffect, useRef} from 'react';
import { Modal, Button } from 'react-bootstrap';
import './TripMapModal.css';
import {GeoJSON, MapContainer, Marker, Popup, TileLayer, useMap} from 'react-leaflet'
import LeafletMapRoute from "@/components/LeafletMapRoute/LeafletMapRoute.jsx";

const TripMapModal = ({show, setShow, mapGeoJSON}) => {
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
                <MapContainer className="hh-map-frame" center={[51.505, -0.09]} zoom={13} scrollWheelZoom={true}>
                    <TileLayer
                        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    />
                    <LeafletMapRoute mapGeoJSON={mapGeoJSON} />
                </MapContainer>
            </Modal.Body>
        </Modal>
    );
}

export default TripMapModal;
