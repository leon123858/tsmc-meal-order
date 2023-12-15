'use client';
import React, { useState, useEffect, useContext } from 'react';
import { Layout, Menu, theme, Radio, Button, Spin } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { MenuAPI } from '../../global';
import { UserContext } from '../../store/userContext'
import UploadDish from '@/app/components/UploadDish/UploadDish';
import Link from 'next/link';

import styles from "./page.module.css";

const { Header, Content, Sider } = Layout;

async function fetchMenu(userID, setMenuFood) {
	try {
	  const res = await fetch(`${MenuAPI}/${userID}`, {
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
  
	useEffect(() => {
		if (userID !== "") {
			console.log('Fetching menu...');
			fetchMenu(userID, setMenuFood);
		}
	}, [userID]);

	useEffect(() => {
		const newItems = menuFood.map((dish, index) => ({
			key: String(index),
			label: dish["name"],
		}));
		setItems(newItems);
	}, [menuFood]);

    const fetchMenuAndUpdate = async () => {
        if (userID !== "") {
            console.log('Fetching menu...');
            await fetchMenu(userID, setMenuFood);
        }
    };

	const handleMenuItemClick = ({ key }) => {
		const index = parseInt(key, 10); // Convert key to index
		setSelectedIndex(index);
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
				{/* <div className={styles.buttonGap}></div>
				<Radio.Button value="default" className={styles.blueButton}>
					銷量報表
				</Radio.Button> */}
				<div className={styles.buttonGap}></div>
				<Link href="/routers/RestaurantReport">
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
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default RestaurantHome;