'use client';
import { useState } from 'react';
import { InputNumber, Button, Radio } from 'antd';
import { useRouter } from 'next/navigation';

import styles from "./NumberButton.module.css";

const NumberButton = () => {
    const [value, setValue] = useState(1);
  
    const onChange = (newValue) => {
      setValue(newValue);
      console.log(value);
    };
  
    const increment = () => {
      setValue((prevValue) => Math.min(prevValue + 1, 10));
      console.log(value);
    };
  
    const decrement = () => {
      setValue((prevValue) => Math.max(prevValue - 1, 1));
      console.log(value);
    };
  
    const router = useRouter();
    const handleButtonClick = async () => {
      console.log('choose', value);
      router.push('/routers/Home'); // Corrected the path by adding a slash
    };
  
    return (
      <div className={styles.container}>
        <div>
          <Button onClick={decrement}>-</Button>
          <InputNumber min={1} max={10} value={value} onChange={onChange} />
          <Button onClick={increment}>+</Button>
        </div>
  
        <div className={styles.sendButton}>
          <Radio.Button value="default" className={styles.blueButton} onClick={handleButtonClick}>
            送出
          </Radio.Button>
        </div>
      </div>
    );
  };
  
  export default NumberButton;
