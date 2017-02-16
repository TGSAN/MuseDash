using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using qtools.qhierarchy.phierarchy;

namespace qtools.qhierarchy.pdata
{
	public enum QSetting
	{
		ShowTreeMapComponent = 0,
		ShowVisibilityComponent,
		ShowLockComponent,
		ShowGameObjectIconComponent,
		ShowMonoBehaviourIconComponent,
		ShowTagLayerComponent,
		ShowErrorComponent,
        ShowRendererComponent,
        ShowSeparatorComponent,
        ShowColorComponent,
        ShowComponentsComponent,
        ShowChildrenCountComponent,
        ShowStaticComponent,
        ShowPrefabComponent,
        ShowTagIconComponent,

        ShowVisibilityComponentDuringPlayMode,
        ShowLockComponentDuringPlayMode,
        ShowGameObjectIconComponentDuringPlayMode,
        ShowMonoBehaviourIconComponentDuringPlayMode,
        ShowTagLayerComponentDuringPlayMode,
        ShowErrorComponentDuringPlayMode,
        ShowRendererComponentDuringPlayMode,
        ShowColorComponentDuringPlayMode,
        ShowComponentsComponentDuringPlayMode,
        ShowChildrenCountComponentDuringPlayMode,
        ShowStaticComponentDuringPlayMode,
        ShowTagIconComponentDuringPlayMode,

		ShowErrorIconParent,
		ShowErrorIconScriptIsMissing,
		ShowErrorIconReferenceIsNull,
		ShowErrorIconStringIsEmpty,
        ShowErrorIconMissingEventMethod,
        ShowErrorIconWhenTagOrLayerIsUndefined,
        IgnoreErrorOfMonoBehaviours,

		TagAndLayerType,
		TagAndLayerSizeType,
		TagAndLayerSizeValuePixel,
        TagAndLayerAligment,
		ComponentOrder,
        Identation,
        CustomTagIcon,
        PreventSelectionOfLockedObjects,
        ShowHiddenQHierarchyObjectList,
        ShowModifierWarning,
        ShowErrorForDisabledComponents,
        IgnoreUnityMonobehaviour,
        TagAndLayerSizeValueType,
        TagAndLayerSizeValuePercent,
        TagAndLayerLabelSize,
        ShowObjectListContent,
        ShowRowShading,
        ShowBreakedPrefabsOnly,
        HideIconsIfNotFit
	}
	
	public enum QHierarchyTagAndLayerType
	{
		Always = 0,
		OnlyIfNotDefault = 1
	}

    public enum QHierarchyTagAndLayerAligment
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    public enum QHierarchyTagAndLayerSizeType
    {
        Pixel = 0,
        Percent = 1
    }

    public enum QHierarchyTagAndLayerLabelSize
    {
        Small = 0,
        Normal = 1,
        SmallIfNeeded
    }
	
	public enum QHierarchyComponentEnum
	{
        LockComponent               = 0,
        VisibilityComponent         = 1,
        StaticComponent             = 2,
        ErrorComponent              = 3,
        RendererComponent           = 4,
        PrefabComponent             = 5,
        TagLayerComponent           = 6,
        ColorComponent              = 7,
        GameObjectIconComponent     = 8,
        TagIconComponent            = 9,
        ChildrenCountComponent      = 10,
        SeparatorComponent          = 1000,
        TreeMapComponent            = 1001,
        MonoBehaviourIconComponent  = 1002,
        ComponentsComponent         = 1003
	}

    public class QTagTexture
    {
        public string tag;
        public Texture2D texture;
        
        public QTagTexture(string tag, Texture2D texture)
        {
            this.tag = tag;
            this.texture = texture;
        }
    }

    public delegate void QSettingChangedHandler();

	public class QSettings 
	{
        // CONST
		private const string PREFS_PREFIX = "QTools.QHierarchy_";
        public const string DEFAULT_ORDER = "0;1;2;3;4;5;6;7;8;9;10";
        public const int DEFAULT_ORDER_COUNT = 11;

        // PRIVATE
		private Dictionary<int, object> settings;

        // EVENTS
        private Dictionary<int, QSettingChangedHandler> settingChangedHandlerList;

        // SINGLETON
        private static QSettings instance;
        public static QSettings getInstance()
        {
            if (instance == null) instance = new QSettings();
            return instance;
        }

