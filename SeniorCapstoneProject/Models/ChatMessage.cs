using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorCapstoneProject.Models
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public bool IsUser { get; set; }
        public List<string> AvailableTimes { get; set; }
        public string DoctorName { get; set; }
        public bool IsTimeSelection { get; set; }
    }
}
