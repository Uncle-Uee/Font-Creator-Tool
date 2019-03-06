using CFC.Utilities;
using FontTool.Generator;
using UnityEditor;
using UnityEngine;

namespace FontTool.Editor
{
    public class FtWindow : EditorWindow
    {
        #region CREATE WINDOW

        /// <summary>
        /// Font Tool Editor Window
        /// </summary>
        private static FtWindow _window;

        /// <summary>
        /// Static Method Called from Menu
        /// </summary>
        [MenuItem("Window/Font Tool")]
        public static void CreateWindow()
        {
            _window = GetWindow<FtWindow>("Font Tool", true);
            _window.Show();
        }

        #endregion


        #region VARIABLES

        /// <summary>
        /// To Enable Scroll View
        /// </summary>
        private Vector2 _scroll;

        /// <summary>
        /// Show the First and Last ASCII Fields
        /// </summary>
        private bool _useAscending;

        /// <summary>
        /// The Name of the Font
        /// </summary>
        private string _fontName;

        /// <summary>
        /// First ASCII Value
        /// </summary>
        private int _firstAscii;

        /// <summary>
        /// Last ASCII Value
        /// </summary>
        private int _lastAscii;

        /// <summary>
        /// Space between Each Character A.K.A Advanced
        /// </summary>
        private int _advanced;

        /// <summary>
        /// Horizontal Padding between Each Character
        /// </summary>
        private int _horizontalPadding;

        /// <summary>
        /// Vertical Padding Betweem Each Character
        /// </summary>
        private int _verticalPadding;

        /// <summary>
        /// PNG Font Texture Required to make the Custom Font
        /// </summary>
        private Texture _fontTexture;

        /// <summary>
        /// Number of Columns - Number of Characters Next to each other Horizontally
        /// </summary>
        private int _columns;

        /// <summary>
        /// Number of Rows - Number of Characters Below each other Vertically.
        /// </summary>
        private int _rows;

        /// <summary>
        /// Material Required to make the Custom Font
        /// </summary>
        private Material _fontMaterial;

        /// <summary>
        /// Custom or Premade Shader Required by the Material
        /// </summary>
        private Shader _shader;

        /// <summary>
        /// Custom Label
        /// </summary>
        private GUIContent _label = new GUIContent("", "");

        #endregion


        #region PROPERTIES

        /// <summary>
        /// Width of the Font Texture
        /// </summary>
        private int TextureWidth => _fontTexture ? _fontTexture.width : 1;

        /// <summary>
        /// Height of the Font Texture
        /// </summary>
        private int TextureHeight => _fontTexture ? _fontTexture.height : 1;

        /// <summary>
        /// Width of Each Character 
        /// </summary>
        private int GlymphWidth => TextureWidth / _columns;

        /// <summary>
        /// Height of Each Character
        /// </summary>
        private int GlymphHeight => -TextureHeight / _rows;

        /// <summary>
        /// Total Number of Characters
        /// </summary>
        private int Size => _lastAscii - _firstAscii + 1;

        /// <summary>
        /// UV Width
        /// </summary>
        private float UvW => 1f / _columns;

        /// <summary>
        /// UV Height
        /// </summary>
        private float UvH => 1f / _rows;

        #endregion


        #region EDITOR WINDOW METHODS

