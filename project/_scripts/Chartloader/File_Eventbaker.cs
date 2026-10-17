using System.Collections.Generic;
using System.Diagnostics;

namespace Raveyard;

// auto-insert certain events like feedback sounds
public class File_Eventbaker
{
    private List<TimelineEvent> newEvents = new List<TimelineEvent>();

    public void Bake(ref Timeline timeline, List<TimelineEvent> _timelineEvents)
    {
        List<TimelineEvent> timelineEvents = _timelineEvents;
        timelineEvents.Sort(new TimelineEventSort());

        FeedbackSounds(timelineEvents);

        foreach (TimelineEvent newEvent in newEvents) { timeline.addEvent(newEvent.eventName, newEvent.beatTime, newEvent.parameters); }
    }

    private void FeedbackSounds(List<TimelineEvent> timelineEvents)
    {
        if (SETTINGS.AUDIO_FEEDBACK_PURE_MODE) { return; }

        bool isTracking = false;
        double lastTrackedBeattime = 0;

        // we don't know when exactly an order starts or ends because the files don't specify
        // so we have to read it from top to bottom
        Queue<TimelineEvent> orderQueue = new Queue<TimelineEvent>();


        foreach (TimelineEvent _event in timelineEvents)
        {
            Debug.WriteLine(_event.eventName);
            double diffFromOrderStart = _event.beatTime - lastTrackedBeattime;

            if (_event.eventName == "start_order")
            {
                isTracking = true;
                lastTrackedBeattime = _event.beatTime;
            }

            // now we do know when the order ends
            if (_event.eventName == "end_order") 
            { 
                isTracking = false; 
                double endOrderBeatTime = _event.beatTime;

                while (orderQueue.Count > 0)
                {
                    TimelineEvent heldEvent = orderQueue.Dequeue();
                    newEvents.Add(new TimelineEvent() {
                        eventName = heldEvent.eventName,
                        beatTime = heldEvent.beatTime + endOrderBeatTime,
                        parameters = new EventParams($"{heldEvent.beatTime + endOrderBeatTime}|{heldEvent.parameters.data[0]}")
                    });
                }
            }

            if (!isTracking) { continue; }

            if (_event.eventName == "press")
            {
                orderQueue.Enqueue(new TimelineEvent() {eventName = "play_sound", beatTime = diffFromOrderStart, 
                parameters = new EventParams($"{diffFromOrderStart}|snd_input_glassclink")});
            }

            if (_event.eventName == "left")
            {
                orderQueue.Enqueue(new TimelineEvent() {eventName = "play_sound", beatTime = diffFromOrderStart, 
                parameters = new EventParams($"{diffFromOrderStart}|snd_input_shakeleft")});
            }

            if (_event.eventName == "right")
            {
                orderQueue.Enqueue(new TimelineEvent() {eventName = "play_sound", beatTime = diffFromOrderStart, 
                parameters = new EventParams($"{diffFromOrderStart}|snd_input_shakeright")});
            }
        }
    }
}