        // CONSTRUCTOR
		private QSettings()
		{
            settingChangedHandlerList = new Dictionary<int, QSettingChangedHandler>();
            settings = new Dictionary<int, object>();

            List<QTagTexture> tagTextureList = new List<QTagTexture>();
            string customTagIcon = (string)getEditorSetting(QSetting.CustomTagIcon, "");
			string[] customTagIconArray = customTagIcon.Split(new char[]{';'});
            List<string> tags = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);
            for (int i = 0; i < customTagIconArray.Length - 1; i+=2)
            {
                string tag = customTagIconArray[i];
                if (!tags.Contains(tag)) continue;
                string texturePath = customTagIconArray[i+1];

                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
                if (texture != null)  
                { 
                    QTagTexture tagTexture = new QTagTexture(tag, texture);
                    tagTextureList.Add(tagTexture);
                }  
            }

            initSetting(QSetting.ShowVisibilityComponent        , true);
            initSetting(QSetting.ShowLockComponent              , true);
            initSetting(QSetting.ShowGameObjectIconComponent    , false);
            initSetting(QSetting.ShowTreeMapComponent           , true);
            initSetting(QSetting.ShowMonoBehaviourIconComponent , true);
            initSetting(QSetting.ShowTagLayerComponent          , true);
            initSetting(QSetting.ShowErrorComponent             , true);
            initSetting(QSetting.ShowTagIconComponent           , false);
            initSetting(QSetting.ShowStaticComponent            , true); 
            initSetting(QSetting.ShowRendererComponent          , false);
            initSetting(QSetting.ShowSeparatorComponent         , true);
            initSetting(QSetting.ShowColorComponent             , true);
            initSetting(QSetting.ShowComponentsComponent        , true);
            initSetting(QSetting.ShowChildrenCountComponent     , false);            
            initSetting(QSetting.ShowPrefabComponent            , false);

            initSetting(QSetting.ShowVisibilityComponentDuringPlayMode       , true);
            initSetting(QSetting.ShowLockComponentDuringPlayMode             , false);
            initSetting(QSetting.ShowGameObjectIconComponentDuringPlayMode   , true);
            initSetting(QSetting.ShowMonoBehaviourIconComponentDuringPlayMode, true);
            initSetting(QSetting.ShowTagLayerComponentDuringPlayMode         , true);
            initSetting(QSetting.ShowErrorComponentDuringPlayMode            , false);
            initSetting(QSetting.ShowRendererComponentDuringPlayMode         , false);
            initSetting(QSetting.ShowColorComponentDuringPlayMode            , true);
            initSetting(QSetting.ShowComponentsComponentDuringPlayMode       , false);
            initSetting(QSetting.ShowChildrenCountComponentDuringPlayMode    , true);
            initSetting(QSetting.ShowStaticComponentDuringPlayMode           , false);
            initSetting(QSetting.ShowTagIconComponentDuringPlayMode          , true);

            initSetting(QSetting.ShowErrorIconParent            , true);
            initSetting(QSetting.ShowErrorIconScriptIsMissing   , true);
            initSetting(QSetting.ShowErrorIconReferenceIsNull   , true);
            initSetting(QSetting.ShowErrorIconStringIsEmpty     , true);
            initSetting(QSetting.ShowErrorIconMissingEventMethod, true);
            initSetting(QSetting.ShowErrorIconWhenTagOrLayerIsUndefined, true);
            initSetting(QSetting.IgnoreErrorOfMonoBehaviours    , "");

            initSetting(QSetting.TagAndLayerType                , (int)QHierarchyTagAndLayerType.OnlyIfNotDefault);
            initSetting(QSetting.TagAndLayerAligment            , (int)QHierarchyTagAndLayerAligment.Left);
            initSetting(QSetting.TagAndLayerSizeValueType       , (int)QHierarchyTagAndLayerSizeType.Pixel);
            initSetting(QSetting.TagAndLayerSizeValuePercent    , 0.25f);
            initSetting(QSetting.TagAndLayerSizeValuePixel      , 75);
            initSetting(QSetting.TagAndLayerLabelSize           , (int)QHierarchyTagAndLayerLabelSize.Small);
            initSetting(QSetting.ComponentOrder                 , DEFAULT_ORDER);
            initSetting(QSetting.Identation                     , 0);
            initSetting(QSetting.CustomTagIcon                  , tagTextureList);
            initSetting(QSetting.PreventSelectionOfLockedObjects, false);
            initSetting(QSetting.ShowHiddenQHierarchyObjectList , true);
            initSetting(QSetting.ShowModifierWarning            , true);
            initSetting(QSetting.ShowErrorForDisabledComponents , true);
            initSetting(QSetting.IgnoreUnityMonobehaviour       , true);
            initSetting(QSetting.ShowObjectListContent          , false);
            initSetting(QSetting.ShowRowShading                 , true);
            initSetting(QSetting.ShowBreakedPrefabsOnly         , false);
            initSetting(QSetting.HideIconsIfNotFit              , false);
		} 

