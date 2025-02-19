const express = require("express");
const cors = require("cors");
const { Pool } = require("pg");
const http = require("http");
const WebSocket = require("ws");
const { v4: uuidv4 } = require("uuid"); // ✅ `uuid` の正しいインポート

const app = express();
const server = http.createServer(app);
const wss = new WebSocket.Server({ server, path: "/ws" });

app.use(cors());
app.use(express.json());

const pool = new Pool({
  user: "masashitakao",
  host: "localhost",
  database: "mydatabase",
  password: "",
  port: 5432,
});

// 📌 接続中のクライアントを管理（ユーザーごと）
const clients = new Map();

// 📌 WebSocket サーバー設定
wss.on("connection", (ws, req) => {
  const userId = req.url.split("userId=")[1] || uuidv4(); // ✅ ユーザー ID の取得 or 新規発行
  clients.set(userId, ws);
  console.log(`✅ ユーザー [${userId}] が接続しました`);

  ws.on("message", (message) => {
    console.log(`📩 [${userId}] のメッセージ: ${message}`);

    const parsedMessage = JSON.parse(message);
    if (parsedMessage.to && clients.has(parsedMessage.to)) {
      // 🎯 特定のユーザーへメッセージ送信
      console.log(`📤 [${parsedMessage.to}] に送信: ${parsedMessage.content}`);
      clients.get(parsedMessage.to).send(`🔔 [${userId}]: ${parsedMessage.content}`);
    } else {
      // 📢 全クライアントにブロードキャスト
      clients.forEach((client, id) => {
        if (id !== userId && client.readyState === WebSocket.OPEN) {
          client.send(`📢 [${userId}]: ${parsedMessage.content}`);
        }
      });
    }
  });

  ws.on("close", () => {
    console.log(`❌ ユーザー [${userId}] が切断しました`);
    clients.delete(userId);
  });

  ws.on("error", (error) => {
    console.error("🚨 WebSocket エラー:", error);
  });
});

// 📌 サーバー起動
server.listen(4000, () => {
  console.log("🚀 サーバーがポート 4000 で起動しました (WebSocket & API)");
});
