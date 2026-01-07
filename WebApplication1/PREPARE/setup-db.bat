@echo off
echo ========================================
echo Настройка базы данных PostgreSQL
echo ========================================

REM Поиск psql.exe в стандартных путях установки PostgreSQL
set FOUND_PSQL=
set PSQL_VERSIONS=18 17 16 15 14 13 12 11 10

echo Поиск установленной версии PostgreSQL...
for %%v in (%PSQL_VERSIONS%) do (
    if exist "C:\Program Files\PostgreSQL\%%v\bin\psql.exe" (
        set PSQL_PATH="C:\Program Files\PostgreSQL\%%v\bin\psql.exe"
        set PG_VERSION=%%v
        set FOUND_PSQL=1
        echo Найдена PostgreSQL версии %%v
        goto :found_psql
    )
)

if not defined FOUND_PSQL (
    echo Ошибка: PostgreSQL не найден!
    echo Проверенные пути:
    for %%v in (%PSQL_VERSIONS%) do (
        echo   C:\Program Files\PostgreSQL\%%v\bin\psql.exe
    )
    echo.
    echo Установите PostgreSQL или укажите правильный путь вручную
    pause
    exit /b 1
)

:found_psql
echo Используется PostgreSQL версии %PG_VERSION%
echo Путь к psql: %PSQL_PATH%

set PGHOST=localhost
set PGPORT=5432
set PGDATABASE=TestBase
set PGUSER=postgres
set PGPASSWORD=1

echo Проверка соединения с сервером PostgreSQL...
%PSQL_PATH% -h %PGHOST% -p %PGPORT% -U %PGUSER% -d postgres -c "SELECT version();"
if %errorlevel% neq 0 (
    echo Ошибка: Не удалось подключиться к серверу PostgreSQL!
    echo Проверьте:
    echo 1. Запущен ли сервер PostgreSQL
    echo 2. Правильность логина/пароля: %PGUSER% / %PGPASSWORD%
    echo 3. Настройки host/port: %PGHOST%:%PGPORT%
    echo.
    echo Пробую подключиться к 127.0.0.1...
    set PGHOST=127.0.0.1
    %PSQL_PATH% -h %PGHOST% -p %PGPORT% -U %PGUSER% -d postgres -c "SELECT 1;"
    if %errorlevel% neq 0 (
        pause
        exit /b 1
    )
    echo Успешно подключился по адресу 127.0.0.1
)

echo Создание базы данных %PGDATABASE%...
%PSQL_PATH% -h %PGHOST% -p %PGPORT% -U %PGUSER% -d postgres -c "CREATE DATABASE \"%PGDATABASE%\";"
if %errorlevel% neq 0 (
    echo Предупреждение: База данных уже существует или возникла ошибка при создании.
    echo Продолжаем выполнение...
)

if not exist init-db.sql (
    echo Ошибка: Файл init-db.sql не найден!
    echo Убедитесь, что файл находится в той же папке, что и этот скрипт
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

if not exist seed-data.sql (
    echo Предупреждение: Файл seed-data.sql не найден!
    echo Продолжаем без заполнения тестовыми данными...
    goto :skip_seed
)

echo Заполнение тестовыми данными...
%PSQL_PATH% -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %PGDATABASE% -f seed-data.sql
if %errorlevel% neq 0 (
    echo Ошибка при заполнении данными!
    pause
    exit /b 1
)

:skip_seed
echo.
echo ========================================
echo База данных успешно настроена!
echo Версия PostgreSQL: %PG_VERSION%
echo Хост: %PGHOST%:%PGPORT%
echo База данных: %PGDATABASE%
echo ========================================

pause