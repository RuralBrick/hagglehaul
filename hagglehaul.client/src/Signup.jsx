import React, { useState } from 'react';
import { Link } from 'react-router-dom'; 
import './Signup.css'; 

const Signup = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isEmailValid, setIsEmailValid] = useState(false);
  const [isPasswordValid, setIsPasswordValid] = useState(false);
  const [isAgreeToList, setIsAgreeToList] = useState(false);
  const [isAgreeToTOS, setIsAgreeToTOS] = useState(false);
  const [signupMessage, setSignupMessage] = useState('');
  const [userType, setUserType] = useState('Driver');

  const handleSignup = async () => {
    if (!isEmailValid) {
      setSignupMessage('Please enter a valid email address.');
      return;
    }

    if (!isPasswordValid) {
      setSignupMessage('Password must be between 2 and 15 characters.');
      return;
    }

    if (!isAgreeToTOS) {
      setSignupMessage('Please agree to the Terms of Service.');
      return;
    }

    try {
      // Perform signup logic   TODo

      setSignupMessage('Signup successful!');
    } catch (error) {
      setSignupMessage('Error during signup. Please try again.');
    }
  };

  const validateEmail = (inputEmail) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const isValid = emailRegex.test(inputEmail);
    setIsEmailValid(isValid);
    return isValid;
  };

  const validatePassword = (inputPassword) => {
    const isValid = inputPassword.length >= 2 && inputPassword.length <= 15;
    setIsPasswordValid(isValid);
    return isValid;
  };

  return (
    <div className="signup-container">
      <h2>Sign Up</h2>
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
        {password !== '' && (
          <>
            <span className={isPasswordValid ? 'check-mark' : 'cross-mark'} />
            <p>Password must be between 2 and 15 characters.</p>
          </>
        )}
        <br />

        <label>User Type:</label>
        <select value={userType} onChange={(e) => setUserType(e.target.value)}>
          <option value="Driver">Driver</option>
          <option value="Rider">Rider</option>
        </select>
        <br />

        <label>
          <input type="checkbox" onChange={() => setIsAgreeToList(!isAgreeToList)} />
          Agree to Mailing List
        </label>
        <br />

        <label>
          <input type="checkbox" onChange={() => setIsAgreeToTOS(!isAgreeToTOS)} required />
          Agree to Terms of Service
        </label>
        <br />

        <button type="button" onClick={handleSignup}>
          Sign Up
        </button>
      </form>

      {signupMessage && <p className={signupMessage.includes('successful') ? 'success-message' : 'error-message'}>{signupMessage}</p>}

      <p>
        Already have an account?{' '}
        <Link to="/signin" style={{ color: '#007bff', textDecoration: 'underline' }}>
          Sign in here
        </Link>
      </p>
    </div>
  );
};

export default Signup;
