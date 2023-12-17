import { Form, Input, Radio, InputNumber, Switch, message } from 'antd';
import { useState, useContext, useEffect } from 'react';
import { UserContext } from '../../store/userContext';
import { UserAPI, MenuAPI } from '../../global';
import { UploadImage } from '../UploadImage/UploadImage';
import Image from 'next/image';


import styles from './UploadDish.module.css';

const tags = ['蛋奶素', '肉類', '海鮮', '早餐', '午餐', '晚餐'];

async function fetchUser(userID, setUser) {
    const res = await fetch(`${UserAPI}/get?uid=${userID}`, {
      method: 'GET',
      headers: {
        'Accept': 'application/json'
      }
    });
    var data = await res.json();
    data = Object.values(data)[2];
    setUser((prevState) => ({
      ...prevState,
      "name": data["name"],
      "uid": userID,
    }))
}

async function fetchUpdateMenu(menuData) {
    try {
        const res = await fetch(`${MenuAPI}`, {
            method: 'POST',
            headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            },
            body: JSON.stringify(menuData),
        });
  
        if (!res.ok) {
            throw new Error(`HTTP error! Status: ${res.status}`);
        }
  
        const response = await res.json();
        console.log('Menu updated successfully:', response);
        return response;
    } catch (error) {
        console.error('Error updating menu:', error.message);
        throw error;
    }
}

const handleCreateMenu = async (user, index, values, menuFood, setMenuFood, selectedTags, setUpdate) => {
    console.log('UPLOAD DISH USER:', user)
    try {
        console.log('Values:', values);

        // Check if any of the values is undefined
        if (
            values.dishName === undefined || values.dishName === '' ||
            selectedTags === undefined || selectedTags === 0 ||
            values.dishPrice === undefined ||
            values.dishCountLimit === undefined ||
            values.dishDescription === undefined || values.dishDescription === '' ||
            values.dishUrl === undefined || values.dishUrl === ''
        ) {
            alert('每個欄位都要填選！');
            return;
        }

        const updatedFoodItems = [...menuFood];
        updatedFoodItems[index] = {
            description: values.dishDescription,
            name: values.dishName,
            price: values.dishPrice,
            countLimit: values.dishCountLimit,
            imageUrl: values.dishUrl,
            tags: selectedTags
        };
        
		const isNewDishExists = updatedFoodItems.some(item => item.name === '新增餐點');
		if (!isNewDishExists) {
			updatedFoodItems.push({
				description: '',
				name: '新增餐點',
				price: 1,
				countLimit: 1,
				imageUrl: '',
				tags: [],
			});
		}

        console.log('Updated Food Items:', updatedFoodItems);

        const menuData = {
            id: user["uid"], 
            name: user["name"],
            foodItems: updatedFoodItems,
        };
        const response = await fetchUpdateMenu(menuData);
        console.log('Menu created successfully:', response);

        setMenuFood(updatedFoodItems);
        setUpdate(true);
    } catch (error) {
        console.error('Error creating menu:', error.message);
    }
};

const handleDeleteMenu = async (user, index, menuFood, setMenuFood, setUpdate) => {
    try {
        const updatedFoodItems = [...menuFood];

        // Remove the item at the specified index
        updatedFoodItems.splice(index, 1);
        setMenuFood(updatedFoodItems);

        const menuData = {
            id: user["uid"], 
            name: user["name"],
            foodItems: updatedFoodItems,
        };
        const response = await fetchUpdateMenu(menuData);

        setMenuFood(updatedFoodItems);
        setUpdate(true);
        console.log('Menu deleted successfully:', response);
    } catch (error) {
        console.error('Error deleting menu:', error.message);
    }
};

