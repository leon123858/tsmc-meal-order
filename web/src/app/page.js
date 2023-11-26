'use client';
import { auth } from './firebase';
import { signInWithPopup, GoogleAuthProvider } from 'firebase/auth';
import { useRouter } from 'next/navigation';
import Image from 'next/image';

import styles from './page.module.css';

const Login = () => {
    const router = useRouter();
    const handleGoogleLogin = async () => {
        const provider = new GoogleAuthProvider();
    
        try {
            const result = await signInWithPopup(auth, provider);
            const user = result.user;
            console.error('Google Sign In Success:', user);
            router.push('routers/Home');
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
    );
  };
  
  export default Login;