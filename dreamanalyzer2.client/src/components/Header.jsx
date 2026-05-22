import '../styles/Header.css';
import { Link } from 'react-router-dom';

function Header() {

    return (
        <header className="header">
            <Link to="/"><h2>Dream Analyzer</h2></Link> 
            <nav className="navigation">
                <Link to="/login"><button>Login</button></Link>
                <Link to="/register" ><button id="registerBtn">Register</button></Link>
            </nav>
        </header>
  );
}

export default Header;