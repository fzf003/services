public static class Ext
{
    /// <summary>
    /// 判断值是否在两个值之间
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable<T>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }
}

public record Area
{
    public Area(int  min, int max)
    {
        this.Min=min;
        this.Max=max;
    }
    public int Min { get; set; }
    public int Max { get; set; }

    
    
    public bool Contains(int value)
    {
        return value.IsBetween(Min, Max);
    }
}



List<Area> areas = new List<Area>()
 {
   new Area(0,100),
   new Area(100,200),
   new Area(200,int.MaxValue)
 };

int value = 100;

foreach (var item in areas.Where(p => p.Contains(value)))
{
    Console.WriteLine(item);
}
