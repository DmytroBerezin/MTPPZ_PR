using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Storage
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsBorrowed { get; set; }

        public Book(int id, string title, string author)
        {
            Id = id;
            Title = title;
            Author = author;
        }
    }

    public class BookRepository
    {
        private readonly List<Book> _books = new();

        public void Add(Book book) => _books.Add(book);

        public Book? GetById(int id) => _books.FirstOrDefault(b => b.Id == id);

        public IEnumerable<Book> GetAll() => _books;
    }
}

namespace Library.Logic
{
    using Library.Storage;

    public class LibraryService
    {
        private readonly BookRepository _repository;

        public LibraryService(BookRepository repository)
        {
            _repository = repository;
        }

        public void RegisterBook(int id, string title, string author)
        {
            _repository.Add(new Book(id, title, author));
        }

        public bool BorrowBook(int id)
        {
            var book = _repository.GetById(id);
            if (book == null || book.IsBorrowed)
                return false;

            book.IsBorrowed = true;
            return true;
        }

        public IEnumerable<Book> GetCatalog() => _repository.GetAll();
    }
}

namespace Library.UI
{
    using Library.Logic;
    using Library.Storage;

    public class LibraryConsoleUI
    {
        private readonly LibraryService _service;

        public LibraryConsoleUI(LibraryService service)
        {
            _service = service;
        }

        public void Run()
        {
            _service.RegisterBook(1, "Чистий код", "Роберт Мартін");
            _service.RegisterBook(2, "Патерни проєктування", "Еріх Гамма та ін.");

            Console.WriteLine(_service.BorrowBook(1)
                ? "Книгу ID=1 успішно видано читачу."
                : "Книга ID=1 недоступна.");

            Console.WriteLine("\n=== КАТАЛОГ БІБЛІОТЕКИ ===");
            foreach (var b in _service.GetCatalog())
            {
                string status = b.IsBorrowed ? "Видана" : "В наявності";
                Console.WriteLine($"[{b.Id}] \"{b.Title}\" — {b.Author} ({status})");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var repo = new BookRepository();
            var service = new LibraryService(repo);
            var ui = new LibraryConsoleUI(service);
            ui.Run();
        }
    }
}