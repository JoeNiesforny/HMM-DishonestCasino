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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ForwardBackwardAlgorithm algorithmForwardBackward;
        ViterbiAlgorithm algorithmViterbi;

        // Start of algorithm variables
        double[,] StateMatrix;
        double[,] ObservationMatrix;
        double[] InitialState;
        int[] ObservationSequence;

        double probabilityOfSequence;
        double probabilityOfSequenceSecond;

        int[] foundedSequence;
        double probabilityOfObservationWhenUsingExactModel;

        List<int[]> allPossibleSequence;
        double sumOfProbabilityOfAllPossibleSequence;
        // End of algorithm variables

        public MainWindow()
        {
            InitializeComponent();
        }

        private void generateModelButton_Click(object sender, RoutedEventArgs e)
        {
            uint diceCount;
            uint throwCount;
            uint successThrowCount;
            try
            {
                diceCount = uint.Parse(diceCountTextBox.Text);
                throwCount = uint.Parse(throwCountTextBox.Text);
                successThrowCount = uint.Parse(successThrowCountTextBox.Text);
            }
            catch (ArgumentException err)
            {
                throw new ArgumentException("Bad format! Please provide natural number.");
            }

            ObservationSequence = new int[throwCount];

            var random = new Random();
            for (uint kick = 0; kick < throwCount; kick++)
            {
                ObservationSequence[kick] = ((int) (random.Next() % diceCount + 1));
            }

            observationSequenceDataGrid.ItemsSource = ObservationSequence;
            //observationMatrixDataGrid.UpdateLayout();
        }

    }
}
