-- Проверка всех данных
SELECT 'Пользователи' as table_name;
SELECT * FROM users ORDER BY id;

SELECT 'Заказы' as table_name;
SELECT * FROM orders ORDER BY user_id, id;

SELECT 'Сообщения' as table_name;
SELECT * FROM messages ORDER BY touserid, id;

-- Проверка бизнес-логики VIP статуса
SELECT 'Проверка VIP статуса' as check_name;
SELECT 
    u.id,
    u.name,
    u.is_vip as current_vip_status,
    COUNT(o.id) as total_orders,
    COUNT(CASE WHEN o.total > 1000 THEN 1 END) as large_orders_count,
    CASE 
        WHEN COUNT(CASE WHEN o.total > 1000 THEN 1 END) > 0 
        THEN 'ДОЛЖЕН БЫТЬ VIP' 
        ELSE 'НЕ ДОЛЖЕН БЫТЬ VIP' 
    END as expected_vip_status
FROM users u
LEFT JOIN orders o ON u.id = o.user_id
GROUP BY u.id, u.name, u.is_vip
ORDER BY u.id;