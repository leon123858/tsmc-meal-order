// 這裡填入你的 Cloud Run 服務的 URL
const cloudRunUrl = 'http://127.0.0.1:8080/api/user/sync';
const utf8Encode = new TextEncoder();

const test = {
    type : 'user-create',
    data : {
        uid : 'knYD26ptPYVkXcREZXlKoZVMl9HQ',
        email : 'test@test.test',
        userType : 'admin',
    }
}
const pubSubMessage = {
    message: {
        // 假設你要發送的 data 是一個 base64 編碼的字串
        // convert to number[] by utf8Encode.encode(JSON.stringify(test))
        data: uint8ArrayToArray(utf8Encode.encode(JSON.stringify(test))),
        id: '消息ID',
    },
    subscription: 'user-create-event',
};
const options = {
    method: 'POST',
    body: JSON.stringify(pubSubMessage),
    headers: {
        'Content-Type': 'application/json',
        // 如果需要身份驗證，請提供合適的 Bearer token
        // 'Authorization': 'Bearer YOUR_ACCESS_TOKEN',
    },
};

(async () => {
    console.log('發送 Pub/Sub 訊息給 Cloud Run 服務');
    const response = await fetch(cloudRunUrl, options);
    console.log(response.status);
    const json = await response.text();
    console.log(json);
} )();

function uint8ArrayToArray(uint8Array) {
    const array = [];

    for (let i = 0; i < uint8Array.byteLength; i++) {
        array[i] = uint8Array[i];
    }

    return array;
}