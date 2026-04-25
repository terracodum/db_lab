using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ParkingReport;

public partial class MainWindow : Window
{
    private static readonly string DbPath =
        Path.Combine(AppContext.BaseDirectory, "../../../../../lab3/parking.db");

    public MainWindow()
    {
        InitializeComponent();
        ReportCombo.SelectedIndex = 0;
        FilterCombo.SelectedIndex = 0;
        ReportCombo.SelectionChanged += ReportCombo_SelectionChanged;
    }

    private void ReportCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ReportCombo.SelectedItem is not ComboBoxItem item) return;
        string tag = item.Tag?.ToString() ?? "";

        bool showFilter = tag == "payments";
        FilterCombo.IsVisible = showFilter;
        FilterLabel.IsVisible = showFilter;
    }

    private void Generate_Click(object? sender, RoutedEventArgs e)
    {
        if (ReportCombo.SelectedItem is not ComboBoxItem reportItem) return;
        string reportTag = reportItem.Tag?.ToString() ?? "payments";

        string filterValue = "All";
        if (FilterCombo.SelectedItem is ComboBoxItem fi)
            filterValue = fi.Tag?.ToString() ?? "All";

        try
        {
            string html = reportTag switch
            {
                "payments" => BuildPaymentsReport(filterValue),
                "noaccess" => BuildNoAccessReport(),
                "summary" => BuildSummaryReport(),
                _ => throw new Exception("Неизвестный отчёт")
            };

            string outPath = Path.Combine(Path.GetTempPath(), $"parking_{reportTag}.html");
            File.WriteAllText(outPath, html);

            StatusText.Text = $"Открыт: {outPath}";
            StatusText.Foreground = Avalonia.Media.Brushes.Green;

            Process.Start(new ProcessStartInfo { FileName = outPath, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Ошибка: {ex.Message}";
            StatusText.Foreground = Avalonia.Media.Brushes.Red;
        }
    }

    // ── Отчёт 1: Въезды ───────────────────────────────────────────────────

    private string BuildPaymentsReport(string filter)
    {
        var rows = new List<(int id, string plate, string type, int client,
                             string date, string time, string entryType)>();

        using var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT p.Entry_ID, p.License_Plate, c.Car_Type, c.Client_ID, p.Work_Date, p.Entry_Time, p.Entry_Type
            FROM Payments p
            JOIN Cars c ON p.License_Plate = c.License_Plate
            WHERE $f = 'All' OR p.Entry_Type = $f
            ORDER BY p.Work_Date, p.Entry_Time
            """;
        cmd.Parameters.AddWithValue("$f", filter);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add((r.GetInt32(0), r.GetString(1), r.GetString(2),
                      r.GetInt32(3), r.GetString(4), r.GetString(5), r.GetString(6)));

        int single = 0, sub = 0;
        foreach (var row in rows)
            if (row.entryType == "Single") single++; else sub++;

        var sb = new StringBuilder();
        AppendHead(sb, "Отчёт по въездам");
        sb.AppendLine($"<h1>Отчёт по въездам</h1>");
        sb.AppendLine($"<div class='meta'>Фильтр: <b>{filter}</b> &nbsp;|&nbsp; {DateTime.Now:dd.MM.yyyy HH:mm}</div>");
        sb.AppendLine("<table><thead><tr><th>#</th><th>Номер</th><th>Тип авто</th><th>Клиент</th><th>Дата</th><th>Время</th><th>Тип въезда</th></tr></thead><tbody>");
        foreach (var row in rows)
        {
            string cls = row.entryType == "Single" ? "single" : "sub";
            sb.AppendLine($"<tr><td>{row.id}</td><td>{row.plate}</td><td>{row.type}</td><td>{row.client}</td><td>{row.date}</td><td>{row.time}</td><td><span class='badge {cls}'>{row.entryType}</span></td></tr>");
        }
        sb.AppendLine("</tbody></table>");
        sb.AppendLine($"<div class='summary'>Итого: <b>{rows.Count}</b> &nbsp;|&nbsp; Single: <b>{single}</b> &nbsp;|&nbsp; Subscription: <b>{sub}</b></div>");
        AppendFoot(sb);
        return sb.ToString();
    }

    // ── Отчёт 2: Машины без допуска ───────────────────────────────────────

    private string BuildNoAccessReport()
    {
        var rows = new List<(string plate, string type, int client, string date)>();

        using var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT c.License_Plate, c.Car_Type, c.Client_ID, pa.Work_Date
            FROM Cars c
            JOIN Parking_Access pa ON c.License_Plate = pa.License_Plate
            WHERE pa.Access_Permit = 0
            ORDER BY pa.Work_Date
            """;
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add((r.GetString(0), r.GetString(1), r.GetInt32(2), r.GetString(3)));

        var sb = new StringBuilder();
        AppendHead(sb, "Машины без допуска");
        sb.AppendLine("<h1>Машины без допуска на стоянку</h1>");
        sb.AppendLine($"<div class='meta'>{DateTime.Now:dd.MM.yyyy HH:mm}</div>");
        sb.AppendLine("<table><thead><tr><th>Номер</th><th>Тип авто</th><th>Клиент ID</th><th>Дата</th></tr></thead><tbody>");
        foreach (var row in rows)
            sb.AppendLine($"<tr><td>{row.plate}</td><td>{row.type}</td><td>{row.client}</td><td>{row.date}</td></tr>");
        sb.AppendLine("</tbody></table>");
        sb.AppendLine($"<div class='summary'>Итого без допуска: <b>{rows.Count}</b></div>");
        AppendFoot(sb);
        return sb.ToString();
    }

    // ── Отчёт 3: Сводка по типам авто ────────────────────────────────────

    private string BuildSummaryReport()
    {
        var rows = new List<(string type, int count)>();

        using var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Car_Type, COUNT(*) AS Cnt
            FROM Cars
            GROUP BY Car_Type
            ORDER BY Cnt DESC
            """;
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add((r.GetString(0), r.GetInt32(1)));

        var sb = new StringBuilder();
        AppendHead(sb, "Сводка по типам авто");
        sb.AppendLine("<h1>Сводка по типам автомобилей</h1>");
        sb.AppendLine($"<div class='meta'>{DateTime.Now:dd.MM.yyyy HH:mm}</div>");
        sb.AppendLine("<table><thead><tr><th>Тип авто</th><th>Количество</th></tr></thead><tbody>");
        foreach (var row in rows)
            sb.AppendLine($"<tr><td>{row.type}</td><td>{row.count}</td></tr>");
        sb.AppendLine("</tbody></table>");
        AppendFoot(sb);
        return sb.ToString();
    }

    // ── HTML хелперы ──────────────────────────────────────────────────────

    private static void AppendHead(StringBuilder sb, string title)
    {
        sb.AppendLine($$"""
            <!DOCTYPE html><html lang="ru"><head>
            <meta charset="UTF-8"><title>{{title}}</title>
            <style>
              body  { font-family: Arial, sans-serif; margin: 40px; color: #222; }
              h1    { font-size: 20px; margin-bottom: 4px; }
              .meta { font-size: 13px; color: #555; margin-bottom: 16px; }
              table { border-collapse: collapse; width: 100%; font-size: 14px; }
              th    { background: #2c3e50; color: #fff; padding: 8px 12px; text-align: left; }
              td    { padding: 7px 12px; border-bottom: 1px solid #ddd; }
              tr:nth-child(even) td { background: #f5f7fa; }
              .badge { display:inline-block; padding:2px 8px; border-radius:10px; font-size:12px; font-weight:bold; }
              .single { background:#d4edda; color:#155724; }
              .sub    { background:#cce5ff; color:#004085; }
              .summary { margin-top: 16px; font-size: 14px; }
              @media print { button { display:none; } }
            </style></head><body>
            <button onclick="window.print()">Печать / PDF</button><br><br>
            """);
    }

    private static void AppendFoot(StringBuilder sb) =>
        sb.AppendLine("</body></html>");
}
