import { DatePicker, Select, Switch, Button } from 'antd';
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
    const [today, setDate] = useState(new Date());

    useEffect(() => {
        const timer = setInterval(() => {
            setDate(new Date());
        }, 60 * 1000)
        return () => {
            clearInterval(today);
        }
    }, []);

    return (
        <div className={styles.container}>
            <div className={styles.topRow}>
                <div className={styles.datePicker}>
                    <DatePicker showTime={{ defaultValue: today}} />
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

                <div className={styles.icon}>
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

            <div className={styles.switchRow}>
                {categories.map((category, index) => (
                    <Switch
                        key={index}
                        checkedChildren={category}
                        unCheckedChildren={`無${category}`}
                        onChange={(checked) => onChange(category, checked, setFilterState)}
                    />
                ))}
            </div>
        </div>

    )
}
export default HomeSelection;