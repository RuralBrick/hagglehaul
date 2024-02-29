import React, {useEffect, useRef, useState} from 'react';
import {Modal, Button, Row} from 'react-bootstrap';
import './DriverLaunchModal.css';

const DriverLaunchModal = ({show, setShow, riderPhone, riderEmail, geoJSON}) => {
    const [pLatLong, setPLatLong] = useState("0,0");
    const [dLatLong, setDLatLong] = useState("0,0");
    
    useEffect(() => {
        if (geoJSON) {
            geoJSON.features.map((feature) => {
                if (feature.properties.name === "P") {
                    setPLatLong(`${feature.geometry.coordinates[1]},${feature.geometry.coordinates[0]}`);
                }
                if (feature.properties.name === "D") {
                    setDLatLong(`${feature.geometry.coordinates[1]},${feature.geometry.coordinates[0]}`);
                }
            });
        }
    }, [geoJSON]);
    
    return (
        <Modal
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            className="driver-launch-modal"
            show={show}
            onHide={() => setShow(false)}
            centered
        >
            <Modal.Header closeButton>
                <Modal.Title id="contained-modal-title-vcenter">
                    Driver Actions
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className="launch-labeled-section google-maps-launch">
                    <Row sm={1} md={2} lg={2}>
                    <Button variant="light" as={"a"}
                            href={`https://www.google.com/maps/dir/?api=1&destination=${pLatLong}&travelmode=driving`}
                            target="_blank" rel="noopener noreferrer" className="launch-button">
                        <span style={{fontSize: "1.5em"}}>&#129517; &#10230; &#128587;</span> <br/>
                        My Location to Pickup
                    </Button>
                    <Button variant="primary" as={"a"}
                            href={`https://www.google.com/maps/dir/?api=1&origin=${pLatLong}&destination=${dLatLong}&travelmode=driving`}
                            target="_blank" rel="noopener noreferrer" className="launch-button">
                        <span style={{fontSize: "1.5em"}}>&#128587; &#10230; &#127937;</span> <br/>
                        Pickup to Destination
                    </Button>
                    </Row>
                </div>
                <div className="launch-labeled-section apple-maps-launch">
                    <Row sm={1} md={2} lg={2}>
                        <Button variant="light" as={"a"}
                                href={`https://maps.apple.com/?daddr=${pLatLong}&dirflg=d`}
                                target="_blank" rel="noopener noreferrer" className="launch-button">
                            <span style={{fontSize: "1.5em"}}>&#129517; &#10230; &#128587;</span> <br/>
                            My Location to Pickup
                        </Button>
                        <Button variant="primary" as={"a"}
                                href={`https://maps.apple.com/?saddr=${pLatLong}&daddr=${dLatLong}&dirflg=d`}
                                target="_blank" rel="noopener noreferrer" className="launch-button">
                            <span style={{fontSize: "1.5em"}}>&#128587; &#10230; &#127937;</span> <br/>
                            Pickup to Destination
                        </Button>
                    </Row>
                </div>
                <div className="launch-labeled-section waze-launch">
                    <Button variant="light" as={"a"} href={`https://www.waze.com/ul?ll=${pLatLong}&navigate=yes`}
                            target="_blank" rel="noopener noreferrer" className="launch-button">
                        <span style={{fontSize: "1.5em"}}>&#129517; &#10230; &#128587;</span> <br/>
                        My Location to Pickup
                    </Button>
                    <Button variant="primary" as={"a"} href={`https://www.waze.com/ul?ll=${dLatLong}&navigate=yes`}
                            target="_blank" rel="noopener noreferrer" className="launch-button">
                        <span style={{fontSize: "1.5em"}}>&#128587; &#10230; &#127937;</span> <br/>
                        Pickup to Destination
                    </Button>
                </div>


                <Button as={"a"} href={`tel:${riderPhone}`} target="_blank" rel="noopener noreferrer"
                        style={{width: "100%", marginBottom: "10px", backgroundColor: "#D96C06"}}>&#128222; Call Rider</Button>
                <Button as={"a"} href={`sms:${riderPhone}`} target="_blank" rel="noopener noreferrer"
                        style={{width: "100%", marginBottom: "10px", backgroundColor: "#D96C06"}}>&#128172; Text Rider</Button>
                <Button as={"a"} href={`mailto:${riderEmail}`} target="_blank" rel="noopener noreferrer"
                        style={{width: "100%", marginBottom: "10px", backgroundColor: "#D96C06"}}>&#128231; Email Rider</Button>
            </Modal.Body>
        </Modal>
    );
}

export default DriverLaunchModal;
    