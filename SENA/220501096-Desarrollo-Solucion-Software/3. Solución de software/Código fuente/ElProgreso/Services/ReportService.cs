using ElProgreso.Models;
using ElProgreso.Repositories;

namespace ElProgreso.Services;

/// <summary>
/// Report 1; "How much money do we have?"
/// </summary>
/// <param name="TotalBalance"></param>
/// <param name="MemberCount"></param>
/// <param name="AverageBalance"></param>
public sealed record CooperativeSummary(decimal TotalBalance, int MemberCount, decimal AverageBalance);

/// <summary>
/// Report 2: "Who are our best members?"
/// </summary>
/// <param name="DocumentNumber"></param>
/// <param name="FullName"></param>
/// <param name="Balance"></param>
public sealed record TopMember(string DocumentNumber, string FullName, decimal Balance);

/// <summary>
/// Report 3: "Who is dormant?"
/// </summary>
/// <param name="DocumentNumber"></param>
/// <param name="FullName"></param>
/// <param name="JoinedAt"></param>
public sealed record DormantMember(string DocumentNumber, string FullName, DateTime JoinedAt);

/// <summary>
/// Report 4: "How did we do in a period?"
/// </summary>
/// <param name="FromDate"></param>
/// <param name="ToDate"></param>
/// <param name="TotalDeposited"></param>
/// <param name="TotalWithdrawn"></param>
/// <param name="Difference"></param>
/// <param name="DepositCount"></param>
/// <param name="WithdrawalCount"></param>
public sealed record PeriodReport(DateTime FromDate, DateTime ToDate, decimal TotalDeposited, decimal TotalWithdrawn, decimal Difference, int DepositCount, int WithdrawalCount);

/// <summary>
/// Report 5: "What were the biggest movements?"
/// </summary>
/// <param name="OccurredAt"></param>
/// <param name="Type"></param>
/// <param name="Amount"></param>
/// <param name="MemberFullName"></param>
public sealed record LargestMovement(DateTime OccurredAt, MovementType Type, decimal Amount, string MemberFullName);

/// <summary>
/// Report 6: "Who is moving the cash the most?"
/// </summary>
/// <param name="FullName"></param>
/// <param name="MovementCount"></param>
/// <param name="TotalDeposited"></param>
/// <param name="TotalWithdrawn"></param>
/// <param name="CurrentBalance"></param>
public sealed record MemberActivity(string FullName, int MovementCount, decimal TotalDeposited, decimal TotalWithdrawn, decimal CurrentBalance);

public sealed class ReportService(IMemberRepository memberRepository)
{
    public CooperativeSummary GetCooperativeSummary()
    {
        var members = memberRepository.GetAll();
        var totalBalance = members.Sum(m => m.Balance);
        var averageBalance = members.Count == 0 ? 0m : totalBalance / members.Count;

        return new CooperativeSummary(totalBalance, members.Count, averageBalance);
    }

    public List<TopMember> GetTopMembers(int count = 5) =>
        memberRepository.GetAll()
            .OrderByDescending(m => m.Balance)
            .Take(count)
            .Select(m => new TopMember(m.DocumentNumber, m.FullName, m.Balance))
            .ToList();
    
    public List<DormantMember> GetDormantMembers() =>
        memberRepository.GetAll()
            .Where(m => !m.HasMovements)
            .OrderBy(m => m.JoinedAt)
            .Select(m => new DormantMember(m.DocumentNumber, m.FullName, m.JoinedAt))
            .ToList();

    public PeriodReport GetPeriodReport(DateTime from, DateTime to)
    {
        var fromDate = from.Date;
        var toDate = to.Date;

        if (fromDate > toDate)
        {
            throw new ArgumentException("La fecha inicial no puede ser posterior a la fecha final.");
        }

        var exclusiveEnd = toDate.AddDays(1);
        var movementsInRange = memberRepository.GetAll()
            .SelectMany(m => m.Movements)
            .Where(mv => mv.OccurredAt >= fromDate && mv.OccurredAt < exclusiveEnd)
            .ToList();

        var deposits = movementsInRange.Where(mv => mv.Type == MovementType.Deposit).ToList();
        var withdrawals = movementsInRange.Where(mv => mv.Type == MovementType.Withdrawal).ToList();
        var totalDeposited = deposits.Sum(mv => mv.Amount);
        var totalWithdrawn = withdrawals.Sum(mv => mv.Amount);

        return new PeriodReport(fromDate, toDate, totalDeposited, totalWithdrawn, totalDeposited - totalWithdrawn,
            deposits.Count, withdrawals.Count);
    }
    
    public List<LargestMovement> GetLargestMovements(int count = 10) =>
        memberRepository.GetAll()
            .SelectMany(m => m.Movements.Select(mv => (Member: m, Movement: mv)))
            .OrderByDescending(x => x.Movement.Amount)
            .Take(count)
            .Select(x => new LargestMovement(x.Movement.OccurredAt, x.Movement.Type, x.Movement.Amount, x.Member.FullName))
            .ToList();

    public List<MemberActivity> GetMemberActivity() =>
        memberRepository.GetAll()
            .Select(m => new MemberActivity(
                m.FullName,
                m.Movements.Count,
                m.Movements.Where(mv => mv.Type == MovementType.Deposit).Sum(mv => mv.Amount),
                m.Movements.Where(mv => mv.Type == MovementType.Withdrawal).Sum(mv => mv.Amount),
                m.Balance))
            .OrderByDescending(a => a.MovementCount)
            .ToList();
}