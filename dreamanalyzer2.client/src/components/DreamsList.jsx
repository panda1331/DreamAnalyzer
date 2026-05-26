import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import '../styles/Dreams.css'
function DreamsList() {
    const [dreams, setDreams] = useState(null);
    const token = localStorage.getItem('token');

    useEffect(() => {
        const fetchDreams = async () => {
            if (!token) {
                console.log("no token")
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
            }
        };

        fetchDreams();
    }, [token]);

    return (
        <div>
            <div className="dreamTitleContainer">
                <h2 className="myDreamsTitle">Мои сны</h2>
                <Link to="/dreams/create"><button className="dreamTitleContainerCreateBtn">Добавить сон</button></Link> 
            </div>
           
            {dreams && dreams.length === 0 && <p>У вас пока нет снов</p>}
            <div className="dreamsList">
                {dreams && dreams.map(dream => (
                    <Link to={`/dreams/${dream.id}`}> <div key={dream.id} className="dreamCard">
                        <div className="dreamCardContainerHeader">
                            
                            <h3>{dream.title}</h3>
                        </div>
                        
                        <p>{dream.content}</p>
                        
                    </div></Link>
                ))}
            </div>
        </div>
    )
}

export default DreamsList;