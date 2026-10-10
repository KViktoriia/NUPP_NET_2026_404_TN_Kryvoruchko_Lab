namespace CoffeeShop.Console;

/// <summary>
/// Демонстрація примітивів синхронізації в .NET:
/// lock (Monitor), SemaphoreSlim, AutoResetEvent, ManualResetEventSlim, Mutex, ReaderWriterLockSlim.
/// </summary>
public static class SynchronizationDemo
{
    public static async Task RunAllDemosAsync()
    {
        DemoLock();
        await DemoSemaphoreSlimAsync();
        DemoAutoResetEvent();
        DemoManualResetEventSlim();
        DemoMutex();
        DemoReaderWriterLockSlim();
    }

    private static void DemoLock()
    {
        const int threadsCount = 10;
        const int iterations = 1000;
        int unsafeCounter = 0;
        var unsafeThreads = new List<Thread>();
        for (int i = 0; i < threadsCount; i++)
        {
            var t = new Thread(() =>
            {
                for (int j = 0; j < iterations; j++) unsafeCounter++;
            });
            unsafeThreads.Add(t);
            t.Start();
        }
        foreach (var t in unsafeThreads) t.Join();

        int safeCounter = 0;
        object lockObject = new();
        var safeThreads = new List<Thread>();
        for (int i = 0; i < threadsCount; i++)
        {
            var t = new Thread(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    lock (lockObject) safeCounter++;
                }
            });
            safeThreads.Add(t);
            t.Start();
        }
        foreach (var t in safeThreads) t.Join();

        System.Console.WriteLine($"[lock]              10 потоків x 1000 інкрементів: без lock = {unsafeCounter} (гонка даних), з lock = {safeCounter} (точно).");
    }

    private static async Task DemoSemaphoreSlimAsync()
    {
        using var semaphore = new SemaphoreSlim(3, 3);
        int maxConcurrent = 0;
        int currentConcurrent = 0;
        object syncRoot = new();

        var tasks = Enumerable.Range(1, 8).Select(async _ =>
        {
            await semaphore.WaitAsync();
            try
            {
                lock (syncRoot)
                {
                    currentConcurrent++;
                    if (currentConcurrent > maxConcurrent) maxConcurrent = currentConcurrent;
                }
                await Task.Delay(10);
            }
            finally
            {
                lock (syncRoot) currentConcurrent--;
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        System.Console.WriteLine($"[SemaphoreSlim]     Обмеження 3 кавомашин на 8 барист: максимум одночасних операцій = {maxConcurrent}.");
    }

    private static void DemoAutoResetEvent()
    {
        using var are = new AutoResetEvent(false);
        bool received = false;
        var thread = new Thread(() =>
        {
            are.WaitOne();
            received = true;
        });
        thread.Start();

        are.Set();
        thread.Join();
        System.Console.WriteLine($"[AutoResetEvent]    Сигнал замовлення: успішно отримано сигнал ({received}), подію автоматично скинуто.");
    }

    private static void DemoManualResetEventSlim()
    {
        using var mre = new ManualResetEventSlim(false);
        int enteredCount = 0;
        var threads = Enumerable.Range(1, 4).Select(_ => new Thread(() =>
        {
            mre.Wait();
            Interlocked.Increment(ref enteredCount);
        })).ToList();

        foreach (var t in threads) t.Start();
        mre.Set();
        foreach (var t in threads) t.Join();

        System.Console.WriteLine($"[ManualResetEvent]  Відкриття кав'ярні: сигнал Set() одночасно розблокував усіх {enteredCount} очікуючих клієнтів.");
    }

    private static void DemoMutex()
    {
        using var mutex = new Mutex();
        int transactionCount = 0;
        var threads = Enumerable.Range(1, 3).Select(_ => new Thread(() =>
        {
            mutex.WaitOne();
            try
            {
                transactionCount++;
                Thread.Sleep(5);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        })).ToList();

        foreach (var t in threads) t.Start();
        foreach (var t in threads) t.Join();

        System.Console.WriteLine($"[Mutex]             Монопольний доступ до каси: {transactionCount} транзакції виконано суворо по черзі.");
    }

    private static void DemoReaderWriterLockSlim()
    {
        var rwLock = new ReaderWriterLockSlim();
        int readsCount = 0;
        int writeSuccess = 0;

        var readers = Enumerable.Range(1, 3).Select(_ => Task.Run(() =>
        {
            rwLock.EnterReadLock();
            try { Interlocked.Increment(ref readsCount); }
            finally { rwLock.ExitReadLock(); }
        }));

        var writer = Task.Run(() =>
        {
            rwLock.EnterWriteLock();
            try { writeSuccess = 1; }
            finally { rwLock.ExitWriteLock(); }
        });

        Task.WaitAll([.. readers, writer]);
        rwLock.Dispose();
        System.Console.WriteLine($"[ReaderWriterLock]  Табло цін: успішно виконано {readsCount} паралельних читань та {writeSuccess} ексклюзивний запис.");
    }
}
