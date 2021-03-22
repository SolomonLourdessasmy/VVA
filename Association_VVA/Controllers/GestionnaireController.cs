using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Association_VVA.Models;
using System.IO;
using Newtonsoft.Json;

namespace Association_VVA.Controllers
{
    public class GestionnaireController : Controller
    {
        // GET: Gestionnaire
        

        private static ReservationDataContext db = new ReservationDataContext();

        private bool CheckFile(HttpPostedFileBase f)
        {
            return Path.GetExtension(f.FileName).ToLower() == ".jpg" || Path.GetExtension(f.FileName).ToLower() == ".png" || Path.GetExtension(f.FileName).ToLower() == ".gif" || Path.GetExtension(f.FileName).ToLower() == ".jpeg" || Path.GetExtension(f.FileName).ToLower() == ".jfif";
        }

        public ActionResult Connexion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Connexion(string id, string mdp)
        {

            List<COMPTE> user = (from c in db.COMPTE
                                 where c.CDUSER == id && c.MDP == mdp && c.DATEFERME == null && c.TYPECOMPTE == "ADMIN"
                                 select c).ToList();

            if (user.Count() == 1)
            {
                COMPTE unCompte = user.First();
                if (unCompte.CDUSER == id && unCompte.MDP == mdp)
                {
                    Session["userAdmin"] = unCompte.CDUSER;

                    return RedirectToAction("Detail", "Gestionnaire");
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

        public ActionResult Detail()
        { 
            if (Session["userAdmin"] != null)
            {
                ViewBag.User = Session["userAdmin"];
                return View(db.HEBERGEMENT.ToList());
            }
            else
            {
                return RedirectToAction("Connexion", "Gestionnaire");
            }
        }

        public ActionResult NouveauHebergement()
        {
            if (Session["userAdmin"] != null)
            {
                ViewBag.User = Session["userAdmin"];
                return View(db.TYPE_HEB.ToList());
            }
            else
            {
                return RedirectToAction("Connexion", "Gestionnaire");
            }
            
        }

        [HttpPost]
        public ActionResult NouveauHebergement(HttpPostedFileBase filename, string type, string nomHeberge, int noPlaceHeb, int surface, int annee, string secteur, string orientation, string etat, string description, string tarif, string internet)
        {
            Session["unType"] = type; Session["nomHeberge"] = nomHeberge; Session["noPlaceHeb"] = noPlaceHeb; Session["surface"] = surface; Session["annee"] = annee; Session["secteur"] = secteur; Session["orientation"] = orientation; Session["etat"] = etat; Session["description"] = description; Session["tarif"] = tarif; Session["internet"] = internet;
            ViewBag.net = (string)Session["internet"];
           
            HEBERGEMENT unHeberge = new HEBERGEMENT();
            string untarif = tarif.Replace('.', ',');

            if (CheckFile(filename))
            {
                string chemin = Server.MapPath("~/photoHeberge");
                string copier_coller = Path.Combine(chemin, Path.GetFileName(filename.FileName));
                filename.SaveAs(copier_coller);
                unHeberge.PHOTOHEB = Path.GetFileName(filename.FileName);
                unHeberge.NOMHEB = nomHeberge;
                unHeberge.CODETYPEHEB = type;
                unHeberge.NBPLACEHEB = noPlaceHeb;
                unHeberge.SURFACEHEB = surface;
                unHeberge.ANNEEHEB = annee;
                unHeberge.SECTEURHEB = secteur;
                unHeberge.ORIENTATIONHEB = orientation;
                unHeberge.ETATHEB = etat;
                unHeberge.DESCRIHEB = description;
                unHeberge.TARIFSEMHEB = decimal.Parse(untarif);
                if(internet == "true")
                {
                    unHeberge.INTERNET = true;
                }
                else
                {
                    unHeberge.INTERNET = false;
                }
                db.HEBERGEMENT.InsertOnSubmit(unHeberge);
                db.SubmitChanges();
                Session["unType"] = null; Session["nomHeberge"] = null; Session["noPlaceHeb"] = null; Session["surface"] = null; Session["annee"] = null; Session["secteur"] = null; Session["orientation"] = null; Session["etat"] = null; Session["description"] = null; Session["tarif"] = null; Session["internet"] = null;
                
                return RedirectToAction("Detail");
            }
            else
            {
                ViewBag.FileError = "Ce fichier n'est pas une photo.";
                return View(db.TYPE_HEB.ToList());
            }
        }

        public  static List<TYPE_HEB> LesTypesNoUtiliser(int id)
        {
            return (from type in db.TYPE_HEB.ToList()
                    where !(from h in db.HEBERGEMENT.ToList()
                            where h.NOHEB == id
                            select h.TYPE_HEB.CODETYPEHEB).Contains(type.CODETYPEHEB)
                   select type).ToList();
        }

        public ActionResult ModifierHebergement(int id)
        {
            HEBERGEMENT unHeberge = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == id);
            return View(unHeberge);
        }

        [HttpPost]
        public ActionResult ModifierHebergement(HttpPostedFileBase filename, string type, string nomHeberge, int noPlaceHeb, int surface, int annee, string secteur, string orientation, string etat, string description, string tarif, string internet, int idHeb)
        {
            HEBERGEMENT unHeberge = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == idHeb);
            TYPE_HEB typeHeb = db.TYPE_HEB.SingleOrDefault(T => T.CODETYPEHEB == type);
            if (filename != null)
            {
                if (CheckFile(filename))
                {
                    System.IO.File.Delete(Server.MapPath("~/photoHeberge/" + unHeberge.PHOTOHEB));
                    string ImageHebergement = Path.GetFileName(filename.FileName);
                    string chemin = Server.MapPath("~/photoHeberge");
                    string copier_coller = Path.Combine(chemin, ImageHebergement);
                    filename.SaveAs(copier_coller);
                    unHeberge.PHOTOHEB = ImageHebergement;
                }
            }

            string untarif = tarif.Replace('.',',');
            unHeberge.NOMHEB = nomHeberge;
            unHeberge.TYPE_HEB = typeHeb;
            unHeberge.NBPLACEHEB = noPlaceHeb;
            unHeberge.SURFACEHEB = surface;
            unHeberge.ANNEEHEB = annee;
            unHeberge.SECTEURHEB = secteur;
            unHeberge.ORIENTATIONHEB = orientation;
            unHeberge.ETATHEB = etat;
            unHeberge.DESCRIHEB = description;
            unHeberge.TARIFSEMHEB = decimal.Parse(untarif);
            unHeberge.INTERNET = (internet == "true") ? true : false;
            db.SubmitChanges();
            return RedirectToAction("Detail", "Gestionnaire");
        }

        private bool CeHebergementEstRerserve(int id)
        {
            return (from r in db.RESA
                    where r.NOHEB == id
                    select r).Count() > 0;
        }

        [HttpPost]
        public ActionResult SupprimerHebergement(int id)
        {
            if (!CeHebergementEstRerserve(id))
            {
                HEBERGEMENT unHeberge = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == id);
                System.IO.File.Delete(Server.MapPath("~/photoHeberge/" + unHeberge.PHOTOHEB));
                db.HEBERGEMENT.DeleteOnSubmit(unHeberge);
                db.SubmitChanges();
            }
            return RedirectToAction("Detail");
        }

