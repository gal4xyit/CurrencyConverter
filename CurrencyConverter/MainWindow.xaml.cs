using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CurrencyConverter;

namespace CurrencyConverter_Static
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            radioRatesForToday.IsChecked = true; // Для отримання сьогоднішнього валютного курсу за замовчуванням
        }

        // Перенесення отриманого валютного курсу до комбо-боксів, коли вибраний варіант отримання курсу на сьогодні
        private async void BindTodaysCurrency()
        {
            await NBUApiFetcher.FetchCurrenciesAsync();

            //Перетворення Листа з отриманими курсами до Солвника для зручності
            Dictionary<string, double> dCurrency = NBUApiFetcher.fetchedCurrencies.ToDictionary(currency => currency.currencyCode, currency => currency.rate);


            cmbFromCurrency.ItemsSource = dCurrency;
            cmbFromCurrency.DisplayMemberPath = "Key";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = dCurrency.Count-1;

            cmbToCurrency.ItemsSource = dCurrency;
            cmbToCurrency.DisplayMemberPath = "Key";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = dCurrency.Count - 1;
        }

        // Перенесення отриманого валютного курсу до комбо-боксів, коли вибраний варіант отримання курсу на вибрану дату
        private async void BindDateCurrency()
        {
            await NBUApiFetcher.FetchCurrenciesAsyncOnDate(GetDateForFetch());

            Dictionary<string, double> dCurrency = NBUApiFetcher.fetchedCurrencies.ToDictionary(currency => currency.currencyCode, currency => currency.rate);

            cmbFromCurrency.ItemsSource = dCurrency;
            cmbFromCurrency.DisplayMemberPath = "Key";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = dCurrency.Count - 1;

            cmbToCurrency.ItemsSource = dCurrency;
            cmbToCurrency.DisplayMemberPath = "Key";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = dCurrency.Count - 1;

            //Відключення можливості взаємодії із інтерфейсом при не правильній даті
            if (cmbFromCurrency.Text == "WRONG DATE")
            {
                ClearControls();
                DisableGUIFunctionality();
            }
            else
            {
                EnableGUIFunctionality();
            }

        }


        //Логіка конвертаціїї одніїє валюти в іншу при натисканні на кнопку "Convert"
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            //Перевірки на те чи не порожнє поле для ведденя кількості валюти
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                txtCurrency.Focus();
                return;
            }

            //Перевірки на те чи не порожні комбо-бокси для вибору валюти
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == NBUApiFetcher.fetchedCurrencies.Count-1)
            {
                MessageBox.Show("Please Select Currency \"From\"", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == NBUApiFetcher.fetchedCurrencies.Count - 1)
            {
                MessageBox.Show("Please Select Currency \"To\"", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }

            //Процес конвератції валют, коли валют однакові або різні
            if(cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);
                lblCurrency.Content = cmbToCurrency.Text + "  " + ConvertedValue.ToString("N3");
            }
            else
            {
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)/double.Parse(cmbToCurrency.SelectedValue.ToString()));

                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }

        }

        //Очищення полів при натисканні на кнопку "Clear"
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        //Обмеженні на введеня лише цифр у полі вводу
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Зброс до стандартних значень
        private void ClearControls()
        {
            EnableGUIFunctionality();
            txtCurrency.Text = string.Empty;
            if(cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if(cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = string.Empty;
            txtCurrency.Focus();   
        }

        //Увімкнення вибору дати(та присвоєння їй сьогоднішньої дати за замовчуванням),  коли вибраний варіант отримання курсу на вибрану дату
        private void radioRatesOnDate_Checked(object sender, RoutedEventArgs e)
        {
            DatePicker.Visibility = Visibility.Visible;
            DatePicker.SelectedDate = DateTime.Now;
            ClearControls();
        }

        //Виключення вибору дати та привязка даних про сьогоднішній курс
        private void radioRatesForToday_Checked(object sender, RoutedEventArgs e)
        {
            DatePicker.Visibility = Visibility.Hidden;
            ClearControls();
            BindTodaysCurrency();
        }

        //Обробка дати з вибору дати(DataPicker) у правильну (для отримання данних за вибрану дату) форму.
        private string GetDateForFetch()
        {
            DateTime dataPickerDate = (DateTime)DatePicker.SelectedDate;
            string datPickerTextCorrected = dataPickerDate.ToString("yyyy/MM/dd");
            string[] charsetNoSlash = datPickerTextCorrected.Split('/');
            string noSlashCorrectedForFetchDate = "";
            foreach (var elem in charsetNoSlash)
            {
                noSlashCorrectedForFetchDate += elem;
            }
            return noSlashCorrectedForFetchDate;
        }

        //Прив'язка данних за вибрану дату після вибору ,безпосередньо, дати.
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            BindDateCurrency();
        }

        //Увімкнення та вимкнення функціональності полів.
        private void EnableGUIFunctionality()
        {
            txtCurrency.IsEnabled = true;
            Convert.IsEnabled = true;
            Clear.IsEnabled = true;
            cmbToCurrency.IsEnabled = true;
        }

        private void DisableGUIFunctionality()
        {
            txtCurrency.IsEnabled = false;
            Convert.IsEnabled = false;
            Clear.IsEnabled = false;
            cmbToCurrency.IsEnabled = false;
        }
    }
}
