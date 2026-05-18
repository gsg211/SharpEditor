using BackEnd.persistence.entities;
using BackEnd.business.interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.persistence.repositories.implementations;

public class UserDocumentRepository : Repository<UserDocument>, IUserDocumentRepository {
    public UserDocumentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<UserDocument>> GetAllByUserIdAsync(int userId)
    {
        return await _context.SharedDocuments
            .Include(ud => ud.Document)
            .Where(ud => ud.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserDocument>> GetAllByDocumentIdAsync(int documentId)
    {
        return await _context.SharedDocuments
            .Where(ud => ud.DocumentId == documentId)
            .ToListAsync();
    }

    public async Task<UserDocument?> GetByUserAndDocumentAsync(int userId, int documentId)
    {
        return await _context.SharedDocuments
            .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DocumentId == documentId);
    }

    public async Task<UserDocument?> GetByUserAndDocumentWithDocumentAsync(int userId, int documentId)
    {
        return await _context.SharedDocuments
            .Include(ud => ud.Document)
            .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DocumentId == documentId);
    }
}