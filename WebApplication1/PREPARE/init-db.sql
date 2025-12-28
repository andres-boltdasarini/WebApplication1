-- Создание таблицы users
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    is_vip BOOLEAN DEFAULT FALSE
);

-- Создание таблицы orders
CREATE TABLE IF NOT EXISTS orders (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    total DECIMAL(10, 2) NOT NULL CHECK (total >= 0)
);

-- Создание таблицы messages
CREATE TABLE IF NOT EXISTS messages (
    id SERIAL PRIMARY KEY,
    touserid INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    content TEXT NOT NULL,
    isread BOOLEAN DEFAULT FALSE
);

-- Создание индексов для улучшения производительности
CREATE INDEX IF NOT EXISTS idx_orders_user_id ON orders(user_id);
CREATE INDEX IF NOT EXISTS idx_messages_touserid ON messages(touserid);
CREATE INDEX IF NOT EXISTS idx_messages_isread ON messages(isread);