'use client';
import BackButton from "../../components/BackButton/BackButton";
import Link from 'next/link'
import { UserOutlined } from "@ant-design/icons";
import { Select } from "antd";
import { Radio } from 'antd';

import styles from "./page.module.css";

const handleChange = (value) => {
    console.log(`selected ${value}`);
  };

export default function User() {
    return (
        <div>
            <header className={styles.header}>
                <Link href={"/routers/Home"}>
                    <BackButton />
                </Link>
            </header>
        
            <main className={styles.main}>
                <UserOutlined className={styles.largerIcon} />
                <div className={styles.settings}>
                    <h3>User Name</h3>
                    <p>User ID</p>
                    <h3>廠區</h3>
                    <div>
                        <Select
                            defaultValue="hsinchu"
                            style={{ width: 120, marginBottom: 10 }}
                            onChange={handleChange}
                            options={[
                                { value: "hsinchu", label: "Hsinchu" },
                                { value: "tainan", label: "Tainan" },
                                { value: "taipei", label: "Taipei" },
                                { value: "taichung", label: "Taichung" },
                            ]}
                        />
                    </div>
                    
                    <Radio.Button value="default" className={styles.blueButton}>
                        確認
                    </Radio.Button>
                </div>
            </main>
        </div>
    );
}