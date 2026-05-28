import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import '../styles/Dreams.css'
function DreamsList() {
    const [dreams, setDreams] = useState(null);
    const [filteredDreams, setFilteredDreams] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [loading, setLoading] = useState(true);
    const token = localStorage.getItem('token');

    useEffect(() => {
        const fetchDreams = async () => {
            if (!token) {
                console.log("no token")
                setLoading(false);
                return;
            }

            try {
                const response = await fetch('/api/Dreams/', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                });
                const data = await response.json();

                if (response.ok) {
                    setDreams(data.data);
                    console.log('Dreams from server:', data.data);

                } else {
                    console.error('Error', data.message);
                }
            }
            catch (err) {
                console.error('Connection error', err);
            } finally {
                setLoading(false);
            }
        };

        fetchDreams();
    }, [token]);

    useEffect(() => {
        if (!dreams) return;

        if (searchTerm.trim() === '') {
            setFilteredDreams(dreams);
        } else {
            const filtered = dreams.filter(dream =>
                dream.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
                dream.content.toLowerCase().includes(searchTerm.toLowerCase()));
            setFilteredDreams(filtered);
        }
    }, [searchTerm, dreams]);

    if (loading) {
        return (
            <div className="dream-container">
                <div className="dreamTitleContainer">
                    <h2 className="myDreamsTitle">Мои сны</h2>
                </div>
                <p>Загрузка снов...</p>
            </div>
        );
    }

    return (
        <div className="dreams-container" >
            <div className="dreamTitleContainer">
                <h2 className="myDreamsTitle">Мои сны</h2>
                <Link to="/dreams/create"><button className="dreamTitleContainerCreateBtn">Добавить сон</button></Link> 
            </div>

            <div className="search-container">
                <input
                    type="text"
                    placeholder="Поиск по снам..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="search-input" />
                {searchTerm && (
                    <button className="search-clear" onClick={() => setSearchTerm('')}>x</button>
                )}
            </div>

            {filteredDreams && filteredDreams.length === 0 && (
                <p className="no-dreams">Сны не найдены</p>
            )}

            {dreams && dreams.length === 0 && <p>У вас пока нет снов</p>}

            <div className="dreamsList">
                {filteredDreams && filteredDreams.map(dream => (
                    <Link to={`/dreams/${dream.id}`}> <div key={dream.id} className="dreamCard">
                        <div className="dreamCardContainerHeader">
                            <h3>{dream.title}</h3>
                        </div>
                        <p className="dream-content-text">{dream.content}</p>
                    </div></Link>
                ))}
            </div>
        </div>
    )
}

export default DreamsList;