# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Назначение

Академический репозиторий с лабораторными работами по базам данных. Работы включают MS Access и MS SQL Server.

## Структура лабораторных

- `old/` — базы данных MS Access в формате `.mdb` (Access 97–2003)
- `new/` — базы данных MS Access в формате `.accdb` (Access 2007+)
- `lab3/` — задание для лабораторной по MS SQL Server (SELECT, UNION, PIVOT, временные таблицы, параметры)

## Контекст тем

**Лаб 1 (lab11):** Две базы данных — вариант A и вариант B. Каждая существует в двух форматах: `.mdb` (old) и `.accdb` (new).

**Лаб 3:** SQL-запросы в MS SQL Server — многотабличные выборки, UNION, PIVOT, временные таблицы (`#temp`), параметризованные запросы.

## Работа с файлами

`.accdb` / `.mdb` — бинарные файлы MS Access, редактируются только через MS Access или совместимые инструменты (например, mdbtools в Linux/macOS для чтения). Их нельзя редактировать текстовым редактором.

SQL-скрипты для MS SQL Server пишутся в `.sql` файлах и выполняются через SSMS или `sqlcmd`.
