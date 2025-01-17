using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using System;

public class XGitExtractor
{
    static readonly string pattern = @"\b[MDAR]\d*\t(XProject/Assets/[\w/\.\-]+)(\n+|(\t(XProject/Assets/[\w/\.\-]+)\b))";

    private static string _result = null;

    private static List<string> _commits = new List<string>();
    private static List<string> _files = new List<string>();

    public static List<string> A_files = new List<string>();
    public static List<string> M_files = new List<string>();
    public static List<string> D_files = new List<string>();

    public static string CurrentBranch()
    {
        return Execute(null, "gitcbranch");
    }

    public static void CreateStableBranch(string tag)
    {
        Execute("stable-" + tag, "gitstable", false);
    }

    public static bool Run(string tag)
    {
        Execute(tag, "gitextract");

        _files.Clear();

        A_files.Clear();
        M_files.Clear();
        D_files.Clear();

        _commits.Clear();

        if (_result.Length < 5)
        {
            return _result.StartsWith("0");
        }

        CrudeRefine();

        for (int i = 0; i < _commits.Count; i++)
            Refine(_commits[i], i);

        return true;
    }

    public static bool Push(string tag)
    {
        Execute(tag, "gitpush");
        return true;
    }

    public static bool Pull(string tag)
    {
        Execute(tag, "gitpull", false);
        return true;
    }

    public static bool TagSrc(string tag)
    {
        Execute(tag, "gittagsrc");
        return true;
    }

    public static bool IsWorkingSpaceCleaning()
    {
        string res = Execute(null, "gitclean");
        return res.Contains("nothing to commit, working directory clean");
    }

    private static void CrudeRefine()
    {
        int start = 0;
        int end = 0;

        while (start >= 0)
        {
            start = _result.IndexOf("commit", start);
            end = start;

            do
            {
                end = _result.IndexOf("commit", end + 5);
            } while (!ValidCommit(end));

            string commit = end < 0 ? _result.Substring(start) : _result.Substring(start, end - start);
            if (!string.IsNullOrEmpty(commit)) _commits.Add(commit);

            start = end;
        }
    }

    private static bool ValidCommit(int end)
    {
        if (end < 0) return true;

        if (_result.Length <= end + 47) return false;

        string sub = _result.Substring(end, 47);
        Regex r = new Regex(@"^commit (\d|[a-z]){40}");

        Match m = r.Match(sub);

        return m.Success;
    }

    private static void Refine(string commit, int idx)
    {
        Regex r = new Regex(pattern);
        MatchCollection mc = r.Matches(commit);

        foreach (Match m in mc)
        {
            string content = m.Groups[1].Value.Substring(16);
            if (content.EndsWith(".meta")) continue;

            if (content.Contains("-version.bytes") || content.Contains(".assetbundle")) continue;

            if (m.Value[0] == 'R')
            {
                string newcontent = m.Groups[4].Value.Substring(16);

                if (!_files.Contains(content))
                {
                    D_files.Add(content);
                    _files.Add(content);
                }

                if (!_files.Contains(newcontent))
                {
                    A_files.Add(newcontent);
                    _files.Add(newcontent);
                }
            }
            else
            {
                if (_files.Contains(content)) continue;

                if (m.Value[0] == 'A')
                {
                    A_files.Add(content);
                    _files.Add(content);
                }
                else if (m.Value[0] == 'M')
                {
                    //ignore other *.dll modified commit
                    if (!content.Contains(".dll"))
                    {
                        M_files.Add(content);
                        _files.Add(content);
                    }
                    else
                    {
                        if (commit.Contains("push release dlls"))
                        {
                            M_files.Add(content);
                            _files.Add(content);
                        }
                    }
                }
                else if (m.Value[0] == 'D')
                {
                    D_files.Add(content);
                    _files.Add(content);
                }
            }
        }
    }

    public static string Execute(string tag, string sh, bool prefix = true)
    {
        if (prefix && !string.IsNullOrEmpty(tag))
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.Android: tag = "android-" + tag; break;
                case UnityEditor.BuildTarget.iOS: tag = "ios-" + tag; break;
                default:
                    {
                        throw new Exception();
                    };
            }
        }

        Process p = new Process();
        p.StartInfo.FileName = "sh";
        p.StartInfo.Arguments = Application.dataPath + "/Editor/XBundleController/" + sh + (string.IsNullOrEmpty(tag) ? ".sh" : ".sh " + tag);

        p.StartInfo.WorkingDirectory = Application.dataPath;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();

        _result = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        return _result;
    }
}
