'use client';
import { auth } from '../../firebase';
import { signInWithPopup, GoogleAuthProvider } from 'firebase/auth';
import { UserAPI } from '../../global'
import { useRouter } from 'next/navigation';
import Image from 'next/image';

import styles from './Login.module.css';
import React from 'react';

const Login = () => {
    const router = useRouter();
    const handleGoogleLogin = async () => {
        const provider = new GoogleAuthProvider();

        try {
            const result = await signInWithPopup(auth, provider);
            const user = result.user;
            console.info('Google Sign In Success:', user);
            if (user) {                
                const idToken = await user.getIdToken();
                // console.log('ID Token:', idToken);
                // console.log('User email:', user.email);
                // console.log('User UID:', user.uid);
      
                const userCredentials = {
                  email: user.email,
                  uid: user.uid,
                };
      
                // Make a POST request to the login APIs
                const response = await fetch(`${UserAPI}/login`, {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${idToken}`,
                      },
                    body: JSON.stringify({ ...userCredentials }),
                });
                response.json().then(r => console.log(r));

                router.push('routers/Home');
            }
        } catch (error) {
            console.error('Google Sign In Error:', error.message);
        }
    };
  
    return (
        <div>
            <main className={styles.main}>
            <Image
                src="/images/tsmc.png"
                alt="TSMC Logo"
                width={100}
                height={100}
                priority
            />
            <h1>Meal Order</h1>
            <button onClick={handleGoogleLogin}>Sign in with Google</button>
            </main>
        </div>      
    )  
  };
  
  export default Login;