        // DESTRUCTOR
        public void OnDestroy()
        {
            settings = null;           
            settingChangedHandlerList = null;
            instance = null;
        }

        // PUBLIC
        public T get<T>(QSetting setting)
        {
            return (T)settings[(int)setting];
        }

        public void set<T>(QSetting setting, T value)
        {
            int settingId = (int)setting;
            settings[settingId] = value;
            setEditorSetting(setting, value);           
            
            if (settingChangedHandlerList.ContainsKey(settingId) && settingChangedHandlerList[settingId] != null)            
                settingChangedHandlerList[settingId].Invoke();

            EditorApplication.RepaintHierarchyWindow();
        }

        public void set(QSetting setting, List<QTagTexture> tagTextureList)
        { 
            string result = "";
            for (int i = 0; i < tagTextureList.Count; i++)            
                result += tagTextureList[i].tag + ";" + AssetDatabase.GetAssetPath(tagTextureList[i].texture.GetInstanceID()) + ";";
            
            setEditorSetting(setting, result);
            settings[(int)setting] =  tagTextureList;
        }

        public void addEventListener(QSetting setting, QSettingChangedHandler handler)
        {
            int settingId = (int)setting;
            
            if (!settingChangedHandlerList.ContainsKey(settingId))          
                settingChangedHandlerList.Add(settingId, null);
            
            if (settingChangedHandlerList[settingId] == null)           
                settingChangedHandlerList[settingId] = handler;
            else            
                settingChangedHandlerList[settingId] += handler;
        }
        
        public void removeEventListener(QSetting setting, QSettingChangedHandler handler)
        {
            int settingId = (int)setting;

            if (settingChangedHandlerList.ContainsKey(settingId) && settingChangedHandlerList[settingId] != null)       
                settingChangedHandlerList[settingId] -= handler;
        }
               
        // PRIVATE
        private void initSetting(QSetting setting, object defaultValue)
        {
            object value = getEditorSetting(setting, defaultValue);
            if (value != defaultValue && value.GetType() == defaultValue.GetType())
            {
                settings[(int)setting] = value; 
            }
            else
            {
                set(setting, defaultValue);
            }            
        }

        private object getEditorSetting(QSetting setting, object defaultValue)
        {
            if (defaultValue is bool)
            {
                return EditorPrefs.GetBool(PREFS_PREFIX + setting.ToString("G"), (bool)defaultValue);
            }
            else if (defaultValue is int)
            {
                return EditorPrefs.GetInt(PREFS_PREFIX + setting.ToString("G"), (int)defaultValue);
            }
            else if (defaultValue is float)
            {
                return EditorPrefs.GetFloat(PREFS_PREFIX + setting.ToString("G"), (float)defaultValue);
            }
            else if (defaultValue is string)
            {
                return EditorPrefs.GetString(PREFS_PREFIX + setting.ToString("G"), (string)defaultValue);
            }
            else
            {
                return defaultValue;
            }
        } 
        
        private void setEditorSetting(QSetting setting, object value)
        {
            if (value is bool)
            {
                EditorPrefs.SetBool(PREFS_PREFIX + setting.ToString("G"), (bool)value);
            }
            else if (value is int)
            {
                EditorPrefs.SetInt(PREFS_PREFIX + setting.ToString("G"), (int)value);
            }
            else if (value is float)
            {
                EditorPrefs.SetFloat(PREFS_PREFIX + setting.ToString("G"), (float)value);
            }
            else if (value is string)
            {
                EditorPrefs.SetString(PREFS_PREFIX + setting.ToString("G"), (string)value);
            }
        }
	}
}