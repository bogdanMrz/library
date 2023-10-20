using System;
using System.Collections.Generic;

class Borrower
{

    public Borrower(int Id, string name) 
    { 
        this.Id = Id;
        this.name = name;
    }

    public int Id { get; set; }
    public string name { get; set; }


}

class Book
{
    private string _ISBN;
    public string ISBN { get { return _ISBN; } set { if (!verifyISBN(value)) { throw new ArgumentException("Bad ISBN fromat"); } _ISBN = value; } }

    public string title { get; set; }
    public decimal rentalPrice { get; set; }
    public int totalCopies { get; set; }

    public string author { get; set; }
    private Dictionary<Borrower, DateOnly> borrowedBook = new Dictionary<Borrower, DateOnly>();
    public Book(string title, string isbn, decimal rentalPrice)
    {
        this.title = title;
        this.rentalPrice = rentalPrice;

        if (!verifyISBN(isbn))
            throw new ArgumentException("Bad ISBN fromat");
        this.ISBN = isbn;


    }

    public Book(string title, string isbn, decimal rentalPrice, int totalCopies)
    {
        this.title = title;
        this.rentalPrice = rentalPrice;
        this.totalCopies = totalCopies;

        if (!verifyISBN(isbn))
            throw new ArgumentException("Bad ISBN fromat");
        this.ISBN = isbn;
    }
    public Book(string title, string isbn, decimal rentalPrice, string author)
    {
        this.title = title;
        this.rentalPrice = rentalPrice;
        this.author = author;

        if (!verifyISBN(isbn))
            throw new ArgumentException("Bad ISBN fromat");
        this.ISBN = isbn;
    }

    public Book(string title, string isbn, decimal rentalPrice, int totalCopies, string author)
    {
        this.title = title;
        this.rentalPrice = rentalPrice;
        this.totalCopies = totalCopies;
        this.author = author;

        if (!verifyISBN(isbn))
            throw new ArgumentException("Bad ISBN fromat");
        this.ISBN = isbn;
    }

    //as per https://www.totalpublishing.ro/index.php?route=stories/show&story_id=4
    public static bool verifyISBN(string isbn)
    {
        var chunks = isbn.Split('-');
        if (chunks.Length != 5)
            return false;

        return chunks.Sum(s => s.Length) == 13;


    }

    public int exemplaryLeft()
    {
        return totalCopies - borrowedBook.Count;

    }

    public bool areExemplaryLeft()
    {
        return exemplaryLeft() > 0;
    }


    public bool tryBorrowBook(Borrower borrower, DateOnly from)
    {
        if (borrower == null)
            return false;

        if (!areExemplaryLeft())
        {
            Console.WriteLine($"No available copies for book with title={title} and ISBN={ISBN} at this time.");
            return false;
        }

        borrowedBook[borrower] = from;

        return true;
    }

    public bool tryBorrowBook(Borrower borrower)
    {
        return tryBorrowBook(borrower, DateOnly.FromDateTime(DateTime.Now));
    }

    public bool tryReturnBook(Borrower borrower)
    {

        if (borrower == null)
            return false;
        if (!borrowedBook.ContainsKey(borrower))
        {
            Console.WriteLine($"The borrower with id={borrower.Id} and name={borrower.name} doesn't borrowed this book({ToString()}).");
            return false;
        }


        var borrowedBookDate = borrowedBook[borrower];
        borrowedBook.Remove(borrower);

        var extraDays = DateOnly.FromDateTime(DateTime.Now).DayNumber - borrowedBookDate.DayNumber - 14;


        if (extraDays > 0)
        {

            var extraPay = (extraDays * rentalPrice) / 100 ;
            Console.WriteLine($"The borrower with id={borrower.Id} and name={borrower.name} needs to pay={extraPay} for beeing late {extraDays} days.");

        }

        return true;
    }


    public override string ToString()
    {

        if (string.IsNullOrEmpty(author))
            return $"{title} (ISBN: {ISBN}), Rental Price: {rentalPrice:C}, Total Copies: {totalCopies}, Available Copies: {exemplaryLeft()}";

        return $"{title} by {author} (ISBN: {ISBN}), Rental Price: {rentalPrice:C}, Available Copies: {totalCopies}, Available Copies: {exemplaryLeft()}";

    }
}

class Library
{
    private List<Book> books = new List<Book>();

    public void addBook(Book book)
    {
        books.Add(book);
    }
    public void addBook(string title, string isbn, decimal rentalPrice, int totalCopies, string author)
    {
        Book book = new Book(title, isbn, rentalPrice, totalCopies, author);
        books.Add(book);
    }
    public void addBook(string title, string isbn, decimal rentalPrice, string author)
    {
        Book book = new Book(title, isbn, rentalPrice, author);
        books.Add(book);
    }

    public void addBook(string title, string isbn, decimal rentalPrice, int totalCopies)
    {
        Book book = new Book(title,isbn,rentalPrice,totalCopies);
        books.Add(book);
    }
    public void addBook(string title, string isbn, decimal rentalPrice)
    {
        Book book = new Book(title, isbn, rentalPrice);
        books.Add(book);
    }


    public List<Book> getAllBooks()
    {
        return books;
    }

    public int getTotalCopies(string isbn)
    {
        return books.Find(b => b.ISBN == isbn)?.totalCopies ?? 0;
    }

    public int getAvailableCopies(string isbn)
    {
        return books.Find(b => b.ISBN == isbn)?.exemplaryLeft() ?? 0;
    }

   

    public bool borrowBook(string isbn, Borrower borrower, DateOnly from)
    {

        Book book = books.Find(b => b.ISBN == isbn);
        if (book != null && book.totalCopies > 0)
        {
            return book.tryBorrowBook(borrower, from);
        }
        return false;
    }

    public bool borrowBook(string isbn, Borrower borrower)
    {
        return borrowBook(isbn, borrower, DateOnly.FromDateTime(DateTime.Now));
    }

    public bool returnBook(string isbn, Borrower borrower)
    {
        Book book = books.Find(b => b.ISBN == isbn);
        if (book != null)
        {
            return book.tryReturnBook(borrower);
        }
        return false;
    }
}

class Program
{
    static void Main()
    {
        Library library = new Library();

        string isbn = "123-456-789-012-3";
        string isbn2 = "987-654-321-01-23";
        string isbn3 = "999-99-888-88-777";

        library.addBook("C# Programming", isbn, 2.5m, 5);
        library.addBook("Introduction to OOP", isbn2, 3.0m, 3);
        library.addBook("The Raven", isbn3, 5m, 1, "Edgar Allan Poe");


        Console.WriteLine("All books in the library:");
        foreach (var book in library.getAllBooks())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine();



        Borrower borrower = new Borrower(1,"John Doe");
        Borrower borrower2 = new Borrower(2, "Joe Low");
        Borrower borrower3 = new Borrower(3, "Harry Dough");


        library.borrowBook(isbn, borrower);
        library.returnBook(isbn2, borrower); //Wrong borrower log
        Console.WriteLine();

        library.borrowBook(isbn2, borrower2, new DateOnly(2023, 9, 30));
        library.returnBook(isbn2, borrower2); //Over the limit log
        Console.WriteLine();

        library.borrowBook(isbn3, borrower3);
        library.borrowBook(isbn3, borrower); //No copies log






    }
}