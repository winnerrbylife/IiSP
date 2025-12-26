using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Начало работы программы ===");
        int mainThreadId = Thread.CurrentThread.ManagedThreadId;
        Console.WriteLine($"Основной поток: {mainThreadId}");

        var passengers = GeneratePassengers(1000);
        Console.WriteLine($"Сгенерировано {passengers.Count} пассажиров");
        
        var streamService = new StreamService<Passenger>();
        
        streamService.OnProgress += message => 
        {
            Console.WriteLine($"# Событие: {message}");
        };
        
        using (var memoryStream = new MemoryStream())
        {
            var progress = new Progress<string>(message => 
            {
                Console.WriteLine($"Прогресс: {message}");
            });
            
            Console.WriteLine("\n--- Запуск потоков для записи и копирования ---");
            
            var writeTask = Task.Run(() => 
                streamService.WriteToStreamAsync(memoryStream, passengers, progress));
            
            await Task.Delay(150);
            
            var copyTask = Task.Run(() => 
                streamService.CopyFromStreamAsync(memoryStream, "passengers.txt", progress));
            
            Console.WriteLine("\nПотоки для записи и копирования запущены");
            Console.WriteLine("Ожидание завершения задач...");
            
            await Task.WhenAll(writeTask, copyTask);
            
            Console.WriteLine("\n--- Запись и копирование завершены ---");
            
            FileInfo fileInfo = new FileInfo("passengers.txt");
            if (fileInfo.Exists)
            {
                Console.WriteLine($"Создан файл: {fileInfo.FullName}");
                Console.WriteLine($"Размер файла: {fileInfo.Length} байт");
            }
            
            Console.WriteLine("\n--- Получение статистики ---");
            
            int passengersWithLuggage = await streamService.GetStatisticsAsync(
                "passengers.txt", 
                p => p.HasLuggage);
            
            Console.WriteLine("\n=== Статистическая информация ===");
            Console.WriteLine($"Количество пассажиров с багажом: {passengersWithLuggage}");
            Console.WriteLine($"Всего пассажиров: {passengers.Count}");
            double percentage = (double)passengersWithLuggage / passengers.Count * 100;
            Console.WriteLine($"Процент пассажиров с багажом: {percentage:F2}%");
        }
        
        Console.WriteLine("\n=== Программа завершена ===");
        Console.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    private static List<Passenger> GeneratePassengers(int count)
    {
        var passengers = new List<Passenger>();
        var random = new Random();
        
        for (int i = 1; i <= count; i++)
        {
            passengers.Add(new Passenger
            {
                Id = i,
                Name = $"Пассажир_{i}",
                HasLuggage = random.Next(2) == 1
            });
        }
        
        return passengers;
    }
}