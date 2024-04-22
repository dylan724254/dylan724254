﻿using System.Text;
using System.Text.RegularExpressions;

namespace ProfileEdit
{
    internal class Program
    {
        //const string pathPrefix = @"../../../../..";
        const string pathPrefix = @"";
        const string profileFileName = "README.md";

        //const string resourcePathPrefix = @"../../../../../ProfileEdit/Resource";
        const string resourcePathPrefix = @"ProfileEdit/Resource";
        const string stateFileName = "state.txt";

        const string _circle = "<img src=\"https://github.com/dylan724254/dylan724254/blob/main/ProfileEdit/Img/o2.jpg?raw=true\" style=\"width:180px; height:180px\">";
        const string _crosses = "<img src=\"https://github.com/dylan724254/dylan724254/blob/main/ProfileEdit/Img/x2.jpg?raw=true\" style=\"width:180px; height:180px\">";
        const string _none = "<img src=\"https://github.com/dylan724254/dylan724254/blob/main/ProfileEdit/Img/none2.jpg?raw=true\" style=\"width:180px; height:180px\">";
        static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                return;
            }

            var site = args[0];

            if (!Regex.IsMatch(site, "([A,B,C][1,2,3]|RE)"))
            {
                return;
            }

            if (site == "RE")
            {
                Restart();
                return;
            }

            var boardState = GetState();
            var ticTacType = GetNextType(boardState);

            var row = Convert.ToInt32(site.Substring(1, 1)) - 1;
            var col = site.Substring(0, 1) switch
            {
                "A" => 0,
                "B" => 1,
                "C" => 2,
                _ => throw new Exception(),
            };

            var siteState = boardState[row, col];

            if (siteState != 0)
            {
                return;
            }

            boardState[row, col] = ticTacType;

            SaveState(boardState);

            var stateText = StateToDisplay(boardState);

            var filePath = Path.Combine(pathPrefix, profileFileName);

            File.Delete(filePath);
            using var writer = File.CreateText(filePath);
            writer.WriteLine(GetTitle(boardState));
            writer.WriteLine(stateText);
            writer.WriteLine(GetSelectArea(boardState));
        }

        private static string GetTitle(int[,] boardState)
        {
            var nextType = GetNextType(boardState);
            var winner = CheckWinner(boardState);

            if (winner == 1 || winner == 2)
            {
                return $"獲勝者: {(winner == 1 ? "Ｏ" : "Ｘ")} 請重新開始下一局<br/><br/>";
            }

            return $"下一個 {(nextType == 1 ? "Ｏ" : "Ｘ")}<br/><br/>";
        }

        private static string GetSelectArea(int[,] boardState)
        {
            var text = new StringBuilder();
            text.AppendLine("<br/><br/>選擇位置<br/>");
            text.AppendLine();
            text.AppendLine("|1|2|3|");
            text.AppendLine("|:----:|:----:|:----:|");
            for (int i = 0; i <= 2; i++)
            {
                text.AppendLine($"|{GetSelectPoint(boardState, i, 0)}|{GetSelectPoint(boardState, i, 1)}|{GetSelectPoint(boardState, i, 2)}|");
            }
            text.AppendLine("<br/>");
            text.AppendLine($"重啟新局: <a href=\"https://github.com/dylan724254/dylan724254/issues/new?body=%E8%AB%8B%E9%BB%9E%E6%93%8ASubmit%20new%20issue%E4%B8%8D%E9%9C%80%E8%A6%81%E4%BF%AE%E6%94%B9%E4%BB%BB%E4%BD%95%E5%85%A7%E5%AE%B9Thanks&title=RE\">點擊</a>");

            return text.ToString();
        }

        private static string GetSelectPoint(int[,] boardState, int row, int col)
        {
            if (boardState[row, col] != 0)
            {
                return "　";
            }

            var targetSite = row switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                _ => throw new NotImplementedException()
            } + (col + 1);

            return $"<a href=\"https://github.com/dylan724254/dylan724254/issues/new?body=%E8%AB%8B%E9%BB%9E%E6%93%8ASubmit%20new%20issue%E4%B8%8D%E9%9C%80%E8%A6%81%E4%BF%AE%E6%94%B9%E4%BB%BB%E4%BD%95%E5%85%A7%E5%AE%B9Thanks&title={targetSite}\">點</a>";
        }

        private static int[,] GetState()
        {
            var state = string.Empty;
            var filePath = Path.Combine(resourcePathPrefix, stateFileName);

            using (var reader = new StreamReader(filePath))
            {
                state = reader.ReadLine();
            }

            var boardState = new int[3, 3];
            var stateNum = state!.Split(",").Select(x => Convert.ToInt32(x)).ToArray();

            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    boardState[i, j] = stateNum[i * 3 + j];
                }
            }

            return boardState;
        }

        private static void SaveState(int[,] boardState)
        {
            var filePath = Path.Combine(resourcePathPrefix, stateFileName);
            File.Delete(filePath);
            using var writer = File.CreateText(filePath);
            writer.WriteLine(string.Join(",", ConverToSingleArray(boardState)));
        }

        private static string StateToDisplay(int[,] boardState)
        {
            var text = new StringBuilder();

            for (int i = 0; i <= 2; i++)
            {
                text.Append("<div style=\"display: inline-block;\">");
                var line = new string[3];
                for (int j = 0; j <= 2; j++)
                {
                    line[j] = boardState[i, j] switch
                    {
                        0 => _none,
                        1 => _circle,
                        2 => _crosses,
                    };
                }

                text.Append(string.Join("", line));
                text.Append("</div>");
            }

            return text.ToString();
        }

        static int CheckWinner(int[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                if ((board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    || (board[0, i] == board[1, i] && board[1, i] == board[2, i]))
                {
                    if (board[i, 0] != 0)
                    {
                        return board[i, 0];
                    }
                }
            }

            if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
            {
                if (board[0, 0] != 0)
                {
                    return board[0, 0];
                }
            }

            if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
            {
                if (board[0, 2] != 0)
                {
                    return board[0, 2];
                }
            }

            if (ConverToSingleArray(board).All(x => x == 0))
            {
                return -1;
            }

            return 0;
        }
        private static int[] ConverToSingleArray(int[,] array)
        {
            var newArray = new int[array.GetLength(0) * array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    newArray[i * 3 + j] = array[i, j];
                }
            }

            return newArray;
        }

        private static int GetNextType(int[,] state)
        {
            return ConverToSingleArray(state).AsEnumerable().Count(x => x == 1) <= ConverToSingleArray(state).AsEnumerable().Count(x => x == 2)
                ? 1
                : 2;
        }

        private static void Restart()
        {
            var newBoardState = new int[3, 3];
            var nextType = GetNextType(newBoardState);

            SaveState(newBoardState);
            var stateText = StateToDisplay(newBoardState);

            var filePath = Path.Combine(pathPrefix, profileFileName);
            File.Delete(filePath);
            using var writer = File.CreateText(filePath);
            writer.WriteLine($"下一個 {(nextType == 1 ? "Ｏ" : "Ｘ")}<br/>");
            writer.WriteLine(stateText);
            writer.WriteLine(GetSelectArea(newBoardState));
        }
    }
}
