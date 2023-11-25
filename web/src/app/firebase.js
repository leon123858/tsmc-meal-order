import { initializeApp } from 'firebase/app';
import { getAuth } from "firebase/auth";

const firebaseConfig = {
  apiKey: "AIzaSyDyHcUzx16B0YeOL_XGbsmA8xuW2CqGILE",
  authDomain: "tsmc-meal-order.firebaseapp.com",
  projectId: "tsmc-meal-order",
  storageBucket: "tsmc-meal-order.appspot.com",
  messagingSenderId: "126195515825",
  appId: "1:126195515825:web:f16ca7d2c82a801dc03d90",
  measurementId: "G-2N6SCNKG3W"
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);

export { auth };