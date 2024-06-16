using UUIDNext;

namespace CommentMap.Mvc.Services;

public class UuidV7GenerationService : IIdGenerationService
{
    public Guid GenerateId()
    {
        return Uuid.NewSequential();
    }
}