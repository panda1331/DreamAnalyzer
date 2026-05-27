import { useState, useEffect } from 'react';
import '../styles/Profile.css';
import Statistics from '../components/Statistics';

function Profile() {
    const [profile, setProfile] = useState(null);
    const [activeTab, setActiveTab] = useState('profile');
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
        <div className="profile-container">
            <h2>Личный кабинет</h2>
            <div className="profile-tabs">
                <button
                    className={`tab-btn ${activeTab === 'profile' ? 'active' : ''}`}
                    onClick={() => setActiveTab('profile')}>
                    Профиль
                </button>
                <button
                    className={`tab-btn ${activeTab === 'stats' ? 'active' : ''}`}
                    onClick={() => setActiveTab('stats')}>
                    Статистика
                </button>
            </div>

            {activeTab === 'profile' ? (
                <div className="profileInfo">
                    <p><strong>👨‍💼 Имя пользователя:</strong> {profile.username}</p>
                    <p><strong>📧 Email:</strong> {profile.email}</p>
                    <p><strong>📅 Дата регистрации:</strong> {new Date(profile.registryDate).toLocaleDateString()}</p>
                </div>
            ) : (
                <Statistics />
            )}
        </div>
        
    );
}

export default Profile;