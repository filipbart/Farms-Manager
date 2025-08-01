using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

public class HatcheryNoteEntity : Entity
{
    public string Title { get; protected internal set; }
    public string Content { get; protected internal set; }
    
    protected HatcheryNoteEntity()
    {
    }

    public static HatcheryNoteEntity CreateNew(string title, string content, Guid? userId = null)
    {
        return new HatcheryNoteEntity
        {
            Title = title,
            Content = content,
            CreatedBy = userId
        };
    }

    public void Update(string title, string content)
    {
        Title = title;
        Content = content;
    }
}