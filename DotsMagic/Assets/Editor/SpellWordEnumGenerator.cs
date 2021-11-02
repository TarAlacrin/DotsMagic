using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;

//[InitializeOnLoad]
public static class SpellWordsEnumGenerator
{
    // an array that hold all tags
    private static MethodInfo[] _methodInfos;
    // a flag if the dataset has changed
    private static bool _hasChanged = false;
    // time when we start to count
    private static double _startTime = 0.0;
    // the time that should elapse between the change of tags and the File write
    // this is importend because changed are triggered as soon as you start typing and this can cause lag
    private static double _timeToWait = 1.0;




    static SpellWordsEnumGenerator()
    {
        
        //subscripe to event
        //EditorApplication.update += Update;
        // get tags
        //_methodInfos = GetAllSpellWords();
        // write file
        //WriteCodeFile();
    }

    //[MenuItem("Build/Generate Spell Word Code")]// TODO: RENABLE
    static void GenerateSpellWords()
	{
        _methodInfos = GetAllSpellWords();
        //WriteCodeFile();//TODO: RENABLE
        Debug.Log("Spell word code generated and saved. Total Methods Processed = " + _methodInfos.Count());
    }

    //[MenuItem("Build/CLEAR Current Spell Word Code")]// TODO: RENABLE
    static void ClearGeneratedCode()
	{
        _methodInfos = GetAllSpellWords();
        //WriteCodeFile(false );// TODO: RENABLE
        Debug.Log("Cleared the generated spell word code. Total Methods Processed = " + _methodInfos.Count());
    }


    private static MethodInfo[] GetAllSpellWords()
	{
        IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t => t.GetTypes())
            .Where(t => t.IsClass && t.Namespace == "SpellWordNamespace");


        List<MethodInfo> toreturn = new List<MethodInfo>();

        foreach (Type type in types)
        {
            var methods = type.GetMethods();

            foreach (var method in methods)
            {
                SpellWordAttribute spellWordAttribute = method.GetCustomAttribute<SpellWordAttribute>();
                if (spellWordAttribute == null)
                    continue;

                toreturn.Add(method);
            }
        }

