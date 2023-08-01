import { Navigate, Outlet, Route } from "react-router-dom";
import AuthStore from "./store.js";
import { observer } from "mobx-react-lite";
import React from "react";
const PrivateRoute = (props) => {
  if (AuthStore.isLoadingAuth) {
    return <div>Checking auth...</div>;
  }
  if (AuthStore.isAuth) {
    return <Outlet />;
  } else return <div>Login to see resources</div>;
};

export default observer(PrivateRoute);
