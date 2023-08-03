import { instance } from "../api.config.js";

export class ChatService {
  getRooms() {
    return instance.get("/api/chat");
  }
}
