import { useState } from "react";
import { Link } from 'react-router-dom';
import "../styles/Forms.css";

function Login() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")

    const handleSubmit = (e) => {
        e.preventDefault();
        // тоже отправка на контроллер
        console.log({ email, password });
    }
    return (
        <form className="formStyle" onSubmit={ handleSubmit }>
            <div className="inputElement">
                <label htmlFor="emailInput" >Email: </label>
                <input id="emailInput" className="input-field" type="text" value={email} onChange={(e) => setEmail(e.target.value)} />
            </div>
            <div className="inputElement">
                <label htmlFor="passwordInput">Password: </label>
                <input id="passwordInput" className="input-field" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
            </div>
            <button className="submitBtn" type="submit">Login</button>
            <Link to="/register"><p>Don't have an account? Click here to register</p></Link> 
        </form>
    )
}

export default Login;