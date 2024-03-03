import React, {useState, useContext} from 'react';
import {Modal, Button, Spinner} from 'react-bootstrap';
import './WithdrawBidModal.css';
import {TokenContext} from "@/App.jsx";

const WithdrawBidModal = ({show, setShow, setError, tripId}) => {
    const [waiting, setWaiting] = useState(false);
    const {token, role} = useContext(TokenContext);

    async function withdrawBid() {
        setWaiting(true);
        await fetch('/api/Driver/bid?tripId=' + tripId, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            },
        }).then(async (response) => {
                if (!response.ok) {
                    try {
                        setError(JSON.parse(await response.text()).error ?? "Something went wrong during bid withdrawal.");
                    } catch (err) {
                        setError("Something went wrong during bid withdrawal.");
                    }
                    return;
                }
                window.location.reload();
            }
        );
    }
    
    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="cancellation-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>Withdraw Bid</Modal.Header>
            <Modal.Body>
                Are you sure you want to withdraw your bid?
            </Modal.Body>
            <Modal.Footer>
                <Button style={{backgroundColor: "#D96C06"}} onClick={() => {setShow(false)}}>No</Button>
                {waiting ?
                    <Spinner animation="border" role="status" style={{color: "#D96C06"}}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <Button style={{backgroundColor: "#D96C06"}} onClick={withdrawBid}>Yes</Button>
                }
            </Modal.Footer>
        </Modal>
    );
}

export default WithdrawBidModal;
