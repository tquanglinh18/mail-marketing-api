/**
 * @desc    Middleware xử lý lỗi 404 (Không tìm thấy trang)
 * @param   {Object} req - Đối tượng Request
 * @param   {Object} res - Đối tượng Response
 * @param   {Function} next - Hàm next middleware
 */
const notFound = (req, res, next) => {
    const error = new Error(`Không tìm thấy - ${req.originalUrl}`);
    res.status(404);
    next(error); // Chuyển lỗi đến middleware xử lý lỗi tiếp theo
};

/**
 * @desc    Middleware xử lý lỗi chung
 * @param   {Object} err - Đối tượng lỗi
 * @param   {Object} req - Đối tượng Request
 * @param   {Object} res - Đối tượng Response
 * @param   {Function} next - Hàm next middleware
 */
const errorHandler = (err, req, res, next) => {
    // Đặt mã trạng thái (status code) là 500 nếu chưa có, hoặc giữ nguyên mã lỗi hiện có
    const statusCode = res.statusCode === 200 ? 500 : res.statusCode;
    res.status(statusCode);
    res.json({
        message: err.message,
        // Chỉ hiển thị stack trace trong môi trường phát triển (development)
        stack: process.env.NODE_ENV === 'production' ? null : err.stack,
    });
};

module.exports = {
    notFound,
    errorHandler
};
