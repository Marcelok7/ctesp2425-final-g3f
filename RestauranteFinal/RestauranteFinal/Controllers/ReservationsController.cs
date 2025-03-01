using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteFinal.Models;


namespace RestaurantReservations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationContext _context;

        public ReservationsController(ReservationContext context)
        {
            _context = context;
        }

        // GET: /reservations
        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _context.Reservations
                .Where(r => !r.IsDeleted)
                .ToListAsync();

            return Ok(reservations);
        }

        // GET: /reservations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _context.Reservations
                .Where(r => r.Id == id && !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        // POST: /reservations
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            // Calcular o hor�rio de t�rmino da nova reserva
            var startTime = reservation.ReservationDate;
            var endTime = startTime.Add(reservation.ReservationTime);

            // Verificar se j� existe uma reserva para a mesma mesa dentro do hor�rio
            bool conflictExists = _context.Reservations
             .Where(r => r.TableNumber == reservation.TableNumber && !r.IsDeleted)
             .AsEnumerable() // Transforma os dados em mem�ria (Ineficiente para grandes tabelas!)
             .Any(r =>
         (r.ReservationDate < endTime && r.ReservationDate.Add(r.ReservationTime) > startTime));

            if (conflictExists)
            {
                return BadRequest("J� existe uma reserva para esta mesa neste hor�rio.");
            }

            // Adicionar e salvar nova reserva
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
        }

        // PUT: /reservations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation reservation)
        {
            var existingReservation = await _context.Reservations
                .Where(r => r.Id == id && !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (existingReservation == null)
                return NotFound();

            existingReservation.CustomerName = reservation.CustomerName;
            existingReservation.ReservationDate = reservation.ReservationDate;
            existingReservation.ReservationTime = reservation.ReservationTime;
            existingReservation.TableNumber = reservation.TableNumber;
            existingReservation.NumberOfPeople = reservation.NumberOfPeople;

            _context.Entry(existingReservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /reservations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteReservation(int id)
        {
            var reservation = await _context.Reservations
                .Where(r => r.Id == id && !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (reservation == null)
                return NotFound();

            reservation.IsDeleted = true;

            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: /reservations?date={date}
        [HttpGet("search")]
        public async Task<IActionResult> GetReservationsByDate([FromQuery] DateTime date)
        {
            var reservations = await _context.Reservations
                .Where(r => r.ReservationDate.Date == date.Date && !r.IsDeleted)
                .ToListAsync();

            return Ok(reservations);
        }
    }
}
