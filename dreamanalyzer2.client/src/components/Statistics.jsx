import { useState, useEffect } from 'react';
import "../styles/Statistics.css";

function Statistics() {
    const [stats, setStats] = useState(null);
    const [loading, setLoading] = useState(false);
    const token = localStorage.getItem('token');

    useEffect(() => {
        const fetchStats = async () => {
            try {
                const response = await fetch('/api/users/statistics', {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    }
                });

                const data = await response.json();

                if (response.ok) {
                    setStats(data.data);
                }
            } catch (err) {
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchStats();
    }, [token]);

    if (loading) return <div>Загрузка статистики...</div>;
    if (!stats) return <div>Нет данных</div>;

    return (
        <div className="statistics-container">
            <div className="stats-cards">
                <div className="stat-card">
                    <div className="stat-value">{stats.totalDreams}</div>
                    <div className="stat-label">Всего снов</div>
                </div>
                <div className="stat-card">
                    <div className="stat-value">{stats.averageDreamLength}</div>
                    <div className="stat-label">Средняя длина (символов)</div>
                </div>
            </div>

            <div className="stats-section">
                <h3>Сны по дням недели</h3>
                <div className="weekday-bars">
                    {Object.entries(stats.dreamsByWeekDay).map(([day, count]) => (
                        <div key={day} className="weekday-bar">
                            <span className="weekday-name">{day}</span>
                            <div className="bar-container">
                                <div className="bar-fill" style={{ width: `${(count / stats.totalDreams) * 100}%` }}></div>
                            </div>
                            <span className="weekday-count">{count}</span>
                        </div>
                    ))}
                </div>
            </div>

            <div className="stats-section">
                <h3>Настроения снов</h3>
                <div className="mood-pie">
                    {Object.entries(stats.moodDistribution).map(([mood, count]) => {
                        const colors = { Peaceful: '#10b981', Anxious: '#f59e0b', NightMare: '#8b5cf6', Lucid: '#06b6d4' };
                        return (
                            <div key={mood} className="mood-item">
                                <div className="mood-color" style={{ backgroundColor: colors[mood] || '#888' }}></div>
                                <span>{mood}: {count}</span>
                            </div>
                        );
                    })}
                </div>
            </div>

            {stats.popularSymbols.length > 0 && (
                <div className="stats-section">
                    <h3>Популярные символы</h3>
                    <div className="symbols-tags">
                        {stats.popularSymbols.map(s => (
                            <div key={s.symbolName} className="symbol-tag">
                                <span className="symbol-name">{s.symbolName}</span>
                                <span className="symbol-count">{s.count}</span>
                            </div>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
}

export default Statistics;