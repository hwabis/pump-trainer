using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class NextHitObjectGenerator
    {
        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeft = new()
        {
            { Column.P1DL, [Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR] },
            { Column.P1UL, [Column.P1DL, Column.P1C, Column.P1UR, Column.P1DR] },
            { Column.P1C, [Column.P1UR, Column.P1DR, Column.P2DL, Column.P2UL] },
            { Column.P1UR, [Column.P1DR, Column.P2DL, Column.P2UL, Column.P2C] },
            { Column.P1DR, [Column.P1UR, Column.P2DL, Column.P2UL, Column.P2C] },
            { Column.P2DL, [Column.P1UR, Column.P2UL, Column.P2C, Column.P2UR, Column.P2DR] },
            { Column.P2UL, [Column.P1DR, Column.P2DL, Column.P2C, Column.P2UR, Column.P2DR] },
            { Column.P2C, [Column.P2UR, Column.P2DR] },
            { Column.P2UR, [Column.P2DR] },
            { Column.P2DR, [Column.P2UR] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRight = [];

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeftHorizontalTwist = new()
        {
            { Column.P1C, [Column.P1DL, Column.P1UL] },
            { Column.P1UR, [Column.P1C] },
            { Column.P1DR, [Column.P1C] },
            { Column.P2C, [Column.P2DL, Column.P2UL] },
            { Column.P2UR, [Column.P2C] },
            { Column.P2DR, [Column.P2C] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRightHorizontalTwist = [];

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeftDiagonalSkip = new()
        {
            { Column.P1UR, [Column.P1DL] },
            { Column.P1DR, [Column.P1UL] },
            { Column.P2UR, [Column.P2DL] },
            { Column.P2DR, [Column.P2UL] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRightDiagonalSkip = [];

        private Dictionary<Column, Column> horizontalFlips = new()
        {
            { Column.P1DL, Column.P2DR },
            { Column.P1UL, Column.P2UR },
            { Column.P1C, Column.P2C },
            { Column.P1UR, Column.P2UL },
            { Column.P1DR, Column.P2DL },
            { Column.P2DL, Column.P1DR },
            { Column.P2UL, Column.P1UR },
            { Column.P2C, Column.P1C },
            { Column.P2UR, Column.P1UL },
            { Column.P2DR, Column.P1DL },
        };

        private ImmutableDictionary<Column, int> columnToPhysicalColumn = new Dictionary<Column, int>()
        {
            { Column.P1DL, 0 },
            { Column.P1UL, 0 },
            { Column.P1C, 1 },
            { Column.P1UR, 2 },
            { Column.P1DR, 2 },
            { Column.P2DL, 3 },
            { Column.P2UL, 3 },
            { Column.P2C, 4 },
            { Column.P2UR, 5 },
            { Column.P2DR, 5 },
        }.ToImmutableDictionary();

        private static Random random = new();

        private List<PumpTrainerHitObject> hitObjectsSoFar = [];

        private Foot? previousFoot = null;
        private Column? previousColumn = null;

        public PumpTrainerBeatmapConverterSettings Settings = new();

        public NextHitObjectGenerator()
        {
            initializeFootRightDictionaries();
        }

        public PumpTrainerHitObject GetNextHitObject(double startTime, IBeatmap beatmap)
        {
            // Always start on the left foot as the first note (for now?)
            Foot nextFoot = previousFoot == null || previousFoot == Foot.Right ? Foot.Left : Foot.Right;

            Column nextColumn = previousColumn == null ? Settings.AllowedColumns[random.Next(Settings.AllowedColumns.Count())]
                : getNextColumn(nextFoot, (Column)previousColumn);

            PumpTrainerHitObject nextHitObject = new(nextColumn)
            {
                IntendedFoot = nextFoot,
                StartTime = startTime,
            };

            hitObjectsSoFar.Add(nextHitObject);

            previousFoot = nextFoot;
            previousColumn = nextColumn;

            return nextHitObject;
        }

        private Column getNextColumn(Foot nextFoot, Column previousColumn)
        {
            List<Column> candidateColumns = getCandidateColumns(nextFoot, previousColumn, false);
            includeOnlyAllowedColumns(candidateColumns);
            banColumnsCausingBannedPatterns(candidateColumns, nextFoot == Foot.Left ? Foot.Right : Foot.Left);

            if (candidateColumns.Count == 0)
            {
                // We messed up. Do the whole thing again but this time, minimize the bans and maximize twists.
                candidateColumns = getCandidateColumns(nextFoot, previousColumn, true);
                includeOnlyAllowedColumns(candidateColumns);
                // Banning patterns here is still safe because if worst case is that we're stuck trilling between two notes
                banColumnsCausingBannedPatterns(candidateColumns, nextFoot == Foot.Left ? Foot.Right : Foot.Left);
            }

            return getRandomCandidateColumn(candidateColumns, 4 /* magic number to reduce likeihood of trills */);
        }

        private List<Column> getCandidateColumns(Foot nextFoot, Column previousColumn, bool maximizeCandidateCount)
        {
            List<Column> candidateColumns = (nextFoot == Foot.Left ?
                nextColumnsPreviousFootRight[previousColumn] : nextColumnsPreviousFootLeft[previousColumn]).ToList();

            possiblyAddHorizontalTwistsToCandidates(candidateColumns, previousColumn, maximizeCandidateCount);
            possiblyAddDiagonalSkipsToCandidates(candidateColumns, previousColumn, maximizeCandidateCount);

            // Easy mod bans
            possiblyBanSinglesTwists(candidateColumns, previousColumn, maximizeCandidateCount);
            possiblyBanFarColumns(candidateColumns, previousColumn, maximizeCandidateCount);

            return candidateColumns;
        }

        private void possiblyAddHorizontalTwistsToCandidates(List<Column> candidates, Column previousColumn, bool alwaysAdd)
        {
            if (random.NextDouble() < Settings.HorizontalTwistFrequency || alwaysAdd)
            {
                List<Column> twistColumnsToAdd;

                if (previousFoot == Foot.Left)
                {
                    if (nextColumnsPreviousFootLeftHorizontalTwist.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootLeftHorizontalTwist[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (nextColumnsPreviousFootRightHorizontalTwist.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootRightHorizontalTwist[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }

                candidates.AddRange(twistColumnsToAdd);
            }
        }

        private void possiblyAddDiagonalSkipsToCandidates(List<Column> candidates, Column previousColumn, bool alwaysAdd)
        {
            if (random.NextDouble() < Settings.DiagonalSkipFrequency || alwaysAdd)
            {
                List<Column> twistColumnsToAdd;

                if (previousFoot == Foot.Left)
                {
                    if (nextColumnsPreviousFootLeftDiagonalSkip.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootLeftDiagonalSkip[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (nextColumnsPreviousFootRightDiagonalSkip.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootRightDiagonalSkip[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }

                candidates.AddRange(twistColumnsToAdd);
            }
        }

        private void possiblyBanSinglesTwists(List<Column> candidates, Column previousColumn, bool neverBan)
        {
            if (neverBan)
            {
                return;
            }

            if (random.NextDouble() > Settings.SinglesTwistFrequency)
            {
                if (previousFoot == Foot.Left)
                {
                    switch (previousColumn)
                    {
                        case Column.P1DL:
                            candidates.Remove(Column.P1UL);
                            return;
                        case Column.P1UL:
                            candidates.Remove(Column.P1DL);
                            return;
                        case Column.P2DL:
                            candidates.Remove(Column.P2UL);
                            return;
                        case Column.P2UL:
                            candidates.Remove(Column.P2DL);
                            return;
                    }
                }
                else
                {
                    switch (previousColumn)
                    {
                        case Column.P1DR:
                            candidates.Remove(Column.P1UR);
                            return;
                        case Column.P1UR:
                            candidates.Remove(Column.P1DR);
                            return;
                        case Column.P2DR:
                            candidates.Remove(Column.P2UR);
                            return;
                        case Column.P2UR:
                            candidates.Remove(Column.P2DR);
                            return;
                    }
                }
            }
        }

        private void possiblyBanFarColumns(List<Column> candidates, Column previousColumn, bool neverBan)
        {
            if (neverBan)
            {
                return;
            }

            if (random.NextDouble() > Settings.FarColumnsFrequency)
            {
                List<Column> columnsToBan;
                int previousPhysicalColumn = columnToPhysicalColumn[previousColumn];

                if (previousColumn == Column.P1C || previousColumn == Column.P2C)
                {
                    // Just ban everything that's two columns away
                    columnsToBan =
                        columnToPhysicalColumn
                        .Where(entry => entry.Value == previousPhysicalColumn + 2 || entry.Value == previousPhysicalColumn - 2)
                        .Select(entry => entry.Key)
                        .ToList();
                }
                else
                {
                    // Ban everything that's two columns away and that's also a center panel
                    columnsToBan =
                        columnToPhysicalColumn
                        .Where(entry =>
                            (entry.Value == previousPhysicalColumn + 2 || entry.Value == previousPhysicalColumn - 2)
                                && (entry.Key == Column.P1C || entry.Key == Column.P2C))
                        .Select(entry => entry.Key)
                        .ToList();
                }

                candidates.RemoveAll(columnsToBan.Contains);
            }
        }

        private void includeOnlyAllowedColumns(List<Column> candidateColumns)
        {
            foreach (Column column in candidateColumns.ToList())
            {
                if (!Settings.AllowedColumns.Contains(column))
                {
                    candidateColumns.Remove(column);
                }
            }
        }

        /// <summary>
        /// Bans columns the given list of candidates based on the columns two most recent hit objects in the generated beatmap.
        /// </summary>
        /// <param name="candidateColumns">Candidate columns to ban from.</param>
        /// <param name="previousFoot">Associated foot of the most recent hit object in the beatmap.</param>
        private void banColumnsCausingBannedPatterns(List<Column> candidateColumns, Foot previousFoot)
        {
            if (hitObjectsSoFar.Count <= 1)
            {
                // No possible bans if we're only on the first or second note
                return;
            }

            Column previousColumn = hitObjectsSoFar[^1].Column;
            Column previousPreviousColumn = hitObjectsSoFar[^2].Column;

            // Ban spins no matter what
            if (previousFoot == Foot.Left)
            {
                if (previousColumn == Column.P1C)
                {
                    if (previousPreviousColumn == Column.P1DL)
                    {
                        candidateColumns.Remove(Column.P1UL);
                    }
                    else if (previousPreviousColumn == Column.P1UL)
                    {
                        candidateColumns.Remove(Column.P1DL);
                    }
                }
                else if (previousColumn == Column.P2C)
                {
                    if (previousPreviousColumn == Column.P2DL || previousPreviousColumn == Column.P1DR)
                    {
                        candidateColumns.Remove(Column.P2UL);
                        candidateColumns.Remove(Column.P1UR);
                    }
                    else if (previousPreviousColumn == Column.P2UL || previousPreviousColumn == Column.P1UR)
                    {
                        candidateColumns.Remove(Column.P2DL);
                        candidateColumns.Remove(Column.P1DR);
                    }
                }
            }
            else if (previousFoot == Foot.Right)
            {
                if (previousColumn == Column.P1C)
                {
                    if (previousPreviousColumn == Column.P1DR || previousPreviousColumn == Column.P2DL)
                    {
                        candidateColumns.Remove(Column.P1UR);
                        candidateColumns.Remove(Column.P2UL);
                    }
                    else if (previousPreviousColumn == Column.P1UR || previousPreviousColumn == Column.P2UL)
                    {
                        candidateColumns.Remove(Column.P1DR);
                        candidateColumns.Remove(Column.P2DL);
                    }
                }
                else if (previousColumn == Column.P2C)
                {
                    if (previousPreviousColumn == Column.P2DR)
                    {
                        candidateColumns.Remove(Column.P2UR);
                    }
                    else if (previousPreviousColumn == Column.P2UR)
                    {
                        candidateColumns.Remove(Column.P2DR);
                    }
                }
            }

            // Ban large diagonal twists and large horizontal twists no matter what
            if (previousColumn == Column.P1C)
            {
                if (previousPreviousColumn == Column.P1DL)
                {
                    candidateColumns.Remove(Column.P2UL);
                    candidateColumns.Remove(Column.P2DL);
                }
                else if (previousPreviousColumn == Column.P1UL)
                {
                    candidateColumns.Remove(Column.P2DL);
                    candidateColumns.Remove(Column.P2UL);
                }
                else if (previousPreviousColumn == Column.P2DL)
                {
                    candidateColumns.Remove(Column.P1UL);
                    candidateColumns.Remove(Column.P1DL);
                }
                else if (previousPreviousColumn == Column.P2UL)
                {
                    candidateColumns.Remove(Column.P1DL);
                    candidateColumns.Remove(Column.P1UL);
                }
            }
            else if (previousColumn == Column.P2C)
            {
                if (previousPreviousColumn == Column.P2DR)
                {
                    candidateColumns.Remove(Column.P1UR);
                    candidateColumns.Remove(Column.P1DR);
                }
                else if (previousPreviousColumn == Column.P2UR)
                {
                    candidateColumns.Remove(Column.P1DR);
                    candidateColumns.Remove(Column.P1UR);
                }
                else if (previousPreviousColumn == Column.P1DR)
                {
                    candidateColumns.Remove(Column.P2UR);
                    candidateColumns.Remove(Column.P2DR);
                }
                else if (previousPreviousColumn == Column.P1UR)
                {
                    candidateColumns.Remove(Column.P2DR);
                    candidateColumns.Remove(Column.P2UR);
                }
            }

            // Ban diagonal twists (single pad)
            if (random.NextDouble() > Settings.DiagonalTwistFrequency)
            {
                if (previousColumn == Column.P1C)
                {
                    if (previousPreviousColumn == Column.P1DL)
                    {
                        candidateColumns.Remove(Column.P1UR);
                    }
                    else if (previousPreviousColumn == Column.P1UL)
                    {
                        candidateColumns.Remove(Column.P1DR);
                    }
                    else if (previousPreviousColumn == Column.P1DR)
                    {
                        candidateColumns.Remove(Column.P1UL);
                    }
                    else if (previousPreviousColumn == Column.P1UR)
                    {
                        candidateColumns.Remove(Column.P1DL);
                    }
                }
                else if (previousColumn == Column.P2C)
                {
                    if (previousPreviousColumn == Column.P2DR)
                    {
                        candidateColumns.Remove(Column.P2UL);
                    }
                    else if (previousPreviousColumn == Column.P2UR)
                    {
                        candidateColumns.Remove(Column.P2DL);
                    }
                    else if (previousPreviousColumn == Column.P2DL)
                    {
                        candidateColumns.Remove(Column.P2UR);
                    }
                    else if (previousPreviousColumn == Column.P2UL)
                    {
                        candidateColumns.Remove(Column.P2DR);
                    }
                }
            }

            // Ban horizontal triples
            if (random.NextDouble() > Settings.HorizontalTripleFrequency)
            {
                int previousPhysicalColumn = columnToPhysicalColumn[previousColumn];
                int previousPreviousPhysicalColumn = columnToPhysicalColumn[previousPreviousColumn];

                List<Column> columnsToBan = [];

                if (previousPhysicalColumn > previousPreviousPhysicalColumn)
                {
                    // Going right! Ban everything to the right
                    columnsToBan = columnToPhysicalColumn.Where(entry => entry.Value > previousPhysicalColumn).Select(entry => entry.Key).ToList();
                }
                else if (previousPhysicalColumn < previousPreviousPhysicalColumn)
                {
                    // Going left! Ban everything to the keft
                    columnsToBan = columnToPhysicalColumn.Where(entry => entry.Value < previousPhysicalColumn).Select(entry => entry.Key).ToList();
                }

                // Don't ban columns that are in the exception case (all three notes are on the same singles pad).
                // Just manually check all four possibilities for this exception
                if (previousPhysicalColumn == 1 && previousPreviousPhysicalColumn == 0)
                {
                    columnsToBan.RemoveAll(column => columnToPhysicalColumn[column] == 2);
                }
                if (previousPhysicalColumn == 1 && previousPreviousPhysicalColumn == 2)
                {
                    columnsToBan.RemoveAll(column => columnToPhysicalColumn[column] == 0);
                }
                if (previousPhysicalColumn == 4 && previousPreviousPhysicalColumn == 3)
                {
                    columnsToBan.RemoveAll(column => columnToPhysicalColumn[column] == 5);
                }
                if (previousPhysicalColumn == 4 && previousPreviousPhysicalColumn == 5)
                {
                    columnsToBan.RemoveAll(column => columnToPhysicalColumn[column] == 3);
                }

                candidateColumns.RemoveAll(columnsToBan.Contains);
            }
        }

        private Column getRandomCandidateColumn(List<Column> candidateColumns, int nonRepeatExtraWeight)
        {
            if (candidateColumns.Count == 0)
            {
                throw new Exception("There were no candidate columns :(");
            }

            if (hitObjectsSoFar.Count < 2)
            {
                return candidateColumns[random.Next(candidateColumns.Count)];
            }

            // Reduce the likelihood of trills using nonRepeatWeight

            Column previousPreviousColumn = hitObjectsSoFar[^2].Column;
            List<Column> candidateColumnsWeighted = [];

            foreach (Column candidateColumn in candidateColumns)
            {
                candidateColumnsWeighted.Add(candidateColumn);

                if (candidateColumn != previousPreviousColumn)
                {
                    for (int i = 0; i < nonRepeatExtraWeight; i++)
                    {
                        candidateColumnsWeighted.Add(candidateColumn);
                    }
                }

                if (Settings.SinglesTwistFrequency > 0)
                {
                    if (candidateColumn == Column.P1C || candidateColumn == Column.P2C)
                    {
                        for (int i = 0; i < Settings.SinglesTwistFrequency; i++)
                        {
                            candidateColumnsWeighted.Add(candidateColumn);
                        }
                    }
                }
            }

            return candidateColumnsWeighted[random.Next(candidateColumnsWeighted.Count)];
        }

        private void initializeFootRightDictionaries()
        {
            foreach (Column column in Enum.GetValues(typeof(Column)))
            {
                Column flippedColumn = horizontalFlips[column];

                if (nextColumnsPreviousFootLeft.ContainsKey(flippedColumn))
                {
                    nextColumnsPreviousFootRight[column] = nextColumnsPreviousFootLeft[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                }

                if (nextColumnsPreviousFootLeftHorizontalTwist.ContainsKey(flippedColumn))
                {
                    nextColumnsPreviousFootRightHorizontalTwist[column] = nextColumnsPreviousFootLeftHorizontalTwist[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                }

                if (nextColumnsPreviousFootLeftDiagonalSkip.ContainsKey(flippedColumn))
                {
                    nextColumnsPreviousFootRightDiagonalSkip[column] = nextColumnsPreviousFootLeftDiagonalSkip[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                }
            }
        }
    }
}
