import axios from "axios";
import React, { useState, useEffect } from "react";
import AuthStore from "../store.js";

const Profile = () => {
  const [message, setMessage] = useState("Couldn't access endpoint");
  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await AuthStore.info();
        console.log(res.data);
        setMessage(res.data.email);
      } catch (err) {
        if (!err.response) {
          setMessage("no server response");
        } else {
          setMessage("token not found or invalid");
        }
      }
    };
    fetchData();
  }, []);

  return (
    <div>
      <h1>Protected Route</h1>
      <p>{message}</p>
    </div>
  );
};

export default Profile;
