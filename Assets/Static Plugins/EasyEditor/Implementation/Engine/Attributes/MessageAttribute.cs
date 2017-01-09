//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using System;

namespace EasyEditor
{
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }

	/// <summary>
	/// Message attribute allows to place a message under an <c>InspecterItemRenderer</c> in the inspector inside an info box.
    /// This message can be of type Info, Warning or Error. It can also be hidden under some conditions.
    /// The parameter 'method' allows to specify a method returning bool. If this method return true,
    /// the message is displayed. If it returns false, the message is hidden.
    /// The parameters 'id' and 'value' allow to specify another field and indicate which value the field
    /// needs to be set to in order to show the message.
	/// </summary>
	[Serializable]
	[AttributeUsageAttribute(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true)]
	public class MessageAttribute : Attribute {

		/// <summary>
		/// Text content of the boxed message label.
		/// </summary>
		public string text = "";
        /// <summary>
        /// The type of the message. Have impact only on the way it is rendered.
        /// </summary>
        public MessageType messageType = MessageType.Info;
        /// <summary>
        /// If a method name is specified, the message will be displayed only if this method returns true.
        /// The method has to return bool and has to be in the same object as the Message attribute is used.
        /// </summary>
        public string method = "";
        /// <summary>
        /// The id refers to another field. If this fields value is equal to the parameter value of MessageAttribute, then
        /// the message is rendered.
        /// </summary>
        public string id = "";
        /// <summary>
        /// The value id needs to equal for the message to be rendered.
        /// </summary>
        public object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyEditor.MessageAttribute"/> class.
        /// </summary>
        /// <param name="text">Text to display in the message. Message is rendered as a comment by default (MessageType = MessageType.Info).</param>
        public MessageAttribute(string text)
		{
            this.text = text;
		}

        public MessageAttribute()
        {
        }
	}
}