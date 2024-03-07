// Import necessary modules and components from React and react-bootstrap
import React, { useState, useContext } from 'react';
import { Modal, Button, Spinner } from 'react-bootstrap';
import './WithdrawBidModal.css'; // Import CSS file for WithdrawBidModal styling
import { TokenContext } from "@/App.jsx"; // Import TokenContext from App.jsx

// Functional component: WithdrawBidModal
const WithdrawBidModal = ({ show, setShow, setError, tripId }) => {
    // State variables
    const [waiting, setWaiting] = useState(false); // State for waiting status
    // Destructure token and role from TokenContext
    const { token, role } = useContext(TokenContext);

    // Function to withdraw bid
    async function withdrawBid() {
        setWaiting(true); // Set waiting status to true
        // Make DELETE request to withdraw bid
        await fetch('/api/Driver/bid?tripId=' + tripId, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token, // Include authorization token in headers
            },
        }).then(async (response) => {
            // Handle response
            if (!response.ok) {
                try {
                    setError(JSON.parse(await response.text()).error ?? "Something went wrong during bid withdrawal."); // Set error message if available
                } catch (err) {
                    setError("Something went wrong during bid withdrawal."); // Set generic error message
                }
                return;
            }
            window.location.reload(); // Reload page upon successful bid withdrawal
        });
    }

    // JSX
    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="cancellation-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>Withdraw Bid</Modal.Header> {/* Modal header */}
            <Modal.Body>
                Are you sure you want to withdraw your bid? {/* Modal body */}
            </Modal.Body>
            <Modal.Footer>
                <Button style={{ backgroundColor: "#D96C06" }} onClick={() => { setShow(false) }}>No</Button> {/* Button for cancelling withdrawal */}
                {/* Conditional rendering of spinner or withdrawal button based on waiting status */}
                {waiting ?
                    <Spinner animation="border" role="status" style={{ color: "#D96C06" }}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <Button style={{ backgroundColor: "#D96C06" }} onClick={withdrawBid}>Yes</Button> // Button for confirming withdrawal
                }
            </Modal.Footer>
        </Modal>
    );
}

export default WithdrawBidModal; // Export the WithdrawBidModal component
