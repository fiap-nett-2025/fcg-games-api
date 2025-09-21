using Domain.Entities;
using Infra.Data;
using Infra.Repository;
using Microsoft.EntityFrameworkCore;
using System;

namespace Tests.Infra.Repository;

[Trait("Category", "Repository")]
[Trait("Layer", "Infrastructure")]
[Trait("TestType", "Unit")]
public class CartRepositoryTests
{
    private readonly GameDbContext _context;
    private readonly CartRepository _repository;

    public CartRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new GameDbContext(options);
        _repository = new CartRepository(_context);
    }

    [Fact]
    [Trait("Operation", "CRUD")]
    [Trait("Method", "AddAsync")]
    public async Task AddAsync_ValidCart_ShouldAddCartToDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var cart = new Cart(userId);

        // Act
        await _repository.AddAsync(cart);
        var savedCart = await _repository.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(savedCart);
        Assert.Equal(userId, savedCart.UserId);
    }

    [Fact]
    [Trait("Operation", "CRUD")]
    [Trait("Method", "DeleteAsync")]
    [Trait("Scenario", "Success")]
    public async Task DeleteAsync_DeleteExistingCart_ShouldRemoveCartFromDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var cart = new Cart(userId);
        await _repository.AddAsync(cart);
        var cartToDelete = await _repository.GetByIdAsync(userId);
        Assert.NotNull(cartToDelete);
        _context.ChangeTracker.Clear();

        // Act
        await _repository.DeleteAsync(cartToDelete.UserId);

        // Assert
        var deletedCart = await _repository.GetByIdAsync(userId);
        Assert.Null(deletedCart);

    }

    [Fact]
    [Trait("Operation", "Query")]
    [Trait("Method", "GetAllAsync")]
    public async Task GetAllAsync_ShouldReturnAllCarts()
    {
        // Arrange
        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        var userId3 = Guid.NewGuid().ToString();
        var carts = new List<Cart>
        {
            new Cart(userId1),
            new Cart(userId2),
            new Cart(userId3)
        };
        _context.Carts.AddRange(carts);
        _context.SaveChanges();
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(_context.Carts.Count(), result.Count());
        Assert.Contains(result, c => c.UserId == userId1);
        Assert.Contains(result, c => c.UserId == userId2);
        Assert.Contains(result, c => c.UserId == userId3);
    }

    [Fact]
    [Trait("Operation", "CRUD")]
    [Trait("Method", "GetByAsync")]
    public void GetById_ExistingCart_ShouldReturnCart()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var cart = new Cart(userId);
        _context.Carts.Add(cart);
        _context.SaveChanges();

        // Act
        var savedCar = _repository.GetByIdAsync(userId)?.Result;

        // Assert
        Assert.NotNull(savedCar);
        Assert.Equal(userId, savedCar.UserId);
    }

    [Fact]
    [Trait("Operation", "CRUD")]
    [Trait("Method", "UpdateAsync")]
    public async Task UpdateAsync_ExistingCart_ShouldUpdateCartInDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var cart = new Cart(userId);
        _context.Carts.Add(cart);
        _context.SaveChanges();

        // Act
        var cartToUpdate = await _repository.GetByIdAsync(userId);
        cartToUpdate!.GameIds.Add(1);
        await _repository.UpdateAsync(cartToUpdate);
        var updatedCart = await _repository.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(updatedCart);
        Assert.Single(updatedCart.GameIds);
        Assert.Equal(1, updatedCart.GameIds.First());
    }
}
