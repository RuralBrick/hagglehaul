import React, { useState } from 'react';
import PropTypes from 'prop-types'
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './LoginRegPage.css';

async function loginUser(credentials) {
    return fetch('http://localhost:5250/api/Authentication/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(credentials)
    })
        .then(data => data.json())
}

async function registerUser(credentials) {
    return fetch('http://localhost:5250/api/Authentication/register', {
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
                            <input type="text" onChange={e => setEmail(e.target.value)} />
                            </label>
                    </div>
                    <div>
                        <label>
                            <p>Password</p>
                            <input type="password" onChange={e => setPassword(e.target.value)} />
                            </label>
                    </div>
                    <div class="btn-group">
                        <input type="radio" class="btn-check" name="roleSelect" id="rider" autocomplete="off" checked={role === "rider"} onChange={() => setRole("rider")} />
                        <label class="btn btn-secondary" for="rider" data-mdb-ripple-init>Rider</label>

                        <input type="radio" class="btn-check" name="roleSelect" id="driver" autocomplete="off" checked={role === "driver"} onChange={() => setRole("driver")} />
                        <label class="btn btn-secondary" for="driver" data-mdb-ripple-init>Driver</label>
                    </div>
                    <div>
                        <button type="submit">Submit</button>
                    </div>
                </form>

                <h3>Already have an account?</h3>
                <div>
                    <button type="submit" onClick={handleSwitchFunc} >Login</button>
                </div>

            </div>
        );

    }

    else {

        return (
            <div className="login-wrapper">
                <h1>Please Log In</h1>
                <form onSubmit={handleLoginSubmit}>
                <div>
                    <label>
                        <p>Email</p>
                        <input type="text" onChange={e => setEmail(e.target.value)} />
                        </label>
                </div>
                <div>
                    <label>
                        <p>Password</p>
                        <input type="password" onChange={e => setPassword(e.target.value)} />
                        </label>
                </div>
                    <div>
                        <button type="submit">Submit</button>
                    </div>
                </form>

                <h3>New?</h3>
                <div>
                    <button type="submit" onClick={handleSwitchFunc} >Register</button>
                </div>

            </div>
        );
    }
}


LoginRegPage.propTypes = {
    setToken: PropTypes.func.isRequired
}

export default LoginRegPage;