        public ActionResult Reservation()
        {
            if(Session["userAdmin"] != null)
            {
                ViewBag.User = Session["userAdmin"];
                return View(db.RESA.ToList());
            }
            else
            {
                return RedirectToAction("Connexion", "Gestionnaire");
            }
            
        }

        [HttpPost]
        public ActionResult Reservation(string date, string typeheb)
        {

            if (Session["userAdmin"] != null)
            {
                ViewBag.date = date;
                if (typeheb == null && date == null)
                {
                    return View(db.RESA.ToList());
                }
                else if (typeheb == null && date != null)
                {
                    List<RESA> lesHebReserveDate = (from r in db.RESA
                                                    where r.DATEDEBSEM == Convert.ToDateTime(date)
                                                    select r).ToList();
                    return View(lesHebReserveDate);
                }
                else if (typeheb != null && date == null)
                {
                    TYPE_HEB untype = (from ty in db.TYPE_HEB
                                       where ty.CODETYPEHEB == typeheb
                                       select ty).ToList().First();
                    ViewBag.typeheb = untype.NOMTYPEHEB;
                    ViewBag.typeCode = untype.CODETYPEHEB;
                    List<RESA> lesHebReserveType = (from r in db.RESA
                                                    where r.HEBERGEMENT.CODETYPEHEB == typeheb
                                                    select r).ToList();
                    return View(lesHebReserveType);
                }
                else
                {
                    TYPE_HEB untype = (from ty in db.TYPE_HEB
                                       where ty.CODETYPEHEB == typeheb
                                       select ty).ToList().First();
                    ViewBag.typeheb = untype.NOMTYPEHEB;
                    ViewBag.typeCode = untype.CODETYPEHEB;
                    List<RESA> lesHebReserveDateType = (from r in db.RESA
                                                        where r.DATEDEBSEM == Convert.ToDateTime(date) && r.HEBERGEMENT.CODETYPEHEB == typeheb
                                                        select r).ToList();
                    return View(lesHebReserveDateType);
                }
            }
            else
            {
                return RedirectToAction("Connexion", "Gestionnaire");
            }

        }
        
        
        public static List<HEBERGEMENT> GetLesHebergementsSansResaActuelle(int noResa)
        {
            return (from h in db.HEBERGEMENT
                    where !(from r in db.RESA
                            where r.NORESA == noResa
                            select r.HEBERGEMENT.NOHEB).Contains(h.NOHEB)
                    select h).ToList();
        }

