// import { useEffect, useState } from 'react';
import './App.css';
import Header from './components/Header';
import Login from './components/Login';
import Info from './components/Info';
import { Routes, Route } from 'react-router-dom';

function App() {
    return (
        <div>
            <Header />

            <Routes>
                <Route path="/" element={ <Info /> } />
                <Route path="/login" element={<Login /> } />
            </Routes>
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