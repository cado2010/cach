namespace cachCore.enums
{
    public enum MoveErrorType
    {
        Ok = 0,
        CachError,
        UnknownError,
        InvalidFormat,
        NoSuchPiece,
        MoreThanOnePieceInRange,
        NoPieceInRange,
        KingInCheck,
        InvalidKill,
        InvalidCastle
    }
}
