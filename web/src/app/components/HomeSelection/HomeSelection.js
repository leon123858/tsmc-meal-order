import { Select, Switch, Button, Radio } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { useContext, useEffect, useState } from 'react'
import { FilterContext } from '../../store/filterContext'
import Link from 'next/link'

import styles from './HomeSelection.module.css';

const categories = ['蛋奶素', '肉類', '海鮮'];

const onChange = (category, checked, setFilterState) => {
    setFilterState((prevState) => ({
        ...prevState,
        [category]: checked
    }));
};

const handleChange = (value, setFilterState) => {
    setFilterState((prevState) => ({
        ...prevState,
        "餐點時間": value
    }));
};


const HomeSelection = () => {
    const { setFilterState } = useContext(FilterContext);

    const today = new Date();
    const todayDateString = today.toLocaleDateString();

    return (
        <div className={styles.container}>
            <div className={styles.topRow}>
                <div className={styles.date}>
                    <p>{todayDateString}</p>
                </div>

                <div className={styles.right}>
                    <div className={styles.icon}>
                        <Link href="/routers/Report">
                            <Radio.Button value="default" className={styles.blueButton}>
                                月結報表
                            </Radio.Button>
                        </Link>
                        
                        <Link href="/routers/User">
                            <Button 
                                type="primary" 
                                shape="circle" 
                                icon={<UserOutlined />} 
                                className={styles.userButton}
                            />
                        </Link>
                    </div>
                </div>
            </div>


            <div className={styles.select}>
                <Select
                    defaultValue="lunch"
                    style={{ width: 120 }}
                    onChange={(value) => handleChange(value, setFilterState)}
                    options={[
                        { value: "Breakfast", label: "Breakfast" },
                        { value: "Lunch", label: "Lunch" },
                        { value: "Dinner", label: "Dinner" },
                    ]}
                />
            </div>

            <div className={styles.switchRow}>
                {categories.map((category, index) => (
                    <Switch
                        key={index}
                        checkedChildren={category}
                        unCheckedChildren={`無${category}`}
                        onChange={(checked) => onChange(category, checked, setFilterState)}
                        defaultChecked={true}
                    />
                ))}
            </div>
        </div>

    )
}
export default HomeSelection;