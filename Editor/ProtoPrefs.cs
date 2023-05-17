using System;
using System.Linq;
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
                CheckExecutionPaths();
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
        [SerializeField] private string _windows_grpcPath = "Packages/Grpc.Tools.2.54.0/tools/windows_x64/grpc_csharp_plugin.exe";
        [SerializeField] private string _linux_grpcPath = "Packages/Grpc.Tools.2.54.0/tools/linux_x64/grpc_csharp_plugin";
        [SerializeField] private string _macOS_grpcPath = "Packages/Grpc.Tools.2.54.0/tools/macosx_x64/grpc_csharp_plugin";

        public string grpcPath
        {
            get
            {
                CheckGrpcPaths();
                switch (SystemInfo.operatingSystemFamily)
                {
                    case OperatingSystemFamily.Other:
                        return null;
                    case OperatingSystemFamily.MacOSX:
                        return _macOS_grpcPath;
                    case OperatingSystemFamily.Windows:
                        return _windows_grpcPath;
                    case OperatingSystemFamily.Linux:
                        return _linux_grpcPath;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
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

        private void CheckExecutionPaths()
        {
            if (!File.Exists(_windows_ExecutionPath))
            {
                var dirs = Directory.GetDirectories("Packages", "Google.Protobuf.Tools*", SearchOption.TopDirectoryOnly);
                for (var i = dirs.Length - 1; i >= 0; i--)
                {
                    var dir = dirs[i];
                    if (File.Exists(Path.Combine(dir, "tools/windows_x64/protoc.exe")))
                    {
                        _windows_ExecutionPath = Path.Combine(dir, "tools/windows_x64/protoc.exe").Replace('\\','/');
                        _linux_ExecutionPath = Path.Combine(dir, "tools/linux_x64/protoc").Replace('\\', '/');
                        _macOS_ExecutionPath = Path.Combine(dir, "tools/macosx_x64/protoc").Replace('\\', '/');
                        var include = IncludePaths.ToList();
                        include.RemoveAll(x => dirs.Any(y => x.StartsWith(y)));
                        include.Add(Path.Combine(dir, "tools/").Replace('\\', '/'));
                        return;
                    }
                }

                CheckGrpcPaths();
            }
        }

        private void CheckGrpcPaths()
        {
            if (!File.Exists(_windows_grpcPath))
            {
                var dirs = Directory.GetDirectories("Packages", "Grpc.Tools*", SearchOption.TopDirectoryOnly);
                for (var i = dirs.Length - 1; i >= 0; i--)
                {
                    var dir = dirs[i];
                    if (File.Exists(Path.Combine(dir, "tools/windows_x64/protoc.exe")))
                    {
                        _windows_grpcPath = Path.Combine(dir, "tools/windows_x64/grpc_csharp_plugin.exe").Replace('\\', '/');
                        _linux_grpcPath = Path.Combine(dir, "tools/linux_x64/grpc_csharp_plugin").Replace('\\', '/');
                        _macOS_grpcPath = Path.Combine(dir, "tools/macosx_x64/grpc_csharp_plugin").Replace('\\', '/');
                        _windows_ExecutionPath = Path.Combine(dir, "tools/windows_x64/protoc.exe").Replace('\\', '/');
                        _linux_ExecutionPath = Path.Combine(dir, "tools/linux_x64/protoc").Replace('\\', '/');
                        _macOS_ExecutionPath = Path.Combine(dir, "tools/macosx_x64/protoc").Replace('\\', '/');
                        var include = IncludePaths.ToList();
                        include.RemoveAll(x => dirs.Any(y => x.StartsWith(y)));
                        include.Add(Path.Combine(dir, "tools/").Replace('\\', '/'));
                        return;
                    }
                }
            }
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