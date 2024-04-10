using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CountryTrivia.Models;
using System.Collections.Generic;
using System.IO;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.Design.Serialization;
using CountryTrivia.Data;
using System;
using Microsoft.AspNetCore.WebUtilities;
using System.Web;
using System.Diagnostics;
using System.Reflection;
using Humanizer;


namespace CountryTrivia.Controllers
{
    public class CountryController : Controller
    {
        private readonly QARepository _qAs;
        public static List<QA> theQAs;
        public static int theIndex;
        public static int rightAnswers;
        public static int questionCount;
        public static Stopwatch qATime;
        public static int playLevel;
        public static int qANumber;
        public static int rndNumber;
        public CountryController(QARepository allQAs)
        {
            _qAs = allQAs;
        }
        public IActionResult Index(int sLevel)
        {
            QA results = new QA();
            theQAs = _qAs.GetAllQAs().ToList();
            if (theQAs.Count > 0)
            {
                qATime = Stopwatch.StartNew();
                qANumber = 1;
                rndNumber = 1;
                theIndex = 0;
                rightAnswers = 0;
                questionCount = 0;
                Random rnd = new Random();
                do
                {
                    theIndex = rnd.Next(0, theQAs.Count);
                } while (playLevel == 1 && !(theQAs[theIndex].IsFlag));
                results = theQAs[theIndex];
                results.QANumber = qANumber;
                results.RndNumber = rndNumber;
                return View(results);
            } else
            {
                return Content("Error Processing Country Data");
            };
        }
        public IActionResult NextQuestion(string status)
        {
            qANumber++;
            QA results = new QA();
            Random rnd = new Random();
            DateTime qAEnd;
            int _playagain = int.Parse(status);
            //Console.WriteLine(_playagain);
            if (_playagain == 1)
            {
                questionCount = 0;
                rightAnswers = 0;
                qANumber = 1;
                rndNumber++;
                qATime.Restart();
            }
            else
            {
                theQAs.RemoveAt(theIndex);
                questionCount++;
            }
            if (questionCount < 10)
            {
                do
                {
                    theIndex = rnd.Next(0, theQAs.Count);
                } while (playLevel == 1 && !(theQAs[theIndex].IsFlag));
                results = theQAs[theIndex];
            } else
            {
                results.CorrectAnswer = rightAnswers;
                results.Flag = TimeSpan.FromMilliseconds((double)qATime.ElapsedMilliseconds).Humanize();
                results.SelectedAnswer = (int) qATime.ElapsedMilliseconds;
                results.Question = "//SESSION COMPLETE\\";
            };
            results.QANumber = qANumber;
            results.RndNumber = rndNumber;
            if (rndNumber > 10)
            {
                return View("ThankYou", results);
            }
            return View(results);
        }
        public ViewResult ShowAnswer(IFormCollection frm)
        {
            theQAs[theIndex].SelectedAnswer = int.Parse(frm["SelectedAnswer"].ToString());
            if (theQAs[theIndex].SelectedAnswer == theQAs[theIndex].CorrectAnswer)
            {
                rightAnswers++;
            }
            return View(theQAs[theIndex]);
        }

        public ViewResult ThankYou()
        {
            return View("ThankYou", theQAs[theIndex]);
        }

        public ActionResult Easy()
        {
            playLevel = 1;
            return RedirectToAction("Index");
        }
        public ActionResult Advanced()
        {
            playLevel = 2;
            return RedirectToAction("Index");
        }
    }
}
