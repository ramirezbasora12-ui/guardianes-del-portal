using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Guardianes.PlayModeTests
{
    public sealed class MapInteractionTests
    {
        WorldMapController controller; string directory;
        Transform Find(string name) { return controller.GetComponentsInChildren<Transform>(true).Single(x => x.name == name); }
        Button Button(string name) { return Find(name).GetComponent<Button>(); }
        [UnitySetUp]
        public IEnumerator LoadMap()
        {
            directory = Path.Combine(Path.GetTempPath(), "guardianes-playmode-" + Guid.NewGuid());
            SaveLocation.TestDirectory = directory;
            SceneManager.LoadScene("World1");
            yield return null; yield return null;
            controller = UnityEngine.Object.FindObjectOfType<WorldMapController>();
            Assert.IsNotNull(controller);
        }
        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            if (controller) UnityEngine.Object.Destroy(controller.gameObject);
            yield return null;
            SaveLocation.TestDirectory = null;
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
        [UnityTest]
        public IEnumerator TenButtonsStarsAndDevelopmentScreens()
        {
            for (int i = 1; i <= 10; i++)
            {
                var button = Find("Nivel_" + i).GetComponentInChildren<Button>();
                Assert.AreEqual(i == 1, button.interactable, "Estado inicial de nodo " + i);
            }
            Assert.IsFalse(controller.SubmitLevelResult(2, 3));
            for (int i = 1; i <= 10; i++)
            {
                var node = Find("Nivel_" + i); var button = node.GetComponentInChildren<Button>();
                Assert.IsTrue(button.interactable); button.onClick.Invoke();
                StringAssert.Contains("NIVEL " + i, Find("ModalTitulo").GetComponent<Text>().text);
                Button("ModalAccion").onClick.Invoke();
                StringAssert.Contains("EN DESARROLLO", Find("ModalTitulo").GetComponent<Text>().text);
                Button("Cerrar").onClick.Invoke();
                Assert.IsTrue(controller.SubmitLevelResult(i, 3));
                for (int star = 0; star < 3; star++) Assert.AreEqual(Color.white, node.Find("Estrellas/Estrella_" + star).GetComponent<Image>().color);
            }
            StringAssert.Contains("30 / 30", Find("TotalEstrellas").GetComponent<Text>().text);
            Assert.IsTrue(Find("PortalSiguiente").gameObject.activeSelf);
            yield return null;
        }
        [UnityTest]
        public IEnumerator ChestsCannotBeClaimedTwiceAndPersist()
        {
            for (int i = 1; i <= 10; i++) Assert.IsTrue(controller.SubmitLevelResult(i, 3));
            for (int i = 0; i < 3; i++)
            {
                Button("Cofre_" + i).onClick.Invoke();
                yield return new WaitForSecondsRealtime(1.2f);
                Assert.AreEqual("RECLAMADO", Button("Cofre_" + i).transform.Find("Estado").GetComponent<Text>().text);
                Button("Cofre_" + i).onClick.Invoke();
                StringAssert.Contains("reclamado", Find("ModalTitulo").GetComponent<Text>().text.ToLowerInvariant());
                Button("Cerrar").onClick.Invoke();
            }
            Assert.AreEqual("850", Find("Saldo0").GetComponent<Text>().text);
            SceneManager.LoadScene("World1"); yield return null; yield return null;
            controller = UnityEngine.Object.FindObjectOfType<WorldMapController>();
            Assert.AreEqual("850", Find("Saldo0").GetComponent<Text>().text);
            for (int i = 0; i < 3; i++) Assert.AreEqual("RECLAMADO", Button("Cofre_" + i).transform.Find("Estado").GetComponent<Text>().text);
        }
        [UnityTest]
        public IEnumerator MainButtonsAndWorldSidebar()
        {
            Button("Entrar").onClick.Invoke(); Assert.IsTrue(Find("Modal").gameObject.activeSelf);
            Button("Cerrar").onClick.Invoke(); Assert.IsFalse(Find("Modal").gameObject.activeSelf);
            for (int i = 1; i < 6; i++) { Button("Mundo_" + i).onClick.Invoke();StringAssert.Contains("bloqueado", Find("ModalTexto").GetComponent<Text>().text.ToLowerInvariant());Button("Cerrar").onClick.Invoke(); }
            Button("Ajustes").onClick.Invoke();Assert.AreEqual(2, controller.GetComponentsInChildren<Slider>().Length);
            foreach (var slider in controller.GetComponentsInChildren<Slider>()) slider.value=.25f;
            Button("Cerrar").onClick.Invoke();Assert.IsFalse(Find("Modal").gameObject.activeSelf);
            yield return null;
        }
        [UnityTest, Category("Visual")]
        public IEnumerator CaptureActualUnityScene()
        {
            string requested = Environment.GetEnvironmentVariable("GUARDIANES_CAPTURE");
            if (requested != "true" && requested != "1") Assert.Ignore("Capturas no solicitadas en esta ejecución.");
            Assert.AreNotEqual(GraphicsDeviceType.Null, SystemInfo.graphicsDeviceType, "No hay renderizador. No se fabricará una captura.");
            var canvas=controller.GetComponentInChildren<Canvas>();var camera=Camera.main;
            Assert.IsNotNull(camera);canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=2;
            Directory.CreateDirectory("CIArtifacts/Screenshots");
            foreach (int width in new[] {1920,2400})
            {
                const int height=1080;
                var target=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32);
                var previous=RenderTexture.active;
                try
                {
                    camera.targetTexture=target;camera.aspect=(float)width/height;
                    yield return null;Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;
                    var image=new Texture2D(width,height,TextureFormat.RGB24,false);
                    try { image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();File.WriteAllBytes("CIArtifacts/Screenshots/World1-"+width+"x"+height+".png",image.EncodeToPNG()); }
                    finally { UnityEngine.Object.Destroy(image); }
                }
                finally { camera.targetTexture=null;RenderTexture.active=previous;target.Release();UnityEngine.Object.Destroy(target); }
            }
            File.WriteAllText("CIArtifacts/Screenshots/procedencia.txt", "Render real de la escena World1 en Unity PlayMode; Camera.Render a RenderTexture. Canvas cambiado temporalmente de Overlay a ScreenSpaceCamera. No es captura Android ni ilustración conceptual. GPU: "+SystemInfo.graphicsDeviceName+"; Unity: "+Application.unityVersion);
        }
    }
}
