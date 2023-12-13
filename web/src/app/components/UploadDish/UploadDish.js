import { Upload, Button, Form, Input, Radio, InputNumber, Switch } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import { useState, useContext, useEffect } from 'react';
import { UserContext } from '../../store/userContext'
import { UserAPI, MenuAPI } from '../../global'

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

async function fetchCreateMenu(menuData) {
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
        console.log('Menu created successfully:', response);
        return response;
    } catch (error) {
        console.error('Error creating menu:', error.message);
        throw error;
    }
}

const handleCreateMenu = async (user, index, values, imageUrl, menuFood, setMenuFood, selectedTags, fetchMenuAndUpdate) => {
    try {
        console.log('Values:', values);
        console.log('Image URL:', imageUrl);

        // Check if any of the values is undefined
        if (
            values.dishName === undefined || values.dishName === '' ||
            values.dishTags === undefined || values.dishTags === [] ||
            values.dishPrice === undefined ||
            values.dishCountLimit === undefined ||
            values.dishDescription === undefined || values.dishDescription === '' ||
            values.dishImage === undefined || values.dishImage === ''
        ) {
            alert('每個欄位都要填選！');
            return;
        }

        // const dishImage = values.dishImage[0].originFileObj;
        // const imageUrl = await uploadImageAndGetUrl(dishImage);
        // console.log('Image URL:', imageUrl);

        const updatedFoodItems = [...menuFood];
        updatedFoodItems[index] = {
            description: values.dishDescription,
            name: values.dishName,
            price: values.dishPrice,
            countLimit: values.dishCountLimit,
            imageUrl: "/Users/chingtingtai/Desktop/daidai/visualstudiocode/2023_fall/CloudNative/tsmc-meal-order/web/public/images/pig.jpeg",
            tags: selectedTags
        };

        const menuData = {
            id: user["uid"], 
            name: user["name"],
            foodItems: updatedFoodItems,
        };
        const response = await fetchCreateMenu(menuData);

        setMenuFood(updatedFoodItems);
        console.log('Menu created successfully:', response);

        try {
            const res = await fetchMenuAndUpdate();
        }
        catch (error) {
            console.error('Error fetching menu:', error.message);
        }
    } catch (error) {
        console.error('Error creating menu:', error.message);
    }
};

const handleDeleteMenu = async (user, index, menuFood, setMenuFood, fetchMenuAndUpdate) => {
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
        const response = await fetchCreateMenu(menuData);

        setMenuFood(updatedFoodItems);
        console.log('Menu deleted successfully:', response);
        
        try {
            const res = await fetchMenuAndUpdate();
        }
        catch (error) {
            console.error('Error fetching menu:', error.message);
        }
    } catch (error) {
        console.error('Error deleting menu:', error.message);
    }
};

const UploadDish = ({ index, menuFood, setMenuFood, fetchMenuAndUpdate }) => {
    const [form] = Form.useForm();
    const { userID } = useContext(UserContext);
    const [user, setUser] = useState({
      name: "",
      uid: "",
    });
    const [price, setPrice] = useState(menuFood[index].price);
    const [countLimit, setCountLimit] = useState(menuFood[index].countLimit);
    const [selectedTags, setSelectedTags] = useState(menuFood[index].tags);
    const [imageUrl, setImageUrl] = useState('');
  
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

    const handleImageChange = (info) => {
        console.log('File Info:', info);
        if (info.file && info.file.status === 'done') {
            setImageUrl(info.file.response.imageUrl);
            console.log('Image URL:', info.file.response.imageUrl);
        }
    };

    useEffect(() => {
        if (userID != "") {
          fetchUser(userID, setUser);
        }
      }, [userID]);

      useEffect(() => {
        if (userID !== "" && menuFood[index]) {
            fetchUser(userID, setUser);
            form.setFieldsValue({
                dishName: menuFood[index].name || '',
                dishPrice: menuFood[index].price || 1,
                dishCountLimit: menuFood[index].countLimit || 1,
                dishDescription: menuFood[index].description || '',
                dishTags: menuFood[index].tags || [],
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
                        // dishImage: menuFood[index].imageUrl || '',
                        // dishImage: "/Users/chingtingtai/Desktop/daidai/visualstudiocode/2023_fall/CloudNative/tsmc-meal-order/web/public/images/pig.jpeg",
                    }}
                >
                    <div className={styles.formContent}>
                        <div className={styles.imageContainer}>
                            <Form.Item
                                name="dishImage"
                                valuePropName="fileList"
                                getValueFromEvent={(normFile) => normFile.fileList}
                                onChange={handleImageChange}
                            >
                                <Upload name="logo" listType="picture" beforeUpload={() => false}>
                                    <Button icon={<UploadOutlined />}>上傳餐點照片</Button>
                                </Upload>
                            </Form.Item>
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
                        </div>
                    </div>

                    <div style={{ marginTop: 'auto', textAlign: 'right' }}>
                        <Radio.Button 
                            value="default" 
                            className={styles.redButton} 
                            onClick={() => handleDeleteMenu(user, index, menuFood, setMenuFood, fetchMenuAndUpdate)}
                        >
                            刪除餐點
                        </Radio.Button>
                        
                        <Radio.Button 
                            value="default" 
                            className={styles.deepBlueButton} 
                            onClick={() => handleCreateMenu(user, index, form.getFieldsValue(), imageUrl, menuFood, setMenuFood, selectedTags, fetchMenuAndUpdate)}
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
