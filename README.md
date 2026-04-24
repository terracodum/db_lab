# db_lab

Лабораторные работы по базам данных. Вариант **11б — «Автостоянка»**.

## Структура

```
db_lab/
└── lab1-2/
|   ├── old/        # Исходные БД в формате .mdb (MS Access 97–2003)
|   ├── new/     # Исходные БД в формате .accdb (MS Access 2007+)
└── lab3/
    ├── task.txt        # Задание
    ├── task.ipynb      # Решение (Jupyter Notebook)
    └── parking.db      # SQLite база данных
```

## Лабораторная работа 3

**Тема:** Разработка запросов — SELECT, UNION, PIVOT, временные таблицы, параметризованные запросы.

**Стек:** Python + SQLite (`sqlite3`, `pandas`)

**Запуск:**
```bash
jupyter notebook lab3/task.ipynb
```

## Схема БД

| Таблица | Поля |
|---|---|
| `Cars` | `License_Plate` (PK), `Car_Type`, `Client_ID` |
| `Parking_Access` | `License_Plate` + `Work_Date` (PK), `Access_Permit` |
| `Payments` | `Entry_ID` (PK), `License_Plate`, `Work_Date`, `Entry_Time`, `Entry_Type` |
