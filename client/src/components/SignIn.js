import React, { useState, useContext } from "react";
import { Form, Row, Col, Container, Button } from "react-bootstrap";
import axios from "axios";
import AuthStore from "../store.js";

const SignIn = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const API = axios.create({
    baseURL: "https://localhost:5001",
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await AuthStore.login(username, password);
    } catch (err) {
      if (err.response === null || err.response === undefined) {
        setError("no server response");
      } else {
        setError("registeration failed");
      }
    }
  };

  return (
    <Container>
      <Row className="justify-content-center">
        <Col md={6}>
          <h2>Sign In</h2>
          <Form onSubmit={handleSubmit}>
            <Form.Group controlId="email">
              <Form.Label>Name:</Form.Label>
              <Form.Control
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </Form.Group>
            <Form.Group controlId="password">
              <Form.Label>Password:</Form.Label>
              <Form.Control
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </Form.Group>
            <Button variant="primary" type="submit">
              Sign In
            </Button>
            {error && <p>{error}</p>}
          </Form>
        </Col>
      </Row>
    </Container>
  );
};

export default SignIn;
