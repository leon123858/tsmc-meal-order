import styles from './DailyHistory.module.css';
import Dish from '../Dish/Dish'
import React from 'react'

function getDish (orderData) {
    const Dishes = [];
    orderData.forEach(order => {
        const foodItems = order["foodItems"];
        foodItems.forEach(foodItem => {
            Dishes.push(foodItem);
        })
    });
    return Dishes;
}

const DailyHistory = ({ data, date }) => {

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
                            isHistory={true}
                        />
                        {index < Dishes.length - 1 && <hr className={styles.hr_meal} />}
                        {index === Dishes.length - 1 && <hr className={styles.hr_date} />}
                    </React.Fragment>
                ))
            }
        </div>
    );
};

export default DailyHistory;