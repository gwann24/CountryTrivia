using CountryTrivia.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Formats.Tar;
using System.IO;

namespace CountryTrivia.Data
{
    public class QARepository
    {
        private readonly IConfiguration _configuration;
        public QARepository(IConfiguration iConfig)
        {
            _configuration = iConfig;
        }
        public async Task<string> QueryAPI()
        {
            string body;
            bool localData = false;
            string cntryData = Directory.GetCurrentDirectory() + @"/wwwroot/data/P9QXMJXX.json";
            if (File.Exists(cntryData))
            {
                DateTime cntryDataDT = (File.GetLastWriteTime(cntryData)).AddDays(30);
                if (cntryDataDT >  DateTime.Now)
                {
                    localData = true;
                }
            };
            if (localData)
            {
                //Console.WriteLine("Got it Local!");
                body = File.ReadAllText(cntryData);
            }
            else
            {
                //Console.WriteLine("Got it from Web!");
                string apikey = _configuration.GetValue<string>("apiKey");
                //Console.WriteLine(apikey);
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://country-facts.p.rapidapi.com/all"),
                    //RequestUri = new Uri("https://localhost:7210/Country.json"),
                    Headers =
                    {
                        { "X-RapidAPI-Key", apikey },
                        { "X-RapidAPI-Host", "country-facts.p.rapidapi.com" },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    //body = await response.Content.ReadAsStringAsync();
                    body = await response.Content.ReadAsStringAsync();
                }
                File.WriteAllText(cntryData, body);
            };
            return body;
        }
        public IEnumerable<QA> GetAllQAs()
        {
            List<Country> countries = new List<Country>();
            JArray _details;
            List<QA> qAs = new List<QA>();
            Random rnd = new Random();
            int qAIndex = 0;
            int cIndex = 0;
            int bIndex = 0;
            try
            {
                _details = JArray.Parse(QueryAPI().Result);
                for (int i = 0; i < _details.Count; i++)
                {
                    var tempCntry = new Country();
                    tempCntry.name = (string)_details[i]["name"]["common"];
                    tempCntry.cca3 = (string)_details[i]["cca3"];
                    tempCntry.capital = _details[i]["capital"].Select(x => (string)x).ToArray();
                    tempCntry.region = (string)_details[i]["region"];
                    tempCntry.subregion = (string)_details[i]["subregion"];
                    tempCntry.landlocked = (bool)_details[i]["landlocked"];
                    tempCntry.borders = _details[i]["borders"].Select(x => (string)x).ToArray();
                    tempCntry.flag = (string)_details[i]["flag"];
                    tempCntry.population = (string)_details[i]["population"];
                    //if (tempCntry.capital.Length > 1)
                    //{
                    //Console.WriteLine(tempCntry.name);
                    //};
                    countries.Add(tempCntry);
                }
                var tempAList = "";
                var cntryMatch = new Country();
                //var cntryName = "";
                List<int> rndNumList = new List<int>();
                List<string> bCountries = new List<string>();
                foreach (var country in countries)
                {
                    for (int y = 1; y < 6; y++)
                    {
                        var tempQA = new QA();
                        tempQA.IsFlag = false;
                        tempQA.TrueFalse = false;
                        tempQA.Flag = country.flag;
                        qAIndex = rnd.Next(0, 4);
                        tempQA.CorrectAnswer = qAIndex;
                        tempAList = "";
                        rndNumList.Clear();
                        switch (y)
                        {
                            case 1:
                                tempQA.Question = "Which country is represented by the flag pictured?";
                                tempQA.IsFlag = true;
                                for (int x = 0; x < 4; x++)
                                {
                                    if (x != qAIndex)
                                    {
                                        do
                                        {
                                            cIndex = rnd.Next(0, countries.Count);
                                        } while (countries[cIndex].name == country.name || rndNumList.Contains(cIndex));
                                        tempAList += countries[cIndex].name;
                                        rndNumList.Add(cIndex);
                                    }
                                    else
                                    {
                                        tempAList += country.name;
                                    };
                                    if (x < 3)
                                    {
                                        tempAList += "|";
                                    }
                                };
                                //Console.WriteLine(tempAList);
                                break;
                            case 2:
                                if (country.capital.Length == 1)
                                {
                                    tempQA.Question = $"Which city is the capital of {country.name}?";
                                    for (int x = 0; x < 4; x++)
                                    {
                                        if (x != qAIndex)
                                        {
                                            do
                                            {
                                                cIndex = rnd.Next(0, countries.Count);
                                            } while (countries[cIndex].name == country.name || rndNumList.Contains(cIndex));
                                            tempAList += countries[cIndex].capital[0];
                                            rndNumList.Add(cIndex);
                                        }
                                        else
                                        {
                                            tempAList += country.capital[0];
                                        };
                                        if (x < 3)
                                        {
                                            tempAList += "|";
                                        }
                                    };
                                };
                                break;
                            case 3:
                                tempQA.Question = $"What is the population of {country.name}?";
                                for (int x = 0; x < 4; x++)
                                {
                                    if (x != qAIndex)
                                    {
                                        do
                                        {
                                            cIndex = rnd.Next(0, countries.Count);
                                        } while (countries[cIndex].name == country.name || rndNumList.Contains(cIndex));
                                        tempAList += countries[cIndex].population;
                                        rndNumList.Add(cIndex);
                                    }
                                    else
                                    {
                                        tempAList += country.population;
                                    };
                                    if (x < 3)
                                    {
                                        tempAList += "|";
                                    }
                                };
                                break;
                            case 4:
                                bCountries.Clear();
                                foreach (var bCountry in country.borders)
                                {
                                    //Console.WriteLine(bCountry);
                                    cntryMatch = countries.Find(x => x.cca3 == bCountry);
                                    if (cntryMatch != null)
                                    {
                                        bCountries.Add(cntryMatch.name);
                                    };
                                    //Console.WriteLine(cntryName);
                                };
                                //Console.WriteLine(country.name);
                                //Console.WriteLine(bCountries.Count);
                                //foreach (var myCntry in bCountries)
                                //{
                                //    Console.WriteLine(myCntry);
                                //};
                                tempQA.Question = $"Which country borders {country.name}?";
                                for (int x = 0; x < 4; x++)
                                {
                                    if (x != qAIndex)
                                    {
                                        do
                                        {
                                            cIndex = rnd.Next(0, countries.Count);
                                        } while (countries[cIndex].name == country.name || rndNumList.Contains(cIndex) || bCountries.Contains(countries[cIndex].name));
                                        tempAList += countries[cIndex].name;
                                        rndNumList.Add(cIndex);
                                    }
                                    else
                                    {
                                        if (bCountries.Count == 0)
                                        {
                                            tempAList += "None";
                                        }
                                        else if (bCountries.Count == 1)
                                        {
                                            tempAList += country.borders[0];
                                        }
                                        else
                                        {
                                            bIndex = rnd.Next(0, bCountries.Count);
                                            tempAList += bCountries[bIndex];
                                        }
                                    };
                                    if (x < 3)
                                    {
                                        tempAList += "|";
                                    }
                                };
                                break;
                            case 5:
                                tempQA.TrueFalse = true;
                                tempQA.Question = $"Is {country.name} a land locked country?";
                                tempQA.CorrectAnswer = country.landlocked ? 0 : 1;
                                tempAList = "True|False||";
                                break;
                        };
                        if (tempAList != "")
                        {
                            tempQA.Answers = tempAList.Split('|').Select(x => x).ToArray();
                            qAs.Add(tempQA);
                        };
                    };
                };
            } catch
            {
                //Do Nothing
            };
            //Console.WriteLine(body);
            return qAs;
        }
    }
}
