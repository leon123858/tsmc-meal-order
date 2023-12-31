'use client';
import Link from 'next/link'
import { useState, useEffect, useContext } from 'react';
import { MenuAPI, UserAPI } from '../../global'
import Dish from "../../components/Dish/Dish";
import HomeSelection from "../../components/HomeSelection/HomeSelection";
import AIwindow from "../../components/AIwindow/AIwindow";
import DescriptionWindow from "../../components/DescriptionWindow/DescriptionWindow";

import { Button, Radio } from 'antd';
import { FilterContext } from '../../store/filterContext'
import { UserContext } from '../../store/userContext';

import styles from "./page.module.css";

async function fetchUser(userID, setPlace) {
    const res = await fetch(`${UserAPI}/get?uid=${userID}`, {
      method: 'GET',
      headers: {
        'Accept': 'application/json'
      }
    });
    var data = await res.json();
    data = Object.values(data)[2];
    setPlace(data["place"])
}

function getDish (menuData) {
    const Dishes = [];
    if (menuData.length !== 0) {
        menuData.forEach(menu => {
            const foodItems = menu["foodItems"];
            const menuID = menu["id"];
            const restaurantName = menu["name"];
            foodItems.forEach((foodItem, index) => {
                foodItem["menuID"] = menuID;
                foodItem["index"] = index;
                foodItem["restaurantName"] = restaurantName;
                Dishes.push(foodItem)
            })
        });
    }
    else { 
        alert("目前該地區沒有餐點。");
    }
    return Dishes;
}

async function fetchMenuData(setMenuData, userID) {
    const res = await fetch(`${MenuAPI}/user/${userID}`, {
        method: 'GET',
        headers: {
            'Accept': 'application/json'
        }
    });
    var data = await res.json();
    data = Object.values(data)[0];
    setMenuData(getDish(data));
}

function filterData(curMenuData, setFilterData, curFilterState) {
    const Dishes = [];
    const MealTypeMapping = {
        "Dinner": "晚餐",
        "Breakfast": "早餐",
        "Lunch": "午餐"
    }
    curMenuData.forEach(dish => {
        const filterTime = dish["tags"].includes(MealTypeMapping[curFilterState["餐點時間"]]);
        const filterSeafood = curFilterState["海鮮"] && dish["tags"].includes("海鮮");
        const filterMeat = curFilterState["肉類"] && dish["tags"].includes("肉類");
        const filterLactotene = curFilterState["蛋奶素"] && dish["tags"].includes("蛋奶素");
        if (filterTime && (filterSeafood || filterMeat || filterLactotene)) {
            Dishes.push(dish);
        }
    });
    setFilterData(Dishes);
}

export default function Home() {
    const [curAIWindowState, setAIWindowState] = useState(false);
    const [curDesWindowState, setDesWindowState] = useState(false);
    const [curSelectDish, setSelectDish] = useState({}); // 設定要傳入給 description window 的菜色
    const [curMenuData, setMenuData] = useState([]);
    const [curFilterData, setFilterData] = useState([]); // 儲存被種類過濾後的菜色
    const [curPlace, setPlace] = useState("None");
    const [isModalOpen, setIsModalOpen] = useState(false);

    const {curFilterState, setFilterState} = useContext(FilterContext);
    const { userID } = useContext(UserContext);

    // 取得 Location
    useEffect(() => {
        if (userID != "") {
            fetchUser(userID, setPlace);
        }
    }, [userID]);

    // 檢查是否為第一次登入的用戶，需先去設定名字及地點
    useEffect(() => {
        if (userID !== '' && curPlace === '') {
            setIsModalOpen(true);
        }
    }, [userID, curPlace]);
    
    // 每次回到 menu 頁時，把 filter 的狀態初始化
    useEffect(() => {
        setFilterState((prevState) => ({
            ...prevState,
            "蛋奶素": true,
            "肉類": true,
            "海鮮": true,
            "餐點時間": "Lunch"
        }));
    }, []);

    // 一進來頁面，先 fetch menu 資料
    useEffect(() => {
        if (userID != "") {
            fetchMenuData(setMenuData, userID);
        }
    }, [userID, curPlace]);

    useEffect(() => {
        filterData(curMenuData, setFilterData, curFilterState)
    }, [curFilterState, curMenuData]);

    const handleDishButton = (dish) => {
        setSelectDish(dish);
        setDesWindowState(true);
    }

    const handleOrderSent = () => {
        fetchMenuData(setMenuData, userID);
    };

    const Modal = ({ isOpen, onClose }) => {
        const handleConfirm = () => {
            onClose();  // Close the modal
        };
    
        return (
            isOpen && (
                <div className={styles.modal}>
                    <p>第一次登入請設定使用者名稱及地點</p>
                    <Link href="/routers/User">
                        <button onClick={handleConfirm}>
                            跳轉設定頁面
                        </button>
                    </Link>
                </div>
            )
        );
    };

    return (
        <div>
            <header className={styles.header}>
                <div className={styles.selectionContainer}>
                    <HomeSelection/>
                </div>
            </header>
        
            <main className={styles.main}>
                <div className={styles.dishContainer}>
                    {
                        curFilterData.map((dish, index) => ( 
                            <>
                                <div onClick={() => handleDishButton(dish)}>
                                    <Dish
                                        dish={dish}
                                        isOrder={false}
                                        orderType={""}
                                        setDeleteOrder={() => {}}
                                    />
                                    {index < curFilterData.length - 1 && <hr className={styles.hr_meal} />}
                                    {index === curFilterData.length - 1 && <hr className={styles.hr_date} />}                 
                                </div>
                            </>
                        ))
                    }
                    <DescriptionWindow
                        curDesWindowState={ curDesWindowState }
                        setDesWindowState={ setDesWindowState }
                        dish={ curSelectDish }
                        onOrderSent={ handleOrderSent }
                    />
                </div>
            </main>

            <footer className={styles.footer}>
                <div className={styles.buttonContainer}>

                    <Button 
                        type="primary"
                        shape="circle"
                        onClick={() => setAIWindowState(true)}
                        className={styles.blueButton}
                    >
                        AI
                    </Button>
                    <AIwindow 
                        curAIWindowState={ curAIWindowState }
                        setAIWindowState={ setAIWindowState }
                        curFilterState={ curFilterState }
                    />

                    <div className={styles.rightButtons}>
                        <Link href="/routers/History">
                            <Radio.Button value="default" className={styles.blueButton}>
                                檢視訂單
                            </Radio.Button>
                        </Link>
                    </div>

                </div>
            </footer>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
        </div>
    );
}