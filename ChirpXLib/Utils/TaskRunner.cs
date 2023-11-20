using System;
using System.Collections.Concurrent;
using ChirpXLib.Hooks;
using Unity.Entities;

namespace ChirpXLib.Utils;

public class TaskRunner
{
    private static readonly ConcurrentQueue<CS2Task> PriorityTaskQueue = new();
    private static readonly ConcurrentQueue<CS2Task> TaskQueue = new();
    private static readonly SizedDictionaryAsync<Guid, CS2TaskResult> TaskResults = new(100);

    public static void Initialize()
    {
        EATSHook.OnUpdate += Update;
    }

    public static Guid Start(Func<World, object> func, bool HighPriority = false, bool runNow = true,
        bool getResult = false, TimeSpan startAfter = default)
    {
        var cs2Task = new CS2Task
        {
            ResultFunction = func,
            RunNow = runNow,
            GetResult = getResult,
            StartAfter = DateTime.UtcNow.Add(startAfter),
            TaskId = Guid.NewGuid()
        };
        if (HighPriority) PriorityTaskQueue.Enqueue(cs2Task);
        else TaskQueue.Enqueue(cs2Task);
        return cs2Task.TaskId;
    }

    public static object GetResult(Guid taskId)
    {
        return TaskResults.TryGetValue(taskId, out var result) ? result : null;
    }

    private static void Update(World world)
    {
        if (PriorityTaskQueue.Count > 0)
            for (var i = 0; i < PriorityTaskQueue.Count; i++)
            {
                if (!PriorityTaskQueue.TryDequeue(out var prioritytask)) continue;

                object priorityresult;
                try
                {
                    priorityresult = prioritytask.ResultFunction.Invoke(world);
                }
                catch
                {
                    if (prioritytask.GetResult)
                        TaskResults.Add(prioritytask.TaskId, new CS2TaskResult { Result = "Error" });
                    continue;
                }

                if (prioritytask.GetResult)
                    TaskResults.Add(prioritytask.TaskId, new CS2TaskResult { Result = priorityresult });
            }

        if (!TaskQueue.TryDequeue(out var task)) return;

        if (!task.RunNow)
            if (task.StartAfter > DateTime.UtcNow)
            {
                TaskQueue.Enqueue(task);
                return;
            }

        object result;
        try
        {
            result = task.ResultFunction.Invoke(world);
        }
        catch
        {
            if (task.GetResult) TaskResults.Add(task.TaskId, new CS2TaskResult { Result = "Error" });
            return;
        }

        if (task.GetResult) TaskResults.Add(task.TaskId, new CS2TaskResult { Result = result });
    }

    public static void Destroy()
    {
        EATSHook.OnUpdate -= Update;
        TaskQueue.Clear();
        TaskResults.Clear();
    }

    private class CS2Task
    {
        public Guid TaskId { get; set; } = Guid.NewGuid();
        public bool RunNow { get; set; }
        public bool GetResult { get; set; }
        public DateTime StartAfter { get; set; }
        public Func<World, object> ResultFunction { get; set; }
    }
}

public class CS2TaskResult
{
    public object Result { get; set; }
    public Exception Exception { get; set; }
}