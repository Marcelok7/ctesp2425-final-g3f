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
