namespace Models;

public interface IBatchPayload<out TPayload>
{
    IEnumerable<TPayload> Payloads { get; }
}