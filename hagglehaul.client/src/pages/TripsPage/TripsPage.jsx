import React, { useState } from 'react';
import Calendar from 'react-calendar';
import './TripsPage.css';

function TripsPage() {
    const trips = [
        {
            id: 1,
            name: 'Riverside Gaming Lab',
            address: '3633 Market St, Riverside, CA 92501',
            dateTime: new Date('2024-01-28T17:22:00'), 
            price: '$23.78'
        },
        {
            id: 2,
            name: 'Disneyland Park',
            address: 'Anaheim, CA 92802',
            driver: 'Will Smith',
            rating: '4.83',
            dateTime: new Date('2024-02-03T06:32:00'), 
            price: '$76.93'
        }
    ];

    const [selectedDate, setSelectedDate] = useState(null);

    const formatDate = (date) => {
        const options = { day: 'numeric', month: 'short' };
        return date.toLocaleDateString('en-US', options);
    };

    // Function to filter trips by date
    const getTripsOnDate = (date) => {
        if (!date) return trips; // Return all trips if no date selected
        const filteredTrips = trips.filter(trip => {
            const tripDate = new Date(trip.dateTime);
            return tripDate.toDateString() === date.toDateString();
        });
        return filteredTrips;
    };

    const handleDateChange = (date) => {
        setSelectedDate(date);
    };
// Function to highlight dates with trips
const tileContent = ({ date, view }) => {
    if (view === 'month') {
        const tripDates = trips.map(trip => new Date(trip.dateTime).toDateString());
        const dateString = date.toDateString();
        if (tripDates.includes(dateString)) {
            return (
                <div className="trip-marker"></div>
            );
        }
    }
};



    return (
        <div className="trips-page container mt-5">
            <h2 className="mb-4">Confirmed Trips</h2>
            <div className="calendar-container">
                <Calendar
                    onChange={handleDateChange}
                    value={selectedDate}
                    className="custom-calendar"
                    tileContent={tileContent}
                />
            </div>
            <div className="trip-list">
                {selectedDate && (
                    <h3>{formatDate(selectedDate)}</h3>
                )}
                {getTripsOnDate(selectedDate).map(trip => (
                    <div key={trip.id} className="trip-item">
                        <h5>{trip.name}</h5>
                        <p>{trip.address}</p>
                        <p>{trip.dateTime.toLocaleString()}</p> {}
                        <p>{trip.price}</p>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default TripsPage;
