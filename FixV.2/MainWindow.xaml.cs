using SolidWorks.Interop.swconst;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FixV._2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MForm;
        private int MClose;
        private int m;
        bool lockForConf;

        public int MChkBlank;

        public MainWindow()
        {
            InitializeComponent();
            Class.GetSolidObject();
            Class.Start();

            Class.FixPropertys();

            Class.GetProperties(Class.configuracione);
            FillControlsWithDefaultValues();
            FillForm();
        }

        private void ReadPropFromForm()
        {
            Propertiy.Designition = TxtNumber.Text;
            Propertiy.Name = DescriptionTxtBox.Text;
            Propertiy.Division = ComboBoxSection?.SelectedItem?.ToString();
            Propertiy.Letter = CboLit.SelectedItem?.ToString();
            Propertiy.Weight = TxtMass.SelectedItem?.ToString();
            Propertiy.Format = CboFormat.SelectedItem?.ToString();
        }

        private void FillControlsWithDefaultValues()
        {

            CboConfig.Items.Clear();
            //КОНФИГУРАЦИЯ
            foreach (var item in Class.GetAllConfigurations(out lockForConf))
            {
                   CboConfig.Items.Add(item);
            }
            if (lockForConf == true) { CboConfig.IsEnabled = false; }

            CboConfig.Text = Class.configuracione;

            //ЛИТЕРА
            string[] literaarray = new string[10];
            literaarray[0] = "";
            literaarray[1] = "П";
            literaarray[2] = "Э";
            literaarray[3] = "Т";
            literaarray[4] = "И";
            literaarray[5] = "О";
            literaarray[6] = "О1";
            literaarray[7] = "О2";
            literaarray[8] = "А";
            literaarray[9] = "Б";
            CboLit.ItemsSource = literaarray;


            // CboDrawingDoc
            string[] drDoc = { "ГК", "ЕК", "МК", "ПК", "Е3", "Е4", "Е5", "Л3", "ВС", "ВП", "ТУ", "ПМ", "ТБ", "РР", "КЕ", "ФО", "ПС", "ЗІ", "КР", "ПЕ3", "ТЗ", "СК" };
            foreach (var item in drDoc)
            {
                CboDrawingDoc.Items.Add(item);
            }

            //РАЗДЕЛ
            ComboBoxSection.Items.Clear();

            string[] razdel = { "Документація", "Комплекси", "Складальні одиниці", "Деталі", "Комплекти", "ЭМ-Сборочные-единицы", "ЭМ-Детали" };
            foreach (string r in razdel)
            {
                ComboBoxSection.Items.Add(r);
            }


            // ЗАПОЛНЕНИЕ СПИСКА МАСС
            CboMass.Items.Clear();
            CboMass.Items.Add("Миллиграммы");
            CboMass.Items.Add("Граммы");
            CboMass.Items.Add("Килограммы");
            CboMass.Items.Add("Фунты");

            // ЗАПОЛНЕНИЕ СПИСКА ТОЧНОСТЕЙ
            CboTol.Items.Clear();
            for (var i = 0; i <= 8; i++)
            {
                CboTol.Items.Add(i);
            }

            // МАССА
            TxtMass.ItemsSource = Class.massaValues;


            mass();
        }

        private void FillForm()
        {
            Clear();
            TxtNumber.Text = Propertiy.Designition;
            DescriptionTxtBox.Text = Propertiy.Name;
            ComboBoxSection.SelectedItem = Propertiy.Division;
            CboLit.SelectedItem = Propertiy.Letter;
            CboConfig.SelectedItem = Class.configuracione;
            CboFormat.SelectedItem = Propertiy.Format;
            TxtMass.SelectedItem = Propertiy.Weight;
        }


        private void Clear()
        {
            TxtNumber.Text = "";
            DescriptionTxtBox.Text = "";
            ComboBoxSection.SelectedItem = "";
            CboLit.SelectedItem = "";
            TxtMass.Text = "";
            CboConfig.SelectedItem = "";
            CboFormat.SelectedItem = "";
        }
        private void mass()
        {
            float singlTemp = 0.0f;
            float mvTemp = 0.0f;
            string strTemp = "";
            float mv = 0;

            // Зависят CboMass, CboTol
            if (true) // Пользовательские настройки массы
            {
                switch (CboMass.SelectedIndex)
                {
                    case 0:
                        singlTemp = 1000000;
                        strTemp = "мг";
                        break;
                    case 1:
                        singlTemp = 1000;
                        strTemp = "г";
                        break;
                    case 2:
                        singlTemp = 1;
                        strTemp = "кг";
                        break;
                    case 3:
                        singlTemp = 0.4536f;
                        strTemp = "ф";
                        break;
                }

                int intTemp = CboTol.SelectedIndex;
                mvTemp = mv * singlTemp;

                double myRoundString = MyRound(mvTemp, Convert.ToInt32(intTemp));

                TxtMass.Text = myRoundString.ToString();

                LblMass.Content = strTemp;
            }
            else // Масса по умолчанию
            {
                mvTemp = mv;

                if (mv > 0.1) // Масса больше 100 грамм
                {
                    var myRoundString = MyRound(mvTemp, 2);

                    TxtMass.Text = myRoundString.ToString();

                    LblMass.Content = "кг";

                    CboMass.SelectedIndex = 2;
                    CboTol.SelectedIndex = 3;
                }
                else // Масса меньше 100 грамм
                {
                    mvTemp = mvTemp * 1000;

                    double myRoundString = MyRound(mvTemp, 1);


                    TxtMass.Text = myRoundString.ToString();
                    LblMass.Content = "г";

                    CboMass.SelectedIndex = 1;
                    CboTol.SelectedIndex = 1;
                }
            }
        }
        // Округление
        private double MyRound(double RoundValue, int PrecValue)
        {
            long j;

            PrecValue = Math.Abs(PrecValue);

            j = 1;

            for (var i = 1; i <= PrecValue; i++)
            {
                RoundValue = RoundValue * 10;
                j = j * 10;
            }

            double Delta = RoundValue - Math.Truncate(RoundValue);

            if (Delta >= 0.5)
            {
                RoundValue = Math.Truncate(RoundValue);
                RoundValue = RoundValue + 1;
            }
            else
            {
                RoundValue = Math.Truncate(RoundValue);
            }

            RoundValue = RoundValue / j;

            return RoundValue;
        }




        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
         
        }

        private void ChkFont_Click(object sender, RoutedEventArgs e)
        {
            
            if (ChkFont.IsChecked == true)
            {
                DescriptionTxtBox.FontSize = 14;
            }
            else
            {
                DescriptionTxtBox.FontSize = 18;
            }
        }

       

        private void Version_Click(object sender, RoutedEventArgs e)
        {
            if (Version.IsChecked == true)
            {
                Propertiy.Designition += "-" + Class.configuracione;
                TxtNumber.Text = Propertiy.Designition;
            }
            else
            {
                Propertiy.Designition = Propertiy.Designition.Replace("-" + Class.configuracione, "");  // (Propertiy.Designition.Length - Class.activeConfigName.Length - 1, );
                TxtNumber.Text = Propertiy.Designition;
            }
        }

        private void CboMass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mass();
        }
        private void CboTol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mass();
        }

        private void ChkFormat_Click(object sender, RoutedEventArgs e)
        {
            if (MForm == 0) // Изменения разрешены
            {
                //Tests(1);
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            ReadPropFromForm();
            Class.SetProperties(CboConfig.Text);

        }
        private void ApplyAndClose_Click(object sender, RoutedEventArgs e)
        {
            MClose = 1;

            //applyMProp();

            Apply_Click(true, e);

            Close();

            Close_Click(true, e);
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (longwarnings != (int)swFileLoadWarning_e.swFileLoadWarning_AlreadyOpen & MDoc != 1)
            //    // Если чертеж не был открыт и редактируется чертеж
            //    {
            //        swApp.QuitDoc(_sDrawName); //  то закрываем его
            //        swModel = swApp.ActivateDoc(Source4);
            //    }
            //    else
            //    {
            //        ok = swDraw.ActivateSheet(strActiveSheetName); // Возвращаем активность листу
            //    }

            //    Close();

            //    if (mRun == 0)
            //    {
            //        //System.Environment.Exit(0);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void CboConfig_DropDownClosed(object sender, EventArgs e)
        {
            if(CboConfig.SelectedItem.ToString() != Class.configuracione)
            {
                Class.configuracione = CboConfig.SelectedItem.ToString();
                Class.GetProperties(Class.configuracione);
                FillControlsWithDefaultValues();
                FillForm();
            }
        }

        private void DeleteProperties_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Profil_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void CboDrawingDoc_DropDownClosed(object sender, EventArgs e)
        {
            
        }

        private void ComboBoxSection_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void ChkBlank_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TxtBlankNumber_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TxtBlankDescription_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EditProp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CboFormat_LayoutUpdated(object sender, EventArgs e)
        {

        }

        private void ChkFormat_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkFormat_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkMassTable_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
