using APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;

namespace APP.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        SGDatabaseEntities db = new SGDatabaseEntities();

        public ActionResult Index()
        {

            //var emp =  db.Employee.ToList();
            return View();
        }

        public ActionResult Employee()
        {
            var emp = db.Employee.ToList();
            return View(emp);
        }

        public ActionResult Organization()
        {
            var org = db.Organization.ToList();
            return View(org);
        }
        public ActionResult StudyGroup()
        {
            var sg = db.StudyGroup.ToList();
            return View(sg);
        }
        public ActionResult Teacher()
        {
            var tch = db.Teacher.ToList();
            return View(tch);
        }
        //
        // GET: /Home/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult DeleteEmployee (int id)
        {
            var d = db.Employee.Where(item => item.Id == id).FirstOrDefault();

            return View(d);
        }
        [HttpPost]
        public ActionResult DeleteEmployee(int id,Employee employee)
        {
            Employee emp = db.Employee.First(i => i.Id == id);
            db.Employee.Remove(emp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Home/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            try
            {
                Employee empTable = new Employee();
                empTable.FIO = employee.FIO;
                empTable.OrganizationId = employee.OrganizationId;

                db.Employee.Add(empTable);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Home/Create

        public ActionResult CreateGroup()
        {




            var myList = new List<SelectListItem>();

            var list = db.Teacher.ToList();

            var items = from g in list
                        select new SelectListItem
                            {
                                Value = g.Id.ToString(),
                                Text = g.FIO
                            };
            foreach (var item in items)
                myList.Add(item);

            ViewBag.BOData = myList;

            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult CreateGroup(StudyGroup studygroup, int List)
        {
            try
            {
                StudyGroup sgTable = new StudyGroup();
                sgTable.Title = studygroup.Title;
                sgTable.TeacherId = List;

                db.StudyGroup.Add(sgTable);
                db.SaveChanges();

                return RedirectToAction("StudyGroup");
            }
            catch
            {
                return View();
            }
        }



        //
        // GET: /Home/Edit/5

        public ActionResult EditStudyGroup(int id)
        {



            var d = db.StudyGroup
                            .Include("Employee")
                           .Where(b => b.Id == id).ToList()
                           .FirstOrDefault();


            ViewBag.EmpList = d;

            var esg = db.StudyGroup;
            return View(esg.SingleOrDefault(sg => sg.Id == id));
        }


        //
        // POST: /Home/Edit/5

        [HttpPost]
        public ActionResult EditStudyGroup(int id, StudyGroup obj)
        {
            var esg = db.StudyGroup;
            StudyGroup old = esg.SingleOrDefault(sg => sg.Id == id);
            old.Title = obj.Title;
            db.SaveChanges();
            return RedirectToAction("StudyGroup");


        }

// Удаление студента их группы

        public ActionResult DeleteStudent(int idSt, int idGr)
        {
            var d = db.Employee.Where(item => item.Id == idSt).FirstOrDefault();


            return View(d);
        }

        [HttpPost]
        public ActionResult DeleteStudent(int idSt, int idGr, Employee employee)
        {
            Employee emp = (from e in db.Employee
                            where e.Id == idSt
                            select e).FirstOrDefault<Employee>();

            StudyGroup sg = emp.StudyGroup.Where( g => g.Id == idGr).FirstOrDefault<StudyGroup>();

            emp.StudyGroup.Remove(sg);
            db.SaveChanges();

            return RedirectToAction("StudyGroup");
        }
// Добавление студента в группу


        public ActionResult AddStudent(int id)
        {
            var d = db.StudyGroup.Where(sg => sg.Id == id).FirstOrDefault();
            var organization = db.Organization.Where(org => org.TeacherId == d.Teacher.Id).ToList();
            var employee = db.Employee.ToList();



            var myList = new List<SelectListItem>();

            var or = from g in organization
                     select new SelectListItem
                     {
                         Value = g.Id.ToString(),
                         Text = g.Title
                     };
            foreach (var item in or)
                myList.Add(item);



            ViewBag.Organization = myList;



            ViewBag.Employee = employee;

            return View(d);
        }

    }
}
