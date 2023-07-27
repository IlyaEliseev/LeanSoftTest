using LeanSoftTest;
using Microsoft.Extensions.Configuration;

var conf = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
var fileName = conf.GetValue<String>("FileName"); 
if (fileName is null)
{
    throw new ArgumentNullException(nameof(fileName));
}

var fullPath = Path.Combine(path, fileName);
if (fileName is null)
{
    throw new ArgumentNullException(nameof(fileName));
}

var personsCount = conf.GetValue<Int32>("PersonsCount");
var maxChildsCount = conf.GetValue<Int32>("MaxChildsCount");
var maxCreditCardsCount = conf.GetValue<Int32>("MaxCreditCardsCount");
var maxPhonesCount = conf.GetValue<Int32>("MaxPhonesCount");

var persons = FixturePersons
    .SetParameters(personsCount, maxChildsCount, maxCreditCardsCount, maxPhonesCount)
    .GetPersons();

var service = new LeanSoftService(persons, fullPath);

File.WriteAllText(fullPath, String.Empty);

await service.WriteToFile(persons);

Array.Clear(persons);

persons = await service.ReadFromFile(persons);

service.DisplayToConsole(persons);