import React, { useEffect, useRef } from "react";
const MessageContainer = ({ messages }) => {
  const messageRef = useRef();
  const userTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
  const options = {
    month: "2-digit",
    day: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    timeZone: userTimeZone,
  };

  useEffect(() => {
    if (messageRef && messageRef.current) {
      const { scrollHeight, clientHeight } = messageRef.current;
      messageRef.current.scrollTo({
        left: 0,
        top: scrollHeight - clientHeight,
        behavior: "smooth",
      });
    }
  }, [messages]);

  return (
    <div ref={messageRef} className="message-container">
      {messages.map((m, index) => (
        <div key={index} className="user-message">
          <div className="from-user">
            {new Intl.DateTimeFormat(undefined, options).format(
              new Date(m.date)
            )}
          </div>
          <div className="message bg-primary">{m.message}</div>
          <div className="from-user">{m.user}</div>
        </div>
      ))}
    </div>
  );
};

export default MessageContainer;
