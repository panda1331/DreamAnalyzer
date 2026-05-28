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
import Profile from './components/Profile';
import AddDream from './components/AddDream';
import DreamDetails from './components/DreamDetails';
import EditDream from './components/EditDream';
import Sandman from './components/Sandman';
import AdminLayout from './components/Admin/AdminLayout';

function App() {
    return (
        <div className="mainDiv">
            <Header />
            <main>
                <Routes>
                    <Route path="/" element={<Info />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/profile" element={
                        <PrivateRoute>
                            <Profile />
                        </PrivateRoute>
                    } />
                    <Route path="/dreams" element={
                        <PrivateRoute>
                            <DreamsList />
                        </PrivateRoute>
                    } />
                    <Route path="/dreams/create" element={
                        <PrivateRoute>
                            <AddDream />
                        </PrivateRoute>
                    } />
                    <Route path="/dreams/:id" element={
                        <PrivateRoute>
                            <DreamDetails/>
                        </PrivateRoute>
                    } />
                    <Route path="/dreams/:id/edit" element={
                        <PrivateRoute>
                            <EditDream />
                        </PrivateRoute>
                    } />
                    <Route path="/admin" element={
                        <PrivateRoute>
                            <AdminLayout />
                        </PrivateRoute>
                    } />
                </Routes>
                <Sandman />
            </main>
            <Footer />
        </div>
    )
}

export default App;

