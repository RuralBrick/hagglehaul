import './LeafletMapRoute.css';
import { GeoJSON, useMap } from 'react-leaflet';
import React, { useEffect, useRef } from 'react';

// Component for rendering a route on a Leaflet map
const LeafletMapRoute = ({ mapGeoJSON }) => {
    const map = useMap();
    const gjRef = useRef(null);

    // Adjust map bounds to fit the GeoJSON route when component mounts
    useEffect(() => {
        if (gjRef.current) {
            map.fitBounds(gjRef.current.getBounds());
        }
    }, []);

    // Render GeoJSON route
    return (
        <GeoJSON ref={gjRef} data={mapGeoJSON} style={{ color: "#D96C06", weight: 5 }} />
    );
}

export default LeafletMapRoute;
