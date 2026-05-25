using Core.business.interfaces;
using Core.persistence.entities;
using Microsoft.EntityFrameworkCore;

namespace Core.persistence.repositories.implementations;

public class UserDocumentRepository : Repository<UserDocument>, IUserDocumentRepository {
    public UserDocumentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<UserDocument>> GetAllByUserIdAsync(int userId)
    {
        return await Queryable
            .Where<UserDocument>(_context.SharedDocuments
                .Include(ud => ud.Document), ud => ud.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserDocument>> GetAllByDocumentIdAsync(int documentId)
    {
        return await Queryable
            .Where<UserDocument>(_context.SharedDocuments, ud => ud.DocumentId == documentId)
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