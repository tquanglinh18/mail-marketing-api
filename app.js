require('dotenv').config(); // Load biến môi trường từ .env
const express = require('express');

// Import các routes và middleware đã tách
const indexRouter = require('./routers/index'); // Giả định bạn đã có index.js
const connectDB = require('./config/db'); // Kết nối đến MongoDB
const { notFound, errorHandler } = require('./middleware/errorMiddleware');

const app = express();
const PORT = process.env.PORT || 3000;

connectDB(); // Kết nối đến MongoDB Atlas

app.use(express.json()); // Cho phép ứng dụng đọc JSON từ body của request
app.use(express.urlencoded({ extended: true })); // Cho phép ứng dụng đọc dữ liệu từ form HTML

// Sử dụng các Routes
// Mọi request đến '/' sẽ được xử lý bởi indexRouter
app.use('/', indexRouter);

app.use(notFound);
app.use(errorHandler);

// Khởi động server Express
app.listen(PORT, () => {
    console.log(`Server đang chạy trên cổng ${PORT}`);
});