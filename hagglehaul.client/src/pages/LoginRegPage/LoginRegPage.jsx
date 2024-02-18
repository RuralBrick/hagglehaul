import React, { useState } from 'react';
import PropTypes from 'prop-types'
import {Spinner} from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './LoginRegPage.css';

async function loginUser(credentials) {
    return fetch('api/Authentication/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(credentials)
    })
        .then(data => data.json())
        .then(data => data.token)
}

async function registerUser(credentials) {
    return fetch('api/Authentication/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(credentials)
    })
        .then(data => data.json())
        .then(data => data.token)
}

const invalidRegistrationErrorMessage = "The user already exists or an unknown error occurred. Please try again.";
const invalidLoginErrorMessage = "Unable to sign in. Please check your email or password.";
const passwordValidationErrorMessage = "Your password does not conform to the requirements.";
const emailValidationErrorMessage = "The email address is invalid.";

const validateEmail = (inputEmail) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const isValid = emailRegex.test(inputEmail);
    return isValid;
};

const validatePassword = (inputPassword) => {
    const isValid = inputPassword.length >= 2 && inputPassword.length <= 99;
    return isValid;
};

function LoginRegPage({ setToken }) {

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [role, setRole] = useState("rider");
    const [func, setFunc] = useState(false);
    const [errorMessage, setErrorMessage] = useState("");
    const [waiting, setWaiting] = useState(false);

    const preSubmitValidation = () => {
        if (!validateEmail(email)) {
            setErrorMessage(emailValidationErrorMessage);
            return false;
        }
        if (!validatePassword(password)) {
            setErrorMessage(passwordValidationErrorMessage);
            return false;
        }
        return true;
    }
    const handleLoginSubmit = async e => {
        e.preventDefault();
        if (!preSubmitValidation()) return;
        
        setWaiting(true);
        const token = await loginUser({
            email,
            password
        });
        if (!token) {
            setErrorMessage(invalidLoginErrorMessage);
            setWaiting(false);
            return;
        }
        setToken(token);
    }

    const handleRegisterSubmit = async e => {
        e.preventDefault();
        if (!preSubmitValidation()) return;

        setWaiting(true);
        const token = await registerUser({
            email,
            password,
            role
        });
        if (!token) {
            setErrorMessage(invalidRegistrationErrorMessage);
            setWaiting(false)
            return;
        }
        setToken(token);
    }

    const handleSwitchFunc = async e => {
        e.preventDefault();
        setEmail("");
        setPassword("");
        setErrorMessage("");
        setFunc(!func);
    }

    if (func) {

        return (
            <div className="login-flex">
            <div className="login-wrapper">
                <h1>Please Register</h1>
                <p className="auth-error-message">{errorMessage}</p>
                <form onSubmit={handleRegisterSubmit}>
                    <div>
                        <label>
                            <p>Email</p>
                            <input type="text" value={email} onChange={e => setEmail(e.target.value)}/>
                        </label>
                    </div>
                    <div>
                        <label>
                            <p>Password</p>
                            <input type="password" value={password} onChange={e => setPassword(e.target.value)}/>
                        </label>
                    </div>
                    <br/>
                    <div className="btn-group" style={{marginBottom: '20px'}}>
                        <input type="radio" className="btn-check" name="roleSelect" id="rider" autoComplete="off"
                               checked={role === "rider"} onChange={() => setRole("rider")}/>
                        <label className="btn btn-secondary rider-toggle" htmlFor="rider"
                               data-mdb-ripple-init>Rider</label>

                        <input type="radio" className="btn-check" name="roleSelect" id="driver" autoComplete="off"
                               checked={role === "driver"} onChange={() => setRole("driver")}/>
                        <label className="btn btn-secondary driver-toggle" htmlFor="driver"
                               data-mdb-ripple-init>Driver</label>
                    </div>
                    <div>
                        {waiting ?
                            <Spinner animation="border" role="status" style={{color: "#D96C06"}}>
                                <span className="visually-hidden">Loading...</span>
                            </Spinner>
                            :
                            <button type="submit">Submit</button>
                        }
                    </div>

                </form>
                <br/>
                <h3>Already have an account?</h3>
                <br/>
                <div>
                    <button type="submit" onClick={handleSwitchFunc} className="custom-button">Login</button>
                </div>

            </div>
            </div>
        );

    } else {

        return (
            <div className="login-flex">
            <div className="login-wrapper">
                <div className="centered-text">
                    <h1>Please Log In</h1>
                </div>
                <p className="auth-error-message">{errorMessage}</p>
                <form onSubmit={handleLoginSubmit}>
                    <div>
                        <label>
                            <p>Email</p>
                            <input type="text" value={email} onChange={e => setEmail(e.target.value)}/>
                        </label>
                    </div>
                    <div>
                        <label>
                            <p>Password</p>
                            <input type="password" value={password} onChange={e => setPassword(e.target.value)}/>
                        </label>
                    </div>
                    <div>
                        {waiting ?
                            <Spinner animation="border" role="status" style={{color: "#D96C06"}}>
                                <span className="visually-hidden">Loading...</span>
                            </Spinner>
                            :
                            <button type="submit">Submit</button>
                        }
                    </div>
                </form>
                <br/>
                <h3>Don't Have an Account?</h3>
                <br/>
                <div>
                        <button type="submit" onClick={handleSwitchFunc} className="custom-button">Register</button>
                </div>

            </div>
            </div>
        );
    }
}


LoginRegPage.propTypes = {
    setToken: PropTypes.func.isRequired
}

export default LoginRegPage;