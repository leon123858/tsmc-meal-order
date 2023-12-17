'use client'
import React from 'react';
import { useState, useEffect, useContext } from 'react';
import { DatePicker } from 'antd';
import Link from 'next/link';
import BackButton from '../../components/BackButton/BackButton';
import { UserAPI, OrderAPI } from '../../global';
import { UserContext } from '../../store/userContext';

import styles from "./page.module.css";

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
      "uid": userID,
      "userType": data["userType"],
    }))
  }

async function fetchOrderData(setOrderData, userID) {
	try {
        const res = await fetch(`${OrderAPI}/${userID}`, {
            method: 'GET',
            headers: {
                'Accept': 'text/plain'
            }
        });

        const response = await res.json();

        if (response.result) {
            setOrderData(response.data);

            console.log('Order fetched successfully:', response.data);
        }
        else {
            console.log('Fetching order failed:', response.message);
        }
	} catch (error) {
        console.error('Error fetching order:', error.message);
	}
}

function filterOrders(orders, selectedMonth, setMonthOrderData) {
	const filteredOrders = orders.filter(order => {
		const orderDate = order.orderDate;
		const orderStatus = order.status;

		// Check if orderDate is the selectedMonth and order status is "Preparing"
		if (!orderDate || !orderDate.includes('T')) {
			return false;
		}

		const orderMonth = orderDate.split('T')[0].slice(0, 7);
		return orderMonth === selectedMonth && orderStatus === "Preparing";
	});
	setMonthOrderData(filteredOrders);
}

function groupOrders(orders, setGroupedOrders, setTotalPrice) {
	// console.log(orders);
    let curTotalPrice = 0;

    const groupedOrders = orders.reduce((result, order) => {
        order.foodItems.forEach(item => {
            const foodKey = `${item.snapshot.name}`;
            
            if (!result[foodKey]) {
                result[foodKey] = {
                    name: item.snapshot.name,
                    totalPrice: item.snapshot.price * item.count,
                    totalCount: item.count,
                };
            } else {
                result[foodKey].totalCount += item.count;
                result[foodKey].totalPrice += item.snapshot.price * item.count;
            }
			curTotalPrice += item.snapshot.price * item.count;
        });

        return result;
    }, {});

    const groupedOrdersArray = Object.values(groupedOrders);
    setGroupedOrders(groupedOrdersArray);
    setTotalPrice(curTotalPrice);
}

export default function Report() {
	const [selectedMonth, setSelectedMonth] = useState("");
    const [orderData, setOrderData] = useState([]);
    const [monthOrderData, setMonthOrderData] = useState([]);
	const [groupedOrders, setGroupedOrders] = useState([]);
	const [totalPrice, setTotalPrice] = useState(0);

	const { userID } = useContext(UserContext); 
    const [user, setUser] = useState({
        uid: "",
        userType: "",
      });

    useEffect(() => {
        if (userID != "") {
          fetchUser(userID, setUser);
        }
    }, [userID]);

	useEffect(() => {
        if (userID != "") {
            fetchOrderData(setOrderData, userID);
        }
    }, [userID]);

	useEffect(() => {
		filterOrders(orderData, selectedMonth, setMonthOrderData);
	}, [selectedMonth, orderData]);

	useEffect(() => {
		groupOrders(monthOrderData, setGroupedOrders, setTotalPrice);
	}, [monthOrderData]);

	const onChange = (date, dateString) => {
		setSelectedMonth(dateString);
	};

  	return (
		<div className={styles.container}>

		<header className={styles.header}>
			{user.userType === 'normal' ? (
				<Link href="/routers/Home">
					<BackButton />
				</Link>
			) : (
				<Link href="/routers/RestaurantHome">
					<BackButton />
				</Link>
			)}

			<DatePicker 
				onChange={onChange} 
				picker="month" 
				style={{ width: '50%', marginLeft: '20px' }}
			/>
		</header>

		<main className={styles.main}>
			<div className={styles.border}>
				<div className={styles.headerMain}>
					{user.userType === 'normal' ? (
						<>
							<div className={styles.column}>品項</div>
							<div className={styles.column}>購買數量</div>
							<div className={styles.column}>總消費金額</div>
						</>
					) : (
						<>
							<div className={styles.column}>品項</div>
							<div className={styles.column}>銷售數量</div>
							<div className={styles.column}>銷售收入</div>
						</>
					)}
				</div>

				<div className={styles.content}>
					{groupedOrders.map(item => (
						<div key={item.name} className={styles.row}>
							<div className={styles.column}>{item.name}</div>
							<div className={styles.column}>{item.totalCount}</div>
							<div className={styles.column}>{item.totalPrice}</div>
						</div>
					))}
				</div>

				<div className={styles.totalPrice}>
					{user.userType === 'normal' ? (
						`總消費金額: $ ${totalPrice}`
					) : (
						`總銷售收入: $ ${totalPrice}`
					)}
				</div>
			</div>
		</main>

		</div>
	);
}
