using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using log4net;
using cachCore.enums;
using cachCore.exceptions;
using cachCore.utils;
using cachCore.rules;

namespace cachCore.models
{
    public class Board
    {
        /// <summary>
        /// Main board of squares
        /// </summary>
        [JsonProperty]
        private BoardSquare[,] _board;

        /// <summary>
        /// Used for move begin/commit phases - this is not game history
        /// </summary>
        [JsonProperty]
        private BoardHistory _boardHistory;

        /// <summary>
        /// map: <PieceColor> -> { map: <PieceType> -> IList<Piece> }
        /// where King list must be [1] size
        /// </summary>
        [JsonIgnore]
        private Dictionary<ItemColor, Dictionary<PieceType, IList<Piece>>> _pieceMap;

        /// <summary>
        /// Material taken so far
        /// </summary>
        [JsonProperty]
        private readonly Dictionary<ItemColor, IList<Piece>> _killedMaterial;

        [JsonProperty]
        private int _previousHistoryLevel;

        [JsonIgnore]
        private ILog _logger;

        [JsonIgnore]
        private static ILog _staticLogger;

        static Board()
        {
            _staticLogger = LogManager.GetLogger(typeof(Board).Name);
        }

        public Board(bool initStartingPosition = true)
        {
            _logger = LogManager.GetLogger(GetType().Name);

            _board = new BoardSquare[8, 8]; // [row, col]
            _boardHistory = new BoardHistory();

            _killedMaterial = new Dictionary<ItemColor, IList<Piece>>();
            _killedMaterial[ItemColor.Black] = new List<Piece>();
            _killedMaterial[ItemColor.White] = new List<Piece>();

            Init(initStartingPosition);

            _logger.Debug($"Board: created");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsWithinPieceRange(Piece piece, Position target)
        {
            bool result = false;

            // get constrainted movement of given piece
            Movement m = GetMovement(piece);

            // check if target is within this piece range
            result = m.Includes(target);

            return result;
        }

        /// <summary>
        /// Called in the middle of movement phases (after Begin and before Commit)
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <returns></returns>
        public bool IsInCheck(ItemColor pieceColor)
        {
            // see if King of given color can be attacked
            King king = GetPieces(pieceColor, PieceType.King)[0] as King;

            InCheckHelper helper = new InCheckHelper(this, pieceColor, king.Position);
            return helper.IsInCheck;
        }

        public BoardSquare this[int row, int column]
        {
            get { return _board[row, column]; }
        }

        public BoardSquare this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
        }

        /// <summary>
        /// Returns list of current pieces of given color, type from map
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <param name="pieceType"></param>
        /// <returns></returns>
        public IList<Piece> GetPieces(ItemColor pieceColor, PieceType pieceType)
        {
            return _pieceMap[pieceColor][pieceType];
        }

        /// <summary>
        /// Returns a new IList of all currently active pieces of the given color
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <returns></returns>
        public IList<Piece> GetAllActivePieces(ItemColor pieceColor)
        {
            List<Piece> activePieces = new List<Piece>();

            Dictionary<PieceType, IList<Piece>> activeMap = _pieceMap[pieceColor];
            activePieces.AddRange(activeMap.SelectMany(kv => kv.Value));

            return activePieces;
        }

