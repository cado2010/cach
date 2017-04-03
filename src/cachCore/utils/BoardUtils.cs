﻿using cachCore.enums;

namespace cachCore.utils
{
    public class BoardUtils
    {
        /// <summary>
        /// Returns starting row of non-Pawn pieces of the given color
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <returns></returns>
        public static int GetPieceStartRow(ItemColor pieceColor)
        {
            return pieceColor == ItemColor.Black ? 7 : 0;
        }

        /// <summary>
        /// Returns starting row of Pawns of the given color
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <returns></returns>
        public static int GetPawnStartRow(ItemColor pieceColor)
        {
            return pieceColor == ItemColor.Black ? 6 : 1;
        }
    }
}
