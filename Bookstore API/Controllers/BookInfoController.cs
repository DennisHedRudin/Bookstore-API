using Bookstore_API.Models;
using Bookstore_API.Repository;
using System.Web.Http;


namespace Bookstore_API.Controllers
{

    public class BookInfoController : ApiController
    {
        private LibraryRepository library = new LibraryRepository();
        [HttpGet]
        public IHttpActionResult SearchBook(string ISBN)
        {
            if (string.IsNullOrWhiteSpace(ISBN))
                return BadRequest("ISBN cannot be null or empty");

            var book = library.GetBook(ISBN);

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpGet]
        public IHttpActionResult SearchBookWithQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search cannot be empty");

            var results = library.SearchBooks(query);

            if(results == null || results.Count == 0)
            {
                return NotFound();
            }

            return Ok(results);
                    
        }

        [HttpPost]
        public IHttpActionResult CreateBook(BookModel book)
        {
            if (book == null ||
                string.IsNullOrWhiteSpace(book.Author) ||
                string.IsNullOrWhiteSpace(book.Title) ||
                string.IsNullOrWhiteSpace(book.ISBN))
            {
                return BadRequest("Invalid data");
            }

            library.AddBook(book.Author, book.Title, book.ISBN);
            return Ok();
        }
    }
}
