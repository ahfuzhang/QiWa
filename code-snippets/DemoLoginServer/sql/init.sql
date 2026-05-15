-- 创建数据库
-- 提示词意图：定义用户数据库和用户表，包含 user_id, user_name, user_password_sha 字段
CREATE DATABASE IF NOT EXISTS login_server CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE login_server;

-- 用户表
CREATE TABLE IF NOT EXISTS users (
    user_id      BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '用户唯一 ID',
    user_name    VARCHAR(64)  NOT NULL UNIQUE COMMENT '用户名，唯一',
    user_password_sha CHAR(64) NOT NULL COMMENT '用户密码的 SHA256 哈希值',
    created_at   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    updated_at   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    INDEX idx_user_name (user_name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户信息表';

-- 插入测试数据（密码 "test123" 的 SHA256：ecd71870d1963316a97e3ac3408c9835ad8cf0f3c1bc703527c30265534f75ae）
INSERT IGNORE INTO users (user_name, user_password_sha) VALUES
  ('testuser', 'ecd71870d1963316a97e3ac3408c9835ad8cf0f3c1bc703527c30265534f75ae');
