import React from 'react';
import Image from 'next/image';
import styles from './Dish.module.css';

const Dish = ({ dish, isHistory }) => {
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
                                <div className={styles.circle} key={index}>{tag}</div>
                            ))
                        }
                        <br />
                    </div>
                </div>
                <div className={styles.priceContainer}>
                    <div className={styles.priceContent}>
                        $ {dish["price"]} <br />
                        {isHistory !== true && `剩下 ${dish["count"]} 份`}
                    </div>
                </div>
          </div>
      </main>
    );
};
export default Dish;
