'use client';
import styles from './page.module.css';
import { Segmented } from 'antd';
import { useState, useEffect, useContext, useMemo } from 'react';
import Link from 'next/link'
import DailyOrder from '../../components/DailyOrder/DailyOrder'
import BackButton from "../../components/BackButton/BackButton";
import { UserContext } from '../../store/userContext';
import { UserAPI, OrderAPI } from '../../global';

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
    try {
        curOrderData.forEach(item => {
            const orderDate = item.orderDate.split('T')[0];

            if (!groupedData[orderDate]) {
                groupedData[orderDate] = [];
            }

            groupedData[orderDate].push(item);
        });
    }
    catch (err) {
        console.log("GroupByDate error: ", err);
    }

    return groupedData;
}

// 從 API 取資料，並設定抓回來的資料
async function fetchOrderData(setOrderData, userID) {
    try {
        const res = await fetch(`${OrderAPI}/${userID}`);
        var data = await res.json();
        data = Object.values(data)[0];
        setOrderData(groupByDate(data));
        console.log('FetchOrderData successfully:', data);
    }
    catch (err) {
        console.log("FetchOrderData error: ", err);
    }
}

export default function History() {
    const OrderTypes = {
        "已訂購": "Init",
        "已取消": "Canceled",
        "已完成": "Preparing",
    };
    const [curOrderState, setOrderState] = useState("已訂購");
    const [curOrderData, setOrderData] = useState([]);
    const [curFilterOrderData, setFilterOrderData] = useState({});
    const [curDeleteOrder, setDeleteOrder] = useState(false);
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
    
    // 一進來頁面，先 fetch order 資料
    useEffect(() => {
        if (userID != "") {
            fetchOrderData(setOrderData, userID);
        }
        setDeleteOrder(false);
    }, [userID, curDeleteOrder]);

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
    dateKeys.sort((a, b) => new Date(b) - new Date(a));
    
    return (
        <div>

            <header className={styles.header}>
                <div className={styles.selectionContainer}>
                    <div className={styles.backButton}>
                        <Link href={"/routers/Home"}>
                            <BackButton />
                        </Link>
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
                                userType={user["userType"]}
                            />
                        ))
                    }
                </div>
            </main>
            
        </div>
    );
}
