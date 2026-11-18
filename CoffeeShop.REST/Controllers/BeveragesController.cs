using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Infrastructure.Services;
using CoffeeShop.REST.Models;
using DbBeverage = CoffeeShop.Infrastructure.Models.BeverageModel;

namespace CoffeeShop.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BeveragesController : ControllerBase
{
    private readonly ICrudServiceAsync<DbBeverage> _beverageService;

    public BeveragesController(ICrudServiceAsync<DbBeverage> beverageService)
    {
        _beverageService = beverageService;
    }

    /// <summary>
    /// Отримати список всіх напоїв або частину списку за допомогою пагінації
    /// </summary>
    /// <param name="page">Номер сторінки (від 1)</param>
    /// <param name="amount">Кількість елементів на сторінці</param>
    /// <response code="200">Успішне отримання списку напоїв</response>
    /// <response code="400">Некоректні параметри пагінації</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BeverageModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BeverageModel>>> GetAll([FromQuery] int? page = null, [FromQuery] int? amount = null)
    {
        if (page.HasValue || amount.HasValue)
        {
            int pageNumber = page ?? 1;
            int pageSize = amount ?? 10;

            if (pageNumber < 1 || pageSize <= 0)
            {
                return BadRequest(new { message = "Номер сторінки має бути >= 1, а кількість елементів > 0." });
            }

            var pagedEntities = await _beverageService.ReadAllAsync(pageNumber, pageSize);
            return Ok(pagedEntities.Select(b => b.ToDto()));
        }

        var entities = await _beverageService.ReadAllAsync();
        return Ok(entities.Select(b => b.ToDto()));
    }

    /// <summary>
    /// Отримати напій за ідентифікатором
    /// </summary>
    /// <param name="id">ID напою</param>
    /// <response code="200">Напій знайдений</response>
    /// <response code="404">Напій не знайдений</response>
    [HttpGet("{id:int}", Name = "GetBeverageById")]
    [ProducesResponseType(typeof(BeverageModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeverageModel>> GetById(int id)
    {
        var entity = await _beverageService.ReadAsync(id);
        if (entity == null)
        {
            return NotFound(new { message = $"Напій з ID {id} не знайдений." });
        }

        return Ok(entity.ToDto());
    }

    /// <summary>
    /// Створити новий напій
    /// </summary>
    /// <param name="model">Дані для створення напою</param>
    /// <response code="201">Напій успішно створений</response>
    /// <response code="400">Невалідні дані</response>
    [HttpPost]
    [ProducesResponseType(typeof(BeverageModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BeverageModel>> Create([FromBody] CreateBeverageModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = model.ToEntity();
        bool created = await _beverageService.CreateAsync(entity);

        if (!created)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Помилка при збереженні напою у БД." });
        }

        var dto = entity.ToDto();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Оновити дані існуючого напою
    /// </summary>
    /// <param name="id">ID напою</param>
    /// <param name="model">Оновлені дані</param>
    /// <response code="200">Дані успішно оновлені</response>
    /// <response code="400">Невалідні дані</response>
    /// <response code="404">Напій не знайдений</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BeverageModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeverageModel>> Update(int id, [FromBody] UpdateBeverageModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingEntity = await _beverageService.ReadAsync(id);
        if (existingEntity == null)
        {
            return NotFound(new { message = $"Напій з ID {id} не знайдений для оновлення." });
        }

        model.UpdateEntity(existingEntity);
        bool updated = await _beverageService.UpdateAsync(existingEntity);

        if (!updated)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Помилка при збереженні оновлень напою." });
        }

        return Ok(existingEntity.ToDto());
    }

    /// <summary>
    /// Видалити напій за ідентифікатором
    /// </summary>
    /// <param name="id">ID напою</param>
    /// <response code="204">Напій успішно видалений</response>
    /// <response code="404">Напій не знайдений</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var existingEntity = await _beverageService.ReadAsync(id);
        if (existingEntity == null)
        {
            return NotFound(new { message = $"Напій з ID {id} не знайдений для видалення." });
        }

        bool deleted = await _beverageService.RemoveAsync(existingEntity);
        if (!deleted)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Помилка при видаленні напою з БД." });
        }

        return NoContent();
    }
}
