namespace Raveyard;

public class EventArgs
{
    private string argsString = "";

    public EventArgs(string _argsString)
    {
        argsString = _argsString;
    }

    public string[] getArgStrings()
    {
        return argsString.Split(",");
    }
}