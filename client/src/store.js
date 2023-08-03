import { makeAutoObservable } from "mobx";
import { AuthService } from "./services/api.auth.js";

class AuthStore {
  isAuth = false;
  isAuthInProgress = false;
  authService = new AuthService();
  constructor() {
    makeAutoObservable(this, {}, { autoBind: true });
  }

  async login(username, password) {
    this.isAuthInProgress = true;

    try {
      const resp = await this.authService.login(username, password);
      console.log(resp);
      localStorage.setItem("token", resp.data);
      this.isAuth = true;
    } catch (err) {
      console.log(err);
    } finally {
      this.isAuthInProgress = false;
    }
  }
  async info() {
    if (this.isAuth) {
      const resp = await this.authService.info();
      return resp;
    }

    return null;
  }
  async checkAuth() {
    this.isAuthInProgress = true;
    try {
      const token = localStorage.getItem("token");
      if (token != null) this.isAuth = true;
    } catch (err) {
      console.log("login error");
    } finally {
      this.isAuthInProgress = false;
    }
  }

  async logout() {
    this.isAuthInProgress = true;
    try {
      this.isAuth = false;
      localStorage.removeItem("token");
    } catch (err) {
      console.log("logout error");
    } finally {
      this.isAuthInProgress = false;
    }
  }
}
export default new AuthStore();
