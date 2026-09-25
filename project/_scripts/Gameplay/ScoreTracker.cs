using System.Collections.Generic;

namespace Raveyard;

public class ScoreTracker
{
    private int orderCount = 0;

    private int[] orderResultCount = {0, 0, 0};
    private double[] orderResultWeights = {1, 0.25, 0}; // how much the result counts towards score (i.e. 0.5 = half, 1 = full)

    public void AddJudgement(JudgementResult result)
    {
        if (result == JudgementResult.none) { return; }
        orderCount += 1;
        
        if (result == JudgementResult.perfect) 
        { 
            orderResultCount[(int) ScoreTrackerResult.perfect] += 1; 
        }
        if (result == JudgementResult.early || result == JudgementResult.late) 
        { 
            orderResultCount[(int) ScoreTrackerResult.barely] += 1; 
        }
        if (result == JudgementResult.miss) 
        { 
            orderResultCount[(int) ScoreTrackerResult.miss] += 1; 
        }
    }

    public double GetFinalPercentage()
    {
        // this will get the percentage of orders you completed

        double scoreSum = orderResultCount[(int) ScoreTrackerResult.perfect] * orderResultWeights[(int) ScoreTrackerResult.perfect]
                        + orderResultCount[(int) ScoreTrackerResult.barely] * orderResultWeights[(int) ScoreTrackerResult.barely]
                        + orderResultCount[(int) ScoreTrackerResult.miss] * orderResultWeights[(int) ScoreTrackerResult.miss];

        return scoreSum / orderCount * 100;
    }
}

public enum ScoreTrackerResult
{
    perfect = 0,
    barely = 1,
    miss = 2,
}