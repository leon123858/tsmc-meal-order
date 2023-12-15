import React, { useContext } from 'react';
import Image from 'next/image';
import styles from './Dish.module.css';
import { UserContext } from '../../store/userContext';
import { OrderAPI } from '../../global';
import { Button } from 'antd';

async function deleteOrder (userID, orderID, setDeleteOrder) {
    const response = await fetch(`${OrderAPI}/delete/${userID}/${orderID}`, {
        method: 'POST',
        headers: {
            'Accept': '*/*',
        },
    });
    console.log(response);
    if (response.ok) {
        setDeleteOrder(true);
    }
}

async function confirmOrder (userID, orderID, setConfirmOrder) {
    const response = await fetch(`${OrderAPI}/confirm/${userID}/${orderID}`, {
        method: 'POST',
        headers: {
            'Accept': '*/*',
        },
    });
    console.log(response);
    if (response.ok) {
        setConfirmOrder(true);
    }
}

const Dish = ({ dish, isOrder, orderType, setDeleteOrder, setConfirmOrder, userType }) => {
    const { userID } = useContext(UserContext);
    var orderCount = 0
    var orderID = "";
    if (isOrder) {
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
                        NT$ {isOrder !== true && dish["price"]}
                        {isOrder === true && orderCount * dish["price"]}  
                        <br />
                        {isOrder !== true && `剩下 ${dish["count"]} 份`}
                        {isOrder === true && `共 ${orderCount} 份`}
                        <br />
                        {isOrder === true && (orderType === "已訂購" || orderType === "未完成") && 
                            <div>
                                <Button value="default" className={styles.redButton} onClick={() => deleteOrder(userID, orderID, setDeleteOrder)}>
                                    刪除訂單
                                </Button>
                                
                                {userType === "admin" && orderType === "未完成" &&
                                    <Button value="default" className={styles.deepBlueButton} onClick={() => confirmOrder(userID, orderID, setConfirmOrder)}>
                                        完成訂單
                                    </Button>
                                }
                            </div>
                        }
                    </div>
                </div>
          </div>
      </main>
    );
};
export default Dish;
