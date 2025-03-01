const express = require("express");
const cors = require("cors");
const sqlite3 = require("sqlite3").verbose();
const { open } = require("sqlite");
const http = require("http");
const WebSocket = require("ws");
const { v4: uuidv4 } = require("uuid");

const app = express();
const server = http.createServer(app);
const wss = new WebSocket.Server({ server, path: "/ws" });

app.use(cors());
app.use(express.json());

// 📌 SQLite データベース接続設定
const dbPromise = open({
  filename: "./database.sqlite", // SQLite データベースファイルのパス
  driver: sqlite3.Database,
});

// 📌 データベースの初期化
async function initializeDatabase() {
  const db = await dbPromise;
  await db.exec(`
    CREATE TABLE IF NOT EXISTS users (
      id TEXT PRIMARY KEY,
      name TEXT
    );
  `);
  console.log("✅ SQLite データベースが準備されました");
}

initializeDatabase();

// 📌 WebSocket 接続中のクライアント管理（ユーザーごと）
const clients = new Map();

// 📌 WebSocket サーバー設定
wss.on("connection", async (ws, req) => {
  const userId = req.url.includes("userId=") ? req.url.split("userId=")[1] : uuidv4();
  clients.set(userId, ws);
  console.log(`✅ ユーザー [${userId}] が接続しました`);

  ws.on("message", async (message) => {
    console.log(`📩 [${userId}] のメッセージ: ${message}`);

    try {
      const parsedMessage = JSON.parse(message);

      if (parsedMessage.to && clients.has(parsedMessage.to)) {
        // 🎯 特定のユーザーへメッセージ送信
        console.log(`📤 [${parsedMessage.to}] に送信: ${parsedMessage.content}`);
        clients.get(parsedMessage.to).send(`🔔 [${userId}]: ${parsedMessage.content}`);
      } else {
        // 📢 全クライアントにブロードキャスト
        clients.forEach((client, id) => {
          if (id !== userId && client.readyState === WebSocket.OPEN) {
            try {
              client.send(`📢 [${userId}]: ${parsedMessage.content}`);
            } catch (error) {
              console.error(`🚨 メッセージ送信エラー (${id}):`, error);
            }
          }
        });
      }

      // 📌 メッセージをデータベースに保存（例: ユーザー情報を管理）
      const db = await dbPromise;
      await db.run("INSERT INTO users (id, name) VALUES (?, ?) ON CONFLICT(id) DO NOTHING", [userId, `User-${userId.substring(0, 5)}`]);

    } catch (error) {
      console.error("🚨 JSON パースエラー:", error);
      ws.send("⚠️ 無効なメッセージ形式です。");
    }
  });

  ws.on("close", () => {
    console.log(`❌ ユーザー [${userId}] が切断しました`);
    ws.terminate(); // 明示的にリソース解放
    clients.delete(userId);
  });

  ws.on("error", (error) => {
    console.error("🚨 WebSocket エラー:", error);
  });
});

// 📌 サーバー起動
const PORT = process.env.WS_PORT || 4000;
server.listen(PORT, () => {
  console.log(`🚀 サーバーがポート ${PORT} で起動しました (WebSocket & API)`);
});
