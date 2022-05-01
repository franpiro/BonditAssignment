using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TestApp.Exceptions;
using TestApp.Models;

namespace TestApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlightController : ControllerBase
    {
        [HttpPost(Name = "UpdateSuccess")]
        public IActionResult Update()
        {
            List<Flight> flights = new List<Flight>();
            try
            {
                using (var reader = new StreamReader(".\\Data\\Flights.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Flight>();
                    records = records.OrderBy(x => (x.Departure - x.Arrival).TotalMinutes);
                    records = records.ToList();
                    int successTotal = 0;
                    foreach (var record in records)
                    {
                        if ((record.Departure - record.Arrival).TotalMinutes >= 180 && successTotal <= 20)
                        {
                            record.Success = "success";
                            successTotal++;
                        }
                        else
                            record.Success = "fail";
                    }
                    records = records.OrderBy(x => x.Arrival);
                    flights = records.ToList();
                }
            }
            catch (FileNotFoundException ex)
            {
                return new InternalServerErrorObjectResult("Missing Flights.csv file");
            }
            catch (HeaderValidationException ex)
            {
                return new InternalServerErrorObjectResult("Flight.csv file header wrong format. Header format should be: FlightId,Arrival,Departure,Success");
            }
            catch (ReaderException ex)
            {
                return new InternalServerErrorObjectResult("There was a problem with the body of Flight.csv");
            }
            catch (Exception ex)
            {
                return new InternalServerErrorObjectResult("Something bad happend");
            }
            
            using (var writer = new StreamWriter(".\\Data\\Flights.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<Flight>();
                csv.NextRecord();
                foreach (var record in flights)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }

            return new OkResult();
        }

        [HttpGet(Name = "{Id}")]
        public IActionResult Get(string Id)
        {
            using (var reader = new StreamReader(".\\Data\\Flights.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Flight>();
                var flight = records.FirstOrDefault(x => x.FlightId == Id);
                if (flight == null)
                    return new NotFoundObjectResult($"There's no flight with id {Id}");
                else
                    return new OkObjectResult(flight);
            }
        }
    }
}
