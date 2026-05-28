import { useState } from "react";
import AdminUsers from "./AdminUsers";
import AdminDreams from "./AdminDreams";
import '../../styles/Admin.css';


function AdminLayout() {
    const [activeTab, setActiveTab] = useState('users');

    return (
        <div className="admin-container">
            <h2>Админ панель</h2>
            <div className="admin-tabs">
                <button className={activeTab === 'users' ? 'active' : ''} onClick={() => setActiveTab('users')}>Пользователи</button>
                <button className={activeTab === 'dreams' ? 'active' : ''} onClick={() => setActiveTab('dreams')}>Сны</button>
            </div>

            <div className="admin-content">
                {activeTab === 'users' && <AdminUsers />}
                {activeTab === 'dreams' && <AdminDreams />}
            </div>
        </div>
    );
}

export default AdminLayout;