'use client';
import Link from "next/link";
import BackButton from "../../components/BackButton/BackButton";
import DetailedDish from "../../components/DetailedDish/DetailedDish";
import NumberButton from "../../components/NumberButton/NumberButton";

import styles from "./page.module.css";

export default function Description() {
    const price = 100;
    return (
        <div className={styles.container}>
            <header className={styles.header}>
                <Link href={"/routers/Home"}>
                   <BackButton />
                </Link>
            </header>

            <main>
                <DetailedDish price = { price } />
                
                <div className={styles.notes}>
                    <div>
                        備註：
                        <br />
                        <input
                            className={styles.biggerInput}
                            type="text"
                            placeholder="請寫入備註"
                        />
                    </div>
                </div>
            </main>

            <footer className={styles.footer}>
                <div className={styles.rightButtons}>
                        <NumberButton />
                </div>
            </footer>

        </div>
    );
}