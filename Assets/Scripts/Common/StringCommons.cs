namespace Assets.Scripts.Common
{
    public class StringCommons
    {
        public const string PRPRToolHead = "PRPR Studio Tools/";
        public const string ResourcesPathInAssets = "Assets/Resources/";
        public const string ConfigPoolMenuItem = PRPRToolHead + "Packager/Package Json Configs";
        public const string ConfigPoolPathInResources = "Configs/Json Configs";
        public const string ConfigPoolPathInAssets = ResourcesPathInAssets + ConfigPoolPathInResources + ".asset";
        public const string PRNodePathInAssets = "/Scripts/Tools/PRHelper/Properties";
        public const string PRNodeDrawerPathInAssets = PRNodePathInAssets + "/Editor";

        public const string PRFormulaTreeEditorSkinPathInAssets =
            "Assets/Scripts/Tools/FormulaTreeManager/Editor/Skins/FormulaTreeSkin.guiskin";

        public const string PRHelperSkinInAssets = "Assets/Scripts/Tools/PRHelper/Editor/Skins/PRPRHelperSkin.guiskin";

        public const string FormulaTreeMenuItem = PRPRToolHead + "FormulaTree Editor";

        public static string FormulaTreePathInAssets = ResourcesPathInAssets + FormulaTreePathInResources + ".asset";
        public static string FormulaTreePathInResources = "Formulas/Formula Data";
    }
}