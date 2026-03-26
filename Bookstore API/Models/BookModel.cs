using System.Xml.Serialization;

namespace Bookstore_API.Models
{
    public class BookModel
    {
        [XmlElement(ElementName = "Author")]
        public string Author { get; set; }
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "ISBN")]
        public string ISBN { get; set; }
    }
}