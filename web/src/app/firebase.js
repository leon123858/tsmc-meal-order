import { initializeApp } from 'firebase/app';
import { getAuth } from "firebase/auth";

const firebaseConfig = {
  apiKey: "AIzaSyBYy4ft2l9mUWc-XoisHW8OufDdgbRwSxE",
  authDomain: "tw-rd-ca-leon-lin.firebaseapp.com",
  projectId: "tw-rd-ca-leon-lin",
  storageBucket: "tw-rd-ca-leon-lin.appspot.com",
  messagingSenderId: "77786086397",
  appId: "1:77786086397:web:1dee69346fa2c141917794"
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);

export { auth };