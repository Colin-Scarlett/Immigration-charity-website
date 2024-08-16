using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Immigration_charity_website.Models;
namespace Immigration_charity_website.Controllers
{
    public class ServicesController : Controller
    {
        // GET: Services
        public ActionResult Index()
        {
            var services = new List<Service>
            {
                new Service
                {
                    Title = "Medical Services",
                    Description = "We provide information about available clinics, hospitals, and health insurance to ensure you receive the best possible care.",
                    Details = new List<string>
                    {
                        "Clinics: A list of local clinics available for immediate healthcare needs.",
                        "Hospitals: Information about nearby hospitals for emergency and specialized care.",
                        "Health Insurance: Guidance on how to obtain and use health insurance."
                    }
                },
                new Service
                {
                    Title = "Psychological Support",
                    Description = "We offer counseling services and support groups to help you manage stress, anxiety, and other mental health concerns.",
                    Details = new List<string>
                    {
                        "Counseling Services: Access to professional counselors for one-on-one support.",
                        "Support Groups: Join support groups to share experiences and receive support from peers."
                    }
                },
                new Service
                {
                    Title = "Legal Aid",
                    Description = "We provide guidance on immigration law and access to legal resources to help you navigate legal challenges.",
                    Details = new List<string>
                    {
                        "Immigration Law: Information and assistance on immigration laws and procedures.",
                        "Legal Resources: Access to legal resources and services to help with your legal needs."
                    }
                }
            };

            return View(services);
        }
    }
}