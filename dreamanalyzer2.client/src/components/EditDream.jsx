import { useParams, Link } from 'react-router-dom';
import { useState, useEffect } from 'react';
import '../styles/Forms.css'

function EditDream() {
    const { id } = useParams();
    const token = localStorage.getItem('token');
    const [error, setError] = useState(null);
    const [title, setTitle] = useState("");
    const [content, setContent] = useState("");
    const [dreamDate, setDreamDate] = useState(new Date().toISOString().split('T')[0]);

    useEffect(() => {
        const fetchDream = async () => {
            try {
                const response = await fetch(`/api/Dreams/${id}`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    }
                });

                const data = await response.json();

                if (response.ok) {
                    setTitle(data.data.title);
                    setContent(data.data.content);
                    setDreamDate(data.data.dreamDate.split('T')[0]);
                } else {
                    setError(data.message || 'Failed to load dream');
                }
            } catch (err) {
                setError('Network error: ', err);
            }
        };

        fetchDream();
    }, [id, token])

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(`/api/Dreams/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                },
                body: JSON.stringify({ title, content, dreamDate }),
            });

            const data = await response.json();

            if (response.ok) {
                window.location.href = `/dreams/${id}`;
            } else {
                alert(data.message || 'Dream editing failed');
            }
        }
        catch (err) {
            console.log('Network error: ', err);
            alert('Unable to connect to the server');
        }
    };

    if (error) return <div>Error: {error}</div>;

    return (
        <div>
            <h2>Редактировать сон</h2>
            <form onSubmit={handleSubmit} className="formStyle">
                <div className="inputElement">
                    <label>Заголовок:</label>
                    <input type="text" value={title} onChange={(e) => setTitle(e.target.value)} />
                </div>

                <div className="inputElement">
                    <label>Содержание:</label>
                    <textarea value={content} onChange={(e) => setContent(e.target.value)} rows={6} />
                </div>

                <div className="inputElement">
                    <label>Дата сна:</label>
                    <input type="date" value={dreamDate} onChange={(e) => setDreamDate(e.target.value)} />
                </div>

                <button type="submit" className="submitBtn">Редактировать</button>
                <Link to='/dreams'>
                    <button type="button" className="submitBtn">Отмена</button>
                </Link>
            </form>
        </div>
    );
}

export default EditDream;