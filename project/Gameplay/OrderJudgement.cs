using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Raveyard;

public class OrderJudgement
{
    private const double MISS_BEATTIME = 1; // how many beats before a miss is forced
    private const double VALID_BEATTIME = 0.5; // how many beats off to count as valid (barely)
    private const double PERFECT_BEATTIME = 0.15; // how many beats off to count as perfect

    public event Action<JudgementResult> inputResult;
    private Queue<OrderJudgeMentInput> listOfInputs = new Queue<OrderJudgeMentInput>();

    private bool isTrackingOrder = false;
    private double startTimeOffset = 0;

    public void StartOrder(double beatTime)
    {
        if (isTrackingOrder) { return; }
        isTrackingOrder = true;
        startTimeOffset = beatTime;
    }

    public void AddInputToOrder(double inputBeatTime, InputType _inputType)
    {
        if (!isTrackingOrder) { return; }
        OrderJudgeMentInput addedInput = new OrderJudgeMentInput
        {
            beatTime = inputBeatTime - startTimeOffset,
            inputType = _inputType
        };
        
        listOfInputs.Enqueue(addedInput);
    }

    public void StopOrderAndListen(double beatTime)
    {
        startTimeOffset = beatTime;
        isTrackingOrder = false;
    }

    private JudgementResult GetResultForInput(double beatTime, InputType inputType)
    {
        if (listOfInputs.Count == 0) { return JudgementResult.none; }

        OrderJudgeMentInput input = listOfInputs.Peek();
        double difference = beatTime - input.beatTime;

        if (Math.Abs(difference) > VALID_BEATTIME || input.inputType != inputType) 
        { 
            return JudgementResult.none; 
        }

        listOfInputs.Dequeue();
        JudgementResult timingResult = difference >= 0 ? JudgementResult.late : JudgementResult.early;
        JudgementResult finalResult = Math.Abs(difference) <= PERFECT_BEATTIME ? JudgementResult.perfect : timingResult;
        return finalResult;
    }

    private void HandleMiss(double beatTime)
    {
        if (listOfInputs.Count == 0) { return; }

        OrderJudgeMentInput input = listOfInputs.Peek();
        double difference = beatTime - input.beatTime;

        if (difference > MISS_BEATTIME)
        {
            listOfInputs.Dequeue();
            inputResult?.Invoke(JudgementResult.miss);
        }
    }

    public void Update(double beatTime)
    {
        if (isTrackingOrder) { return; }

        double trueBeatTime = beatTime - startTimeOffset;
        KeyboardStateExtended keyboardState = KeyboardExtended.GetState();

        if (keyboardState.WasKeyPressed(Keys.Space)) { 
            inputResult?.Invoke(GetResultForInput(trueBeatTime, InputType.press)); 
        }

        if (keyboardState.WasKeyPressed(Keys.Left)) { 
            inputResult?.Invoke(GetResultForInput(trueBeatTime, InputType.left)); 
        }

        if (keyboardState.WasKeyPressed(Keys.Right)) { 
            inputResult?.Invoke(GetResultForInput(trueBeatTime, InputType.right)); 
        }

        HandleMiss(trueBeatTime);
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

struct OrderJudgeMentInput
{
    public double beatTime;
    public InputType inputType;
}