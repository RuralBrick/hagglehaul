// src/pages/SettingsPage/SettingsPage.jsx
import React, { useState, useContext } from 'react';
import { TokenContext } from "@/App.jsx";
import PropTypes from 'prop-types'
import 'bootstrap/dist/css/bootstrap.min.css';
import './SettingsPage.css';

const validatePassword = (inputPassword) => {
    const isValid = inputPassword.length >= 2 && inputPassword.length <= 99;
    return isValid;
};

function SettingsPage() {

    const [oldPass, setOldPass] = useState("");
    const [newPass, setNewPass] = useState("");
    const [phoneNum, setPhoneNum] = useState("");
    const [userName, setName] = useState("");

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
        if (!validatePassword(newPass)) {
            setErrorMessage(passwordValidationErrorMessage);
            return false;
        }
        return true;
    }

    const handleRiderChangeSubmit = async e => {

        e.preventDefault();
        if (newPass && !preSubmitValidation()) return;

        setWaiting(true);
        const results = await customizeRider({
            userName,
            phoneNum,
            oldPass,
            newPass
        });

        if (!results) {
            setErrorMessage(invalidCustomizationErrorMessage);
            setWaiting(false)
            return;
        }

        setWaiting(false)

    }

    if (1==1) {
        return (
            <div className="settings-flex">
                <div className="settings-wrapper">
                    <p className="auth-error-message">{errorMessage}</p>
                    <form onSubmit={handleRiderChangeSubmit}>
                        < br />
                        <div>
                            <label>
                                <p>{role}</p>
                                <input type="text" value={userName} onChange={e => setName(e.target.value)} />
                            </label>
                        </div>
                        <div>
                            <label>
                                <p>Phone Number</p>
                                <input type="text" value={phoneNum} onChange={e => setPhoneNum(e.target.value)} />
                            </label>
                        </div>
                        <div>
                            <div>
                                <label>
                                    <p>New Password</p>
                                    <input type="password" value={newPass} onChange={e => setNewPass(e.target.value)} />
                                </label>
                            </div>
                            <label>
                                <p>Confirm Old Password</p>
                                <input type="password" value={oldPass} onChange={e => setOldPass(e.target.value)} />
                            </label>
                        </div>
                        <br />
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
                    <p className="auth-error-message">{errorMessage}</p>
                    <form onSubmit={handleRiderChangeSubmit}>
                        < br />
                        <div>
                            <label>
                                <p>Name</p>
                                <input type="text" value={userName} onChange={e => setName(e.target.value)} />
                            </label>
                        </div>
                        <div>
                            <label>
                                <p>Phone Number</p>
                                <input type="text" value={phoneNum} onChange={e => setPhoneNum(e.target.value)} />
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
                                    <input type="password" value={newPass} onChange={e => setNewPass(e.target.value)} />
                                </label>
                            </div>
                            <label>
                                <p>Confirm Old Password</p>
                                <input type="password" value={oldPass} onChange={e => setOldPass(e.target.value)} />
                            </label>
                        </div>
                        <br />
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
