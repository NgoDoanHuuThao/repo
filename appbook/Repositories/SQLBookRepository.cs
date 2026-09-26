using Microsoft.EntityFrameworkCore;
using appbook.Data;
using appbook.Models.Domain;
using appbook.Models.DTO;
namespace appbook.Repositories
{
    public class SQLBookRepository : IBookRepository

    {
        private readonly AppDbContext _dbContext;

        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<BookWithAuthorAndPublisherDTO> GetAllBooks()
        {
            var allBooks = _dbContext.Books
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead ? book.DateRead : null,
                    Rate = book.IsRead ? book.Rate : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    DateAdded = book.DateAdded,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors
                        .Select(n => n.Author.FullName)
                        .ToList()
                })
                .ToList();

            return allBooks;
        }

        public BookWithAuthorAndPublisherDTO? GetBookById(int id)
        {
            var bookWithDomain = _dbContext.Books
                .Where(n => n.Id == id);

            // Map Domain Model to DTO
            var bookWithIdDTO = bookWithDomain
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.DateRead,
                    Rate = book.Rate,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    DateAdded = book.DateAdded,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors
                        .Select(n => n.Author.FullName)
                        .ToList()
                })
                .FirstOrDefault();

            return bookWithIdDTO;
        }

        public addBookRequestDTO AddBook(
            addBookRequestDTO addBookRequestDTO)
        {
            // Map DTO to Domain Model
            var bookDomainModel = new Books
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherId = addBookRequestDTO.PublisherId
            };

            // Add Book
            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            // Add Authors
            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var bookAuthor = new Book_Author()
                {
                    BookId = bookDomainModel.Id,
                    AuthorId = id
                };

                _dbContext.Books_Authors.Add(bookAuthor);
                _dbContext.SaveChanges();
            }

            return addBookRequestDTO;
        }

        public addBookRequestDTO? UpdateBookById(
            int id,
            addBookRequestDTO bookDTO)
        {
            var bookDomain = _dbContext.Books
                .FirstOrDefault(n => n.Id == id);

            if (bookDomain == null)
            {
                return null;
            }

            bookDomain.Title = bookDTO.Title;
            bookDomain.Description = bookDTO.Description;
            bookDomain.IsRead = bookDTO.IsRead;
            bookDomain.DateRead = bookDTO.DateRead;
            bookDomain.Rate = bookDTO.Rate;
            bookDomain.Genre = bookDTO.Genre;
            bookDomain.CoverUrl = bookDTO.CoverUrl;
            bookDomain.DateAdded = bookDTO.DateAdded;
            bookDomain.PublisherId = bookDTO.PublisherId;

            _dbContext.SaveChanges();

            // Xóa tác giả cũ
            var authorDomain = _dbContext.Books_Authors
                .Where(a => a.BookId == id)
                .ToList();

            if (authorDomain.Count > 0)
            {
                _dbContext.Books_Authors.RemoveRange(authorDomain);
                _dbContext.SaveChanges();
            }

            // Thêm tác giả mới
            foreach (var authorId in bookDTO.AuthorIds)
            {
                var bookAuthor = new Book_Author()
                {
                    BookId = id,
                    AuthorId = authorId
                };

                _dbContext.Books_Authors.Add(bookAuthor);
            }

            _dbContext.SaveChanges();

            return bookDTO;
        }

        public Books? DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books
                .FirstOrDefault(n => n.Id == id);

            if (bookDomain == null)
            {
                return null;
            }

            // Xóa quan hệ Book - Author trước
            var authorDomain = _dbContext.Books_Authors
                .Where(a => a.BookId == id)
                .ToList();

            if (authorDomain.Count > 0)
            {
                _dbContext.Books_Authors.RemoveRange(authorDomain);
                _dbContext.SaveChanges();
            }

            // Xóa Book
            _dbContext.Books.Remove(bookDomain);
            _dbContext.SaveChanges();

            return bookDomain;
        }
    }
}