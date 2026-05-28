import { useEffect, useState } from 'react';
import '../../styles/Admin.css';


function AdminUsers() {
    const [users, setUsers] = useState(null);
    const [loading, setLoading] = useState(true);
    const token = localStorage.getItem('token');

    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = async () => {
        try {
            const response = await fetch('/api/admin/users', {
                headers: {
                    'Authorization': `Bearer ${token}`,
                }
            });
            const data = await response.json();
            if (response.ok) {
                setUsers(data.data);
            }
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const deleteUser = async (id) => {
        if (!confirm('Удалить пользователя?')) return;
        try {
            const response = await fetch(`/api/admin/users/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                }
            });
            if (response.ok) {
                fetchUsers();
            }
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
                        <th>Username</th>
                        <th>Email</th>
                        <th>Дата регистрации</th>
                        <th>Роль</th>
                        <th>Действия</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr key={user.id}>
                            <td>{user.id?.slice(0, 8)}...</td>
                            <td>{user.username}</td>
                            <td>{user.email}</td>
                            <td>{user.registryDate}</td>
                            <td>{user.role === 'Admin' ? 'Админ' : 'Пользователь'}</td>
                            <td>
                                <button className="delete-btn" onClick={() => deleteUser(user.id)}>Удалить</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );

}

export default AdminUsers;