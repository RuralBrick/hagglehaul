import { useEffect, useRef, useState } from "react";

/*
 * Attribution: https://www.telerik.com/blogs/how-to-create-custom-debounce-hook-react
 * (c) 2024 Progress Software Corporation
 */

const useDebounce = (value, delay = 500) => {
    const [debouncedValue, setDebouncedValue] = useState("");
    const timerRef = useRef();

    useEffect(() => {
        timerRef.current = setTimeout(() => setDebouncedValue(value), delay);

        return () => {
            clearTimeout(timerRef.current);
        };
    }, [value, delay]);

    return debouncedValue;
};

export default useDebounce;
