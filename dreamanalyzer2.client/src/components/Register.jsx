import "../styles/Forms.css";
import { useState } from 'react';
function Register() {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch("/api/authentication/register", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ username, email, password }),
            });

            const data = await response.json();

            if (response.ok) {
                localStorage.setItem('token', data.data.token);
                window.location.href = '/profile';
            } else {
                alert(data.message || 'Register failed');
            }
        } catch (error) {
            console.error('Network error: ', error);
            alert('Unable to connect to the server');
        }

        console.log({ username, email, password });
    };

    return (
        <div>
            <p>Приветствуем в нашем анализаторе снов! Вы здесь впервые? Зарегистрируйтесь и узнайте все тайны своих сновидений!</p>
            <form className="formStyle" onSubmit={ handleSubmit }>
                <div className="inputElement">
                    <label htmlFor="username">Имя пользователя: </label>
                    <input required="true" id="username" type="text" value={username} onChange={ (e) => setUsername(e.target.value) } />
                </div>
                <div className="inputElement">
                    <label htmlFor="email">Email: </label>
                    <input required="true" id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
                </div>
                <div className="inputElement">
                    <label htmlFor="password">Пароль: </label>
                    <input required="true" id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                </div>
                <button type="submit" className="submitBtn">Register</button>
            </form>
        </div>
    )
}

export default Register;