const UploadDish = ({ index, menuFood, setMenuFood }) => {
    const [form] = Form.useForm();
    const { userID } = useContext(UserContext);
    const [user, setUser] = useState({
        name: "",
        uid: "",
    });
    const [price, setPrice] = useState(menuFood[index].price);
    const [countLimit, setCountLimit] = useState(menuFood[index].countLimit);
    const [selectedTags, setSelectedTags] = useState(menuFood[index].tags);
    const [update, setUpdate] = useState(false);
  
    const onPriceChange = (newValue) => {
      setPrice(newValue);
    };

    const onCountLimitChange = (newValue) => {
      setCountLimit(newValue);
    };

    const onTagChange = (tag, checked) => {
        setSelectedTags((prevTags) => {
          if (checked) {
            return [...prevTags, tag];
          } else {
            return prevTags.filter((selectedTag) => selectedTag !== tag);
          }
        });
    };

    useEffect(() => {
        if (userID != "") {
          fetchUser(userID, setUser);
        }
    }, [userID]);

    useEffect(() => {
        setUpdate(false);
    }, [update])

    useEffect(() => {
        if (userID !== "" && menuFood[index]) {
            fetchUser(userID, setUser);
            form.setFieldsValue({
                dishName: menuFood[index].name || '',
                dishPrice: menuFood[index].price || 1,
                dishCountLimit: menuFood[index].countLimit || 1,
                dishDescription: menuFood[index].description || '',
                dishTags: menuFood[index].tags || [],
                dishUrl: menuFood[index].imageUrl || ''
            });
            setSelectedTags(menuFood[index].tags || []);
        }
    }, [userID, index, menuFood]);

    return (
        <main>
            <div className={styles.container}>
                <Form
                    name="uploadDishForm"
                    form={form}
                    onFinish={handleCreateMenu}
                    initialValues={{ 
                        dishName: menuFood[index].name || '', 
                        dishPrice: menuFood[index].price || 1, 
                        dishCountLimit: menuFood[index].countLimit || 1, 
                        dishDescription: menuFood[index].description || '', 
                        dishTags: menuFood[index].tags || [],
                        dishUrl: menuFood[index].imageUrl || '',
                    }}
                >
                    <div className={styles.formContent}>
                        <div className={styles.imageContainer}>
                            <Form.Item
                                name="dishImage"
                                valuePropName="fileList"
                                getValueFromEvent={(normFile) => normFile.fileList}
                            >
                                <UploadImage
									setUrl={(url) => {
										const el = document.createElement('textarea');
										el.value = url;
										document.body.appendChild(el);
										el.select();
										document.execCommand('copy');
										document.body.removeChild(el);
										message.success('以複製上傳圖片網址, 可至下方文字筐貼上');
									}}
                                    _isUpload={false}
                                    text='上傳圖片並複製'
                                    index={index}
                                    update={update}
                                ></UploadImage>
                            </Form.Item>
                            { menuFood[index].imageUrl !== '' && 
                                <div>
                                    <h3>原始上傳圖片</h3>
                                    <Image
                                        src={menuFood[index].imageUrl}
                                        alt="Avatar"
                                        width={200}
                                        height={200}
                                        priority
                                    />
                                </div>
                            }
                        </div>

                        <div>
                            <Form.Item name="dishName" label="名稱" >
                                <Input.TextArea placeholder="輸入餐點名稱" className={styles.dishNameText} />
                            </Form.Item>

                            <Form.Item name="dishTags" label="類別">
                                <div className={styles.switchRow}>
                                    {tags.map((tag, index) => (
                                        <Switch
                                            key={index}
                                            checkedChildren={tag}
                                            unCheckedChildren={`無${tag}`}
                                            checked={selectedTags.includes(tag)}
                                            onChange={(checked) => onTagChange(tag, checked)}
                                        />
                                    ))}
                                </div>
                            </Form.Item>

                            <Form.Item name="dishPrice" label="價格" >
                                <InputNumber min={1} value={price} onChange={onPriceChange} />
                            </Form.Item>

                            <Form.Item name="dishCountLimit" label="份數" >
                                <InputNumber min={1} value={countLimit} onChange={onCountLimitChange} />
                            </Form.Item>

                            <Form.Item name="dishDescription" label="介紹" >
                                <Input.TextArea placeholder="輸入餐點介紹" />
                            </Form.Item>

                            <Form.Item name="dishUrl" label="圖片連結" >
                                <Input.TextArea placeholder="圖片連結" />
                            </Form.Item>
                        </div>
                    </div>

                    <div style={{ marginTop: 'auto', textAlign: 'right' }}>
                        <Radio.Button 
                            value="default" 
                            className={styles.redButton} 
                            onClick={() => handleDeleteMenu(user, index, menuFood, setMenuFood, setUpdate)}
                        >
                            刪除餐點
                        </Radio.Button>
                        
                        <Radio.Button 
                            value="default" 
                            className={styles.deepBlueButton} 
                            onClick={() => handleCreateMenu(user, index, form.getFieldsValue(), menuFood, setMenuFood, selectedTags, setUpdate)}
                        >
                            儲存變更
                        </Radio.Button>
                    </div>

                </Form>
            </div>
        </main>
    );
};

export default UploadDish;
