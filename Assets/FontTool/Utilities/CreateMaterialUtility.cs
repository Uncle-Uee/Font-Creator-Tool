using System.IO;
using UnityEditor;
using UnityEngine;

namespace CFC.Utilities
{
    public static class CreateMaterialUtility
    {
        /// <summary>
        /// Create a Material Asset From a Script
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="color"></param>
        /// <param name="shader"></param>
        /// <returns></returns>
        public static Material CreateMaterial(Texture texture, Color color, Shader shader = null)
        {
            string savePath =
                AssetDatabase.GetAssetPath(texture)
                    .Replace(texture.name + Path.GetExtension(AssetDatabase.GetAssetPath(texture)), "");

            Material _material = new Material(shader ?? Shader.Find("Standard"));
            _material.mainTexture = texture;
            _material.color       = color;

            AssetDatabase.CreateAsset(_material, savePath + $"/{texture.name}.mat");

            return AssetDatabase.LoadAssetAtPath<Material>(savePath + $"/{texture.name}.mat");
        }
    }
}