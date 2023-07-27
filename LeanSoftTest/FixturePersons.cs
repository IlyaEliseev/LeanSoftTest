using AutoFixture;
using System.Text;

namespace LeanSoftTest
{
    public class FixturePersons
    {
        private readonly Int32 personsCount;
        private readonly Int32 maxChildsCount;
        private readonly Int32 maxCreditCardsCount;
        private readonly Int32 maxPhonesCount;
        private readonly Fixture fixture;
        private readonly Random rnd;

        private FixturePersons(
            Int32 personsCount,
            Int32 maxChildsCount,
            Int32 maxCreditCardsCount,
            Int32 maxPhonesCount)
        {
            fixture = new Fixture();
            rnd = new Random();
            this.personsCount = personsCount;
            this.maxChildsCount = maxChildsCount;
            this.maxCreditCardsCount = maxCreditCardsCount;
            this.maxPhonesCount = maxPhonesCount;
        }

        public static FixturePersons SetParameters(
            Int32 personsCount,
            Int32 maxChildsCount,
            Int32 maxCreditCardsCount,
            Int32 maxPhonesCount)
        {
            if (personsCount < 0)
            {
                throw new ArgumentException($"{nameof(personsCount)} less than zero.");
            }

            if (maxChildsCount < 0)
            {
                throw new ArgumentException($"{nameof(maxChildsCount)} less than zero.");
            }

            if (maxCreditCardsCount < 0)
            {
                throw new ArgumentException($"{nameof(maxCreditCardsCount)} less than zero.");
            }

            if (maxPhonesCount < 0)
            {
                throw new ArgumentException($"{nameof(maxPhonesCount)} less than zero.");
            }

            return new FixturePersons(
                personsCount,
                maxChildsCount,
                maxCreditCardsCount,
                maxPhonesCount);
        }

        public Person[] GetPersons()
        {
            var persons = new Person[personsCount];
            for (Int32 i = 0; i < personsCount; i++)
            {
                var timestamp = GenerateRandomPOSIXTimestamp();
                var childs = GenerateRandomChilds(rnd.Next(0, maxChildsCount));
                var creditCardNumbers = GenerateRandomCreditCards(rnd.Next(0, maxCreditCardsCount));
                var phonesCounts = GenerateRandomPhones(rnd.Next(0, maxPhonesCount));
                var age = GetAgePerson(timestamp);

                var person = fixture.Build<Person>()
                    .With(x => x.Age, age)
                    .With(x => x.BirthDate, timestamp)
                    .With(x => x.Children, childs)
                    .With(x => x.CreditCardNumbers, creditCardNumbers)
                    .With(x => x.CreditCardNumbers, creditCardNumbers)
                    .With(x => x.Phones, phonesCounts)
                    .Create();

                persons[i] = person;
            }

            return persons;
        }

        private String[] GenerateRandomCreditCards(Int32 count)
        {
            var creditCards = new String[count];
            for (Int32 i = 0; i < count; i++)
            {
                creditCards[i] = $"{rnd.Next(1000, 10000)}" +
                                 $" {rnd.Next(1000, 10000)}" +
                                 $" {rnd.Next(1000, 10000)} {rnd.Next(1000, 10000)}";
            }

            return creditCards;
        }

        private Child[] GenerateRandomChilds(Int32 count)
        {
            var childs = new Child[count];
            for (Int32 i = 0; i < count; i++)
            {
                var timestamp = GenerateRandomPOSIXTimestamp();
                var child = fixture.Build<Child>()
                    .With(x => x.BirthDate, timestamp)
                    .Create();

                childs[i] = child;
            }

            return childs;
        }

        private Int64 GenerateRandomPOSIXTimestamp()
        {
            var start = new DateTime(1923, 1, 1);
            var end = new DateTime(2000, 1, 1);
            var range = (end - start).Days;
            var date = start.AddDays(rnd.Next(range))
                .AddHours(rnd.Next(0, 24))
                .AddSeconds(rnd.Next(0, 60))
                .ToUniversalTime();

            var timestamp = (Int64)date.Subtract(DateTime.UnixEpoch).TotalSeconds;

            return timestamp;
        }

        private String[] GenerateRandomPhones(int phonesCount)
        {
            var phones = new String[phonesCount];
            var sequince = "123456789";
            var phoneLength = 11;
            for (Int32 i = 0; i < phonesCount; i++)
            {
                var builder = new StringBuilder();
                var phone = builder
                    .Append(Enumerable.Repeat(sequince, phoneLength)
                    .Select(x => x[rnd.Next(0, sequince.Length)])
                    .ToArray())
                    .ToString();

                phones[i] = phone;
            }

            return phones;
        }

        private int GetAgePerson(Int64 timestamp)
        {
            var age = DateTime.UtcNow.Year - DateTime.UnixEpoch.AddSeconds(timestamp).Year;
            return age;
        }
    }
}
