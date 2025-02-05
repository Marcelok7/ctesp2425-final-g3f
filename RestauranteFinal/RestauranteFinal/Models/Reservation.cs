namespace RestauranteFinal.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int TableNumber { get; set; }
        public int NumberOfPeople { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false; // Soft delete flag
    }
}
