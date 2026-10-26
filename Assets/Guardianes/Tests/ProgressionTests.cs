using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
namespace Guardianes.Tests {
 public class ProgressionTests {
  PlayerData d;WorldCatalog c;
  [SetUp] public void Setup(){d=new PlayerData();c=ScriptableObject.CreateInstance<WorldCatalog>();}
  [TearDown] public void Cleanup(){UnityEngine.Object.DestroyImmediate(c);}
  [Test] public void DefaultsAreHonest(){Assert.AreEqual(0,d.gold);Assert.AreEqual(0,d.TotalStars);Assert.IsTrue(d.IsUnlocked(1));for(int i=2;i<=10;i++)Assert.IsFalse(d.IsUnlocked(i));}
  [Test] public void LockedLevelsCannotComplete(){Assert.Throws<InvalidOperationException>(()=>d.Complete(2,3));}
  [Test] public void EveryNodeUnlocksSequentially(){for(int i=1;i<=10;i++){Assert.IsTrue(d.IsUnlocked(i));d.Complete(i,3);}Assert.AreEqual(30,d.TotalStars);Assert.IsTrue(d.worlds[1]);}
  [Test] public void BestStarsNeverDecrease(){d.Complete(1,3);d.Complete(1,1);Assert.AreEqual(3,d.stars[0]);}
  [Test] public void InvalidResultsRejected(){Assert.Throws<ArgumentOutOfRangeException>(()=>d.Complete(1,4));Assert.Throws<ArgumentOutOfRangeException>(()=>d.Complete(1,0));}
  [Test] public void ChestBoundariesAndDoubleClaim(){for(int j=0;j<3;j++){int needed=(j+1)*10;for(int i=1;i<=10;i++){if(d.TotalStars>=needed)break;d.Complete(i,Math.Min(3,needed-d.TotalStars));}Assert.IsTrue(d.Claim(j,c.rewards[j]));int balance=d.gold;Assert.IsFalse(d.Claim(j,c.rewards[j]));Assert.AreEqual(balance,d.gold);}}
  [Test] public void LockedChestsRejectClaim(){for(int j=0;j<3;j++)Assert.IsFalse(d.Claim(j,c.rewards[j]));}
  [Test] public void DataCatalogIsValid(){Assert.IsNull(c.ValidateData());}
  [Test] public void SaveRoundtripAndCorruptRecovery(){string dir=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString());try{var s=new SaveService(dir,new UnitySaveCodec());d.Complete(1,2);d.gold=8;s.Save(d);d.gold=9;s.Save(d);Assert.AreEqual(9,s.Load().gold);File.WriteAllText(Path.Combine(dir,"guardianes-v1.json"),"dañado");Assert.AreEqual(8,s.Load().gold);Assert.IsNotNull(s.RecoveryMessage);}finally{Directory.Delete(dir,true);}}
  [Test] public void MissingAndCorruptWithoutBackupResetSafely(){string dir=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString());try{var s=new SaveService(dir,new UnitySaveCodec());Assert.AreEqual(0,s.Load().TotalStars);File.WriteAllText(Path.Combine(dir,"guardianes-v1.json"),"{}");Assert.AreEqual(0,s.Load().TotalStars);}finally{Directory.Delete(dir,true);}}
  [Test] public void FutureVersionIsRejected(){d.version=2;Assert.Throws<InvalidOperationException>(()=>d.Validate());}
  [Test] public void BrokenProgressionIsRejected(){d.stars[9]=3;Assert.Throws<InvalidOperationException>(()=>d.Validate());}
  [Test] public void NonexistentGameplayReturnsDevelopmentRoute(){Assert.IsFalse(LevelLoader.TryLoad(c,d,1));Assert.IsFalse(LevelLoader.TryLoad(c,d,2));}
 }
}
