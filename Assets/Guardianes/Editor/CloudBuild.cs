using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;

namespace Guardianes.Editor
{
    public static class CloudBuild
    {
        public const string ScenePath = "Assets/Guardianes/WorldMap/Scenes/World1.unity";
        public const string ArtifactRoot = "CIArtifacts";
        [Serializable] sealed class GenerationReport
        {
            public string unityVersion, scene, generatedUtc;
            public int levels, starImages, worlds, chests, environmentLayers;
            public bool referencesValidated;
        }
        [Serializable] sealed class AndroidReport
        {
            public string unityVersion, result, outputPath, generatedUtc;
            public int errors, warnings;
            public long sizeBytes;
            public double durationSeconds;
            public bool installedOnDevice = false;
        }
        public static void PrepareForTests()
        {
            Directory.CreateDirectory(ArtifactRoot + "/Scene");
            World1Builder.Configure();
            World1Builder.GenerateInternal();
            ValidateOrThrow();
            var root = UnityEngine.Object.FindObjectOfType<WorldMapController>();
            var names = root.GetComponentsInChildren<Transform>(true).Select(x => x.name).ToArray();
            var report = new GenerationReport {
                unityVersion = Application.unityVersion, scene = ScenePath,
                generatedUtc = DateTime.UtcNow.ToString("o"), levels = 10,
                starImages = names.Count(x => x.StartsWith("Estrella_")), worlds = 6, chests = 3,
                environmentLayers = EnvironmentPlan.Load().layers.Length, referencesValidated = true
            };
            File.WriteAllText(ArtifactRoot + "/Scene/generation.json", JsonUtility.ToJson(report, true));
            File.Copy(ScenePath, ArtifactRoot + "/Scene/World1.unity", true);
        }
        public static void ValidateOrThrow()
        {
            var root = UnityEngine.Object.FindObjectOfType<WorldMapController>();
            if (!root || !root.catalog) throw new BuildFailedException("Falta escena o catálogo");
            string invalid = root.catalog.ValidateData();
            if (invalid != null) throw new BuildFailedException(invalid);
            string[] art = { "forest", "plaque", "node", "locked", "selected", "bronze", "silver", "gold", "star", "play", "blue", "purple", "coin" };
            foreach (var key in art) if (!MapView.Art(key)) throw new BuildFailedException("Falta sprite: " + key);
            foreach (var effect in EnvironmentPlan.Load().effects)
            {
                if (!Resources.Load<Texture2D>("Art/" + effect.sheet)) throw new BuildFailedException("Falta textura: " + effect.sheet);
                var material = Resources.Load<Material>("Effects/" + effect.key);
                if (!material || !material.shader || ShaderUtil.ShaderHasError(material.shader)) throw new BuildFailedException("Material/shader inválido: " + effect.key);
            }
            var transforms = root.GetComponentsInChildren<Transform>(true);
            for (int i = 1; i <= 10; i++)
            {
                var node = transforms.SingleOrDefault(x => x.name == "Nivel_" + i);
                if (!node || !node.GetComponentInChildren<Button>(true)) throw new BuildFailedException("Falta botón de nivel " + i);
                if (node.GetComponentsInChildren<Image>(true).Count(x => x.name.StartsWith("Estrella_")) != 3) throw new BuildFailedException("Faltan estrellas del nivel " + i);
            }
            foreach (string name in new[] { "Entrar", "Ajustes", "Cerrar", "ModalAccion", "PortalSiguiente", "Cofre_0", "Cofre_1", "Cofre_2" })
                if (!transforms.Any(x => x.name == name && x.GetComponent<Button>())) throw new BuildFailedException("Falta botón: " + name);
        }
        public static void BuildAndroid()
        {
            Directory.CreateDirectory(ArtifactRoot + "/Build");
            Application.logMessageReceived += SaveBuildLog;
            try
            {
                PrepareForTests();
                PlayerSettings.Android.useCustomKeystore = false; // Test APK, default debug signing. No store credentials.
                EditorUserBuildSettings.buildAppBundle = false;
                string output = "build/Android/GuardianesDelPortal.apk";
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android)) throw new BuildFailedException("Android Build Support no está instalado");
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android && !EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android)) throw new BuildFailedException("No se pudo activar Android");
                var build = BuildPipeline.BuildPlayer(new[] { ScenePath }, output, BuildTarget.Android, BuildOptions.Development);
                var summary = build.summary;
                File.WriteAllText(ArtifactRoot + "/Build/build-report.json", JsonUtility.ToJson(new AndroidReport {
                    unityVersion = Application.unityVersion, result = summary.result.ToString(), outputPath = output,
                    errors = (int)summary.totalErrors, warnings = (int)summary.totalWarnings,
                    sizeBytes = (long)summary.totalSize, durationSeconds = summary.totalTime.TotalSeconds,
                    generatedUtc = DateTime.UtcNow.ToString("o")
                }, true));
                if (summary.result != BuildResult.Succeeded || !File.Exists(output)) throw new BuildFailedException("APK no generada: " + summary.result);
            }
            finally { Application.logMessageReceived -= SaveBuildLog; }
        }
        static void SaveBuildLog(string condition, string trace, LogType type)
        {
            File.AppendAllText(ArtifactRoot + "/Build/build.log", "[" + type + "] " + condition + "\n" + trace + "\n");
        }
    }
}
