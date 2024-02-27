import './LeafletMapRoute.css';
import {GeoJSON, useMap} from 'react-leaflet';
import React, {useEffect, useRef} from 'react';

const LeafletMapRoute = ({mapGeoJSON}) => {
    const map = useMap();
    const gjRef = useRef(null);

    useEffect(() => {
        if (gjRef.current) {
            map.fitBounds(gjRef.current.getBounds());
        }
    }, []);
    
    return (
        <GeoJSON ref={gjRef} data={mapGeoJSON} />
    );
}

export default LeafletMapRoute;
