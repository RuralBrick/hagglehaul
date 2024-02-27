// Modal.jsx
import React, {useState, useContext} from 'react';
import {Modal, Button, Spinner} from 'react-bootstrap';
import './SelectBidModal.css';
import {TokenContext} from "@/App.jsx";

const SelectBidModal = ({show, setShow, setError, selectBidData}) => {
    const [waiting, setWaiting] = useState(false);
    const {token, role} = useContext(TokenContext);
    
    async function selectBid() {
        setWaiting(true);
        await fetch('/api/Rider/tripDriver', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + token,
                },
                body: JSON.stringify(selectBidData)
            }).then(async (response) => {
                if (!response.ok) {
                    try {
                        setError(JSON.parse(await response.text()).error ?? "Something went wrong during bid selection.");
                    } catch (err) {
                        setError("Something went wrong during bid selection.");
                    }
                    return;
                }
                window.location.reload();
            });
    }
    
    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="select-bid-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>Select Bid</Modal.Header>
            <Modal.Body>
                Are you sure you want to select this bid?
            </Modal.Body>
            <Modal.Footer>
                <Button style={{backgroundColor: "#D96C06"}} onClick={() => {setShow(false)}}>No</Button>
                {waiting ?
                    <Spinner animation="border" role="status" style={{color: "#D96C06"}}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <Button style={{backgroundColor: "#D96C06"}} onClick={selectBid}>Yes</Button>
                }
            </Modal.Footer>
        </Modal>
    );
}

export default SelectBidModal;
