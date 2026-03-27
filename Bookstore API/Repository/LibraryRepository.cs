using Bookstore_API.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Generic;

namespace Bookstore_API.Repository
{
    internal class LibraryRepository
    {
        private readonly string _databasePath;

        public LibraryRepository()
        {
            var dbPath = ConfigurationManager.AppSettings["DatabasePath"];
            _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbPath);
        }
        //internal BookModel GetBook(string ISBN)
        //{
        //    if (string.IsNullOrEmpty(ISBN))
        //        return null;

        //    var xmlSerializer = new XmlSerializer(typeof(LibraryModel));

        //    using (var context = new StreamReader(_databasePath))
        //    {
        //        var library = (LibraryModel)xmlSerializer.Deserialize(context);

        //        return library.Books.FirstOrDefault(x => x.ISBN.Equals(ISBN));
        //    }
        //}


        /// <summary>
        /// Searches for books that match the specified query in the ISBN, author, or title fields.
        /// </summary>
        /// <remarks>The search is performed against all books in the library database and is
        /// case-insensitive. If the library database cannot be read or deserialized, the method returns an empty
        /// list.</remarks>
        /// <param name="query">The search term to use for matching books. The search is case-insensitive and matches any part of the ISBN,
        /// author, or title. If null, empty, or whitespace, no results are returned.</param>
        /// <returns>A list of books that match the search query. Returns an empty list if no matches are found or if an error
        /// occurs during the search.</returns>
        internal List<BookModel>? SearchBooks(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return new List<BookModel>();

                var xmlSerializer = new XmlSerializer(typeof(LibraryModel));

                using (var context = new StreamReader(_databasePath))
                {
                    var library = (LibraryModel)xmlSerializer.Deserialize(context);

                    if (library?.Books == null)
                        return new List<BookModel>();

                    var b = query.ToLower();

                    var results = library.Books.Where(x =>
                        x.ISBN.ToLower().Contains(b) ||
                        x.Author.ToLower().Contains(b) ||
                        x.Title.ToLower().Contains(b)
                    ).ToList();

                    return results;
                }
            } catch 
            {
                return null;
            }

        }

        /// <summary>
        /// Attempts to save a book using the provided parameters.
        /// </summary>
        /// <param name="author">Full name of the author</param>
        /// <param name="title">Name of Title</param>
        /// <param name="isbn">ISBN number</param>
        internal bool AddBook(string author, string title, string isbn)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(LibraryModel));

                LibraryModel library;

                using (var context = new StreamReader(_databasePath))
                {
                    library = (LibraryModel)xmlSerializer.Deserialize(context);
                }

                var newBook = new BookModel
                {
                    Author = author,
                    Title = title,
                    ISBN = isbn,
                };

                library.Books.Add(newBook);

                SaveLibrary(library);

                return true;
            } catch
            { 
                return false; 
            }
            
        }

        internal void SaveLibrary(LibraryModel library)
        {
            var xmlSerializer = new XmlSerializer(typeof(LibraryModel));

            using (var writer = new StreamWriter(_databasePath))
            {
                xmlSerializer.Serialize(writer, library);
            }
        }

        /// <summary>
        /// Attempts to save a book suggestion to a dated suggestions folder using the provided model information.
        /// </summary>
        /// <param name="model">The suggestion details, including the name, author, and title of the book to be suggested. Cannot be null.</param>
        /// <returns>true if the suggestion was successfully saved; otherwise, false.</returns>
        internal bool SuggestBook(SuggestionModel model)
        {
            try
            {
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Suggestions", date);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var name = model.Name.Trim();
                var author = model.Author.Trim();

                var fileName = $"{name[0]}_{date}_{author[0]}";

                var filePath = Path.Combine(folderPath, fileName + ".txt");

                int counter = 1;

                while (File.Exists(filePath))
                {
                    filePath = Path.Combine(folderPath, $"{fileName}{counter}.txt");
                    counter++;
                }

                File.WriteAllText(filePath, $"Name: {model.Name}\nAuthor: {model.Author}\nTitle: {model.Title}");

                return true;
            }
            catch 
            {
                return false;
            }
            
        }
    }
}