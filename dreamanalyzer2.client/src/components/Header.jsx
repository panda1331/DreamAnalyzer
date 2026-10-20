import '../styles/Header.css';
import { Link } from 'react-router-dom';
import ConfirmModal from './ConfirmModal';
import { useState } from 'react';

function Header() {
    const token = localStorage.getItem('token');
    const [showModal, setShowModal] = useState(false);
    const [isMenuOpen, setIsMenuOpen] = useState(false);

    let userRole = null;
    if (token) {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            userRole = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        } catch (e) {
            console.error('Ошибка декодирования токена', e);
        }
    }

    const handleLogout = () => {
        localStorage.removeItem('token');
        window.location.href = '/';
    };

    const toggleMenu = () => setIsMenuOpen(!isMenuOpen);
    const closeMenu = () => setIsMenuOpen(false);

    return (
        <header className="header">
            <div className="header-top">
                <Link to="/" onClick={closeMenu}>
                    <h2 className="headerTitle">Dream Analyzer</h2>
                </Link>

                <button className="burger-menu" onClick={toggleMenu}>
                    {isMenuOpen ? '✕' : '☰'}
                </button>
            </div>

            <nav className={`navigation ${isMenuOpen ? 'active' : ''}`}>
                {token ? (
                    <>
                        <Link to="/dreams" onClick={closeMenu}><button>Мои сны</button></Link>
                        <Link to="/profile" onClick={closeMenu}><button>Профиль</button></Link>
                        {userRole === 'Admin' && (
                            <Link to="/admin" onClick={closeMenu}><button>Админка</button></Link>
                        )}
                        <button onClick={() => { setShowModal(true); closeMenu(); }}>Выйти</button>

                        <ConfirmModal
                            isOpen={showModal}
                            onConfirm={handleLogout}
                            onCancel={() => setShowModal(false)}
                            message="Вы уверены, что хотите выйти?" />
                    </>
                ) : (
                    <>
                        <Link to="/login" onClick={closeMenu}><button>Войти</button></Link>
                        <Link to="/register" onClick={closeMenu}><button id="registerBtn">Зарегистрироваться</button></Link>
                    </>
                )}
            </nav>
        </header>
    );
}

export default Header;