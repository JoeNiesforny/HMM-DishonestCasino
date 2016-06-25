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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HMM_DishonestCasinoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static double[,] StateMatrix;
        static double[,] ObservationMatrix;
        static double[] InitialState;
        static int[] ObservationSequence;

        private void initializeModel()
        {
            StateMatrix = new double[,] { { 0.7, 0.3 }, { 0.4, 0.6 } };
            ObservationMatrix = new double[,] { { 0.1, 0.4, 0.5 }, { 0.7, 0.2, 0.1 } };
            InitialState = new double[] { 0.6, 0.4 };
            ObservationSequence = new int[] { 0, 1, 0, 2 };
        }

        public MainWindow()
        {
            initializeModel();
            InitializeComponent();
        }

        private void generateModelButton_Click(object sender, RoutedEventArgs e)
        {
            var diceCount = int.Parse(diceCountTextBox.Text);
            var throwCount = int.Parse(throwCountTextBox.Text);
            var successThrowCount = int.Parse(successThrowCountTextBox.Text);
        }
    }
}
