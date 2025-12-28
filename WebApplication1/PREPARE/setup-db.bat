@echo off
echo ========================================
echo Настройка базы данных PostgreSQL
echo ========================================

REM Укажите правильный путь к psql.exe
set PSQL_PATH="C:\Program Files\PostgreSQL\17\bin\psql.exe"
set PGHOST=localhost
set PGPORT=5432
set PGDATABASE=TestBase
set PGUSER=postgres
set PGPASSWORD=1

echo Проверка наличия psql...
if not exist %PSQL_PATH% (
    echo Ошибка: psql не найден по пути: %PSQL_PATH%
    echo Укажите правильный путь в переменной PSQL_PATH
    pause
    exit /b 1
)

echo Создание таблиц...
%PSQL_PATH% -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %PGDATABASE% -f init-db.sql

if %errorlevel% neq 0 (
    echo Ошибка при создании таблиц!
    pause
    exit /b 1
)

echo Заполнение тестовыми данными...
%PSQL_PATH% -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %PGDATABASE% -f seed-data.sql

if %errorlevel% neq 0 (
    echo Ошибка при заполнении данными!
    pause
    exit /b 1
)

echo ========================================
echo База данных успешно настроена!
echo ========================================

pause