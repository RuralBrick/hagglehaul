import React from 'react';
import { Modal, Button } from 'react-bootstrap';
import './AddTripModal.css';
import RiderAddTrip from "@/components/RiderAddTrip/RiderAddTrip.jsx"; // make sure to create a corresponding CSS file for styling

// AddTripModal component for adding a new trip
const AddTripModal = ({ show, setShow }) => {
    // Render AddTripModal component
    return (
        <Modal
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            className="add-trip-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            {/* Modal header */}
            <Modal.Header closeButton>
                <Modal.Title id="contained-modal-title-vcenter">
                    Add Trip
                </Modal.Title>
            </Modal.Header>
            {/* Modal body containing RiderAddTrip component */}
            <Modal.Body>
                <RiderAddTrip />
            </Modal.Body>
        </Modal>
    );
};

export default AddTripModal;
