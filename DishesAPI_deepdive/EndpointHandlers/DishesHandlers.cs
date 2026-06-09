using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesAPI.Extensions;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DishesAPI.EndpointHandlers;

public static class DishesHandlers
{
    public static async Task<Ok<IEnumerable<DishDto>>> GetDishesAsync(DishesDbContext dishesDbContext,
        ILogger<DishDto> logger,
        ClaimsPrincipal claimsPrincipal,
        string? name)
    {
        logger.LogInformation("Getting dishes, authenticated: {IsAuthenticated}",
            claimsPrincipal.Identity?.IsAuthenticated);

        return TypedResults.Ok((await dishesDbContext.Dishes
            .Where(d => name == null || d.Name.Contains(name))
            .ToListAsync())
                .ToDishDtoList());
    }

    public static async Task<Results<NotFound, Ok<DishDto>>> GetDishByIdAsync(DishesDbContext dishesDbContext,
    Guid dishId)
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(dishEntity?.ToDishDto());
    }

    public static async Task<Results<NotFound, Ok<DishDto>>> GetDishByNameAsync(DishesDbContext dishesDbContext,
        string dishName)
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);
        if (dishEntity == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(dishEntity?.ToDishDto());
    }

    public static async Task<CreatedAtRoute<DishDto>> CreateDishAsync(DishesDbContext dishesDbContext,
    DishForCreationDto dishForCreationDto)
    {
        var dishEntity = dishForCreationDto.ToDish();
        dishesDbContext.Add(dishEntity);
        await dishesDbContext.SaveChangesAsync();
        var dishToReturn = dishEntity.ToDishDto();

        return TypedResults.CreatedAtRoute(
             dishToReturn,
             "GetDish",
             new
             {
                 dishId = dishToReturn.Id
             });
    }

    public static async Task<Results<NotFound, NoContent>> UpdateDishAsync(DishesDbContext dishesDbContext,
        Guid dishId,
        DishForUpdateDto dishForUpdateDto)
    {
        var dishEntity = await dishesDbContext.Dishes
            .FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null)
        {
            return TypedResults.NotFound();
        }
        dishEntity.UpdateFromDto(dishForUpdateDto);
        await dishesDbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteDishAsync(
    DishesDbContext dishesDbContext,
    Guid dishId)
    {
        var dishEntity = await dishesDbContext.Dishes
            .FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null)
        {
            return TypedResults.NotFound();
        }
        dishesDbContext.Dishes.Remove(dishEntity);
        await dishesDbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
