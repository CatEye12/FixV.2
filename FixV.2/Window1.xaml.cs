using System;
using System.Windows;

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
           string[,] mas = Class.PropertiesForEachConf();

           int masRank = mas.Rank;

           
        }
    }
}