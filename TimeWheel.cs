using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Timers;
public class Task
{
    public int Id { get; set; }
    public int Delay { get; set; }
    public Action Action { get; set; }
}
public class TimeWheel
{
    private readonly int ticksPerSlot;
    private readonly int slotCount;
    private readonly Dictionary<int, List<Task>> slots;
    private int currentIndex;
    private Timer timer;
    private SQLiteConnection dbConnection;
    public TimeWheel(int ticksPerSlot, int slotCount)
    {
        this.ticksPerSlot = ticksPerSlot;
        this.slotCount = slotCount;
        this.slots = new Dictionary<int, List<Task>>();
        this.currentIndex = 0;
        // Create SQLite connection and table
        dbConnection = new SQLiteConnection("Data Source=tasks.db");
        dbConnection.Open();
        ExecuteNonQuery("CREATE TABLE IF NOT EXISTS Tasks (Id INTEGER PRIMARY KEY AUTOINCREMENT, Delay INTEGER, Action TEXT)");
    }
    public void AddTask(Action action, int delay)
    {
        int targetSlot = (currentIndex + delay / ticksPerSlot) % slotCount;
        var task = new Task { Delay = delay, Action = action };
        ExecuteNonQuery($"INSERT INTO Tasks(Delay, Action) VALUES({delay}, '{action.Method}')");
        if (!slots.ContainsKey(targetSlot))
        {
            slots[targetSlot] = new List<Task>();
        }
        slots[targetSlot].Add(task);
    }
    private void MoveNextSlot(object sender, ElapsedEventArgs e)
    {
        currentIndex = (currentIndex + 1) % slotCount;
        if (!slots.ContainsKey(currentIndex))
        {
            return;
        }
        foreach (var task in slots[currentIndex])
        {
            task.Action.Invoke();
            ExecuteNonQuery($"DELETE FROM Tasks WHERE Id = {task.Id}");
        }
        slots[currentIndex].Clear();
    }
    public void Start()
    {
        timer = new Timer(ticksPerSlot);
        timer.Elapsed += MoveNextSlot;
        timer.Enabled = true;
        Console.WriteLine($"Time wheel started with {ticksPerSlot}ms per tick.");
    }
    
    private void ExecuteNonQuery(string query)
    {
        using (var command = new SQLiteCommand(query, dbConnection))
        {
            command.ExecuteNonQuery();
        }
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        TimeWheel timeWheel = new TimeWheel(1000, 5);
        timeWheel.Start();
        // Add tasks to the time wheel
        timeWheel.AddTask(() => Console.WriteLine("Task 1 executed."), 3000);
        timeWheel.AddTask(() => Console.WriteLine("Task 2 executed."), 2000);
        timeWheel.AddTask(() => Console.WriteLine("Task 3 executed."), 4000);
        Console.ReadLine();
        // Cleanup: Close SQLite connection and dispose the timer
        timeWheel.dbConnection.Close();
        timeWheel.dbConnection.Dispose();
        timeWheel.timer.Dispose();
    }
}
