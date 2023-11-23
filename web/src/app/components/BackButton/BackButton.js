import { LeftOutlined } from '@ant-design/icons';
import { Button } from 'antd';

import styles from './BackButton.module.css';

const UserButton = () => {
    return (
        <div>
            <Button type="primary" shape="circle" icon={<LeftOutlined />} className={styles.blueButton}/>
        </div>
    )
}

export default UserButton;