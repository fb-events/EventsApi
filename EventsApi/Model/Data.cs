using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApi.Model
{
    public class Data
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime End_time { get; set; }
        public DateTime Start_time { get; set; }
        public Place Place { get; set; }
    }
}
