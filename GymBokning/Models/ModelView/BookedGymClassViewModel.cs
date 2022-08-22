using System.ComponentModel.DataAnnotations;

namespace GymBokning.Models.ModelView
{
    public class BookedGymClassViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Start time")]
        public DateTime StartTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Duration { get; set; }

        public string Description { get; set; }

        [Display(Name = "Booked")]
        public bool ApplicationUserGymClassIsBooked { get; set; }
    }
}
