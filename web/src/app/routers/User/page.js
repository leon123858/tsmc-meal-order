'use client';
import React, { useState, useContext } from 'react';
import BackButton from '../../components/BackButton/BackButton';
import Link from 'next/link';
import { UserOutlined } from '@ant-design/icons';
import { Select } from 'antd';
import { Radio } from 'antd';

import styles from './page.module.css';

const User = () => {
    const [user, setUser] = useState({
      name: 'daidai',
      place: 'hsinchu',
      uid: '9QiSGlDRvtU6S7CFzsk0W5UGkKTt',
    });
  
    const [userName, setUserName] = useState(user.name);
    const [selectedPlace, setSelectedPlace] = useState(user.place);
  
    const handleNameChange = (e) => {
      setUserName(e.target.value);
    };
  
    const handlePlaceChange = (value) => {
      setSelectedPlace(value);
    };
  
    const handleClick = async () => {
        const updatedUser = { ...user, name: userName, place: selectedPlace };
    
        try {
            // Make a POST request to the update API
            const response = await fetch('http://localhost:8080/api/user/update', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(updatedUser),
            });
    
            if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
            }
    
            const data = await response.json();
            console.log('User update successful:', data);
    
            // Update the state with the modified user data
            setUser(updatedUser);
        } catch (error) {
            console.error('User update failed:', error.message);
        }
    };
  
    return (
      <div>
        <header className={styles.header}>
                <Link href={"/routers/Home"}>
                   <BackButton />
                </Link>
        </header>

        <main className={styles.main}>
          <UserOutlined className={styles.largerIcon} />
          <div className={styles.settings}>
            <h3>名字</h3>
            <input
              type="text"
              value={userName}
              onChange={handleNameChange}
              placeholder="Enter your name"
            />
            <h3>廠區</h3>
            <div>
              <Select
                value={selectedPlace}
                style={{ width: 120, marginBottom: 10 }}
                onChange={handlePlaceChange}
                options={[
                  { value: 'hsinchu', label: 'Hsinchu' },
                  { value: 'tainan', label: 'Tainan' },
                  { value: 'taipei', label: 'Taipei' },
                  { value: 'taichung', label: 'Taichung' },
                ]}
              />
            </div>
  
            <Radio.Button
              onClick={handleClick}
              className={styles.blueButton}
              style={{ textAlign: 'center' }}
            >
              確認
            </Radio.Button>
          </div>
        </main>
      </div>
    );
  };
  
  export default User;