        private void OnGUI()
        {
            GUI.backgroundColor = new Color(0f, 0.5803921568627451f, 1f);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = new Color(1f, 1f, 1f, 1f);
            _scroll             = EditorGUILayout.BeginScrollView(_scroll);
            GUILayout.FlexibleSpace();

            DataFields();

            PreviewFontSettings();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Display the Font Texture, Shader are components that make up the Material required to make the Custom Font.
        /// </summary>
        private void DataFields()
        {
            GUI.backgroundColor = new Color(0f, 0.5803921568627451f, 1f);

            SetLabel("Required Objects",
                "The Object Fields Below are Required to be non-null Values to a make the Custom Font");
            EditorGUILayout.LabelField(_label, EditorStyles.boldLabel);


            // Check if the Font Texture is Null
            EditorGUI.BeginChangeCheck();
            FontTextureField();

            if (EditorGUI.EndChangeCheck())
            {
                _fontMaterial = null;
            }


            // Shader Field
            ShaderField();


            // Check if the Material is Null
            EditorGUI.BeginChangeCheck();
            FontMaterialField();

            if (EditorGUI.EndChangeCheck())
            {
                _fontTexture = _fontMaterial ? _fontMaterial.mainTexture : _fontTexture;
                _shader      = _fontMaterial ? _fontMaterial.shader : Shader.Find("Standard");
                _fontName    = _fontMaterial ? _fontMaterial.name : _fontName;
            }

            // Create Material From Shader and Font Texture
            CreateMaterial();
        }


        /// <summary>
        /// Display the Font Texture and the Custom Font Settings
        /// </summary>
        private void PreviewFontSettings()
        {
            GUI.backgroundColor = new Color(0f, 0.5803921568627451f, 1f);

            // Some Space
            GUILayout.Space(25f);
            SetLabel("Font Character Settings", "Font Character Settings that will Define Each Individual Character.");
            EditorGUILayout.LabelField(_label, EditorStyles.boldLabel);


            FontNameField();
            AscendingAsciiValueFields();
            DimensionFields();
            AdvancedField();
            PaddingFields();


            // Some Space
            GUILayout.Space(25f);
            SetLabel("Character Info Settings", "The Following Settings are Automatically Completed.");
            EditorGUILayout.LabelField(_label, EditorStyles.boldLabel);


            SizeField();
            UvFields();
            GlymphFields();
            CreateFont();
        }

        #endregion


        #region DATA METHODS

        /// <summary>
        /// Font Texture Field - Texture of Characters
        /// </summary>
        private void FontTextureField()
        {
            // Font Texture Field
            SetLabel("Font Texture", "A Texture consisting of Characters");
            _fontTexture = (Texture) EditorGUILayout.ObjectField(_label, _fontTexture, typeof(Texture), false);
        }


        /// <summary>
        /// Font Material Field - Material Field used by the Custom Font
        /// </summary>
        private void FontMaterialField()
        {
            // Material Field
            SetLabel("Material", "The Material to be used to make the Font");
            _fontMaterial = (Material) EditorGUILayout.ObjectField(_label, _fontMaterial, typeof(Material), false);
        }

        /// <summary>
        /// Shader used by the Material
        /// </summary>
        private void ShaderField()
        {
            // Shader Field
            SetLabel("Shader", "The Shader to be used by the Material");
            _shader = (Shader) EditorGUILayout.ObjectField(_label, _shader ? _shader : Shader.Find("Standard"),
                typeof(Shader), false);
        }


        /// <summary>
        /// Button that Creates a Material from the Texture and Shader
        /// </summary>
        private void CreateMaterial()
        {
            GUI.backgroundColor =
                _fontTexture && !_fontMaterial
                    ? new Color(0, 1, 0.5647058823529412f, 1)
                    : new Color(1, 0.22745098039215686f, 0.22745098039215686f, 1);

            SetLabel("Create Material",
                _fontTexture && !_fontMaterial
                    ? "Create a Material using the Font Texture and Shader Presets."
                    : "Cannot Create a Material");

            if (GUILayout.Button(_label))
            {
                if (_fontTexture && !_fontMaterial)
                {
                    _fontMaterial = CreateMaterialUtility.CreateMaterial(_fontTexture, new Color(1, 1, 1, 0), _shader);
                    _fontName     = _fontMaterial.name;
                }
            }
        }

        #endregion


        #region PREVIEW AND SETTING METHODS

        /// <summary>
        /// The Name of the Custom Font
        /// </summary>
        private void FontNameField()
        {
            // Some Space
            GUILayout.Space(15f);
            SetLabel("Font Name", "The Name of the Custom Font.");
            _fontName = EditorGUILayout.TextField(_label,
                _fontName == "" && _fontMaterial ? _fontMaterial.name : _fontName);
        }

        /// <summary>
        /// The First and Last Characters ASCII Values
        /// </summary>
        private void AscendingAsciiValueFields()
        {
            // Some Space
            GUILayout.Space(15f);

            SetLabel("Decimal ASCII",
                "Font Characters are in Ascending ASCII Order.");
            _useAscending = EditorGUILayout.Foldout(_useAscending, _label);

            if (_useAscending)
            {
                GUI.backgroundColor = new Color(1f, 0.788235294117647f, 0f, 1f);

                GUILayout.Space(10f);
                EditorGUILayout.HelpBox(
                    "The Characters of the Font Texture must follow the Ascending Order of Decimal Values of the ASCII Table.\n" +
                    "Example: 'Space' : ASCII = 32; ! : ASCII = 33; \" : ASCII = 34...",
                    MessageType.Info, true);
                GUILayout.Space(10f);

                GUI.backgroundColor = new Color(0f, 0.5803921568627451f, 1f);

                SetLabel("First ASCII Decimal",
                    "The first Characters ASCII Decimal Value (The First Character of the Font Image).");
                _firstAscii = EditorGUILayout.IntField(_label, _firstAscii);

                SetLabel("Last ASCII Decimal",
                    "The last Characters ASCII Decimal Value (The Last Character of the Font Image.");
                _lastAscii = EditorGUILayout.IntField(_label, _lastAscii);
            }
        }


        /// <summary>
        /// The Number of Columns and Rows.
        /// </summary>
        private void DimensionFields()
        {
            // Some Space
            GUILayout.Space(15f);

            SetLabel("Columns", "The Total # of Characters Horizontally.");
            _columns = EditorGUILayout.IntField(_label, _columns);

            SetLabel("Rows", "The Total # of Characters Vertically.");
            _rows = EditorGUILayout.IntField(_label, _rows);
        }


        /// <summary>
        /// The Space between the Characters.
        /// </summary>
        private void AdvancedField()
        {
            // Some Space
            GUILayout.Space(15f);
            SetLabel("Advanced",
                "The desired horizontal distance from the origin of this character to the origin of the next character in pixels");
            _advanced = EditorGUILayout.IntField(_label, _advanced);
        }


        /// <summary>
        /// Horizontal and Vertical Padding.
        /// </summary>
        private void PaddingFields()
        {
            // Some Space
            GUILayout.Space(15f);

            SetLabel("Horizontal Padding");
            _horizontalPadding = EditorGUILayout.IntField(_label, _horizontalPadding);

            SetLabel("Vertical Padding");
            _verticalPadding = EditorGUILayout.IntField(_label, _verticalPadding);
        }


        /// <summary>
        /// Number of Unique Characters that will be Created
        /// </summary>
        private void SizeField()
        {
            // Some Space
            EditorGUILayout.Space();
            SetLabel("# Unique Characters", "Size of the Character Info Array.");
            EditorGUILayout.IntField(_label, Size);
        }


        /// <summary>
        /// Can't really explain this one.
        /// </summary>
        private void UvFields()
        {
            // Some Space
            GUILayout.Space(15f);

            SetLabel("UV - Width", $"UV - Width is Calculated using : 1/{TextureWidth}.");
            EditorGUILayout.FloatField(_label, UvW);

            SetLabel("UV - Height", $"UV - Height is Calculated using : 1/{TextureHeight}.");
            EditorGUILayout.FloatField(_label, UvH);
        }


        /// <summary>
        /// Width and Height of the Characters.
        /// </summary>
        private void GlymphFields()
        {
            // Some Space
            GUILayout.Space(15f);

            SetLabel("Character Width", "The Pixel Width of an Indivual Character.");
            EditorGUILayout.IntField(_label, _columns > 0 ? GlymphWidth : 0);

            SetLabel("Character Height", "The Pixel Height of an Individual Character.");
            EditorGUILayout.IntField(_label, _rows > 0 ? GlymphHeight : 0);
        }


        /// <summary>
        /// Button that will use the Font Setting Data to make the Custom Font
        /// </summary>
        private void CreateFont()
        {
            // Some Space
            GUILayout.Space(35f);

            GUI.backgroundColor = Size > 0
                ? new Color(0, 1, 0.5647058823529412f, 1)
                : new Color(1, 0.22745098039215686f, 0.22745098039215686f, 1);

            SetLabel("Create Font", "Create The Custom Font.");
            if (GUILayout.Button(_label))
            {
                if (Size > 0)
                {
                    CreateFontUtility.CreateFont(_fontName, _fontMaterial,
                        GenerateCharacterInfo.CharacterInfos(Size, _advanced, _columns, _rows,
                            new Rect(0, 0, UvW, UvH),
                            new Rect(_horizontalPadding, _verticalPadding, GlymphWidth, GlymphHeight),
                            _firstAscii));
                }
            }
        }

        #endregion


        #region HELPER METHODS

        /// <summary>
        /// Modify the Label Text and Tooltip
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tip"></param>
        private void SetLabel(string text, string tip = "")
        {
            _label.text    = text;
            _label.tooltip = tip;
        }

        #endregion
    }
}