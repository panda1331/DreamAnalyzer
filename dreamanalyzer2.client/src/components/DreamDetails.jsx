import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Link } from 'react-router-dom';
import "../styles/Dreams.css"
import ConfirmModal from './ConfirmModal';

function DreamDetails() {
    const token = localStorage.getItem('token');
    const { id } = useParams();
    const [dream, setDream] = useState(null);
    const [error, setError] = useState(null);

    const [showModal, setShowModal] = useState(false);

    useEffect(() => {
        const fetchDream = async () => {
            try {
                const response = await fetch(`/api/Dreams/${id}`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    }
                });

                const data = await response.json();

                if (response.ok) {
                    setDream(data.data);
                } else {
                    setError(data.message || 'Failed to load dream');
                }
            } catch (err) {
                setError('Network error: ', err);
            }
        };

        fetchDream();
    }, [id, token]);

    const handleDeleteClick = () => {
        setShowModal(true);
    };
    const handleConfirmDelete = async () => {
        setShowModal(false);

        const response = await fetch(`/api/Dreams/${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            console.log('successfully deleted');
            window.location.href = '/dreams';
        } else {
            console.log('problems with deleting...');
        }
    }

    // const handleDelete = async () => {
    //     const confirmed = window.confirm("Are you sure you want to delete this dream?");

    //     if (!confirmed) {
    //         return;
    //     }

    //     try {
    //         const response = await fetch(`/api/Dreams/${id}`, {
    //             method: 'DELETE',
    //             headers: {
    //                 'Authorization': `Bearer ${token}`,
    //                 'Content-Type': 'application/json',
    //             },
    //         });

    //         if (response.ok) {
    //             console.log('successfully deleted');
    //             window.location.href = '/dreams';
    //         } else {
    //             console.log('problems with deleting...');
    //         }
    //     } catch (err) {
    //         console.error('network error: ', err);
    //     }
    // }

    if (error) return <div>Error: {error}</div>;
    if(!dream) return <div>Dream not found</div>;

    return (
        <div>
            <div className="dreamTitleContainer">
                <h2>Dream details:</h2>
                <div className="buttons">
                    <Link to={`/dreams/${dream.id}/edit`}><button>Edit</button></Link> 
                    <button onClick={handleDeleteClick}>Delete</button>

                    <ConfirmModal
                        isOpen={showModal}
                        onConfirm={handleConfirmDelete}
                        onCancel={() => setShowModal(false)}
                        message="Are you sure you want to delete this dream?" />
                </div>
            </div>
            <div className="detailsContainer">
                <h4>{dream.title}</h4>
                <p>{dream.content}</p>
                <p>{new Date(dream.dreamDate).toLocaleDateString()}</p>
            </div>
        </div>
    );
}

export default DreamDetails;