using System.Text;

namespace CoffeeShop.Console;

/// <summary>
/// Демонстрація примітивів синхронізації в .NET:
/// lock (Monitor), Semaphore / SemaphoreSlim, AutoResetEvent, ManualResetEventSlim, Mutex, ReaderWriterLockSlim.
/// </summary>
public static class SynchronizationDemo
{
    public static async Task RunAllDemosAsync()
    {
        System.Console.WriteLine("\n" + new string('=', 75));
        System.Console.WriteLine("          ДЕМОНСТРАЦІЯ ПРИМІТИВІВ СИНХРОНІЗАЦІЇ В .NET");
        System.Console.WriteLine(new string('=', 75));

        DemoLock();
        await DemoSemaphoreSlimAsync();
        DemoAutoResetEvent();
        DemoManualResetEventSlim();
        DemoMutex();
        DemoReaderWriterLockSlim();
    }

    /// <summary>
    /// 1. Примітив 'lock' (Monitor): запобігання race condition при паралельному доступі
    /// </summary>
    private static void DemoLock()
    {
        System.Console.WriteLine("\n[1] ДЕМОНСТРАЦІЯ ПРИМІТИВУ 'lock' (Monitor)");
        System.Console.WriteLine("    Сценарій: 10 паралельних потоків додають по 1000 замовлень до загального лічильника.");

        const int threadsCount = 10;
        const int iterations = 1000;
        const int expectedTotal = threadsCount * iterations;

        // Варіант БЕЗ синхронізації (Race Condition)
        int unsafeCounter = 0;
        var unsafeThreads = new List<Thread>();
        for (int i = 0; i < threadsCount; i++)
        {
            var t = new Thread(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    unsafeCounter++; // Не атомарна операція: читання -> модифікація -> запис
                }
            });
            unsafeThreads.Add(t);
            t.Start();
        }
        foreach (var t in unsafeThreads) t.Join();

        System.Console.WriteLine($"    -> БЕЗ lock (гонка даних): очікувано = {expectedTotal}, отримано = {unsafeCounter} {(unsafeCounter < expectedTotal ? "[ВТРАТА ДАНИХ!]" : "[Випадковий збіг]")}");

        // Варіант З lock
        int safeCounter = 0;
        object lockObject = new();
        var safeThreads = new List<Thread>();
        for (int i = 0; i < threadsCount; i++)
        {
            var t = new Thread(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    lock (lockObject)
                    {
                        safeCounter++;
                    }
                }
            });
            safeThreads.Add(t);
            t.Start();
        }
        foreach (var t in safeThreads) t.Join();

        System.Console.WriteLine($"    -> З lock (потокобезпечно): очікувано = {expectedTotal}, отримано = {safeCounter} [ТОЧНО]");
    }

    /// <summary>
    /// 2. Примітив 'Semaphore' / 'SemaphoreSlim': обмеження кількості одночасних потоків
    /// </summary>
    private static async Task DemoSemaphoreSlimAsync()
    {
        System.Console.WriteLine("\n[2] ДЕМОНСТРАЦІЯ ПРИМІТИВУ 'SemaphoreSlim'");
        System.Console.WriteLine("    Сценарій: У кав'ярні лише 3 кавомашини, але 8 барист одночасно хочуть готувати каву.");

        using var coffeeMachines = new SemaphoreSlim(initialCount: 3, maxCount: 3);
        var tasks = new List<Task>();

        for (int baristaId = 1; baristaId <= 8; baristaId++)
        {
            int id = baristaId;
            tasks.Add(Task.Run(async () =>
            {
                System.Console.WriteLine($"       [Бариста #{id}] Очікує на вільну кавомашину...");
                await coffeeMachines.WaitAsync();
                try
                {
                    System.Console.WriteLine($"       -> [Бариста #{id}] Зайняв кавомашину! Готує напій...");
                    await Task.Delay(150); // Імітація приготування
                    System.Console.WriteLine($"       <- [Бариста #{id}] Завершив приготування та звільнив кавомашину.");
                }
                finally
                {
                    coffeeMachines.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);
        System.Console.WriteLine("    -> Усі 8 замовлень успішно приготовлені з дотриманням ліміту в 3 паралельні машини.");
    }

    /// <summary>
    /// 3. Примітив 'AutoResetEvent': потік сигналізує іншому потоку про настання події
    /// </summary>
    private static void DemoAutoResetEvent()
    {
        System.Console.WriteLine("\n[3] ДЕМОНСТРАЦІЯ ПРИМІТИВУ 'AutoResetEvent'");
        System.Console.WriteLine("    Сценарій: Бариста чекає на нове замовлення; клієнт створює замовлення і надсилає сигнал.");

        using var orderSignal = new AutoResetEvent(initialState: false);
        string orderItem = string.Empty;

        var baristaThread = new Thread(() =>
        {
            System.Console.WriteLine("       [Бариста] Чекає на сигнал про надходження замовлення...");
            orderSignal.WaitOne(); // Блокується доки не прийде сигнал
            System.Console.WriteLine($"       [Бариста] Отримав сигнал! Почав готувати: '{orderItem}'.");
            Thread.Sleep(100);
            System.Console.WriteLine($"       [Бариста] Замовлення '{orderItem}' готове!");
        });

        baristaThread.Start();

        Thread.Sleep(80); // Імітація часу до замовлення клієнта
        orderItem = "Флет Вайт на вівсяному молоці";
        System.Console.WriteLine($"       [Клієнт] Зробив замовлення '{orderItem}' та надсилає сигнал Set()...");
        orderSignal.Set(); // Надсилаємо сигнал; AutoResetEvent автоматично скидається в false

        baristaThread.Join();
        System.Console.WriteLine("    -> AutoResetEvent автоматично повернувся в скинутий (unsignaled) стан після пробудження.");
    }

    /// <summary>
    /// 4. Примітив 'ManualResetEventSlim': одночасне розблокування групи потоків
    /// </summary>
    private static void DemoManualResetEventSlim()
    {
        System.Console.WriteLine("\n[4] ДЕМОНСТРАЦІЯ ПРИМІТИВУ 'ManualResetEventSlim'");
        System.Console.WriteLine("    Сценарій: 4 клієнти чекають біля дверей кав'ярні відкриття. О 08:00 двері відчиняються для ВСІХ одразу.");

        using var shopOpenEvent = new ManualResetEventSlim(initialState: false);
        var clientThreads = new List<Thread>();

        for (int clientId = 1; clientId <= 4; clientId++)
        {
            int id = clientId;
            var t = new Thread(() =>
            {
                System.Console.WriteLine($"       [Клієнт #{id}] Чекає відкриття дверей...");
                shopOpenEvent.Wait(); // Чекає сигналу відкриття
                System.Console.WriteLine($"       [Клієнт #{id}] Двері відчинено! Заходить у кав'ярню.");
            });
            clientThreads.Add(t);
            t.Start();
        }

        Thread.Sleep(100);
        System.Console.WriteLine("       [Менеджер] 08:00 - відмикає двері! shopOpenEvent.Set()");
        shopOpenEvent.Set(); // Всі потоки одночасно розблоковуються

        foreach (var t in clientThreads) t.Join();
        System.Console.WriteLine("    -> ManualResetEventSlim залишається активним для всіх потоків, поки не викликано Reset().");
    }

    /// <summary>
    /// 5. Примітив 'Mutex': монопольний доступ до критичного ресурсу (касового апарату)
    /// </summary>
    private static void DemoMutex()
    {
        System.Console.WriteLine("\n[5] ДЕМОНСТРАЦІЯ ПРИМІТИВУ 'Mutex'");
        System.Console.WriteLine("    Сценарій: 3 касири одночасно хочуть провести оплату через єдиний термінал.");

        using var posTerminalMutex = new Mutex();
        var threads = new List<Thread>();

        for (int cashierId = 1; cashierId <= 3; cashierId++)
        {
            int id = cashierId;
            var t = new Thread(() =>
            {
                System.Console.WriteLine($"       [Касир #{id}] Запитує доступ до POS-терміналу...");
                posTerminalMutex.WaitOne(); // Захоплення м'ютекса
                try
                {
                    System.Console.WriteLine($"       -> [Касир #{id}] Отримав монопольний доступ, проводить транзакцію...");
                    Thread.Sleep(100);
                    System.Console.WriteLine($"       <- [Касир #{id}] Транзакцію завершено, чек надруковано.");
                }
                finally
                {
                    posTerminalMutex.ReleaseMutex(); // Звільнення м'ютекса
                }
            });
            threads.Add(t);
            t.Start();
        }

        foreach (var t in threads) t.Join();
        System.Console.WriteLine("    -> Mutex забезпечив суворо послідовний доступ до терміналу без накладання операцій.");
    }

    /// <summary>
    /// 6. Примітив 'ReaderWriterLockSlim': багаторазове паралельне читання та монопольний запис
    /// </summary>
    private static void DemoReaderWriterLockSlim()
    {
        System.Console.WriteLine("\n[6] ДЕМОНСТРАЦІЯ ПРИМІТИВУ 'ReaderWriterLockSlim'");
        System.Console.WriteLine("    Сценарій: Електронне табло меню: кілька клієнтів можуть читати ціни одночасно, але менеджер оновлює ціни монопольно.");

        var rwLock = new ReaderWriterLockSlim();
        decimal currentPrice = 65.00m;
        var readerTasks = new List<Task>();

        // Створюємо 3 паралельних читачів
        for (int readerId = 1; readerId <= 3; readerId++)
        {
            int id = readerId;
            readerTasks.Add(Task.Run(() =>
            {
                rwLock.EnterReadLock();
                try
                {
                    System.Console.WriteLine($"       [Гість #{id}] Читає меню: ціна кави = {currentPrice} грн.");
                    Thread.Sleep(50);
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }));
        }

        // Паралельний записувач
        var writerTask = Task.Run(() =>
        {
            Thread.Sleep(20);
            rwLock.EnterWriteLock();
            try
            {
                System.Console.WriteLine("       -> [Менеджер] Отримує монопольне право на запис. Оновлює меню...");
                currentPrice = 75.00m;
                Thread.Sleep(80);
                System.Console.WriteLine($"       <- [Менеджер] Ціну оновлено до {currentPrice} грн! Звільняє блокування.");
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        });

        Task.WaitAll([.. readerTasks, writerTask]);
        rwLock.Dispose();
        System.Console.WriteLine("    -> ReaderWriterLockSlim дозволяє спільне паралельне читання та виключний ексклюзивний запис.");
    }
}
