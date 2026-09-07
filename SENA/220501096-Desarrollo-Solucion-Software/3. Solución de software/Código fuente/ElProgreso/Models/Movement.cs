namespace ElProgreso.Models;

/// <summary>
/// A single deposit or withdrawal against a member's account.
/// </summary>
public sealed record Movement(MovementType Type, decimal Amount, decimal Fee, DateTime OccurredAt)
{
    /// <summary>
    /// How much this movement adds to (positive) or subtracts from (negative) the balance. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public decimal SignedEffect => Type switch
    {
        MovementType.Deposit => Amount,
        MovementType.Withdrawal => -(Amount + Fee),
        _ => throw new InvalidOperationException($"Unsupported movement type: {Type}")
    };
}