namespace CountryTrivia.Models
{
    public class Country
    {
        public string name { get; set; }
        public string cca3 { get; set; }
        public string[] capital { get; set; }
        public string region { get; set; }
        public string subregion { get; set; }
        public bool landlocked { get; set; }
        public string[] borders { get; set; }
        public string flag { get; set; }
        public string population { get; set; }
    }
}
