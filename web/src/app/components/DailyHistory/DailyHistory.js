import styles from './DailyHistory.module.css';
import { Divider } from 'antd';
import { useState, useEffect } from 'react';
import Dish from '../Dish/Dish'

const DailyHistory = ({ curHistoryState, date }) => {
    // const HistoryTypes = [
    //     "已訂購",
    //     "已取消",
    //     "已完成"
    // ];
    // const [curHistoryState, setHistoryState] = useState("已訂購");
    
    // useEffect(() => {
    //     window.alert(curHistoryState)
    // }, [curHistoryState])

    return (
        <div className={styles.container}>
            <font 
                face="monospace" 
                size="4"
            >
                {date}
            </font>
            <hr className={styles.hr_date} />

            <Dish 
                price = { 100 } 
                number={ -1 } 
            />  
            <hr className={styles.hr_meal} />    

            <Dish 
                price = { 100 } 
                number={ -1 } 
            />  
            <hr className={styles.hr_date} />   
        </div>
    );
};

export default DailyHistory;