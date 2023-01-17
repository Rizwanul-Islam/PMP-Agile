﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMPReportingApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMPReportingApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        List<MasterAgreementDetails> agreement = new List<MasterAgreementDetails>();
        List<MasterAgreement> agreements = new List<MasterAgreement>();
        List<AgreementDetails> agreementDetails = new List<AgreementDetails>();



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            await FetchMasterAgreementsDataFromAPIAsync();
            await FetchMasterAgreementDetailsDataFromAPIAsync();

            agreements = FetchMasterAgreementsDataFromAPIAsync().Result;
            agreementDetails = FetchMasterAgreementDetailsDataFromAPIAsync().Result;

            List<MasterAgreementDetails> agreement = GetMasterAgreementList();
            ViewBag.MasterAgreementCount = agreements.Count();

            ViewBag.ClosedAgreements = agreement.Select(x => x.Status == "closed").ToList().Count();
            ViewBag.OpenAgreements = agreement.Select(x => x.Status != "closed").ToList().Count();


            List<AgreementPosition> positions = new List<AgreementPosition>
            {
                new AgreementPosition(1, "Team Lead", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(2, "Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(3, "Senior Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(4, "Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(5, "Project manager", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(6, "Team Lead", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(7, "Project manager", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(8, "Project manager", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(9, "Team Lead", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(10, "Senior Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(11, "Senior Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(12, "Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(13, "Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(14, "Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(15, "Software Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(16, "QA Engineer", "Development", "Development", "", DateTime.Now),
                new AgreementPosition(17, "QA Engineer", "Development", "Development", "", DateTime.Now)
            };



            List<DoughnutData> chartData = new List<DoughnutData>();

            foreach (var position in agreementDetails.GroupBy(info => info.name)
                        .Select(group => new {
                            name = group.Key,
                            Count = group.Count(),
                            percentage = (int)Math.Round((double)(100 * group.Count()) / agreementDetails.Count())
                        })
                        .OrderBy(x => x.name))
            {
                Console.WriteLine("{0} {1}", position.name, position.Count, position.percentage);
                chartData.Add(new DoughnutData { xValue = position.name, yValue = position.Count, text = position.percentage.ToString()+"%" });
            }


            //foreach(AgreementDetails)

            //{
            //    new DoughnutData { xValue =  "Project Manager", yValue = 13, text = "13%" },
            //    new DoughnutData { xValue =  "Team Lead", yValue = 20, text = "20%" },
            //    new DoughnutData { xValue =  "Software Engineer", yValue = 35, text = "35%" },
            //    new DoughnutData { xValue =  "QA Engineer", yValue = 20, text = "20%" },
            //    new DoughnutData { xValue =  "Others", yValue = 12, text = "12%" }
            //};



            ViewBag.dataSource = chartData;

            List<Provider> providers = new List<Provider>
            {
                new Provider(1, "Alaska Software Inc.","Software",15,3,2,4.2,"",DateTime.Now),
                new Provider(1, "MetaComp GmbH","Software",20,6,3,4.8,"",DateTime.Now),
                new Provider(1, "AcumenCog pvt ltd.","Software",18,4,1,3.3,"",DateTime.Now),
                new Provider(1, "iTester Inc.","Software",10,1,3,4.0,"",DateTime.Now),
                new Provider(1, "Apps Germany GmbH","Software",22,6,4,3.2,"",DateTime.Now),
                new Provider(1, "ComfNet Solutions GmbH","Software",11,2,1,3.5,"",DateTime.Now),
                new Provider(1, "Zucchetti Germany GmbH","Software",13,3,2,4.7,"",DateTime.Now),
                new Provider(1, "OneStream Software ltd.","Software",10,1,1,3.8,"",DateTime.Now)
            };

            ViewBag.Providers = providers;

            return View();
        }

        private async Task<List<MasterAgreement>> FetchMasterAgreementsDataFromAPIAsync()
        {
            string baseUrl = "https://provider-management-platform-server.onrender.com/agreements";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                agreements = JsonConvert.DeserializeObject<List<MasterAgreement>>(data);
                            }
                            else
                            {
                                //Console.WriteLine("NO Data----------");
                            }
                        }
                    }
                }

                return agreements;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        private async Task<List<AgreementDetails>> FetchMasterAgreementDetailsDataFromAPIAsync()
        {
            string baseUrl = "https://provider-management-platform-server.onrender.com/agreementsdetails";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                agreementDetails = JsonConvert.DeserializeObject<List<AgreementDetails>>(data);
                            }
                            else
                            {
                                //Console.WriteLine("NO Data----------");
                            }
                        }
                    }
                }

                return agreementDetails;
            }
            catch (Exception exception)
            {
                return null;
            }
        }


        public IActionResult MasterAgreements()
        {
            agreements = FetchMasterAgreementsDataFromAPIAsync().Result;
            ViewBag.MasterAgreements = agreements;
            return View();
        }

        private static List<MasterAgreementDetails> GetMasterAgreementList()
        {
            List<MasterAgreementDetails> agreement = new List<MasterAgreementDetails>();
            if (agreement.Count() == 0)
            {
                int code = 10000;
                for (int i = 1; i < 5; i++)
                {
                    agreement.Add(new MasterAgreementDetails(code + 1, "Web Development Team", "Team", 1, "Published", new DateTime(2022, 12, 12), new DateTime(2023, 12, 31), "Frankfurt University of Applied Sciences", "Franfurt Am Main", "Kirchgasse 6", new DateTime(2022, 12, 21)));
                    agreement.Add(new MasterAgreementDetails(code + 1, "Software Development Team", "Team", 1, "Published", new DateTime(2022, 12, 20), new DateTime(2023, 07, 10), "Mobile hubs limited", "Hamburg", "Kuntola", new DateTime(2022, 12, 21)));
                    agreement.Add(new MasterAgreementDetails(code + 1, "Web Development Team2", "Single", 1, "Published", new DateTime(2022, 12, 05), new DateTime(2023, 02, 21), "Simplexhub limited", "Berlin", "Mehedi Hasan", new DateTime(2022, 12, 21)));
                    agreement.Add(new MasterAgreementDetails(code + 1, "Web Development Team3", "Team", 1, "Published", new DateTime(2022, 12, 05), new DateTime(2023, 04, 25), "Acer Malaysia", "Malaysia", "Rizwan", new DateTime(2022, 12, 21)));
                    agreement.Add(new MasterAgreementDetails(code + 1, "Web Development Team4", "Team", 2, "Published", new DateTime(2022, 12, 01), new DateTime(2023, 12, 20), "Deloitte", "Franfurt Am Main", "Kirchgasse 6", new DateTime(2022, 12, 21)));
                    code += 5;
                }
            }

            return agreement;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class DoughnutData
    {
        public string xValue;
        public double yValue;
        public string text;
    }
}
