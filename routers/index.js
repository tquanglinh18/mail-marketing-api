const express = require('express');
const router = express.Router();

/**
 * @route   GET /
 * @desc    Route cơ bản để kiểm tra server
 * @access  Public
 */
router.get('/', (req, res) => {
    res.send('Chào mừng đến với ứng dụng Node.js của bạn! (Đã tách route)');
});

module.exports = router;
