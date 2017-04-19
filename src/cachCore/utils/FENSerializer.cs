using System.IO;
using cachCore.models;
using cachCore.enums;
using cachCore.rules;

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
            board.RebuildPieceMapFromBoard();

            // update game status
            board.CheckGameStatus();

            return board;
        }

        public string BoardToFEN(Board board, ItemColor colorToMove)
        {
            string fen = "";

            // add the board spec
            for (int row = 7; row >= 0; row--)
            {
                int col = 0;
                int emptyCount = 0;

                while (col < 8)
                {
                    BoardSquare square = board[row, col];
                    if (!square.IsOccupied())
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fen += emptyCount.ToString();
                            emptyCount = 0;
                        }
                        fen += GetPieceChar(square.Piece);
                    }

                    col++;
                }

                if (emptyCount > 0)
                {
                    fen += emptyCount.ToString();
                }

                if (row != 0)
                {
                    fen += "/";
                }
            }

            // add the next-to-move spec
            fen += colorToMove == ItemColor.Black ? " b " : " w ";

            // add the castle spec
            bool bk = new CastleAttemptValidationHelper(board, ItemColor.Black, isKingSideCastle: true).CanCastle;
            bool bq = new CastleAttemptValidationHelper(board, ItemColor.Black, isKingSideCastle: false).CanCastle;
            bool wk = new CastleAttemptValidationHelper(board, ItemColor.White, isKingSideCastle: true).CanCastle;
            bool wq = new CastleAttemptValidationHelper(board, ItemColor.White, isKingSideCastle: false).CanCastle;
            string cs = "";
            cs += wk ? "K" : "";
            cs += wq ? "Q" : "";
            cs += bk ? "k" : "";
            cs += bq ? "q" : "";
            fen += cs.Length > 0 ? cs + " -" : "- -";
            
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

        private static char GetPieceChar(Piece piece)
        {
            char pieceChar = '?';

            switch (piece.PieceType)
            {
                case PieceType.King:
                    pieceChar = 'k';
                    break;
                case PieceType.Queen:
                    pieceChar = 'q';
                    break;
                case PieceType.Rook:
                    pieceChar = 'r';
                    break;
                case PieceType.Bishop:
                    pieceChar = 'b';
                    break;
                case PieceType.Knight:
                    pieceChar = 'n';
                    break;
                case PieceType.Pawn:
                    pieceChar = 'p';
                    break;
            }

            if (piece.PieceColor == ItemColor.White)
            {
                pieceChar = char.ToUpper(pieceChar);
            }

            return pieceChar;
        }
    }
}
