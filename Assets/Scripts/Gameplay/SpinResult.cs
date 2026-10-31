
using System.Collections.Generic;

public class SpinResult
{
    private readonly List<int> stopIndices;

    public IReadOnlyList<int> StopIndices => stopIndices;

    public SpinResult(List<int> indices)
    {
        stopIndices = new List<int>(indices);
    }

    public int GetStopIndex(int reelIndex)
    {
        return stopIndices[reelIndex];
    }

    public int ReelCount => stopIndices.Count;
}
