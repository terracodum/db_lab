using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ParkingApp;

public partial class PaymentDialog : Window
{
    private readonly string _licensePlate;

    public PaymentDialog(string licensePlate)
    {
        InitializeComponent();
        _licensePlate = licensePlate;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        var date = DateBox.Text?.Trim() ?? "";
        var time = TimeBox.Text?.Trim() ?? "";
        var type = TypeBox.Text?.Trim() ?? "";
        if (date == "" || time == "") return;

        Close(new Payment
        {
            License_Plate = _licensePlate,
            Work_Date = date,
            Entry_Time = time,
            Entry_Type = type
        });
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e) => Close(null);
}
