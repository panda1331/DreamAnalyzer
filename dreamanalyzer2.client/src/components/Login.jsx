import { useState } from "react";
import "../styles/Login.css";

function Login() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")

    function onChangeEmail(e) {
        setEmail(e.target.value);
    }
    function onChangePassword(e) {
        setPassword(e.target.value);
    }

    return (
        <div className="login-form">
            <div>
                <label htmlFor="emailInput" >Email: </label>
                <input id="emailInput" className="input-field" type="text" value={email} onChange={onChangeEmail} />
            </div>
            <div>
                <label htmlFor="passwordInput">Password: </label>
                <input id="passwordInput" className="input-field" type="password" value={password} onChange={onChangePassword} />
            </div>
        </div>
    )
}

export default Login;