using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Raveyard;

public class Timeline
{
    private List<TimelineEvent> timelineEvents = new List<TimelineEvent>();
    private Dictionary<string, Action<EventParams>> eventBus = new Dictionary<string, Action<EventParams>>();

    public double beatTimeNeedle {get; private set;} = 0; // imagine a vinyl record, that's what "needle" means
    public void forceMoveNeedle(double beatTime)
    {
        beatTimeNeedle = beatTime;
    }

    private void registerEventToBus(string eventName)
    {
        if (eventBus.ContainsKey(eventName)) { return; }
        Action<EventParams> action = new Action<EventParams>((EventParams) => {});
        eventBus[eventName] = action;
    }

    public void addEvent(string _eventName, double _beatTime, EventParams _params = null)
    {
        if (_params == null) { _params = new EventParams($"{_beatTime}"); }
        timelineEvents.Add(new TimelineEvent { eventName = _eventName, beatTime = _beatTime, parameters = _params });
        timelineEvents.Sort(new TimelineEventSort());

        registerEventToBus(_eventName);
    }

    public void subscribeToEvent(string eventName, Action<EventParams> action)
    {
        if (!eventBus.ContainsKey(eventName))
        {
            Debug.WriteLine($"this file doesn't contain event {eventName}!");
            return;
        }
        eventBus[eventName] += action;
    }

    public void Update(double time)
    {
        if (time < beatTimeNeedle) { return; }

        foreach (TimelineEvent _event in timelineEvents)
        {
            if (_event.beatTime < beatTimeNeedle) { continue; }
            if (_event.beatTime > time) { continue; }

            //Debug.WriteLine($"Fire! {_event.eventName} at {_event.beatTime}");
            eventBus[_event.eventName]?.Invoke(_event.parameters);
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
    public EventParams parameters;
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