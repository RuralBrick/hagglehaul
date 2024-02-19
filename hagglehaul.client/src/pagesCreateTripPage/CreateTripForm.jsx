import React, { useState } from 'react';

function CreateTripForm() {
    const [formData, setFormData] = useState({
        destination: '',
        date: '',
        time: '',
        price: '',
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        //
        console.log(formData);
    };

    return (
        <form onSubmit={handleSubmit}>
            {/* Form fields */}
            <input type="text" name="destination" value={formData.destination} onChange={handleChange} placeholder="Destination" />
            <input type="date" name="date" value={formData.date} onChange={handleChange} />
            {/*  */}
            <button type="submit">Create Trip</button>
        </form>
    );
}

export default CreateTripForm;
