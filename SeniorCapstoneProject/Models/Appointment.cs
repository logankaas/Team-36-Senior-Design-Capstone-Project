using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorCapstoneProject.Models
{
    public class Appointment
    {
        public string Id { get; set; }
        public string DoctorId { get; set; } // Reference to Doctor
        public string DoctorName { get; set; }
        public string TimeRange { get; set; }
        public DateTime Date { get; set; }
        public string UserEmail { get; set; }
        public string Location { get; set; }
    }
}
