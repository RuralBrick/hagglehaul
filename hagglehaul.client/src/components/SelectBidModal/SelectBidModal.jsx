// Importing React, useState, and useContext from 'react' library
import React, { useState, useContext } from 'react';
// Importing necessary components from 'react-bootstrap' library
import { Modal, Button, Spinner } from 'react-bootstrap';
// Importing custom CSS file for styling
import './SelectBidModal.css';
// Importing TokenContext from App.jsx
import { TokenContext } from "@/App.jsx";

// Functional component for Select Bid Modal
const SelectBidModal = ({ show, setShow, setError, selectBidData }) => {
    // State hook for waiting status
    const [waiting, setWaiting] = useState(false);
    // Context hook for token and role
    const { token, role } = useContext(TokenContext);

    // Function to select a bid
    async function selectBid() {
        // Setting waiting status to true
        setWaiting(true);
        // Making POST request to select bid
        await fetch('/api/Rider/tripDriver', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token, // Including authorization token
            },
            body: JSON.stringify(selectBidData) // Converting data to JSON string
        }).then(async (response) => {
            // Handling response errors
            if (!response.ok) {
                try {
                    // Setting error message if available
                    setError(JSON.parse(await response.text()).error ?? "Something went wrong during bid selection.");
                } catch (err) {
                    // Setting generic error message
                    setError("Something went wrong during bid selection.");
                }
                return;
            }
            // Reloading page on successful bid selection
            window.location.reload();
        });
    }

    // Rendering Select Bid Modal component
    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="select-bid-modal" // Custom CSS class for styling
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            {/* Modal header */}
            <Modal.Header closeButton>Select Bid</Modal.Header>
            <Modal.Body>
                {/* Confirmation message */}
                Are you sure you want to select this bid?
            </Modal.Body>
            <Modal.Footer>
                {/* Cancel button */}
                <Button style={{ backgroundColor: "#D96C06" }} onClick={() => { setShow(false) }}>No</Button>
                {/* Conditional rendering based on waiting status */}
                {waiting ?
                    // Spinner animation while waiting
                    <Spinner animation="border" role="status" style={{ color: "#D96C06" }}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    // Button to confirm bid selection
                    <Button style={{ backgroundColor: "#D96C06" }} onClick={selectBid}>Yes</Button>
                }
            </Modal.Footer>
        </Modal>
    );
}

// Exporting SelectBidModal component
export default SelectBidModal;
