Message Dispatcher
by ootii


Complete documentation is found here:
http://www.ootii.com/UnityDispatcher.cshtml


For feedback or support, contact:
Tim Tryzbiak
support@ootii.com


While the online documentation more fully describes how to use the dispatcher, the following will help get you started:


Event and message dispatching is a critical part of any game. As games get more and more complex, so too are the interactions between objects. Message dispatching ensures that game objects are able to communicate in a consistant, reliable, and efficient way. 

Dispatcher does this and makes it easy too! Simply tell the Dispatcher what an object wants to listen for. When another object sends that message to the Dispatcher, the Dispatcher will ensure all the "listeners" are notified. 

With this dispatcher, you can create your own custom messages and take advantage of the message pool for super fast performance.

** Set Up **

The package you download from the Unity Asset Store contains all of the scripts you need to use the message dispatcher. Feel free to use the code as-is or modify it. 

To setup the Dispatcher, follow these simple steps: 

1. Create a new unity project or open an existing one. 
The scripts you've just downloaded will work with any Unity project. 

2. Copy these scripts into your project 
Copy the entire "ootii" folder from the "scripts" folder of this downloaded package to the "scripts" folder in your project. 

You can actually move the individual script files into other folders and it won't hurt anything, but this will help keep your project organized. 

3. Inside your GameObject scripts, call the DispatchManager to register and send messages. 
It really is that simple. 

The DispatchManager class is used to collect messages and then send them out to "listeners" that you've registered with it. See the "Usage" section below. 

** Usage **

Using Dispatcher requires four steps: 

1. Include these scripts in your scripts 
As with any classes you want to use, you have to tell your scripts they exist. To do this, you'll need to add the "using" directive to the top of your script files that call the DispatchManager. 

using com.ootii.Messages;

2. Create a function to respond to messages 
When the MessageDispatcher sends out messages, your classes will be notified through functions you create. 

public void OnLevelWon(IMessage rMessage) 
{ 
// Your code here; 
}

In the next step, you'll setup a "listener" that references the function you created in this step. 

3. Register the listeners 
From within your scripts that you want to use the DispatchManager, you simply give the dispatcher a string to identify the message you want to listen for and a function for it to call when a message comes in with that identifier. 

MessageDispatcher.AddListener("LEVEL_WON", OnLevelWon);

Note that adding and removing listeners is done at the end of the frame. So a listerner won't actually exist until the frame has been completed. To ignore the delay and add or remove the listerner immediately, add 'true' as the last argument.

4. Send the messages 
When the time comes, another object in the game simply sends a message with the appropriate identifier to the Dispatcher. At that point, anyone who has registered to listen for that message will be notified. In this example, the "OnLevelWon" function will be called. 

MessageDispatcher.SendMessage("LEVEL_WON");
