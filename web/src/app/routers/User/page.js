'use client';
import React, { useState, useContext, useEffect } from 'react';
import BackButton from '../../components/BackButton/BackButton';
import Link from 'next/link';
import { UserOutlined } from '@ant-design/icons';
import { Select, Radio } from 'antd';
import { UserAPI } from '../../global';
import { UserContext } from '../../store/userContext';

import styles from './page.module.css';

async function fetchUser(userID, setUser) {
	const res = await fetch(`${UserAPI}/get?uid=${userID}`, {
		method: 'GET',
		headers: {
			'Accept': 'application/json'
		}
	});
	var data = await res.json();
	data = Object.values(data)[2];
	setUser((prevState) => ({
		...prevState,
		"name": data["name"],
		"place": data["place"],
		"uid": userID,
		"userType": data["userType"]
	}))
}

const User = () => {
    const { userID } = useContext(UserContext);
    const [user, setUser] = useState({
      name: "",
      place: "",
      uid: "",
      userType: "",
    });
    const [isUpdateSuccess, setIsUpdateSuccess] = useState(false);
	const [isModalOpen, setIsModalOpen] = useState(false);
	
    const Modal = ({ isOpen, success, onClose }) => {
        const handleConfirm = () => {
            onClose();  // Close the modal
        };
    
        return (
            isOpen && (
                <div className={styles.modal}>
                    <p>{success ? 'Update user successfully!' : 'Please enter both name and place.'}</p>
                    <button onClick={handleConfirm}>
                        Confirm
                    </button>
                </div>
            )
        );
    };

    const handleNameChange = (e) => {
      setUser((prevState) => ({
        ...prevState,
        "name": e.target.value
      }))
    };
  
    const handlePlaceChange = (value) => {
      setUser((prevState) => ({
        ...prevState,
        "place": value
      }))
    };
  
    const handleClick = async () => {
      if (!user.name || !user.place) {
			setIsUpdateSuccess(false);
			setIsModalOpen(true);
			return;
    	}

        const updatedUser = { ...user, name: user["name"], place: user["place"] };
    
        try {
            // Make a POST request to the update API
            const response = await fetch(`${UserAPI}/update`, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(updatedUser),
            });
    
            if (!response.ok) {
              throw new Error(`HTTP error! Status: ${response.status}`);
            }
    
            const data = await response.json();
            console.log('User update successful:', data);
			setIsUpdateSuccess(true);
			setIsModalOpen(true);
        } catch (error) {
            console.error('User update failed:', error.message);
        }
    };

    useEffect(() => {
      if (userID != "") {
        fetchUser(userID, setUser);
      }
    }, [userID]);
  
    return (
      <div>
        <header className={styles.header}>
          {
            user["userType"] === "normal" ? (
              <Link href={"/routers/Home"}>
                <BackButton />
              </Link>
            ) : user["userType"] === "admin" ? (
              <Link href={"/routers/RestaurantHome"}>
                <BackButton />
              </Link>
            ) : null
          }
        </header>

        <main className={styles.main}>
          <UserOutlined className={styles.largerIcon} />
          <div className={styles.settings}>
            <h3>名字</h3>
            <input
              type="text"
              value={user["name"]}
              onChange={handleNameChange}
              placeholder="Enter your name"
              defaultValue={user["name"]}
            />
            <h3>廠區</h3>
            <div>
              <Select
                value={user["place"]}
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

			<Modal isOpen={isModalOpen} success={isUpdateSuccess} onClose={() => setIsModalOpen(false)} />
          </div>
        </main>
      </div>
    );
  };
  
  export default User;