namespace BuildingBlocks.Application.Messaging;

public interface ICommand<TResponse> { }

public interface ICommand : ICommand<Unit>
{
}

public readonly struct Unit
{
    public static readonly Unit Value = new();
}
