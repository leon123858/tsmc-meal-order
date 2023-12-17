// 這裡填入你的 Cloud Run 服務的 URL
// const cloudRunUrl = 'http://127.0.0.1:5122/api/mail/event/fail-mail-event';
const cloudRunUrl = 'http://127.0.0.1:5122/api/mail/event/send-mail-event';
const utf8Encode = new TextEncoder();

const test = {
    to: "a0970785699@gmail.com",
    subject: "test-tsmc-mail",
    mailId: "16dbec2a-9382-4c09-89cb-001b2d173710",
    content: "final test",
}
const objJsonStr = JSON.stringify(test);
const objJsonB64 = Buffer.from(objJsonStr).toString("base64");
const pubSubMessage = {
    message: {
        // 假設你要發送的 data 是一個 base64 編碼的字串
        // convert to number[] by utf8Encode.encode(JSON.stringify(test))
        messageId: '消息 ID',
        publishTime: '2020-07-22T02:26:26.000Z',
        data: objJsonB64,
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
})();

function uint8ArrayToArray(uint8Array) {
    const array = [];

    for (let i = 0; i < uint8Array.byteLength; i++) {
        array[i] = uint8Array[i];
    }

    return array;
}