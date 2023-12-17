'use client';
import { auth } from '../../firebase';
import { signInWithPopup, GoogleAuthProvider, signInWithEmailAndPassword } from 'firebase/auth';

import { UserAPI } from '../../global'
import { useRouter } from 'next/navigation';
import Image from 'next/image';

import styles from './Login.module.css';
import React, { useState } from 'react';

const Login = () => {
    const router = useRouter();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isSignupSuccess, setIsSignupSuccess] = useState(false);
    const [adminData, setAdminData] = useState({
        name: '',
        email: '',
        password: '',
    });
    const [adminLoginData, setAdminLoginData] = useState({
        email: '',
        password: '',
    });

    const Modal = ({ isOpen, success, onClose }) => {
        const handleConfirm = () => {
            onClose();  // Close the modal
            clearForm(); // Clear the form data
        };
        const clearForm = () => {
            setAdminData({
                name: '',
                email: '',
                password: '',
            });
        };
    
        return (
            isOpen && (
                <div className={styles.modal}>
                    <p>{success ? 'Admin account created successfully!' : 'Admin creation failed.'}</p>
                    <button onClick={handleConfirm}>
                        Confirm
                    </button>
                </div>
            )
        );
    };

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
      
                const userLogin = {
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
                    body: JSON.stringify({ ...userLogin }),
                });
                response.json().then(r => {
                    console.log("Google Sign In Success: ", r);
                    if (r.data === "first login success") {
                        router.push('/routers/Welcome?userType=normal');
                    }
                    else {
                        router.push('routers/Home');
                    }
                });
            }
        } catch (error) {
            console.error('Google Sign In Error:', error.message);
        }
    };

    const handleAdminLogin = async (email, password) => {
        try {
            const userCredential = await signInWithEmailAndPassword(auth, email, password);
            
            const idToken = userCredential._tokenResponse.idToken;
            const uid = userCredential.user.uid;

            if (userCredential.user) {
                const userLogin = {
                    email: email,
                    uid: uid,
                };
                const response = await fetch(`${UserAPI}/login`, {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${idToken}`,
                    },
                    body: JSON.stringify({ ...userLogin }),
                });
                response.json().then(r => {
                    console.log("Admin Sign In Success: ", r);
                    if (r.data === "first login success") {
                        router.push('/routers/Welcome?userType=admin');
                    }
                    else {
                        router.push('routers/RestaurantHome');
                    }
                });
            }
        } catch (error) {
            console.error('Admin Sign In Error:', error.message);
        }
    };

    const handleAdminSignup = async (name, email, password) => {
        try {
            const adminCredentials = {
                email: email,
                name: name,
                password: password,
                type: "admin"
            };

            const response = await fetch(`${UserAPI}/create`, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ ...adminCredentials }),
            });

            response.json().then(r => {
                if (r.result) {
                    setIsSignupSuccess(true);
                    setIsModalOpen(true);
                    console.log('Admin Sign Up Success:', r);
                } else {
                    setIsSignupSuccess(false);
                    setIsModalOpen(true);
                    console.error('Admin creation failed:', r);
                }
            });

        } catch (error) {
            console.error('Admin Sign Up Error:', error.message);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setAdminData((prevData) => ({
            ...prevData,
            [name]: value,
        }));
    };

    const handleLoginInputChange = (e) => {
        const { name, value } = e.target;
        setAdminLoginData((prevData) => ({
            ...prevData,
            [name]: value,
        }));
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

                <div className={styles.loginSection}>
                    <h2>User Login</h2>
                    <button onClick={handleGoogleLogin}>User Sign in with Google</button>
                </div>

                <div className={styles.loginSection}>
                    <h2>Admin Login</h2>
                    <input
                        type="text"
                        placeholder="Admin Email"
                        name="email"
                        value={adminLoginData.email}
                        onChange={handleLoginInputChange}
                    />
                    <br />
                    <input
                        type="password"
                        placeholder="Admin Password"
                        name="password"
                        value={adminLoginData.password}
                        onChange={handleLoginInputChange}
                    />
                    <br />
                    <button onClick={() => handleAdminLogin(adminLoginData.email, adminLoginData.password)}>
                        Admin Login
                    </button>
                </div>

                <div className={styles.loginSection}>
                    <h2>Create Admin Account</h2>
                    <input
                        type="text"
                        placeholder="Admin Name"
                        name="name"
                        value={adminData.name}
                        onChange={handleInputChange}
                    />
                    <br />
                    <input
                        type="text"
                        placeholder="Admin Email"
                        name="email"
                        value={adminData.email}
                        onChange={handleInputChange}
                    />
                    <br />
                    <input
                        type="password"
                        placeholder="Admin Password"
                        name="password"
                        value={adminData.password}
                        onChange={handleInputChange}
                    />
                    <br />
                    <button onClick={() => handleAdminSignup(adminData.name, adminData.email, adminData.password)}>
                        Create Admin Account
                    </button>

                    <Modal isOpen={isModalOpen} success={isSignupSuccess} onClose={() => setIsModalOpen(false)} />
                </div>
            </main>
        </div>      
    )  
};
  
export default Login;
