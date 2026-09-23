using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appbook.Data;
using appbook.Models.Domain;
using appbook.Models.DTO;

namespace appbook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public BookController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors)
                .ThenInclude(ba => ba.Author)
                .ToList();

            var bookDTOs = books.Select(book => new
            {
                book.Id,
                book.Title,
                book.Description,
                book.IsRead,
                book.DateRead,
                book.Rate,
                book.Genre,
                book.CoverUrl,
                book.DateAdded,

                PublisherName = book.Publisher != null
                    ? book.Publisher.Name
                    : "Unknown",

                AuthorNames = book.Book_Authors
                    .Where(x => x.Author != null)
                    .Select(x => x.Author.FullName)
                    .ToList()
            }).ToList();

            return Ok(bookDTOs);
        }

        [HttpGet]
        [Route("get-book-by-id/{id:int}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookDomain = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors)
                .ThenInclude(ba => ba.Author)
                .FirstOrDefault(b => b.Id == id);

            if (bookDomain == null)
            {
                return NotFound();
            }

            var bookDTO = new
            {
                Id = bookDomain.Id,
                Title = bookDomain.Title,
                Description = bookDomain.Description,
                IsRead = bookDomain.IsRead,
                DateRead = bookDomain.DateRead,
                Rate = bookDomain.Rate,
                Genre = bookDomain.Genre,
                CoverUrl = bookDomain.CoverUrl,
                DateAdded = bookDomain.DateAdded,

                PublisherName = bookDomain.Publisher != null
                    ? bookDomain.Publisher.Name
                    : "Unknown",

                AuthorNames = bookDomain.Book_Authors?
                    .Where(y => y.Author != null)
                    .Select(y => y.Author.FullName)
                    .ToList()
                    ?? new List<string>()
            };

            return Ok(bookDTO);
        }
        [HttpPost("add-book")]
        public IActionResult AddBook(
            [FromBody] addBookRequestDTO addBookRequestDTO)
        {
            var publisherDomain = _dbContext.Publishers
                .FirstOrDefault(x =>
                    x.Id == addBookRequestDTO.PublisherId);

            if (publisherDomain == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy NXB"
                });
            }
            var bookDomain = new Books
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherId = publisherDomain.Id
            };

            _dbContext.Books.Add(bookDomain);
            _dbContext.SaveChanges();

            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorDomain = _dbContext.Authors
                    .FirstOrDefault(x => x.Id == authorId);

                if (authorDomain == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy tác giả"
                    });
                }

                var bookAuthorDomain = new Book_Author
                {
                    BookId = bookDomain.Id,
                    AuthorId = authorDomain.Id
                };

                _dbContext.Books_Authors.Add(bookAuthorDomain);
                _dbContext.SaveChanges();
            }

            return Ok(bookDomain);
        }

        [HttpPut("update-book-by-id/{id:int}")]
        public IActionResult UpdateBookById(
            int id,
            [FromBody] addBookRequestDTO addBookRequestDTO)
        {
            var bookDomain = _dbContext.Books
                .FirstOrDefault(x => x.Id == id);

            if (bookDomain == null)
            {
                return NotFound();
            }

            bookDomain.Title = addBookRequestDTO.Title;
            bookDomain.Description = addBookRequestDTO.Description;
            bookDomain.IsRead = addBookRequestDTO.IsRead;
            bookDomain.DateRead = addBookRequestDTO.DateRead;
            bookDomain.Rate = addBookRequestDTO.Rate;
            bookDomain.Genre = addBookRequestDTO.Genre;
            bookDomain.CoverUrl = addBookRequestDTO.CoverUrl;
            bookDomain.DateAdded = addBookRequestDTO.DateAdded;
            bookDomain.PublisherId = addBookRequestDTO.PublisherId;

            _dbContext.SaveChanges();

            var existingBookAuthors = _dbContext.Books_Authors
                .Where(x => x.BookId == id)
                .ToList();

            if (existingBookAuthors.Count > 0)
            {
                _dbContext.Books_Authors
                    .RemoveRange(existingBookAuthors);

                _dbContext.SaveChanges();
            }

            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorDomain = _dbContext.Authors
                    .FirstOrDefault(x => x.Id == authorId);

                if (authorDomain == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy tác giả"
                    });
                }

                var bookAuthorDomain = new Book_Author
                {
                    BookId = bookDomain.Id,
                    AuthorId = authorDomain.Id
                };

                _dbContext.Books_Authors.Add(bookAuthorDomain);
                _dbContext.SaveChanges();
            }

            return Ok(addBookRequestDTO);
        }

        [HttpDelete("delete-book-by-id/{id:int}")]
        public IActionResult DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books
                .FirstOrDefault(x => x.Id == id);

            if (bookDomain == null)
            {
                return NotFound();
            }

            var existingBookAuthors = _dbContext.Books_Authors
                .Where(x => x.BookId == id)
                .ToList();

            if (existingBookAuthors.Count > 0)
            {
                _dbContext.Books_Authors
                    .RemoveRange(existingBookAuthors);

                _dbContext.SaveChanges();
            }

            _dbContext.Books.Remove(bookDomain);
            _dbContext.SaveChanges();

            return Ok(bookDomain);
        }
        }
}
