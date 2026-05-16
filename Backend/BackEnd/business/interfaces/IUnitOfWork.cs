namespace BackEnd.business.interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IDocumentRepository Documents { get; }
    IUserDocumentRepository SharedDocuments { get; }
    Task<int> CompleteAsync();
}