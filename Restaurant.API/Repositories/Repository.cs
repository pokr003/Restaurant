using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Types;

namespace Restaurant.API.Repositories;

public class Repository<T>(RestaurantDbContext context, ITransactional transactional) : IRepository<T> where T : class
{
    private readonly RestaurantDbContext _context = context;
    private readonly ITransactional _transactional = transactional;

    public List<T> SelectAll() => [.. _context.Set<T>()];

    public List<TResult> SelectAll<TResult>() => [.. _context.Set<T>().ProjectToType<TResult>()];

    public async Task<List<T>> SelectAllAsync() =>
        await _context.Set<T>().ToListAsync();

    public async Task<List<TResult>> SelectAllAsync<TResult>() =>
        await _context.Set<T>().ProjectToType<TResult>().ToListAsync();

    public T? SelectById(Guid id) => _context.Set<T>().Find(id);

    public async Task<T?> SelectByIdAsync(Guid id) =>
        await _context.Set<T>().FindAsync(id);

    public IQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression) =>
        _context.Set<T>().Select(expression);

    public IQueryable<T> Where(Expression<Func<T, bool>> expression) =>
       _context.Set<T>().Where(expression);

    public TResult? WhereFirst<TResult>(Expression<Func<T, bool>> expression) =>
        _context.Set<T>().Where(expression).ProjectToType<TResult>().FirstOrDefault();

    public async Task<TResult?> WhereFirstAsync<TResult>(Expression<Func<T, bool>> expression) =>
        await _context.Set<T>().Where(expression).ProjectToType<TResult>().FirstOrDefaultAsync();

    public List<TResult> Where<TResult>(Expression<Func<T, bool>> expression) =>
        [.. _context.Set<T>().Where(expression).ProjectToType<TResult>()];

    public async Task<List<TResult>> WhereAsync<TResult>(Expression<Func<T, bool>> expression) =>
        await _context.Set<T>().Where(expression).ProjectToType<TResult>().ToListAsync();

    public T? FirstOrDefault(Expression<Func<T, bool>> expression) => Where(expression).FirstOrDefault();

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression) => await Where(expression).FirstOrDefaultAsync();

    public T? Add(T value) =>
        _transactional.UseTransaction((context) =>
        {
            context.Entry(value).State = EntityState.Added;
            context.SaveChanges();

            return value;
        });

    public async Task<T?> AddAsync(T value) =>
        await _transactional.UseTransactionAsync(async (context) =>
        {
            context.Entry(value).State = EntityState.Added;
            await context.SaveChangesAsync();
            return value;
        });


    public bool Update(T value) =>
        _transactional.UseTransaction((context) =>
        {
            context.Entry(value).State = EntityState.Modified;
            context.SaveChanges();

            return true;
        }, valueWhenError: false);


    public async Task<bool> UpdateAsync(T value) =>
        await _transactional.UseTransactionAsync(async (context) =>
        {
            context.Entry(value).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return true;
        }, valueWhenError: false);

    public bool Remove(T value) =>
        _transactional.UseTransaction((context) =>
        {
            context.Entry(value).State = EntityState.Deleted;
            context.SaveChanges();

            return true;
        }, valueWhenError: false);

    public async Task<bool> RemoveAsync(T value) =>
        await _transactional.UseTransactionAsync(async (context) =>
        {
            context.Entry(value).State = EntityState.Deleted;
            await context.SaveChangesAsync();

            return true;
        }, valueWhenError: false);
}