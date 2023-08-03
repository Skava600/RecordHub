import {
  Form,
  Button,
  FormControl,
  InputGroup,
  FormGroup,
} from "react-bootstrap";
import React, { useState } from "react";

const SendMessageForm = ({ sendMessage }) => {
  const [message, setMessage] = useState("");

  return (
    <Form
      onSubmit={(e) => {
        e.preventDefault();
        sendMessage(message);
        setMessage("");
      }}
    >
      <InputGroup>
        <FormControl
          type="text"
          placeholder="message..."
          onChange={(e) => setMessage(e.target.value)}
          value={message}
        />
        <Button variant="primary" type="submit" disabled={!message}>
          Send
        </Button>
      </InputGroup>
    </Form>
  );
};
export default SendMessageForm;
