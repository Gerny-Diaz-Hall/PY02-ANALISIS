// See https://aka.ms/new-console-template for more information
using System;

public class Office
{
    public int Id { get; set; } = 0;
    public List<Specialties> Specialties { get; set; } = new List<Specialty>();
    public bool Open { get; set; } = true;

    public Office(int id, List<string> Schedule, List<string> Specialties, bool Open)
    {
        Id = id+1;
        Schedule = Schedule;
        Specialties = Specialties;
        Open = Open;
    }

    public bool changeStatus()
    {
        if Open == true {
            Open == false;
        }
        else
            {
                Open == true;
            }
    }

    public bool getStatus()
    {
        return Open
    }
}
