using Microsoft.AspNetCore.Mvc;
using Personal_Website.Models;
using Personal_Website.Utilities;
using System.Data;

namespace Personal_Website.Controllers
{
    public class Admin : Controller
    {
        Database db = new Database();
        IWebHostEnvironment _webHostEnvironment;
        public Admin(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            db = new Database();
        }
        public ActionResult Index()
        {
            Profile profile = new Profile();

            if (db.IsExistRecord("Profile", 1))
            {
                DataTable profile_data = db.ReadData("profile", limit: 1);

                // Fill profile
                profile.Name = profile_data.Rows[0]["Name"].ToString();
                profile.Description = profile_data.Rows[0]["Description"].ToString();
                profile.Email = profile_data.Rows[0]["Email"].ToString();
                profile.Work_Experience = profile_data.Rows[0]["Work_Experience"].ToString();
                profile.Twitter = profile_data.Rows[0]["Twitter"].ToString();
                profile.Instagram = profile_data.Rows[0]["Instagram"].ToString();
                profile.Linkedin = profile_data.Rows[0]["Linkedin"].ToString();
            }
            return View(profile);
        }

        [HttpPost]
        public ActionResult SaveProfile(IFormFile file_image,IFormFile file_resume)
        {
            string image_path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profile_image.png");
            string image_path_in_program = Path.Combine("images", "profile_image.png");
            string resume_file_path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName.ToString() + @"\\wwwroot\\documents\\resume.pdf";
            
            
            if (file_image != null)
            {
                if (file_image.Length > 0)
                {

                    if (System.IO.File.Exists(image_path))
                    {
                        System.IO.File.Delete(image_path);
                    }

                    using (FileStream stream = new FileStream(image_path, FileMode.Create))
                    {
                        file_image.CopyTo(stream);
                        stream.Flush();
                    }
                }
            }

            if ( file_resume != null)
            {
                if (file_resume.Length > 0)
                {
                    if (System.IO.File.Exists(resume_file_path))
                    {
                        System.IO.File.Delete(resume_file_path);
                    }

                    using (FileStream stream = new FileStream(resume_file_path,FileMode.Create))
                    {
                        file_resume.CopyTo(stream);
                        stream.Flush();
                    }
                }
            }
            Profile profile = new Profile()
            {
                Name = Request.Form["name"].ToString(),
                Email = Request.Form["email"].ToString(),
                Instagram = Request.Form["instagram"].ToString(),
                Work_Experience = Request.Form["work_experience"].ToString(),
                Linkedin = Request.Form["linkedin"].ToString(),
                Twitter = Request.Form["twitter"].ToString(),
                Description = Request.Form["description"].ToString(),
                Image = image_path_in_program
                };

            if (db.IsExistRecord("Profile", 1))
            {
                db.UpdateProfile_One(profile);
            }
            else
            {
                db.AddData(profile);
            }
            return Redirect("/admin");
        }
        public ActionResult Experiences()
        {
            List<Experience> experiences = new List<Experience>();
            DataTable experiences_data = db.ReadData("Experience");

            // Fill expriences
            foreach (DataRow row in experiences_data.Rows)
            {
                experiences.Add(
                    new Experience()
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        Company_Name = row["Company_Name"].ToString(),
                        Time_Period = row["Time_Period"].ToString(),
                        Description = row["Description"].ToString(),
                    });
            }

            return View(experiences);
        }

        public ActionResult Educations()
        {
            List<Education> educations = new List<Education>();
            DataTable educations_data = db.ReadData("Education");

            foreach (DataRow row in educations_data.Rows)
            {
                educations.Add(
                    new Education()
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        University = row["University"].ToString(),
                        Time_Period = row["Time_Period"].ToString(),
                        Description = row["Description"].ToString()
                    });
            }

            return View(educations);
        }

        public ActionResult Skills()
        {
            List<Skill> skills = new List<Skill>();
            DataTable skills_data = db.ReadData("Skill");

            foreach (DataRow row in skills_data.Rows)
            {
                skills.Add(
                    new Skill()
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        Name = row["Name"].ToString(),
                        Ability_Rate = Convert.ToInt32(row["Ability_Rate"].ToString())
                    });
            }
            return View(skills);
        }

        public ActionResult Admins()
        {
            List<Models.Admin> admins = new List<Models.Admin>();
            DataTable admins_data = db.ReadData("admin");

            foreach (DataRow row in admins_data.Rows)
            {
                admins.Add(
                    new Models.Admin()
                    {
                        ID = Convert.ToInt32(row["ID"].ToString()),
                        Username = row["Username"].ToString(),
                        Password = row["Password"].ToString()
                    });
            }

            return View(admins);
        }
        public ActionResult Create(string t)
        {
            if (Request.Method == "GET")
            {
                switch (t.ToLower())
                {
                    case "experience":
                        return View("AddExperience");
                    case "education":
                        return View("AddEducation");
                    case "skill":
                        return View("AddSkill");
                    case "admin":
                        return View("AddAdmin");
                    default:
                        return NotFound();
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(string t, bool for_overload)
        {
            switch (t)
            {
                case "experience":
                    Experience experience = new Experience()
                    {
                        Company_Name = Request.Form["company_name"].ToString(),
                        Time_Period = Request.Form["time_period"].ToString(),
                        Description = Request.Form["description"].ToString()
                    };
                    db.AddData(experience);
                    break;
                case "education":
                    Education education = new Education()
                    {
                        University = Request.Form["university"].ToString(),
                        Time_Period = Request.Form["time_period"].ToString(),
                        Description = Request.Form["description"].ToString()
                    };
                    db.AddData(education);
                    break;
                case "skill":
                    Skill skill = new Skill()
                    {
                        Name = Request.Form["name"].ToString(),
                        Ability_Rate = Convert.ToInt32(Request.Form["ability_rate"].ToString())
                    };
                    db.AddData(skill);
                    break;
                case "admin":
                    Models.Admin admin = new Models.Admin()
                    {
                        Username = Request.Form["username"].ToString(),
                        Password = Request.Form["password"].ToString()
                    };
                    db.AddData(admin);
                    break;
                default:
                    return NotFound();
            }
            return RedirectToAction(t + "s");
        }

        public ActionResult Delete(string t, int id, string callback)
        {
            db.DeleteData(t, id);
            return Redirect(callback);
        }

    }
}