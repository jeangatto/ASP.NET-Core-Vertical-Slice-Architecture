using System.Threading.Tasks;

namespace Blog.PublicAPI.Domain.PostAggregate;

public interface IPostUniquenessChecker
{
    Task ValidateTitleIsUniqueAsync(string title);
    Task ValidateTitleIsUniqueAsync(string title, Post postBeingUpdated);

}