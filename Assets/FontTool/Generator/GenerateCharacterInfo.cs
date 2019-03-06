using UnityEngine;

namespace FontTool.Generator
{
    public static class GenerateCharacterInfo
    {
        /// <summary>
        /// Generate an Array of Character Info to be Used by the Custom Font
        /// </summary>
        /// <param name="size"></param>
        /// <param name="advanced"></param>
        /// <param name="startAscii"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="uv"></param>
        /// <param name="vert"></param>
        /// <returns></returns>
        public static CharacterInfo[] CharacterInfos(int size, int advanced, int columns, int rows,
            Rect uv, Rect vert, int startAscii)
        {
            CharacterInfo[] _characterInfos = new CharacterInfo[size];

            int index = 0;
            for (int r = rows - 1; r >= 0; r--)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (index == size) break;

                    CharacterInfo characterInfo = new CharacterInfo
                    {
                        advance = advanced,
                        index   = startAscii + index,
                        uv      = UV(c, r, uv.width, uv.height),
                        vert    = Vert((int) vert.x, (int) vert.y, (int) vert.width, (int) vert.height)
                    };


                    _characterInfos[index] = characterInfo;

                    index++;
                }
            }

            return _characterInfos;
        }

        /// <summary>
        /// UV Component of the Character Info
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="uvW"></param>
        /// <param name="uvH"></param>
        /// <returns></returns>
        private static Rect UV(float column, float row, float uvW, float uvH)
        {
            return new Rect(column * uvW, row * uvH, uvW, uvH);
        }


        /// <summary>
        /// Vert Component of the Character Info
        /// </summary>
        /// <param name="horizontalPadding"></param>
        /// <param name="verticalPadding"></param>
        /// <param name="vertW"></param>
        /// <param name="vertH"></param>
        /// <returns></returns>
        private static Rect Vert(int horizontalPadding, int verticalPadding, int vertW, int vertH)
        {
            return new Rect(horizontalPadding, verticalPadding, vertW, vertH);
        }
    }
}