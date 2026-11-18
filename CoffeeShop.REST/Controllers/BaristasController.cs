using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Infrastructure.Services;
using CoffeeShop.REST.Models;
using DbBarista = CoffeeShop.Infrastructure.Models.BaristaModel;

namespace CoffeeShop.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BaristasController : ControllerBase
{
    private readonly ICrudServiceAsync<DbBarista> _baristaService;

    public BaristasController(ICrudServiceAsync<DbBarista> baristaService)
    {
        _baristaService = baristaService;
    }

    /// <summary>
    /// Отримати список всіх бариста або частину списку за допомогою пагінації
    /// </summary>
    /// <param name="page">Номер сторінки (від 1)</param>
    /// <param name="amount">Кількість елементів на сторінці</param>
    /// <response code="200">Успішне отримання списку бариста</response>
    /// <response code="400">Некоректні параметри пагінації</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BaristaModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BaristaModel>>> GetAll([FromQuery] int? page = null, [FromQuery] int? amount = null)
    {
        if (page.HasValue || amount.HasValue)
        {
            int pageNumber = page ?? 1;
            int pageSize = amount ?? 10;

            if (pageNumber < 1 || pageSize <= 0)
            {
                return BadRequest(new { message = "Номер сторінки має бути >= 1, а кількість елементів > 0." });
            }

            var pagedEntities = await _baristaService.ReadAllAsync(pageNumber, pageSize);
            return Ok(pagedEntities.Select(b => b.ToDto()));
        }

        var entities = await _baristaService.ReadAllAsync();
        return Ok(entities.Select(b => b.ToDto()));
    }

    /// <summary>
    /// Отримати бариста за ідентифікатором
    /// </summary>
    /// <param name="id">ID бариста</param>
    /// <response code="200">Бариста знайдений</response>
    /// <response code="404">Бариста не знайдений</response>
    [HttpGet("{id:int}", Name = "GetBaristaById")]
    [ProducesResponseType(typeof(BaristaModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaristaModel>> GetById(int id)
    {
        var entity = await _baristaService.ReadAsync(id);
        if (entity == null)
        {
            return NotFound(new { message = $"Бариста з ID {id} не знайдений." });
        }

        return Ok(entity.ToDto());
    }

    /// <summary>
    /// Створити нового бариста
    /// </summary>
    /// <param name="model">Дані для створення бариста</param>
    /// <response code="201">Бариста успішно створений</response>
    /// <response code="400">Невалідні дані</response>
    [HttpPost]
    [ProducesResponseType(typeof(BaristaModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaristaModel>> Create([FromBody] CreateBaristaModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = model.ToEntity();
        bool created = await _baristaService.CreateAsync(entity);

        if (!created)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Помилка при створенні бариста у БД." });
        }

        var dto = entity.ToDto();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Оновити дані існуючого бариста
    /// </summary>
    /// <param name="id">ID бариста</param>
    /// <param name="model">Оновлені дані</param>
    /// <response code="200">Дані успішно оновлені</response>
    /// <response code="400">Невалідні дані</response>
    /// <response code="404">Бариста не знайдений</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BaristaModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaristaModel>> Update(int id, [FromBody] UpdateBaristaModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingEntity = await _baristaService.ReadAsync(id);
        if (existingEntity == null)
        {
            return NotFound(new { message = $"Бариста з ID {id} не знайдений для оновлення." });
        }

        model.UpdateEntity(existingEntity);
        bool updated = await _baristaService.UpdateAsync(existingEntity);

        if (!updated)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Помилка при збереженні оновлень бариста." });
        }

        return Ok(existingEntity.ToDto());
    }

    /// <summary>
    /// Видалити бариста за ідентифікатором
    /// </summary>
    /// <param name="id">ID бариста</param>
    /// <response code="204">Бариста успішно видалений</response>
    /// <response code="404">Бариста не знайдений</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var existingEntity = await _baristaService.ReadAsync(id);
        if (existingEntity == null)
        {
            return NotFound(new { message = $"Бариста з ID {id} не знайдений для видалення." });
        }

        bool deleted = await _baristaService.RemoveAsync(existingEntity);
        if (!deleted)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Помилка при видаленні бариста з БД." });
        }

        return NoContent();
    }
}
