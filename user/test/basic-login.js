// create a admin user

const baseUrl = 'http://127.0.0.1:8080/api/user';
const testAuthServerAPI = 'http://127.0.0.1:9099/identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=testkey';

const userData = {
    email: 'test1@ggcc.bb',
    name: 'test123',
    password: 'string',
    type: 'admin'
}

async function loginOutSide() {
    const rawResponse = await fetch(testAuthServerAPI, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            email: userData.email,
            password: userData.password,
            returnSecureToken: true
        })
    });
    const content = await rawResponse.json();
    console.log(content);
    return {
        token: content.idToken,
        uid: content.localId
    };
}

async function login(uid, token) {
    const rawResponse = await fetch(`${baseUrl}/login`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
            email: userData.email,
            uid: uid,
        })
    });
    const content = await rawResponse.json();
    console.log(content);
    return content.data;
}

async function create() {
    const rawResponse = await fetch(`${baseUrl}/create`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            email: userData.email,
            password: userData.password,
            name: userData.name,
            type: userData.type
        })
    });
    const content = await rawResponse.json();
    console.log(content);
    return content.data;
}

(async () => {
    await create();
    const {token, uid} = await loginOutSide();
    const loginResult = await login(uid, token);
    console.log(loginResult);
})();