import React from 'react';
import Image from 'next/image';
import styles from './Dish.module.css';

const Dish = ({ price = 0, number = -1 }) => {
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
                    <h3>DISH NAME</h3>
                    <div className={styles.switchContainer}>
                        <div className={styles.circle}>牛</div>
                        <div className={styles.circle}>蝦</div> <br />
                    </div>
                </div>
                <div className={styles.priceContainer}>
                    $ {price} <br />
                    {number !== -1 && `剩下 ${number} 份`}
                </div>
          </div>
      </main>
    );
};
export default Dish;
