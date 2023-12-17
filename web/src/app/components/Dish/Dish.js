import React, { useContext } from 'react';
import Image from 'next/image';
import styles from './Dish.module.css';
import { UserContext } from '../../store/userContext';
import { OrderAPI } from '../../global';
import { Button } from 'antd';

const TagMapping = {
    "早餐": "早",
    "午餐": "午",
    "晚餐": "晚",
    "蛋奶素": "素",
    "肉類": "肉",
    "海鮮": "海",
};

async function deleteOrder (userID, orderID, setDeleteOrder) {
    try {
        const response = await fetch(`${OrderAPI}/delete/${userID}/${orderID}`, {
            method: 'POST',
            headers: {
                'Accept': '*/*',
            },
        });
        if (response.ok) {
            setDeleteOrder(true);
            console.log("Delete Order successfully: ", response);
        }
        else {
            console.log("Delete Order failed: ", response);
            alert("刪除訂單失敗，請再試一次。");
        }
    }
    catch (err) {
        console.log("Delete Order error: ", err);
    }
}

async function confirmOrder (userID, orderID, setConfirmOrder) {
    try {
        const response = await fetch(`${OrderAPI}/confirm/${userID}/${orderID}`, {
            method: 'POST',
            headers: {
                'Accept': '*/*',
            },
        });
        if (response.ok) {
            setConfirmOrder(true);
            console.log("Confirm Order successfully: ", response);
        }
        else {
            console.log("Confirm Order failed: ", response);
            alert("完成訂單失敗，請再試一次。");
        }
    }
    catch (err) {
        console.log("Confirm Order error: ", err);
    }
}

const Dish = ({ dish, isOrder, orderType, setDeleteOrder, setConfirmOrder, userType }) => {
    const { userID } = useContext(UserContext);
    var orderCount = 0;
    var orderID = "";
    var restaurantName = "";
    if (isOrder) {
        orderCount = dish["count"];
        orderID = dish["orderID"];
        restaurantName = dish["restaurantName"];
        dish = dish["snapshot"];
    }
    return (
      <main>
          <div className={styles.container}>
                <div className={styles.imageContainer}>
                    <Image
                        src={dish["imageUrl"]}
                        alt="Avatar"
                        width={50}
                        height={50}
                        priority
                    />
                </div>
                <div className={styles.textContainer}>
                    {isOrder === true && <h3>店家: {restaurantName}</h3>}
                    {isOrder === true ? null : <h3>{dish["restaurantName"]}</h3>}
                    <h3>{dish["name"]}</h3>
                    <div className={styles.switchContainer}>
                        {
                            dish["tags"].map((tag, index) => (
                                tag != "Dinner" && tag != "Lunch" && tag != "Breakfast" &&
                                <div className={styles.circle} key={index}>{TagMapping[tag]}</div>
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
