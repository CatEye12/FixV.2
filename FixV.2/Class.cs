using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace FixV._2
{
   class Class
   {
        static SldWorks swApp;
        static ModelDoc2 swModel;
        public static swDocumentTypes_e docType;
        static View swView;
        static DrawingDoc drw;
        static int m;
        private static string configName = String.Empty;
        public static string[] massaValues = null;
        public static int unitsType = 0;
        public static bool configChanged = false;
        public static string[] configNames;
        public static int configIterator = 99;
        public static bool lockForConf;
        internal static string[] razdel = { "Документація", "Комплекси", "Складальні одиниці", "Деталі", "Комплекти", "ЭМ-Сборочные-единицы", "ЭМ-Детали" };


        public static string configuracione
        {
            get { return configName; }
            set { configName = value; }
        }

        public static void GetSolidObject()
        {
            swApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application");
            swModel = swApp.ActiveDoc;
            docType = (swDocumentTypes_e)swModel.GetType();
            configName = swModel.GetActiveConfiguration().Name;//при первом вызове присваеваеться активная конфигурация
            Class.configNames = Class.GetAllConfigurations(out lockForConf);
        }

        public static void Start()
        {
            // Проверка открытого документа
            if (swModel == null)
            {
                swApp.SendMsgToUser2("Откройте модель, сборку или чертеж!", (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);

                return;
            }

            if (string.IsNullOrEmpty(swModel.GetPathName()))
            {
                swApp.SendMsgToUser2("Сохраните файл!", (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);

                return;
            }

            // Определение типа документа
            if (docType == swDocumentTypes_e.swDocDRAWING)
            {
                drw = (DrawingDoc)swModel;

                // Получение первого листа
                Sheet swSheet = drw.GetCurrentSheet();
                string strActiveSheetName = swSheet.GetName();

                // Узнаем имя активного листа
                string[] vSheetNames = drw.GetSheetNames();
                drw.ActivateSheet(vSheetNames[0]);
                swSheet = drw.GetCurrentSheet();
                swView = drw.GetFirstView();

                m = 0;

                if (swSheet.CustomPropertyView == "По умолчанию" | swSheet.CustomPropertyView == "Default")
                {
                    swView = swView.GetNextView();                  
                    // Получаем первый вид
                }
                else
                {
                    while (swView != null)
                    {
                        if (swView.GetName2() == swSheet.CustomPropertyView)
                        {
                            m = 1;
                        }
                        swView = swView.GetNextView();
                    }
                    if (m == 0)
                    {
                        swView = drw.GetFirstView();
                        swView = swView.GetNextView();
                        swApp.SendMsgToUser2("Не удалось определить вид из свойств листа. Ипользуется первый вид.", (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                    }
                }

                if (swView == null)
                {
                    swApp.SendMsgToUser2("Отсутсвует модель!", (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                    // Возвращение активного листа
                    drw.ActivateSheet(strActiveSheetName);
                    return;
                }

                if (swView.ReferencedDocument == null)
                {
                    swApp.SendMsgToUser2("Отсутсвует модель!", (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                    // Возвращение активного листа
                    drw.ActivateSheet(strActiveSheetName);
                    return;
                }
                string sModelName = swView.GetReferencedModelName();
                Class.configuracione = swView.ReferencedConfiguration;
                swModel = swView.ReferencedDocument;
            }
        }
        

        /// <summary>
        /// Удаляет свойства, если они не в своей категории 
        /// </summary>
        public static void FixPropertys()
        {
            string ValOut;
            string ResValOut;
            bool WasResolved;
            

            swModel.Extension.CustomPropertyManager[""].Delete2("Обозначение");
            swModel.Extension.CustomPropertyManager[""].Delete2("Наименование");
            swModel.Extension.CustomPropertyManager[""].Delete2("Наименование_ФБ");
            swModel.Extension.CustomPropertyManager[""].Delete2("Number");
            swModel.Extension.CustomPropertyManager[""].Delete2("RenameSWP");
            swModel.Extension.CustomPropertyManager[""].Delete2("DescriptionEng");
            swModel.Extension.CustomPropertyManager[""].Delete2("Сборка");
            swModel.Extension.CustomPropertyManager[""].Delete2("Примечание");
            swModel.Extension.CustomPropertyManager[""].Delete2("Формат");
            swModel.Extension.CustomPropertyManager[""].Delete2("DrawnBy");

            swModel.Extension.CustomPropertyManager[Class.configuracione].Get5("Проверил", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.ChechedBy = ResValOut;
            swModel.Extension.CustomPropertyManager[Class.configuracione].Delete2("Проверил");
            swModel.Extension.CustomPropertyManager[""].Add3("Проверил", 30, Propertiy.ChechedBy, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);

            swModel.Extension.CustomPropertyManager[Class.configuracione].Get5("Утвердил", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.ApprovedBy = ResValOut;
            swModel.Extension.CustomPropertyManager[Class.configuracione].Delete2("Утвердил");
            swModel.Extension.CustomPropertyManager[""].Add3("Утвердил", 30, ResValOut, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);

            swModel.Extension.CustomPropertyManager[Class.configuracione].Get5("Техконтроль", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.TControl = ResValOut;
            swModel.Extension.CustomPropertyManager[Class.configuracione].Delete2("Техконтроль");
            swModel.Extension.CustomPropertyManager[""].Add3("Техконтроль", 30, ResValOut, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);

            swModel.Extension.CustomPropertyManager[Class.configuracione].Get5("Масса_Таблица", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Weight = ResValOut;
            swModel.Extension.CustomPropertyManager[Class.configuracione].Delete2("Масса_Таблица");
            swModel.Extension.CustomPropertyManager[Class.configuracione].Add3("Масса", 30, ResValOut, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);


            swModel.Extension.CustomPropertyManager[Class.configuracione].Delete2("CheckedBy");
            swModel.Extension.CustomPropertyManager[Class.configuracione].Delete2("EngineeringApproval");

            //Масса_Таблица


            // string[] propForConf = { "Обозначение", "Раздел", "Масса", "Наименование" };

            // string[] propForSettings = { "Код документа", "Тип документа", "Разработал", "Проверил", "Техконтроль",
            //     "Н.контр.", "Утвердил", "Контора", "", "N извещения" };

            // string[] propForDrw = { "Конструктор", "Проверил", "Техконтроль", "Н.контр.", "Нач.отд.", "Утвердил",
            //     "Литера", "Масса", "Масштаб", "Материал", "Тип документа1", "Обозначение",
            //     "Наименование", "Литера2", "Литера3", "Контора", "Изменение", "№ извещения",
            //     "Раздел", "Код документа", "Лист", "Листов", "Формат" };

            // object propName = null;

            // swModel.Extension.CustomPropertyManager[Class.activeConfigName].GetAll2(ref propName, null, null, null);

            // string[] arpropName = (string[])propName; // приводим названия свойств к типу строкового массива

            // if (docType == swDocumentTypes_e.swDocDRAWING)
            // {
            //     foreach (var item in arpropName)
            //     {
            //         if (!propForDrw.Contains(item))
            //         {
            //             swModel.Extension.CustomPropertyManager[""].Delete2(item);
            //         }
            //     }
            //}
            //else
            //{
            //     if (Class.activeConfigName == "") //главная конф, редактируем свойства SummuryInfo
            //     {
            //         foreach (var item in arpropName)
            //         {
            //             if (!propForSettings.Contains(item))
            //             {
            //                 swModel.Extension.CustomPropertyManager[""].Delete2(item);
            //             }
            //         }
            //     }
            //     else //редактируем свойства Configuration
            //     {
            //         foreach (var item in arpropName)
            //         {
            //             if (!propForConf.Contains(item))
            //             {
            //                 swModel.Extension.CustomPropertyManager[""].Delete2(item);
            //             }
            //         }
            //     }
            // }
        }


        public static void GetProperties(string ConfigName)
        {
            string ValOut;
            string ResValOut;
            bool WasResolved;
            
            swModel.Extension.CustomPropertyManager[""].Get5("Код документа", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.DocCode = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Тип документа", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.DocType = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Конструктор", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.DevelopedBy = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Проверил", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.ChechedBy = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Техконтроль", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.TControl = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Н.контр.", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.NControl = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Нач.отд.", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.NachOtd = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Утвердил", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.ApprovedBy = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Контора", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Subvision = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Изменения", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Changing = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("N извещения", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Notification = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("MassaFormat", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.MassaFormat = ResValOut;

            swModel.Extension.CustomPropertyManager[ConfigName].Get5("Обозначение", false, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Designition = ResValOut;
            swModel.Extension.CustomPropertyManager[ConfigName].Get5("Раздел", false, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Division = ResValOut;
            swModel.Extension.CustomPropertyManager[ConfigName].Get5("Масса", false, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Weight = ResValOut;
            swModel.Extension.CustomPropertyManager[ConfigName].Get5("Наименование", false, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Name = ResValOut;


            //Drawing
            swModel.Extension.CustomPropertyManager[""].Get5("Литера", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Letter2 = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Масштаб", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Scale = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Материал", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Material = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Тип документа1", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.DocType1 = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Литера2", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Letter2 = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Литера3", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Letter3 = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Лист", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Sheet = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Листов", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Sheets = ResValOut;
            swModel.Extension.CustomPropertyManager[""].Get5("Формат", true, out ValOut, out ResValOut, out WasResolved);
            Propertiy.Format = ResValOut;
            //Масса  Под вопросом, посмотреть интермех
            //MassaFormat


            AddPropertiesFromModel();
        }
        public static void AddPropertiesFromModel()
        {
            if (Propertiy.Designition == String.Empty)
            {
                GetModelName();
            }
            else
            {
                WithVersionOrNo();
            }
            
            if (Propertiy.Division == String.Empty)
            {
                if (Class.docType == swDocumentTypes_e.swDocASSEMBLY)
                {
                    Propertiy.Division = "Складальні одиниці";
                }
                else if (Class.docType == swDocumentTypes_e.swDocPART)
                {
                    Propertiy.Division = "Деталі";
                }
            }
            
            massaValues = Class.GetModelWeight();
            if (massaValues.Length > 1)
            {
                // МАССА
                if (Propertiy.MassaFormat != String.Empty)
                {

                    switch (Propertiy.MassaFormat.Substring(1,1)) // проверяем последнюю цифру свойства MassaFormat
                    {
                        case "0":
                            Propertiy.Weight = massaValues[1].ToString();
                            break;
                        case "1":
                            Propertiy.Weight = massaValues[2].ToString();
                            break;
                        case "2":
                            Propertiy.Weight = massaValues[3].ToString();
                            break;
                        case "3":
                            Propertiy.Weight = massaValues[4].ToString();
                            break;
                        case "4":
                            Propertiy.Weight = massaValues[5].ToString();
                            break;
                    }
                }
                else
                {
                    Propertiy.Weight = massaValues[1].ToString(); 
                }
            }
        }
        public static void DefineMassaFormat(int selectedIndex, int unitsType)
        {
            int accuracyType = 0;
            switch (selectedIndex)
            {
                case 0:
                    if (unitsType == (int)MassaFormatUnits.kilo)
                    {
                        accuracyType = (int)MassaFormatAccuracy.without_ext;
                        unitsType = (int)MassaFormatUnits.grams;
                    }
                    else
                    {
                        accuracyType = (int)MassaFormatAccuracy._3_symbol_after_point;
                        unitsType = (int)MassaFormatUnits.kilo;
                    }
                    break;
                case 1:
                    accuracyType = (int)MassaFormatAccuracy.without_ext;
                    break;
                case 2:
                    accuracyType = (int)MassaFormatAccuracy._1_symbol_after_point;
                    break;
                case 3:
                    accuracyType = (int)MassaFormatAccuracy._2_symbol_after_point;
                    break;
                case 4:
                    accuracyType = (int)MassaFormatAccuracy._3_symbol_after_point;
                    break;
                default:
                    accuracyType = (int)MassaFormatAccuracy._4_symbol_after_point;
                    break;
            }

            Propertiy.MassaFormat = unitsType.ToString() + accuracyType.ToString();
        }

        public static void SetProperties(string confName)
        {

            if (docType != swDocumentTypes_e.swDocDRAWING && confName != "")//  свойства для Configuration
            {
                swModel.Extension.CustomPropertyManager[confName].Add3("Обозначение", 30, Propertiy.Designition, 2);
                swModel.Extension.CustomPropertyManager[confName].Add3("Раздел", 30, Propertiy.Division, 2);
                swModel.Extension.CustomPropertyManager[confName].Add3("Масса", 30, Propertiy.Weight, 2);
                swModel.Extension.CustomPropertyManager[confName].Add3("Наименование", 30, Propertiy.Name, 2);
            }
            if (docType != swDocumentTypes_e.swDocDRAWING)
            {
                
                swModel.Extension.CustomPropertyManager[""].Add3("Код документа", 30, Propertiy.DocCode, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Разработал", 30, Propertiy.DevelopedBy, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Проверил", 30, Propertiy.ChechedBy, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Техконтроль", 30, Propertiy.TControl, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Н.контр.", 30, Propertiy.NControl, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Утвердил", 30, Propertiy.ApprovedBy, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Контора", 30, Propertiy.Subvision, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("N извещения", 30, Propertiy.Notification, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Изменение", 30, Propertiy.Changing, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Тип документа", 30, Propertiy.DocType, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("MassaFormat", 30, Propertiy.MassaFormat, 2);

            }
            else if (docType == swDocumentTypes_e.swDocDRAWING)
            {

                swModel.Extension.CustomPropertyManager[""].Add3("Литера", 30, Propertiy.Letter, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Масштаб", 30, Propertiy.Scale, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Материал", 30, Propertiy.Material, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Тип документа1", 30, Propertiy.DocType1, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Литера2", 30, Propertiy.Letter2, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Литера3", 30, Propertiy.Letter3, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Раздел", 30, Propertiy.Division, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Лист", 30, Propertiy.Sheet, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Листов", 30, Propertiy.Sheets, 2);
                swModel.Extension.CustomPropertyManager[""].Add3("Формат", 30, Propertiy.Format, 2);
            }
            
           swModel.ForceRebuild3(false);
        }
        

        //значение обозначения из модели/чертежа
        private static void GetModelName()
        {
            string name = swModel.GetTitle();

            if (name.ToUpper().Contains(".SLD"))
            {
                if (docType == swDocumentTypes_e.swDocDRAWING)
                {
                    Propertiy.Designition = name.Remove(name.Length - 15, 15);
                }
                else
                {
                    Propertiy.Designition = name.Remove(name.Length - 7, 7);
                }
            }
            else
            {
                if (docType == swDocumentTypes_e.swDocDRAWING)
                {
                    Propertiy.Designition = name.Remove(name.Length - 7, 7);
                }
            }
        }
        public static string[] GetModelWeight()
        {
            string path = swModel.GetPathName();

            if (Class.configChanged  == true || Class.configIterator == 99)
            {
                swModel.ShowConfiguration2(Class.configuracione);
            }
            else 
            {
                swModel.ShowConfiguration2(Class.configNames[configIterator]);
                configIterator++;
            }



            int status = 99;
            double[] masProperties = swModel.Extension.GetMassProperties2(1, out status, true);
            double massa = Convert.ToDouble(masProperties?[5]); // в килограммах
            double mass = massa * 1000; //в граммах

            if (mass > 0)// если деталь имеет массу
            {
                string[] resMas = new string[7];

                string mantisa = String.Empty;
                string integerVal = String.Empty;
                string tempValue = String.Empty;

                if (mass < 1000)
                {
                    unitsType = (int)MassaFormatUnits.grams;

                    mantisa = (mass - (1* (Math.Floor(mass/ 1) * Math.Sign(mass)))).ToString();
                    integerVal = Math.Truncate(mass).ToString() + ",";

                    if (mantisa != "0")// г-кг
                    {
                        resMas[0] = massa.ToString().Substring(0, 5); // в кг с точностью 3 знака

                        resMas[1] = Math.Truncate(mass).ToString() + "  г";

                        tempValue = mantisa.Substring(2, 1);
                        resMas[2] = integerVal + tempValue; // 1 знак после запятой

                        tempValue = mantisa.Substring(2, 2);
                        resMas[3] = integerVal + tempValue; // 2 знака после запятой

                        tempValue = mantisa.Substring(2, 3);
                        resMas[4] = integerVal + tempValue; // 3 знака после запятой

                        tempValue = mantisa.Substring(2, 4);
                        resMas[5] = integerVal + tempValue; // 4 знака после запятой
                    }
                    else
                    {
                        resMas[0] = massa.ToString().Substring(0, 5); // в кг с точностью 3 знака

                        resMas[1] = Math.Truncate(mass * 1000).ToString();

                        tempValue = "0";
                        resMas[2] = integerVal + tempValue; // 1 знак после запятой

                        tempValue = "00";
                        resMas[3] = integerVal + tempValue; // 2 знака после запятой

                        tempValue = "000";
                        resMas[4] = integerVal + tempValue; // 3 знака после запятой

                        tempValue = "0000";
                        resMas[5] = integerVal + tempValue; // 4 знака после запятой
                    }
                }
                else
                {
                    unitsType = (int)MassaFormatUnits.kilo;

                    integerVal = Math.Truncate(massa).ToString() + ",";
                    mantisa = (massa - (1 * (Math.Floor(massa / 1) * Math.Sign(massa)))).ToString();

                    if (mantisa != "0") // т-кг
                    {
                        resMas[0] = Math.Truncate(mass).ToString(); // в грамах

                        resMas[1] = Math.Truncate(massa).ToString() + "  кг";

                        tempValue = mantisa.Substring(2, 1);
                        resMas[2] = integerVal + tempValue; // 1 знак после запятой

                        tempValue = mantisa.Substring(2, 2);
                        resMas[3] = integerVal + tempValue; // 2 знака после запятой

                        tempValue = mantisa.Substring(2, 3);
                        resMas[4] = integerVal + tempValue; // 3 знака после запятой

                        tempValue = mantisa.Substring(2, 4);
                        resMas[5] = integerVal + tempValue; // 4 знака после запятой

                    }
                    else
                    {
                        resMas[0] = Math.Truncate(mass).ToString(); // в грамах

                        resMas[1] = Math.Truncate(massa).ToString() + "  кг";

                        tempValue = "0";
                        resMas[2] = integerVal + tempValue; // 1 знак после запятой

                        tempValue = "00";
                        resMas[3] = integerVal + tempValue; // 1 знак после запятой

                        tempValue = "000";
                        resMas[4] = integerVal + tempValue; // 3 знака после запятой

                        tempValue = "000";
                        resMas[5] = integerVal + tempValue; // 4 знака после запятой
                    }
                }
                return resMas;
            }
            return new string[1];
        }
        public static string [] GetAllConfigurations(out bool lockForConfBox)
        {
            lockForConfBox = false;
            string[] mas = {};
            if (Class.docType != swDocumentTypes_e.swDocDRAWING)
            {
                mas = swModel.GetConfigurationNames();
            }
            else
            {
                lockForConfBox = true;
            }
            return mas;
        }
        public static bool WithVersionOrNo()
        {
            Propertiy._Version = false;
            if (Propertiy.Designition.Substring(Propertiy.Designition.Length - 3, 3).StartsWith("-"))
            {
                Propertiy._Version = true;
            }
            return Propertiy._Version;
        }

        public enum MassaFormatAccuracy
        {
            without_ext = 0,
            _1_symbol_after_point = 1,
            _2_symbol_after_point = 2,
            _3_symbol_after_point = 3,
            _4_symbol_after_point = 4
        }
        public enum MassaFormatUnits
        {
            grams = 1,
            kilo = 2
        }
    }
}