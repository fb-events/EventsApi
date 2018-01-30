using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsApi.Model;
using Flurl.Http;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventsApi.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        [HttpGet("{groupId}")]
        public async Task<IActionResult> Get(string groupId)
        {
            var response = JsonConvert.DeserializeObject<Response>(await GetData(groupId));
            var calendar = ParseResponseToCalendar(response);
            var encodedCalendar = Encoding.UTF8.GetBytes(calendar);

            return File(encodedCalendar, "application/octet-stream", "kalendorius.ics");
        }

        private async Task<string> GetData(string groupId)
        {
           return await (await $"https://graph.facebook.com/v2.11/{groupId}/events?access_token=EAACEdEose0cBAPTMu24Va7DQ31NYO0mOcoCD5wBjhSoyZASpt6860WNcagnvpIV9RVE2ywC2wZBQI5ZC0VqYgmbniYVObNZBJZBybEnB5I2O6jxmKYOEndGTxreCZAqaeun7YaX6oSL8fZBan55GpsPklC2JMrDtoIpWuCSj2Q2JnHLrEUgKQjAlJYwqOQxZCvMZD&debug=all&format=json&method=get&pretty=0&suppress_http_code=1".GetAsync()).Content.ReadAsStringAsync();
        }

        private string ParseResponseToCalendar(Response response)
        {
            var calendarEvents = new List<CalendarEvent>();

            foreach(var d in response.Data)
            {
                calendarEvents.Add(
                    new CalendarEvent
                    {
                        End = d.End_time == default(DateTime) ? new CalDateTime(d.Start_time.AddHours(1), "Europe/Vilnius") : new CalDateTime(d.End_time, "Europe/Vilnius"),
                        Start = new CalDateTime(d.Start_time, "Europe/Vilnius"),
                        Summary = d.Name,
                        Location = d.Place != null ? d.Place.Name : "",
                        Description = (d.Place != null && d.Place.Location != null) ? $"{d.Place.Location.Street}, {d.Place.Location.City}\n{d.Description}" : d.Description
                    }
                );
            }

            var calendar = new Calendar();
            calendar.Events.AddRange(calendarEvents);

            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar);
        }
    }
}
