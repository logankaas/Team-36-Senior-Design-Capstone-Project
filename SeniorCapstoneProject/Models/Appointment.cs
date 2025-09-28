using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorCapstoneProject.Models
{
    public class Appointment
    {
        public string DoctorName { get; set; }
        public DateTime Date { get; set; }
        public string TimeRange { get; set; }
        public string UserEmail { get; set; }
    }
}
