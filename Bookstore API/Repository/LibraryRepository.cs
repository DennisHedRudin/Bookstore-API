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
        internal BookModel GetBook(string ISBN)
        {
            if (string.IsNullOrEmpty(ISBN))
                return null;

            var xmlSerializer = new XmlSerializer(typeof(LibraryModel));

            using (var context = new StreamReader(_databasePath))
            {
                var library = (LibraryModel)xmlSerializer.Deserialize(context);

                return library.Books.FirstOrDefault(x => x.ISBN.Equals(ISBN));
            }
        }

        internal List<BookModel> SearchBooks(string query)
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
        }

        internal void AddBook(string author, string title, string isbn)
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
        }

        internal void SaveLibrary(LibraryModel library)
        {
            var xmlSerializer = new XmlSerializer(typeof(LibraryModel));

            using (var writer = new StreamWriter(_databasePath))
            {
                xmlSerializer.Serialize(writer, library);
            }
        }

        internal void SuggestBook(SuggestionModel model)
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
                filePath = Path.Combine(folderPath, $"{fileName}_{counter}.txt");
                counter++;
            }

            File.WriteAllText(filePath, $"Name: {model.Name}\nAuthor: {model.Author}\nTitle: {model.Title}");
        }
    }
}