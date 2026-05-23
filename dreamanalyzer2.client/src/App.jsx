// import { useEffect, useState } from 'react';
import './App.css';
import Header from './components/Header';
import Footer from './components/Footer';
import Login from './components/Login';
import Info from './components/Info';
import Register from './components/Register'
import { Routes, Route } from 'react-router-dom';
import DreamsList from './components/DreamsList';
import PrivateRoute from './components/PrivateRoute';

function App() {
    return (
        <div className="mainDiv">
            <Header />
            <main>
                <Routes>
                    <Route path="/" element={<Info />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/dreams" element={
                        <PrivateRoute>
                            <DreamsList />
                        </PrivateRoute>
                    } />
                </Routes>
            </main>
            <Footer />
        </div>
    )
}

export default App;


// const [forecasts, setForecasts] = useState();

// useEffect(() => {
//     populateWeatherData();
// }, []);
// <h2>Dream analyzer</h2>
// const contents = forecasts === undefined
//     ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https:aka.ms/jspsintegrationreact">https:aka.ms/jspsintegrationreact</a> for more details.</em></p>
//     : <table className="table table-striped" aria-labelledby="tableLabel">
//         <thead>
//             <tr>
//                 <th>Date</th>
//                 <th>Temp. (C)</th>
//                 <th>Temp. (F)</th>
//                 <th>Summary</th>
//             </tr>
//         </thead>
//         <tbody>
//             {forecasts.map(forecast =>
//                 <tr key={forecast.date}>
//                     <td>{forecast.date}</td>
//                     <td>{forecast.temperatureC}</td>
//                     <td>{forecast.temperatureF}</td>
//                     <td>{forecast.summary}</td>
//                 </tr>
//             )}
//         </tbody>
//     </table>;

// return (
//     <div>
//         <h1 id="tableLabel">Weather forecast</h1>
//         <p>This component demonstrates fetching data from the server.</p>
//         {contents}
//     </div>
// );

// async function populateWeatherData() {
//     const response = await fetch('/api/authentication/login');
//     if (response.ok) {
//         const data = await response.json();
//         setForecasts(data);
//     }
// }