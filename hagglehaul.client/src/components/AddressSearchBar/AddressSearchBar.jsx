import './AddressSearchBar.css';
import {TokenContext} from "@/App.jsx";
import {useContext, useState, useEffect, useRef} from "react";
import useDebounce from "@/utils/useDebounce.jsx";

function AddressSearchBar({setCoordinates}) {
    const { token } = useContext(TokenContext);
    const [inputText, setInputText] = useState('');
    const [shouldFetch, setShouldFetch] = useState(false);
    const [results, setResults] = useState([]);
    const debouncedText = useDebounce(inputText, 500);
    let inputRef = useRef(null);
    const canvasRef = useRef(null); // Add this line
    


    useEffect(() => {
        if (!shouldFetch) return;
        fetchResults();
    }, [debouncedText, shouldFetch, token, inputText]);

    const fetchResults = async () => {
        const m_results = await fetch('api/PlaceLookup?' + new URLSearchParams({
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
    }
    
    const handleAddressInput = async (e) => {
        setInputText(e.target.value);
        setCoordinates(null);
        if (e.target.value.length >= 3) setShouldFetch(true);
        else setShouldFetch(false);
    }
    
    const handleResultClick = (text, coords) => {
        setCoordinates(coords);
        setResults([]);
        setInputText(text);
        setShouldFetch(false);
        setTimeout(() => inputRef.current.blur(), 5);
    }

    return (
        <div className="address-search-container">
            <svg className="address-connector" width="25" height="200" xmlns="http://www.w3.org/2000/svg">
                {/* Top rectangle */}
                <rect x="5" y="0" width="25" height="25" fill="#d96c06"/>
                {/* Bottom rectangle */}
                <rect x="5" y="175" width="25" height="25" fill="#d96c06"/>
                {/* Top circle */}
                <circle cx="17.5" cy="12.5" r="8" fill="white" stroke="#d96c06" strokeWidth="2"/>
                {/* Bottom circle */}
                <circle cx="17.5" cy="187.5" r="8" fill="white" stroke="#d96c06" strokeWidth="2"/>
                {/* Dotted line */}
                <line x1="17.5" y1="20" x2="17.5" y2="175" stroke="#d96c06" strokeWidth="2" strokeDasharray="5,5"/>
            </svg>
            <input
                type="text"
                ref={inputRef}
                className="address-input"
                placeholder="Enter Address"
                value={inputText}
                onChange={handleAddressInput}
            />
            {results.length > 0 && (
                <>
                    <canvas ref={canvasRef} id="address-connector" width="25" height="100"
                            className="connector-canvas"></canvas>
                    <div className="results-container">
                        {results.map((result, index) => (
                            <div
                                key={index}
                                className="result-item"
                                onClick={() => handleResultClick(result.text, result.center)}
                            >
                                <strong>{result.text}</strong><br/>
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
