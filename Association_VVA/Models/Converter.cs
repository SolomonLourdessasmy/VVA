using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Association_VVA.Models
{

  
    public static class Temps
    {
        public static List<DateTime> lesSamedis()
        {
            List<DateTime> lesSamedi = new List<DateTime>();

            if (lesSamedi.Count > 0)
            {
                lesSamedi.Clear();
            }

            DateTime date = DateTime.Today;
            for (int i = 1; i < 200; i++)
            {
                if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    lesSamedi.Add(date);
                }
                date = date.AddDays(1);
            }

            return lesSamedi;
        }

    }

    public class Date
    {
        public string date="";
        public Date(string date)
        {
            this.date = date;
        }
    }


    public static class Convertir
    {
        public static string FormatDate(DateTime? date)
        {
            if (date != null)
            {
                DateTime dt = (DateTime)date;
                return dt.Year + "-" + dt.Month + "-" + dt.Day;
            }
            else
            {
                return "";
            }
        }
    }


}