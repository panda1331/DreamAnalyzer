import { useState } from 'react';
import '../styles/Forms.css';

function AddDream() {
    const [title, setTitle] = useState("");
    const [content, setContent] = useState("");
    const [dreamDate, setDreamDate] = useState(new Date().toISOString().split('T')[0]);
    const token = localStorage.getItem('token');

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch("/api/Dreams", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                },
                body: JSON.stringify({ title, content, dreamDate }),
            });

            const data = await response.json();

            if (response.ok) {
                window.location.href = '/dreams';
            } else {
                alert(data.message || 'Dream creation failed');
            }
        }
        catch (error) {
            console.log('Network error: ', error);
            alert('Unable to connect to the server');
        }
    }

    return (
        <div>
            <h2>Опиши свой сон:</h2>
            <form className="formStyle" onSubmit={ handleSubmit }>
                <div className="inputElement">
                    <label>Название: </label>
                    <input type="text" value={title} onChange={ (e) => setTitle(e.target.value) } />
                </div>
                <div className="inputElement">
                    <label>Описание: </label>
                    <textarea type="text" value={content} onChange={(e) => setContent(e.target.value)} />
                </div>
                <div className="inputElement">
                    <label>Дата сна: </label>
                    <input type="date" value={dreamDate} onChange={(e) => setDreamDate(e.target.value)} />
                </div>
                <button type="submit" className="submitBtn">Добавить</button>
            </form>
        </div>
    );
}

export default AddDream;