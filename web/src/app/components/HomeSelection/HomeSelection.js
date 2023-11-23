import { DatePicker, Select, Switch, Button } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import Link from 'next/link'

import styles from './HomeSelection.module.css';

const categories = ['蛋奶素', '肉類', '海鮮'];

const onChange = (date, dateString) => {
    console.log(date, dateString);
  };

const handleChange = (value) => {
    console.log(`selected ${value}`);
  };


const HomeSelection = () => {
    return (
        <div className={styles.container}>
            <div className={styles.topRow}>
                <div className={styles.datePicker}>
                    <DatePicker onChange={onChange} style={{ width: "100%" }} />
                </div>

                <div className={styles.select}>
                    <Select
                        defaultValue="lunch"
                        style={{ width: 120 }}
                        onChange={handleChange}
                        options={[
                            { value: "breakfast", label: "Breakfast" },
                            { value: "lunch", label: "Lunch" },
                            { value: "dinner", label: "Dinner" },
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
                {/* <Switch checkedChildren="蛋奶素" unCheckedChildren="無蛋奶素" onChange={onChange}/>
                <Switch checkedChildren="肉類" unCheckedChildren="無肉類" onChange={onChange}/>
                <Switch checkedChildren="海鮮" unCheckedChildren="無海鮮" onChange={onChange}/> */}

                {categories.map((category, index) => (
                    <Switch
                        key={index}
                        checkedChildren={category}
                        unCheckedChildren={`無${category}`}
                        onChange={(checked) => onChange(category, checked)}
                    />
                ))}
            </div>
        </div>

    )
}
export default HomeSelection;