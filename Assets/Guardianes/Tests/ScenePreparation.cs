using NUnit.Framework;
namespace Guardianes.Tests
{
    [SetUpFixture]
    public sealed class ScenePreparation
    {
        [OneTimeSetUp]
        public void GenerateAndValidateRealScene()
        {
            Guardianes.Editor.CloudBuild.PrepareForTests();
        }
    }
}
