﻿using System.Text.Json.Serialization;

public class Person
{
    public Int32 Id { get; set; }
    public Guid TransportId { get; set; }
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public Int32 SequenceId { get; set; }
    public String[] CreditCardNumbers { get; set; }
    public Int32 Age { get; set; }
    public String[] Phones { get; set; }
    public Int64 BirthDate { get; set; }
    public Double Salary { get; set; }
    public Boolean IsMarred { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; set; }
    public Child[] Children { get; set; }
}

public class Child
{
    public Int32 Id { get; set; }
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public Int64 BirthDate { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; set; }
}

public enum Gender
{
    Male,
    Female
}
