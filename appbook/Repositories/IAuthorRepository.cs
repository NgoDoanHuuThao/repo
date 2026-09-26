using appbook.Models.DTO;
using appbook.Models.Domain;
namespace appbook.Repositories
{
    public interface IAuthorRepository
    {
        List<AuthorDTO> GellAllAuthors();

        AuthorNoIdDTO GetAuthorById(int id);

        AddAuthorRequestDTO AddAuthor(
            AddAuthorRequestDTO addAuthorRequestDTO);

        AuthorNoIdDTO UpdateAuthorById(
            int id,
            AuthorNoIdDTO authorNoIdDTO);

        Authors? DeleteAuthorById(int id);
    }
}
