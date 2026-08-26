using System.Linq;

namespace Raveyard;

public class EventParams
{
    public string[] data_raw { get; private set; }
    public string[] data { get; private set; }
    public double beatTime { get; private set; }

    public EventParams(string paramsString)
    {
        data_raw = paramsString.Split(",");
        beatTime = double.Parse(data_raw[0]);
        data = data_raw.Skip(1).ToArray();
    }
}