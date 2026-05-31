import "../styles/Forms.css";
import { useState } from 'react';
function Register() {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [passwordError, setPasswordError] = useState("");
    const [serverError, setServerError] = useState("");
    const [isLoading, setIsLoading] = useState(false);

    const validatePassword = (pass) => {
        if (pass.length < 8) {
            return "Пароль должен содержать минимум 8 символов";
        }
        if (!/[A-Z]/.test(pass)) {
            return "Пароль должен содержать хотя бы одну заглавную букву";
        }
        if (!/[0-9]/.test(pass)) {
            return "Пароль должен содержать хотя бы одну цифру";
        }
        return "";
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setPasswordError("");
        setServerError("");

        const error = validatePassword(password);
        if (error) {
            setPasswordError(error);
            return;
        }

        setIsLoading(true);

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
                setServerError(data.message || 'Регистрация отклонена сервером');
            }
        } catch (error) {
            console.error('Network error: ', error);
            setServerError('Не удалось установить соединение с сервером. Попробуйте позже.');
        } finally {
            setIsLoading(false);
        }

        console.log({ username, email, password });
    };

    return (
        <div>
            <p>Приветствуем в нашем анализаторе снов! Вы здесь впервые? Зарегистрируйтесь и узнайте все тайны своих сновидений!</p>
            <form className="formStyle" onSubmit={handleSubmit}>
                {serverError && <div className="error-summary">{serverError}</div>}
                <div className="inputElement">
                    <label htmlFor="username">Имя пользователя: </label>
                    <input required="true" id="username" type="text" value={username} onChange={(e) => { setUsername(e.target.value); if (serverError) setServerError(""); }} />
                </div>
                <div className="inputElement">
                    <label htmlFor="email">Email: </label>
                    <input required="true" id="email" type="email" value={email} onChange={(e) => { setEmail(e.target.value); if (serverError) setServerError(""); }} />
                </div>
                <div className="inputElement">
                    <label htmlFor="password">Пароль: </label>
                    <input required="true" id="password" type="password" value={password} onChange={(e) => { setPassword(e.target.value); setPasswordError(""); if (serverError) setServerError(""); }} />
                </div>
                {passwordError && <div className="error-message">{passwordError}</div>}

                <button
                    type="submit"
                    className="submitBtn"
                    disabled={isLoading}
                >
                    {isLoading ? "Регистрация..." : "Зарегистрироваться"}
                </button>
            </form>
        </div>
    )
}

export default Register;