using cachCore.enums;

namespace cachCore.models
{
    public class BoardMove
    {
        /// <summary>
        /// This is the "official" move number which consists of both a White and a Black move
        /// </summary>
        public int MoveNumber { get; set; }

        public int MoveStepNumber { get; set; }
        public ItemColor PieceColor { get; set; }
        public string Move { get; set; }
    }
}
