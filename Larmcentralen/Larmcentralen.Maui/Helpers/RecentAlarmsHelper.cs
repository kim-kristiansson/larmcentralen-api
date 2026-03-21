namespace Larmcentralen.Maui.Helpers;

public static class RecentAlarmsHelper
{
    private const string Key = "recent_alarm_ids";
    private const int MaxRecent = 10;

    public static List<int> Get()
    {
        var stored = Preferences.Get(Key, "");
        if (string.IsNullOrEmpty(stored)) return [];

        return stored.Split(',')
            .Where(s => int.TryParse(s, out _))
            .Select(int.Parse)
            .ToList();
    }

    public static void Add(int alarmId)
    {
        var ids = Get();
        ids.Remove(alarmId);
        ids.Insert(0, alarmId);
        if (ids.Count > MaxRecent)
            ids = ids.Take(MaxRecent).ToList();

        Preferences.Set(Key, string.Join(",", ids));
    }
}