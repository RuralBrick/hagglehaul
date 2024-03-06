import React, { useState, useContext } from 'react';
import { Modal, Button, Spinner } from 'react-bootstrap';
import './AddBidModal.css';
import { TokenContext } from "@/App.jsx";

// AddBidModal component for adding bids to a specific trip
const AddBidModal = ({ show, setShow, setError, tripId }) => {
    // State variables for managing bid input and loading state
    const [waiting, setWaiting] = useState(false);
    const [cents, setCents] = useState(0);
    const [textValue, setTextValue] = useState("$0.00");
    // Accessing token and role from context
    const { token, role } = useContext(TokenContext);

    // Function to handle adding bid
    async function addBid() {
        setWaiting(true); // Set loading state
        // Sending POST request to add bid
        await fetch('/api/Driver/bid', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            },
            body: JSON.stringify({ tripId: tripId, centsAmount: cents }),
        }).then(async (response) => {
            if (!response.ok) {
                // Handling error response
                try {
                    setError(JSON.parse(await response.text()).error ?? "Something went wrong during bid modification.");
                } catch (err) {
                    setError("Something went wrong during bid modification.");
                }
                return;
            }
            window.location.reload(); // Refreshing page on success
        });
    }

    // Function to handle key press events for bid input
    async function handleKeyPress(e) {
        var number = ""
        for (var i = 0; i < e.target.value.length; i++) {
            if (e.target.value[i] >= '0' && e.target.value[i] <= '9') {
                number += e.target.value[i];
            }
        }
        // Updating bid amount and text display
        setCents(parseInt(number));
        setTextValue("$" + (parseInt(number) / 100).toFixed(2));
    }

    // Render AddBidModal component
    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="cancellation-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>Add Bid</Modal.Header>
            <Modal.Body>
                {/* Input for bid amount */}
                <input type="text" placeholder="Enter new bid" className="cents-input" value={textValue} onChange={handleKeyPress} inputmode="numeric" />
            </Modal.Body>
            <Modal.Footer>
                {/* Cancel button */}
                <Button style={{ backgroundColor: "#D96C06" }} onClick={() => { setShow(false) }}>Cancel</Button>
                {/* Conditional rendering based on loading state */}
                {waiting ?
                    <Spinner animation="border" role="status" style={{ color: "#D96C06" }}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <Button style={{ backgroundColor: "#D96C06" }} onClick={addBid}>Add Bid</Button>
                }
            </Modal.Footer>
        </Modal>
    );
}

export default AddBidModal;
