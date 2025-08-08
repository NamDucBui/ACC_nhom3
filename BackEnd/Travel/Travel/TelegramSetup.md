# Hướng dẫn setup Telegram Bot cho thông báo Crawl

## Bước 1: Tạo Telegram Bot

1. Mở Telegram và tìm `@BotFather`
2. Gửi lệnh `/newbot`
3. Đặt tên cho bot (ví dụ: "Crawl Notification Bot")
4. Đặt username cho bot (phải kết thúc bằng "bot", ví dụ: "my_crawl_bot")
5. BotFather sẽ trả về Bot Token, hãy lưu lại token này

## Bước 2: Lấy Chat ID

### Cách 1: Gửi tin nhắn cho bot
1. Tìm bot bạn vừa tạo và gửi tin nhắn bất kỳ
2. Truy cập URL: `https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getUpdates`
3. Tìm `chat_id` trong response JSON

### Cách 2: Sử dụng @userinfobot
1. Tìm `@userinfobot` trên Telegram
2. Gửi tin nhắn bất kỳ cho bot này
3. Bot sẽ trả về thông tin của bạn, bao gồm Chat ID

## Bước 3: Cấu hình trong appsettings.json

Thay thế các giá trị trong file `appsettings.json`:

```json
{
  "Telegram": {
    "BotToken": "YOUR_ACTUAL_BOT_TOKEN",
    "ChatId": "YOUR_ACTUAL_CHAT_ID"
  }
}
```

## Bước 4: Test thông báo

Sau khi cấu hình xong, khi bạn chạy crawl, bot sẽ gửi thông báo:

### Thông báo bắt đầu crawl:
```
🚀 Bắt đầu Crawl

📋 Cấu hình: [Tên cấu hình]
🔗 URL: [URL nguồn]
⏰ Thời gian: [Thời gian bắt đầu]
```

### Thông báo hoàn thành crawl:
```
✅ Crawl Thành công

📋 Cấu hình: [Tên cấu hình]
📊 Kết quả:
   • Tìm thấy: [Số tour tìm thấy]
   • Lưu: [Số tour lưu]
   • Bỏ qua: [Số tour bỏ qua]
   • Trang crawl: [Số trang crawl]
⏱️ Thời gian: [Thời gian thực hiện] phút
⏰ Hoàn thành: [Thời gian hoàn thành]
```

### Thông báo lỗi:
```
❌ Crawl Thất bại

📋 Cấu hình: [Tên cấu hình]
❌ Lỗi: [Chi tiết lỗi]
```

## Lưu ý bảo mật

- Không commit Bot Token vào git repository
- Sử dụng User Secrets hoặc Environment Variables cho production
- Chỉ chia sẻ Bot Token với những người cần thiết 