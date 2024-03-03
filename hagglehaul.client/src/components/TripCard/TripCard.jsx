import {Card, Button, Row, Col, ListGroup} from 'react-bootstrap';
import './TripCard.css';

function TripCard({ image, title, attributes, actionComponent, bidComponents, onClickImg }) {
    return (
        <>
            <Card className="small-medium-card">
                <Card.Img onClick={onClickImg} className="map-thumbnail" variant="top" src={image} />
                <Card.Body>
                    <Card.Title>{title}</Card.Title>
                    <Card.Text>
                        <Row>
                            <Col>
                                <h5>{attributes[0][0]}</h5>
                            </Col>
                            <Col className="text-end">
                                <p>{attributes[0][1]}</p>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <h5>{attributes[1][0]}</h5>
                            </Col>
                            <Col className="text-end">
                                <p>{attributes[1][1]}</p>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <h5>{attributes[2][0]}</h5>
                            </Col>
                            <Col className="text-end">
                                <p>{attributes[2][1]}</p>
                            </Col>
                        </Row>
                    </Card.Text>
                    {actionComponent}
                </Card.Body>
                <ListGroup className="list-group-flush">
                    {bidComponents.map((component, index) => 
                        <ListGroup.Item key={index}>{component}</ListGroup.Item>
                    )}
                </ListGroup>
            </Card>
            <Card className="large-card">
                <Card.Body>
                    <Row>
                        <Col md={3}>
                                <img onClick={onClickImg} className="map-thumbnail" src={image} style={{height: '8rem', borderRadius: '10px'}}/>
                        </Col>
                        <Col md={9}>
                            <Row>
                                <Col md={6}>
                                    <Card.Title>{title}</Card.Title>
                                </Col>
                                <Col className="text-end" md={6}>
                                    {actionComponent}
                                </Col>
                            </Row>
                            <Row style={{marginTop: "10px"}}>
                                <Col>
                                    <h5>{attributes[0][0]}</h5>
                                    <p>{attributes[0][1]}</p>
                                </Col>
                                <Col>
                                    <h5>{attributes[1][0]}</h5>
                                    <p>{attributes[1][1]}</p>
                                </Col>
                                <Col>
                                    <h5>{attributes[2][0]}</h5>
                                    <p>{attributes[2][1]}</p>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Card.Body>
                <ListGroup className="list-group-flush">
                    {bidComponents.map((component, index) =>
                        <ListGroup.Item key={index}>{component}</ListGroup.Item>
                    )}
                </ListGroup>
            </Card>
        </>

    );
}

export default TripCard;