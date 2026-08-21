using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Raveyard;

public class Timeline
{
    private List<TimelineEvent> timelineEvents = new List<TimelineEvent>();
    private Dictionary<string, Action<EventArgs>> eventBus = new Dictionary<string, Action<EventArgs>>();

    private double beatTimeNeedle = 0; // imagine a vinyl record, that's what "needle" means

    public void addEvent(string _eventName, double _beatTime)
    {
        timelineEvents.Add(new TimelineEvent { eventName = _eventName, beatTime = _beatTime });
        timelineEvents.Sort(new TimelineEventSort());
    }

    public void forceMoveNeedle(double beatTime)
    {
        beatTimeNeedle = beatTime;
    }

    public void Update(double time)
    {
        if (time < beatTimeNeedle) { return; }

        foreach (TimelineEvent _event in timelineEvents)
        {
            if (_event.beatTime < beatTimeNeedle) { continue; }
            if (_event.beatTime > time) { continue; }

            // TODO: replace this with actual events
            Debug.WriteLine($"Fire! {_event.eventName} at {_event.beatTime}");
        }

        beatTimeNeedle = time;
    }

    public void debugShowTimeline()
    {
        Debug.WriteLine("START");
        foreach (TimelineEvent _event in timelineEvents)
        {
            Debug.WriteLine($"{_event.eventName}:{_event.beatTime}");
        }
        Debug.WriteLine("END");
    }
}

public struct TimelineEvent
{
    public string eventName;
    public double beatTime;
}

public class TimelineEventSort : IComparer<TimelineEvent>
{
    public int Compare(TimelineEvent a, TimelineEvent b)
    {
        if (a.beatTime > b.beatTime) { return 1; }
        if (a.beatTime < b.beatTime) { return -1; }
        return 0;
    }
}