import "../styles/Forms.css";
import { useState } from 'react';
function Register() {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const handleSubmit = (e) => {
        e.preventDefault();
        // тут сделать вызов api моего конторолера
        console.log({ username, email, password });
    };

    return (
        <div>
            <p>Hi, welcome to our analyzer! You're new here? Register below and find out all the mysteries of your dreams!</p>
            <form className="formStyle" onSubmit={ handleSubmit }>
                <div className="inputElement">
                    <label htmlFor="username">Username: </label>
                    <input id="username" type="text" value={username} onChange={ (e) => setUsername(e.target.value) } />
                </div>
                <div className="inputElement">
                    <label htmlFor="email">Email: </label>
                    <input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
                </div>
                <div className="inputElement">
                    <label htmlFor="password">Password: </label>
                    <input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                </div>
                <button type="submit" className="submitBtn">Register</button>
            </form>
        </div>
    )
}

export default Register;