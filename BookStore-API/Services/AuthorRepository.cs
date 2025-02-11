﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore_API.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        public readonly ApplicationDbContext _db;

        public AuthorRepository(ApplicationDbContext db) => _db = db;

        public async Task<IList<Author>> FindAll()
        {
            var authors = await _db.Authors
                .Include(b => b.Books)
                .ToListAsync();
            return authors;
        }

        public async Task<Author> FindById(int id)
        {
            var author = await _db.Authors
                .Include(b => b.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
            return author;
        }

        public async Task<bool> Create(Author entity)
        {
            await _db.Authors.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _db.Authors.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> Update(Author entity)
        {
            _db.Authors.Update(entity);
            return await Save();
        }

        public async Task<bool> Delete(Author entity)
        {
            _db.Authors.Remove(entity);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }
    }
}
