using Newtonsoft.Json;

namespace Task_Managament_System.Models
{
    public class UserDump
    {
        public int id { get; set; }
        [JsonProperty("firstName")]
        public string? first_name { get; set; }
        [JsonProperty("lastName")]
        public string? last_name { get; set; }
        public int age { get; set; }
        public string? gender { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        [JsonProperty("birthDate")]
        public DateTime birth_date { get; set; }
        public string? image { get; set; }
        public Address? address { get; set; }
        public Company? company { get; set; }
        public Bank? bank { get; set; }
    }

    public class Address
    {
        public string? address { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? stateCode { get; set; }
        public string? postalCode { get; set; }
        public string? country { get; set; }
    }

    public class Company
    {
        public string? department { get; set; }
        public string? name { get; set; }
        public string? title { get; set; }
        public Address? address { get; set; }
    }

    public class UserDumpResponse
    {
        public List<UserDump>? users { get; set; }
    }

        public class Bank
    {
        public string? cardExpire { get; set; }
        public string? cardNumber { get; set; }
        public string? cardType { get; set; }
        public string? currency { get; set; }
        public string? iban { get; set; }
    }

    public class UserDumpRS
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public List<UserDump>? users { get; set; }
    }
}
