using System.Diagnostics.Eventing.Reader;

namespace CountryTrivia.Models
{
    public class QA
    {
        public string Question { get; set; }
        public bool IsFlag { get; set; }
        public bool TrueFalse {  get; set; }
        public string[] Answers { get; set; }
        public int CorrectAnswer { get; set; }
        public string Flag { get; set; }
        public int SelectedAnswer { get; set; }
        public int QANumber { get; set; }
        public int RndNumber { get; set; }

    }
}
