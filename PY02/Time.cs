using System;
using System.Timers;

public class Time
{
    public event Action<string> OnChangeTime;
    private const int SecsPerMins = 7;
    private const int MinsPerHours = 60;
    private const int HoursPerDays = 24;
    private int days;
    private int hours;
    private int mins;

    private Timer timer;

    public Time()
    {
        dias = 0;
        horas = 0;
        minutos = 0;
        timer = new Timer(1000); // 1000 ms = 1 sec
        timer.Elapsed += AdvanceTime;
    }

    // Start the time system
    public void StartTimer()
    {
        timer.Start();
    }

    // Stop the time system
    public void StopTimer()
    {
        timer.Stop();
    }

    // Advance timeflod
    private void AdvanceTime(object sender, ElapsedEventArgs e)
    {
        mins++;

        if (mins >= MinsPerHours)
        {
            mins = 0;
            hours++;

            if (hours >= HoursPerDays)
            {
                hours = 0;
                days++;
            }
        }

    }

    // formato militar
    public override string ToString()
    {
        return $"{days}d {hours:D2}:{mins:D2}";
    }

    // Obtener tiempo actual como un objeto
    public (int Days, int Hours, int Mins) getTime()
    {
        return (days, hours, mins);
    }
}