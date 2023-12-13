import styles from './DailyOrder.module.css';
import Dish from '../Dish/Dish'
import React from 'react'

function getDish (orderData) {
    const Dishes = [];
    orderData.forEach(order => {
        const foodItems = order["foodItems"];
        foodItems.forEach(foodItem => {
            foodItem["orderID"] = order["id"];
            Dishes.push(foodItem);
        })
    });
    return Dishes;
}

const DailyOrder = ({ data, date, orderType, setDeleteOrder, setConfirmOrder, userType }) => {

    const Dishes = getDish(data);

    return (
        <div className={styles.container}>
            <font face="monospace" size="4">
                {date}
            </font>
            <hr className={styles.hr_date} />
            {
                Dishes.map((dish, index) => (
                    <React.Fragment key={index}>
                        <Dish 
                            dish={dish}
                            isOrder={true}
                            orderType={orderType}
                            setDeleteOrder={setDeleteOrder}
                            setConfirmOrder={setConfirmOrder}
                            userType={userType}
                        />
                        {index < Dishes.length - 1 && <hr className={styles.hr_meal} />}
                        {index === Dishes.length - 1 && <hr className={styles.hr_date} />}
                    </React.Fragment>
                ))
            }
        </div>
    );
};

export default DailyOrder;