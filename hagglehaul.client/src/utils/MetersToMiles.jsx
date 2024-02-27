const MetersToMiles = (meters) => {
    const miles = meters * 0.000621371;
    return Math.round(miles * 100) / 100;
}

export default MetersToMiles;
