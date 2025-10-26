public interface IBlock
{
    bool IsBlocking { get; }
    bool IsPerfectBlocking { get; }
    float BlockMultiplier { get; }

    bool IsOnCooldown { get; }
    bool TryBlock();
}