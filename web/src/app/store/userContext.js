'use client';
import { React, createContext, useEffect, useState } from 'react'
import { auth } from '../firebase'
import { onAuthStateChanged } from 'firebase/auth'

const UserContext = createContext({
    userID: "",
    isLogin: false,
});

function UserProvider({ children }) {
    const [userID, setUserID] = useState("");
    const [isLogin, setLogin] = useState(false);

    useEffect(() => {
        onAuthStateChanged(auth, (user) => {
            if (user) {
                setLogin(true);
                setUserID(user.uid);
            } else {
                setLogin(false);
            }
        })
    }, []);


    const defaultValue = {
        userID: userID,
        isLogin: isLogin,
    };

    return (
        <UserContext.Provider value={defaultValue}>
            {children}
        </UserContext.Provider>
    );
} 

export { UserContext, UserProvider };