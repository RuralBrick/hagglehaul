import React, { useState, useContext } from 'react';
import { Modal, Button, Spinner } from 'react-bootstrap';
import './CancellationModal.css';
import { TokenContext } from "@/App.jsx";

// CancellationModal component for canceling a trip
const CancellationModal = ({ show, setShow, setError, cancellationId }) => {
    // State variables for managing waiting state
    const [waiting, setWaiting] = useState(false);
    // Context for accessing token and role
    const { token, role } = useContext(TokenContext);

    // Function to cancel trip
    async function cancelTrip() {
        // Set waiting state to true
        setWaiting(true);
        // Make DELETE request to cancel trip
        await fetch('/api/Rider/trip?tripId=' + cancellationId, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            },
        }).then(async (response) => {
            // Handle response
            if (!response.ok) {
                try {
                    setError(JSON.parse(await response.text()).error ?? "Something went wrong during trip cancellation.");
                } catch (err) {
                    setError("Something went wrong during trip cancellation.");
                }
                return;
            }
            // Reload window upon success
            window.location.reload();
        });
    }

    // Render CancellationModal component
    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="cancellation-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            {/* Modal header */}
            <Modal.Header closeButton>Cancel Trip</Modal.Header>
            {/* Modal body */}
            <Modal.Body>
                Are you sure you want to cancel this trip?
            </Modal.Body>
            {/* Modal footer with Yes and No buttons */}
            <Modal.Footer>
                <Button style={{ backgroundColor: "#D96C06" }} onClick={() => { setShow(false) }}>No</Button>
                {/* Display spinner while waiting for response */}
                {waiting ?
                    <Spinner animation="border" role="status" style={{ color: "#D96C06" }}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <Button style={{ backgroundColor: "#D96C06" }} onClick={cancelTrip}>Yes</Button>
                }
            </Modal.Footer>
        </Modal>
    );
}

export default CancellationModal;
