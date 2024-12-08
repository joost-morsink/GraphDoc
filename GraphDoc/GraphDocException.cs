namespace GraphDoc;

public class GraphDocException : ApplicationException
{
    public GraphDocException(string message) : base(message) { }
    public GraphDocException(string message, Exception? inner) : base(message,inner) { }
}