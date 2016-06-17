using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DishonestCasino;

namespace AlgorithmUnitTests
{
    [Subject("AlgorithmForwardBackward")]
    public class AlgorithmUnitTests
    {
        static double[,] StateMatrix;
        static double[,] ObservationMatrix;
        static double[] InitialState;
        static int[] ObservationSequence;
        static AlgorithmForwardBackward algorithm;

        static double probabilityOfSequence;
        static double probabilityOfSequenceSecond;

        static int[] foundedSequence;
        static double probabilityOfOccurenceFoundedSequence;

        Establish context = () =>
        {
            StateMatrix = new double[,] { {0.7, 0.3 }, {0.4, 0.6 } };
            ObservationMatrix = new double[,] { { 0.1, 0.4, 0.5 }, { 0.7, 0.2, 0.1 } };
            InitialState = new double[] { 0.6, 0.4 };
            ObservationSequence = new int[] { 0, 1, 0, 2 };
            algorithm = new AlgorithmForwardBackward(StateMatrix, ObservationMatrix, InitialState, ObservationSequence);
        };

        Because of = () => 
        {
            probabilityOfSequence = Math.Round(algorithm.ProbabilityOfStateSequence(new int[] { 0, 0, 1, 1 }), 6);
            probabilityOfSequenceSecond = Math.Round(algorithm.ProbabilityOfStateSequence(new int[] { 1, 1, 1, 0 }), 6);

            probabilityOfOccurenceFoundedSequence = algorithm.FindStateSequence(out foundedSequence);
        };

        It ProbabilityOfStateSequenceShouldBeEqualToReferenceResult = () =>
        {
            probabilityOfSequence.ShouldEqual(0.000212);
            probabilityOfSequenceSecond.ShouldEqual(0.002822);
        };

        It AlgorithmFoundTheMostProbablyStateSequenceFromObservation = () => foundedSequence.ShouldEqual(new int[] { 1, 1, 1, 0 });

        Cleanup after = () =>
        {
        };
    }
}
