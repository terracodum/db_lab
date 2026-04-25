# db_lab

Лабораторные работы по базам данных. Вариант **11б — «Автостоянка»**.

## Схема БД

| Таблица | Поля |
|---|---|
| `Cars` | `License_Plate` (PK), `Car_Type`, `Client_ID` |
| `Parking_Access` | `License_Plate` + `Work_Date` (PK), `Access_Permit` |
| `Payments` | `Entry_ID` (PK), `License_Plate`, `Work_Date`, `Entry_Time`, `Entry_Type` |

## Лабораторные работы

### Лаб 1–2 — MS Access
Исходные базы данных в `lab1-2/old/` (.mdb) и `lab1-2/new/` (.accdb).

### Лаб 3 — Запросы (SQLite)
SELECT, UNION, PIVOT, временные таблицы, параметризованные запросы.

**Стек:** Python + SQLite (`sqlite3`, `pandas`)
```bash
jupyter notebook lab3/task.ipynb
```

### Лаб 4 — Формы для работы с БД (C# + Avalonia)
Desktop-приложение с master-detail интерфейсом: таблица автомобилей + связанные платежи. CRUD через диалоговые окна.

**Стек:** C# + Avalonia UI + SQLite
```bash
dotnet run --project lab4/ParkingApp
```

### Лаб 5 — Функции в СУБД (PostgreSQL)
Скалярные и табличные функции, функции с проверкой существования записи.

**Стек:** Python + PostgreSQL (`psycopg2`, `pandas`)
```bash
brew services start postgresql@17
jupyter notebook lab5/lab5.ipynb
```
