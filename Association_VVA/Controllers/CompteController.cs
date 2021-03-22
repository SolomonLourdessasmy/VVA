using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Association_VVA.Models;

namespace Association_VVA.Controllers
{
    public class CompteController : Controller
    {
        // GET: Compte
        private static ReservationDataContext db = new ReservationDataContext();
        public ActionResult Connexion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Connexion(string id, string mdp)
        {
            List<COMPTE> user = (from c in db.COMPTE
                                 where c.CDUSER == id && c.MDP == mdp && c.DATEFERME == null
                                 select c).ToList();

            if(user.Count() == 1)
            {
                COMPTE unCompte = user.First();
                if (unCompte.MDP == mdp)
                {
                    Session["user"] = unCompte.CDUSER;
                    return RedirectToAction("Mes_Reservation", "Compte");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }

        }

        public ActionResult Deconnexion()
        {
            Session["user"] = null;
            return RedirectToAction("Connexion", "Compte");
        }
        

        public ActionResult Ajout_Reservation(int id)
        {
            if(Session["user"] != null)
            {
                HEBERGEMENT unHeberge = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == id);
                ViewBag.arrhes = unHeberge.TARIFSEMHEB * 0.2m;
               
                return View(unHeberge);
            }
            else
            {
                return RedirectToAction("Connexion", "Compte");
            }


        }

        [HttpPost]
        public ActionResult Ajout_Reservation(string idHeb, int nbOccupant, string dateDispo)
        {
            HEBERGEMENT unHeberge = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == int.Parse(idHeb));
            SEMAINE semaine = new SEMAINE();
            RESA unResa = new RESA();
            unResa.CDUSER = (string)Session["user"];
            unResa.DATEDEBSEM = Convert.ToDateTime(dateDispo);
            unResa.NOHEB = int.Parse(idHeb);
            unResa.CODEETATRESA = "BLOC";
            unResa.DATERESA = DateTime.Today.Date;
            unResa.NBOCCUPANT = nbOccupant;
            unResa.MONTANTARRHES = unHeberge.TARIFSEMHEB * 0.2m;
            unResa.TARIFSEMRESA = unHeberge.TARIFSEMHEB;

            List<SEMAINE> uneSemaine = (from s in db.SEMAINE
                                        where s.DATEDEBSEM == Convert.ToDateTime(dateDispo)
                                        select s).ToList();
            if(uneSemaine.Count == 0)
            {
                semaine.DATEDEBSEM = Convert.ToDateTime(dateDispo);
                semaine.DATEFINSEM = Convert.ToDateTime(dateDispo).AddDays(7);
                db.SEMAINE.InsertOnSubmit(semaine);
                db.SubmitChanges();
            }
            db.RESA.InsertOnSubmit(unResa);
            db.SubmitChanges();
            return RedirectToAction("Mes_Reservation", "Compte");
        }

        public ActionResult Mes_Reservation()
        {
            if (Session["user"] != null)
            {
                List<RESA> reserver = (from r in db.RESA
                                                where r.CDUSER == (string)Session["user"]
                                                select r).ToList();
                return View(reserver);
            }
            else
            {
                return RedirectToAction("Connexion", "Compte");
            }
            
        }

        public static List<DateTime> lesSamediDispo(int id)
        {

            return (from s in Temps.lesSamedis()
                    where !(from d in db.RESA
                            where d.NOHEB == id && d.CODEETATRESA != "ANUL"
                            select d.DATEDEBSEM).Contains(s)
                    select s).ToList();
        }

        public static List<ETAT_RESA> LesTypes()
        {
            return db.ETAT_RESA.ToList();
        }

    }
}