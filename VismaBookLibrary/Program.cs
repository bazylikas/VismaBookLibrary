using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace VismaBookLibrary
{
    class Program
    {
        static List<Book> bookList;
        
        static void Main(string[] args)
        {
            bookList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText("../../../books.json"));
            Console.WriteLine("Visma Book Library");

            while (true)
            {
                printoutMenu();
                getCommand();
            }
        }
        private static void printoutMenu()
        {
            Console.WriteLine(new string('─', 35));
            Console.WriteLine("Type the keyword to call a command");
            Console.WriteLine();
            Console.WriteLine("[list] lists books by filter");
            Console.WriteLine("[take] - take a book");
            Console.WriteLine("[return] - return a book");
            Console.WriteLine("[add] - add a new book");
            Console.WriteLine("[remove] - remove a book");
            Console.WriteLine(new string('─', 35));

        }
        private static void getCommand()
        {
            string command = Console.ReadLine();

            switch (command)
            {
                case "list":

                    Console.WriteLine(new string('─', 35));
                    Console.WriteLine("Type which filter to use (type [any] to list all books)");
                    Console.WriteLine("Available filters: (title, author, category, language, ISBN, available, taken)");
                    string filter = Console.ReadLine().ToLower();

                    if (filter == "any" || filter == "title" || filter == "author" || filter == "category" || filter == "language" || filter == "ISBN" || filter == "available" || filter == "taken")
                    {
                        string filterValue = "";
                        if (filter != "any" && filter != "taken" && filter != "available")
                        {
                            Console.WriteLine("Enter search criteria: ");
                            filterValue = Console.ReadLine();
                        }

                        listBooks(filterValue, filter);
                    }
                    else
                    {
                        Console.WriteLine("Wrong filter!");
                        return;
                    }
                    break;

                case "take":

                    Console.WriteLine("Enter ISBN of the book you wish to take: ");
                    string ISBN = Console.ReadLine();
                    if(bookList.Find(p => p.ISBN == ISBN) == null)
                    {
                        Console.WriteLine("Book not found!");
                    }
                    else if (bookList.Find(p => p.ISBN == ISBN).taken)
                    {
                        Console.WriteLine("Book is already taken!");
                    }
                    else if(bookList.Find(p => p.ISBN == ISBN) != null)
                    {
                        REPEAT_IF_NEEDED:
                        Console.WriteLine("Type your first name:");
                        string first_name = Console.ReadLine();
                        if(first_name.Contains(" ") || first_name == null || first_name.Length < 1)
                        {
                            Console.WriteLine("First Name has to be a one word, please try again.");
                            goto REPEAT_IF_NEEDED;
                        }

                        REPEAT_IF_NEEDED2:
                        Console.WriteLine("Type your last name:");
                        string last_name = Console.ReadLine();
                        if (last_name.Contains(" ") || last_name == null || last_name.Length < 1)
                        {
                            Console.WriteLine("Last Name has to be a one word, please try again.");
                            goto REPEAT_IF_NEEDED2;
                        }

                        string full_name = first_name + " " + last_name;

                        if(bookList.FindAll(p => p.reader == full_name).Count != 3)
                        {
                            REPEAT_IF_NEEDED3:
                            Console.WriteLine("How long will you take it for? (type in the number of days)");
                            int numberOfDays = Convert.ToInt16(Console.ReadLine());
                            if (numberOfDays <= 0 || numberOfDays > 60)
                            {
                                Console.WriteLine("Incorrect number of days, try again.");
                                Console.WriteLine("(min. 1 day, max. 60 days)");
                                goto REPEAT_IF_NEEDED3;
                            }
                            else
                            {
                                string takeDate = DateTime.Now.ToString();
                                string returnDate = DateTime.Now.AddDays(numberOfDays).ToString();

                                bookList.Find(p => p.ISBN == ISBN).taken = true;
                                bookList.Find(p => p.ISBN == ISBN).reader = full_name;
                                bookList.Find(p => p.ISBN == ISBN).takeDate = takeDate;
                                bookList.Find(p => p.ISBN == ISBN).returnDate = returnDate;
                                addBooksToJson();
                                Console.WriteLine("Have fun reading!");
                            }

                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("You already have three books taken!");
                            Console.WriteLine("Return them before taking another one.");
                        }


                    }
                    break;

                case "return":
                    Console.WriteLine("Enter ISBN of the book you wish to return: ");
                    ISBN = Console.ReadLine();
                    if (bookList.Find(p => p.ISBN == ISBN) != null)
                    {
                        var parsedReturnDate = DateTime.Parse(bookList.Find(p => p.ISBN == ISBN).returnDate);
                        if(parsedReturnDate < DateTime.Now)
                        {
                            Console.WriteLine("YOU WERE LATE!!");
                            Console.WriteLine("you will have to pay up now!");
                        }
                        else
                        {
                            Console.WriteLine("Hope you enjoyed reading it!");
                        }
                        
                        bookList.Find(p => p.ISBN == ISBN).reader = null;
                        bookList.Find(p => p.ISBN == ISBN).taken = false;
                        bookList.Find(p => p.ISBN == ISBN).takeDate = null;
                        bookList.Find(p => p.ISBN == ISBN).returnDate = null;

                        addBooksToJson();

                    }
                    else
                    {
                        Console.WriteLine("Book not found!");
                    }


                    break;

                case "add":

                    Console.WriteLine("Enter book title:");
                    string title = Console.ReadLine();
                    Console.WriteLine("Enter book author:");
                    string author = Console.ReadLine();
                    Console.WriteLine("Enter book category:");
                    string category = Console.ReadLine();
                    Console.WriteLine("Enter book language:");
                    string language = Console.ReadLine();
                    Console.WriteLine("Publication date:");
                    string publicationDate = Console.ReadLine();
                    Console.WriteLine("Enter book ISBN:");
                    ISBN = Console.ReadLine();

                    Book b = new Book();
                    b.title = title;
                    b.author = author;
                    b.category = category;
                    b.language = language;
                    b.ISBN = ISBN;
                    b.publicationDate = publicationDate;
                    b.taken = false;
                    bookList.Add(b);

                    addBooksToJson();
                    Console.WriteLine("New book successfully added!");

                    break;

                case "remove":
                    Console.WriteLine("Enter book's ISBN which you want to be removed:");
                    ISBN = Console.ReadLine();
                    if(bookList.Find(p => p.ISBN == ISBN) != null)
                    {
                        listBooks(ISBN, "ISBN");
                        YESNO:
                        Console.WriteLine("Are you sure you want to delete this book? (y/n)");
                        string answer = Console.ReadLine().ToLower();
                        if(answer == "y")
                        {
                            bookList.Remove(bookList.Find(p => p.ISBN == ISBN));
                            addBooksToJson();
                            Console.WriteLine("Book successfully removed!");
                        }
                        else if(answer == "n")
                        {
                            Console.WriteLine("Canceled.");
                        }
                        else
                        {
                            Console.WriteLine("Wrong input, try again.");
                            goto YESNO;
                        }

                    }
                    else
                    {
                        Console.WriteLine("Book not found!");
                    }

                    break;

                default:
                    Console.WriteLine("Wrong command, try again.");
                    break;
            }


        }

        private static void listBooks(string filterValue, string filter)
        {
            var subList = bookList;

            switch (filter)
            {
                case "any":
                    break;

                case "title":
                    subList = bookList.Where(b => b.title == filterValue).ToList();
                    break;

                case "author":
                    subList = bookList.Where(b => b.author == filterValue).ToList();
                    break;

                case "category":
                    subList = bookList.Where(b => b.category == filterValue.ToLower()).ToList();
                    break;

                case "language":
                    subList = bookList.Where(b => b.language == filterValue.ToLower()).ToList();
                    break;

                case "ISBN":
                    subList = bookList.Where(b => b.ISBN == filterValue).ToList();
                    break;

                case "taken":

                    subList = bookList.Where(b => b.taken).ToList();
                    break;

                case "available":
                    subList = bookList.Where(b => b.taken == false).ToList();
                    break;

                default:
                    break;
            }

            if (subList.Count != 0)
            {
                Console.WriteLine();
                Console.WriteLine($"Books found: {subList.Count}");
                foreach (Book book in subList)
                {
                    Console.WriteLine(new string('─', 35));
                    Console.WriteLine("Title: {0}\nAuthor: {1}\nCategory: {2}\nLanguage: {3}\nPublication date: {4}\nISBN: {5}", book.title, book.author, book.category, book.language, book.publicationDate, book.ISBN);
                    if(book.taken)
                    {
                        Console.WriteLine("Taken!");
                    }
                    else
                    {
                        Console.WriteLine("Available!");
                    }
                }
            }
            else
            {
                Console.WriteLine("No books found!");
            }

        }

        private static void addBooksToJson()
        {
            var bookListJson = Newtonsoft.Json.JsonConvert.SerializeObject(bookList);
            File.WriteAllText("../../../books.json", bookListJson);


        }
    }
}
