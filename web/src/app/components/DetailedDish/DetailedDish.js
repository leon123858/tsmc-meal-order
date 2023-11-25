import styles from './DetailedDish.module.css';

const DetailedDish = ({ price = 0 }) => {

    return (
    <main>
        <div className={styles.container}>
            <div className={styles.imageContainer}>
                <img src="https://www.w3schools.com/howto/img_avatar.png" alt="Avatar" />
            </div>
            <div className={styles.textContainer}>
                <h2>DISH NAME</h2>
                <div className={styles.ingredientContainer}>
                    <div className={styles.circle}>蝦</div>
                    <div className={styles.circle}>魚</div> <br />
                </div>
                <p>$ {price}</p>
            </div>
        </div>
        <div className={styles.container}>
            <p>
                餐點介紹： <br /> 
                This is a delicious food! This is a delicious food! This is a delicious food! This is a delicious food! This is a delicious food! 
            </p>
        </div>

    </main>
    );
}
export default DetailedDish;