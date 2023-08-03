import React, { useState, useEffect } from "react";
import { Form, Button, ListGroup } from "react-bootstrap";
import { ChatService } from "../services/api.chat";
import { redirect } from "react-router";
const Lobby = ({ joinRoom }) => {
  const [user, setUser] = useState();
  const [rooms, setRooms] = useState([]);
  useEffect(() => {
    async function fetchRooms() {
      const chatService = new ChatService();
      const resp = await chatService.getRooms();
      console.log(resp);
      setRooms(resp.data); // Assuming `resp` is an array of room objects
    }

    fetchRooms();
  }, []);

  const handleJoinButtonClick = (roomId) => {
    joinRoom(user, roomId);
  };
  return (
    <Form className="lobby">
      <Form.Group>
        <Form.Control
          placeholder="name"
          onChange={(e) => setUser(e.target.value)}
        />
      </Form.Group>
      <div>
        {Object.entries(rooms).map(([roomId, roomData]) => (
          <div key={roomId}>
            <ListGroup variant="flush">
              <ListGroup.Item>
                <strong>Room:</strong> {roomData.room}
              </ListGroup.Item>
              <ListGroup.Item>
                <strong>Users:</strong>{" "}
                {Object.values(roomData).map((user) => (
                  <span key={user}>{user}</span>
                ))}
              </ListGroup.Item>
              <ListGroup.Item>
                <Button
                  variant="primary"
                  onClick={() => handleJoinButtonClick(roomData.room)}
                  disabled={!user}
                >
                  Join Room
                </Button>
              </ListGroup.Item>
            </ListGroup>
          </div>
        ))}
      </div>
    </Form>
  );
};

export default Lobby;
