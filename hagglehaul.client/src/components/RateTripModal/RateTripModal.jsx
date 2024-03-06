import React, { useContext, useEffect, useState } from 'react';
import { Modal, Button, Spinner } from 'react-bootstrap';
import './RateTripModal.css';
import { TokenContext } from "@/App.jsx"; // Ensure to create a corresponding CSS file

// Component for rating a trip
const RateTripModal = ({ show, setShow, setError, tripId, rating, isRider }) => {
    const [localRating, setLocalRating] = useState(5); // Local state to handle the rating within the modal
    const [waiting, setWaiting] = useState(false);

    const { token, role } = useContext(TokenContext);

    // Function to handle click on star rating
    const handleStarClick = (ratingValue) => {
        setLocalRating(ratingValue);
    };

    // Function to submit the rating
    const submitRating = async () => {
        console.log('Submitting rating', localRating, 'for tripId', tripId);
        setWaiting(true);
        await fetch(`/api/${isRider ? "Rider" : "Driver"}/rating`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            },
            body: JSON.stringify({ tripId: tripId, ratingGiven: localRating }),
        }).then(async (response) => {
            if (!response.ok) {
                try {
                    setError(JSON.parse(await response.text()).error ?? "Something went wrong during bid modification.");
                } catch (err) {
                    setError("Something went wrong during bid modification.");
                }
                return;
            }
            window.location.reload();
        });
        setShow(false);
    };

    // Effect to set the local rating when the rating prop changes
    useEffect(() => {
        setLocalRating(rating);
    }, [rating]);

    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="rate-trip-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>Rate Trip</Modal.Header>
            <Modal.Body>
                <div className="rate-trip-stars">
                    {`Are you sure you want to give a rating of ${localRating}?`}
                    <div>
                        {[1, 2, 3, 4, 5].map((star) => (
                            <span
                                key={star}
                                className={`star${localRating >= star ? ' filled' : ''}`}
                                onClick={() => handleStarClick(star)}
                            >
                                &#9733; {/* Star Unicode Character */}
                            </span>
                        ))}
                    </div>
                </div>
            </Modal.Body>
            <Modal.Footer className="rate-trip-button">
                <Button variant="secondary" onClick={() => setShow(false)}>No</Button>
                {waiting ?
                    <Spinner animation="border" role="status" style={{ color: "#D96C06" }}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <Button variant="primary" onClick={submitRating}>Yes</Button>
                }
            </Modal.Footer>
        </Modal>
    );
};

export default RateTripModal;
