using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Association_VVA.Models;

namespace Association_VVA.Controllers
{
    public class AccueilController : Controller
    {
        // GET: Accueil
        private  ReservationDataContext db = new ReservationDataContext();
        
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Hebergement()
        {
            ViewBag.lesSamedi = Temps.lesSamedis();
            ViewBag.type = db.TYPE_HEB.ToList();
            return View(db.HEBERGEMENT.ToList());
        }

        [HttpPost]
        public ActionResult Hebergement(string dateDispo, string typeheb)
        {
            ViewBag.Date = dateDispo;
            ViewBag.lesSamedi = Temps.lesSamedis();
            ViewBag.type = db.TYPE_HEB.ToList();
            if (typeheb == null && dateDispo == null)
            {
                return View(db.HEBERGEMENT.ToList());
            }
            else if (typeheb == null && dateDispo != null)
            {
                List<HEBERGEMENT> heblibreEnDate = (from h in db.HEBERGEMENT
                                                    where !(from r in db.RESA
                                                            where r.DATEDEBSEM == Convert.ToDateTime(dateDispo)
                                                            select r.NOHEB).Contains(h.NOHEB)
                                                    select h).ToList();
                return View(heblibreEnDate);
            }
            else if (typeheb != null && dateDispo == null)
            {
                TYPE_HEB untype = (from ty in db.TYPE_HEB
                                   where ty.CODETYPEHEB == typeheb
                                   select ty).ToList().First();
                ViewBag.typeheb = untype.NOMTYPEHEB;
                ViewBag.typeCode = untype.CODETYPEHEB;
                List<HEBERGEMENT> heblibreEnType = (from h in db.HEBERGEMENT
                                                    where h.CODETYPEHEB == typeheb
                                                    select h).ToList();
                return View(heblibreEnType);
            }
            else 
            {
                TYPE_HEB untype = (from ty in db.TYPE_HEB
                                   where ty.CODETYPEHEB == typeheb
                                   select ty).ToList().First();
                ViewBag.typeheb = untype.NOMTYPEHEB;
                ViewBag.typeCode = untype.CODETYPEHEB;
                List<HEBERGEMENT> heblibre = (from h in db.HEBERGEMENT
                                              where !(from r in db.RESA
                                                      where r.DATEDEBSEM == Convert.ToDateTime(dateDispo)
                                                      select r.NOHEB).Contains(h.NOHEB) && h.CODETYPEHEB == typeheb
                                              select h).ToList();
                return View(heblibre);
            }
                
        }

    }
}

