import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Signin.css';

const Signin = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isEmailValid, setIsEmailValid] = useState(false);
  const [isPasswordValid, setIsPasswordValid] = useState(false);
  const [signinMessage, setSigninMessage] = useState('');
  const navigate = useNavigate();

  const handleSignin = async () => {

    try {
      // Perform signin logic TODO

      setSigninMessage('Signin successful!');
      navigate('/dashboard');
    } catch (error) {
      setSigninMessage('Error during signin. Please try again.');
    }
  };

  return (
    <div className="signin-container">
      <h2>Sign In</h2>
      <form>
        <label>Email:</label>
        <input
          type="email"
          value={email}
          onChange={(e) => {
            setEmail(e.target.value);
            validateEmail(e.target.value);
          }}
          className={email !== '' ? (isEmailValid ? 'valid' : 'invalid') : ''}
        />
        {email !== '' && <span className={isEmailValid ? 'check-mark' : 'cross-mark'} />}
        <br />

        <label>Password:</label>
        <input
          type="password"
          value={password}
          onChange={(e) => {
            setPassword(e.target.value);
            validatePassword(e.target.value);
          }}
          className={password !== '' ? (isPasswordValid ? 'valid' : 'invalid') : ''}
        />
        {password !== '' && <span className={isPasswordValid ? 'check-mark' : 'cross-mark'} />}
        <br />

        <button type="button" onClick={handleSignin}>
          Sign In
        </button>
      </form>

      {signinMessage && <p className={signinMessage.includes('successful') ? 'success-message' : 'error-message'}>{signinMessage}</p>}

      <p>
        Don't have an account?{' '}
        <Link to="/signup" style={{ color: '#007bff', textDecoration: 'underline' }}>
          Sign up here
        </Link>
      </p>
    </div>
  );
};

export default Signin;

