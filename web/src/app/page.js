'use client';
import { auth } from './firebase';
import { signInWithPopup, GoogleAuthProvider } from 'firebase/auth';
import { useRouter } from 'next/navigation';

import styles from './page.module.css';

const Login = () => {
    const router = useRouter();
    const handleGoogleLogin = async () => {
        const provider = new GoogleAuthProvider();
    
        try {
            const result = await signInWithPopup(auth, provider);
            const user = result.user;
            // Redirect or perform actions after successful login
            router.push('routers/Home');
        } catch (error) {
            console.error('Google Sign In Error:', error.message);
        }
    };
  
    return (
      <div>
        <main className={styles.main}>
          <img src="/images/tsmc.png" alt="TSMC Logo" style={{ width: '100px', height: '100px' }} />
          <h1>Meal Order</h1>
          <button onClick={handleGoogleLogin}>Sign in with Google</button>
        </main>
      </div>
    );
  };
  
  export default Login;