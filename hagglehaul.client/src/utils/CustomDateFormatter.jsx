const CustomDateFormatter = (date) => {
    return new Intl.DateTimeFormat(navigator.language, {
        weekday: 'short',
        month: 'short',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    }).format(new Date(date));
}

export default CustomDateFormatter;