        /// <summary>
        /// Saves current board into given file path in JSON format
        /// </summary>
        /// <param name="path"></param>
        public void WriteToFile(string path)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(this,
                    new JsonSerializerSettings
                    { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented });
                File.WriteAllText(path, jsonString);
            }
            catch (Exception ex)
            {
                _logger.Error("WriteToFile: exception: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Reads board from a given file path
        /// </summary>
        /// <param name="path"></param>
        public static Board ReadFromFile(string path)
        {
            try
            {
                string jsonString = File.ReadAllText(path);
                var b = JsonConvert.DeserializeObject<Board>(jsonString,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

                // repopulate active pieces
                b.RebuildPieceMap();

                return b;
            }
            catch(Exception ex)
            {
                _staticLogger.Error("ReadFromFile: exception: " + ex.Message, ex);
            }

            return null;
        }

        /// <summary>
        /// Rebuilds active piece map from current board (used when deserializing from JSON)
        /// </summary>
        public void RebuildPieceMap()
        {
            CreatePieceMap(true);

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    BoardSquare square = this[row, col];
                    Piece piece = square.Piece;
                    if (piece != null)
                    {
                        _pieceMap[piece.PieceColor][piece.PieceType].Add(piece);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the possible movement paths for the give piece with constraints
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public Movement GetMovement(Piece piece)
        {
            Movement m = piece.GetMovement();

            // prune paths that are blocked due to board population
            IList<IList<Position>> constrainedPaths = new List<IList<Position>>();

            foreach (var path in m.Paths)
            {
                List<Position> constrainedPath = new List<Position>();

                // prune path if blocked by own color (exclude)
                // prune path if blocked by opponent color (include)
                foreach (var pos in path)
                {
                    BoardSquare square = this[pos];

                    // special processing for Pawns required when attempting to move out of File
                    if (piece.PieceType == PieceType.Pawn && !pos.IsSameFile(piece.Position))
                    {
                        // out of file movement allowed for Pawn only if there is an opponent there
                        if (square.IsOccupied() && !square.IsOccupiedByPieceOfColor(piece.PieceColor))
                        {
                            constrainedPath.Add(pos);
                        }

                        // TODO: implement en passant rule checks here
                    }
                    else
                    {
                        if (square.IsOccupiedByPieceOfColor(piece.PieceColor))
                        {
                            break;
                        }

                        constrainedPath.Add(pos);

                        if (square.IsOccupied())
                        {
                            break;
                        }
                    }
                }

                if (constrainedPath.Count > 0)
                {
                    constrainedPaths.Add(constrainedPath);
                }
            }

            return new Movement(m.Start, constrainedPaths, true);
        }

        /// <summary>
        // Moves or returns invalid move
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public MoveErrorType Move(ItemColor pieceColor, string move)
        {
            try
            {
                MoveInputParser mip = null;

                try
                {
                    mip = new MoveInputParser(move);
                }
                catch(CachException ex)
                {
                    _logger.Error($"Move[{pieceColor}]: exception during parse: {ex.Message}, move: {move}", ex);
                    return MoveErrorType.InvalidFormat;
                }

                if (!mip.IsValid)
                {
                    _logger.Info($"Move[{pieceColor}]: invalid format: {move}");
                    return MoveErrorType.InvalidFormat;
                }

                IList<Piece> pieces = GetMoveInputPieces(pieceColor, mip);
                if (pieces == null || pieces.Count == 0)
                {
                    // no such piece
                    _logger.Info($"Move[{pieceColor}]: no applicable pieces: {move}");
                    return MoveErrorType.NoSuchPiece;
                }

                // if more than one piece, then check for range of each and if the
                // target is within range of more than one piece, then invalid move
                Piece pieceToMove = null;
                foreach (var piece in pieces)
                {
                    if (IsWithinPieceRange(piece, mip.TargetPosition))
                    {
                        if (pieceToMove != null)
                        {
                            // more than one piece can reach the target - invalid move
                            _logger.Info($"Move[{pieceColor}]: more than one piece can reach this position: {move}");
                            return MoveErrorType.MoreThanOnePieceInRange;
                        }
                        else
                        {
                            pieceToMove = piece;
                        }
                    }
                }

                if (pieceToMove == null)
                {
                    // no piece in range
                    _logger.Info($"Move[{pieceColor}]: none of the pieces can reach this position: {move}");
                    return MoveErrorType.NoPieceInRange;
                }

                // if we reach here, then there is a potential piece that can be moved
                // so attempt the move and check for Check of this color, if in Check, then move is invalid
                bool pieceKilled = MoveBegin(pieceToMove, mip.TargetPosition);

                // piece must get killed for kill spec in move input - or else its an invalid move spec
                if (mip.IsKill && !pieceKilled)
                {
                    // invalid kill
                    _logger.Info($"Move[{pieceColor}]: nothing to kill: {move}");

                    // revert the steps that MoveBegin() did
                    MoveRevert();
                    return MoveErrorType.InvalidKill;
                }

                var inCheckHelper = new InCheckHelper(this, pieceColor);
                if (inCheckHelper.IsInCheck)
                {
                    // King cannot be in check after move
                    _logger.Info($"Move[{pieceColor}]: King in check: {move}");

                    // revert the steps that MoveBegin() did
                    MoveRevert();
                    return MoveErrorType.KingInCheck;
                }

                // if we reach here, then the move is valid
                MoveCommit();

                _logger.Debug($"Move[{pieceColor}]: valid move: {move}");
                return MoveErrorType.Ok;
            }
            catch (CachException ex)
            {
                _logger.Error($"Move[{pieceColor}]: Cach exception: {ex.Message} during move: {move}");
                return MoveErrorType.CachError;
            }
        }

        //-------------------------------------------------------------------------------
        // private impl

        /// <summary>
        /// Initializes game board with pieces and positions
        /// </summary>
        private void Init(bool initStartingPosition)
        {
            // create BoardSquares
            for (int row = 0; row < 8; row++)
            {
                ItemColor squareColor = row % 2 == 0 ? ItemColor.Black : ItemColor.White;

                for (int col = 0; col < 8; col++)
                {
                    _board[row, col] = new BoardSquare(new Position(row, col), squareColor);
                    squareColor = squareColor == ItemColor.Black ? ItemColor.White : ItemColor.Black;
                }
            }

            if (initStartingPosition)
            {
                CreatePieceMap(false);

                // create pieces and place in Board
                InitAndPlacePieces(ItemColor.Black);
                InitAndPlacePieces(ItemColor.White);
            }
            else
            {
                CreatePieceMap(true);
            }
        }

        private void InitAndPlacePieces(ItemColor pieceColor)
        {
            int row = BoardUtils.GetPieceStartRow(pieceColor);

            List<Piece> king = new List<Piece>() { new King(pieceColor, new Position(row, 4)) };
            this[row, 4].SetPiece(king[0]);

            List<Piece> queen = new List<Piece>() { new Queen(pieceColor, new Position(row, 3)) };
            this[row, 3].SetPiece(queen[0]);

            List<Piece> rooks = new List<Piece>()
            {
                new Rook(pieceColor, new Position(row, 0)),
                new Rook(pieceColor, new Position(row, 7)),
            };
            this[row, 0].SetPiece(rooks[0]);
            this[row, 7].SetPiece(rooks[1]);

            List<Piece> bishops = new List<Piece>()
            {
                new Bishop(pieceColor, new Position(row, 2)),
                new Bishop(pieceColor, new Position(row, 5)),
            };
            this[row, 2].SetPiece(bishops[0]);
            this[row, 5].SetPiece(bishops[1]);

            List<Piece> knights = new List<Piece>()
            {
                new Knight(pieceColor, new Position(row, 1)),
                new Knight(pieceColor, new Position(row, 6)),
            };
            this[row, 1].SetPiece(knights[0]);
            this[row, 6].SetPiece(knights[1]);

            // adjust row for pawns
            row = BoardUtils.GetPawnStartRow(pieceColor);

            // create and place pawns
            List<Piece> pawns = new List<Piece>();
            for (int col = 0; col < 8; col++)
            {
                Pawn p = new Pawn(pieceColor, new Position(row, col));
                pawns.Add(p);

                this[row, col].SetPiece(p);
            }

            // set in map
            SetInPieceMap(king);
            SetInPieceMap(queen);
            SetInPieceMap(rooks);
            SetInPieceMap(bishops);
            SetInPieceMap(knights);
            SetInPieceMap(pawns);
        }

        private void SetInPieceMap(IList<Piece> pieces)
        {
            ItemColor pieceColor = pieces[0].PieceColor;
            PieceType pieceType = pieces[0].PieceType;
            _pieceMap[pieceColor][pieceType] = pieces;
        }

        private void CreatePieceMap(bool initLists)
        {
            // create Piece Map and optionally initialize the lists too
            if (initLists)
            {
                _pieceMap = new Dictionary<ItemColor, Dictionary<PieceType, IList<Piece>>>()
                {
                    {
                        ItemColor.Black,
                        new Dictionary<PieceType, IList<Piece>>()
                        {
                            {  PieceType.King,  new List<Piece>() },
                            {  PieceType.Queen,  new List<Piece>() },
                            {  PieceType.Rook,  new List<Piece>() },
                            {  PieceType.Bishop,  new List<Piece>() },
                            {  PieceType.Knight,  new List<Piece>() },
                            {  PieceType.Pawn,  new List<Piece>() },
                        }
                    },
                    {
                        ItemColor.White,
                        new Dictionary<PieceType, IList<Piece>>()
                        {
                            {  PieceType.King,  new List<Piece>() },
                            {  PieceType.Queen,  new List<Piece>() },
                            {  PieceType.Rook,  new List<Piece>() },
                            {  PieceType.Bishop,  new List<Piece>() },
                            {  PieceType.Knight,  new List<Piece>() },
                            {  PieceType.Pawn,  new List<Piece>() },
                        }
                    }
                };
            }
            else
            {
                _pieceMap = new Dictionary<ItemColor, Dictionary<PieceType, IList<Piece>>>()
                {
                    { ItemColor.Black, new Dictionary<PieceType, IList<Piece>>() },
                    { ItemColor.White, new Dictionary<PieceType, IList<Piece>>() }
                };
            }
        }

        private IList<Piece> GetMoveInputPieces(ItemColor pieceColor, MoveInputParser mip)
        {
            IList<Piece> mipPieces;

            IList<Piece> pieces = GetPieces(pieceColor, mip.PieceType);
            if (mip.IsStartPositionInfoAvailable)
            {
                mipPieces = pieces.Where(p =>
                    p.Position.IsSameFile(mip.StartPosition) ||
                    p.Position.IsSameRank(mip.StartPosition)).ToList();
            }
            else
            {
                mipPieces = pieces;
            }

            return mipPieces;
        }

        private bool MoveBegin(Piece piece, Position target)
        {
            bool pieceKilled = false;
            BoardSquare square = this[target];

            // sanity check
            if (square.IsOccupiedByPieceOfColor(piece.PieceColor))
            {
                throw new CachException("Cannot move to square occupied your color: " + piece.PieceColor);
            }

            _previousHistoryLevel = _boardHistory.Level;

            if (square.IsOccupied())
            {
                Piece previousPiece = square.Piece;

                // record enemy alive status
                _boardHistory.PushAliveStatus(previousPiece);
                mKill(previousPiece);

                pieceKilled = true;
            }

            // record piece current pos
            _boardHistory.PushPosition(piece);
            mMove(piece, target);

            return pieceKilled;
        }

        private void mMove(Piece piece, Position target)
        {
            // remove piece from previous square
            this[piece.Position].RemovePiece();

            // move piece to new pos
            piece.MoveTo(target);

            // set piece in current square
            this[target].SetPiece(piece);
        }

        private void mKill(Piece piece)
        {
            piece.Kill();

            // remove from active piece map
            IList<Piece> activePieces = GetPieces(piece.PieceColor, piece.PieceType);
            activePieces.Remove(piece);

            // move into gobbled material
            _killedMaterial[piece.PieceColor].Add(piece);
        }

        private void mUnkill(Piece piece)
        {
            piece.Unkill();

            // remove from gobbled material
            _killedMaterial[piece.PieceColor].Remove(piece);

            // add to active piece map
            IList<Piece> activePieces = GetPieces(piece.PieceColor, piece.PieceType);
            activePieces.Add(piece);

            // set unkilled piece back into BoardSquare
            this[piece.Position].SetPiece(piece);
        }

        private void MovePromoteBegin(Piece pieceOriginal, Piece piecePromoted, Position target)
        {
            // TODO: implement MoveBegin with promotion support
        }

        private void MoveCommit(Piece piecePromoted = null)
        {
            // TODO:
            // record move into Move History
            // if piecePromoted is not null, add to piece map
        }

        private void MoveRevert()
        {
            // pop history until level reaches _previousHistoryLevel and undo each
            while (_boardHistory.Level != _previousHistoryLevel)
            {
                mRevert();
            }
        }

        /// <summary>
        /// Pops top of BoardHistory stack and reverts that step
        /// </summary>
        private void mRevert()
        {
            BoardHistoryItem hi = _boardHistory.Pop();
            Piece piece = null;

            switch (hi.Type)
            {
                case BoardHistoryType.PieceAliveStatus:
                    // bring piece back alive
                    PieceAliveStatusHistoryItem pahi = hi as PieceAliveStatusHistoryItem;
                    piece = Piece.Get(pahi.PieceId);
                    mUnkill(piece);
                    break;

                case BoardHistoryType.PiecePosition:
                    // move piece back to original position
                    PiecePositionHistoryItem pphi = hi as PiecePositionHistoryItem;
                    piece = Piece.Get(pphi.PieceId);
                    mMove(piece, pphi.Position);
                    break;

                case BoardHistoryType.PiecePromotion:
                    // TODO:
                    break;
            }
        }
    }
}
