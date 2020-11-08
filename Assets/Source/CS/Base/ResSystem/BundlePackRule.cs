using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum BundlePackRuleDefine
{
    PackOneByOne = 0,               //  a/b/c.prb 打包为  a@b@c.ab
    PackToRootFolder = 1,           //  a/b/c.prb 打包为  a.ab
    PackToParentFolder = 2,         //  a/b/c.prb 打包为  a@b.ab
    PackToSelfFolderByUnderline = 3,//  a/b/c_1.prb a/b/c_2.prb 打包为  a@b@c.ab
    DoNotPack = 4                 //目录不会打包
}

public class BundlePackRule
{
    private static BundlePackRule DefaultRule = new BundlePackRule("/", BundlePackRuleDefine.PackToParentFolder); // 默认打包规则

    public string directory;        //要打包的目录
    public BundlePackRuleDefine rule;   //打包规则
    public string _rule;

    public BundlePackRule()
    {
        directory = "";
        rule = BundlePackRuleDefine.PackOneByOne;
        _rule = rule.ToString();
    }


    public BundlePackRule(string dir, BundlePackRuleDefine r)
    {
        if (!dir.EndsWith("/"))
        {
            dir = dir + "/";     // +"/" 避免前缀相同
        }
        directory = dir;
        rule = r;
        _rule = r.ToString();
    }

    //allRules: 会遍历所有规则找到第一个匹配的规则，因此子目录的规则应该排在父目录规则前面
    public static string CalculateBundleNameByPath(string path, List<BundlePackRule> allRules)
    {
        BundlePackRule matchRule = DefaultRule;

        foreach (BundlePackRule bundleRule in allRules)
        {
            if (path.StartsWith(bundleRule.directory))
            {
                matchRule = bundleRule;
                break;
            }
        }
                    
        //switch (matchRule.rule)
        switch(BundlePackRuleDefine.PackOneByOne)
        {
            case BundlePackRuleDefine.PackOneByOne:
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return path.Replace("\\","/").Replace('/', Defines.ABNameSeparatorChar).ToLower();
#else
                return path.Replace('/', Defines.ABNameSeparatorChar).ToLower();
#endif
            case BundlePackRuleDefine.PackToParentFolder:
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return Path.GetDirectoryName(path).Replace("\\","/").Replace('/', Defines.ABNameSeparatorChar).ToLower();
#else
                return Path.GetDirectoryName(path).Replace('/', Defines.ABNameSeparatorChar).ToLower();
#endif
            case BundlePackRuleDefine.PackToRootFolder:
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return matchRule.directory.Trim('/').Replace("\\","/").Replace('/', Defines.ABNameSeparatorChar).ToLower();
#else
                return matchRule.directory.Trim('/').Replace('/', Defines.ABNameSeparatorChar).ToLower();
#endif
            case BundlePackRuleDefine.PackToSelfFolderByUnderline:
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                string[] sArray = fileNameWithoutExtension.Split('_');
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return string.Join("_", Path.GetDirectoryName(path).Replace("\\", "/").Replace('/', Defines.ABNameSeparatorChar), sArray[0]).ToLower();
#else
                return string.Join("_", Path.GetDirectoryName(path).Replace('/', Defines.ABNameSeparatorChar), sArray[0]).ToLower();
#endif
            case BundlePackRuleDefine.DoNotPack:
                return null;
        }

        return null;
    }
}