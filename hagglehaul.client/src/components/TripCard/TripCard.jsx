// Import necessary components from react-bootstrap
import { Card, Button, Row, Col, ListGroup } from 'react-bootstrap';
import './TripCard.css'; // Import CSS file for TripCard styling

// Functional component: TripCard
function TripCard({ image, title, attributes, actionComponent, bidComponents, onClickImg }) {
    return (
        <>
            {/* Card for small and medium screens */}
            <Card className="small-medium-card">
                {/* Display image as card thumbnail, with onClickImg function */}
                <Card.Img onClick={onClickImg} className="map-thumbnail" variant="top" src={image} />
                <Card.Body>
                    {/* Display trip title */}
                    <Card.Title>{title}</Card.Title>
                    <Card.Text>
                        {/* Display trip attributes in rows */}
                        <Row>
                            <Col>
                                <h5>{attributes[0][0]}</h5> {/* Attribute label */}
                            </Col>
                            <Col className="text-end">
                                <p>{attributes[0][1]}</p> {/* Attribute value */}
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <h5>{attributes[1][0]}</h5> {/* Attribute label */}
                            </Col>
                            <Col className="text-end">
                                <p>{attributes[1][1]}</p> {/* Attribute value */}
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <h5>{attributes[2][0]}</h5> {/* Attribute label */}
                            </Col>
                            <Col className="text-end">
                                <p>{attributes[2][1]}</p> {/* Attribute value */}
                            </Col>
                        </Row>
                    </Card.Text>
                    {actionComponent} {/* Display action component */}
                </Card.Body>
                <ListGroup className="list-group-flush">
                    {/* Display bid components as list items */}
                    {bidComponents.map((component, index) =>
                        <ListGroup.Item key={index}>{component}</ListGroup.Item>
                    )}
                </ListGroup>
            </Card>

            {/* Card for large screens */}
            <Card className="large-card">
                <Card.Body>
                    <Row>
                        <Col md={3}>
                            {/* Display image with onClickImg function */}
                            <img onClick={onClickImg} className="map-thumbnail" src={image} style={{height: '8rem', borderRadius: '10px'}}/>
                        </Col>
                        <Col md={9}>
                            <Row>
                                <Col md={6}>
                                    <Card.Title>{title}</Card.Title> {/* Display trip title */}
                                </Col>
                                <Col className="text-end" md={6}>
                                    {actionComponent} {/* Display action component */}
                                </Col>
                            </Row>
                            <Row style={{marginTop: "10px"}}>
                                {/* Display trip attributes */}
                                <Col>
                                    <h5>{attributes[0][0]}</h5> {/* Attribute label */}
                                    <p>{attributes[0][1]}</p> {/* Attribute value */}
                                </Col>
                                <Col>
                                    <h5>{attributes[1][0]}</h5> {/* Attribute label */}
                                    <p>{attributes[1][1]}</p> {/* Attribute value */}
                                </Col>
                                <Col>
                                    <h5>{attributes[2][0]}</h5> {/* Attribute label */}
                                    <p>{attributes[2][1]}</p> {/* Attribute value */}
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Card.Body>
                <ListGroup className="list-group-flush">
                    {/* Display bid components as list items */}
                    {bidComponents.map((component, index) =>
                        <ListGroup.Item key={index}>{component}</ListGroup.Item>
                    )}
                </ListGroup>
            </Card>
        </>
    );
}

export default TripCard; // Export the TripCard component
