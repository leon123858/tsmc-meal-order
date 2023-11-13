const AuthServerAPI = 'https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=';

const userData = {
    email: 'asd111@asdasd.ccy',
    password: 'string',
}

async function loginOutSide() {
    const rawResponse = await fetch(AuthServerAPI + "xxx", {
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