        public ActionResult Nouvelle_Reservation(int id)
        {
            if (Session["userAdmin"] != null)
            {
                HEBERGEMENT unHeberge = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == id);
                ViewBag.arrhes = unHeberge.TARIFSEMHEB * 0.2m;
                return View(unHeberge);
            }
            else
            {
                return RedirectToAction("Connexion", "Gestionnaire");
            }
        }


        [HttpPost]
        public ActionResult Nouvelle_Reservation(string user, string idHeb, int nbOccupant, string dateDispo)
        {
            if (Session["userAdmin"] != null)
            {
                HEBERGEMENT unHeberge = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == int.Parse(idHeb));
                SEMAINE semaine = new SEMAINE();
                RESA unResa = new RESA();
                unResa.CDUSER = user;
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
                if (uneSemaine.Count == 0)
                {
                    semaine.DATEDEBSEM = Convert.ToDateTime(dateDispo);
                    semaine.DATEFINSEM = Convert.ToDateTime(dateDispo).AddDays(7);
                    db.SEMAINE.InsertOnSubmit(semaine);
                    db.SubmitChanges();
                }
                db.RESA.InsertOnSubmit(unResa);
                db.SubmitChanges();
                return RedirectToAction("Reservation", "Gestionnaire");
            }
            else
            {
                return RedirectToAction("Connexion", "Gestionnaire");
            }

        }

        public ActionResult ModifierReservation(int id)
        {
            if (Session["userAdmin"] != null)
            {
                RESA unResa = db.RESA.SingleOrDefault(R => R.NORESA == id);
                return View(unResa);
            }
            else
            {
                return RedirectToAction("Connexion", "Gestionnaire");
            }

            
        }

        private bool DateExiste(string date)
        {
            return (from d in db.SEMAINE
                    where d.DATEDEBSEM == Convert.ToDateTime(date)
                    select d).Count() == 1;
        }

        [HttpPost]
        public ActionResult ModifierReservation(string etat, string dateDeb, int noResa, string noHeb, string datesArrhes, int occupant)
        {
            if (Session["userAdmin"] != null)
            {
                RESA unResa = db.RESA.SingleOrDefault(R => R.NORESA == noResa);
                if (dateDeb != null)
                {
                    if (!DateExiste(dateDeb))
                    {
                        SEMAINE semaine = new SEMAINE();
                        semaine.DATEDEBSEM = Convert.ToDateTime(dateDeb);
                        semaine.DATEFINSEM = Convert.ToDateTime(dateDeb).AddDays(7);
                        db.SEMAINE.InsertOnSubmit(semaine);
                        db.SubmitChanges();
                    }

                    if (unResa.DATEDEBSEM != Convert.ToDateTime(dateDeb))
                    {
                        SEMAINE uneSemaine = db.SEMAINE.SingleOrDefault(S => S.DATEDEBSEM == Convert.ToDateTime(dateDeb));
                        unResa.SEMAINE = uneSemaine;
                    }

                }

                if (etat != null)
                {
                    if (unResa.CODEETATRESA != etat)
                    {
                        ETAT_RESA unEtat = db.ETAT_RESA.SingleOrDefault(E => E.CODEETATRESA == etat);
                        unResa.ETAT_RESA = unEtat;
                    }


                }
                if (datesArrhes != "")
                {
                    unResa.DATEARRHES = DateTime.Parse(datesArrhes);
                }

                unResa.NBOCCUPANT = occupant;
                if (noHeb != null)
                {
                    HEBERGEMENT unHeb = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == Convert.ToInt32(noHeb));
                    if (unResa.NOHEB != Convert.ToInt32(noHeb))
                    {
                        unResa.MONTANTARRHES = unHeb.TARIFSEMHEB * 0.2m;
                        unResa.TARIFSEMRESA = unHeb.TARIFSEMHEB;
                        unResa.HEBERGEMENT = unHeb;
                    }

                }

                db.SubmitChanges();
                List<SEMAINE> lesSemainesInutiles = LesDateInutiles();
                foreach (SEMAINE S in lesSemainesInutiles)
                {
                    db.SEMAINE.DeleteOnSubmit(S);
                    db.SubmitChanges();

                }
                return RedirectToAction("Reservation", "Gestionnaire");
            }
            else
            {
                return RedirectToAction("Connexion", "Gestionnaire");
            }
        }

        [HttpPost]
        public ActionResult SupprimerResa(int noResa)
        {
            RESA unResa = db.RESA.SingleOrDefault(R => R.NORESA == noResa);
            db.RESA.DeleteOnSubmit(unResa);
            db.SubmitChanges();
            return RedirectToAction("Reservation", "Gestionnaire");
        }

        public ActionResult Deconnexion()
        {
            Session["userAdmin"] = null;
            return RedirectToAction("Connexion", "Gestionnaire");
        }

        [HttpPost]
        public JsonResult GetunHerbergementAReserve(string noheb)
        {
            HEBERGEMENT hebergement = db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == Convert.ToInt32(noheb));
            string internet = "";
            if ((bool)hebergement.INTERNET)
            {
                internet = "internet est disponible";
            }
            else
            {
                internet = "internet n'est pas disponible";
            }

            string json = "{\"photo\":\"" + hebergement.PHOTOHEB + "\", \"nom\":\"" + hebergement.NOMHEB + "\", \"type\":\"" + hebergement.TYPE_HEB.NOMTYPEHEB + "\", \"nbPlace\":" + hebergement.NBPLACEHEB + ", \"surface\":" + (hebergement.SURFACEHEB).ToString().Replace(',', '.') + ", \"anneeConstruct\":" + hebergement.ANNEEHEB + ", \"secteur\":\"" + hebergement.SECTEURHEB + "\", \"orientation\":\"" + hebergement.ORIENTATIONHEB + "\", \"etat\":\"" + hebergement.ETATHEB + "\", \"tarif\":" + (hebergement.TARIFSEMHEB).ToString().Replace(',', '.') + ", \"internet\":\"" + internet + "\"}";
            
            
            
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LesSemaines(string id)
        {
            List<Date> lesDates = new List<Date>();
            foreach (DateTime d in lesSamediDispo(Convert.ToInt32(id)))
            {

                lesDates.Add(new Date(d.ToShortDateString()));
            }
            string listSemaine = JsonConvert.SerializeObject(lesDates);
            return Json(listSemaine, JsonRequestBehavior.AllowGet);
        }

       public static List<ETAT_RESA> LesEtatResaSansActuelle(string codeEtat)
        {
            return (from e in db.ETAT_RESA
                    where !(from r in db.RESA
                            where r.ETAT_RESA.CODEETATRESA == codeEtat
                            select r.ETAT_RESA.CODEETATRESA).Contains(e.CODEETATRESA)
                    select e).ToList();
        }

        public static List<DateTime> lesSamediDispo(int id)
        {
            return (from s in Temps.lesSamedis()
                    where !(from d in db.RESA
                            where d.NOHEB == id && d.CODEETATRESA != "ANUL"
                            select d.DATEDEBSEM).Contains(s)
                    select s).ToList();
        }
        public static List<SEMAINE> LesSemaines()
        {
            return db.SEMAINE.ToList();
        }

        public static List<TYPE_HEB> LesTypes()
        {
            return db.TYPE_HEB.ToList();
        }

        public static HEBERGEMENT GetHEBERGEMENT(int noheb)
        {
            return db.HEBERGEMENT.SingleOrDefault(H => H.NOHEB == noheb);
        }

        public static List<COMPTE> lesComptes(string cdUser)
        {
            return (from c in db.COMPTE
                    where c.CDUSER != cdUser && c.DATEFERME == null
                    select c).ToList();
        }

        private List<SEMAINE> LesDateInutiles()
        {
            return (from s in db.SEMAINE
                    where !(from r in db.RESA
                            select r.SEMAINE).Contains(s)
                    select s).ToList();
        }
    }
}