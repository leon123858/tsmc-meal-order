'use client';
import { useRouter, useSearchParams } from 'next/navigation';
import React, { useState, useEffect } from 'react';
import styles from './page.module.css'; // Make sure to adjust the path

const Welcome = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const userType = searchParams.get('userType');

  const [countdown, setCountdown] = useState(5);

  useEffect(() => {
    const countdownInterval = setInterval(() => {
      setCountdown((prevCountdown) => prevCountdown - 1);
    }, 1000);

    return () => clearInterval(countdownInterval);
  }, []);

  useEffect(() => {
    if (countdown === 1) {
      if (userType === 'normal') {
        router.push('Home');
      } else if (userType === 'admin') {
        router.push('RestaurantHome');
      }
    }
  }, [countdown]);

  return (
	<div className={styles.body}>
		<div className={styles.container}>
		<h1 className={styles.h1}>
			Welcome to TSMC Meal Order!
		</h1>
		<p className={styles.p}>
			You are logging in for the first time.
		</p>
		<p className={styles.redirectInfo}>
			Redirecting to Home Page of {userType} user in{' '}
			<span className={styles.countdown}>{countdown}</span> seconds...
		</p>
		</div>
	</div>
  );
};

export default Welcome;
