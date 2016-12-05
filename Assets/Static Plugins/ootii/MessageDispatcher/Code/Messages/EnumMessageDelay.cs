using System;

namespace com.ootii.Messages
{
    /// <summary>
    /// Basic enumeration for specifiying when a message is sent
    /// </summary>
	public class EnumMessageDelay
	{
        // Immediately send the message through
        public static float IMMEDIATE = 0;
        
        // Send the message through the next frame
        public static float NEXT_UPDATE = -1;
        
        // Send the message through in one second
        public static float ONE_SECOND = 1;
	}
}