        return toreturn.ToArray();
    }


    private static bool CheckHasChanged(ref MethodInfo[] newMethodInfos)
	{
        bool result = false;
        if (newMethodInfos.Length != _methodInfos.Length)
        {
            result =  true;
        }
        else
        {
            // loop thru all new tags and compare them to the old ones
            for (int i = 0; i < newMethodInfos.Length; i++)
            {
                if (newMethodInfos[i] != _methodInfos[i])
                {
                    result = true;
                    break;
                }
            }
        }

        return result;
    }

    private static void Update()
    {
        // returns if we are in play mode
        /*if (Application.isPlaying == true)
            return;

        WaitThenExecuteWhenChanged();

        MethodInfo[] newMethodInfos = GetAllSpellWords();

		if(CheckHasChanged(ref newMethodInfos))
		{
            _methodInfos = newMethodInfos;
            _hasChanged = true;
            _startTime = EditorApplication.timeSinceStartup;
        }*/
    }

    private static void WaitThenExecuteWhenChanged()
    {
        // if nothing has changed return
        if (_hasChanged == false)
            return;

        // if the time delta between now and the last change, is greater than the time we schould wait Than write the file
        if (EditorApplication.timeSinceStartup - _startTime > _timeToWait)
        {
           // WriteCodeFile();
            _hasChanged = false;
        }
    }


    private static string StringFromParamInfoArray(in ParameterInfo[] paramInfos)
	{
        string stringToFormat = "SplWrdDel";

        foreach(ParameterInfo parinfo in paramInfos)
		{
            stringToFormat += "_";

            if (parinfo.IsIn)
                stringToFormat += ("IN");
            if (parinfo.IsOut)
                stringToFormat += ("OUT");
            if (parinfo.IsOptional)
                stringToFormat += ("OPTION");
            if (parinfo.IsRetval)
			{
                stringToFormat += ("RETURN");

                Debug.LogError("Attempting to generate code for spell word delegates and encountered one with a return type. These should all be functions with NO return type");
            }

            stringToFormat += parinfo.ParameterType.Name;
        }

        stringToFormat= stringToFormat.Replace("&", "");
        stringToFormat= stringToFormat.Replace("*", "PTR");

        return stringToFormat;
    }

    private static string BuildIndividualParameterDeclaration(ParameterInfo para)
    {
        string toreturn = para.ParameterType.FullName + " " + para.Name;

        bool isRef = toreturn.Contains('&');

        toreturn = toreturn.Replace("&", "");
        toreturn = toreturn.Replace('+', '.');

        if (para.IsIn)
        {
            if (para.IsOut)
                toreturn = "ref " + toreturn;
            else
                toreturn = "in " + toreturn;
        }
        else if(para.IsOut)
            toreturn = "out " + toreturn;
        else if(isRef)
            toreturn = "ref " + toreturn;

        return toreturn;
    }


    private static string BuildDelegateParameterString(ParameterInfo[] paramInfos)
	{
        string stringToFormat = "";

        for(int i=0; i < paramInfos.Length; i++)
        {
            if (i != 0)
                stringToFormat += ", ";
            stringToFormat += BuildIndividualParameterDeclaration(paramInfos[i]);
        }

        return stringToFormat;
	}

    //searches through the existing declared delegates and, if this new method matches one of the existing ones, uses that, or declares a new delegate type and uses that
    private static void CompareAndAddDelegateIfNecessary(in MethodInfo methodInfo, ref StringBuilder builder, ref HashSet<string> existingDelegates, ref Dictionary<string, string> delegateNameForMethodInfoName)
	{
        ParameterInfo[] paramInfos = methodInfo.GetParameters();
        string getSpellWordDelegateName = StringFromParamInfoArray(paramInfos);
        
        if (!existingDelegates.Contains(getSpellWordDelegateName))
		{
            existingDelegates.Add(getSpellWordDelegateName);
            builder.AppendLine(string.Format("\t\tpublic delegate void {0}({1});", getSpellWordDelegateName, BuildDelegateParameterString(paramInfos)));
        }

        delegateNameForMethodInfoName.Add(methodInfo.Name, getSpellWordDelegateName);

    }

    // writes a file to the project folder
    private static void WriteCodeFile( bool writeDelegateDictionary = true)
    {
        // the path we want to write to
        string path = string.Concat(Application.dataPath, Path.DirectorySeparatorChar, "SpellWordsEnum.cs");
        try
        {
            //Whipe the old version to write the new version
            if (File.Exists(path) == true)
                File.Delete(path);
            // opens the file if it allready exists, creates it otherwise
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("// ----- AUTO GENERATED CODE ----- //");
                    builder.AppendLine("using System.Reflection;");

                    //namespace start
                    builder.AppendLine("namespace SpellWordNamespace");
                    builder.AppendLine("{");


                    //Writes all Method names with a SpellWordAttribute to an enum
                    builder.AppendLine("\tpublic enum SpellWordEnum");
                    builder.AppendLine("\t{");
                    foreach (MethodInfo methodInfo in _methodInfos)
                    {
                        builder.AppendLine(string.Format("\t\t{0},", methodInfo.Name));
                    }

                    builder.AppendLine("\t}");


                    //in this class I store the links between methodinfos and spellGroups
                    builder.AppendLine("\tpublic static unsafe class SpellWords");
                    builder.AppendLine("\t{");
                    
                    HashSet<string> delegateParameterTypes = new HashSet<string>();
                    Dictionary<string, string> delegateNamesForEachMethod = new Dictionary<string, string>();
                    //Builds and writes a declaration a delegate for each unique set of parameters
                    foreach (MethodInfo methodInfo in _methodInfos)
                    {
                        CompareAndAddDelegateIfNecessary(methodInfo, ref builder, ref delegateParameterTypes, ref delegateNamesForEachMethod);
                    }

                    builder.AppendLine("\t\tpublic static MethodInfo[] spellWordArray = {");

                    if(writeDelegateDictionary)
					{
                        foreach (MethodInfo methodInfo in _methodInfos)
                        {
                            builder.AppendLine(string.Format("\t\t\t(({0}){1}.{2}).GetMethodInfo(),", delegateNamesForEachMethod[methodInfo.Name], methodInfo.DeclaringType.FullName, methodInfo.Name));
                        }
                    }

                    builder.AppendLine("\t\t};");
                    builder.AppendLine("\t}");

                    builder.AppendLine("}");
                    //TODO: RENABLE : writer.Write(builder.ToString());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);

            // if we have an error, it is certainly that the file is screwed up. Delete to be save
            if (File.Exists(path) == true)
                File.Delete(path);
        }

        AssetDatabase.Refresh();
    }
}

