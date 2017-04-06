using System.IO;
using cachCore.models;
using cachCore.enums;

namespace cachCore.utils
{
    public class FENSerializer
    {
        /// <summary>
        /// Creates a new Board from a text file containing a FEN string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Board BoardFromFENFile(string path)
        {
            return BoardFromFEN(File.ReadAllText(path));
        }

        /// <summary>
        /// Creates a new Board from FEN string
        /// examples: 
        ///   rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -
        ///   8/8/8/3k4/3Q4/3K4/8/8 w - -
        /// </summary>
        /// <param name="fen"></param>
        /// <returns></returns>
        public static Board BoardFromFEN(string fen)
        {
            // this will create a blank board with no pieces
            Board board = new Board(false);

            // parse FEN string and walk thru rows (FEN left starts at row 7)
            int parseIndex = 0;
            for (int row = 7; row >= 0; row--)
            {
                int col = 0;
                while (fen[parseIndex] != '/' && col < 8)
                {
                    char c = fen[parseIndex];
                    if (char.IsDigit(c))
                    {
                        // skip that many squares
                        for (int i = 0; i < char.GetNumericValue(c); i++)
                        {
                            col++;
                        }
                    }
                    else
                    {
                        Piece p = CreatePiece(c, new Position(row, col));
                        board[row, col].SetPiece(p);
                        col++;
                    }

                    parseIndex++;
                }
                parseIndex++;
            }

            // recreate active piece map from current Board
            board.RebuildPieceMap();

            return board;
        }

        public string BoardToFEN(Board board)
        {
            string fen = "";

            for (int row = 7; row >= 0; row--)
            {
                for (int col = 0; col < 8; col++)
                {
                }
            }

            return fen;
        }

        private static Piece CreatePiece(char pc, Position pos)
        {
            Piece p = null;
            ItemColor pieceColor = char.IsUpper(pc) ? ItemColor.White : ItemColor.Black;

            switch (char.ToLower(pc))
            {
                case 'k':
                    p = new King(pieceColor, pos);
                    break;
                case 'q':
                    p = new Queen(pieceColor, pos);
                    break;
                case 'r':
                    p = new Rook(pieceColor, pos);
                    break;
                case 'b':
                    p = new Bishop(pieceColor, pos);
                    break;
                case 'n':
                    p = new Knight(pieceColor, pos);
                    break;
                case 'p':
                    p = new Pawn(pieceColor, pos);
                    break;
                default:
                    // TODO: log error
                    break;
            }

            return p;
        }
    }
}
