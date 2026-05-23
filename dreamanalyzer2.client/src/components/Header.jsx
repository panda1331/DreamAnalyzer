import '../styles/Header.css';
import { Link } from 'react-router-dom';

function Header() {
    const token = localStorage.getItem('token');

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
                        <Link to="/dreams"><button>My dreams</button></Link>
                        <Link to="/profile"><button>Profile</button></Link>
                        <button onClick={ handleLogout }>Logout</button>
                    </>
                ) : (
                    <>
                        <Link to="/login"><button>Login</button></Link>
                        <Link to="/register" ><button id="registerBtn">Register</button></Link>
                    </>
                )}
            </nav>
        </header>
  );
}

export default Header;