'use client';
import styles from './page.module.css';
import { UserContext, UserProvider } from './store/userContext'
import React, { useContext } from 'react';
import Login from './components/Login/Login'

const App = () => {
    return (
        <React.StrictMode>
            <UserProvider>
                <div>
                    <Login/>
                </div>            
            </UserProvider>
        </React.StrictMode>
    );
  };
  
  export default App;