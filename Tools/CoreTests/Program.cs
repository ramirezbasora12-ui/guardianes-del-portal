using Guardianes;
using System.Text.Json;

// Runs the actual model and storage source files, not copies or mocked Unity APIs.
sealed class SystemTextCodec : ISaveCodec
{
    readonly JsonSerializerOptions options = new() { IncludeFields = true, IgnoreReadOnlyProperties = true };
    public string Encode<T>(T value) => JsonSerializer.Serialize(value, options);
    public T Decode<T>(string text)
    {
        try { return JsonSerializer.Deserialize<T>(text, options); }
        catch (JsonException ex) { throw new ArgumentException("JSON inválido", ex); }
    }
}
static class Program
{
    static readonly List<object> results = new();
    static readonly RewardDefinition[] rewards = {
        new() { stars=10,gold=100,blue=5 }, new() { stars=20,gold=250,blue=15 },
        new() { stars=30,gold=500,blue=30,purple=3 }
    };
    static void Assert(bool value, string error = "Aserción fallida") { if (!value) throw new Exception(error); }
    static void Throws<T>(Action action) where T : Exception
    {
        try { action(); } catch (T) { return; }
        throw new Exception("Faltó excepción " + typeof(T).Name);
    }
    static void Test(string name, Action test)
    {
        try { test(); results.Add(new { name, passed=true, error="" }); Console.WriteLine("OK " + name); }
        catch (Exception ex) { results.Add(new { name, passed=false, error=ex.ToString() }); Console.WriteLine("FALLO " + name + ": " + ex.Message); }
    }
    static PlayerData Stars(int total)
    {
        var data = new PlayerData();
        for (int i=1;total>0;i++) { int earned=Math.Min(3,total);data.Complete(i,earned);total-=earned; }
        return data;
    }
    static void Storage(Action<SaveService,string> test)
    {
        string dir=Path.Combine(Path.GetTempPath(),"guardianes-tests-"+Guid.NewGuid());
        try { test(new SaveService(dir,new SystemTextCodec()),dir); }
        finally { if (Directory.Exists(dir)) Directory.Delete(dir,true); }
    }
    public static int Main(string[] args)
    {
        Test("Saldos iniciales cero",()=>{var d=new PlayerData();Assert(d.gold==0&&d.blue==0&&d.purple==0&&d.TotalStars==0);});
        for(int node=1;node<=10;node++) { int n=node;Test("Desbloqueo y resultado del nodo "+n,()=>{
            var d=Stars((n-1)*3);Assert(d.IsUnlocked(n));if(n<10)Assert(!d.IsUnlocked(n+1));d.Complete(n,3);Assert(d.stars[n-1]==3);if(n<10)Assert(d.IsUnlocked(n+1));
        }); }
        Test("Rechazo de niveles bloqueados",()=>{var d=new PlayerData();for(int n=2;n<=10;n++){int level=n;Throws<InvalidOperationException>(()=>d.Complete(level,3));}Assert(!d.IsUnlocked(0)&&!d.IsUnlocked(11));});
        Test("Las estrellas no disminuyen",()=>{var d=Stars(3);d.Complete(1,1);Assert(d.TotalStars==3);});
        Test("Resultados inválidos",()=>{var d=new PlayerData();Throws<ArgumentOutOfRangeException>(()=>d.Complete(1,0));Throws<ArgumentOutOfRangeException>(()=>d.Complete(1,4));});
        Test("Máximo 30 y mundo siguiente",()=>{var d=Stars(30);Assert(d.TotalStars==30&&d.worlds[1]);d.Validate();Assert(!d.worlds[2]);});
        foreach(int total in new[]{0,9,10,19,20,29,30}){int t=total;Test("Cofres con "+t+" estrellas",()=>{
            var d=Stars(t);for(int c=0;c<3;c++){bool eligible=t>=rewards[c].stars;Assert(d.Claim(c,rewards[c])==eligible);int balance=d.gold;Assert(!d.Claim(c,rewards[c]));Assert(balance==d.gold);}
        });}
        Test("Recompensas acumuladas exactas",()=>{var d=Stars(30);for(int i=0;i<3;i++)Assert(d.Claim(i,rewards[i]));Assert(d.gold==850&&d.blue==50&&d.purple==3);});
        Test("Overflow no concede recompensa parcial",()=>{var d=Stars(10);d.gold=int.MaxValue;Throws<OverflowException>(()=>d.Claim(0,rewards[0]));Assert(!d.claimed[0]&&d.blue==0);});
        Test("Guardado y carga de todo el progreso",()=>Storage((s,dir)=>{
            var d=Stars(30);d.selected=10;d.Claim(0,rewards[0]);d.settings.music=.23f;d.settings.sfx=.44f;d.settings.quality=2;s.Save(d);
            var loaded=s.Load();Assert(loaded.selected==10&&loaded.TotalStars==30&&loaded.claimed[0]&&loaded.gold==100&&loaded.settings.music==.23f&&loaded.settings.quality==2);
            Assert(!loaded.Claim(0,rewards[0]));
        }));
        Test("Recuperación de respaldo",()=>Storage((s,dir)=>{var d=Stars(3);d.gold=7;s.Save(d);d.gold=9;s.Save(d);File.WriteAllText(Path.Combine(dir,"guardianes-v1.json"),"corrupto");Assert(s.Load().gold==7);Assert(s.RecoveryMessage!=null&&File.Exists(Path.Combine(dir,"guardianes-v1.json.corrupt")));}));
        Test("Archivos ausentes",()=>Storage((s,dir)=>Assert(s.Load().TotalStars==0)));
        Test("Ambos archivos dañados",()=>Storage((s,dir)=>{File.WriteAllText(Path.Combine(dir,"guardianes-v1.json"),"{}");File.WriteAllText(Path.Combine(dir,"guardianes-v1.json.bak"),"{}");Assert(s.Load().TotalStars==0);}));
        Test("Checksum alterado",()=>Storage((s,dir)=>{s.Save(Stars(3));string file=Path.Combine(dir,"guardianes-v1.json");string text=File.ReadAllText(file);File.WriteAllText(file,text.Replace("sha256\":\"","sha256\":\"cambiado"));Assert(s.Load().TotalStars==0);}));
        Test("Versión futura rechazada",()=>{var d=new PlayerData{version=2};Throws<InvalidOperationException>(d.Validate);});
        Test("Progresión inconsistente rechazada",()=>{var d=new PlayerData();d.stars[9]=3;Throws<InvalidOperationException>(d.Validate);});
        Test("Volúmenes y calidad normalizados",()=>{var d=new PlayerData();d.settings.music=4;d.settings.sfx=-2;d.settings.quality=99;d.Validate();Assert(d.settings.music==1&&d.settings.sfx==0&&d.settings.quality==2);});
        Test("NaN rechazado",()=>{var d=new PlayerData();d.settings.music=float.NaN;Throws<InvalidOperationException>(d.Validate);});
        int failed=results.Count(x=>!(bool)x.GetType().GetProperty("passed").GetValue(x));
        string output=args.Length>0?args[0]:"Validation/core-results.json";
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
        File.WriteAllText(output,JsonSerializer.Serialize(new { scope="Modelo y almacenamiento C# reales; codec System.Text.Json; no Unity ni clics UI", total=results.Count,failed,unityExecuted=false,tests=results },new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine($"Resultado: {results.Count-failed}/{results.Count}. No valida UnitySaveCodec ni la escena Unity.");
        return failed==0?0:1;
    }
}
