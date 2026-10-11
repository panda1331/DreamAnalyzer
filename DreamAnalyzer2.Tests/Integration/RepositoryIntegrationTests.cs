using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Enums;
using DreamAnalyzer2.Infrastructure.Data;
using DreamAnalyzer2.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Integration
{
    public class RepositoryIntegrationTests 
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task UserRepository_AddAndGet_Works()
        {
            using var ctx = CreateContext();
            var repo = new UserRepository(ctx);

            var user = new User("testuser", "test@example.com", RoleType.User);
            user.SetPasswordHash("hash");

            await repo.AddAsync(user);
            await ctx.SaveChangesAsync();

            var found = await repo.GetByIdAsync(user.Id);
            found.Should().NotBeNull();
            found!.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task UserRepository_GetByEmail_ReturnsUser()
        {
            using var ctx = CreateContext();
            var repo = new UserRepository(ctx);

            var user = new User("testuser", "test@example.com", RoleType.User);
            user.SetPasswordHash("hash");
            await repo.AddAsync(user);
            await ctx.SaveChangesAsync();

            var found = await repo.GetByEmailAsync("test@example.com");
            found.Should().NotBeNull();
            found!.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task DreamRepository_GetByUserId_ReturnsOnlyUserDreams()
        {
            using var ctx = CreateContext();
            var repo = new DreamRepository(ctx);
            var userId = Guid.NewGuid();
            var otherId = Guid.NewGuid();

            await repo.AddAsync(new Dream(userId, "Dream 1", "Content 1", DateTime.UtcNow));
            await repo.AddAsync(new Dream(userId, "Dream 2", "Content 2", DateTime.UtcNow));
            await repo.AddAsync(new Dream(otherId, "Dream 3", "Content 3", DateTime.UtcNow));
            await ctx.SaveChangesAsync();

            var dreams = await repo.GetByUserIdAsync(userId);

            dreams.Should().HaveCount(2);
            dreams.Should().OnlyContain(d => d.UserId == userId);
        }

        [Fact]
        public async Task DreamRepository_Delete_RemovesDream()
        {
            using var ctx = CreateContext();
            var repo = new DreamRepository(ctx);
            var dream = new Dream(Guid.NewGuid(), "Title", "Content", DateTime.UtcNow);

            await repo.AddAsync(dream);
            await ctx.SaveChangesAsync();

            await repo.DeleteAsync(dream.Id);
            await ctx.SaveChangesAsync();

            var found = await repo.GetByIdAsync(dream.Id);
            found.Should().BeNull();
        }

        [Fact]
        public async Task SymbolRepository_AddAndGetByName_Works()
        {
            using var ctx = CreateContext();
            var repo = new SymbolRepository(ctx);

            var symbol = new DreamSymbol("кошка", "Независимость", "Peaceful");
            await repo.AddAsync(symbol);
            await ctx.SaveChangesAsync();

            var found = await repo.GetByNameAsync("кошка");
            found.Should().NotBeNull();
            found!.Name.Should().Be("кошка");
        }

        [Fact]
        public async Task SymbolRepository_GetAll_ReturnsAllSymbols()
        {
            using var ctx = CreateContext();
            var repo = new SymbolRepository(ctx);

            await repo.AddAsync(new DreamSymbol("кошка", "Независимость", "Peaceful"));
            await repo.AddAsync(new DreamSymbol("огонь", "Страсть", "Anxious"));
            await ctx.SaveChangesAsync();

            var symbols = await repo.GetAllAsync();

            symbols.Should().HaveCount(2);
        }

        [Fact]
        public async Task SymbolRepository_Delete_RemovesSymbol()
        {
            using var ctx = CreateContext();
            var repo = new SymbolRepository(ctx);
            var symbol = new DreamSymbol("вода", "Эмоции", "Peaceful");

            await repo.AddAsync(symbol);
            await ctx.SaveChangesAsync();

            await repo.DeleteAsync(symbol.Id);
            await ctx.SaveChangesAsync();

            var found = await repo.GetByIdAsync(symbol.Id);
            found.Should().BeNull();
        }
    }
}
