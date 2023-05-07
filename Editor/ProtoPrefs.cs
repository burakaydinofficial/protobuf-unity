using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace E7.Protobuf
{
    [CreateAssetMenu]
    public class ProtoPrefs : ScriptableObject
    {
        public bool enabled;
        public bool logError;
        public bool logStandard;
        //public string rawExcPath;
        [SerializeField] private string _windows_ExecutionPath = "Packages/Google.Protobuf.Tools.3.22.4/tools/windows_x64/protoc.exe";
        [SerializeField] private string _linux_ExecutionPath = "Packages/Google.Protobuf.Tools.3.22.4/tools/linux_x64/protoc";
        [SerializeField] private string _macOS_ExecutionPath = "Packages/Google.Protobuf.Tools.3.22.4/tools/macosx_x64/protoc";
        public string excPath
        {
            get
            {
                switch (SystemInfo.operatingSystemFamily)
                {
                    case OperatingSystemFamily.Other:
                        return null;
                    case OperatingSystemFamily.MacOSX:
                        return _macOS_ExecutionPath;
                    case OperatingSystemFamily.Windows:
                        return _windows_ExecutionPath;
                    case OperatingSystemFamily.Linux:
                        return _linux_ExecutionPath;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public string grpcPath;
        public string[] IncludePaths = new string[]
        {
            "Packages/Google.Protobuf.Tools.3.22.4/tools/"
        };

        [ContextMenu("Compile All")]
        private void CompileAllInProject()
        {
            _prefs = this;
            ProtobufUnityCompiler.CompileAllInProject();
        }
        
        private static ProtoPrefs _prefs;
        public static ProtoPrefs Instance => _prefs ? _prefs : FindPrefs();

        private static ProtoPrefs FindPrefs()
        {
            var paths = AssetDatabase.FindAssets("t:ProtoPrefs");
            foreach (var path in paths)
            {
                var prefs = AssetDatabase.LoadAssetAtPath<ProtoPrefs>(path);
                if (prefs)
                {
                    _prefs = prefs;
                    break;
                }
            }

            return null;
        }
    }
}