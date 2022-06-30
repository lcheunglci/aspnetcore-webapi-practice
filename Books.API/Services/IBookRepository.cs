﻿namespace Books.API.Services
{
    public interface IBookRepository
    {
        IEnumerable<Entities.Book> GetBooks();

        //Entities.Book GetBook(Guid id);

        Task<IEnumerable<Entities.Book>> GetBooksAsync();

        Task<Entities.Book> GetBookAsync(Guid id);

        void AddBook(Entities.Book bookToAdd);

        Task<bool> SaveChangesAsync();
    }
}
