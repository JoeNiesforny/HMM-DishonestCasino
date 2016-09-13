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
            uint diceCount;
            uint throwCount;
            try
            {
                diceCount = uint.Parse(diceCountTextBox.Text);
                throwCount = uint.Parse(throwCountTextBox.Text);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Bad format! Please provide natural number.");
            }
            model = new Model(diceCount, throwCount);
            observationSequenceDataGrid.ItemsSource = model.ObservationSequence;
            initialMatrixDataGrid.ItemsSource = model.InitialState;
            stateMatrixDataGrid.ItemsSource = model.StateMatrix;
            observationMatrixDataGrid.ItemsSource = model.ObservationMatrix;
        }

        private void compute_button_Click(object sender, RoutedEventArgs e)
        {
            model.Compute();
            FBAStateSequenceDataGrid.ItemsSource = model.ResultForwardBackward.FoundedSequence;
            FBAProbabiltyTextBlock.Text = "Probability: " + model.ResultForwardBackward.ProbabilityOfSequence;
            ViterbiStateSequenceDataGrid.ItemsSource = model.ResultViterbi.FoundedSequence;
            ViterbiProbabiltyTextBlock.Text = "Probability: " + model.ResultViterbi.ProbabilityOfSequence;
            newInitialMatrixDataGrid.ItemsSource = model.NewModelForwardBackward.InitialState;
            newStateMatrixDataGrid.ItemsSource = model.NewModelForwardBackward.StateMatrix;
            newObservationMatrixDataGrid.ItemsSource = model.NewModelForwardBackward.ObservationMatrix;
        }
    }
}
