﻿using APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APP.Controllers
{
    public class GroupController : Controller
    {
        SGDatabaseEntities db = new SGDatabaseEntities();
        //
        // GET: /Group/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Group/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Group/Create

        public ActionResult CreateGroup()
        {
            return View();
        }

        //
        // POST: /Group/Create

        [HttpPost]
        public ActionResult CreateGroup(StudyGroup group)
        {
            try
            {
                StudyGroup sg = new StudyGroup();
                sg.Title = group.Title;
                sg.TeacherId = group.TeacherId;

                db.StudyGroup.Add(sg);
                db.SaveChanges();

                return RedirectToAction("CreateGroup");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Group/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Group/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Group/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Group/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
