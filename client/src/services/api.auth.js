import { instance } from "../api.config.js";

export class AuthService {
  login(username, password) {
    return instance.post("/api/Auth/login", { username, password });
  }

  info() {
    return instance.get("/api/Auth/info");
  }

  logout() {
    return instance.post("/api/logout");
  }
}
