using UnityEngine;

public class Field
{
    public Condition Weather {  get; set; }
    public int? WeatherDuration { get; set; }
    public void SetWeather(ConditionID weather)
    {
        Weather = ConditionDB.Conditions[weather];
        Weather.Id = weather;
        Weather.OnStart?.Invoke(null);
    }
}
