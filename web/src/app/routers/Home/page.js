'use client';
import Link from 'next/link'
import { useState, useEffect } from 'react';
import Dish from "../../components/Dish/Dish";
import HomeSelection from "../../components/HomeSelection/HomeSelection";
import AIwindow from '../../components/AIwindow/AIwindow'
import { Button, Radio } from 'antd';

import styles from "./page.module.css";

export default function Home() {
    const price = 10, number = 1;
    const [curWindowState, setWindowState] = useState(false);
    
    return (
        <div>

            <header className={styles.header}>
                <div className={styles.selectionContainer}>
                    <HomeSelection />
                </div>
            </header>
        
            <main className={styles.main}>
                <div className={styles.dishContainer}>
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                    <Dish price={price} number={number} />
                </div>
            </main>

            <footer className={styles.footer}>
                <div className={styles.buttonContainer}>

                    <Button 
                        type="primary"
                        shape="circle"
                        onClick={() => setWindowState(true)}
                        className={styles.blueButton}
                    >
                        AI
                    </Button>
                    <AIwindow 
                        curWindowState={ curWindowState }
                        setWindowState={ setWindowState }
                    />

                    <div className={styles.rightButtons}>
                        <div className={styles.items}>X items</div>


                        <Link href="/routers/History">
                            <Radio.Button value="default" className={styles.blueButton}>
                                檢視訂單
                            </Radio.Button>
                        </Link>
                    </div>

                </div>
            </footer>

        </div>
    );
}