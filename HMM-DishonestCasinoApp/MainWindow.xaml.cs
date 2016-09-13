using DishonestCasino;
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
    public partial class MainWindow : Window
    {
        Model model;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void generateModelButton_Click(object sender, RoutedEventArgs e)
        {
            uint eventCount;
            uint throwCount;
            try
            {
                eventCount = uint.Parse(diceCountTextBox.Text);
                throwCount = uint.Parse(throwCountTextBox.Text);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Bad format! Please provide natural number.");
            }
            model = new Model(eventCount, throwCount);
            observationSequenceDataGrid.ItemsSource = model.ObservationSequence.DefaultView;
            initialMatrixDataGrid.ItemsSource = model.InitialState.DefaultView;
            stateMatrixDataGrid.ItemsSource = model.StateMatrix.DefaultView;
            observationMatrixDataGrid.ItemsSource = model.ObservationMatrix.DefaultView;
        }

        private void compute_button_Click(object sender, RoutedEventArgs e)
        {
            model.Compute();
            ResultDataGrid.ItemsSource = model.ShowResult();
            newInitialMatrixDataGrid.ItemsSource = model.NewModelForwardBackward.InitialState.DefaultView;
            newStateMatrixDataGrid.ItemsSource = model.NewModelForwardBackward.StateMatrix.DefaultView;
            newObservationMatrixDataGrid.ItemsSource = model.NewModelForwardBackward.ObservationMatrix.DefaultView;
        }
    }
}
