import '../styles/Header.css';
import { Link } from 'react-router-dom';

function Header() {

    return (
        <header className="header">
            <Link to="/"><h2>Dream Analyzer</h2></Link> 
            <nav className="navigation">
                <Link to="/login" className="nav-button">Login</Link>
                <button className="nav-button">Register</button>
            </nav>
        </header>
  );
}

export default Header;