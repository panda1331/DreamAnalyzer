import { useState } from "react";
import { Link } from 'react-router-dom';
import "../styles/Forms.css";

function Login() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch("/api/authentication/login", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            const data = await response.json();

            if (response.ok) {
                localStorage.setItem('token', data.data.token);
                window.location.href = '/dreams';
            } else {
                alert(data.message || 'Login failed');
            }
        } catch (error) {
            console.error('Network error: ', error);
            alert('Unable to connect to the server');
        }

        console.log({ email, password });
    }
    return (
        <form className="formStyle" onSubmit={ handleSubmit }>
            <div className="inputElement">
                <label htmlFor="emailInput" >Email: </label>
                <input required="true" id="emailInput"type="text" value={email} onChange={(e) => setEmail(e.target.value)} />
            </div>
            <div className="inputElement">
                <label htmlFor="passwordInput">Пароль: </label>
                <input required="true" id="passwordInput"type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
            </div>
            <button className="submitBtn" type="submit">Login</button>
            <Link to="/register"><p>У вас нету аккаунта? Нажмите здесь, чтобы зарегистрироваться</p></Link> 
        </form>
    )
}

export default Login;