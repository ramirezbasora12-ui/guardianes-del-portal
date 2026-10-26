using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace Guardianes {
 public sealed class SaveService {
  [Serializable] public class Envelope { public string payload; public string sha256; }
  readonly string path; readonly ISaveCodec codec; public string RecoveryMessage{get;private set;}
  public SaveService(string directory,ISaveCodec serializer){codec=serializer??throw new ArgumentNullException(nameof(serializer));Directory.CreateDirectory(directory);path=Path.Combine(directory,"guardianes-v1.json");}
  static string Hash(string s){using(var h=SHA256.Create())return Convert.ToBase64String(h.ComputeHash(Encoding.UTF8.GetBytes(s)));}
  PlayerData Read(string file){var e=codec.Decode<Envelope>(File.ReadAllText(file));if(e==null||e.payload==null||e.sha256!=Hash(e.payload))throw new InvalidDataException("Integridad inválida");var d=codec.Decode<PlayerData>(e.payload);if(d==null)throw new InvalidDataException();d.Validate();return d;}
  public PlayerData Load(){
   RecoveryMessage=null;
   if(!File.Exists(path)&&!File.Exists(path+".bak"))return new PlayerData();
   try{return Read(path);}catch(Exception e)when(e is IOException||e is InvalidDataException||e is ArgumentException||e is InvalidOperationException){
    RecoveryMessage="Se recuperó un guardado dañado. Se conserva el archivo para diagnóstico.";
    try{if(File.Exists(path))File.Copy(path,path+".corrupt",true);return Read(path+".bak");}
    catch(Exception b)when(b is IOException||b is InvalidDataException||b is ArgumentException||b is InvalidOperationException){return new PlayerData();}
   }
  }
  public void Save(PlayerData d){
   d.Validate();string payload=codec.Encode(d);var text=codec.Encode(new Envelope{payload=payload,sha256=Hash(payload)});
   string tmp=path+".tmp";
   using(var f=new FileStream(tmp,FileMode.Create,FileAccess.Write,FileShare.None)){
    byte[] bytes=Encoding.UTF8.GetBytes(text);f.Write(bytes,0,bytes.Length);f.Flush(true);
   }
   if(File.Exists(path)){// Never replace a good backup with a corrupt primary.
    try{Read(path);File.Copy(path,path+".bak",true);}catch(Exception e)when(e is IOException||e is InvalidDataException||e is ArgumentException||e is InvalidOperationException){}
    File.Delete(path);
   }
   File.Move(tmp,path);
  }
 }
}
