'use client';
import styles from './page.module.css';
import { Segmented } from 'antd';
import { useState, useEffect } from 'react';
import Link from 'next/link'
import DailyHistory from '../../components/DailyHistory/DailyHistory'
import BackButton from "../../components/BackButton/BackButton";


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
async function fetchOrderData(setOrderData, status) {
    const res = await fetch("http://localhost:4000/datas");
    var data = await res.json();
    data = data.filter(item => item.status == status);
    setOrderData(groupByDate(data));
}


export default function History() {
    const HistoryTypes = {
        "已訂購": 0,
        "已取消": 1,
        "已完成": 2
    };
    const [curHistoryState, setHistoryState] = useState("已訂購");
    const [curOrderData, setOrderData] = useState([]);
    
    // 當狀態改變時，取新的歷史 order
    useEffect(() => {
        const status = HistoryTypes[curHistoryState];
        fetchOrderData(setOrderData, status);
    }, [curHistoryState, HistoryTypes])


    // 取出所有 date，並按照降序排列
    const dateKeys = Object.keys(curOrderData);
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
                        dateKeys.map((datakey, index) => (
                            <DailyHistory
                                key={index}
                                data={curOrderData[datakey]}
                                date={datakey}
                            />
                        ))
                    }
                </div>
            </main>
            
        </div>
    );
}
