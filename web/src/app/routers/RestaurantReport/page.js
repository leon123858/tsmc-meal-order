'use client'
import React from 'react';
import { useState, useEffect, useContext } from 'react';
import { DatePicker } from 'antd';
import Link from 'next/link';
import BackButton from '../../components/BackButton/BackButton';
import { OrderAPI } from '../../global';
import { UserContext } from '../../store/userContext';

import styles from "./page.module.css";

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

function groupOrders(orders, setGroupedOrders, setTotalRevenue) {
	console.log(orders);
    let totalRevenue = 0;

    const groupedOrders = orders.reduce((result, order) => {
        order.foodItems.forEach(item => {
            const foodKey = `${item.snapshot.name}`;
            
            if (!result[foodKey]) {
                result[foodKey] = {
                    name: item.snapshot.name,
                    totalRevenue: item.snapshot.price * item.count,
                    totalCount: item.count,
                };
            } else {
                result[foodKey].totalCount += item.count;
                result[foodKey].totalRevenue += item.snapshot.price * item.count;
            }
			totalRevenue += item.snapshot.price * item.count;
        });

        return result;
    }, {});

    const groupedOrdersArray = Object.values(groupedOrders);

    setGroupedOrders(groupedOrdersArray);
    setTotalRevenue(totalRevenue);
}

export default function RestaurantReport() {
	const [selectedMonth, setSelectedMonth] = useState("");
    const [orderData, setOrderData] = useState([]);
    const [monthOrderData, setMonthOrderData] = useState([]);
	const [groupedOrders, setGroupedOrders] = useState([]);
	const [totalRevenue, setTotalRevenue] = useState(0);

	const { userID } = useContext(UserContext); 

	useEffect(() => {
        if (userID != "") {
            fetchOrderData(setOrderData, userID);
        }
    }, [userID]);

	useEffect(() => {
		filterOrders(orderData, selectedMonth, setMonthOrderData);
	}, [selectedMonth, orderData]);

	useEffect(() => {
		groupOrders(monthOrderData, setGroupedOrders, setTotalRevenue);
	}, [monthOrderData]);

	const onChange = (date, dateString) => {
		setSelectedMonth(dateString);
	};

  	return (
		<div className={styles.container}>

		<header className={styles.header}>
			<Link href={"/routers/RestaurantHome"}>
				<BackButton />
			</Link>
			<DatePicker 
				onChange={onChange} 
				picker="month" 
				style={{ width: '50%', marginLeft: '20px' }}
			/>
		</header>

		<main className={styles.main}>
			<div className={styles.border}>
				<div className={styles.headerMain}>
					<div className={styles.column}>品項</div>
					<div className={styles.column}>銷售數量</div>
					<div className={styles.column}>銷售收入</div>
				</div>
				<div className={styles.content}>
					{groupedOrders.map(item => (
						<div key={item.name} className={styles.row}>
							<div className={styles.column}>{item.name}</div>
							<div className={styles.column}>{item.totalCount}</div>
							<div className={styles.column}>{item.totalRevenue}</div>
						</div>
					))}
				</div>
				<div className={styles.totalRevenue}>
					總銷售收入:  $ {totalRevenue}
				</div>
			</div>
		</main>

		</div>
	);
}
