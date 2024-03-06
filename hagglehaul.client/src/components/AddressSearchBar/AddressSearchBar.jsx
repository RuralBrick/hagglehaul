import './AddressSearchBar.css';
import { TokenContext } from "@/App.jsx";
import { useContext, useState, useEffect, useRef } from "react";
import useDebounce from "@/utils/useDebounce.jsx";

// AddressSearchBar component for location searching
function AddressSearchBar({ setCoordinates, setAddressText }) {
    // Context for accessing token
    const { token } = useContext(TokenContext);
    // State variables for managing input and results
    const [inputText, setInputText] = useState('');
    const [shouldFetch, setShouldFetch] = useState(false);
    const [results, setResults] = useState([]);
    // Debouncing input text
    const debouncedText = useDebounce(inputText, 500);
    // Ref for input element
    let inputRef = useRef(null);
    const canvasRef = useRef(null); // This line remains as it seems unrelated to the SVG you mentioned.

    // Effect to fetch results when debounced text changes
    useEffect(() => {
        if (!shouldFetch) return;
        fetchResults();
    }, [debouncedText]);

    // Function to fetch address results
    const fetchResults = async () => {
        const m_results = await fetch('/api/PlaceLookup?' + new URLSearchParams({
            placeName: inputText,
        }), {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token,
            }
        })
            .then(data => data.json());
        setResults(m_results);
    };

    // Function to handle input change
    const handleAddressInput = async (e) => {
        setInputText(e.target.value);
        setCoordinates(null);
        if (e.target.value.length >= 3) setShouldFetch(true);
        else setShouldFetch(false);
    };

    // Function to handle result click
    const handleResultClick = (text, coords, address) => {
        setCoordinates(coords);
        setAddressText({ summary: text, address: address });
        setResults([]);
        setInputText(text);
        setShouldFetch(false);
        setTimeout(() => inputRef.current.blur(), 5);
    };

    // Render AddressSearchBar component
    return (
        <div className="address-search-container">
            {/* Input for address search */}
            <input
                type="text"
                ref={inputRef}
                className="address-input"
                placeholder="Enter Address"
                value={inputText}
                onChange={handleAddressInput}
            />
            {/* Displaying results if available */}
            {results.length > 0 && (
                <>
                    <canvas ref={canvasRef} id="address-connector" width="25" height="100" className="connector-canvas"></canvas>
                    <div className="results-container">
                        {results.map((result, index) => (
                            <div
                                key={index}
                                className="result-item"
                                onClick={() => handleResultClick(result.text, result.center, result.place_name)}
                            >
                                <strong>{result.text}</strong><br />
                                <span className="place-name">{result.place_name}</span>
                            </div>
                        ))}
                    </div>
                </>
            )}
        </div>
    );
}

export default AddressSearchBar;
