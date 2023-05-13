using Muinmos.TechnicalTask;

var mLogger = new MLogger();

Task.Run(() => { for (int i = 0; i < 2000; i++) mLogger.Log($"Caller 1: Log message {i}"); });
Task.Run(() => { for (int i = 2000; i < 4000; i++) mLogger.Log($"Caller 2: Log message {i}"); });
Task.Run(() => { for (int i = 4000; i < 6000; i++) mLogger.Log($"Caller 3: Log message {i}"); });
Task.Run(() => { for (int i = 6000; i < 8000; i++) mLogger.Log($"Caller 4: Log message {i}"); });
Task.Run(() => { for (int i = 8000; i < 10000; i++) mLogger.Log($"Caller 5: Log message {i}"); });
Task.Run(() => { for (int i = 0; i < 500; i++) mLogger.Log($"Caller 6: Log extra message {i}"); });

Task.Delay(10000).Wait(); // Just for Test: wait to make sure all threads completed.

mLogger.Flush();
mLogger.Flush();

for (int i = 0; i < 500; i++) mLogger.Log($"Caller 7: Log extra message after flush {i}");

mLogger.Flush();
