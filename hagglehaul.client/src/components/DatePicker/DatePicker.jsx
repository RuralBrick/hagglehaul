import React from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

// DatePickerComponent for selecting dates and times
function DatePickerComponent({ selectedDate, onChange }) {
    // Render DatePicker component with specified props
    return (
        <DatePicker
            selected={selectedDate}
            onChange={date => onChange(date)}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={15}
            dateFormat="MMMM d, yyyy h:mm aa"
            timeCaption="Time"
        />
    );
}

export default DatePickerComponent;
