using System.Text.Json;

namespace LeanSoftTest
{
    public class LeanSoftService
    {
        private readonly Person[]? persons;
        private readonly string path;

        public LeanSoftService(Person[] persons, string path)
        {
            this.persons = persons;
            this.path = path;
            CalculationInformation = CalculatePersonsInformation(persons);
        }

        public CalculatedInformation? CalculationInformation { get; }

        public async Task WriteToFile(Person[] persons)
        {
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync(
                    fs,
                    persons,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
            }
        }

        public async Task<Person[]?> ReadFromFile(Person[]? persons)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                persons = await JsonSerializer.DeserializeAsync<Person[]>(
                    fs,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
            }

            return persons;
        }

        public void DisplayToConsole(Person[]? persons)
        {
            var information = CalculatePersonsInformation(persons);
            if (information is null)
            {
                Console.WriteLine($"Persons count: {0} " +
                              $"Credit card count: {0} " +
                              $"Average Child Age: {0}");
                return;
            }

            Console.WriteLine($"Persons count: {information.PersonsCount} " +
                              $"Credit card count: {information.CreditCardsCount} " +
                              $"Average Child Age: {information.AverageChildAgeYear}");
        }

        private CalculatedInformation? CalculatePersonsInformation(Person[]? persons)
        {
            if (persons is null)
            {
                return null;
            }

            var personsCount = persons.Length;
            var creditCardsCount = persons.SelectMany(x => x.CreditCardNumbers).Count();

            var averageChildAgeTimestamp = (Int64)persons.
                SelectMany(x => x.Children
                    .Select(x => x.BirthDate))
                    .Average();

            var averageChildAgeYear =
                DateTime.UtcNow.Year - DateTime.UnixEpoch.AddSeconds(averageChildAgeTimestamp).Year;

            return new CalculatedInformation
            {
                PersonsCount = personsCount,
                CreditCardsCount = creditCardsCount,
                AverageChildAgeYear = averageChildAgeYear
            };
        }
    }
}
