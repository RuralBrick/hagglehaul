import React, { useState } from 'react';
import PropTypes from 'prop-types'
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
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
}

function LoginRegPage({ setToken }) {

    const [email, setEmail] = useState();
    const [password, setPassword] = useState();
    const [role, setRole] = useState("rider");
    const [func, setFunc] = useState(false);
 

    const handleLoginSubmit = async e => {
        e.preventDefault();
        const token = await loginUser({
            email,
            password
        });
        setToken(token);
    }

    const handleRegisterSubmit = async e => {
        e.preventDefault();
        const token = await registerUser({
            email,
            password,
            role
        });
        setFunc(false)
    }

    const handleSwitchFunc = async e => {
        e.preventDefault();
        setFunc(!func)
    }

    if (func) {

        return (
            <div className="login-wrapper">
                <h1>Please Register</h1>
                <form onSubmit={handleRegisterSubmit}>
                    <div>
                        <label>
                            <p>Email</p>
                            <input type="text" onChange={e => setEmail(e.target.value)}/>
                        </label>
                    </div>
                    <div>
                        <label>
                            <p>Password</p>
                            <input type="password" onChange={e => setPassword(e.target.value)}/>
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
                        <button type="submit">Submit</button>
                    </div>

                </form>
                <br/>
                <h3>Already have an account?</h3>
                <br/>
                <div>
                    <button type="submit" onClick={handleSwitchFunc} className="custom-button">Login</button>
                </div>

            </div>
        );

    } else {

        return (
            <div className="login-wrapper">
                <div className="centered-text">
                    <h1>Please Log In</h1>
                </div>
                <form onSubmit={handleLoginSubmit}>
                    <div>
                        <label>
                            <p>Email</p>
                            <input type="text" onChange={e => setEmail(e.target.value)}/>
                        </label>
                    </div>
                    <div>
                        <label>
                            <p>Password</p>
                            <input type="password" onChange={e => setPassword(e.target.value)}/>
                        </label>
                    </div>
                    <div>
                        <button type="submit">Submit</button>
                    </div>
                </form>
                <br/>
                <h3>Don't Have an Account?</h3>
                <br/>
                <div>
                    <button type="submit" onClick={handleSwitchFunc} className="custom-button">Register</button>
                </div>

            </div>
        );
    }
}


LoginRegPage.propTypes = {
    setToken: PropTypes.func.isRequired
}

export default LoginRegPage;