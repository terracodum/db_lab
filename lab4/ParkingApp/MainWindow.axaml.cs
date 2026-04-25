using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;

namespace ParkingApp;

public class Car
{
    public string License_Plate { get; set; } = "";
    public string Car_Type { get; set; } = "";
    public int Client_ID { get; set; }
}

public class Payment
{
    public int Entry_ID { get; set; }
    public string License_Plate { get; set; } = "";
    public string Work_Date { get; set; } = "";
    public string Entry_Time { get; set; } = "";
    public string Entry_Type { get; set; } = "";
}

public partial class MainWindow : Window
{
    private const string DbPath = "lab3/parking.db";
    private readonly ObservableCollection<Car> _cars = new();
    private readonly ObservableCollection<Payment> _payments = new();

    public MainWindow()
    {
        InitializeComponent();
        CarsGrid.ItemsSource = _cars;
        PaymentsGrid.ItemsSource = _payments;
        LoadCars();
    }

    private SqliteConnection OpenDb()
    {
        var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();
        return conn;
    }

    private void LoadCars()
    {
        _cars.Clear();
        using var conn = OpenDb();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT License_Plate, Car_Type, Client_ID FROM Cars ORDER BY License_Plate";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            _cars.Add(new Car
            {
                License_Plate = reader.GetString(0),
                Car_Type = reader.IsDBNull(1) ? "" : reader.GetString(1),
                Client_ID = reader.GetInt32(2)
            });
    }

    private void LoadPayments(string licensePlate)
    {
        _payments.Clear();
        using var conn = OpenDb();
        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Entry_ID, License_Plate, Work_Date, Entry_Time, Entry_Type
            FROM Payments WHERE License_Plate = $plate ORDER BY Work_Date
            """;
        cmd.Parameters.AddWithValue("$plate", licensePlate);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            _payments.Add(new Payment
            {
                Entry_ID = reader.GetInt32(0),
                License_Plate = reader.GetString(1),
                Work_Date = reader.GetString(2),
                Entry_Time = reader.GetString(3),
                Entry_Type = reader.IsDBNull(4) ? "" : reader.GetString(4)
            });
    }

    private void CarsGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CarsGrid.SelectedItem is Car car)
            LoadPayments(car.License_Plate);
        else
            _payments.Clear();
    }

    private async void AddCar_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new CarDialog();
        var result = await dialog.ShowDialog<Car?>(this);
        if (result == null) return;

        using var conn = OpenDb();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Cars VALUES ($plate, $type, $client)";
        cmd.Parameters.AddWithValue("$plate", result.License_Plate);
        cmd.Parameters.AddWithValue("$type", result.Car_Type);
        cmd.Parameters.AddWithValue("$client", result.Client_ID);
        cmd.ExecuteNonQuery();
        LoadCars();
    }

    private async void EditCar_Click(object? sender, RoutedEventArgs e)
    {
        if (CarsGrid.SelectedItem is not Car car) return;

        var dialog = new CarDialog(car);
        var result = await dialog.ShowDialog<Car?>(this);
        if (result == null) return;

        using var conn = OpenDb();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Cars SET Car_Type = $type, Client_ID = $client WHERE License_Plate = $plate";
        cmd.Parameters.AddWithValue("$plate", car.License_Plate);
        cmd.Parameters.AddWithValue("$type", result.Car_Type);
        cmd.Parameters.AddWithValue("$client", result.Client_ID);
        cmd.ExecuteNonQuery();
        LoadCars();
    }

    private async void DeleteCar_Click(object? sender, RoutedEventArgs e)
    {
        if (CarsGrid.SelectedItem is not Car car) return;

        var ok = await new ConfirmDialog($"Удалить {car.License_Plate}?").ShowDialog<bool>(this);
        if (!ok) return;

        using var conn = OpenDb();
        var cmd = conn.CreateCommand();

        cmd.CommandText = "DELETE FROM Payments WHERE License_Plate = $plate";
        cmd.Parameters.AddWithValue("$plate", car.License_Plate);
        cmd.ExecuteNonQuery();

        cmd.CommandText = "DELETE FROM Parking_Access WHERE License_Plate = $plate";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "DELETE FROM Cars WHERE License_Plate = $plate";
        cmd.ExecuteNonQuery();

        _payments.Clear();
        LoadCars();
    }

    private async void AddPayment_Click(object? sender, RoutedEventArgs e)
    {
        if (CarsGrid.SelectedItem is not Car car) return;

        var dialog = new PaymentDialog(car.License_Plate);
        var result = await dialog.ShowDialog<Payment?>(this);
        if (result == null) return;

        using var conn = OpenDb();
        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Payments (License_Plate, Work_Date, Entry_Time, Entry_Type)
            VALUES ($plate, $date, $time, $type)
            """;
        cmd.Parameters.AddWithValue("$plate", result.License_Plate);
        cmd.Parameters.AddWithValue("$date", result.Work_Date);
        cmd.Parameters.AddWithValue("$time", result.Entry_Time);
        cmd.Parameters.AddWithValue("$type", result.Entry_Type);
        cmd.ExecuteNonQuery();
        LoadPayments(car.License_Plate);
    }

    private async void DeletePayment_Click(object? sender, RoutedEventArgs e)
    {
        if (PaymentsGrid.SelectedItem is not Payment payment) return;

        var ok = await new ConfirmDialog($"Удалить платёж #{payment.Entry_ID}?").ShowDialog<bool>(this);
        if (!ok) return;

        using var conn = OpenDb();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Payments WHERE Entry_ID = $id";
        cmd.Parameters.AddWithValue("$id", payment.Entry_ID);
        cmd.ExecuteNonQuery();

        if (CarsGrid.SelectedItem is Car car)
            LoadPayments(car.License_Plate);
    }
}
