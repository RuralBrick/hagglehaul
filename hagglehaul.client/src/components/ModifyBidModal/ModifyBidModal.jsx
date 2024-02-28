import React, {useState, useContext} from 'react';
import {Modal, Button, Spinner} from 'react-bootstrap';
import './ModifyBidModal.css';
import {TokenContext} from "@/App.jsx";

const ModifyBidModal = ({show, setShow, setError, tripId}) => {
    const [waiting, setWaiting] = useState(false);
    const [cents, setCents] = useState(0);
    const [textValue, setTextValue] = useState("$0.00");
    const {token, role} = useContext(TokenContext);
    
    async function modifyBid() {
        setWaiting(true);
        await fetch('/api/Driver/bid', {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            },
            body: JSON.stringify({tripId: tripId, centsAmount: cents}),
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
    }
    
    async function handleKeyPress(e) {
        var number = ""
        for (var i = 0; i < e.target.value.length; i++) {
            if (e.target.value[i] >= '0' && e.target.value[i] <= '9') {
                number += e.target.value[i];
            }
        }
        setCents(parseInt(number));
        setTextValue("$" + (parseInt(number) / 100).toFixed(2));
    }
    
    return (
        <Modal
            aria-labelledby="contained-modal-title-vcenter"
            className="cancellation-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>Change Bid Amount</Modal.Header>
            <Modal.Body>
                <input type="text" placeholder="Enter new bid" className="cents-input" value={textValue} onChange={handleKeyPress} inputmode="numeric" />
            </Modal.Body>
            <Modal.Footer>
                <Button style={{backgroundColor: "#D96C06"}} onClick={() => {setShow(false)}}>Cancel</Button>
                {waiting ?
                    <Spinner animation="border" role="status" style={{color: "#D96C06"}}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    :
                    <Button style={{backgroundColor: "#D96C06"}} onClick={modifyBid}>Modify</Button>
                }
            </Modal.Footer>
        </Modal>
    );
}

export default ModifyBidModal;
