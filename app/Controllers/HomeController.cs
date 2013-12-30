using APP.Models;
using System;
using System.Linq;
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
       
        public ActionResult DeleteEmployee(int id)
        {
            var d = db.Employee.Where(item => item.Id == id).FirstOrDefault();

            return View(d);
        }
        [HttpPost]
        public ActionResult DeleteEmployee(int id, Employee employee)
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




            //var myList = new List<SelectListItem>();

            //var list = db.Teacher.ToList();

            //var items = from g in list
            //            select new SelectListItem
            //                {
            //                    Value = g.Id.ToString(),
            //                    Text = g.FIO
            //                };
            //foreach (var item in items)
            //    myList.Add(item);


            ViewBag.List = new SelectList(db.Teacher, "Id", "FIO");

            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult CreateGroup(StudyGroup studygroup, int List = 0)
        {
            try
            {
                if (studygroup.Title != null & List != 0)
                {
                    StudyGroup sgTable = new StudyGroup();
                    sgTable.Title = studygroup.Title;
                    sgTable.TeacherId = List;

                    db.StudyGroup.Add(sgTable);
                    db.SaveChanges();
                    return RedirectToAction("EditStudyGroup", sgTable);
                }
                return RedirectToAction("CreateGroup");
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

            StudyGroup sg = emp.StudyGroup.Where(g => g.Id == idGr).FirstOrDefault<StudyGroup>();

            emp.StudyGroup.Remove(sg);
            db.SaveChanges();

            return RedirectToAction("EditStudyGroup", sg);
        }


        public ActionResult AddStudent(int id)
        {
            var d = db.StudyGroup.Where(sg => sg.Id == id).FirstOrDefault();
            LoadOrganization("0", id);

            return View(d);
        }


        [ValidateInput(false)]
        [AcceptVerbs("POST")]
        public ActionResult AddStudent(int id, StudyGroup studygroup)
        {
            string empId;
            var d = db.StudyGroup.Where(sg => sg.Id == id).FirstOrDefault();
            var Organization = Request["OrgId"];
            LoadOrganization(Organization, id);
            try
            {
                LoadEmployee(Convert.ToInt32(Organization), studygroup.Id);
                empId = Request["EmployeeId"];
                int orgEmp = 0;
                if (empId != null)
                {
                    int eId = Convert.ToInt32(empId);
                    var _emp = db.Employee.Where(x => x.Id == eId).FirstOrDefault();
                    orgEmp = _emp.OrganizationId;
                }
                int idSt = Convert.ToInt32(empId);
                if (empId != null & Convert.ToInt32(Organization) == orgEmp)
                {

                    Employee emp = (from e in db.Employee
                                    where e.Id == idSt
                                    select e).FirstOrDefault<Employee>();

                    StudyGroup sg = db.StudyGroup.Where(g => g.Id == id).FirstOrDefault<StudyGroup>();

                    emp.StudyGroup.Add(sg);
                    db.SaveChanges();
                    return RedirectToAction("EditStudyGroup", studygroup);

                }
            }
            catch
            {

            }
            return View(d);
        }

        public void LoadOrganization(string selOrg, int id)
        {

            var teacher = db.StudyGroup.Include("Teacher").Where(stg => stg.Id == id).FirstOrDefault();

            var query = db.Organization.Where(x => x.TeacherId == teacher.TeacherId).Select(c => new { c.Id, c.Title });

            ViewData["Organization"] = new SelectList(query.AsEnumerable(), "Id", "Title", selOrg);

        }

        public void LoadEmployee(int id, int stdgrpId)
        {
            var EmplList = EmployeeNotInGroup(stdgrpId);
            var query1 = EmplList.Where(w => w.OrganizationId == id).Select(c => new { c.Id, c.FIO });

            ViewData["Employee"] = new SelectList(query1.AsEnumerable(), "Id", "FIO", 3);
        }


        //Немного отрефакторил костыль, что был в прошлой версии, через использование Except
        public IQueryable<Employee> EmployeeNotInGroup(int idGr)
        {
            var d = db.StudyGroup
            .Include("Employee")
            .Where(b => b.Id == idGr).ToList()
            .FirstOrDefault();

            var students = d.Employee.ToList();
            var employee = db.Employee.ToList();

            var notSt = employee.AsQueryable().Except(students);

            return notSt;
        }
    }
}
