using backend.Helpers;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend.ML
{
    public class PresentsPrediction
    {
        [ColumnName("Score")]
        public float[] Score { get; set; }

        public string[] GetTopThreePresents()
        {
            var topScores = Score;
            Array.Sort(topScores);
            topScores = topScores.Take(3).ToArray();
            var topIndexes = new List<int>();
            foreach(var topScore in topScores)
            {
                topIndexes.Add(Array.FindIndex(Score, score => topScore == score));
            }

            return new[] { PresentsHelper.Presents[topIndexes[0]], 
                           PresentsHelper.Presents[topIndexes[1]], 
                           PresentsHelper.Presents[topIndexes[2]] };
        }
    }
}
