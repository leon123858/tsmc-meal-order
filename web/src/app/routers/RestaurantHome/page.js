'use client';
import React, { useState, useEffect, useContext } from 'react';
import { Layout, Menu, theme, Radio, Button, Spin } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { UserAPI, MenuAPI } from '../../global';
import { UserContext } from '../../store/userContext'
import UploadDish from '@/app/components/UploadDish/UploadDish';
import Link from 'next/link';

import styles from "./page.module.css";

const { Header, Content, Sider } = Layout;

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
    }));
}

async function fetchCreateEmptyMenu(user) {
    try {
        const menuData = {
            id: user["uid"], 
            name: user["name"],
            foodItems: [
                {
                    "description": "",
                    "name": "新增餐點",
                    "price": 1,
                    "countLimit": 1,
                    "imageUrl": "",
                    "tags": []
                }
            ]
        };

        const res = await fetch(`${MenuAPI}`, {
            method: 'POST',
            headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            },
            body: JSON.stringify(menuData),
        });
  
        if (!res.ok) {
            throw new Error(`HTTP error! Status: ${res.status}`);
        }
  
        const response = await res.json();
        console.log('Menu created successfully:', response);
        return response;
    } catch (error) {
        console.error('Error creating menu:', error.message);
        throw error;
    }
}

async function fetchMenu(user, setMenuFood, setFirstCreate) {
	try {
	  const res = await fetch(`${MenuAPI}/${user["uid"]}`, {
		method: 'GET',
		headers: {
		  'Accept': 'application/json'
		}
	  });
	  
	  const response = await res.json();
	  if (response.result) {
		const curMenuFood = response.data['foodItems'];
		const isNewDishExists = curMenuFood.some(item => item.name === '新增餐點');

		if (!isNewDishExists) {
			// "新增餐點" doesn't exist, so add it
			curMenuFood.push({
				description: '',
				name: '新增餐點',
				price: 1,
				countLimit: 1,
				imageUrl: '',
				tags: [],
			});
		}
		setMenuFood(curMenuFood);
		console.log('Menu fetched successfully:', curMenuFood);
	  }
	  else if (response.message === 'Data Not Exist.') {
		console.log('Menu not exist, creating...');
		await fetchCreateEmptyMenu(user);
		console.log('Menu created successfully.');
		setFirstCreate(true);
	  }
	  else {
		console.log('Fetching menu failed:', response.message);
	  }
	} catch (error) {
	  console.error('Error fetching menu:', error.message);
	}
  }
  
const RestaurantHome = () => {
	const { userID } = useContext(UserContext);
	const {
	  token: { colorBgContainer },
	} = theme.useToken();
  
	const [menuFood, setMenuFood] = useState([]);
	const [items, setItems] = useState([]);
	const [selectedIndex, setSelectedIndex] = useState(0);
    const [user, setUser] = useState({
		name: "",
		place: "None",
		uid: "",
	  });
    const [isModalOpen, setIsModalOpen] = useState(false);
	const [firstCreate, setFirstCreate] = useState(false);
  
    useEffect(() => {
        if (userID != "") {
          fetchUser(userID, setUser);
        }
      }, [userID]);

    // 檢查是否為第一次登入的用戶，需先去設定名字及地點
    useEffect(() => {
        if (userID !== '' && user["place"] === '') {
            setIsModalOpen(true);
        }
    }, [userID, user["place"]]);

	useEffect(() => {
		if (userID !== '' && user["place"] !== '' && user["name"] !== '') {
			fetchMenuAndUpdate(user, setFirstCreate);
		}
	}, [userID, user["place"], firstCreate]);

	useEffect(() => {
		const newItems = menuFood.map((dish, index) => ({
			key: String(index),
			label: dish["name"],
		}));
		setItems(newItems);
	}, [menuFood]);

    const fetchMenuAndUpdate = async (user, setFirstCreate) => {
		if (userID !== '' || user["place"] !== '') {
			console.log('Fetching menu...');
			await fetchMenu(user, setMenuFood, setFirstCreate);
		}
    };

	const handleMenuItemClick = ({ key }) => {
		const index = parseInt(key, 10);
		setSelectedIndex(index);
	};

	const Modal = ({ isOpen, onClose }) => {
		const handleConfirm = () => {
			onClose();  // Close the modal
		};
	
		return (
			isOpen && user.place === '' && (
				<div className={styles.modal}>
					<p>第一次登入請設定使用者名稱及地點</p>
					<Link href="/routers/User">
						<button onClick={handleConfirm}>
							跳轉至設定頁面
						</button>
					</Link>
				</div>
			)
		);
	};	

  return (
    <Layout hasSider>
      <Sider
        style={{
			overflow: 'auto',
			height: '100vh',
			position: 'fixed',
			left: 0,
			top: 0,
			bottom: 0,
        }}
      >
        <div className="demo-logo-vertical" />
        <Menu theme="dark" mode="inline" defaultSelectedKeys={['0']} items={items} onClick={handleMenuItemClick} />
      </Sider>
      <Layout
        className="site-layout"
        style={{
        	marginLeft: 200,
        }}
      >
        <header className={styles.header}>
			<div className={styles.buttonContainer}>
				<Link href="/routers/RestaurantOrder">
					<Radio.Button value="default" className={styles.blueButton}>
						檢視訂單
					</Radio.Button>
				</Link>
				<div className={styles.buttonGap}></div>
				<Link href="/routers/Report">
					<Radio.Button value="default" className={styles.blueButton}>
						月結報表
					</Radio.Button>
				</Link>

				<div className={styles.icon}>
					<Link href="/routers/User">
					<Button 
						type="primary" 
						shape="circle" 
						icon={<UserOutlined />} 
						className={styles.userButton}
					/>
					</Link>
				</div>
			</div>
        </header>

        <Content
			style={{
				margin: '24px 16px 0',
				overflow: 'initial',
			}}
        >
			<div
				style={{
					padding: 24,
					textAlign: 'center',
					background: colorBgContainer,
				}}
			>
				{selectedIndex !== null && menuFood && menuFood.length > selectedIndex ? (
					<UploadDish 
						index={selectedIndex} 
						menuFood={menuFood} 
						setMenuFood={setMenuFood} 
						fetchMenuAndUpdate={fetchMenuAndUpdate}
					/>
				) : (
					<Spin size="large" />
				)}

			<Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />

          </div>
        </Content>
      </Layout>
    </Layout>
	
  );
};

export default RestaurantHome;