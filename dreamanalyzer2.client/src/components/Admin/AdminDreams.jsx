import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import '../../styles/Admin.css';

function AdminDreams() {
    const [dreams, setDreams] = useState([]);
    const [loading, setLoading] = useState(true);
    const token = localStorage.getItem('token');

    useEffect(() => {
        fetchDreams();
    }, []);

    const fetchDreams = async () => {
        try {
            const response = await fetch('/api/admin/dreams', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            const data = await response.json();
            if (response.ok)
                setDreams(data.data);
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const deleteDream = async (id) => {
        if (!confirm('Удалить сон?')) return;
        try {
            const response = await fetch(`/api/admin/dreams/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            if (response.ok)
                fetchDreams();
        } catch (err) {
            console.error(err);
        }
    };

    if (loading) return <div>Загрузка...</div>;

    return (
        <div className="admin-table-container">
            <table className="admin-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Заголовок</th>
                        <th>Содержание</th>
                        <th>Дата сна</th>
                        <th>Действия</th>
                    </tr>
                </thead>
                <tbody>
                    {dreams.map(dream => (
                        <tr key={dream.id}>
                            <td>{dream.id?.slice(0, 8)}...</td>
                            <td>{dream.title}</td>
                            <td className="dream-content-cell">{dream.content?.slice(0, 50)}...</td>
                            <td>{new Date(dream.dreamDate).toLocaleDateString()}</td>
                            <td>
                                <Link to={`/dreams/${dream.id}`}>
                                    <button className="view-btn">Просмотр</button>
                                </Link>
                                <button className="delete-btn" onClick={() => deleteDream(dream.id)}>Удалить</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default AdminDreams;