import '../styles/Header.css';
import { Link } from 'react-router-dom';
import ConfirmModal from './ConfirmModal';
import { useState } from 'react';

function Header() {
    const token = localStorage.getItem('token');
    const [showModal, setShowModal] = useState(false);

    const handleLogout = () => {
        localStorage.removeItem('token');
        window.location.href = '/';
    };

    return (
        <header className="header">
            <Link to="/"><h2 className="headerTitle">Dream Analyzer</h2></Link> 
            <nav className="navigation">
                {token ? (
                    <>
                        <Link to="/dreams"><button>Мои сны</button></Link>
                        <Link to="/profile"><button>Профиль</button></Link>
                        <button onClick={() => setShowModal(true)}>Выйти</button>
                        <ConfirmModal
                            isOpen={showModal}
                            onConfirm={ handleLogout }
                            onCancel={() => setShowModal(false)}
                            message="Are you sure you want to logout?" />
                    </>
                ) : (
                    <>
                        <Link to="/login"><button>Войти</button></Link>
                        <Link to="/register" ><button id="registerBtn">Зарегистрироваться</button></Link>
                    </>
                )}
            </nav>
        </header>
  );
}

export default Header;