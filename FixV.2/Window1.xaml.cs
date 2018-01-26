using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FixV._2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
           
            FillGrid();
        }
        private void FillGrid()
        {
            this.Show();
            Class.configChanged = false;
            Class.configIterator = 0;
            DataGridCheckBoxColumn checkVersion;
            DataGridComboBoxColumn cmbBoxCol;
            DataGridTextColumn textColumn;
            string[] colNames = new string[] { "Конфигурация", "Обозначение", "Наименование", "Масса" };
            foreach (var item in colNames)
            {
                textColumn = new DataGridTextColumn();
                textColumn.Header = item;
                textColumn.Binding = new Binding(item);
                if (item == "Конфигурация") { textColumn.IsReadOnly = true; }
                dataGrid.Columns.Add(textColumn);
            }

            // COMBOBOX
            cmbBoxCol = new DataGridComboBoxColumn();
            cmbBoxCol.Header = "Раздел";
            cmbBoxCol.ItemsSource = Class.razdel;
            cmbBoxCol.SelectedItemBinding = new Binding("Раздел");
            dataGrid.Columns.Add(cmbBoxCol);

            //CHECHBOX
            checkVersion = new DataGridCheckBoxColumn();
            checkVersion.Header = "Версия";
            checkVersion.IsThreeState = false;
            checkVersion.Binding = new Binding("Версия");
            dataGrid.Columns.Add(checkVersion);

            dataGrid.ItemsSource = WorkWithCommonConfFixer.PropertiesForEachConf().AsDataView();
        }

        private void SaveChangesOnGrid_Click(object sender, RoutedEventArgs e)
        {
            DataView dt = (DataView)dataGrid.ItemsSource;
            WorkWithCommonConfFixer.GetValuesFromGrid(dt.Table);
        }

        private void UndoChanges_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangingGridDynamically(object sender, RoutedEventArgs e)
        {

        }
    }
    public class CustomDataGridCheckBoxColumn : DataGridCheckBoxColumn
    {

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {

            CheckBox checkBox = base.GenerateEditingElement(cell, dataItem) as CheckBox;
            checkBox.Checked += new RoutedEventHandler(HandleClick);
            checkBox.Unchecked += new RoutedEventHandler(HandleClick);
            return checkBox;
        }
        public void HandleClick(object sender, RoutedEventArgs e)
        {
        }
    }
}