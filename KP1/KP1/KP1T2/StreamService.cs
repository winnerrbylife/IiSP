using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class StreamService<T>
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    public event Action<string> OnProgress;

    public async Task WriteToStreamAsync(Stream stream, IEnumerable<T> data, 
        IProgress<string> progress)
    {
        await _semaphore.WaitAsync();
        try
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            progress?.Report($"Поток {threadId}: Начало записи в поток");
            OnProgress?.Invoke($"Поток {threadId}: Начало записи в поток");
            
            var dataList = new List<T>(data);
            
            for (int i = 0; i < dataList.Count; i++)
            {
                var item = dataList[i];
                string jsonString = JsonSerializer.Serialize(item);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonString + Environment.NewLine);
                
                await stream.WriteAsync(bytes, 0, bytes.Length);
                
                await Task.Delay(3);
                
                if ((i + 1) % 100 == 0)
                {
                    progress?.Report($"Поток {threadId}: Записано {i + 1}/{dataList.Count} объектов");
                    OnProgress?.Invoke($"Поток {threadId}: Записано {i + 1}/{dataList.Count} объектов");
                }
            }
            
            progress?.Report($"Поток {threadId}: Запись в поток завершена");
            OnProgress?.Invoke($"Поток {threadId}: Запись в поток завершена");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task CopyFromStreamAsync(Stream stream, string fileName, 
        IProgress<string> progress)
    {
        await _semaphore.WaitAsync();
        try
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            progress?.Report($"Поток {threadId}: Начало копирования из потока в файл");
            OnProgress?.Invoke($"Поток {threadId}: Начало копирования из потока в файл");
            
            stream.Position = 0;
            
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            
            progress?.Report($"Поток {threadId}: Копирование из потока в файл завершено");
            OnProgress?.Invoke($"Поток {threadId}: Копирование из потока в файл завершено");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<int> GetStatisticsAsync(string fileName, Func<T, bool> filter)
    {
        int count = 0;
        
        using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        using (var reader = new StreamReader(fileStream))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                try
                {
                    var item = JsonSerializer.Deserialize<T>(line);
                    if (filter(item))
                    {
                        count++;
                    }
                }
                catch (JsonException)
                {
                    continue;
                }
            }
        }
        
        return count;
    }
}