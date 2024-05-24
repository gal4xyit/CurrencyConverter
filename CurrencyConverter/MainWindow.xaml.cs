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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            radioRatesForToday.IsChecked = true;
        }

        private async void BindTodaysCurrency()
        {
            await NBUApiFetcher.FetchCurrenciesAsync();

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

        private async void BindTodayCurrency()
        {
            await NBUApiFetcher.FetchCurrenciesAsync();

            Dictionary<string, double> dCurrency = NBUApiFetcher.fetchedCurrencies.ToDictionary(currency => currency.currencyCode, currency => currency.rate);
           
            cmbFromCurrency.ItemsSource = dCurrency;
            cmbFromCurrency.DisplayMemberPath = "Key";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = dCurrency.Count - 1;

            cmbToCurrency.ItemsSource = dCurrency;
            cmbToCurrency.DisplayMemberPath = "Key";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = dCurrency.Count-1;
        }

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

            if (cmbFromCurrency.Text == "WRONG DATE")
            {
                txtCurrency.IsEnabled = false;
            }
            else
            {
                txtCurrency.IsEnabled = true;
            }

        }



        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                txtCurrency.Focus();
                return;
            }

            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency \"From\"", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency \"To\"", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }

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

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if(cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if(cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = string.Empty;
            txtCurrency.Focus();   
        }

        private void radioRatesOnDate_Checked(object sender, RoutedEventArgs e)
        {
            DatePicker.Visibility = Visibility.Visible;
            DatePicker.SelectedDate = DateTime.Now;
            ClearControls();
        }

        private void radioRatesForToday_Checked(object sender, RoutedEventArgs e)
        {
            DatePicker.Visibility = Visibility.Hidden;
            ClearControls();
            BindTodaysCurrency();
        }

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

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            BindDateCurrency();
        }
    }
}
