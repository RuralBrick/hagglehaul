// Modal.jsx
import React, {useState, useContext} from 'react';
import {Modal, Button, Spinner} from 'react-bootstrap';
import './CancellationModal.css';
import {TokenContext} from "@/App.jsx";

const CancellationModal = ({show, setShow, setError, cancellationId}) => {
    const [waiting, setWaiting] = useState(false);
    const {token, role} = useContext(TokenContext);
    async function cancelTrip() {
        setWaiting(true);
        await fetch('/api/Rider/trip?tripId=' + cancellationId, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + token,
                },
            }).then(async (response) => {
                if (!response.ok) {
                    try {
                        setError(JSON.parse(await response.text()).error ?? "Something went wrong during trip cancellation.");
                    } catch (err) {
                        setError("Something went wrong during trip cancellation.");
                    }
                    return;
                }
                window.location.reload();
            });

    }
    
    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="cancellation-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>Cancel Trip</Modal.Header>
            <Modal.Body>
                Are you sure you want to cancel this trip?
            </Modal.Body>
            <Modal.Footer>
                <Button style={{backgroundColor: "#D96C06"}} onClick={() => {setShow(false)}}>No</Button>
                {waiting ?
                    <Spinner animation="border" role="status" style={{color: "#D96C06"}}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <Button style={{backgroundColor: "#D96C06"}} onClick={cancelTrip}>Yes</Button>
                }
            </Modal.Footer>
        </Modal>
    );
}

export default CancellationModal;
