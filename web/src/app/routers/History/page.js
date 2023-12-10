'use client';
import styles from './page.module.css';
import { Segmented } from 'antd';
import { useState, useEffect, useContext, useMemo } from 'react';
import Link from 'next/link'
import DailyHistory from '../../components/DailyHistory/DailyHistory'
import BackButton from "../../components/BackButton/BackButton";
import { UserContext } from '../../store/userContext';
import { OrderAPI } from '../../global';

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

// 從 API 取資料，並設定抓回來的資料
async function fetchOrderData(setOrderData, userID) {
    const res = await fetch(`${OrderAPI}/${userID}`);
    var data = await res.json();
    data = Object.values(data)[0];
    setOrderData(groupByDate(data));
}

export default function History() {
    const HistoryTypes = {
        "已訂購": "Init",
        "已取消": "Canceled",
        "準備中": "Preparing",
        "已完成": "Finished"
    };
    const [curHistoryState, setHistoryState] = useState("已訂購");
    const [curOrderData, setOrderData] = useState([]);
    const [curFilterOrderData, setFilterOrderData] = useState({});
    const [curDeleteHistory, setDeleteHistory] = useState(false);
    const { userID } = useContext(UserContext);  
    
    // 一進來頁面，先 fetch 資料
    useEffect(() => {
        if (userID != "") {
            fetchOrderData(setOrderData, userID);
        }
        setDeleteHistory(false);
    }, [userID, curDeleteHistory]);

    // 當狀態改變時，取新的歷史 order
    useEffect(() => {
        const filterData = {};
        const status = HistoryTypes[curHistoryState];
        Object.entries(curOrderData).forEach(([key, value]) => {
            const filteritem = value.filter(item => item.status == status);
            if (filteritem.length !== 0) {
                filterData[key] = filteritem;
            }
        });
        setFilterOrderData(filterData);
    }, [curHistoryState, curOrderData]);

    // 取出所有 date，並按照降序排列
    const dateKeys = Object.keys(curFilterOrderData);
    dateKeys.sort((a, b) => b - a);
    
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
                        options={Object.keys(HistoryTypes)}
                        value={curHistoryState}
                        onChange={setHistoryState}
                        className={styles.segment}
                    ></Segmented>
                </div>
            </header>

            <main className={styles.main}>
                <div className={styles.dishContainer}>
                    {
                        dateKeys.map((datekey, index) => (
                            <DailyHistory
                                key={index}
                                data={curFilterOrderData[datekey]}
                                date={datekey}
                                historyType={curHistoryState}
                                setDeleteHistory={setDeleteHistory}
                            />
                        ))
                    }
                </div>
            </main>
            
        </div>
    );
}
