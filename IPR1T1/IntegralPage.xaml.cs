using System;
using System.Threading;
using System.Threading.Tasks;

namespace IPR1_1
{
    public partial class IntegralPage : ContentPage
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isCalculating = false;

        public IntegralPage()
        {
            InitializeComponent();
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            if (_isCalculating)
                return;

            _isCalculating = true;
            _cancellationTokenSource = new CancellationTokenSource();
            StatusLabel.Text = "Вычисление...";
            ProgressBar.Progress = 0;
            ProgressLabel.Text = "0%";

            try
            {
                double result = await CalculateIntegralAsync(_cancellationTokenSource.Token);
                
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    StatusLabel.Text = "Задание отменено";
                }
                else
                {
                    StatusLabel.Text = $"Результат: {result:F6}";
                }
            }
            catch (OperationCanceledException)
            {
                StatusLabel.Text = "Задание отменено";
            }
            finally
            {
                _isCalculating = false;
                _cancellationTokenSource?.Dispose();
            }
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task<double> CalculateIntegralAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                double a = 0;
                double b = 1;
                double step = 0.00000001;
                long totalSteps = (long)((b - a) / step);
                double sum = 0;
                long currentStep = 0;

                for (double x = a; x < b; x += step)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    double y = Math.Sin(x);
                    sum += y * step;
                    
                    for (int i = 0; i < 100000; i++)
                    {
                        double dummy = 1.0 * 1.0;
                    }

                    currentStep++;
                    
                    double progress = (double)currentStep / totalSteps;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ProgressBar.Progress = progress;
                        ProgressLabel.Text = $"{(int)(progress * 100)}%";
                    });
                }

                return sum;
            }, cancellationToken);
        }
    }
}