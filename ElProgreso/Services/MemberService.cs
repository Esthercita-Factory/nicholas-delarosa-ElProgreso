using ElProgreso.Models;
using ElProgreso.Repositories;

namespace ElProgreso.Services;

public sealed record BalanceInUsd(decimal BalanceCop, decimal ExchangeRate, decimal BalanceUsd, DateTime RateValidFrom, DateTime RateValidUntil);

public sealed class MemberService(IMemberRepository memberRepository, ExchangeRateService exchangeRateService)
{
    public Member Register(string documentNumber, string fullName, string? phoneNumber, string? address)
    {
        if (memberRepository.ExistsByDocumentNumber(documentNumber))
        {
            throw new InvalidOperationException($"Ya existe un asociado registrado con el documento {documentNumber}.");
        }

        var member = new Member(documentNumber, fullName, phoneNumber, address);
        memberRepository.Add(member);
        return member;
    }

    public List<Member> GetAll() => memberRepository.GetAll();

    public Member GetByDocument(string documentNumber) =>
        memberRepository.GetByDocumentNumber(documentNumber)
        ?? throw new InvalidOperationException($"No se encontró ningún asociado con el documento {documentNumber}.");

    public List<Member> SearchByName(string namePart) => memberRepository.SearchByName(namePart);

    public Member Update(string documentNumber, string fullName, string? phoneNumber, string? address)
    {
        var member = GetByDocument(documentNumber);
        member.UpdateContactInfo(fullName, phoneNumber, address);
        return member;
    }

    public void Delete(string documentNumber)
    {
        var member = GetByDocument(documentNumber);

        if (member.HasMovements)
        {
            throw new InvalidOperationException("No se puede eliminar un asociado que tiene saldo o movimientos registrados.");
        }

        memberRepository.Remove(member);
    }

    public async Task<BalanceInUsd> GetBalanceInUsdAsync(string documentNumber)
    {
        var member = GetByDocument(documentNumber);
        var rate = await exchangeRateService.GetLastestRateAsync();

        if (rate is null)
        {
            throw new InvalidOperationException("No fue posible obtener la TRM en este momento. Intente de nuevo más tarde.");
        }

        var balanceUsd = rate.Value == 0 ? 0 : member.Balance / rate.Value;
        return new BalanceInUsd(member.Balance, rate.Value, balanceUsd, rate.ValidFrom, rate.ValidUntil);
    }
}