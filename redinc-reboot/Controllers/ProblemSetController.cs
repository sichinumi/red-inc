﻿using core.Modules.ProblemSet;
using redinc_reboot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace redinc_reboot.Controllers
{
    public class ProblemSetController : Controller
    {     
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Message = "Modify the problem set.";
            ProblemSetData set = GlobalStaticVars.StaticCore.GetSetById(id);
            ICollection<ProblemSetData> prereqs = GlobalStaticVars.StaticCore.GetSetPrereqs(id);
            ProblemSetViewModel model = new ProblemSetViewModel();
            model.Set = set;
            model.Prereqs = prereqs;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProblemSetViewModel model)
        {
            if (ModelState.IsValid)
            {
                GlobalStaticVars.StaticCore.ModifySet(model.Set);
                GlobalStaticVars.StaticCore.UpdateSetPrereqs(model.Set.Id, model.Prereqs);

                //This is necessary in case bad prereqs (ex. duplicates) are removed by the backend
                model.Prereqs = GlobalStaticVars.StaticCore.GetSetPrereqs(model.Set.Id);
            }

            return View(model);
        }

        public ActionResult NewPrereq(int classId)
        {
            ICollection<ProblemSetData> sets = GlobalStaticVars.StaticCore.GetSetsForClass(classId);
            ViewBag.Sets = new SelectList(sets.Select(s => new { s.Id, s.Name }), "Id", "Name");
            return PartialView("NewPrereqDialog", new ProblemSetData());
        }

        [HttpPost]
        public ActionResult NewPrereq(ProblemSetData model)
        {
            return PartialView("EditorTemplates/PrereqRow", model);
        }

        public JsonResult Search(string term)
        {
            //TODO Get class id from persistent storage
            List<ProblemSetData> sets = GlobalStaticVars.StaticCore.SearchSetsInClass(2, term);
            return Json(sets.Select(s => new { Id = s.Id, label = s.Name, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
    }
}