
import React from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

function DatePickerComponent({ selectedDate, onChange }) {
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
