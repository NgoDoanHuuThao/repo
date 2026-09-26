using appbook.Models.Domain;
using appbook.Models.DTO;

namespace appbook.Repositories
{
    public interface IBookRepository
    {
        List<BookWithAuthorAndPublisherDTO> GetAllBooks();
        BookWithAuthorAndPublisherDTO? GetBookById(int id);
        addBookRequestDTO AddBook(addBookRequestDTO addBookRequestDTO);
        addBookRequestDTO? UpdateBookById(int id, addBookRequestDTO bookDTO);
        Books? DeleteBookById(int id);
    }
}
