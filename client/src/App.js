import { useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import Lobby from "./components/Lobby";
import Chat from "./components/Chat";
import Signin from "./components/SignIn";
import Profile from "./components/Profile";
import React from "react";
import "./App.css";
import "bootstrap/dist/css/bootstrap.min.css";
import { Route, Routes, BrowserRouter } from "react-router-dom";
import { observer } from "mobx-react-lite";
import AuthStore from "./store.js";
import PrivateRoute from "./privateRoute.jsx";
const App = () => {
  const [connection, setConnection] = useState();
  const [messages, setMessages] = useState([]);
  const [users, setUsers] = useState([]);

  const joinRoom = async (user, room) => {
    try {
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:5009/chat")
        .configureLogging(LogLevel.Information)
        .build();

      connection.on("ReceiveMessage", (user, message, date) => {
        setMessages((messages) => [...messages, { user, message, date }]);
      });

      connection.on("UsersInRoom", (users) => {
        setUsers(users);
      });

      connection.onclose((e) => {
        setConnection();
        setMessages([]);
        setUsers([]);
      });

      await connection.start();
      await connection.invoke("JoinRoom", { user, room });
      setConnection(connection);
    } catch (e) {
      console.log(e);
    }
  };

  const sendMessage = async (message) => {
    try {
      if (connection == null) {
        var roomId = localStorage.getItem("roomId");
        if (roomId == null) {
          roomId = crypto.randomUUID();
          localStorage.setItem("roomId", roomId);
        }

        await joinRoom("client", roomId);
      }
      await connection.invoke("SendMessage", message);
    } catch (e) {
      console.log(e);
    }
  };

  const closeConnection = async () => {
    try {
      await connection.stop();
    } catch (e) {
      console.log(e);
    }
  };
  useEffect(() => {
    AuthStore.checkAuth();
  }, []);

  return (
    <BrowserRouter>
      <Routes>
        //страница, для посещения которой авторизация не требуется
        <Route path="/login" element={<Signin />} />
        //страницы, для посещения которых требуется авторизация
        <Route path="/profile" element={<PrivateRoute />}>
          <Route path="" element={<Profile />} />
        </Route>
        <Route path="/lobby" element={<PrivateRoute />}>
          <Route
            path=""
            element={
              !connection ? (
                <Lobby joinRoom={joinRoom} />
              ) : (
                <Chat
                  sendMessage={sendMessage}
                  messages={messages}
                  users={users}
                  closeConnection={closeConnection}
                />
              )
            }
          ></Route>
        </Route>
        <Route
          path="/chat"
          element={
            <Chat
              sendMessage={sendMessage}
              messages={messages}
              users={users}
              closeConnection={closeConnection}
            />
          }
        />
        <Route path="*" element={<div>404... not found </div>} />
      </Routes>
    </BrowserRouter>
  );
};

export default App;
