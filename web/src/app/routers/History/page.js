'use client';
import styles from './page.module.css';
import { Segmented } from 'antd';
import { useState, useEffect } from 'react';
import Link from 'next/link'
import DailyHistory from '../../components/DailyHistory/DailyHistory'
import BackButton from "../../components/BackButton/BackButton";

export default function History() {
    const HistoryTypes = [
        "已訂購",
        "已取消",
        "已完成"
    ];
    const [curHistoryState, setHistoryState] = useState("已訂購");
    
    useEffect(() => {
        // window.alert(curHistoryState)
    }, [curHistoryState])

    return (
        <div>

            <header className={styles.header}>
                <div className={styles.selectionContainer}>
                    <div className={styles.backButton}>
                        <Link href={"/routers/Home"}>
                            <BackButton />
                        </Link>
                    </div>
                    
                    <Segmented
                        options={HistoryTypes}
                        value={curHistoryState}
                        onChange={setHistoryState}
                        className={styles.segment}
                    ></Segmented>
                </div>
            </header>

            <main className={styles.main}>
                <div className={styles.dishContainer}>
                    <DailyHistory 
                        curHistoryState={curHistoryState}
                        date={"2023/11/08"}
                    />
                    <DailyHistory 
                        curHistoryState={curHistoryState}
                        date={"2023/11/07"}
                    />
                    <DailyHistory 
                        curHistoryState={curHistoryState}
                        date={"2023/11/06"}
                    />
                    <DailyHistory 
                        curHistoryState={curHistoryState}
                        date={"2023/11/05"}
                    />
                </div>
            </main>
        </div>
    );
}
