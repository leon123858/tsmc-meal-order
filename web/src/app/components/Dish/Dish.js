import React, { useContext } from 'react';
import Image from 'next/image';
import styles from './Dish.module.css';
import { UserContext } from '../../store/userContext';
import { OrderAPI } from '../../global';
import { Button } from 'antd';

async function deleteOrder (userID, orderID, setDeleteHistory) {
    const response = await fetch(`${OrderAPI}/delete/${userID}/${orderID}`, {
        method: 'POST',
        headers: {
            'Accept': '*/*',
        },
    });
    console.log(response);
    if (response.ok) {
        setDeleteHistory(true);
    }
}

const Dish = ({ dish, isHistory, historyType, setDeleteHistory}) => {
    const { userID } = useContext(UserContext);
    var orderCount = 0
    var orderID = "";
    if (isHistory) {
        orderCount = dish["count"];
        orderID = dish["orderID"];
        dish = dish["snapshot"];
    }
    return (
      <main>
          <div className={styles.container}>
                <div className={styles.imageContainer}>
                    <Image
                        src="/images/tsmc.png"
                        alt="Avatar"
                        width={50}
                        height={50}
                        priority
                    />
                </div>
                <div className={styles.textContainer}>
                    <h3>{dish["name"]}</h3>
                    <div className={styles.switchContainer}>
                        {
                            dish["tags"].map((tag, index) => (
                                tag != "Dinner" && tag != "Lunch" && tag != "Breakfast" &&
                                <div className={styles.circle} key={index}>{tag}</div>
                            ))
                        }
                        <br />
                    </div>
                </div>
                <div className={styles.priceContainer}>
                    <div className={styles.priceContent}>
                        NT$ {isHistory !== true && dish["price"]}
                        {isHistory === true && orderCount * dish["price"]}  
                        <br />
                        {isHistory !== true && `剩下 ${dish["count"]} 份`}
                        {isHistory === true && `共 ${orderCount} 份`}
                        <br />
                        {isHistory === true && historyType === "已訂購" && 
                            <Button value="default" className={styles.blueButton} onClick={() => deleteOrder(userID, orderID, setDeleteHistory)}>
                                刪除訂單
                            </Button>
                        }
                    </div>
                </div>
          </div>
      </main>
    );
};
export default Dish;
