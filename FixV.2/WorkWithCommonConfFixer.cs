using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FixV._2
{
    class WorkWithCommonConfFixer
    {
        public static DataTable PropertiesForEachConf()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Конфигурация");
            dt.Columns.Add("Обозначение");
            dt.Columns.Add("Наименование");
            dt.Columns.Add("Раздел");
            dt.Columns.Add("Масса");
            dt.Columns.Add("Версия");

            for (int i = 0; i < Class.configNames.Length; i++)
            {
                dt.Rows.Add();
                Class.GetProperties(Class.configNames[i]);

                dt.Rows[i]["Конфигурация"] = Class.configNames[i];
                dt.Rows[i]["Обозначение"] = Propertiy.Designition;
                dt.Rows[i]["Наименование"] = Propertiy.Name;
                dt.Rows[i]["Масса"] = Propertiy.Weight;
                dt.Rows[i]["Раздел"] = Propertiy.Division;
                dt.Rows[i]["Версия"] = Propertiy._Version;
                
            }
            return dt;
        }

        public static void GetValuesFromGrid(DataTable dt)
        {
            string temp;
            foreach (var item in dt.AsEnumerable())
            {
                temp = item["Конфигурация"].ToString();
                Propertiy.Designition = item["Обозначение"].ToString();
                Propertiy.Name = item["Наименование"].ToString();
                Propertiy.Division = item["Раздел"].ToString();
                Propertiy.Weight = item["Масса"].ToString();
                Class.SetProperties(temp);
            }
        }

       
    }
}