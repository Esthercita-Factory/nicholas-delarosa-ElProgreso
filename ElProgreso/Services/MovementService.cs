using ElProgreso.Models;
using ElProgreso.Repositories;

namespace ElProgreso.Services;

public sealed class MovementService(IMemberRepository memberRepository)
{
    public Movement RegisterDeposit(string documentNumber, decimal amount)
    {
        var member = GetMemberOrThrow(documentNumber);
        return member.RegisterDeposit(amount);
    }

    public Movement RegisterWithdrawal(string documentNumber, decimal amount)
    {
        var member = GetMemberOrThrow(documentNumber);
        return member.RegisterWithdrawal(amount);
    }

    public IReadOnlyList<Movement> GetMovements(string documentNumber) => GetMemberOrThrow(documentNumber).Movements;

    private Member GetMemberOrThrow(string documentNumber) =>
        memberRepository.GetByDocumentNumber(documentNumber)
        ?? throw new InvalidOperationException($"No se encontró ningún asociado con el documento {documentNumber}.");
}