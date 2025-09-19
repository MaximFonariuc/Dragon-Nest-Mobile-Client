using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class InjectEditor
{
	[MenuItem("LuaTools/Injector/Inject")]
	private static void CodeInjectoring()
	{
		// Вручную запускать не нужно, оставлено для совместимости
	}

	[MenuItem("LuaTools/Injector/Clean")]
	private static void CleanInject()
	{
		string dllPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library", "ScriptAssemblies");
		if (Directory.Exists(dllPath))
		{
			Debug.Log("Для очистки удалите Unity и вручную уберите папку ScriptAssemblies. Автоматически из редактора это делать нельзя.");
		}
	}

	public static void DoInjectMainClient()
	{
		CodeInjector injector = new CodeInjector();
		string file = Application.dataPath + @"/Lib/XMainClient.dll";
		injector.AddAssembly(file);
		injector.Run();
		Debug.Log("Do MainClient End");
	}

	private static readonly string[] editorAssemblies =
	{
		"Assembly-CSharp-Editor.dll", "Assembly-CSharp-firstpass.dll","Assembly-CSharp-Editor-firstpass.dll"
	};

	private static bool DoCodeInjector(string fromPath)
	{
		CodeInjector injector = new CodeInjector();
		DirectoryInfo dir = new DirectoryInfo(fromPath);
		FileInfo[] files = dir.GetFiles("*.dll");
		for (int index = 0; index < files.Length; index++)
		{
			if (!editorAssemblies.Contains(Path.GetFileName(files[index].FullName)))
			{
				injector.AddAssembly(files[index].FullName);
			}
		}
		injector.Run();
		return true;
	}

	public static void DoCodeInjectorFolder(string folderPath)
	{
		DirectoryInfo assemblyDir = new DirectoryInfo(folderPath);
		string outputPath = assemblyDir.Parent.FullName + Path.DirectorySeparatorChar + "CodeInjectored";
		DoCodeInjector(folderPath);

		DirectoryInfo codeInjectoredDir = new DirectoryInfo(outputPath);
		CopyFilesFromDirectory(codeInjectoredDir, assemblyDir);

		codeInjectoredDir.Delete(true);
		Debug.Log("CodeInjector: Finished injectoring and generating assemblies.");
	}

	[PostProcessBuild(1000)]
	private static void OnPostprocessBuildPlayer(BuildTarget buildTarget, string buildPath)
	{
		if (ABSystem.ABBuilder.isBuildAB) return;
		Debug.Log("PostProcessBuild::OnPostprocessBuildPlayer");

		bool windowsOrLinux = (buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneWindows64);
		if (windowsOrLinux)
		{
			var buildDir = new FileInfo(buildPath).Directory;
			DirectoryInfo dataDir = buildDir.GetDirectories(Path.GetFileNameWithoutExtension(buildPath) + "_Data")[0];
			DirectoryInfo managedDir = new DirectoryInfo(dataDir.FullName + Path.DirectorySeparatorChar + "Managed");
			DoCodeInjectorFolder(managedDir.FullName);
		}
		else if (buildTarget == BuildTarget.StandaloneOSXIntel)
		{
			FileInfo buildFileInfo = new FileInfo(buildPath);
			DirectoryInfo dataDir = new DirectoryInfo(buildFileInfo.FullName + Path.DirectorySeparatorChar + "Contents" + Path.DirectorySeparatorChar + "Data");
			DirectoryInfo managedDir = new DirectoryInfo(dataDir.FullName + Path.DirectorySeparatorChar + "Managed");
			DoCodeInjectorFolder(managedDir.FullName);
		}
		else if (buildTarget == BuildTarget.Android || buildTarget == BuildTarget.iOS)
		{
			// Ничего не делаем
		}
		else
		{
			Debug.LogWarning("CodeInjector: Post-build injection is not implemented for: " + buildTarget);
			return;
		}
		Debug.Log("CodeInjector: Post-build injection and protection finished.");
	}

	private static bool CopyFilesFromDirectory(DirectoryInfo source, DirectoryInfo destination)
	{
		if (!source.Exists)
		{
			Debug.LogError("CodeInjector: Cannot copy from " + source + " since it doesn't exist!");
			return false;
		}
		if (!destination.Exists) destination.Create();

		FileInfo[] files = source.GetFiles();
		for (int index = 0; index < files.Length; index++)
		{
			FileInfo file = files[index];
			file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
		}

		DirectoryInfo[] dirs = source.GetDirectories();
		for (int index = 0; index < dirs.Length; index++)
		{
			DirectoryInfo directory = dirs[index];
			string destinationDir = Path.Combine(destination.FullName, directory.Name);
			CopyFilesFromDirectory(directory, new DirectoryInfo(destinationDir));
		}
		return true;
	}
}
