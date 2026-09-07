using ElProgreso.Models;

namespace ElProgreso.Repositories;

public interface IMemberRepository
{
    void Add(Member member);
    
    Member? GetByDocumentNumber(string documentNumber);

    List<Member> GetAll();

    List<Member> SearchByName(string namePart);

    bool ExistsByDocumentNumber(string documentNumber);
    
    void Remove(Member member);
}