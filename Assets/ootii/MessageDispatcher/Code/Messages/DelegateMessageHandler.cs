using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ootii.Messages
{
    /// <summary>
    /// Delegate for message callbacks and handlers
    /// </summary>
    /// <param name="rMessage">Message that is to be handled</param>
    public delegate void MessageHandler(IMessage rMessage);
}
