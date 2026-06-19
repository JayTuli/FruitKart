import { useState, useRef, useEffect } from "react";
import { useSendMessageMutation } from "../../store/api/chatBotApi";

const BOT_NAME = "FruitKart Assistant";

function ChatBot() {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([
    { role: "bot", text: "Hi! I'm your FruitKart assistant. How can I help you today?" },
  ]);
  const [input, setInput] = useState("");
  const bottomRef = useRef(null);
  const inputRef = useRef(null);

  const [sendMessage, { isLoading }] = useSendMessageMutation();

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  useEffect(() => {
    if (isOpen) inputRef.current?.focus();
  }, [isOpen]);

  const handleSend = async () => {
    const text = input.trim();
    if (!text || isLoading) return;

    setMessages((prev) => [...prev, { role: "user", text }]);
    setInput("");

    try {
      const res = await sendMessage(text).unwrap();
      const botText = res?.result?.response ?? "Sorry, I didn't get that. Please try again.";
      setMessages((prev) => [...prev, { role: "bot", text: botText }]);
    } catch {
      setMessages((prev) => [
        ...prev,
        { role: "bot", text: "Something went wrong. Please try again later." },
      ]);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <>
      {/* Floating panel */}
      {isOpen && (
        <div
          style={{
            position: "fixed",
            bottom: "80px",
            right: "24px",
            width: "340px",
            maxHeight: "480px",
            display: "flex",
            flexDirection: "column",
            background: "var(--bs-body-bg)",
            border: "1px solid var(--bs-border-color)",
            borderRadius: "16px",
            overflow: "hidden",
            zIndex: 1050,
            boxShadow: "0 8px 32px rgba(0,0,0,0.12)",
          }}
          role="dialog"
          aria-label={BOT_NAME}
        >
          {/* Header */}
          <div
            style={{
              padding: "12px 16px",
              borderBottom: "1px solid var(--bs-border-color)",
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              background: "var(--bs-secondary-bg)",
            }}
          >
            <div className="d-flex align-items-center gap-2">
              <div
                style={{
                  width: 32,
                  height: 32,
                  borderRadius: "50%",
                  background: "var(--bs-primary)",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <i className="bi bi-robot text-white" style={{ fontSize: 16 }} />
              </div>
              <div>
                <p className="mb-0 fw-500" style={{ fontSize: 14 }}>{BOT_NAME}</p>
                <p className="mb-0 text-success" style={{ fontSize: 11 }}>● Online</p>
              </div>
            </div>
            <button
              className="btn btn-sm"
              onClick={() => setIsOpen(false)}
              aria-label="Close chat"
            >
              <i className="bi bi-x-lg" />
            </button>
          </div>

          {/* Messages */}
          <div
            style={{
              flex: 1,
              overflowY: "auto",
              padding: "16px",
              display: "flex",
              flexDirection: "column",
              gap: 12,
              background: "var(--bs-tertiary-bg)",
            }}
          >
            {messages.map((msg, i) => (
              <div
                key={i}
                className={`d-flex align-items-end gap-2 ${msg.role === "user" ? "flex-row-reverse" : ""}`}
              >
                {msg.role === "bot" && (
                  <div
                    style={{
                      width: 24,
                      height: 24,
                      borderRadius: "50%",
                      background: "var(--bs-primary)",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      flexShrink: 0,
                    }}
                  >
                    <i className="bi bi-robot text-white" style={{ fontSize: 11 }} />
                  </div>
                )}
                <div
                  style={{
                    maxWidth: "75%",
                    padding: "10px 14px",
                    borderRadius: msg.role === "user" ? "12px 12px 2px 12px" : "12px 12px 12px 2px",
                    background: msg.role === "user" ? "var(--bs-primary)" : "var(--bs-body-bg)",
                    border: msg.role === "bot" ? "1px solid var(--bs-border-color)" : "none",
                    color: msg.role === "user" ? "#fff" : "var(--bs-body-color)",
                    fontSize: 13,
                    lineHeight: 1.5,
                    wordBreak: "break-word",
                  }}
                >
                  {msg.text}
                </div>
              </div>
            ))}

            {isLoading && (
              <div className="d-flex align-items-end gap-2">
                <div
                  style={{
                    width: 24, height: 24, borderRadius: "50%",
                    background: "var(--bs-primary)",
                    display: "flex", alignItems: "center", justifyContent: "center", flexShrink: 0,
                  }}
                >
                  <i className="bi bi-robot text-white" style={{ fontSize: 11 }} />
                </div>
                <div
                  style={{
                    padding: "10px 14px",
                    borderRadius: "12px 12px 12px 2px",
                    background: "var(--bs-body-bg)",
                    border: "1px solid var(--bs-border-color)",
                    fontSize: 13,
                    color: "var(--bs-secondary-color)",
                  }}
                >
                  <span className="spinner-grow spinner-grow-sm me-1" />
                  Typing...
                </div>
              </div>
            )}
            <div ref={bottomRef} />
          </div>

          {/* Input */}
          <div
            style={{
              padding: "12px 16px",
              borderTop: "1px solid var(--bs-border-color)",
              display: "flex",
              gap: 8,
              background: "var(--bs-body-bg)",
            }}
          >
            <input
              ref={inputRef}
              type="text"
              className="form-control"
              style={{ borderRadius: 20, fontSize: 13 }}
              placeholder="Type a message..."
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={handleKeyDown}
              disabled={isLoading}
            />
            <button
              className="btn btn-primary d-flex align-items-center justify-content-center"
              style={{ width: 36, height: 36, borderRadius: "50%", padding: 0, flexShrink: 0 }}
              onClick={handleSend}
              disabled={isLoading || !input.trim()}
              aria-label="Send message"
            >
              <i className="bi bi-send" style={{ fontSize: 15 }} />
            </button>
          </div>
        </div>
      )}

      {/* FAB button */}
      <button
        onClick={() => setIsOpen((prev) => !prev)}
        style={{
          position: "fixed",
          bottom: "24px",
          right: "24px",
          width: 52,
          height: 52,
          borderRadius: "50%",
          background: "var(--bs-primary)",
          border: "none",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          zIndex: 1051,
          cursor: "pointer",
          boxShadow: "0 4px 16px rgba(0,0,0,0.18)",
        }}
        aria-label={isOpen ? "Close chat" : "Open chat"}
      >
        <i
          className={`bi ${isOpen ? "bi-x-lg" : "bi-chat-dots-fill"} text-white`}
          style={{ fontSize: 22 }}
        />
      </button>
    </>
  );
}

export default ChatBot;