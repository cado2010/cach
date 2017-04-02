using System.Collections.Generic;
using cachCore.enums;
using cachCore.exceptions;
using cachCore.models;

namespace cachCore.utils
{
    /// <summary>
    /// Parses input in algebraic notation like Nf3 Qxe4 etc
    /// </summary>
    public class MoveInputParser
    {
        private readonly ItemColor _pieceColor;
        private string _input;
        private string _lowerInput;
        private readonly Dictionary<char, PieceType> _charToPieceTypeMap;

        public MoveInputParser(ItemColor pieceColor, string input)
        {
            _pieceColor = pieceColor;
            _input = input.Trim();
            _lowerInput = _input.ToLower();

            _charToPieceTypeMap = new Dictionary<char, PieceType>()
            {
                { 'k', PieceType.King },
                { 'q', PieceType.Queen },
                { 'r', PieceType.Rook },
                // { 'B', PieceType.Bishop }, // not used as hard coded rule
                { 'n', PieceType.Knight },
                { 'a', PieceType.Pawn },
                { 'b', PieceType.Pawn },
                { 'c', PieceType.Pawn },
                { 'd', PieceType.Pawn },
                { 'e', PieceType.Pawn },
                { 'f', PieceType.Pawn },
                { 'g', PieceType.Pawn },
                { 'h', PieceType.Pawn },
            };

            Parse();
        }

        public ItemColor PieceColor
        {
            get { return _pieceColor; }
        }

        public PieceType PieceType { get; private set; }

        /// <summary>
        /// Where input specifies piece to move to
        /// </summary>
        public Position TargetPosition { get; private set; }

        /// <summary>
        /// Where input specifies piece is moving from (this may be partial)
        /// </summary>
        public Position StartPosition { get; private set; }

        public bool IsKingSideCastle { get; private set; }

        public bool IsQueenSideCastle { get; private set; }

        public bool IsKill { get; private set; }

        private int _startRow;
        private int _startColumn;

        private void Parse()
        {
            _startRow = Position.InvalidCoordinate;
            _startColumn = Position.InvalidCoordinate;

            StartPosition = Position.Invalid;
            TargetPosition = Position.Invalid;

            if (_input.Length < 2)
            {
                throw new CachException("Movement input string has to be >= 2 chars");
            }

            if (_lowerInput == "o-o")
            {
                PieceType = PieceType.King;
                IsKingSideCastle = true;
                return;
            }
            if (_lowerInput == "o-o-o")
            {
                PieceType = PieceType.King;
                IsQueenSideCastle = true;
                return;
            }

            if (_input.Length > 6)
            {
                // for now, truncate and dont allow
                _input = _input.Substring(0, 6);
                _lowerInput = _input.ToLower();
            }

            switch (_input.Length)
            {
                case 2:
                    PieceType = PieceType.Pawn;
                    TargetPosition = Position.FromAlgebraic(_lowerInput);
                    break;

                case 3:
                    CheckSetPieceType();
                    if (PieceType == PieceType.Pawn)
                    {
                        SetOptions(_lowerInput[0]);
                    }
                    TargetPosition = Position.FromAlgebraic(_lowerInput.Substring(1));
                    break;

                case 4:
                    CheckSetPieceType();
                    if (PieceType == PieceType.Pawn)
                    {
                        SetOptions(_lowerInput[0]);
                    }
                    SetOptions(_lowerInput[1]);
                    TargetPosition = Position.FromAlgebraic(_lowerInput.Substring(2));
                    break;

                case 5:
                    CheckSetPieceType();
                    if (PieceType == PieceType.Pawn)
                    {
                        SetOptions(_lowerInput[0]);
                    }
                    SetOptions(_lowerInput[1]);
                    SetOptions(_lowerInput[2]);
                    TargetPosition = Position.FromAlgebraic(_lowerInput.Substring(3));
                    break;

                case 6:
                    CheckSetPieceType();
                    SetOptions(_lowerInput[1]);
                    SetOptions(_lowerInput[2]);
                    SetOptions(_lowerInput[3]);
                    TargetPosition = Position.FromAlgebraic(_lowerInput.Substring(4));
                    break;
            }

            StartPosition = new Position(_startRow, _startColumn);
        }

        private void CheckSetPieceType()
        {
            if (_input[0] == 'B')
            {
                PieceType = PieceType.Bishop;
                return;
            }
            if (!_charToPieceTypeMap.ContainsKey(_lowerInput[0]))
            {
                throw new CachException("Invalid first char in input: " + _input);
            }
            PieceType = _charToPieceTypeMap[_lowerInput[0]];
        }

        private void SetOptions(char opt)
        {
            if (opt == 'x')
            {
                IsKill = true;
            }
            else if (char.IsDigit(opt))
            {
                _startRow = Position.RowFromRank(opt);
            }
            else if (char.IsLetter(opt))
            {
                _startColumn = Position.ColumnFromFile(opt);
            }
        }
    }
}
