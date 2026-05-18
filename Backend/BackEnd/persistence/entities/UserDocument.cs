using Microsoft.EntityFrameworkCore;

namespace BackEnd.persistence.entities;

public enum SharePermission
{
    Owner,
    ReadOnly,
    ReadWrite,
}
[PrimaryKey(nameof(UserId),nameof(DocumentId))]
public class UserDocument
{
    public User User
    {
        get;
        set;
    }

    public int UserId
    {
        get;
        set;
    }
    public Document Document
    {
        get;
        set;
    }
    public int DocumentId
    {
        get;
        set;
    }
    public SharePermission PermissionLevel
    {
        get;
        set;
    }
}