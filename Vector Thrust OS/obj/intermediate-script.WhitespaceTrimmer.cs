// <mdk sortorder="30" />   // BlockClasses.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame;
using VRageMath;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Text;
using VRage.Game.ModAPI.Ingame.Utilities;
class Program : MyGridProgram
{
sealed class Y{readonly Program A;float B=float.NaN;public readonly IMyThrust C;public readonly D E;public F G;public
bool H,I;public double J;public long K{get{return C.EntityId;}}public Vector3D L{get{return C.WorldMatrix.Backward;}}public
Vector3D M{get{return C.WorldMatrix.Forward;}}public double P{get{if(C==null||C.Closed||!C.IsFunctional){return 0;}double N=C.
MaxEffectiveThrust;return N>O?N:0;}}public Vector3D Q{get{if(C==null||C.Closed||!C.IsFunctional){return Vector3D.Zero;}return L*C.
CurrentThrust;}}public bool S{get{return(E&D.R)!=0;}}public bool U{get{if(C==null||C.Closed||!C.IsFunctional||P<=O){return false;}
return C.Enabled||A.T(K);}}public Y(IMyThrust V,Program A,D W,bool X){C=V;this.A=A;E=W;H=X;}public void Z(){J=0;}public double
h(ref Vector3D a){if(!H||!U){return 0;}Vector3D d=b.c(L);double e=Vector3D.Dot(a,d);if(e<=O){return 0;}double f=P-J;if(f
<=O){return 0;}double g=Math.Min(e,f);J+=g;a-=d*g;return g;}public void k(){if(!H||C==null||C.Closed){return;}double i=P;
float j=i>O?(float)MathHelper.Clamp(J/i,0,1):0;if(!float.IsNaN(B)&&Math.Abs(j-B)<1e-4f&&Math.Abs(C.ThrustOverridePercentage-j
)<1e-4f){return;}C.ThrustOverridePercentage=B=j;}public void l(){J=0;if(C==null||C.Closed){return;}if(Math.Abs(C.
ThrustOverridePercentage)>1e-5f){C.ThrustOverridePercentage=0;}B=0;}public void m(){l();H=false;}}sealed class z{readonly Program A;double n=
double.NaN,o;bool p,q;public readonly IMyMotorStator C;public readonly D E;public F G;public bool H;public long K{get{return C
.EntityId;}}public bool r{get{return C.BlockDefinition.SubtypeId.IndexOf("Hinge",StringComparison.OrdinalIgnoreCase)>=0;}
}public Vector3D s{get{return C.WorldMatrix.Up;}}public bool y{get{if(C==null||C.Closed||C.Top==null||!C.IsFunctional||!C
.Enabled||C.RotorLock){return false;}double t=C.LowerLimitRad;double u=C.UpperLimitRad;return!v(t)||!w(u)||Math.Abs(u-t)>
x;}}public z(IMyMotorStator V,Program A,D W,bool X){C=V;this.A=A;E=W;H=X;}public double Ê(Vector3D ª){if(!H||!y||G==null)
{µ(0);return 0;}º();Vector3D Á=G.À;double Ã=b.Â(ª,Á,s);double Å=Ä(Ã);µ(Å*Æ);Vector3D È=b.Ç(Á,s,-Å);return b.É(È,ª);}
public bool Î(Vector3D ª,out double Ë,out double Ì){Ë=0;Ì=0;if(!H||!y||G==null){return false;}Vector3D Á=G.À;if(Á.
LengthSquared()<=Í||ª.LengthSquared()<=Í){return false;}double Ã=b.Â(ª,Á,s);Ë=Ä(Ã);Vector3D È=b.Ç(Á,s,-Ë);Ì=b.É(È,ª);return true;}
public void Ý(Vector3D Ï,Vector3D Ð){p=false;q=false;if(!H||!y||G==null){µ(0);q=true;return;}double Ë;double Ì;if(Ï.
LengthSquared()>Í){Vector3D Ñ=-b.c(Ï);if(Î(Ñ,out Ë,out Ì)&&Ì>=Ò){Ó(Ë);return;}}Vector3D Õ=G.Ô();Vector3D Ö=C.GetPosition();Vector3D Ø
=Õ-Ö;Vector3D Ù=Ð-Ö;Vector3D Û=b.Ú(Ø,s);Vector3D Ü=b.Ú(Ù,s);if(Û.LengthSquared()<=Í||Ü.LengthSquared()<=Í){Ó(0);return;}Ë
=b.Â(Ü,Û,s);Ë=Ä(Ë);Ó(Ë);}public void â(){if(q){return;}if(!p||!H||!y){µ(0);q=true;return;}double Þ=o-C.Angle;if(!ß()){Þ=b
.à(Þ);}if(Math.Abs(Þ)<=á){µ(0);q=true;return;}µ(Þ*Æ);}public void º(){p=false;q=false;}public void m(){µ(0);H=false;}void
Ó(double Ë){o=C.Angle+Ë;if(v(C.LowerLimitRad)){o=Math.Max(o,C.LowerLimitRad);}if(w(C.UpperLimitRad)){o=Math.Min(o,C.
UpperLimitRad);}p=true;}double Ä(double ã){ã=b.à(ã);bool ä=v(C.LowerLimitRad);bool å=w(C.UpperLimitRad);if(!ä&&!å){return ã;}double æ
=C.Angle;double ç=double.NaN;double è=double.MaxValue;for(int é=-2;é<=2;é++){double ê=ã+é*MathHelper.TwoPi;double ë=æ+ê;
if(ä&&ë<C.LowerLimitRad-á){continue;}if(å&&ë>C.UpperLimitRad+á){continue;}double ì=Math.Abs(ê);if(ì<è){è=ì;ç=ê;}}if(!
double.IsNaN(ç)){return ç;}double í=æ+ã;if(ä){í=Math.Max(í,C.LowerLimitRad);}if(å){í=Math.Min(í,C.UpperLimitRad);}return í-æ;}
void µ(double î){if(C==null||C.Closed){return;}î=MathHelper.Clamp(î,-ï,ï);if(Math.Abs(î)<=ð){î=0;}if(!double.IsNaN(n)&&Math.
Abs(n-î)<ð&&Math.Abs(C.TargetVelocityRad-î)<ð){return;}C.TargetVelocityRad=(float)î;n=î;}bool ß(){return v(C.LowerLimitRad)
||w(C.UpperLimitRad);}static bool v(double ñ){return!double.IsNaN(ñ)&&!double.IsInfinity(ñ)&&ñ>-1e20;}static bool w(double
ñ){return!double.IsNaN(ñ)&&!double.IsInfinity(ñ)&&ñ<1e20;}}sealed class F{sealed class ô{public Vector3D ò;public double
ó;}readonly Program A;readonly List<ô>õ=new List<ô>();public readonly z z;public readonly List<Y>ö=new List<Y>();public
readonly List<IMyCubeGrid>ø=new List<IMyCubeGrid>();public Vector3D ù=Vector3D.Zero,ú=Vector3D.Zero;public double û;public F(z ü
,Program A){z=ü;this.A=A;ü.G=this;}public Vector3D s{get{return z.s;}}public Vector3D À{get{IMyCubeGrid ý=z.C.TopGrid;if(
ý==null||ù.LengthSquared()<=Í){return Vector3D.Zero;}Vector3D ÿ=b.þ(ù,ý.WorldMatrix);return-b.c(ÿ);}}public void Ċ(){õ.
Clear();for(int Ā=0;Ā<ö.Count;Ā++){ö[Ā].I=false;}IMyCubeGrid ý=z.C.TopGrid;if(ý==null){ù=Vector3D.Zero;û=0;return;}MatrixD ā=
ý.WorldMatrix;for(int Ā=0;Ā<ö.Count;Ā++){Y Ă=ö[Ā];if(!Ă.H){continue;}double ă=Ă.P;if(ă<=O){continue;}Vector3D ą=b.c(b.Ą(Ă
.M,ā));ô Ć=null;for(int ć=0;ć<õ.Count;ć++){if(Vector3D.Dot(õ[ć].ò,ą)>=Ĉ){Ć=õ[ć];break;}}if(Ć==null){Ć=new ô{ò=ą};õ.Add(Ć)
;}Ć.ó+=ă;}ô ĉ=null;for(int Ā=0;Ā<õ.Count;Ā++){if(ĉ==null||õ[Ā].ó>ĉ.ó){ĉ=õ[Ā];}}if(ĉ==null){ù=Vector3D.Zero;û=0;return;}ù=
ĉ.ò;û=ĉ.ó;for(int Ā=0;Ā<ö.Count;Ā++){Y Ă=ö[Ā];Vector3D ą=b.c(b.Ą(Ă.M,ā));Ă.I=Vector3D.Dot(ą,ù)>=Ĉ;}}public double ċ(
Vector3D ª){ú=ª;if(ª.LengthSquared()<=Í){z.Ê(Vector3D.Zero);return 0;}return z.Ê(ª);}public double Ē(ref Vector3D a,Vector3D Č,
ref Vector3D č){double Ď=0;for(int Ā=0;Ā<ö.Count;Ā++){Y Ă=ö[Ā];if(!Ă.I){continue;}double ď=Ă.h(ref a);if(ď<=O){continue;}
Vector3D Đ=Ă.L*ď;Vector3D đ=Ă.C.GetPosition()-Č;č+=Vector3D.Cross(đ,Đ);Ď+=ď;}return Ď;}public double ē(ref Vector3D a,Vector3D Č
,ref Vector3D č){double Ď=0;for(int Ā=0;Ā<ö.Count;Ā++){Y Ă=ö[Ā];if(Ă.I){continue;}double ď=Ă.h(ref a);if(ď<=O){continue;}
Vector3D Đ=Ă.L*ď;Vector3D đ=Ă.C.GetPosition()-Č;č+=Vector3D.Cross(đ,Đ);Ď+=ď;}return Ď;}public Vector3D Ô(){if(ø.Count==0){return
z.C.TopGrid!=null?z.C.TopGrid.WorldAABB.Center:z.C.GetPosition();}Vector3D Ĕ=Vector3D.Zero;int ĕ=0;for(int Ā=0;Ā<ø.Count;
Ā++){IMyCubeGrid Ė=ø[Ā];if(Ė==null||Ė.Closed){continue;}Ĕ+=Ė.WorldAABB.Center;ĕ++;}return ĕ>0?Ĕ/ĕ:z.C.GetPosition();}}
sealed class ĝ{public readonly List<F>ė=new List<F>();public Vector3D s{get{return ė.Count>0?b.c(ė[0].s):Vector3D.Zero;}}
public double ę{get{double Ę=0;for(int Ā=0;Ā<ė.Count;Ā++){Ę+=ė[Ā].û;}return Ę;}}public Vector3D Ě(Vector3D Đ){return b.Ú(Đ,s);
}public double Ĝ(Vector3D a){Vector3D ě=Ě(a);if(ě.LengthSquared()<=Í){return 0;}return Math.Min(ě.Length(),ę);}}sealed
class ĩ{readonly Program A;bool Ğ;float ğ=float.NaN,Ġ=float.NaN,ġ=float.NaN;public readonly IMyGyro C;public readonly D E;
public readonly double Ģ;public bool H;public double ę{get{if(!H||C==null||C.Closed||!C.IsFunctional||!C.Enabled){return 0;}
return Ģ*MathHelper.Clamp(C.GyroPower,0,1);}}public ĩ(IMyGyro V,Program A,D W,bool X){C=V;this.A=A;E=W;H=X;bool ģ=V.CubeGrid.
GridSizeEnum==VRage.Game.MyCubeSize.Small;bool Ĥ=V.BlockDefinition.SubtypeId.IndexOf("Prototech",StringComparison.OrdinalIgnoreCase)
>=0;if(Ĥ){Ģ=ģ?ĥ:Ħ;}else{Ģ=ģ?ħ:Ĩ;}}public void Ĵ(Vector3D Ī){if(!H||C==null||C.Closed||ę<=Í){ī();return;}Ī=b.Ĭ(Ī,ĭ);
Vector3D Į=b.Ą(Ī,C.WorldMatrix);float į=(float)Į.X;float İ=(float)Į.Y;float ı=(float)Į.Z;bool ĳ=Į.LengthSquared()>Ĳ*Ĳ;if(!ĳ){ī()
;return;}if(!Ğ||Math.Abs(į-ğ)>Ĳ){C.Pitch=į;ğ=į;}if(!Ğ||Math.Abs(İ-Ġ)>Ĳ){C.Yaw=İ;Ġ=İ;}if(!Ğ||Math.Abs(ı-ġ)>Ĳ){C.Roll=ı;ġ=ı
;}if(!Ğ||!C.GyroOverride){C.GyroOverride=true;}Ğ=true;}public void ī(){if(C==null||C.Closed){return;}if(C.GyroOverride){C
.GyroOverride=false;}if(Math.Abs(C.Pitch)>Ĳ){C.Pitch=0;}if(Math.Abs(C.Yaw)>Ĳ){C.Yaw=0;}if(Math.Abs(C.Roll)>Ĳ){C.Roll=0;}Ğ
=false;ğ=0;Ġ=0;ġ=0;}public void m(){ī();H=false;}}sealed class Ľ{public readonly IMyTerminalBlock ĵ;public readonly
IMyTextSurface Ķ;public readonly int ķ;bool ĸ;public string Ĺ{get{return ĵ.EntityId+":"+ķ;}}public Ľ(IMyTerminalBlock ĺ,IMyTextSurface
Ļ,int ļ){ĵ=ĺ;Ķ=Ļ;ķ=ļ;}public void Ŀ(string ľ){if(ĵ==null||ĵ.Closed||Ķ==null){return;}if(!ĸ){Ķ.ContentType=VRage.Game.GUI.
TextPanel.ContentType.TEXT_AND_IMAGE;Ķ.Font="Monospace";Ķ.FontSize=0.8f;Ķ.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.LEFT;ĸ
=true;}Ķ.WriteText(ľ,false);}}enum Ņ{ŀ,Ł,ł,Ń,ń}[Flags]enum D{ņ=0,Ň=1,R=2,ň=4,ŉ=8,Ŋ=16}sealed class ő{public long ŋ,Ō,ō;
public Vector3D Ŏ;public bool ŏ,Ő;public void œ(ő Œ){ŋ=Œ.ŋ;Ō=Œ.Ō;ō=Œ.ō;Ŏ=Œ.Ŏ;ŏ=Œ.ŏ;Ő=Œ.Ő;}}sealed class ś{readonly Program A;
double Ŕ;double ŕ,Ŗ;int ŗ;public double Ř{get{return Ŕ;}}public double ř{get{return ŕ;}}public double Ś{get{return Ŗ;}}public
ś(Program A){this.A=A;}public void Ŝ(){Ŗ=A.Runtime.LastRunTimeMs;}public void Ş(){double ŝ=A.Runtime.LastRunTimeMs;ŗ++;if
(ŗ==1){Ŕ=ŝ;ŕ=ŝ;return;}Ŕ+=(ŝ-Ŕ)*0.05;if(ŝ>ŕ){ŕ=ŝ;}else if(ŗ%600==0){ŕ=Ŕ;}}}public static class b{public static Vector3D c
(Vector3D ş){if(Vector3D.IsZero(ş)){return Vector3D.Zero;}if(Vector3D.IsUnit(ref ş)){return ş;}return Vector3D.Normalize(
ş);}public static Vector3D Ú(Vector3D Š,Vector3D š){double Ţ=š.LengthSquared();if(Š.LengthSquared()<=Í||Ţ<=Í){return
Vector3D.Zero;}return Š-Vector3D.Dot(Š,š)/Ţ*š;}public static Vector3D ţ(Vector3D Š,Vector3D š){double Ţ=š.LengthSquared();if(Š.
LengthSquared()<=Í||Ţ<=Í){return Vector3D.Zero;}return Vector3D.Dot(Š,š)/Ţ*š;}public static double É(Vector3D Š,Vector3D š){double Ţ=
Math.Sqrt(Š.LengthSquared()*š.LengthSquared());if(Ţ<=Í){return 0;}return MathHelper.Clamp(Vector3D.Dot(Š,š)/Ţ,-1,1);}public
static Vector3D Ĭ(Vector3D ş,double Ť){double ť=ş.LengthSquared();double Ŧ=Ť*Ť;if(ť<=Ŧ){return ş;}if(ť<=Í){return Vector3D.
Zero;}return ş*(Ť/Math.Sqrt(ť));}public static double à(double ŧ){while(ŧ>Math.PI){ŧ-=MathHelper.TwoPi;}while(ŧ<-Math.PI){ŧ
+=MathHelper.TwoPi;}return ŧ;}public static Vector3D Ç(Vector3D ş,Vector3D Ũ,double ŧ){Ũ=c(Ũ);if(Ũ.LengthSquared()<=Í){
return ş;}double ũ=Math.Cos(ŧ);double Ū=Math.Sin(ŧ);return ş*ũ+Vector3D.Cross(Ũ,ş)*Ū+Ũ*Vector3D.Dot(Ũ,ş)*(1.0-ũ);}public
static double Â(Vector3D ū,Vector3D Ŭ,Vector3D ŭ){Vector3D Ů=Ú(ū,ŭ);Vector3D ů=Ú(Ŭ,ŭ);if(Ů.LengthSquared()<=Í||ů.LengthSquared
()<=Í){return 0;}Ů=c(Ů);ů=c(ů);ŭ=c(ŭ);return Math.Atan2(Vector3D.Dot(ŭ,Vector3D.Cross(Ů,ů)),Vector3D.Dot(Ů,ů));}public
static Vector3D Ą(Vector3D Ű,MatrixD ű){return Vector3D.TransformNormal(Ű,MatrixD.Transpose(ű));}public static Vector3D þ(
Vector3D Ų,MatrixD ű){return Vector3D.TransformNormal(Ų,ű);}}sealed class Ÿ{public readonly IMyCubeGrid ų;public readonly List<Ŵ
>ŵ=new List<Ŵ>();public Ŷ ŷ;public Ÿ Ź;public Ŵ ź;public int Ż=int.MaxValue;public bool ż;public Ÿ(IMyCubeGrid Ė){ų=Ė;}}
sealed class Ŵ{public readonly Ÿ Ž,ž;public readonly IMyTerminalBlock ſ;public Ŵ(Ÿ Š,Ÿ š,IMyTerminalBlock ƀ){Ž=Š;ž=š;ſ=ƀ;}
public Ÿ Ƃ(Ÿ Ɓ){return Ɓ==Ž?ž:Ž;}}sealed class Ŷ{public readonly List<Ÿ>ƃ=new List<Ÿ>();public readonly List<IMyShipController
>Ƅ=new List<IMyShipController>();public readonly List<IMyProgrammableBlock>ƅ=new List<IMyProgrammableBlock>();public bool
ż,Ɔ,Ƈ,ƈ;}sealed class Ƌ{public IMyShipConnector Ž,ž;public Ÿ Ɖ,Ɗ;}sealed class Ǝ{public IMyShipConnector ƌ;public Ƌ ƍ;}
sealed class Ə{public IMyLandingGear ƌ;}sealed class ƞ{public bool Ɛ=true,Ƒ=true,ƒ=true,Ɠ,Ɣ=true;public readonly List<double>ƕ
=new List<double>{0.15,0.50,1.00};public string Ɩ="[VT-use]",Ɨ="[VT-ignore]",Ƙ="[VT-status]",ƙ="[VT-park]",ƚ=
"[VT-unpark]";public int ƛ,Ɯ,Ɲ;}readonly MyIni Ɵ=new MyIni();bool ƶ(bool Ơ){string ơ=Me.CustomData??string.Empty;if(!Ơ&&ơ==Ƣ){return
false;}Ɵ.Clear();MyIniParseResult ƣ;if(!Ɵ.TryParse(ơ,out ƣ)){Echo(Ƥ+"\n\nCustom Data could not be parsed as INI:\n"+ƣ);Ƣ=ơ;
return false;}bool Ʀ=ƥ.Ɛ;bool Ƨ=ƥ.Ƒ;bool ƨ=ƥ.ƒ;string Ʃ=ƥ.Ɩ;string ƪ=ƥ.Ɨ;string ƫ=ƥ.Ƙ;string Ƭ=ƥ.ƙ;string ƭ=ƥ.ƚ;ƥ.Ɛ=Ɵ.Get(Ʈ,
"Greedy").ToBoolean(ƥ.Ɛ);ƥ.Ƒ=Ɵ.Get(Ʈ,"CanMaster").ToBoolean(ƥ.Ƒ);ƥ.ƒ=Ɵ.Get(Ʈ,"CanSlave").ToBoolean(ƥ.ƒ);ƥ.Ɠ=Ɵ.Get("Parking",
"ParkOnlyByCommand").ToBoolean(ƥ.Ɠ);ƥ.Ɣ=Ɵ.Get("Flight","CruiseLevelsWithGravity").ToBoolean(ƥ.Ɣ);Ư(Ɵ.Get("Flight","GearPercentages").
ToString("15; 50; 100"));if(ư>=ƥ.ƕ.Count){ư=ƥ.ƕ.Count-1;}ƥ.Ɩ=Ʊ("Tags","Use",ƥ.Ɩ);ƥ.Ɨ=Ʊ("Tags","Ignore",ƥ.Ɨ);ƥ.Ƙ=Ʊ("Tags",
"Status",ƥ.Ƙ);ƥ.ƙ=Ʊ("Tags","ParkTimer",ƥ.ƙ);ƥ.ƚ=Ʊ("Tags","UnparkTimer",ƥ.ƚ);ƥ.ƛ=Math.Max(0,Ɵ.Get("Performance","Update1Skip").
ToInt32(ƥ.ƛ));ƥ.Ɯ=Math.Max(0,Ɵ.Get("Performance","Update10Skip").ToInt32(ƥ.Ɯ));ƥ.Ɲ=Math.Max(0,Ɵ.Get("Performance",
"Update100Skip").ToInt32(ƥ.Ɲ));Ʋ();string Ƴ=Ɵ.ToString();if(Ƴ!=Me.CustomData){Me.CustomData=Ƴ;}Ƣ=Me.CustomData;bool ƴ=Ʀ!=ƥ.Ɛ||Ƨ!=ƥ.Ƒ||ƨ
!=ƥ.ƒ||!Ʃ.Equals(ƥ.Ɩ,StringComparison.OrdinalIgnoreCase)||!ƪ.Equals(ƥ.Ɨ,StringComparison.OrdinalIgnoreCase)||!ƫ.Equals(ƥ.Ƙ
,StringComparison.OrdinalIgnoreCase)||!Ƭ.Equals(ƥ.ƙ,StringComparison.OrdinalIgnoreCase)||!ƭ.Equals(ƥ.ƚ,StringComparison.
OrdinalIgnoreCase);if(!Ơ&&ƴ){Ƶ();}return true;}string Ʊ(string Ʒ,string Ƹ,string ƹ){string ñ=Ɵ.Get(Ʒ,Ƹ).ToString(ƹ).Trim();return ñ.
Length==0?ƹ:ñ;}void Ư(string ƺ){string[]ƻ=ƺ.Split(new[]{';',','},StringSplitOptions.RemoveEmptyEntries);List<double>Ƽ=new List
<double>();for(int Ā=0;Ā<ƻ.Length;Ā++){double ƽ;if(!double.TryParse(ƻ[Ā].Trim(),out ƽ)){continue;}if(ƽ>0){Ƽ.Add(ƽ/100.0);
}}if(Ƽ.Count==0){return;}ƥ.ƕ.Clear();ƥ.ƕ.AddRange(Ƽ);}void Ʋ(){Ɵ.Set(Ʈ,"Greedy",ƥ.Ɛ);Ɵ.Set(Ʈ,"CanMaster",ƥ.Ƒ);Ɵ.Set(Ʈ,
"CanSlave",ƥ.ƒ);Ɵ.SetSectionComment(Ʈ," Vector Thrust Redux ownership and coordination.\n"+
" Greedy controls eligible mechanical-subgrid blocks unless ignored.\n"+" Main-grid player thrusters and gyros remain read-only unless explicitly tagged.");Ɵ.Set("Parking","ParkOnlyByCommand"
,ƥ.Ɠ);Ɵ.Set("Flight","CruiseLevelsWithGravity",ƥ.Ɣ);Ɵ.Set("Flight","GearPercentages",ƾ());Ɵ.Set("Tags","Use",ƥ.Ɩ);Ɵ.Set(
"Tags","Ignore",ƥ.Ɨ);Ɵ.Set("Tags","Status",ƥ.Ƙ);Ɵ.Set("Tags","ParkTimer",ƥ.ƙ);Ɵ.Set("Tags","UnparkTimer",ƥ.ƚ);Ɵ.SetComment(
"Tags","Use"," Tag may appear in a block name, group name, or block Custom Data.");Ɵ.SetComment("Tags","Ignore",
" Ignore always prevents Redux from modifying the block.");Ɵ.Set("Performance","Update1Skip",ƥ.ƛ);Ɵ.Set("Performance","Update10Skip",ƥ.Ɯ);Ɵ.Set("Performance","Update100Skip",ƥ.Ɲ
);Ɵ.SetSectionComment("Performance"," Number of matching update intervals skipped between executions.\n"+
" Heartbeat publication is never skipped.");}string ƾ(){StringBuilder ƿ=new StringBuilder();for(int Ā=0;Ā<ƥ.ƕ.Count;Ā++){if(Ā>0){ƿ.Append("; ");}ƿ.Append((ƥ.ƕ[Ā]*
100.0).ToString("0.########"));}return ƿ.ToString();}readonly List<ĝ>ǀ=new List<ĝ>();readonly HashSet<F>ǁ=new HashSet<F>();
IMyShipController ǂ;void ǃ(){ƶ(false);Ƶ();}void ǔ(){Ǆ();ǅ();ǆ();Ǉ=ƥ.Ƒ&&ǈ!=null&&ǈ.IsUnderControl;ǉ();if(ƥ.ƒ&&!Ǉ&&!Ǌ){ǋ();}if(ǌ){Ǎ=0;}else
if(ǎ!=long.MinValue){Ǎ++;}Ǐ=ǎ!=long.MinValue&&Ǎ<2;ǌ=false;ǐ();if(Ǒ==Ņ.ń){ǒ();}Ǔ(false);}void ǧ(double Ǖ){Ǆ();if(Ǒ==Ņ.ń||Ǒ
==Ņ.ŀ||ǈ==null){ǖ();Ǘ();return;}ǘ();Ǚ();Vector3D ǚ=ǈ.CenterOfMass;Vector3D Ǜ;if(Ǒ==Ņ.Ń){Ǜ=ǜ.Ŏ*ǝ;Ǜ-=Ǟ();}else{Ǜ=ǟ(Ǖ);if(ǝ>O
){Ǡ=b.Ĭ(Ǜ/ǝ,1.0);}else{Ǡ=Vector3D.Zero;}}ǡ=Ǜ;Ǣ(Ǜ,ǚ);bool Ǥ=Ǒ==Ņ.Ń?ǜ.Ő:ǣ&&ƥ.Ɣ;ǥ(Ǧ,Ǥ);}void ǆ(){for(int Ā=0;Ā<Ǩ.Count;Ā++){
Ǩ[Ā].Ċ();}Ǚ();}void Ǚ(){ǝ=0;for(int Ā=0;Ā<ǩ.Count;Ā++){Y Ă=ǩ[Ā];if(!Ă.U){continue;}ǝ+=Ă.P;}}Vector3D ǟ(double Ǖ){
MyShipMass Ǫ=ǈ.CalculateShipMass();double ǫ=Ǫ.PhysicalMass;if(ǫ<=O){return Vector3D.Zero;}MyShipVelocities Ǭ=ǈ.GetShipVelocities()
;Vector3D ǭ=Ǭ.LinearVelocity;Vector3D Ǯ=ǈ.GetNaturalGravity();Vector3D ǯ=Vector3D.TransformNormal(ǈ.MoveIndicator,ǈ.
WorldMatrix);bool ǰ=ǯ.LengthSquared()>Í;Vector3D Ǳ=b.c(ǯ);double ǲ=ǝ/ǫ;double ǳ=ƥ.ƕ[MathHelper.Clamp(ư,0,ƥ.ƕ.Count-1)];Vector3D Ǵ=Ǳ
*ǲ*ǳ;ǵ=ǈ.DampenersOverride;if(ǵ){Vector3D Ƕ=ǭ;if(ǰ){double Ƿ=Vector3D.Dot(ǭ,Ǳ);if(Ƿ>0){Ƕ-=Ǳ*Ƿ;}}if(ǣ){Vector3D Ǹ=ǈ.
WorldMatrix.Forward;double ǹ=Vector3D.Dot(Ƕ,Ǹ);if(ǹ>0){Ƕ-=Ǹ*ǹ;}}Vector3D ǻ=-Ƕ/Math.Max(Ǖ,Ǻ);ǻ=b.Ĭ(ǻ,ǲ);Ǵ+=ǻ;}Ǵ=b.Ĭ(Ǵ,ǲ);Vector3D Ǽ=
ǫ*(Ǵ-Ǯ);return Ǽ-Ǟ();}Vector3D Ǟ(){Vector3D Đ=Vector3D.Zero;for(int Ā=0;Ā<ǽ.Count;Ā++){Đ+=ǽ[Ā].Q;}return Đ;}void Ǣ(
Vector3D Ǜ,Vector3D ǚ){Ǿ=Ǜ;Ǧ=Vector3D.Zero;for(int Ā=0;Ā<ǩ.Count;Ā++){ǩ[Ā].Z();}ǁ.Clear();for(int Ā=0;Ā<ǿ.Count;Ā++){Ȁ(ǿ[Ā],ref
Ǿ,ǚ,ref Ǧ);}for(int Ā=0;Ā<Ǩ.Count;Ā++){Ǩ[Ā].ē(ref Ǿ,ǚ,ref Ǧ);}ǀ.Clear();for(int Ā=0;Ā<ȁ.Count;Ā++){ǀ.Add(ȁ[Ā]);}while(ǀ.
Count>0&&Ǿ.LengthSquared()>O*O){int Ȃ=-1;double ȃ=O;for(int Ā=0;Ā<ǀ.Count;Ā++){double Ȅ=ǀ[Ā].Ĝ(Ǿ);if(Ȅ<=ȃ){continue;}ȃ=Ȅ;Ȃ=Ā;
}if(Ȃ<0){break;}ĝ ȅ=ǀ[Ȃ];ǀ.RemoveAt(Ȃ);Vector3D Ȇ=ȅ.Ě(Ǿ);if(Ȇ.LengthSquared()<=Í){continue;}for(int Ā=0;Ā<ȅ.ė.Count;Ā++){
F ȇ=ȅ.ė[Ā];ȇ.ċ(Ȇ);ǁ.Add(ȇ);}for(int Ā=0;Ā<ȅ.ė.Count;Ā++){ȅ.ė[Ā].Ē(ref Ǿ,ǚ,ref Ǧ);}}for(int Ā=0;Ā<Ǩ.Count;Ā++){F ȇ=Ǩ[Ā];if
(!ǁ.Contains(ȇ)){ȇ.ċ(Vector3D.Zero);}}for(int Ā=0;Ā<ǩ.Count;Ā++){ǩ[Ā].k();}}void Ȁ(Y Ă,ref Vector3D a,Vector3D ǚ,ref
Vector3D č){double ď=Ă.h(ref a);if(ď<=O){return;}Vector3D Đ=Ă.L*ď;Vector3D đ=Ă.C.GetPosition()-ǚ;č+=Vector3D.Cross(đ,Đ);}void ǥ(
Vector3D č,bool Ǥ){if(Ȉ.Count==0||ǈ==null){return;}double ȉ=0;for(int Ā=0;Ā<Ȉ.Count;Ā++){ȉ+=Ȉ[Ā].ę;}if(ȉ<=Í){Ǘ();return;}
Vector3D Ȋ=-č/ȉ*ĭ;if(Ǥ){Vector3D Ǯ=ǈ.GetNaturalGravity();if(Ǯ.LengthSquared()>Í){Vector3D ȋ=-b.c(Ǯ);Vector3D Ȍ=ǈ.WorldMatrix.Up;
Vector3D ȍ=Vector3D.Cross(Ȍ,ȋ);double Ȏ=MathHelper.Clamp(Vector3D.Dot(Ȍ,ȋ),-1,1);double ȏ=Math.Atan2(ȍ.Length(),Ȏ);if(ȍ.
LengthSquared()>Í){ȍ=b.c(ȍ);Ȋ+=ȍ*ȏ*Ȑ;}Vector3D ȑ=ǈ.GetShipVelocities().AngularVelocity;Vector3D Ȓ=b.Ú(ȑ,ȋ);Ȋ-=Ȓ*ȓ;}}if(Ȋ.
LengthSquared()<=Ĳ*Ĳ){Ǘ();return;}for(int Ā=0;Ā<Ȉ.Count;Ā++){Ȉ[Ā].Ĵ(Ȋ);}}void Ǘ(){for(int Ā=0;Ā<Ȉ.Count;Ā++){Ȉ[Ā].ī();}}void Ǆ(){
IMyShipController Ȕ=null;for(int Ā=0;Ā<ȕ.Count;Ā++){IMyShipController Ȗ=ȕ[Ā];if(Ȗ==null||Ȗ.Closed||!Ȗ.IsFunctional||!Ȗ.CanControlShip){
continue;}if(Ȗ.IsUnderControl){Ȕ=Ȗ;break;}if(Ȕ==null||Ȗ.IsMainCockpit){Ȕ=Ȗ;}}ǈ=Ȕ;ȗ=ǈ==null;Ǉ=ƥ.Ƒ&&ǈ!=null&&ǈ.IsUnderControl;}
void ǐ(){Ǆ();if(Ǒ==Ņ.Ń&&!Ǐ){Ș=ș;}Ņ Ț;if(ȗ||Me.CubeGrid.IsStatic||Ǌ){Ț=Ņ.ń;}else if(ƥ.ƒ&&Ǐ&&!Ǉ){Ț=Ņ.Ń;}else if(ț||Ș){Ț=Ņ.ń;}
else if(Ǉ){Ț=Ņ.ł;}else{Ț=Ņ.Ł;}if(Ț==Ǒ){return;}Ȝ(Ț);}void Ȝ(Ņ ȝ){Ņ Ȟ=Ǒ;if(Ȟ==Ņ.ń&&ȝ!=Ņ.ń){ȟ();}if(Ȟ==Ņ.Ń&&ȝ!=Ņ.Ń&&!Ǐ){Ș=ș;}Ǒ
=ȝ;if(ȝ==Ņ.Ń){ș=Ȟ==Ņ.ń;Ș=false;}if(ȝ==Ņ.ń&&Ȟ!=Ņ.ń){Ƞ();Ý();}ȡ=true;}void ǉ(){if(ƥ.Ɠ){ț=false;return;}bool Ȣ=false;for(int
Ā=0;Ā<ȣ.Count;Ā++){if(Ȥ(ȣ[Ā])){Ȣ=true;break;}}if(!Ȣ){for(int Ā=0;Ā<ȥ.Count;Ā++){if(Ȧ(ȥ[Ā])){Ȣ=true;break;}}}ț=Ȣ;}bool Ȥ(Ǝ
ȧ){IMyShipConnector Ȩ=ȧ.ƌ;if(Ȩ==null||Ȩ.Closed||Ȩ.Status!=MyShipConnectorStatus.Connected){return false;}IMyShipConnector
Œ=Ȩ.OtherConnector;if(Œ==null){return false;}Ÿ ȩ;if(!Ȫ.TryGetValue(Œ.CubeGrid.EntityId,out ȩ)){return Œ.CubeGrid.IsStatic
;}Ŷ ȫ=ȩ.ŷ;if(ȫ==null){return Œ.CubeGrid.IsStatic;}if(ȫ.Ƈ){return true;}if(ȫ.Ƅ.Count==0){return false;}if(Ǉ&&ȫ.ƈ){return
false;}return true;}bool Ȧ(Ə Ȭ){IMyLandingGear ȭ=Ȭ.ƌ;if(ȭ==null||ȭ.Closed||!ȭ.IsFunctional){return false;}return ȭ.IsLocked;}
void Ý(){ǖ();Ǘ();Vector3D Ǯ=ǈ!=null?ǈ.GetNaturalGravity():Vector3D.Zero;Vector3D Ð=Me.CubeGrid.WorldAABB.Center;for(int Ā=0;
Ā<ǩ.Count;Ā++){Y Ă=ǩ[Ā];long Ȯ=Ă.K;if(!ȯ.ContainsKey(Ȯ)){ȯ.Add(Ȯ,Ă.C.Enabled);}Ă.l();Ă.C.Enabled=false;}for(int Ā=0;Ā<Ȱ.
Count;Ā++){Ȱ[Ā].Ý(Ǯ,Ð);}ȱ(Ȳ);}void ȟ(){ǘ();for(int Ā=0;Ā<Ȱ.Count;Ā++){Ȱ[Ā].º();}ȱ(ȳ);}void ȴ(){for(int Ā=0;Ā<ǩ.Count;Ā++){Y Ă
=ǩ[Ā];if(!ȯ.ContainsKey(Ă.K)){ȯ.Add(Ă.K,Ă.C.Enabled);}Ă.l();Ă.C.Enabled=false;}Ǘ();for(int Ā=0;Ā<Ȱ.Count;Ā++){if(Math.Abs
(Ȱ[Ā].C.TargetVelocityRad)>ð){Ȱ[Ā].C.TargetVelocityRad=0;}}}void ǒ(){for(int Ā=0;Ā<Ȱ.Count;Ā++){Ȱ[Ā].â();}}void ǖ(){for(
int Ā=0;Ā<ǩ.Count;Ā++){ǩ[Ā].l();}}void ǘ(){if(ȯ.Count==0){return;}for(int Ā=0;Ā<ǩ.Count;Ā++){Y Ă=ǩ[Ā];bool ȵ;if(!ȯ.
TryGetValue(Ă.K,out ȵ)){continue;}Ă.C.Enabled=ȵ;}ȯ.Clear();}void ȶ(long Ȯ,IMyThrust V){bool ȵ;if(!ȯ.TryGetValue(Ȯ,out ȵ)){return;}
if(V!=null&&!V.Closed){V.Enabled=ȵ;}ȯ.Remove(Ȯ);}bool T(long Ȯ){return ȯ.ContainsKey(Ȯ);}void ȱ(List<IMyTimerBlock>ȷ){for(
int Ā=0;Ā<ȷ.Count;Ā++){IMyTimerBlock ȸ=ȷ[Ā];if(ȸ==null||ȸ.Closed||!ȸ.IsFunctional){continue;}ȸ.Trigger();}}void ȼ(string ȹ)
{if(string.IsNullOrWhiteSpace(ȹ)){return;}string[]Ⱥ=ȹ.ToLowerInvariant().Split(new[]{';','\n','\r'},StringSplitOptions.
RemoveEmptyEntries);for(int Ā=0;Ā<Ⱥ.Length;Ā++){string Ȼ=Ⱥ[Ā].Trim();if(Ȼ=="park"){Ǌ=!Ǌ;Ș=false;}else if(Ȼ=="park on"){Ǌ=true;Ș=false;}
else if(Ȼ=="park off"||Ȼ=="unpark"){Ǌ=false;Ș=false;}else if(Ȼ=="cruise"){ǣ=!ǣ;}else if(Ȼ=="cruise on"){ǣ=true;}else if(Ȼ==
"cruise off"){ǣ=false;}else if(Ȼ=="dampeners"){ǵ=!ǵ;if(ǈ!=null){ǈ.DampenersOverride=ǵ;}}else if(Ȼ=="gear"){ư++;if(ư>=ƥ.ƕ.Count){ư=0;
}}else if(Ȼ=="rescan"){Ƶ();}}Save();}void ǅ(){HashSet<long>Ƚ=new HashSet<long>();for(int Ā=0;Ā<Ⱦ.Count;Ā++){
IMyShipConnector Ȩ=Ⱦ[Ā];if(Ȩ==null||Ȩ.Closed){continue;}long ȿ=Ȩ.OtherConnector!=null?Ȩ.OtherConnector.EntityId:0;long ɀ;if(!Ɂ.
TryGetValue(Ȩ.EntityId,out ɀ)||ɀ!=ȿ){Ɂ[Ȩ.EntityId]=ȿ;Ƶ();}Ƚ.Add(Ȩ.EntityId);}for(int Ā=0;Ā<ȥ.Count;Ā++){IMyLandingGear ɂ=ȥ[Ā].ƌ;if(
ɂ==null||ɂ.Closed){continue;}bool Ƀ;if(!Ʉ.TryGetValue(ɂ.EntityId,out Ƀ)||Ƀ!=ɂ.IsLocked){Ʉ[ɂ.EntityId]=ɂ.IsLocked;Ƶ();}}}
void ɋ(){if(Ǒ!=Ņ.ł||ǈ==null){Ƞ();return;}if(ǂ!=null&&ǂ.EntityId!=ǈ.EntityId){Ʌ(ǂ);}ǂ=ǈ;StringBuilder Ʒ=new StringBuilder();Ʒ
.Append('[').Append(Ɇ).AppendLine("]");Ʒ.Append("Version=").AppendLine(ɇ);Ʒ.Append("MasterProgrammableBlockId=").
AppendLine(Me.EntityId.ToString());Ʒ.Append("ControllerId=").AppendLine(ǈ.EntityId.ToString());Ʒ.Append("Sequence=").AppendLine(Ɉ.
ToString());Ʒ.Append("Demand=").AppendLine(ɉ(Ǡ));Ʒ.Append("Cruise=").AppendLine(ǣ.ToString());Ʒ.Append("LevelWithGravity=").
AppendLine((ǣ&&ƥ.Ɣ).ToString());ǈ.CustomData=Ɋ(ǈ.CustomData,Ɇ,Ʒ.ToString());}void Ƞ(){if(ǂ==null){return;}Ʌ(ǂ);ǂ=null;}void Ʌ(
IMyShipController Ȗ){if(Ȗ==null||Ȗ.Closed){return;}string Ɍ;if(!ɍ(Ȗ.CustomData,Ɇ,"MasterProgrammableBlockId",out Ɍ)){return;}long Ɏ;if(!
long.TryParse(Ɍ,out Ɏ)||Ɏ!=Me.EntityId){return;}Ȗ.CustomData=ɏ(Ȗ.CustomData,Ɇ);}void ǋ(){for(int Ā=0;Ā<ɐ.Count;Ā++){ő Ȼ;if(!
ɑ(ɐ[Ā],out Ȼ)){continue;}ɒ(Ȼ);return;}}void ɓ(){for(int Ā=0;Ā<ɐ.Count;Ā++){IMyShipController Ȗ=ɐ[Ā];if(ǜ.Ō!=0&&Ȗ.EntityId
!=ǜ.Ō){continue;}ő Ȼ;if(!ɑ(Ȗ,out Ȼ)){continue;}if(ǜ.ŋ!=0&&Ȼ.ŋ!=ǜ.ŋ){continue;}ɒ(Ȼ);return;}}void ɒ(ő Ȼ){if(Ȼ.ō!=ǎ||Ȼ.ŋ!=ɔ)
{ǎ=Ȼ.ō;ɔ=Ȼ.ŋ;Ǎ=0;ǌ=true;Ǐ=true;}ǜ.œ(Ȼ);}bool ɑ(IMyShipController Ȗ,out ő Ȼ){Ȼ=null;if(Ȗ==null||Ȗ.Closed){return false;}
string ɕ;string ɖ;string ɗ;string ɘ;string ə;string ɚ;if(!ɍ(Ȗ.CustomData,Ɇ,"MasterProgrammableBlockId",out ɕ)||!ɍ(Ȗ.CustomData
,Ɇ,"ControllerId",out ɖ)||!ɍ(Ȗ.CustomData,Ɇ,"Sequence",out ɗ)||!ɍ(Ȗ.CustomData,Ɇ,"Demand",out ɘ)){return false;}long Ɍ;
long ɛ;long ɜ;Vector3D ɝ;if(!long.TryParse(ɕ,out Ɍ)||!long.TryParse(ɖ,out ɛ)||!long.TryParse(ɗ,out ɜ)||!ɞ(ɘ,out ɝ)){return
false;}ɍ(Ȗ.CustomData,Ɇ,"Cruise",out ə);ɍ(Ȗ.CustomData,Ɇ,"LevelWithGravity",out ɚ);bool ɟ;bool ɠ;bool.TryParse(ə,out ɟ);bool.
TryParse(ɚ,out ɠ);Ȼ=new ő{ŋ=Ɍ,Ō=ɛ,ō=ɜ,Ŏ=b.Ĭ(ɝ,1),ŏ=ɟ,Ő=ɠ};return true;}static int ɨ(string ơ,string ɡ){if(string.IsNullOrEmpty(ơ
)){return-1;}string ɢ="["+ɡ+"]";int ɣ=0;while(ɣ<ơ.Length){int ɤ=ơ.IndexOf(ɢ,ɣ,StringComparison.OrdinalIgnoreCase);if(ɤ<0)
{return-1;}bool ɥ=ɤ==0||ơ[ɤ-1]=='\n';int ɦ=ɤ+ɢ.Length;bool ɧ=ɦ>=ơ.Length||ơ[ɦ]=='\r'||ơ[ɦ]=='\n';if(ɥ&&ɧ){return ɤ;}ɣ=ɤ+1
;}return-1;}static int ɬ(string ơ,int ɣ){while(ɣ<ơ.Length){int ɩ=ơ.IndexOf('\n',ɣ);if(ɩ<0||ɩ+1>=ơ.Length){return ơ.Length
;}ɩ++;int ɪ=ɩ;while(ɪ<ơ.Length&&(ơ[ɪ]==' '||ơ[ɪ]=='\t'||ơ[ɪ]=='\r')){ɪ++;}if(ɪ<ơ.Length&&ơ[ɪ]=='['){int ɫ=ơ.IndexOf(']',ɪ
+1);if(ɫ>=0){return ɩ;}}ɣ=ɩ;}return ơ.Length;}static bool ɍ(string ơ,string ɡ,string Ƹ,out string ñ){ñ=null;int ɭ=ɨ(ơ,ɡ);
if(ɭ<0){return false;}int ɮ=ɬ(ơ,ɭ+ɡ.Length+2);int ɯ=ơ.IndexOf('\n',ɭ);if(ɯ<0||ɯ>=ɮ){return false;}string Ʒ=ơ.Substring(ɯ+1
,ɮ-ɯ-1);string[]ɰ=Ʒ.Replace("\r",string.Empty).Split('\n');for(int Ā=0;Ā<ɰ.Length;Ā++){string ɱ=ɰ[Ā];int ɲ=ɱ.IndexOf('=')
;if(ɲ<=0){continue;}string ɳ=ɱ.Substring(0,ɲ).Trim();if(!ɳ.Equals(Ƹ,StringComparison.OrdinalIgnoreCase)){continue;}ñ=ɱ.
Substring(ɲ+1).Trim();return true;}return false;}static string Ɋ(string ơ,string ɡ,string ɴ){ơ=ơ??string.Empty;ɴ=ɴ.TrimEnd('\r',
'\n')+"\n";int ɭ=ɨ(ơ,ɡ);if(ɭ<0){if(ơ.Length==0){return ɴ;}string ɲ=ơ.EndsWith("\n")?string.Empty:"\n";return ơ+ɲ+ɴ;}int ɮ=ɬ(
ơ,ɭ+ɡ.Length+2);return ơ.Substring(0,ɭ)+ɴ+ơ.Substring(ɮ);}static string ɏ(string ơ,string ɡ){if(string.IsNullOrEmpty(ơ)){
return ơ;}int ɭ=ɨ(ơ,ɡ);if(ɭ<0){return ơ;}int ɮ=ɬ(ơ,ɭ+ɡ.Length+2);string ɵ=ơ.Substring(0,ɭ);string ɦ=ơ.Substring(ɮ);if(ɵ.
EndsWith("\n")&&ɦ.StartsWith("\n")){ɦ=ɦ.Substring(1);}return ɵ+ɦ;}static string ɉ(Vector3D ş){return ş.X.ToString("R")+";"+ş.Y.
ToString("R")+";"+ş.Z.ToString("R");}static bool ɞ(string ɶ,out Vector3D ş){ş=Vector3D.Zero;if(string.IsNullOrWhiteSpace(ɶ)){
return false;}string[]ɷ=ɶ.Split(';');if(ɷ.Length!=3){return false;}double ɸ;double ɹ;double ɺ;if(!double.TryParse(ɷ[0],out ɸ)
||!double.TryParse(ɷ[1],out ɹ)||!double.TryParse(ɷ[2],out ɺ)){return false;}ş=new Vector3D(ɸ,ɹ,ɺ);return true;}void Ǔ(bool
Đ){ɻ.Clear();ɻ.AppendLine(Ƥ).Append("v").AppendLine(ɇ).AppendLine();ɻ.Append("Mode: ").AppendLine(Ǒ.ToString());ɻ.Append(
"Controller: ").AppendLine(ǈ!=null?ǈ.CustomName:"NONE");ɻ.Append("Dampeners: ").AppendLine(ǵ?"ON":"OFF");ɻ.Append("Cruise: ").
AppendLine(ǣ?"ON":"OFF");ɻ.Append("Gear: ").Append(ư+1).Append("/").Append(ƥ.ƕ.Count).Append(" (").Append((ƥ.ƕ[MathHelper.Clamp(ư,
0,ƥ.ƕ.Count-1)]*100).ToString("0.##")).AppendLine("%)");ɻ.Append("Nacelles: ").AppendLine(Ǩ.Count.ToString());ɻ.Append(
"Controlled thrust: ").Append((ǝ/1000.0).ToString("0.##")).AppendLine(" kN");ɻ.Append("Residual: ").Append((Ǿ.Length()/1000.0).ToString(
"0.##")).AppendLine(" kN");ɻ.Append("Gyros: ").AppendLine(Ȉ.Count.ToString());if(Ǒ==Ņ.Ń){ɻ.Append("Heartbeat age: ").Append(Ǎ)
.AppendLine("/2");}ɻ.Append("Runtime: ").Append(Runtime.LastRunTimeMs.ToString("0.###")).Append(" ms | avg ").Append(ɼ.Ř.
ToString("0.###")).Append(" | max ").AppendLine(ɼ.ř.ToString("0.###"));ɻ.Append("Instructions: ").Append(Runtime.
CurrentInstructionCount).Append("/").AppendLine(Runtime.MaxInstructionCount.ToString());Echo(ɻ.ToString());if(!Đ&&ɽ.Count==0){return;}ɾ.Clear()
;ɾ.AppendLine("VECTOR THRUST REDUX").Append("MODE  ").AppendLine(Ǒ.ToString().ToUpperInvariant()).Append("DAMP  ").
AppendLine(ǵ?"ON":"OFF").Append("CRUISE ").AppendLine(ǣ?"ON":"OFF").Append("GEAR  ").Append(ư+1).Append("/").AppendLine(ƥ.ƕ.Count.
ToString()).Append("VECTORS ").AppendLine(Ǩ.Count.ToString()).Append("THRUST ").Append((ǝ/1000.0).ToString("0.0")).AppendLine(
" kN").Append("ERROR ").Append((Ǿ.Length()/1000.0).ToString("0.0")).AppendLine(" kN");for(int Ā=0;Ā<ɽ.Count;Ā++){ɽ[Ā].Ŀ(ɾ.
ToString());}}const string Ƥ="Vector Thrust Redux",ɇ="0.1.0",Ʈ="Vector Thrust Redux",Ɇ="Vector Thrust Redux Heartbeat",ɿ=
"VT-Redux:";const double Í=1e-8,O=1e-3,á=1e-4,x=1e-4,Ĉ=1.0-1e-6,ʀ=1.0-1e-4,Ò=1.0-1e-4,Æ=4.0,ï=Math.PI,ð=1e-3,Ȑ=4.0,ȓ=1.5,ĭ=30.0,Ĳ=
1e-3,ħ=448000.0,Ĩ=33600000.0,ĥ=4480000.0,Ħ=201600000.0,Ǻ=1.0/120.0,ʁ=0.25;readonly ƞ ƥ=new ƞ();readonly MyIni ʂ=new MyIni();
string Ƣ=string.Empty;bool ǣ,ț,ȡ;bool ǵ=true,Ǌ,ȗ=true,Ǉ,Ǐ,Ș,ș,ǌ,ʃ=true;int ư,Ǎ,ʄ,ʅ;Ņ Ǒ=Ņ.ŀ;IMyShipController ǈ;long Ɉ;long ǎ=
long.MinValue,ɔ;ő ǜ=new ő();Vector3D ǡ;Vector3D Ǿ,Ǡ,Ǧ;double ǝ;double ʆ,ʇ;int ʈ;readonly List<IMyShipController>ȕ=new List<
IMyShipController>(),ɐ=new List<IMyShipController>();readonly List<Y>ʉ=new List<Y>(),ǩ=new List<Y>(),ǿ=new List<Y>(),ǽ=new List<Y>();
readonly List<z>Ȱ=new List<z>();readonly List<F>Ǩ=new List<F>();readonly List<ĝ>ȁ=new List<ĝ>();readonly List<ĩ>Ȉ=new List<ĩ>();
readonly List<Ǝ>ȣ=new List<Ǝ>();readonly List<Ə>ȥ=new List<Ə>();readonly List<IMyTimerBlock>Ȳ=new List<IMyTimerBlock>(),ȳ=new
List<IMyTimerBlock>();readonly List<Ľ>ɽ=new List<Ľ>();readonly Dictionary<long,bool>ȯ=new Dictionary<long,bool>();readonly
Dictionary<long,Ÿ>Ȫ=new Dictionary<long,Ÿ>();readonly StringBuilder ɻ=new StringBuilder(),ɾ=new StringBuilder();IEnumerator<int>ʊ;
ś ɼ;public
 Program
(){ɼ=new ś(this);ʋ();ƶ(true);Runtime.UpdateFrequency=UpdateFrequency.Update1|UpdateFrequency.Update10|UpdateFrequency.
Update100;Ƶ();}public void
 Save
(){ʂ.Clear();ʂ.Set("State","Cruise",ǣ);ʂ.Set("State","Dampeners",ǵ);ʂ.Set("State","ManualPark",Ǌ);ʂ.Set("State","Gear",ư)
;Storage=ʂ.ToString();}public void
 Main
(string ȹ,UpdateType ʌ){ɼ.Ŝ();double ʍ=Runtime.TimeSinceLastRun.TotalSeconds;if(ʍ<Ǻ){ʍ=Ǻ;}else if(ʍ>ʁ){ʍ=ʁ;}ʇ+=ʍ;bool ʎ=(
ʌ&(UpdateType.Terminal|UpdateType.Trigger|UpdateType.Script))!=0||!string.IsNullOrWhiteSpace(ȹ);if(ʎ){ȼ(ȹ);}ʏ();if((ʌ&
UpdateType.Update100)!=0&&ʐ(ref ʅ,ƥ.Ɲ)){ǃ();}if((ʌ&UpdateType.Update10)!=0&&ʐ(ref ʄ,ƥ.Ɯ)){ǔ();}if((ʌ&UpdateType.Update1)!=0){Ɉ++;
if(Ǒ==Ņ.Ń){ɓ();}ǐ();if(ʐ(ref ʈ,ƥ.ƛ)){ʆ=MathHelper.Clamp(ʇ,Ǻ,ʁ);ʇ=0;ǧ(ʆ);}ɋ();}if(ʎ){ǐ();ɋ();ȡ=true;}if(ȡ){Ǔ(true);ȡ=false;
}ɼ.Ş();}static bool ʐ(ref int ʑ,int ʒ){if(ʑ<ʒ){ʑ++;return false;}ʑ=0;return true;}void ʋ(){if(string.IsNullOrWhiteSpace(
Storage)){return;}MyIniParseResult ƣ;if(!ʂ.TryParse(Storage,out ƣ)){return;}ǣ=ʂ.Get("State","Cruise").ToBoolean(false);ǵ=ʂ.Get(
"State","Dampeners").ToBoolean(true);Ǌ=ʂ.Get("State","ManualPark").ToBoolean(false);ư=Math.Max(0,ʂ.Get("State","Gear").ToInt32(
0));}sealed class ʯ{public readonly List<IMyTerminalBlock>ʓ=new List<IMyTerminalBlock>();public readonly List<
IMyShipController>Ƅ=new List<IMyShipController>(),ʔ=new List<IMyShipController>(),ʕ=new List<IMyShipController>();public readonly List<
IMyThrust>ʖ=new List<IMyThrust>();public readonly List<IMyMotorStator>ʗ=new List<IMyMotorStator>();public readonly List<
IMyPistonBase>ʘ=new List<IMyPistonBase>();public readonly List<IMyGyro>ʙ=new List<IMyGyro>();public readonly List<IMyShipConnector>ʚ=
new List<IMyShipConnector>(),ʛ=new List<IMyShipConnector>();public readonly List<IMyLandingGear>ʜ=new List<IMyLandingGear>(
);public readonly List<IMyTimerBlock>ʝ=new List<IMyTimerBlock>(),ʞ=new List<IMyTimerBlock>(),ʟ=new List<IMyTimerBlock>();
public readonly List<IMyProgrammableBlock>ʠ=new List<IMyProgrammableBlock>();public readonly Dictionary<long,D>E=new
Dictionary<long,D>();public readonly Dictionary<long,Ÿ>ʡ=new Dictionary<long,Ÿ>();public readonly List<Ŷ>ʢ=new List<Ŷ>();public
readonly List<Ƌ>ʣ=new List<Ƌ>();public readonly List<Y>ö=new List<Y>(),ʤ=new List<Y>(),ʥ=new List<Y>(),ʦ=new List<Y>();public
readonly List<z>ʧ=new List<z>();public readonly List<F>ʨ=new List<F>();public readonly List<ĝ>ʩ=new List<ĝ>();public readonly
List<ĩ>ʪ=new List<ĩ>();public readonly List<Ǝ>ʫ=new List<Ǝ>();public readonly List<Ə>ʬ=new List<Ə>();public readonly List<Ľ>
ʭ=new List<Ľ>();public Ŷ ʮ;}readonly List<Ƌ>ʰ=new List<Ƌ>();readonly List<IMyShipConnector>Ⱦ=new List<IMyShipConnector>()
;readonly Dictionary<long,long>Ɂ=new Dictionary<long,long>();readonly Dictionary<long,bool>Ʉ=new Dictionary<long,bool>();
void Ƶ(){ʃ=true;}void ʏ(){if(ʊ==null){if(!ʃ){return;}ʃ=false;ʊ=ʱ().GetEnumerator();}int ʲ=Runtime.MaxInstructionCount;int ʳ=
Math.Max(1000,ʲ*3/4);int ʴ=0;while(ʊ!=null&&Runtime.CurrentInstructionCount<ʳ&&ʴ<512){ʴ++;if(ʊ.MoveNext()){continue;}ʊ.
Dispose();ʊ=null;if(ʃ){ʃ=false;ʊ=ʱ().GetEnumerator();}}}IEnumerable<int>ʱ(){ʯ ʵ=new ʯ();GridTerminalSystem.GetBlocks(ʵ.ʓ);List<
IMyBlockGroup>ʶ=new List<IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(ʶ);List<IMyTerminalBlock>ʷ=new List<IMyTerminalBlock>();
for(int Ā=0;Ā<ʶ.Count;Ā++){IMyBlockGroup ȅ=ʶ[Ā];D ʹ=ʸ(ȅ.Name);if(ʹ==D.ņ){continue;}ʷ.Clear();ȅ.GetBlocks(ʷ);for(int ć=0;ć<ʷ
.Count;ć++){ʺ(ʵ.E,ʷ[ć].EntityId,ʹ);}yield return 1;}for(int Ā=0;Ā<ʵ.ʓ.Count;Ā++){IMyTerminalBlock V=ʵ.ʓ[Ā];D ʻ=ʸ(V.
CustomName)|ʸ(V.CustomData);ʺ(ʵ.E,V.EntityId,ʻ);ʼ(ʵ.ʡ,V.CubeGrid);IMyShipController Ȗ=V as IMyShipController;if(Ȗ!=null){ʵ.Ƅ.Add(Ȗ
);}IMyThrust N=V as IMyThrust;if(N!=null){ʵ.ʖ.Add(N);}IMyMotorStator ü=V as IMyMotorStator;if(ü!=null){ʵ.ʗ.Add(ü);}
IMyPistonBase ʽ=V as IMyPistonBase;if(ʽ!=null){ʵ.ʘ.Add(ʽ);}IMyGyro ʾ=V as IMyGyro;if(ʾ!=null){ʵ.ʙ.Add(ʾ);}IMyShipConnector Ȩ=V as
IMyShipConnector;if(Ȩ!=null){ʵ.ʚ.Add(Ȩ);}IMyLandingGear ȭ=V as IMyLandingGear;if(ȭ!=null){ʵ.ʜ.Add(ȭ);}IMyTimerBlock ȸ=V as IMyTimerBlock
;if(ȸ!=null){ʵ.ʝ.Add(ȸ);}IMyProgrammableBlock ʿ=V as IMyProgrammableBlock;if(ʿ!=null){ʵ.ʠ.Add(ʿ);}yield return 1;}Ÿ ˀ=ʼ(ʵ
.ʡ,Me.CubeGrid);for(int Ā=0;Ā<ʵ.ʗ.Count;Ā++){IMyMotorStator ü=ʵ.ʗ[Ā];if(ü.TopGrid==null){continue;}ˁ(ʵ.ʡ,ü.CubeGrid,ü.
TopGrid,ü);yield return 1;}for(int Ā=0;Ā<ʵ.ʘ.Count;Ā++){IMyPistonBase ʽ=ʵ.ʘ[Ā];if(ʽ.TopGrid==null){continue;}ˁ(ʵ.ʡ,ʽ.CubeGrid,ʽ
.TopGrid,ʽ);yield return 1;}for(int Ā=0;Ā<ʵ.ʚ.Count;Ā++){IMyShipConnector Œ=ʵ.ʚ[Ā].OtherConnector;if(Œ!=null){ʼ(ʵ.ʡ,Œ.
CubeGrid);}yield return 1;}ˆ(ʵ);for(int Ā=0;Ā<ʵ.Ƅ.Count;Ā++){IMyShipController Ȗ=ʵ.Ƅ[Ā];Ÿ Ɓ;if(!ʵ.ʡ.TryGetValue(Ȗ.CubeGrid.
EntityId,out Ɓ)){continue;}Ɓ.ŷ.Ƅ.Add(Ȗ);if(Ȗ.CubeGrid==Me.CubeGrid){ʵ.ʔ.Add(Ȗ);}yield return 1;}for(int Ā=0;Ā<ʵ.ʠ.Count;Ā++){
IMyProgrammableBlock ʿ=ʵ.ʠ[Ā];if(!ˇ(ʿ)){continue;}Ÿ Ɓ;if(!ʵ.ʡ.TryGetValue(ʿ.CubeGrid.EntityId,out Ɓ)){continue;}Ɓ.ŷ.ƅ.Add(ʿ);if(ˈ(ʿ)){Ɓ.ŷ.ƈ=
true;}yield return 1;}HashSet<long>ˉ=new HashSet<long>();for(int Ā=0;Ā<ʵ.ʚ.Count;Ā++){IMyShipConnector Ȩ=ʵ.ʚ[Ā];
IMyShipConnector Œ=Ȩ.OtherConnector;if(Œ==null){continue;}long ˊ=Math.Min(Ȩ.EntityId,Œ.EntityId);long ˋ=Math.Max(Ȩ.EntityId,Œ.EntityId);
long ˌ=unchecked(ˊ*397L^ˋ);if(!ˉ.Add(ˌ)){continue;}Ÿ ˍ;Ÿ ˎ;if(!ʵ.ʡ.TryGetValue(Ȩ.CubeGrid.EntityId,out ˍ)||!ʵ.ʡ.TryGetValue(
Œ.CubeGrid.EntityId,out ˎ)){continue;}ʵ.ʣ.Add(new Ƌ{Ž=Ȩ,ž=Œ,Ɖ=ˍ,Ɗ=ˎ});yield return 1;}ʵ.ʮ=ˀ.ŷ;ˏ(ʵ);ː(ʵ,ˀ);for(int Ā=0;Ā<ʵ
.ʢ.Count;Ā++){Ŷ ˑ=ʵ.ʢ[Ā];if(!ˑ.Ɔ||ˑ.ż||ˑ.ƅ.Count==0){continue;}for(int ć=0;ć<ˑ.Ƅ.Count;ć++){ʵ.ʕ.Add(ˑ.Ƅ[ć]);}yield return
1;}Dictionary<long,z>ˠ=new Dictionary<long,z>();for(int Ā=0;Ā<ʵ.ʗ.Count;Ā++){IMyMotorStator V=ʵ.ʗ[Ā];Ÿ Ɓ;if(!ʵ.ʡ.
TryGetValue(V.CubeGrid.EntityId,out Ɓ)||!Ɓ.ż){continue;}D W=ˡ(ʵ.E,V.EntityId);bool X=ˢ(W);z ü=new z(V,this,W,X);ˠ.Add(V.EntityId,ü)
;if(X){ʵ.ʧ.Add(ü);}yield return 1;}Dictionary<long,F>ˣ=new Dictionary<long,F>();for(int Ā=0;Ā<ʵ.ʖ.Count;Ā++){IMyThrust V=
ʵ.ʖ[Ā];Ÿ Ɓ;if(!ʵ.ʡ.TryGetValue(V.CubeGrid.EntityId,out Ɓ)){continue;}bool ˤ=Ɓ.ż||Ɓ.ŷ.Ɔ;if(!ˤ){continue;}D W=ˡ(ʵ.E,V.
EntityId);z ˮ=ˬ(Ɓ,ˠ);bool Ͱ=Ɓ.ż&&Ɓ.Ż>0;bool X=ͱ(W,Ɓ.ż,Ͱ,ˮ);Y Ă=new Y(V,this,W,X);ʵ.ö.Add(Ă);if(!X){ʵ.ʦ.Add(Ă);yield return 1;
continue;}ʵ.ʤ.Add(Ă);if(ˮ==null||!ˮ.H){ʵ.ʥ.Add(Ă);yield return 1;continue;}F ȇ;if(!ˣ.TryGetValue(ˮ.K,out ȇ)){ȇ=new F(ˮ,this);ˣ.
Add(ˮ.K,ȇ);ʵ.ʨ.Add(ȇ);}Ă.G=ȇ;ȇ.ö.Add(Ă);Ͳ(ȇ,Ɓ,ˮ);yield return 1;}for(int Ā=0;Ā<ʵ.ʧ.Count;Ā++){z ü=ʵ.ʧ[Ā];if(ü.G==null&&Math
.Abs(ü.C.TargetVelocityRad)>ð){ü.C.TargetVelocityRad=0;}yield return 1;}for(int Ā=0;Ā<ʵ.ʨ.Count;Ā++){ʵ.ʨ[Ā].Ċ();ͳ(ʵ.ʩ,ʵ.ʨ
[Ā]);yield return 1;}for(int Ā=0;Ā<ʵ.ʙ.Count;Ā++){IMyGyro V=ʵ.ʙ[Ā];Ÿ Ɓ;if(!ʵ.ʡ.TryGetValue(V.CubeGrid.EntityId,out Ɓ)||!Ɓ
.ż||!ʹ(V)){continue;}D W=ˡ(ʵ.E,V.EntityId);bool X=Ͷ(W,Ɓ.Ż>0);if(!X){continue;}ʵ.ʪ.Add(new ĩ(V,this,W,true));yield return
1;}for(int Ā=0;Ā<ʵ.ʚ.Count;Ā++){IMyShipConnector V=ʵ.ʚ[Ā];Ÿ Ɓ;if(!ʵ.ʡ.TryGetValue(V.CubeGrid.EntityId,out Ɓ)||!Ɓ.ż){
continue;}ʵ.ʛ.Add(V);D W=ˡ(ʵ.E,V.EntityId);if(!ͷ(W)){continue;}ʵ.ʫ.Add(new Ǝ{ƌ=V,ƍ=ͺ(ʵ.ʣ,V)});yield return 1;}for(int Ā=0;Ā<ʵ.ʜ.
Count;Ā++){IMyLandingGear V=ʵ.ʜ[Ā];Ÿ Ɓ;if(!ʵ.ʡ.TryGetValue(V.CubeGrid.EntityId,out Ɓ)||!Ɓ.ż){continue;}D W=ˡ(ʵ.E,V.EntityId);
if(!ͷ(W)){continue;}ʵ.ʬ.Add(new Ə{ƌ=V});yield return 1;}for(int Ā=0;Ā<ʵ.ʝ.Count;Ā++){IMyTimerBlock ȸ=ʵ.ʝ[Ā];if(ȸ.CubeGrid
!=Me.CubeGrid){continue;}D W=ˡ(ʵ.E,ȸ.EntityId);if((W&D.ŉ)!=0){ʵ.ʞ.Add(ȸ);}if((W&D.Ŋ)!=0){ʵ.ʟ.Add(ȸ);}yield return 1;}ͻ(ʵ);
ͼ(ʵ);yield return 1;}void ˆ(ʯ ʵ){List<Ÿ>ͽ=new List<Ÿ>();foreach(KeyValuePair<long,Ÿ>Ά in ʵ.ʡ){Ÿ Έ=Ά.Value;if(Έ.ŷ!=null){
continue;}Ŷ ˑ=new Ŷ();ʵ.ʢ.Add(ˑ);ͽ.Clear();ͽ.Add(Έ);Έ.ŷ=ˑ;for(int ɤ=0;ɤ<ͽ.Count;ɤ++){Ÿ Ɓ=ͽ[ɤ];ˑ.ƃ.Add(Ɓ);if(Ɓ.ų.IsStatic){ˑ.Ƈ=
true;}for(int Ή=0;Ή<Ɓ.ŵ.Count;Ή++){Ÿ Ί=Ɓ.ŵ[Ή].Ƃ(Ɓ);if(Ί.ŷ!=null){continue;}Ί.ŷ=ˑ;ͽ.Add(Ί);}}}}void ˏ(ʯ ʵ){Ŷ Ό=ʵ.ʮ;if(Ό==null
){return;}List<Ŷ>ͽ=new List<Ŷ>();Ό.Ɔ=true;ͽ.Add(Ό);for(int ɤ=0;ɤ<ͽ.Count;ɤ++){Ŷ ˑ=ͽ[ɤ];for(int Ā=0;Ā<ʵ.ʣ.Count;Ā++){Ƌ Ύ=ʵ
.ʣ[Ā];Ŷ Œ=null;if(Ύ.Ɖ.ŷ==ˑ){Œ=Ύ.Ɗ.ŷ;}else if(Ύ.Ɗ.ŷ==ˑ){Œ=Ύ.Ɖ.ŷ;}if(Œ==null||Œ.Ɔ){continue;}Œ.Ɔ=true;ͽ.Add(Œ);}}}void ː(ʯ
ʵ,Ÿ ˀ){Ŷ Ό=ʵ.ʮ;if(Ό==null){return;}Ό.ż=true;Ώ(Ό,ˀ,0);bool ΐ;do{ΐ=false;for(int Ā=0;Ā<ʵ.ʣ.Count;Ā++){Ƌ Ύ=ʵ.ʣ[Ā];bool Α=Ύ.Ɖ
.ŷ.ż;bool Β=Ύ.Ɗ.ŷ.ż;if(Α==Β){continue;}Ÿ Γ=Α?Ύ.Ɖ:Ύ.Ɗ;Ÿ ȫ=Α?Ύ.Ɗ:Ύ.Ɖ;Ŷ Δ=ȫ.ŷ;if(Δ.ƅ.Count>0){continue;}if(!ƥ.Ƒ){continue;}Δ
.ż=true;int Ε=Γ.Ż==int.MaxValue?0:Γ.Ż;Ώ(Δ,ȫ,Ε);ΐ=true;}}while(ΐ);foreach(KeyValuePair<long,Ÿ>Ά in ʵ.ʡ){Ά.Value.ż=Ά.Value.
ŷ.ż;}}void Ώ(Ŷ ˑ,Ÿ ɭ,int Ζ){List<Ÿ>ͽ=new List<Ÿ>();if(ɭ.Ż>Ζ){ɭ.Ż=Ζ;ɭ.Ź=null;ɭ.ź=null;}ͽ.Add(ɭ);for(int ɤ=0;ɤ<ͽ.Count;ɤ++)
{Ÿ Ɓ=ͽ[ɤ];for(int Ā=0;Ā<Ɓ.ŵ.Count;Ā++){Ŵ Ύ=Ɓ.ŵ[Ā];Ÿ Ί=Ύ.Ƃ(Ɓ);if(Ί.ŷ!=ˑ){continue;}int Η=Ɓ.Ż+1;if(Η>=Ί.Ż){continue;}Ί.Ż=Η;
Ί.Ź=Ɓ;Ί.ź=Ύ;ͽ.Add(Ί);}}}z ˬ(Ÿ Ɓ,Dictionary<long,z>ˠ){Ÿ Θ=Ɓ;while(Θ!=null&&Θ.Ź!=null){Ŵ Ύ=Θ.ź;IMyMotorStator Ι=Ύ!=null?Ύ.ſ
as IMyMotorStator:null;if(Ι!=null&&Ι.TopGrid==Θ.ų){z ü;if(ˠ.TryGetValue(Ι.EntityId,out ü)&&ü.y){return ü.H?ü:null;}}Θ=Θ.Ź;
}return null;}void Ͳ(F ȇ,Ÿ Κ,z ü){Ÿ Θ=Κ;while(Θ!=null){Λ(ȇ.ø,Θ.ų);if(Θ.ź!=null&&Θ.ź.ſ.EntityId==ü.K){break;}Θ=Θ.Ź;}}void
ͳ(List<ĝ>ʶ,F ȇ){Vector3D Ũ=b.c(ȇ.s);for(int Ā=0;Ā<ʶ.Count;Ā++){Vector3D Μ=ʶ[Ā].s;if(Math.Abs(Vector3D.Dot(Ũ,Μ))<ʀ){
continue;}ʶ[Ā].ė.Add(ȇ);return;}ĝ ȅ=new ĝ();ȅ.ė.Add(ȇ);ʶ.Add(ȅ);}void ͻ(ʯ ʵ){HashSet<string>Ν=new HashSet<string>(StringComparer
.Ordinal);List<int>Ξ=new List<int>();for(int Ā=0;Ā<ʵ.ʓ.Count;Ā++){IMyTerminalBlock V=ʵ.ʓ[Ā];Ÿ Ɓ;if(!ʵ.ʡ.TryGetValue(V.
CubeGrid.EntityId,out Ɓ)||Ɓ.ŷ!=ʵ.ʮ){continue;}D W=ˡ(ʵ.E,V.EntityId);IMyTextPanel Ο=V as IMyTextPanel;if(Ο!=null&&(W&D.ň)!=0){Π(ʵ
.ʭ,Ν,V,Ο,0);}IMyTextSurfaceProvider Ρ=V as IMyTextSurfaceProvider;if(Ρ==null||Ρ.SurfaceCount<=0){continue;}Ξ.Clear();Σ(V.
CustomData,Ρ.SurfaceCount,Ξ);if((W&D.ň)!=0&&Ξ.Count==0){Ξ.Add(0);}for(int ɤ=0;ɤ<Ξ.Count;ɤ++){int ļ=Ξ[ɤ];Π(ʵ.ʭ,Ν,V,Ρ.GetSurface(ļ),
ļ);}}}void ͼ(ʯ ʵ){HashSet<long>Τ=new HashSet<long>();HashSet<long>Υ=new HashSet<long>();HashSet<long>Φ=new HashSet<long>(
);for(int Ā=0;Ā<ʵ.ʤ.Count;Ā++){Τ.Add(ʵ.ʤ[Ā].K);}for(int Ā=0;Ā<ʵ.ʧ.Count;Ā++){Υ.Add(ʵ.ʧ[Ā].K);}for(int Ā=0;Ā<ʵ.ʪ.Count;Ā++
){Φ.Add(ʵ.ʪ[Ā].C.EntityId);}for(int Ā=0;Ā<ǩ.Count;Ā++){Y Χ=ǩ[Ā];if(!Τ.Contains(Χ.K)){Χ.m();ȶ(Χ.K,Χ.C);}}for(int Ā=0;Ā<Ȱ.
Count;Ā++){z Ψ=Ȱ[Ā];if(!Υ.Contains(Ψ.K)){Ψ.m();}}for(int Ā=0;Ā<Ȉ.Count;Ā++){ĩ Ω=Ȉ[Ā];if(!Φ.Contains(Ω.C.EntityId)){Ω.m();}}Ϊ(
ȕ,ʵ.ʔ);Ϊ(ɐ,ʵ.ʕ);Ϊ(ʉ,ʵ.ö);Ϊ(ǩ,ʵ.ʤ);Ϊ(ǿ,ʵ.ʥ);Ϊ(ǽ,ʵ.ʦ);Ϊ(Ȱ,ʵ.ʧ);Ϊ(Ǩ,ʵ.ʨ);Ϊ(ȁ,ʵ.ʩ);Ϊ(Ȉ,ʵ.ʪ);Ϊ(ȣ,ʵ.ʫ);Ϊ(ȥ,ʵ.ʬ);Ϊ(ʰ,ʵ.ʣ);Ϊ(Ⱦ,ʵ.
ʛ);Ϊ(Ȳ,ʵ.ʞ);Ϊ(ȳ,ʵ.ʟ);Ϊ(ɽ,ʵ.ʭ);Ȫ.Clear();foreach(KeyValuePair<long,Ÿ>Ά in ʵ.ʡ){Ȫ.Add(Ά.Key,Ά.Value);}Ǆ();if(Ǒ==Ņ.ń){ȴ();}ȡ
=true;}D ʸ(string ľ){if(string.IsNullOrEmpty(ľ)){return D.ņ;}D Ϋ=D.ņ;if(ά(ľ,ƥ.Ɩ)){Ϋ|=D.Ň;}if(ά(ľ,ƥ.Ɨ)){Ϋ|=D.R;}if(ά(ľ,ƥ.Ƙ
)){Ϋ|=D.ň;}if(ά(ľ,ƥ.ƙ)){Ϋ|=D.ŉ;}if(ά(ľ,ƥ.ƚ)){Ϋ|=D.Ŋ;}return Ϋ;}bool ˢ(D W){if((W&D.R)!=0){return false;}return ƥ.Ɛ||(W&D.
Ň)!=0;}bool ͱ(D W,bool έ,bool Ͱ,z ˮ){if(!έ||(W&D.R)!=0){return false;}bool ή=(W&D.Ň)!=0;if(!ƥ.Ɛ){return ή;}return ή||Ͱ||ˮ
!=null;}bool Ͷ(D W,bool Ͱ){if((W&D.R)!=0){return false;}bool ή=(W&D.Ň)!=0;if(!ƥ.Ɛ){return ή;}return ή||Ͱ;}bool ͷ(D W){if((
W&D.R)!=0){return false;}return ƥ.Ɛ||(W&D.Ň)!=0;}bool ʹ(IMyGyro ʾ){string ί=ʾ.BlockDefinition.SubtypeId;if(ί.Equals(
"SmallBlockGyro",StringComparison.OrdinalIgnoreCase)||ί.Equals("LargeBlockGyro",StringComparison.OrdinalIgnoreCase)){return true;}return
ί.Equals("SmallPrototechGyro",StringComparison.OrdinalIgnoreCase)||ί.Equals("LargePrototechGyro",StringComparison.
OrdinalIgnoreCase)||ί.Equals("SmallPrototechGyroscope",StringComparison.OrdinalIgnoreCase)||ί.Equals("LargePrototechGyroscope",
StringComparison.OrdinalIgnoreCase);}bool ˇ(IMyProgrammableBlock ʿ){if(ʿ==null){return false;}return ɨ(ʿ.CustomData,Ʈ)>=0;}bool ˈ(
IMyProgrammableBlock ʿ){string ɶ;if(!ɍ(ʿ.CustomData,Ʈ,"CanSlave",out ɶ)){return true;}bool ñ;return bool.TryParse(ɶ,out ñ)?ñ:true;}void Σ(
string ơ,int ΰ,List<int>α){if(string.IsNullOrEmpty(ơ)){return;}string[]ɰ=ơ.Replace("\r",string.Empty).Split('\n');for(int Ā=0;
Ā<ɰ.Length;Ā++){string ɱ=ɰ[Ā].Trim();if(!ɱ.StartsWith(ɿ,StringComparison.OrdinalIgnoreCase)){continue;}string β=ɱ.
Substring(ɿ.Length).Trim();int ɤ;if(!int.TryParse(β,out ɤ)||ɤ<0||ɤ>=ΰ||α.Contains(ɤ)){continue;}α.Add(ɤ);}}static void Π(List<Ľ>α
,HashSet<string>g,IMyTerminalBlock ĺ,IMyTextSurface Ļ,int ļ){string Ƹ=ĺ.EntityId+":"+ļ;if(!g.Add(Ƹ)){return;}α.Add(new Ľ(
ĺ,Ļ,ļ));}static void Λ(List<IMyCubeGrid>γ,IMyCubeGrid Ė){for(int Ā=0;Ā<γ.Count;Ā++){if(γ[Ā].EntityId==Ė.EntityId){return;
}}γ.Add(Ė);}static Ƌ ͺ(List<Ƌ>δ,IMyShipConnector Ȩ){for(int Ā=0;Ā<δ.Count;Ā++){if(δ[Ā].Ž.EntityId==Ȩ.EntityId||δ[Ā].ž.
EntityId==Ȩ.EntityId){return δ[Ā];}}return null;}static Ÿ ʼ(Dictionary<long,Ÿ>ε,IMyCubeGrid Ė){Ÿ Ɓ;if(!ε.TryGetValue(Ė.EntityId,
out Ɓ)){Ɓ=new Ÿ(Ė);ε.Add(Ė.EntityId,Ɓ);}return Ɓ;}static void ˁ(Dictionary<long,Ÿ>ε,IMyCubeGrid ζ,IMyCubeGrid η,
IMyTerminalBlock ƀ){Ÿ Š=ʼ(ε,ζ);Ÿ š=ʼ(ε,η);Ŵ Ύ=new Ŵ(Š,š,ƀ);Š.ŵ.Add(Ύ);š.ŵ.Add(Ύ);}static void ʺ(Dictionary<long,D>W,long Ȯ,D θ){if(θ==D.
ņ){return;}D ι;W.TryGetValue(Ȯ,out ι);W[Ȯ]=ι|θ;}static D ˡ(Dictionary<long,D>W,long Ȯ){D Ϋ;return W.TryGetValue(Ȯ,out Ϋ)?
Ϋ:D.ņ;}static bool ά(string ľ,string κ){return!string.IsNullOrEmpty(ľ)&&!string.IsNullOrEmpty(κ)&&ľ.IndexOf(κ,
StringComparison.OrdinalIgnoreCase)>=0;}static void Ϊ<λ>(List<λ>ȫ,List<λ>Γ){ȫ.Clear();ȫ.AddRange(Γ);}}
