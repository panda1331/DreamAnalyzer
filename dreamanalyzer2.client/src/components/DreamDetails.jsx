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

    const [analysis, setAnalysis] = useState(null);
    const [aiLoading, setAiLoading] = useState(false);
    const [aiError, setAiError] = useState(null);
    const [activeStrategy, setActiveStrategy] = useState(null);

    const strategies = [
        { id: 'freudian', label: 'Психоанализ Фрейда' },
        { id: 'jungian', label: 'Архетипы Юнга' },
        { id: 'cognitive', label: 'КПТ-анализ' },
        { id: 'symbols', label: 'Словарь символов в бд' },
    ];

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

    const handleAnalyzeClick = async (strategyType) => {
        setAiLoading(true);
        setAiError(null);
        setActiveStrategy(strategyType);
        setAnalysis(null);

        try {
            const response = await fetch(`/api/Dreams/${id}/analyze?strategyType=${strategyType}`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                }
            });

            const data = await response.json();

            if (response.ok) {
                setAnalysis(data.data);
            } else {
                setAiError(data.message || "Couldn't load AI analysis");
            }
        } catch (err) {
            setAiError("Network request error", err);
        } finally {
            setAiLoading(false);
        }
    };

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

    if (error) return <div>Error: {error}</div>;
    if(!dream) return <div>Dream not found</div>;

    return (
        <div>
            <div className="dreamTitleContainer">
                <h2>Детали сна:</h2>
                <div className="buttons">
                    <Link to={`/dreams/${dream.id}/edit`}><button>Редактировать</button></Link> 
                    <button onClick={handleDeleteClick}>Удалить</button>

                    <ConfirmModal
                        isOpen={showModal}
                        onConfirm={handleConfirmDelete}
                        onCancel={() => setShowModal(false)}
                        message="Вы уверены, что хотите удалить этот сон?" />
                </div>
            </div>
            <div className="detailsContainer">
                <div className="dreamCardContainerHeader">
                    <h4>{dream.title}</h4>
                </div>
                <p>{dream.content}</p>
                <p>{new Date(dream.dreamDate).toLocaleDateString()}</p>
            </div>

            <div className="ai-analysis-section">
                <h3>Аналитические модули системы</h3>
                <p className="ai-subtitle">Выберите парадигму для интерпретации содержания сна:</p>

                <div className="strategy-tabs">
                    {strategies.map((strat) => (
                        <button
                            key={strat.id}
                            className={`strategy-tab-btn ${activeStrategy === strat.id ? 'active' : ''}`}
                            onClick={() => handleAnalyzeClick(strat.id)}
                            disabled={aiLoading}>
                            {strat.label}
                        </button>
                    ))}
                </div>

                {aiLoading && (
                    <div className="ai-loader">
                        <div className="ai-spinner"></div>
                        <p>Выполняется обработка алгоритмами анализа...</p>
                    </div>
                )}

                {aiError && <div className="ai-error">Ошибка анализа: {aiError}</div>}

                {analysis && !aiLoading && (
                    <div className="ai-result-card">
                        <div className="ai-result-header">
                            <h4>
                                {strategies.find(s => s.id === analysis.strategy || s.id + 's' === analysis.strategy)?.label}
                            </h4>
                            <span
                                className="ai-mood-badge"
                                style={analysis.moodColor ? { backgroundColor: analysis.moodColor, color: '#fff' } : {}}>
                                Настроение: <b>{analysis.moodName}</b>
                            </span>
                        </div>

                        {analysis.strategy === 'symbols' ? (
                            <div className="db-symbols-container">
                                {analysis.symbols && analysis.symbols.length > 0 ? (
                                    <div className="symbols-db-list">
                                        <div className="symbols-grid">
                                            {analysis.interpretation.split('; ').map((item, idx) => {
                                                if (!item.includes(':')) return null;
                                                const [name, desc] = item.split(': ');
                                                return (
                                                    <div key={idx} className="symbol-db-card">
                                                        <div className="symbol-db-name"><h4>{name}</h4></div>
                                                        <div className="symbol-db-desc">{desc}</div>
                                                    </div>
                                                );
                                            })}
                                        </div>
                                    </div>
                                ) : (
                                    <p className="no-symbols-text">Стеммер завершил поиск. Совпадений с базой данных символов не обнаружено.</p>
                                )}
                            </div>
                        ) : (
                            <>
                                <div className="ai-interpretation-text" dangerouslySetInnerHTML={{ __html: analysis.interpretation }} />

                                {analysis.symbols && analysis.symbols.length > 0 && (
                                    <div className="ai-symbols-block">
                                        <h5>Выделенные ИИ семантические маркеры:</h5>
                                        <div className="ai-symbols-list">
                                            {analysis.symbols.map((symbol, idx) => (
                                                <span key={idx} className="ai-symbol-tag">{symbol}</span>
                                            ))}
                                        </div>
                                    </div>
                                )}
                            </>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
}

export default DreamDetails;