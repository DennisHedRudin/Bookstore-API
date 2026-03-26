using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bookstore_API.Models
{
    [XmlRoot("Library")]
    public class LibraryModel
    {
        [XmlElement("Book")]
        public List<BookModel> Books { get; set; }
    }
}