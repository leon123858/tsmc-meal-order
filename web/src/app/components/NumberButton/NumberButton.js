'use client';
import { useEffect, useState } from 'react';
import { InputNumber, Button } from 'antd';

import styles from "./NumberButton.module.css";

const NumberButton = ({curNumber, setNumber}) => {
    const onChange = (newValue) => {
      setNumber(newValue);
    };
  
    const increment = () => {
      setNumber((prevValue) => Math.min(prevValue + 1, 10));
    };
  
    const decrement = () => {
      setNumber((prevValue) => Math.max(prevValue - 1, 1));
    };
  
    return (
      <div className={styles.container}>
        <div>
          <Button onClick={decrement}>-</Button>
          <InputNumber min={1} max={10} value={curNumber} onChange={onChange} />
          <Button onClick={increment}>+</Button>
        </div>
      </div>
    );
  };
  
  export default NumberButton;
