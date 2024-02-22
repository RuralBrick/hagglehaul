// src/pages/SettingsPage/SettingsPage.jsx
import React, { useState, useContext } from 'react';
import { TokenContext } from "@/App.jsx";
import { Spinner } from 'react-bootstrap';
import PropTypes from 'prop-types'
import 'bootstrap/dist/css/bootstrap.min.css';
import './SettingsPage.css';

const validatePassword = (inputPassword) => {
    const isValid = inputPassword.length >= 2 && inputPassword.length <= 99;
    return isValid;
};

async function customizeRider(details, token) {
    return await fetch('/api/Rider/modifyAcc', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify(details)
    })

}
async function customizeDriver(details, token) {
    return await fetch('/api/Driver/modifyAcc', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify(details)
    })

}


const invalidCustomizationErrorMessage = "There was an issue changing your details. Please try again.";
const passwordValidationErrorMessage = "Your password does not conform to the requirements.";
const customizationCompleteMessage = "Your account details have been changed!"

function SettingsPage() {

    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [phone, setPhoneNum] = useState("");
    const [name, setName] = useState("");

    const [carBrand, setCarBrand] = useState("");
    const [carColor, setCarColor] = useState("");
    const [license, setLicense] = useState("");

    const { token, role } = useContext(TokenContext)
    const [waiting, setWaiting] = useState(false);
    const [errorMessage, setErrorMessage] = useState("");

    // const role = useContext(RoleContext)

    //useEffect(() => {

    //    setInitialValues();

    //}, []);

    const preSubmitValidation = () => {
        if (!validatePassword(newPassword)) {
            setErrorMessage(passwordValidationErrorMessage);
            return false;
        }
        return true;
    }

    const handleRiderChangeSubmit = async e => {

        e.preventDefault();
        if (!preSubmitValidation()) return;

        setWaiting(true);
        const results = await customizeRider({
            name,
            phone,
            currentPassword,
            newPassword
        }, token);

        if (!results.ok) {
            setErrorMessage(invalidCustomizationErrorMessage);
            setWaiting(false)
            return;
        }

        setErrorMessage(customizationCompleteMessage);
        setWaiting(false)

    }

    const handleDriverChangeSubmit = async e => {

        e.preventDefault();
        if (!preSubmitValidation()) return;

        const carDescription = carBrand + carColor + license;

        setWaiting(true);
        const results = await customizeDriver({
            name,
            phone,
            carDescription,
            currentPassword,
            newPassword
        }, token);

        if (!results.ok) {
            setErrorMessage(invalidCustomizationErrorMessage);
            setWaiting(false)
            return;
        }

        setErrorMessage(customizationCompleteMessage);
        setWaiting(false)

    }

    if (role == "rider") {
        return (
            <div className="settings-flex">
                <div className="settings-wrapper">
                    <form onSubmit={handleRiderChangeSubmit}>
                        < br />
                        <div>
                            <label>
                                <p>{role}</p>
                                <input type="text" value={name} onChange={e => setName(e.target.value)} />
                            </label>
                        </div>
                        <div>
                            <label>
                                <p>Phone Number</p>
                                <input type="text" value={phone} onChange={e => setPhoneNum(e.target.value)} />
                            </label>
                        </div>
                        <div>
                            <div>
                                <label>
                                    <p>New Password</p>
                                    <input type="password" value={newPassword} onChange={e => setNewPassword(e.target.value)} />
                                </label>
                            </div>
                            <label>
                                <p>Confirm Old Password</p>
                                <input type="password" value={currentPassword} onChange={e => setCurrentPassword(e.target.value)} />
                            </label>
                        </div>
                        <br />
                        <p className="auth-error-message">{errorMessage}</p>
                        <div>                          
                            {waiting ?
                                <Spinner animation="border" role="status" style={{ color: "#D96C06" }}>
                                    <span className="visually-hidden">Loading...</span>
                                </Spinner>
                                :
                                <button type="submit">Submit Changes</button>
                            }
                        </div>
                    </form>
                </div>
            </div>
        );

    }

    else if (role == 'driver') {

        return (
            <div className="settings-flex">
                <div className="settings-wrapper">
                    <form onSubmit={handleDriverChangeSubmit}>
                        < br />
                        <div>
                            <label>
                                <p>Name</p>
                                <input type="text" value={name} onChange={e => setName(e.target.value)} />
                            </label>
                        </div>
                        <div>
                            <label>
                                <p>Phone Number</p>
                                <input type="text" value={phone} onChange={e => setPhoneNum(e.target.value)} />
                            </label>
                        </div>
                        <div className='car-details-wrapper'>
                            <label>
                                <p>Car Brand</p>
                                <input type="text" value={carBrand} onChange={e => setCarBrand(e.target.value)} />
                            </label>
                            <label>
                                <p>Car Color</p>
                                <input type="text" value={carColor} onChange={e => setCarColor(e.target.value)} />
                            </label>
                            <label>
                                <p>License Plate Number</p>
                                <input type="text" value={license} onChange={e => setLicense(e.target.value)} />
                            </label>
                        </div>
                        <div>
                            <div>
                                <label>
                                    <p>New Password</p>
                                    <input type="password" value={newPassword} onChange={e => setNewPassword(e.target.value)} />
                                </label>
                            </div>
                            <label>
                                <p>Confirm Old Password</p>
                                <input type="password" value={currentPassword} onChange={e => setCurrentPassword(e.target.value)} />
                            </label>
                        </div>
                        <br />
                        <p className="auth-error-message">{errorMessage}</p>
                        <div>                          
                            {waiting ?
                                <Spinner animation="border" role="status" style={{ color: "#D96C06" }}>
                                    <span className="visually-hidden">Loading...</span>
                                </Spinner>
                                :
                                <button type="submit">Submit Changes</button>
                            }
                        </div>
                    </form>
                </div>
            </div>
        );

    }

}

export default SettingsPage;
