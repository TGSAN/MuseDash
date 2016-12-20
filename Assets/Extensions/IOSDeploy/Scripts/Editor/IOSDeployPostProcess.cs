using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class IOSDeployPostProcess  {


	[PostProcessBuild(100)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
		#if UNITY_IPHONE &&  UNITY_EDITOR_WIN
			UnityEngine.Debug.LogWarning("ISD Postprocess is not avaliable for Win");
		#endif


		#if UNITY_IPHONE && UNITY_EDITOR_OSX

		Process myCustomProcess = new Process();		
		myCustomProcess.StartInfo.FileName = "python";

		List<string> frmwrkWithOpt = new List<string>();

		foreach(ISD_Framework framework in ISDSettings.Instance.Frameworks) {
			string optional = "|0";
			if(framework.IsOptional) {
				optional = "|1";
			}

			frmwrkWithOpt.Add (framework.Name + optional);
		}



		List<string> libWithOpt = new List<string>();

		foreach(ISD_Lib lib in ISDSettings.Instance.Libraries) { 
			string optional = "|0";
			if(lib.IsOptional) {
				optional = "|1";
			}

			libWithOpt.Add (lib.Name + optional);
		}

		foreach(string fileName in ISDSettings.Instance.langFolders)
		{
			string tempPath = Path.Combine (pathToBuiltProject, fileName + ".lproj");
			if(!Directory.Exists (tempPath))
			{
				Directory.CreateDirectory (tempPath);
			}
		}

		string frameworks 		= string.Join(" ", frmwrkWithOpt.ToArray());
		string libraries 		= string.Join(" ", libWithOpt.ToArray());
		string linkFlags 		= string.Join(" ", ISDSettings.Instance.linkFlags.ToArray());
		string compileFlags 	= string.Join(" ", ISDSettings.Instance.compileFlags.ToArray());
		string languageFolders  = string.Join (" ", ISDSettings.Instance.langFolders.ToArray ());


		myCustomProcess.StartInfo.Arguments = string.Format("Assets/Extensions/IOSDeploy/Scripts/Editor/post_process.py \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\"", new object[] { pathToBuiltProject, frameworks, libraries, compileFlags, linkFlags, languageFolders });
		myCustomProcess.StartInfo.UseShellExecute = false;
		myCustomProcess.StartInfo.RedirectStandardOutput = true;
		myCustomProcess.Start(); 
		myCustomProcess.WaitForExit();

		XmlDocument document = new XmlDocument();
		string filePath = Path.Combine (pathToBuiltProject, "Info.plist");
		document.Load (filePath);
		document.PreserveWhitespace = true;

		foreach(ISD_Variable var in ISDSettings.Instance.PlistVariables)
		{
			XmlNode temp = document.SelectSingleNode( "/plist/dict/key[text() = '" + var.Name + "']" );
			if(temp == null)
			{
				XmlNode valNode = null;
				switch(var.Type)
				{
				case ISD_PlistValueTypes.Array:
					valNode = document.CreateElement("array");
					AddArrayToPlist(var, valNode, document);
					break;
					
				case ISD_PlistValueTypes.Boolean:
					valNode = document.CreateElement(var.BooleanValue.ToString ().ToLower ());
					break;
					
				case ISD_PlistValueTypes.Dictionary:
					valNode = document.CreateElement("dict");
					AddDictionaryToPlist(var, valNode, document);
					break;
					
				case ISD_PlistValueTypes.Float:
					valNode = document.CreateElement("real");
					valNode.InnerText = var.FloatValue.ToString ();
					break;
					
				case ISD_PlistValueTypes.Integer:
					valNode = document.CreateElement("integer");
					valNode.InnerText = var.IntegerValue.ToString ();
					break;
					
				case ISD_PlistValueTypes.String:
					valNode = document.CreateElement("string");
					valNode.InnerText = var.StringValue;
					break;
				}
				XmlNode keyNode = document.CreateElement ("key");
				keyNode.InnerText = var.Name;
				document.DocumentElement.FirstChild.AppendChild (keyNode);
				document.DocumentElement.FirstChild.AppendChild (valNode);
			}
		}
		
		XmlWriterSettings settings  = new XmlWriterSettings {
			Indent = true,
			IndentChars = "\t",
			NewLineHandling = NewLineHandling.None
		};
		XmlWriter xmlwriter = XmlWriter.Create (filePath, settings );
		document.Save (xmlwriter);
		xmlwriter.Close ();
		
		System.IO.StreamReader reader = new System.IO.StreamReader(filePath);
		string textPlist = reader.ReadToEnd();
		reader.Close ();
		
		//strip extra indentation (not really necessary)
		textPlist = (new Regex("^\\t",RegexOptions.Multiline)).Replace(textPlist,"");
		
		//strip whitespace from booleans (not really necessary)
		textPlist = (new Regex("<(true|false) />",RegexOptions.IgnoreCase)).Replace(textPlist,"<$1/>");
		
		int fixupStart = textPlist.IndexOf("<!DOCTYPE plist PUBLIC");
		if(fixupStart <= 0)
			return;
		int fixupEnd = textPlist.IndexOf('>', fixupStart);
		if(fixupEnd <= 0)
			return;
		
		string fixedPlist = textPlist.Substring(0, fixupStart);
		fixedPlist += "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">";
		fixedPlist += textPlist.Substring(fixupEnd+1);
		
		System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false);
		writer.Write(fixedPlist);
		writer.Close ();

		UnityEngine.Debug.Log("ISD Executing post process done.");

		#endif
	}
	
	static void AddArrayToPlist (ISD_Variable var, XmlNode node, XmlDocument doc)
	{
		if(var.ArrayValue.Count == 0) return;

		foreach(ISD_VariableListed varArray in var.ArrayValue)
		{
			switch(var.ArrayType)
			{
			case ISD_PlistValueTypes.Boolean:
				XmlNode tempBoolNode = doc.CreateElement (varArray.BooleanValue.ToString ().ToLower ());
				node.AppendChild (tempBoolNode);
				break;
				
			case ISD_PlistValueTypes.Float:
				XmlNode tempFloatNode = doc.CreateElement ("real");
				tempFloatNode.InnerText = varArray.FloatValue.ToString ();
				node.AppendChild (tempFloatNode);
				break;
				
			case ISD_PlistValueTypes.Integer:
				XmlNode tempIntegerNode = doc.CreateElement ("integer");
				tempIntegerNode.InnerText = varArray.IntegerValue.ToString ();
				node.AppendChild (tempIntegerNode);
				break;
				
			case ISD_PlistValueTypes.String:
				XmlNode tempStringNode = doc.CreateElement ("string");
				tempStringNode.InnerText = varArray.StringValue;
				node.AppendChild (tempStringNode);
				break;
			}
		}
	}

	static void AddDictionaryToPlist (ISD_Variable var, XmlNode node, XmlDocument doc)
	{
		if(var.DictValues.Count == 0) return;
		
		foreach(ISD_VariableListed varDict in var.DictValues)
		{
			XmlNode tempNode = null;
			switch(varDict.Type)
			{
			case ISD_PlistValueTypes.Boolean:
				tempNode = doc.CreateElement (varDict.BooleanValue.ToString ().ToLower ());
				break;
				
			case ISD_PlistValueTypes.Float:
				tempNode = doc.CreateElement ("real");
				tempNode.InnerText = varDict.FloatValue.ToString ();
				break;
				
			case ISD_PlistValueTypes.Integer:
				tempNode = doc.CreateElement ("integer");
				tempNode.InnerText = varDict.IntegerValue.ToString ();
				break;
				
			case ISD_PlistValueTypes.String:
				tempNode = doc.CreateElement ("string");
				tempNode.InnerText = varDict.StringValue;
				break;
			}
			XmlNode tempKeyNode = doc.CreateElement ("key");
			tempKeyNode.InnerText = varDict.DictKey;
			node.AppendChild (tempKeyNode);
			node.AppendChild (tempNode);
		}
	}
}
