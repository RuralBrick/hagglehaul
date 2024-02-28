import React, {useEffect, useRef, useState} from 'react';
import { Modal, Button } from 'react-bootstrap';
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
                <Button variant="light" as={"a"} href={`https://www.google.com/maps/dir/?api=1&origin=${pLatLong}&destination=${dLatLong}&travelmode=driving`} target="_blank" rel="noopener noreferrer" style={{width: "100%"}}>Google Maps</Button>
                <Button variant="light" as={"a"} href={`https://www.waze.com/ul?ll=${dLatLong}&navigate=yes`} target="_blank" rel="noopener noreferrer" style={{width: "100%"}}>Waze</Button>
                <Button variant="light" as={"a"} href={`https://maps.apple.com/?saddr=${pLatLong}&daddr=${dLatLong}&dirflg=d`} target="_blank" rel="noopener noreferrer" style={{width: "100%"}}>Apple Maps</Button>
                <Button variant="light" as={"a"} href={`tel:${riderPhone}`} target="_blank" rel="noopener noreferrer" style={{width: "100%"}}>Call Rider</Button>
                <Button variant="light" as={"a"} href={`mailto:${riderEmail}`} target="_blank" rel="noopener noreferrer" style={{width: "100%"}}>Email Rider</Button>
            </Modal.Body>
        </Modal>
    );
}

export default DriverLaunchModal;
    