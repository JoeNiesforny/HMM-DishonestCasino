using DishonestCasino;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM_DishonestCasinoApp
{
    public static class Algorithm
    {
        public static ForwardBackwardAlgorithm AlgorithmForwardBackward;
        public static ViterbiAlgorithm AlgorithmViterbi;
    }

    public class Model
    {
        private readonly int _accurarcy = 15;
        public uint EventCount;
        public uint ThrowCount;

        public DataTable StateMatrix;
        public DataTable ObservationMatrix;
        public DataTable InitialState;
        public DataTable ObservationSequence;

        private int[] _observationSequence;
        private double[] _initialState;
        private double[,] _stateMatrix;
        private double[,] _observationMatrix;

        public Result ResultViterbi;
        public Result ResultForwardBackward;
        public Model NewModelForwardBackward;

        // Generate new model
        public Model(uint diceCount, uint throwCount)
        {
            EventCount = diceCount;
            ThrowCount = throwCount;
            var random = new Random();
            _observationSequence = new int[throwCount];
            for (uint kick = 0; kick < throwCount; kick++)
                _observationSequence[kick] = (int)(random.Next() % diceCount);
            _initialState = new double[2];
            var value = Math.Round(random.NextDouble(), _accurarcy);
            _initialState[0] = value;
            _initialState[1] = Math.Abs(value - 1);
            _stateMatrix = new double[2, 2];
            for (uint kick = 0; kick < 2; kick++)
            {
                value = Math.Round(random.NextDouble(), _accurarcy);
                _stateMatrix[kick, 0] = value;
                _stateMatrix[kick, 1] = Math.Abs(value - 1);
            }
            _observationMatrix = new double[2, diceCount];
            for (uint i = 0; i < 2; i++)
            {
                double[] values = new double[diceCount];
                for (uint j = 0; j < diceCount; j++)
                    values[j] = Math.Round(random.NextDouble(), _accurarcy);
                var divider = (values.Sum()) / 1;
                for (uint j = 0; j < diceCount; j++)
                    _observationMatrix[i, j] = Math.Round(values[j] / divider, _accurarcy);
            }
            Converter.ConvertToDataTable(out ObservationSequence, _observationSequence);
            Converter.ConvertToDataTable(out InitialState, _initialState);
            Converter.ConvertToDataTable(out StateMatrix, _stateMatrix);
            Converter.ConvertToDataTable(out ObservationMatrix, _observationMatrix);
        }

        // Get a new model with known initial, state and observation matrix
        Model(double [] initial, double[,] state, double[,] observation)
        {
            _initialState = initial;
            _stateMatrix = state;
            _observationMatrix = observation;
            Converter.ConvertToDataTable(out InitialState, _initialState);
            Converter.ConvertToDataTable(out StateMatrix, _stateMatrix);
            Converter.ConvertToDataTable(out ObservationMatrix, _observationMatrix);
        }

        private void Gather()
        {
            Converter.ConvertToArray(out _observationMatrix, ObservationMatrix);
            Converter.ConvertToArray(out _stateMatrix, StateMatrix);
            Converter.ConvertToArray(out _initialState, InitialState);
            Converter.ConvertToArray(out _observationSequence, ObservationSequence);
        }

        public void Compute()
        {
            Gather();
            // ForwardBackward
            {
                Algorithm.AlgorithmForwardBackward = new ForwardBackwardAlgorithm(_stateMatrix, _observationMatrix, _initialState, _observationSequence);
                int[] newStates;
                var result = Algorithm.AlgorithmForwardBackward.FindStateSequence(out newStates);
                ResultForwardBackward = new Result(newStates, result);
                double[] initial;
                double[,] state;
                double[,] observation;
                Algorithm.AlgorithmForwardBackward.GetNewModel(out initial, out state, out observation);
                NewModelForwardBackward = new Model(initial, state, observation);
            }
            // Viterbi
            {
                Algorithm.AlgorithmViterbi = new ViterbiAlgorithm(_stateMatrix, _observationMatrix, _initialState, _observationSequence);
                int[] newStates;
                var result = Algorithm.AlgorithmViterbi.FindStateSequence(out newStates);
                ResultViterbi = new Result(newStates, result);
            }
        }

        public DataView ShowResult()
        {
            var result = new DataTable();
            result.Columns.Add(new DataColumn("Forward&Backward sequence"));
            result.Columns.Add(new DataColumn("Forward&Backward probability"));
            result.Columns.Add(new DataColumn("Viterbi sequence"));
            result.Columns.Add(new DataColumn("Viterbi probability"));
            for (int i = 0; i < _observationSequence.Length; i++)
            {
                var newRow = result.NewRow();
                if (i == 0)
                {
                    newRow[1] = ResultForwardBackward.ProbabilityOfSequence;
                    newRow[3] = ResultViterbi.ProbabilityOfSequence;
                }
                else
                    newRow[1] = newRow[3] = "";
                newRow[0] = ResultForwardBackward.FoundedSequence[i];
                newRow[2] = ResultViterbi.FoundedSequence[i];
                result.Rows.Add(newRow);
            }
            return result.DefaultView;
        }
    }

    public class Result
    {
        public double ProbabilityOfSequence { get; set; }
        public int[] FoundedSequence;
        public Result(int[] foundedStates, double probabilityOfSequence)
        {
            FoundedSequence = foundedStates;
            ProbabilityOfSequence = probabilityOfSequence;
        }
    }

    static class Converter
    {
        public static void ConvertToDataTable(out DataTable table, int[] array)
        {
            table = new DataTable();
            table.Columns.Add();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                var newRow = table.NewRow();
                newRow[0] = array[i];
                table.Rows.Add(newRow);
            }
            table.DefaultView.AllowNew = false;
            table.DefaultView.AllowDelete = false;
        }
        public static void ConvertToArray(out int[] array, DataTable table)
        {
            array = new int[table.Rows.Count];
            for (var row = 0; row < table.Rows.Count; row++)
                array[row] = int.Parse(table.Rows[row][0].ToString().Replace('.', ','));
        }
        public static void ConvertToDataTable(out DataTable table, double[] array)
        {
            table = new DataTable();
            for (int i = 0; i < array.GetLength(0); i++)
                table.Columns.Add();
            var newRow = table.NewRow();
            for (int i = 0; i < array.GetLength(0); i++)
                newRow[i] = array[i];
            table.Rows.Add(newRow);
            table.DefaultView.AllowNew = false;
            table.DefaultView.AllowDelete = false;
        }
        public static void ConvertToArray(out double[] array, DataTable table)
        {
            array = new double[table.Columns.Count];
            for (var i = 0; i < table.Columns.Count; i++)
                array[i] = double.Parse(table.Rows[0][i].ToString().Replace('.', ','));
        }
        public static void ConvertToDataTable(out DataTable table, double[,] array)
        {
            table = new DataTable();
            for (int i = 0; i < array.GetLength(1); i++)
                table.Columns.Add();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                var newRow = table.NewRow();
                for (int j = 0; j < array.GetLength(1); j++)
                    newRow[j] = array[i, j];
                table.Rows.Add(newRow);
            }
            table.DefaultView.AllowNew = false;
            table.DefaultView.AllowDelete = false;
        }
        public static void ConvertToArray(out double[,] array, DataTable table)
        {
            array = new double[table.Rows.Count, table.Columns.Count];
            for (var i = 0; i < table.Columns.Count; i++)
            {
                for (int j = 0; j < table.Rows.Count; j++)
                    array[j, i] = double.Parse(table.Rows[j][i].ToString().Replace('.', ','));
            }
        }
    }
}
