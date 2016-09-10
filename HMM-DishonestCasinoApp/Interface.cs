using DishonestCasino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM_DishonestCasinoApp
{
    public class Algorithm
    {
        public ForwardBackwardAlgorithm algorithmForwardBackward;
        public ViterbiAlgorithm algorithmViterbi;
    }

    // Value of throw is integer.
    public class Observation
    {
        public Observation(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }

    // Value of transition is double.
    public class StateTransition
    {
        public StateTransition(double value1, double value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
        public double Value1 { get; set; }
        public double Value2 { get; set; }
    }
    public class Model
    {
        public uint DiceCount;
        public uint ThrowCount;

        public List<StateTransition> StateMatrix;
        public List<StateTransition> ObservationMatrix;
        public List<StateTransition> InitialState;
        public List<Observation> ObservationSequence;

        private int[] _observationSequence;
        private double[] _initialState;
        private double[,] _stateMatrix;
        private double[,] _observationMatrix;

        public Model(uint diceCount, uint throwCount)
        {
            DiceCount = diceCount;
            ThrowCount = throwCount;
            var random = new Random();

            ObservationSequence = new List<Observation>();
            for (uint kick = 0; kick < throwCount; kick++)
            {
                ObservationSequence.Add(new Observation((int)(random.Next() % diceCount + 1)));
            }

            InitialState = new List<StateTransition>();
            var value = double.Parse(random.NextDouble().ToString().Substring(0,5));
            InitialState.Add(new StateTransition(value, Math.Abs(value - 1)));

            ObservationMatrix = new List<StateTransition>();
            StateMatrix = new List<StateTransition>();
            for (uint kick = 0; kick < 2; kick++)
            {
                value = double.Parse(random.NextDouble().ToString().Substring(0, 5));
                ObservationMatrix.Add(new StateTransition(value, Math.Abs(value - 1)));
                value = double.Parse(random.NextDouble().ToString().Substring(0, 5));
                StateMatrix.Add(new StateTransition(value, Math.Abs(value - 1)));
            }
        }

        private void Gather()
        {
            _observationSequence = new int[ObservationSequence.Count];
            var iter = 0;
            foreach (var v in ObservationSequence)
            {
                _observationSequence[iter] = v.Value;
                iter++;
            }
            _initialState = new double[2];
            _initialState[0] = InitialState[0].Value1;
            _initialState[1] = InitialState[0].Value2;
            _stateMatrix = new double[2,StateMatrix.Count];
            iter = 0;
            foreach (var v in StateMatrix)
            {
                _stateMatrix[0,iter] = v.Value1;
                _stateMatrix[1,iter] = v.Value2;
                iter++;
            }
            _observationMatrix = new double[2, ObservationMatrix.Count];
            iter = 0;
            foreach (var v in ObservationMatrix)
            {
                _observationMatrix[0, iter] = v.Value1;
                _observationMatrix[1, iter] = v.Value2;
                iter++;
            }
        }

    }

    public class Result
    {
        public double ProbabilityOfSequence { get; set; }
        public double ProbabilityOfSequenceSecond { get; set; }

        private int[] _foundedSequence;
        public List<Observation> FoundedSequence;
        public double ProbabilityOfObservationWhenUsingExactModel { get; set; }

        // many list
        public List<Observation> AllPossibleSequence;
        public double SumOfProbabilityOfAllPossibleSequence;
    }
}
