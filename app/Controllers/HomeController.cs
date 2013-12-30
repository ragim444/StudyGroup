using APP.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace APP.Controllers
{
    public class HomeController : Controller
    {
        readonly SGDatabaseEntities db = new SGDatabaseEntities();

        public ActionResult Index()
        {
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
            var d = db.Employee.FirstOrDefault(item => item.Id == id);

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
        
        public ActionResult CreateEmployee()
        {
            ViewBag.List = new SelectList(db.Organization, "Id", "Title");
            return View();
        }

        [HttpPost]
        public ActionResult CreateEmployee(Employee employee, int list = 0)
        {
            try
            {
                var empTable = new Employee();
                empTable.FIO = employee.FIO;
                empTable.OrganizationId = list;

                db.Employee.Add(empTable);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        public ActionResult CreateGroup()
        {
            ViewBag.List = new SelectList(db.Teacher, "Id", "FIO");

            return View();
        }

        [HttpPost]
        public ActionResult CreateGroup(StudyGroup studygroup, int list = 0)
        {
            try
            {
                if (studygroup.Title != null & list != 0)
                {
                    var sgTable = new StudyGroup();
                    sgTable.Title = studygroup.Title;
                    sgTable.TeacherId = list;

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
        
        [HttpPost]
        public ActionResult EditStudyGroup(int id, StudyGroup obj)
        {
            var esg = db.StudyGroup;
            if (obj.Title != null)
            {
                StudyGroup old = esg.SingleOrDefault(sg => sg.Id == id);
                if (old != null) old.Title = obj.Title;
                db.SaveChanges();
                return RedirectToAction("StudyGroup");
            }
            return RedirectToAction("EditStudyGroup", id);

        }

        // Удаление студента их группы

        public ActionResult DeleteStudent(int idSt, int idGr)
        {
            var d = db.Employee.FirstOrDefault(item => item.Id == idSt);

            return View(d);
        }

        [HttpPost]
        public ActionResult DeleteStudent(int idSt, int idGr, Employee employee)
        {
            Employee emp = (from e in db.Employee
                            where e.Id == idSt
                            select e).FirstOrDefault<Employee>();

            if (emp != null)
            {
                var sg = emp.StudyGroup.FirstOrDefault(g => g.Id == idGr);

                emp.StudyGroup.Remove(sg);
                db.SaveChanges();

                return RedirectToAction("EditStudyGroup", sg);
            }
            return RedirectToAction("EditStudyGroup", idGr);
        }
        
        public ActionResult AddStudent(int id)
        {
            var d = db.StudyGroup.FirstOrDefault(sg => sg.Id == id);
            LoadOrganization("0", id);

            return View(d);
        }


        [ValidateInput(false)]
        [AcceptVerbs("POST")]
        public ActionResult AddStudent(int id, StudyGroup studygroup)
        {
            var d = db.StudyGroup.FirstOrDefault(sg => sg.Id == id);
            var organization = Request["OrgId"];
            LoadOrganization(organization, id);
            try
            {
                LoadEmployee(Convert.ToInt32(organization), studygroup.Id);
                string empId = Request["EmployeeId"];
                int orgEmp = 0;
                if (empId != null)
                {
                    int eId = Convert.ToInt32(empId);
                    var emp = db.Employee.FirstOrDefault(x => x.Id == eId);
                    orgEmp = emp.OrganizationId;
                }
                int idSt = Convert.ToInt32(empId);
                if (empId != null & Convert.ToInt32(organization) == orgEmp)
                {
                    var emp = (from e in db.Employee
                               where e.Id == idSt
                               select e).FirstOrDefault<Employee>();
                    var sg = db.StudyGroup.FirstOrDefault(g => g.Id == id);
                    emp.StudyGroup.Add(sg);
                    db.SaveChanges();

                    return RedirectToAction("EditStudyGroup", studygroup);
                }
            }
            catch (Exception)
            {

            }
            return View(d);
        }

        public void LoadOrganization(string selOrg, int id)
        {

            var teacher = db.StudyGroup.Include("Teacher").FirstOrDefault(stg => stg.Id == id);
            var query = db.Organization.Where(x => x.TeacherId == teacher.TeacherId).Select(c => new { c.Id, c.Title });
            ViewData["Organization"] = new SelectList(query.AsEnumerable(), "Id", "Title", selOrg);
        }

        public void LoadEmployee(int id, int stdgrpId)
        {
            var emplList = EmployeeNotInGroup(stdgrpId);
            var query1 = emplList.Where(w => w.OrganizationId == id).Select(c => new { c.Id, c.FIO });

            ViewData["Employee"] = new SelectList(query1.AsEnumerable(), "Id", "FIO", 3);
        }

        //Немного отрефакторил костыль, что был в прошлой версии, через использование Except
        public IQueryable<Employee> EmployeeNotInGroup(int idGr)
        {
            var d = db.StudyGroup
                .Include("Employee").FirstOrDefault(b => b.Id == idGr).Employee.ToList();
            var employee = db.Employee.ToList();
            var notSt = employee.AsQueryable().Except(d);

            return notSt;
        }
    }
}
