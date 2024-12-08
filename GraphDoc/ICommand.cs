namespace GraphDoc;

public interface ICommand
{
    Task<string> Execute();
}