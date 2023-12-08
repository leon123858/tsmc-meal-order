import React from 'react';
import { ConfigProvider, Button, Modal, Switch } from 'antd';
import styles from './AIwindow.module.css';

const AIwindow = ({curAIWindowState, setAIWindowState}) => {
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
                    // <Button 
                    // key="back" 
                    // onClick={() => setAIWindowState(false)}
                    // >
                    // 取消
                    // </Button>,
                    // <Button 
                    //     key="submit" 
                    //     type="primary" 
                    //     onClick={() => setAIWindowState(false)}
                    // >
                    //     送出
                    // </Button>,
                    <div key={"recommend"} className={styles.recommend_div}>                
                        <Button 
                            // key="recommend" 
                            className={styles.recommend_button}
                            onClick={() => setAIWindowState(false)}
                        >
                            推薦
                        </Button>
                    </div>
                ]}
            >
                <div>
                    <b
                        face="monospace" 
                        size="6"
                    >
                        篩選條件<br />
                    </b>
                    <Switch defaultChecked/>&nbsp;&nbsp;<Switch defaultChecked></Switch>&nbsp;&nbsp;<Switch defaultChecked></Switch>
                </div>
                <div>
                    <b
                        face="monospace" 
                        size="6"
                    >
                        推薦天數<br />
                    </b>
                    <Switch defaultChecked></Switch>&nbsp;&nbsp;<Switch defaultChecked></Switch>
                </div>
                <div>
                    <b
                        face="monospace" 
                        size="4"
                    >
                        價格上限<br />
                    </b>
                    <Switch defaultChecked></Switch>&nbsp;&nbsp;<Switch defaultChecked></Switch>&nbsp;&nbsp;<Switch defaultChecked></Switch>&nbsp;&nbsp;<Switch defaultChecked></Switch>
                </div>
            </Modal>
        </ConfigProvider>
    </>
  );
};
export default AIwindow;