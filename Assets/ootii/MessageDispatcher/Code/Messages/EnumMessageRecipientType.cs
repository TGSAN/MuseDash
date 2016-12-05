using System;

namespace com.ootii.Messages
{
    /// <summary>
    /// Basic enumeration for specifiying how the dispatcher
    /// filters messages to recipients
    /// </summary>
	public class EnumMessageRecipientType
	{
        // Send to recipient based on name
        public static int NAME = 0;
        
        // Send to recipient based on tag
        public static int TAG = 1;
	}
}
