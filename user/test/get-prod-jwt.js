const AuthServerAPI = 'https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=';

const userData = {
    email: 'test123@tsmc.bunny.com',
    password: '123456',
}

async function loginOutSide() {
    const rawResponse = await fetch(AuthServerAPI + "token", {
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

(async () => {
    const {token, uid} = await loginOutSide();
    console.log(token, uid);
})();