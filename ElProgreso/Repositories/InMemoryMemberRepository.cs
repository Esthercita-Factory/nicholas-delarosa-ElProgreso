using ElProgreso.Models;

namespace ElProgreso.Repositories;

/// <summary>
/// Keeps members in memory for the lifetime of the process.
/// Good enough for a single teller working one session at a time; a real deployment would swap this for a database-backed repository behind the same interface, lol.
/// </summary>
public class InMemoryMemberRepository : IMemberRepository
{
    private readonly List<Member> _members = [];

    public void Add(Member member) => _members.Add(member);
    
    public Member? GetByDocumentNumber(string documentNumber) =>
        _members.FirstOrDefault(m => m.DocumentNumber == documentNumber.Trim());

    public List<Member> GetAll() => [.. _members.OrderBy(m => m.FullName)];

    public List<Member> SearchByName(string namePart)
    {
        var normalized = namePart.Trim();
        return _members
            .Where(m => m.FullName.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.FullName)
            .ToList();
    }

    public bool ExistsByDocumentNumber(string documentNumber) =>
        _members.Any(m => m.DocumentNumber == documentNumber.Trim());

    public void Remove(Member member) => _members.Remove(member);
}