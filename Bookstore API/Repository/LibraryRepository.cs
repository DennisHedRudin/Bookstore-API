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
        /// <returns>A list of books that match the search query. Returns an empty list if no matches are found. Returns null if an exception
        /// occurs during the search.</returns>
        internal List<BookModel>? SearchBooks(string searchQuery)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                    return new List<BookModel>();

                var xmlSerializer = new XmlSerializer(typeof(LibraryModel));

                using (var context = new StreamReader(_databasePath))
                {
                    var library = (LibraryModel)xmlSerializer.Deserialize(context);

                    if (library?.Books == null)
                        return new List<BookModel>();

                    var s = searchQuery.ToLower();

                    var results = library.Books.Where(x =>
                        x.ISBN.ToLower().Contains(s) ||
                        x.Author.ToLower().Contains(s) ||
                        x.Title.ToLower().Contains(s)
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
                    Directory.CreateDirectory(folderPath);
                

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