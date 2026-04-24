# Лабораторная работа 4 — Формы для работы с БД

## Что нужно было реализовать

### 1. Подключение к БД и DataSet

В WinForms через мастер:
- Данные → Добавить новый источник данных → База данных
- Visual Studio создаёт `DataSet` — объект, который хранит таблицы в памяти

В нашей программе аналог — классы `Car` и `Payment`, которые хранят строки из БД:
```csharp
public class Car
{
    public string License_Plate { get; set; }
    public string Car_Type { get; set; }
    public int Client_ID { get; set; }
}
```

---

### 2. TableAdapter

В WinForms `TableAdapter` — автоматически сгенерированный класс, который выполняет SQL-запросы к БД (SELECT, INSERT, UPDATE, DELETE).

В нашей программе аналог — метод `OpenDb()` и SQL-запросы в каждом обработчике кнопок:
```csharp
private SqliteConnection OpenDb()
{
    var conn = new SqliteConnection($"Data Source={DbPath}");
    conn.Open();
    return conn;
}
```

---

### 3. BindingSource

В WinForms `BindingSource` — посредник между таблицей данных и элементами формы. При изменении данных форма обновляется автоматически.

В нашей программе аналог — `ObservableCollection<T>`. Она автоматически уведомляет DataGrid об изменениях:
```csharp
private readonly ObservableCollection<Car> _cars = new();
// ...
CarsGrid.ItemsSource = _cars; // DataGrid следит за коллекцией
```

---

### 4. DataGridView (табличное представление)

В WinForms `DataGridView` — таблица на форме, отображающая строки из БД.

В нашей программе аналог — `DataGrid` из Avalonia:
```xml
<dgx:DataGrid Name="CarsGrid" AutoGenerateColumns="False" IsReadOnly="True">
    <dgx:DataGrid.Columns>
        <dgx:DataGridTextColumn Header="Гос. номер" Binding="{Binding License_Plate}"/>
    </dgx:DataGrid.Columns>
</dgx:DataGrid>
```

---

### 5. BindingNavigator (навигация по записям)

В WinForms `BindingNavigator` — панель с кнопками «первый», «предыдущий», «следующий», «последний», «добавить», «удалить».

В нашей программе аналог — кнопки «Добавить», «Изменить», «Удалить» с обработчиками:
```csharp
private async void AddCar_Click(object? sender, RoutedEventArgs e)
{
    var dialog = new CarDialog();
    var result = await dialog.ShowDialog<Car?>(this);
    // сохраняем в БД через INSERT
}
```

---

### 6. Master-Detail (главная и подчинённая форма)

Главная форма — `Cars` (автомобили).  
Подчинённая форма — `Payments` (платежи), фильтруется по выбранной машине.

При выборе строки в верхней таблице нижняя автоматически обновляется:
```csharp
private void CarsGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    if (CarsGrid.SelectedItem is Car car)
        LoadPayments(car.License_Plate); // загружаем платежи только этой машины
}
```

В WinForms это настраивалось через связь между таблицами в `DataSet` — drag & drop из окна «Источники данных».

---

### 7. Диалоговые окна для редактирования

В WinForms — отдельная форма с текстовыми полями, которая открывается по кнопке.

В нашей программе — `CarDialog` и `PaymentDialog`:
```csharp
var dialog = new CarDialog(car);           // открыть окно с данными
var result = await dialog.ShowDialog<Car?>(this); // дождаться результата
```

---

## Итог: что реализовано

| Концепция WinForms | Наш аналог |
|---|---|
| DataSet | классы `Car`, `Payment` |
| TableAdapter | методы с SQL-запросами |
| BindingSource | `ObservableCollection<T>` |
| DataGridView | `DataGrid` (Avalonia) |
| BindingNavigator | кнопки Добавить/Изменить/Удалить |
| Master-Detail форма | Cars → Payments по выбранной машине |
| Диалог редактирования | `CarDialog`, `PaymentDialog` |
