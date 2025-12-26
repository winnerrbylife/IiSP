using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using IPR1_2.Models;
using IPR1_2.Services;

namespace IPR1_2
{
    public partial class CurrencyConverterPage : ContentPage
    {
        private readonly IRateService _rateService;
        private List<Rate> _rates;

        public CurrencyConverterPage(IRateService rateService)
        {
            InitializeComponent();
            _rateService = rateService;
            _rates = new List<Rate>();
            DatePicker.Date = DateTime.Today;
            DatePicker.MaximumDate = DateTime.Today;
        }

        private async void OnLoadRatesClicked(object sender, EventArgs e)
        {
            try
            {
                await DisplayAlert("Загрузка", "Загружаем курсы валют...", "OK");
                
                var rates = await _rateService.GetDailyRates(DatePicker.Date);
                _rates = rates.ToList();
                
                if (!_rates.Any())
                {
                    await DisplayAlert("Информация", "Курсы на выбранную дату не найдены", "OK");
                    return;
                }
                
                RatesCollectionView.ItemsSource = _rates;
                CurrencyPicker.ItemsSource = _rates;
                
                if (_rates.Any())
                    CurrencyPicker.SelectedIndex = 0;
                    
                await DisplayAlert("Успешно", $"Загружено {_rates.Count} валют", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить курсы: {ex.Message}", "OK");
            }
        }

        private void OnConvertToCurrencyClicked(object sender, EventArgs e)
        {
            ConvertCurrency(true);
        }

        private void OnConvertToBynClicked(object sender, EventArgs e)
        {
            ConvertCurrency(false);
        }

        private void ConvertCurrency(bool fromByn)
        {
            if (CurrencyPicker.SelectedItem is not Rate selectedRate)
            {
                DisplayAlert("Ошибка", "Сначала загрузите курсы и выберите валюту", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(AmountEntry.Text) || 
                !decimal.TryParse(AmountEntry.Text, out decimal amount) || 
                amount <= 0)
            {
                DisplayAlert("Ошибка", "Введите корректную сумму (больше 0)", "OK");
                return;
            }

            decimal result;
            string resultText;

            if (fromByn)
            {
                result = amount / (selectedRate.CurOfficialRate / selectedRate.CurScale);
                resultText = $"{amount:F2} BYN = {result:F4} {selectedRate.CurAbbreviation}";
            }
            else
                result = amount * (selectedRate.CurOfficialRate / selectedRate.CurScale);
                resultText = $"{amount:F2} {selectedRate.CurAbbreviation} = {result:F4} BYN";
            }

            ResultLabel.Text = $"Результат: {resultText}";
        }
    }
}