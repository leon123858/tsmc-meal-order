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
    menuData.forEach(menu => {
        const foodItems = menu["foodItems"];
        const menuID = menu["id"];
        foodItems.forEach((foodItem, index) => {
            foodItem["menuID"] = menuID;
            foodItem["index"] = index;
            Dishes.push(foodItem)
        })
    });
    return Dishes;
}

async function fetchMenuData(setMenuData, location) {
    const res = await fetch(`${MenuAPI}`, {
        method: 'GET',
        headers: {
            'Accept': 'application/json'
        }
    });
    // const res = await fetch(`${MenuAPI}/menu`);
    var data = await res.json();
    data = Object.values(data)[0].filter(item => item.location == location);
    setMenuData(getDish(data));
}

function filterData(curMenuData, setFilterData, curFilterState) {
    const Dishes = [];
    curMenuData.forEach(dish => {
        const filterSeafood = curFilterState["海鮮"] === ["蝦", "魚"].some((tag) => dish["tags"].includes(tag));
        const filterMeat = curFilterState["肉類"] === ["雞", "豬", "牛", "羊", "鴨", "鵝"].some((tag) => dish["tags"].includes(tag));
        const filterLactotene = curFilterState["蛋奶素"] === ["牛奶", "蛋"].some((tag) => dish["tags"].includes(tag));
        if (filterSeafood && filterMeat && filterLactotene) {
            Dishes.push(dish);
        }
    });
    setFilterData(Dishes);
}

export default function Home() {
    const [curAIWindowState, setAIWindowState] = useState(false);
    const [curDesWindowState, setDesWindowState] = useState(false);
    const [curMenuData, setMenuData] = useState([]);
    const [curSelectDish, setSelectDish] = useState({}); // 設定要傳入給 description window 的菜色
    const [curFilterData, setFilterData] = useState([]); // 儲存被種類過濾後的菜色
    const [curPlace, setPlace] = useState("None");
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
        if (userID != "" && curPlace == "") {
            alert("請先設定使用者名稱及地點");
        }
    }, [userID, curPlace])

    // 每次回到 menu 頁時，把 filter 的狀態初始化
    useEffect(() => {
        setFilterState((prevState) => ({
            ...prevState,
            "蛋奶素": false,
            "肉類": false,
            "海鮮": false
        }));
    }, [])    

    // 取回 menu，並進行 filter
    useEffect(() => {
        // fetchMenuData(setMenuData, curPlace);
        // filterData(curMenuData, setFilterData, curFilterState);
    }, [curPlace, curFilterState])

    const handleDishButton = (dish) => {
        setSelectDish(dish);
        setDesWindowState(true);
    }

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
                        curMenuData.map((dish, index) => ( 
                            <>
                                <div onClick={() => handleDishButton(dish)}>
                                    <Dish
                                        dish={dish}
                                        isHistory={false}
                                        historyType={""}
                                        setDeleteHistory={() => {}}
                                    />
                                    {index < curMenuData.length - 1 && <hr className={styles.hr_meal} />}
                                    {index === curMenuData.length - 1 && <hr className={styles.hr_date} />}                 
                                </div>
                            </>
                        ))
                    }
                    <DescriptionWindow
                        curDesWindowState={ curDesWindowState }
                        setDesWindowState={ setDesWindowState }
                        dish={ curSelectDish }
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

        </div>
    );
}