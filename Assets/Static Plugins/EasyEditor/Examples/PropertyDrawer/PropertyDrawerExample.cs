using System.Collections;
using UnityEngine;

namespace EasyEditor
{
    public enum PlatformCompatibility
    {
        iOS = 0x001,
        Android = 0x002,
        WindowsPhone = 0x004,
        PS4 = 0x008,
        Wii = 0x010,
        XBoxOne = 0x020,
        WindowsDesktop = 0x040,
        Linux = 0x080,
        MacOS = 0x100
    }

    public enum IngredientUnit { Spoon, Cup, Bowl, Piece }

    // Custom serializable class rendered with the PropertyDrawer implemented in Drawer.cs
    [System.Serializable]
    public class Ingredient
    {
        public string name;
        public int amount = 1;
        public IngredientUnit unit;
    }

    /// <summary>
    /// Class demonstrating how EE automatically handle property drawers.
    /// </summary>
    public class PropertyDrawerExample : MonoBehaviour
    {
        [Image]
        public string easyEditorImage = "Assets/EasyEditor/Examples/icon.png";

        [Inspector(group = "Popup")]
        [Popup("Wizard Potion", "Warrior Potion", "Priest Potion")]
        public string potionType;

        [Visibility("potionType", "Wizard Potion")]
        [Popup(1, 2, 3)]
        public int wizardLevel;

        public Ingredient potionResult;
        public Ingredient[] potionIngredients;

        [Inspector(group = "Progress Bar")]
        [ProgressBarAttribute(10)]
        public float life = 6f;

        [Inspector(group = "Read Only")]
        [ReadOnly]
        public Vector3 velocity;

        [Inspector(group = "Enum Flag")]
        [EnumFlag]
        public PlatformCompatibility compatibleDevice;

        [Inspector(group = "Path")]
        [CommentAttribute("Drag an asset from the project panel to the string field.")]
        [Path]
        public string assetPath;

        [Inspector(group = "2D assets slot")]
        [Texture]
        public Texture texture;

        [Sprite]
        public Sprite sprite;
    }
}