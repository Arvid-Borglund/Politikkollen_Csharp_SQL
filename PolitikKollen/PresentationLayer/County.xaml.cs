using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for County.xaml
    /// </summary>
    public partial class County : Window
    {
        public County()
        {
            InitializeComponent();
        }

        Controller.MainController controller = new();

        private void btnAddCounty_Click(object sender, RoutedEventArgs e)
        {
            String county = txtBoxAddCounty.Text;

        }


    }
}
