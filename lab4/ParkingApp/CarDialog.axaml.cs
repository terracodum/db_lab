using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ParkingApp;

public partial class CarDialog : Window
{
    private readonly bool _editMode;

    public CarDialog()
    {
        InitializeComponent();
    }

    public CarDialog(Car car) : this()
    {
        _editMode = true;
        PlateBox.Text = car.License_Plate;
        PlateBox.IsEnabled = false;
        TypeBox.Text = car.Car_Type;
        ClientBox.Text = car.Client_ID.ToString();
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        var plate = PlateBox.Text?.Trim() ?? "";
        var type = TypeBox.Text?.Trim() ?? "";
        if (!int.TryParse(ClientBox.Text, out var clientId) || plate == "")
            return;

        Close(new Car { License_Plate = plate, Car_Type = type, Client_ID = clientId });
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e) => Close(null);
}
