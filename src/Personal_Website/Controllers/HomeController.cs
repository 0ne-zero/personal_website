using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Personal_Website.Models;
using Personal_Website.Utilities;
using System.Data;
using System.Security.Claims;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp.Pdf;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUglify.Html;
using OpenQA.Selenium.Chrome;

namespace Personal_Website.Controllers
{
    public class HomeController : Controller
    {
        Database db;
        IViewRenderService _viewRenderService;
        public HomeController(IViewRenderService viewRenderService)
        {
            db = new Database();
            _viewRenderService = viewRenderService;
        }
        [Route("/")]
        [Route("/home")]
        [Route("/home/index")]
        public ViewResult Index()
        {
            return View(Load_All_Information());
        }

        private All_Information Load_All_Information()
        {
            // Get data in DataTable
            DataTable profile_data = db.ReadData("Profile");
            DataTable educations_data = db.ReadData("Education");
            DataTable experiences_data = db.ReadData("Experience");
            DataTable skills_data = db.ReadData("Skill");

            // Data in Model
            Profile profile = new Profile();
            List<Education> education = new List<Education>();
            List<Experience> experience = new List<Experience>();
            List<Skill> skill = new List<Skill>();

            // Fill Models
            foreach (DataRow row in profile_data.Rows)
            {
                profile.Name = row["Name"].ToString();
                profile.Email = row["Email"].ToString();
                profile.Work_Experience = row["Work_Experience"].ToString();
                profile.Instagram = row["Instagram"].ToString();
                profile.Twitter = row["Twitter"].ToString();
                profile.Linkedin = row["Linkedin"].ToString();
                profile.Description = row["Description"].ToString();
                profile.Image = row["Image"].ToString();
            }
            foreach (DataRow row in educations_data.Rows)
            {
                education.Add(
                    new Education()
                    {
                        University = row["University"].ToString(),
                        Time_Period = row["Time_Period"].ToString(),
                        Description = row["Description"].ToString()
                    });
            }
            foreach (DataRow row in experiences_data.Rows)
            {
                experience.Add(
                    new Experience()
                    {
                        Company_Name = row["Company_Name"].ToString(),
                        Time_Period = row["Time_Period"].ToString(),
                        Description = row["Description"].ToString()
                    });
            }
            foreach (DataRow row in skills_data.Rows)
            {
                skill.Add(
                    new Skill()
                    {
                        Name = row["Name"].ToString(),
                        Ability_Rate = Convert.ToInt32(row["Ability_Rate"].ToString()),
                    });
            }

            All_Information all_Information = new All_Information()
            {
                Profile = profile,
                Education = education,
                Experience = experience,
                Skill = skill
            };
            return all_Information;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(bool just_for_overload)
        {
            Models.Admin admin_model = new Models.Admin()
            {
                Username = Request.Form["username"].ToString(),
                Password = Request.Form["password"].ToString()
            };

            if (!db.IsExistAdmin(admin_model))
            {
                ViewData["Error"] = "Information is wrong...";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim("isAdmin", "true"),
            };
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties()
            {
                IsPersistent = false
            };

            HttpContext.SignInAsync(principal, properties);
            return Redirect("/admin");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Login");
        }

        [Route("/Home/NotFound/{code:int}")]
        public IActionResult HandleError(int code)
        {
            return View("NotFound");
        }

        [Route("/Home/DownloadPdf")]
        [Route("/DownloadPdf")]
        public IActionResult DownloadPdf()
        {
            // Get Index html code and convert it to pdf file and finally return that pdf file.
            // But this method dosen't works properly

            //All_Information view_model = Load_All_Information();
            //var site_html = _viewRenderService.RenderToStringAsync("Index", view_model).Result.ToString();
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //byte[] pdf_bytes = ConvertHtmlToPdf(site_html).Result;
            //return File(pdf_bytes, "application/octet-stream",$"{view_model.Profile.Name}-resume.pdf");


            // Return /wwwroot/documents/resume.pdf
            string resume_file_path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName.ToString() + @"\\wwwroot\\documents\\resume.pdf";
            if (System.IO.File.Exists(resume_file_path))
            {
                byte[] file_data = System.IO.File.ReadAllBytes(resume_file_path);

                // File name
                DataTable profile_data = db.ReadData("Profile");
                
                string upload_file_name = profile_data.Rows[0]["Name"].ToString();
                upload_file_name = upload_file_name.Replace(" ", "-");
                
                return File(file_data, "application/force-download", $"{upload_file_name}-resume.pdf");
            }
            return View("Index");
        }

        private async Task<byte[]> ConvertHtmlToPdf(string html)
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "ApplicationName");
            Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, $"{Guid.NewGuid()}.html");
            await System.IO.File.WriteAllTextAsync(filePath, html);

            var driverOptions = new ChromeOptions();
            // In headless mode, PDF writing is enabled by default (tested with driver major version 85)
            //driverOptions.AddArgument("headless");
            using var driver = new ChromeDriver("./", driverOptions);
            driver.Navigate().GoToUrl(filePath);

            // Output a PDF of the first page in A4 size at 90% scale
            var printOptions = new Dictionary<string, object>
            {
                { "paperWidth", 210 / 25.4 },
                { "paperHeight", 297 / 25.4 },
                { "scale", 0.9 },
                { "pageRanges", "1" }
            };
            var printOutput = driver.ExecuteChromeCommandWithResult("Page.printToPDF", printOptions) as Dictionary<string, object>;
            var pdf = Convert.FromBase64String(printOutput["data"] as string);

            System.IO.File.Delete(filePath);

            return pdf;
        }

    }
}