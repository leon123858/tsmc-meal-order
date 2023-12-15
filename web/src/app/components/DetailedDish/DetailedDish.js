import Image from 'next/image';
import styles from './DetailedDish.module.css';

const DetailedDish = ({ dish }) => {

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
                <h2>{dish.name}</h2>
                <div className={styles.ingredientContainer}>
                    {
                        dish["tags"].map((tag, index) => (
                            <div className={styles.circle} key={index}>{tag}</div>
                        ))
                    }
                    <br />
                </div>
                <p>$ {dish.price}</p>
            </div>
        </div>
        <div className={styles.container}>
            <p>
                餐點介紹： <br /> 
                {dish.description} 
            </p>
        </div>

    </main>
    );
}
export default DetailedDish;