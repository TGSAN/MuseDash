using UnityEngine;

namespace EasyEditor
{
    /// <summary>
    /// Enum flag attribute. Render an enum as a mask. Your enum needs to be
    /// declared as follow :
    /// 
    /// public enum Compatibility
    ///{
    ///    iOS             = 0x001,
    ///    Android         = 0x002,
    ///    WindowsPhone    = 0x004,
    ///    PS4             = 0x008,
    ///    Wii             = 0x010,
    ///    XBoxOne         = 0x020,
    ///    WindowsDesktop  = 0x040,
    ///    Linux           = 0x080,
    ///    MacOS           = 0x100
    ///}
    /// 
    /// </summary>
    public class EnumFlagAttribute : PropertyAttribute
    {
        /// <summary>
        /// The label in the inspector. If empty, the nicified name of the property is used.
        /// </summary>
    	public string enumName = "";
     
    	public EnumFlagAttribute() {}
     
        /// <summary>
        /// Initializes a new instance of the <see cref="EasyEditor.EnumFlagAttribute"/> class.
        /// </summary>
        /// <param name="name">The label in the inspector. If empty, the nicified name of the property is used.</param>
    	public EnumFlagAttribute(string name)
    	{
    		enumName = name;
    	} 
    }
}