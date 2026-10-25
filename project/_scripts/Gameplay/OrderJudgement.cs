using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Raveyard;

public class OrderJudgement
{
    private double bpm_crotchet = 1;
    public void SetBPM(double BPM) { bpm_crotchet = 60.0 / BPM; }

    private const double MISS_BEATTIME_SEC = 0.3; // how many beats before a miss is forced
    private const double VALID_BEATTIME_SEC = 0.3; // how many beats off to count as valid (barely)
    private const double PERFECT_BEATTIME_SEC = 0.1; // how many beats off to count as perfect

    private const int QUEUE_SIZE = 10; // preallocate memory for the inputs

    public event Action<(JudgementResult, InputType)> inputResult;
    private Queue<OrderJudgementInput> listOfInputs = new Queue<OrderJudgementInput>(QUEUE_SIZE);

    private bool isTrackingOrder = false;

    private double temp_startTimeOffset = 0;
    private Queue<OrderJudgementInput> temp_listOfInputs = new Queue<OrderJudgementInput>(QUEUE_SIZE);

    public void StartOrder(double beatTime)
    {
        if (isTrackingOrder) { return; }
        isTrackingOrder = true;
        temp_startTimeOffset = beatTime;
    }

    public void AddInputToOrder(double inputBeatTime, InputType _inputType)
    {
        if (!isTrackingOrder) { return; }
        OrderJudgementInput addedInput = new OrderJudgementInput
        {
            beatTime = inputBeatTime - temp_startTimeOffset,
            inputType = _inputType
        };
        
        temp_listOfInputs.Enqueue(addedInput);
    }

    public void StopOrderAndListen(double beatTime) // TODO: this isnt true anymore
    {
        while (temp_listOfInputs.Count > 0)
        {
            OrderJudgementInput input = temp_listOfInputs.Dequeue();
            input.beatTime += beatTime;
            listOfInputs.Enqueue(input);
        }
        isTrackingOrder = false;
    }

    private JudgementResult GetResult(double beatTime, InputType inputType)
    {
        if (listOfInputs.Count == 0) { return JudgementResult.none; }

        OrderJudgementInput input = listOfInputs.Peek();
        double difference = (beatTime - input.beatTime) * bpm_crotchet;
        difference -= SETTINGS.AUDIO_JUDGEMENT_OFFSET_MS / 1000f;

        if (Math.Abs(difference) > VALID_BEATTIME_SEC) //|| input.inputType != inputType) 
        { 
            return JudgementResult.none; 
        }


        listOfInputs.Dequeue();
        if (input.inputType != inputType) { return JudgementResult.miss; } // wrong input button will count as a miss

        Debug.WriteLine($"hit! diff: {difference*1000}ms");
        JudgementResult timingResult = difference >= 0 ? JudgementResult.late : JudgementResult.early;
        JudgementResult finalResult = Math.Abs(difference) <= PERFECT_BEATTIME_SEC ? JudgementResult.perfect : timingResult;
        return finalResult;
    }

    private (JudgementResult, InputType) GetResultForInput(double beatTime, InputType input)
    {
        return (GetResult(beatTime, input), input);
    }

    private void HandleMiss(double beatTime)
    {
        if (listOfInputs.Count == 0) { return; }

        OrderJudgementInput input = listOfInputs.Peek();
        double difference = (beatTime - input.beatTime) * bpm_crotchet;
        difference -= SETTINGS.AUDIO_JUDGEMENT_OFFSET_MS / 1000f;

        if (difference > MISS_BEATTIME_SEC)
        {
            listOfInputs.Dequeue();
            inputResult?.Invoke((JudgementResult.miss, InputType.press));
        }
    }

    public void Update(double beatTime)
    {
        KeyboardStateExtended keyboardState = KeyboardExtended.GetState();
        
        if (DebugTool.debugOption_autoplay) { debug_Update(beatTime); return; }

        if (keyboardState.WasKeyPressed(Keys.Space)) { 
            inputResult?.Invoke(GetResultForInput(beatTime, InputType.press)); 
        }

        if (keyboardState.WasKeyPressed(Keys.Left) || keyboardState.WasKeyPressed(Keys.A)) { 
            inputResult?.Invoke(GetResultForInput(beatTime, InputType.left)); 
        }

        if (keyboardState.WasKeyPressed(Keys.Right) || keyboardState.WasKeyPressed(Keys.D)) { 
            inputResult?.Invoke(GetResultForInput(beatTime, InputType.right)); 
        }

        HandleMiss(beatTime);
    }
    
    private void debug_Update(double beatTime)
    {
        if (!(listOfInputs.Count > 0)) { return; }
        OrderJudgementInput currentNextInput = listOfInputs.Peek();

        double difference = beatTime - currentNextInput.beatTime;
        if (difference >= 0)
        {
            inputResult?.Invoke(GetResultForInput(beatTime, currentNextInput.inputType));
        }
    }
}

public enum InputType
{
    press,
    left,
    right,
}

public enum JudgementResult
{
    none, // misinputs, usually
    miss, // failing to do what you're supposed to
    early,
    perfect, // perfect!
    late,
}

struct OrderJudgementInput
{
    public double beatTime;
    public InputType inputType;
}