'use client';
import DetailedDish from "../DetailedDish/DetailedDish"
import NumberButton from "../NumberButton/NumberButton";
import { UserContext } from "../../store/userContext";
import { FilterContext } from "../../store/filterContext";
import { OrderAPI } from "../../global";
import { useContext, useState } from "react";
import { ConfigProvider, Button, Modal } from 'antd';

import styles from "./DescriptionWindow.module.css";

async function sendOrderData(currentDate, mealType, dish, number, note, userID, setDesWindowState, setNumber, setNote, onOrderSent) {
    const TaiwanDate = new Date(currentDate.getTime() + (8 * 60 * 60 * 1000));
    const orders = {
        "orderDate": TaiwanDate.toISOString(),
        "mealType": mealType,
        "menuId": dish["menuID"],
        "foodItemId": dish["index"],
        "count": number,
        "description": note
    }

    try {
        const response = await fetch(`${OrderAPI}/create/${userID}`, {
            method: 'POST',
            headers: {
                'Accept': 'text/plain',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify( {...orders} ),
        });
        response.json().then(r => {
            console.log("Send order data: ", r);
            if (!r.result) {
                alert("訂餐失敗，請再試一次。");
            }
            else {
                alert("訂餐成功！");
            }
        });
        
        setDesWindowState(false);
        setNumber(1);
        setNote('');
        onOrderSent();
    }
    catch (err) {
        console.log("Send order data error: ", err);
        alert("訂餐失敗，請再試一次。");
    }
}

const DescriptionWindow = ({curDesWindowState, setDesWindowState, dish, onOrderSent}) => {
    const { userID } = useContext(UserContext);
    const { curFilterState } = useContext(FilterContext)
    const [ curNumber, setNumber ] = useState(1);
    const [ note, setNote] = useState('');
    const currentDate = new Date();
    
    return (
        <>
            <ConfigProvider
                theme={{
                    components: {
                        Modal: {
                            contentBg: '#e0f2ff',
                            width: 200
                        },
                    },
                }}
            >
                <Modal
                    width="100vw"
                    className={styles.modal_content}
                    open={curDesWindowState}
                    onOk={() => {
                        setDesWindowState(false); 
                        setNumber(1); 
                        setNote('');
                    }}
                    onCancel={() => {
                        setDesWindowState(false); 
                        setNumber(1); 
                        setNote('');
                    }}
                    footer={[
                        <div key={"recommend"} className={styles.recommend_div}>                
                            <Button 
                                // key="recommend" 
                                className={styles.recommend_button}
                                onClick={() => sendOrderData(currentDate, curFilterState["餐點時間"], dish, curNumber, note, userID, setDesWindowState, setNumber, setNote, onOrderSent)}
                            >
                                送出
                            </Button>
                        </div>
                    ]}
                    >
                    <div className={styles.container}>
                        <main>
                            <DetailedDish dish={ dish } />
                            
                            <div className={styles.notes}>
                                <div>
                                    備註：
                                    <br />
                                    <input
                                        className={styles.biggerInput}
                                        type="text"
                                        placeholder="請寫入備註"
                                        value={note}
                                        onChange={(e) => {setNote(e.target.value);}}
                                    />
                                </div>
                            </div>
                        </main>

                        <footer className={styles.footer}>
                            <div className={styles.rightButtons}>
                                <NumberButton curNumber={curNumber} setNumber={setNumber}/>
                            </div>
                        </footer>

                    </div>
                </Modal>
            </ConfigProvider>
        </>
    );
}

export default DescriptionWindow;