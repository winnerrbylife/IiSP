using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== КОНТРОЛЬНАЯ РАБОТА №1 - ЗАДАНИЕ 1 ===");
       
        Console.WriteLine("=== 1. Запуск в отдельном потоке ===");
        RunSingleThreadExample();
        
        Console.WriteLine("\n=== 2. Два потока с разными приоритетами ===");
        RunPriorityThreadsExample();
        
        Console.WriteLine("\n=== 3. Пункт 5: Только один поток из 5 ===");
        RunSingleThreadLockExample();
        
        Console.WriteLine("\n=== 4. Пункт 6: Максимум 2 потока из 5 ===");
        RunLimitedThreadsExample();
        
        Console.WriteLine("\n=== Все задания выполнены ===");
    }
    
    static void RunSingleThreadExample()
    {
        var calculator = new IntegralCalculator();
        
        calculator.CalculationProgress += (threadId, progress, ticks) =>
        {
            Console.WriteLine($"Поток {threadId}: Прогресс {progress:F1}%");
        };
        
        calculator.CalculationCompleted += (threadId, result, ticks) =>
        {
            string formattedThreadId = threadId.ToString().PadLeft(10, '0');
            Console.WriteLine($"Поток {formattedThreadId}: Завершен с результатом: {result:F8}");
            Console.WriteLine($"Поток {formattedThreadId}: Время: {ticks} тиков");
        };
        
        Thread thread = new Thread(() => calculator.CalculateIntegral());
        thread.Start();
        thread.Join();
    }
    
    static void RunPriorityThreadsExample()
    {
        var calculatorHigh = new IntegralCalculator();
        var calculatorLow = new IntegralCalculator();
        
        calculatorHigh.CalculationCompleted += (threadId, result, ticks) =>
        {
            string formattedThreadId = threadId.ToString().PadLeft(10, '0');
            Console.WriteLine($"Поток {formattedThreadId} (Highest): Завершен с результатом: {result:F8}");
            Console.WriteLine($"Поток {formattedThreadId} (Highest): Время: {ticks} тиков");
        };
        
        calculatorLow.CalculationCompleted += (threadId, result, ticks) =>
        {
            string formattedThreadId = threadId.ToString().PadLeft(10, '0');
            Console.WriteLine($"Поток {formattedThreadId} (Lowest): Завершен с результатом: {result:F8}");
            Console.WriteLine($"Поток {formattedThreadId} (Lowest): Время: {ticks} тиков");
        };
        
        Thread highPriorityThread = new Thread(() => calculatorHigh.CalculateIntegral())
        {
            Priority = ThreadPriority.Highest,
            Name = "HighestPriority"
        };
        
        Thread lowPriorityThread = new Thread(() => calculatorLow.CalculateIntegral())
        {
            Priority = ThreadPriority.Lowest,
            Name = "LowestPriority"
        };
        
        highPriorityThread.Start();
        lowPriorityThread.Start();
        
        highPriorityThread.Join();
        lowPriorityThread.Join();
    }
    
    static void RunSingleThreadLockExample()
    {
        var calculator = new IntegralCalculator();
        int completedCount = 0;
        
        calculator.CalculationCompleted += (threadId, result, ticks) =>
        {
            completedCount++;
            string formattedThreadId = threadId.ToString().PadLeft(10, '0');
            Console.WriteLine($"Поток {formattedThreadId} ({completedCount}/5): Завершен с результатом: {result:F8}");
        };
        
        Thread[] threads = new Thread[5];
        for (int i = 0; i < 5; i++)
        {
            threads[i] = new Thread(() => calculator.CalculateIntegral(useSingleThread: true));
            threads[i].Start();
        }
        
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
    
    static void RunLimitedThreadsExample()
    {
        var calculator = new IntegralCalculator();
        int completedCount = 0;
        object lockObject = new object();
        
        calculator.CalculationCompleted += (threadId, result, ticks) =>
        {
            lock (lockObject)
            {
                completedCount++;
                string formattedThreadId = threadId.ToString().PadLeft(10, '0');
                Console.WriteLine($"Поток {formattedThreadId} ({completedCount}/5): Завершен с результатом: {result:F8}");
            }
        };
        
        Thread[] threads = new Thread[5];
        for (int i = 0; i < 5; i++)
        {
            int threadNum = i + 1;
            threads[i] = new Thread(() => 
            {
                calculator.CalculateIntegral(useLimitedThreads: true);
            });
            threads[i].Start();
        }
        
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}