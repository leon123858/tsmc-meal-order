// 這裡填入你的 Cloud Run 服務的 URL
// const cloudRunUrl = 'http://127.0.0.1:5122/api/mail/event/fail-mail-event';
const cloudRunUrl = 'http://127.0.0.1:5122/api/mail/event/send-mail-event';
const utf8Encode = new TextEncoder();

const test = {
    to: "a0970785699@gmail.com",
    subject: "test-tsmc-mail",
    mailId: "da83d9d7-e3b8-4622-b487-dfd360fc6d17",
    content: "final test",
}
const pubSubMessage = {
    message: {
        // 假設你要發送的 data 是一個 base64 編碼的字串
        // convert to number[] by utf8Encode.encode(JSON.stringify(test))
        data: uint8ArrayToArray(utf8Encode.encode(JSON.stringify(test))),
        id: '消息ID',
    },
    subscription: 'submission-name-訂閱名稱',
};
// console.log(pubSubMessage.message.data);
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