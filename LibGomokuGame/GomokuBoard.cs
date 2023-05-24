using System.Text;

namespace LibGomokuGame
{
    /// <summary>
    /// 五子棋棋盘
    /// </summary>
    public record class GomokuBoard
    {
        public GomokuBoard(int size)
        {
            Size = size;
            this.board = new int[size, size];
        }


        private int[,] board;

        /// <summary>
        /// 大小
        /// </summary>
        public int Size { get; }

        public void Set(int x, int y, int value)
        {
            if (x < 0 || x >= Size)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Size)
                throw new ArgumentOutOfRangeException(nameof(y));
            if (value < 0 || value > 26)
                throw new ArgumentOutOfRangeException(nameof(value));

            board[x, y] = value;
        }

        public int Get(int x, int y)
        {
            if (x < 0 || x >= Size)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Size)
                throw new ArgumentOutOfRangeException(nameof(y));

            return board[x, y];
        }

        public int[,] GetBoardArray()
        {
            int[,] board_copy = new int[Size, Size];
            Array.Copy(board, board_copy, board_copy.Length);

            return board_copy;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int value = board[x, y];

                    char cellChar =
                        value == 0 ? ' ' : (char)('A' + (value - 1));

                    sb.Append(cellChar);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}