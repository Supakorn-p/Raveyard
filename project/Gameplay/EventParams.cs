namespace Raveyard;

public class EventParams
{
    private string paramsString = "";

    public EventParams(string _paramsString)
    {
        paramsString = _paramsString;
    }

    public string[] getParameters()
    {
        return paramsString.Split(",");
    }
}