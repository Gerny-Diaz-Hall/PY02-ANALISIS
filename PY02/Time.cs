using System;
using System.Collections.Generic;
using System.Timers;

public class Time {
    public event Action<string> OnChangeTime;
    private const int SecsPerMins = 7;
    private const int MinsPerHours = 60;
    private const int HoursPerDays = 24;
    private int days;
    private int hours;
    private int mins;

    private Timer timer;

    public Time() {
        // CORRECCIÓN: Se deben usar los nombres de las variables declaradas
        // en la clase (en inglés).
        days = 0;
        hours = 0;
        mins = 0;
        timer = new Timer(1000); // 7000 ms = 7 sec
        timer.Elapsed += AdvanceTime;
    }

    // Start the time system
    public void StartTimer() {
        timer.Start();
    }

    // Stop the time system
    public void StopTimer() {
        timer.Stop();
    }

    // Advance time
    private void AdvanceTime(object sender, ElapsedEventArgs e) {
        mins++;

        if (mins >= MinsPerHours) {
            mins = 0;
            hours++;

            if (hours >= HoursPerDays) {
                hours = 0;
                days++;
            }
        }

        OnChangeTime?.Invoke(ToString());
    }

    // Timer per Patient that returns a Action to stop timer if needed
    public Action StartPatientTimer(Action<string> onTick) {
        int pDays = 0;
        int pHours = 0;
        int pMins = 0;

        Timer patientTimer = new Timer(SecsPerMins * 1000);
        patientTimer.Elapsed += (s, e) => {
            pMins++;

            if (pMins >= MinsPerHours) {
                pMins = 0;
                pHours++;

                if (pHours >= HoursPerDays) {
                    pHours = 0;
                    pDays++;
                }
            }

            // Notification of time
            onTick?.Invoke($"{pDays}d {pHours:D2}:{pMins:D2}");
        };

        patientTimer.Start();

        // Action to stop timer when needed
        return () => patientTimer.Stop();
    }

    // formato militar
    public override string ToString() {
        return $"{days}d {hours:D2}:{mins:D2}";
    }

    // Obtener tiempo actual como un objeto
    public (int Days, int Hours, int Mins) getTime() {
        return (days, hours, mins);
    }

    private static Dictionary<string, int> EspecialidadDuraciones = new()
    {
        { "Medicina General", 30 },
        { "Cardiología", 45 },
        { "Dermatología", 25 },
        { "Pediatría", 40 },
        { "Oftalmología", 35 },
        { "Ginecología", 50 },
        { "Psiquiatría", 60 },
        { "Neurología", 55 },
        { "Ortopedia", 50 },
        { "Oncología", 70 },
        { "Traumatología", 50 }
    };

    public static int GetTime(string specialty) {
        if (EspecialidadDuraciones.TryGetValue(specialty, out int duracion))
            return duracion;

        return 30; // Default por si no está definida
    }
    public static void SetTime(string specialty, int duration) {
        if (!string.IsNullOrWhiteSpace(specialty) && duration > 0) {
            EspecialidadDuraciones[specialty] = duration;
        }
    }

}
