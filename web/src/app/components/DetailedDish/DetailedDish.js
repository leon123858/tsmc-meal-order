import Image from 'next/image';
import styles from './DetailedDish.module.css';

const TagMapping = {
    "早餐": "早",
    "午餐": "午",
    "晚餐": "晚",
    "蛋奶素": "素",
    "肉類": "肉",
    "海鮮": "海",
};

const DetailedDish = ({ dish }) => {

    return (
    <main>
        <div className={styles.container}>
            <div className={styles.imageContainer}>
                <Image
                    src={dish.imageUrl}
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
                            <div className={styles.circle} key={index}>{TagMapping[tag]}</div>
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