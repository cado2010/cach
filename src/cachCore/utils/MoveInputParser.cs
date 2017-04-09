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
        private string _input;
        private string _lowerInput;
        private readonly static Dictionary<char, PieceType> _charToPieceTypeMap;

        public MoveInputParser(string input)
        {
            _input = input.Trim();
            _lowerInput = _input.ToLower();

            Parse();
        }

        public MoveDescriptor MoveDescriptor { get; private set; }

        static MoveInputParser()
        {
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
        }

        //-------------------------------------------------------------------------------

        /// <summary>
        /// Piece Type specified by this move, must be available
        /// </summary>
        private PieceType _pieceType;

        /// <summary>
        /// Where input specifies piece to move to, must be specified or Castle
        /// </summary>
        private Position _targetPosition;

        /// <summary>
        /// Where input specifies piece is moving from (this may be partial or unavailable)
        /// </summary>
        private Position _startPosition;

        private bool _isKingSideCastle;

        private bool _isQueenSideCastle;

        private bool _isKill;

        private int _startRow;
        private int _startColumn;

        private bool _isDrawOffer;

        private void Parse()
        {
            _pieceType = PieceType.Unknown;

            _startRow = Position.InvalidCoordinate;
            _startColumn = Position.InvalidCoordinate;

            _startPosition = Position.Invalid;
            _targetPosition = Position.Invalid;

            if (_input.Length < 2)
            {
                throw new CachException("Movement input string has to be >= 2 chars");
            }

            if (_lowerInput == "(=)")
            {
                _isDrawOffer = true;
                CreateMoveDescriptor();
                return;
            }
            if (_lowerInput == "o-o")
            {
                _pieceType = PieceType.King;
                _isKingSideCastle = true;
                CreateMoveDescriptor();
                return;
            }
            if (_lowerInput == "o-o-o")
            {
                _pieceType = PieceType.King;
                _isQueenSideCastle = true;
                CreateMoveDescriptor();
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
                    _pieceType = PieceType.Pawn;
                    _targetPosition = Position.FromAlgebraic(_lowerInput);
                    break;

                case 3:
                    CheckSetPieceType();
                    if (_pieceType == PieceType.Pawn)
                    {
                        SetOptions(_lowerInput[0]);
                    }
                    _targetPosition = Position.FromAlgebraic(_lowerInput.Substring(1));
                    break;

                case 4:
                    CheckSetPieceType();
                    if (_pieceType == PieceType.Pawn)
                    {
                        SetOptions(_lowerInput[0]);
                    }
                    SetOptions(_lowerInput[1]);
                    _targetPosition = Position.FromAlgebraic(_lowerInput.Substring(2));
                    break;

                case 5:
                    CheckSetPieceType();
                    if (_pieceType == PieceType.Pawn)
                    {
                        SetOptions(_lowerInput[0]);
                    }
                    SetOptions(_lowerInput[1]);
                    SetOptions(_lowerInput[2]);
                    _targetPosition = Position.FromAlgebraic(_lowerInput.Substring(3));
                    break;

                case 6:
                    CheckSetPieceType();
                    SetOptions(_lowerInput[1]);
                    SetOptions(_lowerInput[2]);
                    SetOptions(_lowerInput[3]);
                    _targetPosition = Position.FromAlgebraic(_lowerInput.Substring(4));
                    break;
            }

            _startPosition = new Position(_startRow, _startColumn);

            CreateMoveDescriptor();
        }

        private void CreateMoveDescriptor()
        {
            MoveDescriptor = new MoveDescriptor()
            {
                PieceType = _pieceType,
                TargetPosition = _targetPosition,
                StartPosition = _startPosition,
                IsKingSideCastle = _isKingSideCastle,
                IsQueenSideCastle = _isQueenSideCastle,
                IsKill = _isKill,
                IsDrawOffer = _isDrawOffer
            };
        }

        private void CheckSetPieceType()
        {
            if (_input[0] == 'B')
            {
                _pieceType = PieceType.Bishop;
                return;
            }
            if (!_charToPieceTypeMap.ContainsKey(_lowerInput[0]))
            {
                throw new CachException("Invalid first char in input: " + _input);
            }
            _pieceType = _charToPieceTypeMap[_lowerInput[0]];
        }

        private void SetOptions(char opt)
        {
            if (opt == 'x')
            {
                _isKill = true;
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
