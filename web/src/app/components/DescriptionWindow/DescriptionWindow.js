'use client';
import DetailedDish from "../DetailedDish/DetailedDish"
import NumberButton from "../NumberButton/NumberButton";
import { UserContext } from "../../store/userContext";
import { OrderAPI } from "../../global";
import { useContext } from "react";
import { ConfigProvider, Button, Modal } from 'antd';

import styles from "./DescriptionWindow.module.css";

async function sendOrderData(dish, number, date, userID, setDesWindowState) {
    const foodItems = Array(number).fill(dish["index"]);
    const orders = {
        "menuId": dish["menuID"],
        "orderDate": date,
        "foodItemIds": foodItems
    }
    const response = await fetch(`${OrderAPI}/create/${userID}`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify( {...orders} ),
    });
    response.json().then(r => console.log(r));
    setDesWindowState(false);
}

const DescriptionWindow = ({curDesWindowState, setDesWindowState, dish}) => {
    const { userID } = useContext(UserContext);
    console.log(dish["index"]);
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
                    onOk={() => setDesWindowState(false)}
                    onCancel={() => setDesWindowState(false)}
                    footer={[
                        <div key={"recommend"} className={styles.recommend_div}>                
                            <Button 
                                // key="recommend" 
                                className={styles.recommend_button}
                                onClick={() => sendOrderData(dish, 1, "2023-12-08", userID, setDesWindowState)}
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
                                    />
                                </div>
                            </div>
                        </main>

                        <footer className={styles.footer}>
                            <div className={styles.rightButtons}>
                                    <NumberButton />
                            </div>
                        </footer>

                    </div>
                </Modal>
            </ConfigProvider>
        </>
    );
}

export default DescriptionWindow;