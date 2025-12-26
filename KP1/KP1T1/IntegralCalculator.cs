using System;
using System.Diagnostics;
using System.Threading;

public class IntegralCalculator
{
    public event Action<int, double, long> CalculationProgress = delegate { };
    public event Action<int, double, long> CalculationCompleted = delegate { };
    
    private static readonly object _singleLock = new object();
    private static readonly SemaphoreSlim _limitedSemaphore = new SemaphoreSlim(2, 2);
    
    public void CalculateIntegral(bool useSingleThread = false, bool useLimitedThreads = false)
    {
        // Для пункта 5: только один поток
        if (useSingleThread)
        {
            lock (_singleLock)
            {
                CalculateInternal();
            }
            return;
        }
        
        if (useLimitedThreads)
        {
            _limitedSemaphore.Wait();
            try
            {
                CalculateInternal();
            }
            finally
            {
                _limitedSemaphore.Release();
            }
            return;
        }
        
        CalculateInternal();
    }
    
    private void CalculateInternal()
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;
        var stopwatch = Stopwatch.StartNew();
        double result = 0;
        double step = 0.00000001; // ТОЧНО по условию
        
        long totalIterations = (long)(1.0 / step);
        long iteration = 0;
        
        for (double x = 0; x <= 1; x += step)
        {
            result += Math.Sin(x) * step;
            iteration++;
            
            for (int i = 0; i < 100000; i++)
            {
                double dummy = i * 1.1;
            }
            
            if (iteration % (totalIterations / 100) == 0)
            {
                double progress = (double)iteration / totalIterations * 100;
                CalculationProgress?.Invoke(threadId, progress, stopwatch.ElapsedTicks);
            }
        }
        
        stopwatch.Stop();
        CalculationCompleted?.Invoke(threadId, result, stopwatch.ElapsedTicks);
    }
}