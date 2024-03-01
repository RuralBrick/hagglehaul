import React, { useState } from 'react';
import { Modal, Button } from 'react-bootstrap';
import './RateTripModal.css'; // Ensure to create a corresponding CSS file

const RateTripModal = ({ show, setShow, tripId }) => {
    const [localRating, setLocalRating] = useState(0); // Local state to handle the rating within the modal

    const handleStarClick = (ratingValue) => {
        setLocalRating(ratingValue);
    };

    const submitRating = () => {
        console.log('Submitting rating', localRating, 'for tripId', tripId);
        // Implement the API call to submit the rating here
        // After submitting, you can close the modal or show a success message
        setShow(false);
    };

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
            <Modal.Footer>
                <Button variant="secondary" onClick={() => setShow(false)}>No</Button>
                <Button variant="primary" onClick={submitRating}>Yes</Button>
            </Modal.Footer>
        </Modal>
    );
};

export default RateTripModal;
