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

    useEffect(() => {
        if (!shouldFetch) return;
        fetchResults();
    }, [debouncedText]);
    
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
        <div >
            <input type="text" ref={inputRef} placeholder="Enter Address" value={inputText} onChange={handleAddressInput} />
            {results.map((result, index) => (
                <div key={index} onClick={() => handleResultClick(result.text, result.center)}>
                    {result.text}
                    ({result.place_name})
                </div>
            ))}
        </div>
    );
}

export default AddressSearchBar;
