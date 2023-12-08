'use client';
import { React, createContext, useEffect, useState } from 'react'
import { auth } from '../firebase'
import { onAuthStateChanged } from 'firebase/auth'

const UserContext = createContext({
    userID: "",
    isLogin: false,
    place: "Hsinchu",
    username: "user",
    setPlace: () => {},
    setUsername: () => {}
});

function UserProvider({ children }) {
    const [userID, setUserID] = useState("");
    const [isLogin, setLogin] = useState(false);
    const [place, setPlace] = useState("Hsinchu");
    const [username, setUsername] = useState("user");

    useEffect(() => {
        onAuthStateChanged(auth, (user) => {
            if (user) {
                setLogin(true);
                setUserID(user.uid);
                setUsername(user.displayName)
            } else {
                setLogin(false);
            }
        })
    }, []);


    const defaultValue = {
        userID: userID,
        isLogin: isLogin,
        place: place,
        username: username,
        setPlace: setPlace,
        setUsername: setUsername
    };

    return (
        <UserContext.Provider value={defaultValue}>
            {children}
        </UserContext.Provider>
    );
} 

export { UserContext, UserProvider };