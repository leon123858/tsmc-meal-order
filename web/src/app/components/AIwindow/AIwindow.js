import React from 'react';
import { ConfigProvider, Button, Modal, Input } from 'antd';
import styles from './AIwindow.module.css';

const AIwindow = ({curAIWindowState, setAIWindowState}) => {
    const [text, setText] = React.useState("");

    const handleTextChange = (e, setText) => {
        setText(e.target.value);
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