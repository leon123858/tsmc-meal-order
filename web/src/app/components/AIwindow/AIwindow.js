import React from 'react';
import { useState, useEffect, useContext } from 'react';
import { ConfigProvider, Button, Modal, Input } from 'antd';
import { MenuAPI } from '../../global';
import Dish from '../Dish/Dish';
import DescriptionWindow from '../DescriptionWindow/DescriptionWindow';

import { UserContext } from '../../store/userContext';

import styles from './AIwindow.module.css';


function getRecDish (recMenuDish) {
    const recDishes = [];
    recMenuDish.forEach(menu => {
        if (menu["foodItem"]["name"] !== '新增餐點') {
            const foodItem = menu["foodItem"];
            const menuId = menu["menuId"];
            const restaurantName = menu["restaurantName"];
            const index = menu["index"];
            foodItem["menuID"] = menuId;
            foodItem["index"] = index;
            foodItem["restaurantName"] = restaurantName;
            recDishes.push(foodItem)
        }
    });
    return recDishes;
}

async function fetchRecMenuData(userID, userInput, setRecMenuData) {
    try {
        const res = await fetch(`${MenuAPI}/recommend/${userID}/${userInput}`, {
            method: 'GET',
            headers: {
                'Accept': 'application/json'
            }
        });
        var data = await res.json();
        data = Object.values(data)[0]["recFoodItems"];
        console.log("fetchRecommendMenuData Success: ", data);

        setRecMenuData(getRecDish(data));
    }
    catch (err) {
        console.log("fetchRecommendMenuData error: ", err);
    }
}

function filterData(curRecMenuData, setFilterData, curFilterState) {
    const RecDishes = [];
    const MealTypeMapping = {
        "Dinner": "晚餐",
        "Breakfast": "早餐",
        "Lunch": "午餐"
    }
    try {
        curRecMenuData.forEach(dish => {
            const filterTime = dish["tags"].includes(MealTypeMapping[curFilterState["餐點時間"]]);
            const filterSeafood = curFilterState["海鮮"] && dish["tags"].includes("海鮮");
            const filterMeat = curFilterState["肉類"] && dish["tags"].includes("肉類");
            const filterLactotene = curFilterState["蛋奶素"] && dish["tags"].includes("蛋奶素");
            if (filterTime && (filterSeafood || filterMeat || filterLactotene)) {
                RecDishes.push(dish);
            }
        });
        setFilterData(RecDishes);
        console.log("filterData Success: ", RecDishes);
    }
    catch (err) {
        console.log("filterData error: ", err);
    }
}

const AIwindow = ({ curAIWindowState, setAIWindowState, curFilterState }) => {
    const [text, setText] = useState("");
    const [curRecMenuData, setRecMenuData] = useState([]);
    const [curFilterData, setFilterData] = useState([]); // 儲存被種類過濾後的菜色
    const [curDesWindowState, setDesWindowState] = useState(false);
    const [curSelectDish, setSelectDish] = useState({}); // 設定要傳入給 description window 的菜色

    const { userID } = useContext(UserContext);

    const handleTextChange = (e, setText) => {
        setText(e.target.value);
    };

    const onClick = async () => {
        if (text === "") {
            alert("請輸入您的需求！");
            return;
        }
        fetchRecMenuData(userID, text, setRecMenuData);
    }

    useEffect(() => {
        filterData(curRecMenuData, setFilterData, curFilterState)
    }, [curRecMenuData]);

    const handleDishButton = (dish) => {
        setSelectDish(dish);
        setDesWindowState(true);
    }

    const handleOrderSent = () => {
        fetchRecMenuData(userID, text, setRecMenuData);
    };

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
                    open={curAIWindowState}
                    onOk={() => setAIWindowState(false)}
                    onCancel={() => setAIWindowState(false)}
                    footer={[
                            <div key={"recommend"} className={styles.recommend_div}>
                                <div className={styles.recommend_wrapper}>
                                    <Button
                                        className={styles.recommend_button}
                                        onClick={onClick}
                                    >
                                        推薦
                                    </Button>
                                    <div className={styles.dishContainer}>
                                        {curFilterData.map((dish, index) => (
                                            <div key={index} onClick={() => handleDishButton(dish)}>
                                                <Dish
                                                    dish={dish}
                                                    isOrder={false}
                                                    orderType={""}
                                                    setDeleteOrder={() => {}}
                                                />
                                                {index < curFilterData.length - 1 && <hr className={styles.hr_meal} />}
                                                {index === curFilterData.length - 1 && <hr className={styles.hr_date} />}
                                            </div>
                                        ))}
                                        <DescriptionWindow
                                            curDesWindowState={curDesWindowState}
                                            setDesWindowState={setDesWindowState}
                                            dish={curSelectDish}
                                            onOrderSent={handleOrderSent}
                                        />
                                    </div>
                                </div>
                            </div>
                    ]}
                >
                    <div>
                        <b
                            face="monospace" 
                            size="6"
                        >
                            請輸入您的需求，我們將為您推薦最適合的餐點！
                        <br />
                        </b>
                        <Input value={text} onChange={(e) => handleTextChange(e, setText)} />
                    </div>
                </Modal>
            </ConfigProvider>
        </>
    );
};
export default AIwindow;