using CasaRutterCards.Entities;
using Microsoft.EntityFrameworkCore;

namespace CasaRutterCards.Infra;

public class CardsRepository : ICardsRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Card> _set;

    public CardsRepository(AppDbContext context)
    {
        _context = context;
        _set = context.Cards;
    }

    public async Task<List<Card>> GetCardByName(string name)
    {
        return await _set.Where(card => name.ToLower().Trim() == card.NamePortuguese.ToLower().Trim() 
                                     || name.ToLower().Trim() == card.NameEnglish.ToLower().Trim())
            .Include(card => card.CardItems)
            .ThenInclude(cardItem => cardItem.Edition)
            .Include(card => card.CardItems)
            .ThenInclude(cardItem => cardItem.Prices)
            .ToListAsync();
    }

    public async Task<Card?> GetCardByRutterCode(int rutterCode)
    {
        return await _set.Where(card => card.RutterCode == rutterCode)
            .Include(card => card.CardItems)
            .ThenInclude(cardItem => cardItem.Edition)
            .Include(card => card.CardItems)
            .ThenInclude(cardItem => cardItem.Prices)
            .FirstOrDefaultAsync();
    }
}

public interface ICardsRepository
{
    Task<Card?> GetCardByRutterCode(int rutterCode);
    Task<List<Card>> GetCardByName(string name);
}