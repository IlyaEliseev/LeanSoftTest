using AutoFixture;
using LeanSoftTest;

namespace LeanSoftServiceTests
{
    public class LeanSoftServiceTests : IAsyncLifetime
    {
        private readonly Fixture fixture;
        private readonly string fullPath;
        private readonly string pathDirectory;

        public LeanSoftServiceTests()
        {
            fixture = new Fixture();
            pathDirectory = @"E:\projects\LeanSoftTest\LeanSoftServiceTests\TestDirectory\";
            var fileName = "Persons.json";
            fullPath = Path.Combine(pathDirectory, fileName);
        }

        public Person[] GeneratePersons(
            Int32 personsCount,
            Int32 childsCount,
            Int32 creditCardsCount,
            Int64 timestamp)
        {
            var childs = fixture
                .Build<Child>()
                .With(x => x.BirthDate, timestamp)
                .CreateMany(childsCount)
                .ToArray();

            var creditCards = fixture
                .Build<string>()
                .CreateMany(creditCardsCount)
                .ToArray();

            var persons = fixture
                .Build<Person>()
                .With(x => x.Children, childs)
                .With(x => x.CreditCardNumbers, creditCards)
                .CreateMany(personsCount)
                .ToArray();

            return persons;
        }

        [Fact]
        public async Task Calculate_persons_information()
        {
            // arrange
            var personsCount = 10;
            var childsCount = 10;
            var creditCardsCount = 10;
            var allPersonsCreditCardsCount = personsCount * creditCardsCount;
            var timestamp = (Int64)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
            var averageChildAgeYear = DateTime.UtcNow.Year - DateTime.UnixEpoch.AddSeconds(timestamp).Year;
            var persons = GeneratePersons(personsCount, childsCount, creditCardsCount, timestamp);

            // act
            var service = new LeanSoftService(persons, fullPath);
            var calculatedInformation = service.CalculationInformation;

            // assert
            Assert.Equal(personsCount, calculatedInformation.PersonsCount);
            Assert.Equal(allPersonsCreditCardsCount, calculatedInformation.CreditCardsCount);
            Assert.Equal(averageChildAgeYear, calculatedInformation.AverageChildAgeYear);
        }

        [Fact]
        public async Task Write_to_file()
        {
            // arrange
            var personsCount = 10;
            var childsCount = 10;
            var creditCardsCount = 10;
            var timestamp = (Int64)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
            var persons = GeneratePersons(personsCount, childsCount, creditCardsCount, timestamp);

            // act
            File.WriteAllText(fullPath, String.Empty);
            var service = new LeanSoftService(persons, fullPath);
            await service.WriteToFile(persons);
            var files = Directory.GetFiles(pathDirectory);

            // assert
            Assert.True(files.Length == 1);
        }

        [Fact]
        public async Task Read_from_file()
        {
            // arrange
            var personsCount = 10;
            var childsCount = 10;
            var creditCardsCount = 10;
            var allPersonsCreditCardsCount = personsCount * creditCardsCount;
            var timestamp = (Int64)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
            var averageChildAgeYear = DateTime.UtcNow.Year - DateTime.UnixEpoch.AddSeconds(timestamp).Year;
            var persons = GeneratePersons(personsCount, childsCount, creditCardsCount, timestamp);

            // act
            File.WriteAllText(fullPath, String.Empty);
            var service = new LeanSoftService(persons, fullPath);
            await service.WriteToFile(persons);
            Array.Clear(persons);
            persons = await service.ReadFromFile(persons);

            var serviceAfterReading = new LeanSoftService(persons, fullPath);
            var personsInformation = serviceAfterReading.CalculationInformation;

            // assert
            Assert.NotNull(persons);
            Assert.Equal(personsCount, personsInformation.PersonsCount);
            Assert.Equal(allPersonsCreditCardsCount, personsInformation.CreditCardsCount);
            Assert.Equal(averageChildAgeYear, personsInformation.AverageChildAgeYear);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await ClearDirectory(pathDirectory);
        }

        private async Task ClearDirectory(string pathDirectory)
        {
            var files = new DirectoryInfo(pathDirectory).GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }
        }
    }
}