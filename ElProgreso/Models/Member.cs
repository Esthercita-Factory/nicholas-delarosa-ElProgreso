using System.Text.RegularExpressions;

namespace ElProgreso.Models;

/// <summary>
/// A cooperative member and their single savings account.
/// Deposits and withdrawal can only be added through <see cref="RegisterDeposit"/> and <see cref="RegisterWithdrawal"/>, so the balance is always a consequence of the recorded movements, never a value that gets typed or corrected by hand.
/// </summary>
public sealed partial class Member
{
    private const decimal LargeWithdrawalThreshold = 1_000_000m;
    private const decimal CashHandlingFee = 8_000m;

    private readonly List<Movement> _movements = [];

    public Member(string documentNumber, string fullName, string? phoneNumber, string? address)
    {
        DocumentNumber = ValidateDocumentNumber(documentNumber);
        FullName = ValidateFullName(fullName);
        PhoneNumber = ValidatePhoneNumber(phoneNumber);
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        JoinedAt = DateTime.Now;
    }

    public string DocumentNumber { get; }

    public string FullName { get; private set; }

    public string? PhoneNumber { get; private set; }

    public string? Address { get; private set; }

    public DateTime JoinedAt { get; }

    public IReadOnlyList<Movement> Movements => _movements;

    /// <summary>
    /// The account balance, always derived from the recorded movements.
    /// </summary>
    public decimal Balance => _movements.Sum(m => m.SignedEffect);

    public bool HasMovements => _movements.Count > 0;

    public void UpdateContactInfo(string fullName, string? phoneNumber, string? address)
    {
        FullName = ValidateFullName(fullName);
        PhoneNumber = ValidatePhoneNumber(phoneNumber);
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
    }

    public Movement RegisterDeposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("El valor de la consignación debe ser mayor a cero.");
        }

        var movement = new Movement(MovementType.Deposit, amount, Fee: 0m, DateTime.Now);
        _movements.Add(movement);
        return movement;
    }

    public Movement RegisterWithdrawal(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("El valor del retiro debe ser mayor a cero.");
        }

        var fee = amount > LargeWithdrawalThreshold ? CashHandlingFee : 0m;
        var totalDebit = amount + fee;

        if (totalDebit > Balance)
        {
            throw new InvalidOperationException(
                fee > 0
                    ? $"Saldo insuficiente: el retiro de {amount:N0} más la comisión de manejo de efectivo de {fee:N0} deja el saldo en negativo."
                    : "Saldo insuficiente: el asociado no tiene fondos suficientes para este retiro.");
        }

        var movement = new Movement(MovementType.Withdrawal, amount, fee, DateTime.Now);
        _movements.Add(movement);
        return movement;
    }

    private static string ValidateDocumentNumber(string? documentNumber)
    {
        var trimmed = documentNumber?.Trim() ?? string.Empty;

        if (trimmed.Length is < 6 or > 10 || !DigitsOnly().IsMatch(trimmed))
        {
            throw new ArgumentException("El número de documento debe tener sólo números, entre 6 y 10 dígitos.");
        }

        return trimmed;
    }

    private static string ValidateFullName(string? fullName)
    {
        var trimmed = fullName?.Trim() ?? string.Empty;

        if (trimmed.Length < 3 || !NameCharactersOnly().IsMatch(trimmed))
        {
            throw new ArgumentException("El nombre debe tener al menos 3 letras y no puede contener números ni símbolos.");
        }

        return trimmed;
    }

    private static string? ValidatePhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        var normalized = phoneNumber.Replace(" ", string.Empty).Replace("-", string.Empty).Trim();
        var digitCount = normalized.TrimStart('+').Length;

        if (digitCount is < 7 or > 15 || !PhoneCharactersOnly().IsMatch(normalized))
        {
            throw new ArgumentException("El teléfono debe tener sólo números (puede empezar con +), entre 7 y 15 dígitos.");
        }

        return normalized;
    }

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex DigitsOnly();

    [GeneratedRegex(@"^[\p{L}'\- ]+$")]
    private static partial Regex NameCharactersOnly();

    [GeneratedRegex(@"^\+?\d+$")]
    private static partial Regex PhoneCharactersOnly();
}