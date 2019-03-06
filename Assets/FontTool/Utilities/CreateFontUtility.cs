using UnityEngine;
using UnityEditor;

namespace CFC.Utilities
{
    public static class CreateFontUtility
    {
        /// <summary>
        /// Create a Custom Font with Character Info Properties.
        /// </summary>
        /// <param name="fontName"></param>
        /// <param name="material"></param>
        /// <param name="characterInfos"></param>
        /// <returns></returns>
        public static Font CreateFont(string fontName = "default", Material material = null,
            CharacterInfo[] characterInfos = null)
        {
            Font _font = new Font
            {
                name          = fontName,
                material      = material,
                characterInfo = characterInfos ?? new CharacterInfo[0]
            };

            string savePath = material
                ? AssetDatabase.GetAssetPath(material).Replace($"{material.name}.mat", $"{fontName}.fontsettings")
                : $"Assets/{fontName}.fontsettings";

            AssetDatabase.CreateAsset(_font, savePath);

            return AssetDatabase.LoadAssetAtPath<Font>(savePath);
        }
    }
}