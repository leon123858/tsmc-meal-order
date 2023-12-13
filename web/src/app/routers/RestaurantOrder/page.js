'use client';
import styles from './page.module.css';
import { Segmented } from 'antd';
import { useState, useEffect, useContext, useMemo } from 'react';
import Link from 'next/link'
import DailyOrder from '../../components/DailyOrder/DailyOrder'
import BackButton from "../../components/BackButton/BackButton";
import { UserAPI, OrderAPI } from '../../global';
import { UserContext } from '../../store/userContext';

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

// 將資料依照日期分組
function groupByDate (curOrderData) {
    const groupedData = {};
    curOrderData.forEach(item => {
        const orderDate = item.orderDate.split('T')[0];

        if (!groupedData[orderDate]) {
            groupedData[orderDate] = [];
        }

        groupedData[orderDate].push(item);
    });

    return groupedData;
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
            setOrderData(groupByDate(response.data));

            console.log('Order fetched successfully:', response.data);
        }
        else {
            console.log('Fetching order failed:', response.message);
        }
	} catch (error) {
        console.error('Error fetching order:', error.message);
	}
}

export default function RestaurantOrder() {
    const OrderTypes = {
        "未完成": "Init",
        "已取消": "Canceled",
        "已完成": "Preparing",
        // "準備中": "Preparing",
        // "已完成": "Finished"
    };
    const [curOrderState, setOrderState] = useState("未完成");
    const [curOrderData, setOrderData] = useState([]);
    const [curFilterOrderData, setFilterOrderData] = useState({});
    const [curDeleteOrder, setDeleteOrder] = useState(false);
    const [curConfirmOrder, setConfirmOrder] = useState(false);

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
            setDeleteOrder(false);
            setConfirmOrder(false);
        }
    }, [userID, curDeleteOrder, curConfirmOrder]);

    // 當狀態改變時，取新的歷史 order
    useEffect(() => {
        const filterData = {};
        const status = OrderTypes[curOrderState];
        Object.entries(curOrderData).forEach(([key, value]) => {
            const filteritem = value.filter(item => item.status == status);
            if (filteritem.length !== 0) {
                filterData[key] = filteritem;
            }
        });
        setFilterOrderData(filterData);
    }, [curOrderState, curOrderData]);

    // 取出所有 date，並按照降序排列
    const dateKeys = Object.keys(curFilterOrderData);
    dateKeys.sort((a, b) => b - a);
    
    return (
        <div>

            <header className={styles.header}>
                <div className={styles.selectionContainer}>
                    <div className={styles.backButton}>
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
                    </div>
                    
                    <Segmented
                        options={Object.keys(OrderTypes)}
                        value={curOrderState}
                        onChange={setOrderState}
                        className={styles.segment}
                    ></Segmented>
                </div>
            </header>

            <main className={styles.main}>
                <div className={styles.dishContainer}>
                    {
                        dateKeys.map((datekey, index) => (
                            <DailyOrder
                                key={index}
                                data={curFilterOrderData[datekey]}
                                date={datekey}
                                orderType={curOrderState}
                                setDeleteOrder={setDeleteOrder}
                                setConfirmOrder={setConfirmOrder}
                                userType={user["userType"]}
                            />
                        ))
                    }
                </div>
            </main>
            
        </div>
    );
}
