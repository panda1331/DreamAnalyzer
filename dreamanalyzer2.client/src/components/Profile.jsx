import { useState, useEffect } from 'react';
import '../styles/Profile.css';
function Profile() {
    const [ profile, setProfile ] = useState(null);
    const token = localStorage.getItem('token');
    useEffect(() => {
        const fetchProfile = async () => {
            const response = await fetch("/api/users", {
                headers: {
                    'Authorization': `Bearer ${token}`,
                },
            });
            const data = await response.json();
            if (response.ok) {
                setProfile(data.data);
            }
        };
        fetchProfile();
    }, [token]);

    if (!profile) return <div>Loading...</div>

    return (
        <div>
            <h2>Личный кабинет</h2>
            <div className="profileContainer">
                <div className="profileInfo">
                    <p>Имя пользователя: {profile.username}</p>
                    <p>Email: {profile.email}</p>
                    <p>Счетчик снов: {profile.dreamsCount}</p>
                    <p>Дата регистрации: {profile.registryDate }</p>
                </div>
            </div>
        </div>
        
    );
}

export default Profile;