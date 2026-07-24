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
sealed class g{readonly Program A;float B=float.NaN;C D;public readonly IMyThrust E;public readonly F G;public H I;
public bool J;public double K;public C L{get{return D;}}public bool P{get{return D!=C.M;}set{N(C.O,value);}}public long Q{get{
return E.EntityId;}}public Vector3D R{get{return E.WorldMatrix.Backward;}}public Vector3D S{get{return E.WorldMatrix.Forward;}
}public double V{get{if(E==null||E.Closed||!E.IsFunctional){return 0;}double T=E.MaxEffectiveThrust;return T>U?T:0;}}
public Vector3D W{get{if(E==null||E.Closed||!E.IsFunctional){return Vector3D.Zero;}return R*E.CurrentThrust;}}public bool Y{
get{return(G&F.X)!=0;}}public bool a{get{return(G&F.Z)!=0;}}public bool c{get{if(E==null||E.Closed||!E.IsFunctional||Y||V<=
U){return false;}return E.Enabled||A.b(Q);}}public g(IMyThrust d,Program A,F e,bool f){E=d;this.A=A;G=e;if(f&&!Y){D=C.O;}
}public void N(C h,bool i){if(Y){D=C.M;j();return;}if(i){D|=h;return;}D&=~h;if(D==C.M){j();}}public bool k(C h){return(D&
h)!=0;}public void l(){K=0;}public double r(Vector3D m){if(!c){return 0;}m=n.o(m);if(m.LengthSquared()<=p){return 0;}
double q=Vector3D.Dot(R,m);return q>0?q*V:0;}public double x(ref Vector3D s){if(!P||!c){return 0;}Vector3D t=n.o(R);double u=
Vector3D.Dot(s,t);if(u<=U){return 0;}double v=V-K;if(v<=U){return 0;}double w=Math.Min(u,v);K+=w;s-=t*w;return w;}public void º(
){if(!P||E==null||E.Closed){return;}if(K>U){A.y(this);}double z=V;float ª=z>U?(float)MathHelper.Clamp(K/z,0,1):0;if(!
float.IsNaN(B)&&Math.Abs(ª-B)<µ&&Math.Abs(E.ThrustOverridePercentage-ª)<µ){return;}E.ThrustOverridePercentage=ª;B=ª;}public
void j(){K=0;if(E==null||E.Closed){return;}if(Math.Abs(E.ThrustOverridePercentage)>U){E.ThrustOverridePercentage=0;}B=0;}
public void À(){j();D=C.M;}}sealed class Ò{readonly Program A;double Á=double.NaN,Â=double.NaN,Ã,Ä;double Å;bool Æ,Ç;C D;
public readonly IMyMotorStator E;public readonly F G;public H I;public C L{get{return D;}}public bool P{get{return D!=C.M;}set
{N(C.O,value);}}public long Q{get{return E.EntityId;}}public bool Y{get{return(G&F.X)!=0;}}public bool a{get{return(G&F.Z
)!=0;}}public bool È{get{return E.BlockDefinition.SubtypeId.IndexOf("Hinge",StringComparison.OrdinalIgnoreCase)>=0;}}
public Vector3D É{get{return E.WorldMatrix.Up;}}public double Ê{get{return Ã;}}public double Ë{get{return Å;}}public bool Ñ{
get{if(E==null||E.Closed||E.Top==null||!E.IsFunctional||!E.Enabled||E.RotorLock){return false;}double Ì=E.LowerLimitRad;
double Í=E.UpperLimitRad;return!Î(Ì)||!Ï(Í)||Math.Abs(Í-Ì)>Ð;}}public Ò(IMyMotorStator d,Program A,F e,bool f){E=d;this.A=A;G=
e;if(f&&!Y){D=C.O;}}public void N(C h,bool i){if(Y){D=C.M;Ó();return;}if(i){D|=h;return;}D&=~h;if(D==C.M){Ó();}}public
bool k(C h){return(D&h)!=0;}public void Ù(double Ô){if(E==null||E.Closed){Ã=0;Â=double.NaN;return;}double Õ=E.Angle;if(
double.IsNaN(Â)||Ô<=p){Ã=0;}else{double Ö=Õ-Â;Ö=n.Ø(Ö);Ã=Ö/Ô;}Â=Õ;}public double Û(Vector3D Ú){return Û(Ú,1,A.Ü);}public
double Û(Vector3D Ú,double Ý,double Ô){if(!P||!Ñ||I==null||Ú.LengthSquared()<=p){Ó();return 0;}Þ();Ù(Ô);Vector3D à=I.ß;double
â=á(Ú,à);double ä=ã(â);Ý=MathHelper.Clamp(Ý,0,1);double ç=å+(æ-å)*Ý;double é=ä-Ã*Math.Max(Ô,è);if(ä*é<=0){é=0;}ê(é*ç);
Vector3D ì=n.ë(à,É,-ä);return n.í(ì,Ú);}public void ò(î ï,double Ý,double Ô){if(ï==null||ï.I!=I||!P||!Ñ){Ó();return;}Þ();Ù(Ô);
double ñ=ï.ð;Ý=MathHelper.Clamp(Ý,0,1);double ç=å+(æ-å)*Ý;double é=ñ-Ã*Math.Max(Ô,è);if(ñ*é<=0){é=0;}ê(é*ç);}public bool ô(
Vector3D Ú,out double ñ,out double ó){ñ=0;ó=0;if(!Ñ||I==null){return false;}Vector3D à=I.ß;if(à.LengthSquared()<=p||Ú.
LengthSquared()<=p){return false;}double â=á(Ú,à);ñ=ã(â);Vector3D ì=n.ë(à,É,-ñ);ó=n.í(ì,Ú);return true;}public double ö(Vector3D Ú,
Vector3D õ){return ã(á(Ú,õ));}public double ù(double ø){return ã(ø);}public void Ĉ(Vector3D ú,Vector3D û){Æ=false;Ç=false;if(!P
||!Ñ||I==null){Ó();Ç=true;A.ü.Remove(Q);return;}double ñ;double ó;if(ú.LengthSquared()>p){Vector3D ý=-n.o(ú);if(ô(ý,out ñ,
out ó)&&ó>=þ){ÿ(ñ);return;}}Vector3D ā=I.Ā();Vector3D Ă=E.GetPosition();Vector3D ă=ā-Ă;Vector3D Ą=û-Ă;Vector3D Ć=n.ą(ă,É);
Vector3D ć=n.ą(Ą,É);if(Ć.LengthSquared()<=p||ć.LengthSquared()<=p){ÿ(0);return;}ñ=á(ć,Ć);ñ=ã(ñ);ÿ(ñ);}public void Ċ(double ĉ){Ä=
ĉ;if(Î(E.LowerLimitRad)){Ä=Math.Max(Ä,E.LowerLimitRad);}if(Ï(E.UpperLimitRad)){Ä=Math.Min(Ä,E.UpperLimitRad);}Æ=true;Ç=
false;}public void Đ(){if(!Æ||!P||!Ñ){Ó();Ç=true;return;}double ċ=Ä-E.Angle;if(!Č()){ċ=n.Ø(ċ);}double Ď=ċ*č;Ç=Math.Abs(Ď)<=ď;
ê(Ç?0:Ď);}public void Þ(){Æ=false;Ç=false;}public void Ó(){ê(0);}public void À(){Ó();D=C.M;}double á(Vector3D m,Vector3D
đ){double ñ=n.Ē(m,đ,É);if(Math.Abs(Math.Abs(ñ)-Math.PI)<=ē){if(Å!=0){ñ=Math.Abs(ñ)*Å;}else{ñ=Math.Abs(ñ);}}return ñ;}void
ÿ(double ñ){Ä=E.Angle+ñ;if(Î(E.LowerLimitRad)){Ä=Math.Max(Ä,E.LowerLimitRad);}if(Ï(E.UpperLimitRad)){Ä=Math.Min(Ä,E.
UpperLimitRad);}Æ=true;Ç=false;A.ü[Q]=Ä;}double ã(double ø){ø=n.Ø(ø);bool Ĕ=Î(E.LowerLimitRad);bool ĕ=Ï(E.UpperLimitRad);if(!Ĕ&&!ĕ){
return ø;}double Õ=E.Angle;double Ė=double.NaN;double ė=double.MaxValue;for(int Ę=-2;Ę<=2;Ę++){double ę=ø+Ę*MathHelper.TwoPi;
double Ě=Õ+ę;if(Ĕ&&Ě<E.LowerLimitRad-ē){continue;}if(ĕ&&Ě>E.UpperLimitRad+ē){continue;}double ě=Math.Abs(ę);if(ě<ė){ė=ě;Ė=ę;}}
if(!double.IsNaN(Ė)){return Ė;}double Ĝ=Õ+ø;if(Ĕ){Ĝ=Math.Max(Ĝ,E.LowerLimitRad);}if(ĕ){Ĝ=Math.Min(Ĝ,E.UpperLimitRad);}
return Ĝ-Õ;}void ê(double ĝ){if(E==null||E.Closed){return;}ĝ=MathHelper.Clamp(ĝ,-Ğ,Ğ);if(Math.Abs(ĝ)<=ď){ĝ=0;}if(ĝ!=0){Å=Math.
Sign(ĝ);}if(!double.IsNaN(Á)&&Math.Abs(Á-ĝ)<ď&&Math.Abs(E.TargetVelocityRad-ĝ)<ď){return;}E.TargetVelocityRad=(float)ĝ;Á=ĝ;}
bool Č(){return Î(E.LowerLimitRad)||Ï(E.UpperLimitRad);}static bool Î(double ğ){return!double.IsNaN(ğ)&&!double.IsInfinity(ğ
)&&ğ>-1e20;}static bool Ï(double ğ){return!double.IsNaN(ğ)&&!double.IsInfinity(ğ)&&ğ<1e20;}}sealed class H{sealed class Ģ
{public Vector3D Ġ;public double ġ;}readonly Program A;readonly List<Ģ>ģ=new List<Ģ>();readonly List<double>Ĥ=new List<
double>();public readonly Ò Ò;public readonly List<g>ĥ=new List<g>();public readonly List<IMyCubeGrid>Ħ=new List<IMyCubeGrid>(
);public Vector3D ħ=Vector3D.Zero,Ĩ=Vector3D.Zero;public double ĩ;public H(Ò Ī,Program A){Ò=Ī;this.A=A;Ī.I=this;}public
Vector3D É{get{return Ò.É;}}public Vector3D ß{get{IMyCubeGrid ī=Ò.E.TopGrid;if(ī==null||ħ.LengthSquared()<=p){return Vector3D.
Zero;}Vector3D ĭ=n.Ĭ(ħ,ī.WorldMatrix);return-n.o(ĭ);}}public void ĸ(){ģ.Clear();for(int Į=0;Į<ĥ.Count;Į++){ĥ[Į].J=false;}
IMyCubeGrid ī=Ò.E.TopGrid;if(ī==null){ħ=Vector3D.Zero;ĩ=0;return;}MatrixD į=ī.WorldMatrix;for(int Į=0;Į<ĥ.Count;Į++){g İ=ĥ[Į];if(!İ
.P||!İ.c){continue;}double ı=İ.V;if(ı<=U){continue;}Vector3D ĳ=n.o(n.Ĳ(İ.S,į));Ģ Ĵ=null;for(int ĵ=0;ĵ<ģ.Count;ĵ++){if(
Vector3D.Dot(ģ[ĵ].Ġ,ĳ)>=Ķ){Ĵ=ģ[ĵ];break;}}if(Ĵ==null){Ĵ=new Ģ{Ġ=ĳ};ģ.Add(Ĵ);}Ĵ.ġ+=ı;}Ģ ķ=null;for(int Į=0;Į<ģ.Count;Į++){if(ķ==
null||ģ[Į].ġ>ķ.ġ){ķ=ģ[Į];}}if(ķ==null){ħ=Vector3D.Zero;ĩ=0;return;}ħ=ķ.Ġ;ĩ=ķ.ġ;for(int Į=0;Į<ĥ.Count;Į++){g İ=ĥ[Į];if(!İ.P||
!İ.c){continue;}Vector3D ĳ=n.o(n.Ĳ(İ.S,į));İ.J=Vector3D.Dot(ĳ,ħ)>=Ķ;}}public bool ň(Vector3D m,î Ĺ){if(Ĺ==null){return
false;}Ĺ.ĺ();Ĺ.I=this;m=n.o(m);if(!Ò.P||!Ò.Ñ||m.LengthSquared()<=p){return false;}Ĥ.Clear();Ļ(0);Vector3D ļ=ß;if(ļ.
LengthSquared()>p){Ļ(Ò.ö(m,ļ));}for(int Į=0;Į<ĥ.Count;Į++){g İ=ĥ[Į];if(!İ.P||!İ.c){continue;}Ļ(Ò.ö(m,İ.R));}double Õ=Ò.E.Angle;double
Ì=Ò.E.LowerLimitRad;double Í=Ò.E.UpperLimitRad;if(!double.IsNaN(Ì)&&!double.IsInfinity(Ì)&&Ì>-1e20){Ļ(Ì-Õ);}if(!double.
IsNaN(Í)&&!double.IsInfinity(Í)&&Í<1e20){Ļ(Í-Õ);}double ľ=Ľ(m,0);double Ŀ=-1;double ŀ=0;for(int Į=0;Į<Ĥ.Count;Į++){double Ł=Ò
.ù(Ĥ[Į]);double ł=Ľ(m,Ł);if(ł>Ŀ+U){Ŀ=ł;ŀ=Ł;continue;}if(Math.Abs(ł-Ŀ)>U){continue;}double Ń=Math.Abs(Ł);double ė=Math.Abs
(ŀ);if(Ń<ė-ē){ŀ=Ł;continue;}if(Math.Abs(Ń-ė)<=ē&&Ò.Ë!=0&&Math.Sign(Ł)==Ò.Ë){ŀ=Ł;}}if(Ŀ<=U){return false;}Ĺ.ð=ŀ;Ĺ.ń=Ŀ;Ĺ.Ņ=
ľ;Vector3D ņ=n.ë(ļ,É,-ŀ);Ĺ.Ň=n.í(ņ,m);return true;}public double ŉ(Vector3D m){î ï=new î();return ň(m,ï)?ï.ń:0;}public
double Ŋ(Vector3D Ú){Ĩ=Ú;if(Ú.LengthSquared()<=p){Ò.Ó();return 0;}î ï=new î();if(!ň(Ú,ï)){Ò.Ó();return 0;}double Ý=ï.ń>U?
MathHelper.Clamp(Ú.Length()/ï.ń,0,1):0;Ò.ò(ï,Ý,A.Ü);return ï.Ň;}public void Ŋ(î ï,double ŋ,double Ô){if(ï==null||ï.I!=this||ï.ń<=U
){Ĩ=Vector3D.Zero;Ò.Ó();return;}double Ý=MathHelper.Clamp(ŋ/ï.ń,0,1);Ò.ò(ï,Ý,Ô);}public double Œ(ref Vector3D s,Vector3D
Ō,ref Vector3D ō){double Ŏ=0;for(int Į=0;Į<ĥ.Count;Į++){g İ=ĥ[Į];if(!İ.J){continue;}double ŏ=İ.x(ref s);if(ŏ<=U){continue
;}Vector3D Ő=İ.R*ŏ;Vector3D ő=İ.E.GetPosition()-Ō;ō+=Vector3D.Cross(ő,Ő);Ŏ+=ŏ;}return Ŏ;}public double œ(ref Vector3D s,
Vector3D Ō,ref Vector3D ō){double Ŏ=0;for(int Į=0;Į<ĥ.Count;Į++){g İ=ĥ[Į];if(İ.J){continue;}double ŏ=İ.x(ref s);if(ŏ<=U){
continue;}Vector3D Ő=İ.R*ŏ;Vector3D ő=İ.E.GetPosition()-Ō;ō+=Vector3D.Cross(ő,Ő);Ŏ+=ŏ;}return Ŏ;}public Vector3D Ā(){if(Ħ.Count
==0){return Ò.E.TopGrid!=null?Ò.E.TopGrid.WorldAABB.Center:Ò.E.GetPosition();}Vector3D Ŕ=Vector3D.Zero;int ŕ=0;for(int Į=0
;Į<Ħ.Count;Į++){IMyCubeGrid Ŗ=Ħ[Į];if(Ŗ==null||Ŗ.Closed){continue;}Ŕ+=Ŗ.WorldAABB.Center;ŕ++;}return ŕ>0?Ŕ/ŕ:Ò.E.
GetPosition();}void Ļ(double ñ){ñ=Ò.ù(ñ);for(int Į=0;Į<Ĥ.Count;Į++){if(Math.Abs(Ĥ[Į]-ñ)<=ē){return;}}Ĥ.Add(ñ);}double Ľ(Vector3D m,
double ñ){double ł=0;for(int Į=0;Į<ĥ.Count;Į++){g İ=ĥ[Į];if(!İ.P||!İ.c){continue;}Vector3D ŗ=n.ë(İ.R,É,-ñ);double q=Vector3D.
Dot(ŗ,m);if(q<=0){continue;}ł+=q*İ.V;}return ł;}}sealed class ŝ{public readonly List<H>Ř=new List<H>();public Vector3D É{
get{return Ř.Count>0?n.o(Ř[0].É):Vector3D.Zero;}}public double ř{get{double ł=0;for(int Į=0;Į<Ř.Count;Į++){ł+=Ř[Į].ĩ;}
return ł;}}public Vector3D Ś(Vector3D Ő){return n.ą(Ő,É);}public double Ŝ(Vector3D s){Vector3D ś=Ś(s);if(ś.LengthSquared()<=p)
{return 0;}return Math.Min(ś.Length(),ř);}}sealed class ũ{readonly Program A;bool Ş;float ş=float.NaN,Š=float.NaN,š=float
.NaN;C D;public readonly IMyGyro E;public readonly F G;public readonly double Ţ;public C L{get{return D;}}public bool P{
get{return D!=C.M;}set{N(C.O,value);}}public bool Y{get{return(G&F.X)!=0;}}public bool a{get{return(G&F.Z)!=0;}}public
double ř{get{if(!P||E==null||E.Closed||!E.IsFunctional||!E.Enabled){return 0;}return Ţ*MathHelper.Clamp(E.GyroPower,0,1);}}
public ũ(IMyGyro d,Program A,F e,bool f){E=d;this.A=A;G=e;if(f&&!Y){D=C.O;}bool ţ=d.CubeGrid.GridSizeEnum==VRage.Game.
MyCubeSize.Small;bool Ť=d.BlockDefinition.SubtypeId.IndexOf("Prototech",StringComparison.OrdinalIgnoreCase)>=0;if(Ť){Ţ=ţ?ť:Ŧ;}else
{Ţ=ţ?ŧ:Ũ;}}public void N(C h,bool i){if(Y){D=C.M;Ū();return;}if(i){D|=h;return;}D&=~h;if(D==C.M){Ū();}}public bool k(C h)
{return(D&h)!=0;}public void Ŵ(Vector3D ū){if(!P||E==null||E.Closed||ř<=p){Ū();return;}ū=n.Ŭ(ū,ŭ);Vector3D Ů=n.Ĳ(ū,E.
WorldMatrix);float ů=(float)Ů.X;float Ű=(float)Ů.Y;float ű=(float)Ů.Z;bool ų=Ů.LengthSquared()>Ų*Ų;if(!ų){Ū();return;}if(!Ş||Math.
Abs(ů-ş)>Ų){E.Pitch=ů;ş=ů;}if(!Ş||Math.Abs(Ű-Š)>Ų){E.Yaw=Ű;Š=Ű;}if(!Ş||Math.Abs(ű-š)>Ų){E.Roll=ű;š=ű;}if(!Ş||!E.
GyroOverride){E.GyroOverride=true;}Ş=true;}public void Ū(){if(E==null||E.Closed){return;}if(E.GyroOverride){E.GyroOverride=false;}if
(Math.Abs(E.Pitch)>Ų){E.Pitch=0;}if(Math.Abs(E.Yaw)>Ų){E.Yaw=0;}if(Math.Abs(E.Roll)>Ų){E.Roll=0;}Ş=false;ş=0;Š=0;š=0;}
public void À(){Ū();D=C.M;}}sealed class Ž{public readonly IMyTerminalBlock ŵ;public readonly IMyTextSurface Ŷ;public readonly
int ŷ;bool Ÿ;public string Ź{get{return ŵ.EntityId+":"+ŷ;}}public Ž(IMyTerminalBlock ź,IMyTextSurface Ż,int ż){ŵ=ź;Ŷ=Ż;ŷ=ż;
}public void ſ(string ž){if(ŵ==null||ŵ.Closed||Ŷ==null){return;}if(!Ÿ){Ŷ.ContentType=VRage.Game.GUI.TextPanel.ContentType
.TEXT_AND_IMAGE;Ŷ.Font="Monospace";Ŷ.FontSize=0.8f;Ŷ.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.LEFT;Ÿ=true;}Ŷ.
WriteText(ž,false);}}enum ƅ{ƀ,Ɓ,Ƃ,ƃ,Ƅ}[Flags]enum F{M=0,Z=1,X=2,Ɔ=4,Ƈ=8,ƈ=16}[Flags]enum C{M=0,O=1,Ɖ=2,ƃ=4}[Flags]enum ƌ{M=0,Ɗ=1,
Ɖ=2,Ƌ=4}sealed class Ə{public bool ƍ;public ƌ Ǝ;}sealed class Ɩ{public double Ɛ,Ƒ,ƒ,Ɠ,Ɣ,ƕ;public void ĺ(){Ɛ=0;Ƒ=0;ƒ=0;Ɠ=0
;Ɣ=0;ƕ=0;}public void Ƙ(Ɩ Ɨ){if(Ɨ==null){return;}Ɛ+=Ɨ.Ɛ;Ƒ+=Ɨ.Ƒ;ƒ+=Ɨ.ƒ;Ɠ+=Ɨ.Ɠ;Ɣ+=Ɨ.Ɣ;ƕ+=Ɨ.ƕ;}public void ƙ(Ɩ Ɨ){if(Ɨ==null
){ĺ();return;}Ɛ=Ɨ.Ɛ;Ƒ=Ɨ.Ƒ;ƒ=Ɨ.ƒ;Ɠ=Ɨ.Ɠ;Ɣ=Ɨ.Ɣ;ƕ=Ɨ.ƕ;}public double ƛ(Vector3D ƚ){double ł=0;if(ƚ.X>0){ł+=Ɠ*ƚ.X;}else if(ƚ.X
<0){ł+=ƒ*-ƚ.X;}if(ƚ.Y>0){ł+=Ɣ*ƚ.Y;}else if(ƚ.Y<0){ł+=ƕ*-ƚ.Y;}if(ƚ.Z>0){ł+=Ƒ*ƚ.Z;}else if(ƚ.Z<0){ł+=Ɛ*-ƚ.Z;}return ł;}}
sealed class î{public H I;public double ð,ń,Ņ,Ň;public void ĺ(){I=null;ð=0;ń=0;Ņ=0;Ň=0;}}struct Ɯ:IEquatable<Ɯ>{public long Ɲ;
public ulong ƞ,Ɵ;public bool Equals(Ɯ Ɨ){return Ɲ==Ɨ.Ɲ&&ƞ==Ɨ.ƞ&&Ɵ==Ɨ.Ɵ;}public override bool Equals(object Ơ){return Ơ is Ɯ&&
Equals((Ɯ)Ơ);}public override int GetHashCode(){unchecked{int ơ=(int)Ɲ^(int)(Ɲ>>32);ơ=ơ*397^(int)ƞ^(int)(ƞ>>32);ơ=ơ*397^(int)Ɵ
^(int)(Ɵ>>32);return ơ;}}public static bool operator==(Ɯ Ƣ,Ɯ ƣ){return Ƣ.Equals(ƣ);}public static bool operator!=(Ɯ Ƣ,Ɯ ƣ
){return!Ƣ.Equals(ƣ);}}struct Ƥ:IEquatable<Ƥ>{public readonly long ƥ,Ʀ;public Ƥ(long Ƨ,long ƨ){ƥ=Math.Min(Ƨ,ƨ);Ʀ=Math.Max
(Ƨ,ƨ);}public bool Equals(Ƥ Ɨ){return ƥ==Ɨ.ƥ&&Ʀ==Ɨ.Ʀ;}public override bool Equals(object Ơ){return Ơ is Ƥ&&Equals((Ƥ)Ơ);}
public override int GetHashCode(){unchecked{int Ʃ=(int)ƥ^(int)(ƥ>>32);int ƪ=(int)Ʀ^(int)(Ʀ>>32);return Ʃ*397^ƪ;}}}sealed class
Ƶ{public long ƫ,Ƭ,ƭ;public Vector3D Ʈ;public bool Ư,Ɖ,ư;public double Ʊ,Ʋ;public int Ƴ,ƴ;public void ƙ(Ƶ Ɨ){ƫ=Ɨ.ƫ;Ƭ=Ɨ.Ƭ;ƭ
=Ɨ.ƭ;Ʈ=Ɨ.Ʈ;Ư=Ɨ.Ư;Ɖ=Ɨ.Ɖ;Ʊ=Ɨ.Ʊ;Ƴ=Ɨ.Ƴ;ƴ=Ɨ.ƴ;Ʋ=Ɨ.Ʋ;ư=Ɨ.ư;}public void ĺ(){ƫ=0;Ƭ=0;ƭ=0;Ʈ=Vector3D.Zero;Ư=true;Ɖ=false;Ʊ=0;Ƴ=0;
ƴ=0;Ʋ=0;ư=false;}}sealed class ƽ{readonly Program A;double ƶ;double Ʒ,Ƹ;int ƹ;public double ƺ{get{return ƶ;}}public
double ƻ{get{return Ʒ;}}public double Ƽ{get{return Ƹ;}}public ƽ(Program A){this.A=A;}public void ƾ(){Ƹ=A.Runtime.LastRunTimeMs
;}public void ǀ(){double ƿ=A.Runtime.LastRunTimeMs;ƹ++;if(ƹ==1){ƶ=ƿ;Ʒ=ƿ;return;}ƶ+=(ƿ-ƶ)*0.05;if(ƿ>Ʒ){Ʒ=ƿ;}else if(ƹ%600
==0){Ʒ=ƶ;}}}public static class n{public static Vector3D o(Vector3D ǁ){if(Vector3D.IsZero(ǁ)){return Vector3D.Zero;}if(
Vector3D.IsUnit(ref ǁ)){return ǁ;}return Vector3D.Normalize(ǁ);}public static Vector3D ą(Vector3D ǂ,Vector3D ǃ){double Ǆ=ǃ.
LengthSquared();if(ǂ.LengthSquared()<=p||Ǆ<=p){return Vector3D.Zero;}return ǂ-Vector3D.Dot(ǂ,ǃ)/Ǆ*ǃ;}public static Vector3D ǅ(
Vector3D ǂ,Vector3D ǃ){double Ǆ=ǃ.LengthSquared();if(ǂ.LengthSquared()<=p||Ǆ<=p){return Vector3D.Zero;}return Vector3D.Dot(ǂ,ǃ)/
Ǆ*ǃ;}public static double í(Vector3D ǂ,Vector3D ǃ){double Ǆ=Math.Sqrt(ǂ.LengthSquared()*ǃ.LengthSquared());if(Ǆ<=p){
return 0;}return MathHelper.Clamp(Vector3D.Dot(ǂ,ǃ)/Ǆ,-1,1);}public static Vector3D Ŭ(Vector3D ǁ,double ǆ){if(ǆ<=0){return
Vector3D.Zero;}double Ǉ=ǁ.LengthSquared();double ǈ=ǆ*ǆ;if(Ǉ<=ǈ){return ǁ;}if(Ǉ<=p){return Vector3D.Zero;}return ǁ*(ǆ/Math.Sqrt(Ǉ
));}public static double Ø(double ǉ){while(ǉ>Math.PI){ǉ-=MathHelper.TwoPi;}while(ǉ<-Math.PI){ǉ+=MathHelper.TwoPi;}return
ǉ;}public static Vector3D ë(Vector3D ǁ,Vector3D Ǌ,double ǉ){Ǌ=o(Ǌ);if(Ǌ.LengthSquared()<=p){return ǁ;}double ǋ=Math.Cos(ǉ
);double ǌ=Math.Sin(ǉ);return ǁ*ǋ+Vector3D.Cross(Ǌ,ǁ)*ǌ+Ǌ*Vector3D.Dot(Ǌ,ǁ)*(1.0-ǋ);}public static double Ē(Vector3D m,
Vector3D đ,Vector3D Ǎ){Vector3D ǎ=ą(m,Ǎ);Vector3D Ǐ=ą(đ,Ǎ);if(ǎ.LengthSquared()<=p||Ǐ.LengthSquared()<=p){return 0;}ǎ=o(ǎ);Ǐ=o(Ǐ
);Ǎ=o(Ǎ);return Math.Atan2(Vector3D.Dot(Ǎ,Vector3D.Cross(ǎ,Ǐ)),Vector3D.Dot(ǎ,Ǐ));}public static Vector3D Ĳ(Vector3D ǐ,
MatrixD Ǒ){return Vector3D.TransformNormal(ǐ,MatrixD.Transpose(Ǒ));}public static Vector3D Ĭ(Vector3D ƚ,MatrixD Ǒ){return
Vector3D.TransformNormal(ƚ,Ǒ);}}static string ǒ(ƅ ğ){switch(ğ){case ƅ.ƀ:return"Initializing";case ƅ.Ɓ:return"Active";case ƅ.Ƃ:
return"Master";case ƅ.ƃ:return"Slave";case ƅ.Ƅ:return"Parked";default:return"Unknown";}}sealed class ǘ{public readonly
IMyCubeGrid Ǔ;public readonly List<ǔ>Ǖ=new List<ǔ>();public ǖ Ǘ;public ǘ Ǚ;public ǔ ǚ;public int Ǜ=int.MaxValue;public bool ǜ;
public ǘ(IMyCubeGrid Ŗ){Ǔ=Ŗ;}}sealed class ǔ{public readonly ǘ ƥ,Ʀ;public readonly IMyTerminalBlock ǝ;public ǔ(ǘ ǂ,ǘ ǃ,
IMyTerminalBlock Ǟ){ƥ=ǂ;Ʀ=ǃ;ǝ=Ǟ;}public ǘ Ǡ(ǘ ǟ){return ǟ==ƥ?Ʀ:ƥ;}}sealed class ǖ{public readonly List<ǘ>ǡ=new List<ǘ>();public readonly
List<IMyShipController>Ǣ=new List<IMyShipController>();public readonly List<IMyProgrammableBlock>ǣ=new List<
IMyProgrammableBlock>();public bool ǜ,Ǥ,ǥ,Ǧ,ǧ=true,Ǩ=true;public IMyProgrammableBlock ǩ;}sealed class Ǭ{public IMyShipConnector ƥ,Ʀ;public ǘ
Ǫ,ǫ;}sealed class ǯ{public IMyShipConnector ǭ;public Ǭ Ǯ;}sealed class ǰ{public IMyLandingGear ǭ;}void Ǿ(string Ǳ){if(
string.IsNullOrWhiteSpace(Ǳ)){return;}string[]ǲ=Ǳ.Split(new[]{';','\n','\r'},StringSplitOptions.RemoveEmptyEntries);bool ǳ=
false;bool Ǵ=false;StringBuilder ǵ=new StringBuilder();for(int Į=0;Į<ǲ.Length;Į++){string Ƿ=Ƕ(ǲ[Į]);if(Ƿ.Length==0){continue;
}ǳ=true;string Ǹ;bool ǹ;if(!Ǻ(Ƿ,out Ǹ,out ǹ)){Ǹ="Unknown command: \""+Ƿ+"\"";ǹ=true;}if(ǻ&&!string.IsNullOrEmpty(Ǽ)){if(Ǹ
.Length>0){Ǹ+="\n";}Ǹ+=Ǽ;ǹ=true;}if(ǵ.Length>0){ǵ.AppendLine();}ǵ.Append(Ǹ);Ǵ|=ǹ;}if(!ǳ){return;}ǽ(ǵ.ToString(),Ǵ);Save()
;}bool Ǻ(string Ƿ,out string Ǹ,out bool ǹ){Ǹ=string.Empty;ǹ=false;string[]ǿ=Ƿ.Split(new[]{' '},StringSplitOptions.
RemoveEmptyEntries);if(ǿ.Length==0){return false;}int Ȁ;if(ȁ(ǿ,out Ȁ)){return Ȃ(ǿ,Ȁ,out Ǹ,out ǹ);}if(ȃ(ǿ,out Ȁ)){return Ȅ(ǿ,Ȁ,out Ǹ,out ǹ)
;}if(ȅ(ǿ,out Ȁ)){return Ȇ(ǿ,Ȁ,out Ǹ,out ǹ);}if(ǿ[0]=="unpark"||ǿ[0]=="undock"){ȇ=false;Ȉ=false;Ǹ="Parking: OFF";return
true;}if(ǿ[0]=="gear"){return ȉ(ǿ,1,out Ǹ,out ǹ);}if(ǿ[0]=="rescan"||ǿ[0]=="scan"||ǿ[0]=="refresh"){if(ǿ.Length!=1){Ǹ=
"Invalid rescan command: \""+Ƿ+"\"";ǹ=true;return true;}Ȋ();Ǹ="Deep rescan requested.";return true;}return false;}bool Ȃ(string[]ǿ,int Ȁ,out string
Ǹ,out bool ǹ){Ǹ=string.Empty;ǹ=false;bool i;if(!ȋ(ǿ,Ȁ,Ȍ,out i)){Ǹ="Expected dampeners "+"on, off, or toggle.";ǹ=true;
return true;}ȍ(i);Ǹ="Local dampeners: "+(Ȍ?"ON":"OFF");if(Ȏ==ƅ.ƃ){Ǹ+=" (master remains effective while slaved)";}return true;}
bool Ȅ(string[]ǿ,int Ȁ,out string Ǹ,out bool ǹ){Ǹ=string.Empty;ǹ=false;if(Ȁ>=ǿ.Length){ȏ();Ǹ=Ȑ();return true;}string ȑ=ǿ[Ȁ];
bool Ȓ;if(ȓ(ȑ,Ȕ,out Ȓ)){if(Ȁ+1!=ǿ.Length){Ǹ="Unexpected text after "+"Cruise state.";ǹ=true;return true;}ȕ(Ȓ);Ǹ=Ȑ();return
true;}double Ȗ;if(ȗ(ǿ,Ȁ,out Ȗ)){Ș(Ȗ);Ǹ=Ȑ();return true;}bool ș=ȑ=="increase"||ȑ=="increment"||ȑ=="up"||ȑ=="add"||ȑ=="faster"
;bool Ț=ȑ=="decrease"||ȑ=="decrement"||ȑ=="down"||ȑ=="subtract"||ȑ=="slower";if(ș||Ț){if(Ȁ+2!=ǿ.Length){Ǹ=
"Cruise adjustment requires "+"one speed value.";ǹ=true;return true;}double ț;if(!double.TryParse(ǿ[Ȁ+1],out ț)){Ǹ="Invalid Cruise speed: \""+ǿ[Ȁ+1]+
"\"";ǹ=true;return true;}ț=Math.Abs(ț);Ș(ș?ț:-ț);Ǹ=Ȑ();return true;}if(ȑ=="target"||ȑ=="speed"||ȑ=="set"){if(Ȁ+2!=ǿ.Length){
Ǹ="Cruise target requires "+"one speed value.";ǹ=true;return true;}double Ȝ;if(!double.TryParse(ǿ[Ȁ+1],out Ȝ)){Ǹ=
"Invalid Cruise target: \""+ǿ[Ȁ+1]+"\"";ǹ=true;return true;}ȝ=Ȝ;Ȟ=true;Ǹ=Ȑ();return true;}Ǹ="Expected Cruise on, off, toggle, "+
"+value, -value, increase value, "+"decrease value, or target value.";ǹ=true;return true;}bool Ȇ(string[]ǿ,int Ȁ,out string Ǹ,out bool ǹ){Ǹ=string.Empty;ǹ
=false;bool i;if(!ȋ(ǿ,Ȁ,ȇ,out i)){Ǹ="Expected parking "+"on, off, or toggle.";ǹ=true;return true;}ȇ=i;Ȉ=false;Ǹ=
"Parking: "+(ȇ?"ON":"OFF");return true;}bool ȉ(string[]ǿ,int Ȁ,out string Ǹ,out bool ǹ){Ǹ=string.Empty;ǹ=false;int ȡ=ȟ.Ƞ.Count;if(ȡ
<=0){Ǹ="No gears are configured.";ǹ=true;return true;}if(Ȁ>=ǿ.Length){Ȣ=(Ȣ+1)%ȡ;Ǹ=ȣ();return true;}if(Ȁ+1!=ǿ.Length){Ǹ=
"Gear accepts one argument.";ǹ=true;return true;}string ȑ=ǿ[Ȁ];if(ȑ=="next"||ȑ=="up"||ȑ=="increase"||ȑ=="increment"){Ȣ=(Ȣ+1)%ȡ;Ǹ=ȣ();return true;}if
(ȑ=="previous"||ȑ=="prev"||ȑ=="down"||ȑ=="decrease"||ȑ=="decrement"){Ȣ--;if(Ȣ<0){Ȣ=ȡ-1;}Ǹ=ȣ();return true;}int Ȥ;if(!int.
TryParse(ȑ,out Ȥ)){Ǹ="Invalid gear: \""+ȑ+"\"";ǹ=true;return true;}if(Ȥ<1||Ȥ>ȡ){Ǹ="Gear must be between 1 and "+ȡ+".";ǹ=true;
return true;}Ȣ=Ȥ-1;Ǹ=ȣ();return true;}static bool ȁ(string[]ǿ,out int Ȁ){Ȁ=1;if(ǿ[0]=="dampeners"||ǿ[0]=="dampener"||ǿ[0]==
"dampers"||ǿ[0]=="damper"||ǿ[0]=="damping"||ǿ[0]=="dampening"){return true;}if(ǿ.Length>=2&&ǿ[0]=="inertia"&&(ǿ[1]=="dampeners"||
ǿ[1]=="dampener"||ǿ[1]=="damping")){Ȁ=2;return true;}return false;}static bool ȃ(string[]ǿ,out int Ȁ){Ȁ=1;if(ǿ.Length>=2
&&ǿ[0]=="cruise"&&ǿ[1]=="control"){Ȁ=2;return true;}return ǿ[0]=="cruise"||ǿ[0]=="cruisecontrol";}static bool ȅ(string[]ǿ,
out int Ȁ){Ȁ=1;return ǿ[0]=="park"||ǿ[0]=="parking";}static bool ȋ(string[]ǿ,int Ȁ,bool ȥ,out bool Ǹ){if(Ȁ>=ǿ.Length){Ǹ=!ȥ;
return true;}if(Ȁ+1!=ǿ.Length){Ǹ=ȥ;return false;}return ȓ(ǿ[Ȁ],ȥ,out Ǹ);}static bool ȓ(string ȑ,bool ȥ,out bool Ǹ){if(ȑ=="on"
||ȑ=="enable"||ȑ=="enabled"||ȑ=="start"){Ǹ=true;return true;}if(ȑ=="off"||ȑ=="disable"||ȑ=="disabled"||ȑ=="stop"){Ǹ=false;
return true;}if(ȑ=="toggle"||ȑ=="switch"){Ǹ=!ȥ;return true;}Ǹ=ȥ;return false;}static bool ȗ(string[]ǿ,int Ȁ,out double Ö){Ö=0;
if(Ȁ>=ǿ.Length){return false;}if(Ȁ+1==ǿ.Length){string Ȧ=ǿ[Ȁ];if(Ȧ.Length<2||Ȧ[0]!='+'&&Ȧ[0]!='-'){return false;}return
double.TryParse(Ȧ,out Ö);}if(Ȁ+2==ǿ.Length&&(ǿ[Ȁ]=="+"||ǿ[Ȁ]=="-")){double ț;if(!double.TryParse(ǿ[Ȁ+1],out ț)){return false;}
Ö=ǿ[Ȁ]=="+"?Math.Abs(ț):-Math.Abs(ț);return true;}return false;}string Ȑ(){string Ǹ="Local Cruise: "+(Ȕ?"ON":"OFF");if(Ȟ)
{Ǹ+=" @ "+ȝ.ToString("0.###")+" m/s";}if(Ȏ==ƅ.ƃ){Ǹ+=" (master remains effective while slaved)";}return Ǹ;}string ȣ(){int
ȡ=ȟ.Ƞ.Count;double ȧ=ȡ>0?ȟ.Ƞ[MathHelper.Clamp(Ȣ,0,ȡ-1)]*100:0;string Ǹ="Local gear: "+(Ȣ+1)+"/"+ȡ+" ("+ȧ.ToString("0.##")
+"%)";if(Ȏ==ƅ.ƃ){Ǹ+=" (master remains effective while slaved)";}return Ǹ;}static string Ƕ(string Ƿ){if(string.
IsNullOrWhiteSpace(Ƿ)){return string.Empty;}Ƿ=Ƿ.Trim().ToLowerInvariant();StringBuilder Ȩ=new StringBuilder(Ƿ.Length);bool ȩ=false;for(int
Į=0;Į<Ƿ.Length;Į++){char Ȫ=Ƿ[Į];bool ȫ=char.IsWhiteSpace(Ȫ);if(ȫ){if(!ȩ&&Ȩ.Length>0){Ȩ.Append(' ');}ȩ=true;continue;}Ȩ.
Append(Ȫ);ȩ=false;}if(Ȩ.Length>0&&Ȩ[Ȩ.Length-1]==' '){Ȩ.Length--;}return Ȩ.ToString();}sealed class ȹ{public bool Ȭ=true,ȭ=
true,Ȯ=true,ȯ,Ȱ=true;public readonly List<double>Ƞ=new List<double>{0.15,0.50,1.00};public string ȱ="[VT-use]",Ȳ=
"[VT-ignore]",ȳ="[VT-status]",ȴ="[VT-park]",ȵ="[VT-unpark]";public int ȶ,ȷ,ȸ;}readonly MyIni Ⱥ=new MyIni();bool Ɏ(bool Ȼ){string ȼ=Me
.CustomData??string.Empty;if(!Ȼ&&ȼ==Ƚ){return false;}Ⱥ.Clear();MyIniParseResult Ⱦ;if(!Ⱥ.TryParse(ȼ,out Ⱦ)){Echo(ȿ+
"\n\nCustom Data could not be parsed as INI:\n"+Ⱦ);Ƚ=ȼ;return false;}bool ɀ=ȟ.Ȭ;bool Ɂ=ȟ.ȭ;bool ɂ=ȟ.Ȯ;string Ƀ=ȟ.ȱ;string Ʉ=ȟ.Ȳ;string Ʌ=ȟ.ȳ;string Ɇ=ȟ.ȴ;string ɇ=ȟ.ȵ;
ȟ.Ȭ=Ⱥ.Get(Ɉ,"Greedy").ToBoolean(ȟ.Ȭ);ȟ.ȭ=Ⱥ.Get(Ɉ,"CanMaster").ToBoolean(ȟ.ȭ);ȟ.Ȯ=Ⱥ.Get(Ɉ,"CanSlave").ToBoolean(ȟ.Ȯ);ȟ.ȯ=Ⱥ
.Get("Parking","ParkOnlyByCommand").ToBoolean(ȟ.ȯ);ȟ.Ȱ=Ⱥ.Get("Flight","CruiseLevelsWithGravity").ToBoolean(ȟ.Ȱ);ɉ(Ⱥ.Get(
"Flight","GearPercentages").ToString("15; 50; 100"));if(Ȣ>=ȟ.Ƞ.Count){Ȣ=ȟ.Ƞ.Count-1;}ȟ.ȱ=Ɋ("Tags","Use",ȟ.ȱ);ȟ.Ȳ=Ɋ("Tags",
"Ignore",ȟ.Ȳ);ȟ.ȳ=Ɋ("Tags","Status",ȟ.ȳ);ȟ.ȴ=Ɋ("Tags","ParkTimer",ȟ.ȴ);ȟ.ȵ=Ɋ("Tags","UnparkTimer",ȟ.ȵ);ȟ.ȶ=Math.Max(0,Ⱥ.Get(
"Performance","Update1Skip").ToInt32(ȟ.ȶ));ȟ.ȷ=Math.Max(0,Ⱥ.Get("Performance","Update10Skip").ToInt32(ȟ.ȷ));ȟ.ȸ=Math.Max(0,Ⱥ.Get(
"Performance","Update100Skip").ToInt32(ȟ.ȸ));ɋ();string Ɍ=Ⱥ.ToString();if(Ɍ!=Me.CustomData){Me.CustomData=Ɍ;}Ƚ=Me.CustomData;bool ɍ=ɀ
!=ȟ.Ȭ||Ɂ!=ȟ.ȭ||ɂ!=ȟ.Ȯ||!Ƀ.Equals(ȟ.ȱ,StringComparison.OrdinalIgnoreCase)||!Ʉ.Equals(ȟ.Ȳ,StringComparison.OrdinalIgnoreCase
)||!Ʌ.Equals(ȟ.ȳ,StringComparison.OrdinalIgnoreCase)||!Ɇ.Equals(ȟ.ȴ,StringComparison.OrdinalIgnoreCase)||!ɇ.Equals(ȟ.ȵ,
StringComparison.OrdinalIgnoreCase);if(!Ȼ&&ɍ){Ȋ();}return true;}string Ɋ(string ɏ,string ɐ,string ɑ){string ğ=Ⱥ.Get(ɏ,ɐ).ToString(ɑ).
Trim();return ğ.Length==0?ɑ:ğ;}void ɉ(string ɒ){string[]ɓ=ɒ.Split(new[]{';'},StringSplitOptions.RemoveEmptyEntries);List<
double>ɔ=new List<double>();for(int Į=0;Į<ɓ.Length;Į++){double ȧ;if(!double.TryParse(ɓ[Į].Trim(),out ȧ)){continue;}if(ȧ>0){ɔ.
Add(ȧ/100.0);}}if(ɔ.Count==0){return;}ȟ.Ƞ.Clear();ȟ.Ƞ.AddRange(ɔ);}void ɋ(){Ⱥ.Set(Ɉ,"Greedy",ȟ.Ȭ);Ⱥ.Set(Ɉ,"CanMaster",ȟ.ȭ);
Ⱥ.Set(Ɉ,"CanSlave",ȟ.Ȯ);Ⱥ.SetSectionComment(Ɉ," Vector Thrust Redux ownership and coordination.\n"+
" Greedy controls eligible mechanical-subgrid blocks unless ignored.\n"+" Main-grid player thrusters and gyros remain read-only unless explicitly tagged.");Ⱥ.Set("Parking","ParkOnlyByCommand"
,ȟ.ȯ);Ⱥ.Set("Flight","CruiseLevelsWithGravity",ȟ.Ȱ);Ⱥ.Set("Flight","GearPercentages",ɕ());Ⱥ.Set("Tags","Use",ȟ.ȱ);Ⱥ.Set(
"Tags","Ignore",ȟ.Ȳ);Ⱥ.Set("Tags","Status",ȟ.ȳ);Ⱥ.Set("Tags","ParkTimer",ȟ.ȴ);Ⱥ.Set("Tags","UnparkTimer",ȟ.ȵ);Ⱥ.SetComment(
"Tags","Use"," Tag may appear in a block name, group name, or block Custom Data.");Ⱥ.SetComment("Tags","Ignore",
" Ignore always prevents Redux from modifying the block.");Ⱥ.Set("Performance","Update1Skip",ȟ.ȶ);Ⱥ.Set("Performance","Update10Skip",ȟ.ȷ);Ⱥ.Set("Performance","Update100Skip",ȟ.ȸ
);Ⱥ.SetSectionComment("Performance"," Number of matching update intervals skipped between executions.\n"+
" Heartbeat publication is never skipped.");}string ɕ(){StringBuilder ɖ=new StringBuilder();for(int Į=0;Į<ȟ.Ƞ.Count;Į++){if(Į>0){ɖ.Append("; ");}ɖ.Append((ȟ.Ƞ[Į]*
100.0).ToString("0.########"));}return ɖ.ToString();}readonly HashSet<H>ɗ=new HashSet<H>(),ɘ=new HashSet<H>();void ɚ(){Ɏ(
false);ə();}void ɫ(){ɛ();ɜ();ɝ();ɞ();ɟ();ɠ=ȟ.ȭ&&ɡ!=null&&ɡ.IsUnderControl;ɢ();if(ȟ.Ȯ&&!ɠ&&!ȇ){ɣ();}if(ɤ){ɥ=0;}else if(ɦ!=long
.MinValue){ɥ++;}ɧ=ɦ!=long.MinValue&&ɥ<2;ɤ=false;ɨ();if(Ȏ==ƅ.Ƅ){ɩ();}ɪ(false);}void ʁ(double Ô){ɛ();if(Ȏ==ƅ.Ƅ||Ȏ==ƅ.ƀ||ɡ==
null){ɬ();ɭ();return;}ɮ();ɝ();ɞ();ɟ();Vector3D ɯ=ɡ.CenterOfMass;Vector3D ɰ;if(Ȏ==ƅ.ƃ){Vector3D ɲ=n.Ŭ(ɱ.Ʈ,1);Vector3D t=n.o(ɲ
);double Ý=ɲ.Length();double ɴ=ɳ(t);ɰ=t*Ý*ɴ;}else{Vector3D ɶ=ɵ(Ô);ɶ-=ɷ();if(Ȏ==ƅ.Ƃ){Vector3D t=n.o(ɶ);double ɴ=ɳ(t);
double ɹ=ɸ(t);double ɺ=ɴ+ɹ;double Ý=ɺ>U?MathHelper.Clamp(ɶ.Length()/ɺ,0,1):0;ɻ=t*Ý;ɰ=t*Ý*ɴ;}else{ɻ=Vector3D.Zero;ɰ=ɶ;}}ɼ=ɰ;ɽ(ɰ
,ɯ,Ô);bool ɾ=Ȏ==ƅ.ƃ?ɱ.ư:ǻ&&ȟ.Ȱ;ɿ(ʀ,ɾ);}void ɟ(){for(int Į=0;Į<ʂ.Count;Į++){ʂ[Į].ĸ();}ʃ();ʄ();}void ʃ(){ʅ=0;for(int Į=0;Į<
ʆ.Count;Į++){g İ=ʆ[Į];if(!İ.P||!İ.c){continue;}ʅ+=İ.V;}}void ʄ(){ʇ.ĺ();ʈ.ĺ();ʉ.ĺ();if(ɡ==null){return;}MatrixD ʊ=ɡ.
WorldMatrix;ʇ.Ɛ=ɳ(ʊ.Forward);ʇ.Ƒ=ɳ(ʊ.Backward);ʇ.ƒ=ɳ(ʊ.Left);ʇ.Ɠ=ɳ(ʊ.Right);ʇ.Ɣ=ɳ(ʊ.Up);ʇ.ƕ=ɳ(ʊ.Down);ʈ.Ɛ=ɸ(ʊ.Forward);ʈ.Ƒ=ɸ(ʊ.
Backward);ʈ.ƒ=ɸ(ʊ.Left);ʈ.Ɠ=ɸ(ʊ.Right);ʈ.Ɣ=ɸ(ʊ.Up);ʈ.ƕ=ɸ(ʊ.Down);ʉ.ƙ(ʇ);ʉ.Ƙ(ʈ);}double ɳ(Vector3D m){m=n.o(m);if(m.LengthSquared
()<=p){return 0;}double ł=0;for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į];if(!İ.P||!İ.c){continue;}if(İ.I!=null&&İ.I.Ò.P){continue;
}ł+=İ.r(m);}for(int Į=0;Į<ʂ.Count;Į++){H ʋ=ʂ[Į];if(!ʋ.Ò.P){continue;}ł+=ʋ.ŉ(m);}return ł;}double ʌ(Vector3D m){double ł=ɳ
(m);if(Ȏ==ƅ.Ƃ){ł+=ɸ(m);}return ł;}double ɸ(Vector3D m){m=n.o(m);if(m.LengthSquared()<=p){return 0;}double ł=ʍ(ʎ,m);for(
int Į=0;Į<ʏ.Count;Į++){ł+=ʏ[Į].ŉ(m);}return ł;}Vector3D ɵ(double Ô){MyShipMass ʐ=ɡ.CalculateShipMass();double ʑ=ʐ.
PhysicalMass;if(ʑ<=U){return Vector3D.Zero;}MyShipVelocities ʒ=ɡ.GetShipVelocities();Vector3D ʓ=ʒ.LinearVelocity;Vector3D ʔ=ɡ.
GetNaturalGravity();Vector3 ʕ=ɡ.MoveIndicator;Vector3D ʖ;if(ǻ){ʖ=ʗ(ʕ,ʓ,ʑ,Ô);}else{ʖ=ʘ(ʕ,ʓ,ʑ,Ô);}return ʑ*(ʖ-ʔ);}Vector3D ʘ(Vector3 ʕ,
Vector3D ʓ,double ʑ,double Ô){Vector3D ʙ=Vector3D.TransformNormal(ʕ,ɡ.WorldMatrix);Vector3D ʚ=n.o(ʙ);bool ʛ=ʚ.LengthSquared()>p;
Vector3D ʖ=Vector3D.Zero;if(ʛ){double ʜ=ʌ(ʚ);double ʞ=ʜ/ʑ*ʝ;ʖ=ʚ*ʞ;}if(!ʟ){return ʠ(ʖ,ʑ);}Vector3D ʡ=ʓ;if(ʡ.Length()<=ʢ){ʡ=
Vector3D.Zero;}if(ʛ){double ʣ=Vector3D.Dot(ʡ,ʚ);if(ʣ>0){ʡ-=ʚ*ʣ;}}if(ʡ.LengthSquared()>p){ʖ+=-ʡ/Math.Max(Ô,è);}return ʠ(ʖ,ʑ);}
Vector3D ʗ(Vector3 ʕ,Vector3D ʓ,double ʑ,double Ô){if(Ȏ!=ƅ.ƃ){ʤ();double ʥ=-ʕ.Z;if(Math.Abs(ʥ)>p){Vector3D ʦ=ʥ>=0?ɡ.WorldMatrix.
Forward:ɡ.WorldMatrix.Backward;double ł=ʌ(ʦ);double ʧ=ł/ʑ*ʝ;ȝ+=ʥ*ʧ*Ô;}}Vector3 ʨ=ʕ;ʨ.Z=0;Vector3D ʩ=Vector3D.TransformNormal(ʨ,
ɡ.WorldMatrix);Vector3D ʪ=n.o(ʩ);Vector3D ʖ=Vector3D.Zero;if(ʪ.LengthSquared()>p){double ʫ=ʌ(ʪ);ʖ+=ʪ*(ʫ/ʑ*ʝ);}Vector3D ʬ=
ɡ.WorldMatrix.Forward;Vector3D ʮ=ʬ*ʭ;Vector3D ʯ=ʮ-ʓ;if(!ʟ){ʯ=n.ǅ(ʯ,ʬ);}if(ʯ.Length()<=ʢ){ʯ=Vector3D.Zero;}if(ʯ.
LengthSquared()>p){ʖ+=ʯ/Math.Max(Ô,è);}return ʠ(ʖ,ʑ);}Vector3D ʠ(Vector3D ʞ,double ʑ){double ʰ=ʞ.Length();if(ʰ<=p||ʑ<=U){return
Vector3D.Zero;}Vector3D t=ʞ/ʰ;double ʱ=ʌ(t)/ʑ;return n.Ŭ(ʞ,ʱ);}Vector3D ɷ(){Vector3D Ő=Vector3D.Zero;for(int Į=0;Į<ʲ.Count;Į++){
g İ=ʲ[Į];if(!İ.P){Ő+=İ.W;}}if(Ȏ==ƅ.Ƃ){for(int Į=0;Į<ʳ.Count;Į++){g İ=ʳ[Į];if(!İ.P){Ő+=İ.W;}}}return Ő;}Vector3D ʴ(){
return ɷ();}void ɽ(Vector3D ɰ,Vector3D ɯ,double Ô){ʵ=ɰ;ʀ=Vector3D.Zero;for(int Į=0;Į<ʆ.Count;Į++){if(ʆ[Į].P){ʆ[Į].l();}}ɗ.
Clear();ɘ.Clear();for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į];if(!İ.P||İ.I!=null&&İ.I.Ò.P){continue;}ʶ(İ,ref ʵ,ɯ,ref ʀ);}for(int Į=0;
Į<ʂ.Count;Į++){H ʋ=ʂ[Į];if(!ʋ.Ò.P){continue;}ʋ.œ(ref ʵ,ɯ,ref ʀ);ɘ.Add(ʋ);}while(ɘ.Count>0&&ʵ.LengthSquared()>U*U){
Vector3D m=n.o(ʵ);H ʷ=null;î ʸ=null;foreach(H ʋ in ɘ){î ï=new î();if(!ʋ.ň(m,ï)){continue;}bool ʹ=ʸ==null||ï.ń>ʸ.ń+U;bool ʺ=ʸ!=
null&&Math.Abs(ï.ń-ʸ.ń)<=U;bool ʻ=ʺ&&ʷ!=null&&ʋ.Ò.Q<ʷ.Ò.Q;if(ʹ||ʻ){ʷ=ʋ;ʸ=ï;}}if(ʷ==null){break;}ɘ.Remove(ʷ);double ʼ=
Vector3D.Dot(ʵ,m);ʷ.Ŋ(ʸ,ʼ,Ô);ɗ.Add(ʷ);ʷ.Œ(ref ʵ,ɯ,ref ʀ);}for(int Į=0;Į<ʂ.Count;Į++){H ʋ=ʂ[Į];if(!ʋ.Ò.P){continue;}if(!ɗ.
Contains(ʋ)){ʋ.Ò.Ó();}}ʽ();for(int Į=0;Į<ʆ.Count;Į++){if(ʆ[Į].P){ʆ[Į].º();}}}void ʶ(g İ,ref Vector3D s,Vector3D ɯ,ref Vector3D ō
){double ŏ=İ.x(ref s);if(ŏ<=U){return;}Vector3D Ő=İ.R*ŏ;Vector3D ő=İ.E.GetPosition()-ɯ;ō+=Vector3D.Cross(ő,Ő);}void ʽ(){
if(!ǻ){ʾ(ƌ.Ɖ);return;}for(int Į=0;Į<ʿ.Count;Į++){g İ=ʿ[Į];if(!İ.P){continue;}if(İ.K>U){ʾ(İ,ƌ.Ɖ);y(İ);continue;}ˀ(İ,ƌ.Ɖ);}}
void ɿ(Vector3D ō,bool ɾ){if(ˁ.Count==0||ɡ==null){return;}double ˆ=0;for(int Į=0;Į<ˁ.Count;Į++){ˆ+=ˁ[Į].ř;}if(ˆ<=p){ɭ();
return;}Vector3D ˇ=-ō/ˆ*ŭ;if(ɾ){Vector3D ʔ=ɡ.GetNaturalGravity();if(ʔ.LengthSquared()>p){Vector3D ˈ=-n.o(ʔ);Vector3D ˉ=ɡ.
WorldMatrix.Up;Vector3D ˊ=Vector3D.Cross(ˉ,ˈ);double q=MathHelper.Clamp(Vector3D.Dot(ˉ,ˈ),-1,1);double ˋ=Math.Atan2(ˊ.Length(),q);
if(ˊ.LengthSquared()>p){ˊ=n.o(ˊ);ˇ+=ˊ*ˋ*ˌ;}Vector3D ˍ=ɡ.GetShipVelocities().AngularVelocity;Vector3D ˎ=n.ą(ˍ,ˈ);ˇ-=ˎ*ˏ;}}
if(ˇ.LengthSquared()<=Ų*Ų){ɭ();return;}for(int Į=0;Į<ˁ.Count;Į++){ˁ[Į].Ŵ(ˇ);}}void ɭ(){for(int Į=0;Į<ˁ.Count;Į++){ˁ[Į].Ū()
;}}void ˮ(){if(!ȟ.ȭ){ː();return;}if(Ȏ!=ƅ.Ƃ||ɡ==null||!ɡ.IsUnderControl){ː();return;}if(ˑ!=null&&ˑ.EntityId!=ɡ.EntityId){ˠ
(ˑ);}ˑ=ɡ;StringBuilder ɏ=new StringBuilder();ɏ.Append('[').Append(ˡ).AppendLine("]");ɏ.Append("Version=").AppendLine(ˢ);ɏ
.Append("MasterProgrammableBlockId=").AppendLine(Me.EntityId.ToString());ɏ.Append("ControllerId=").AppendLine(ɡ.EntityId.
ToString());ɏ.Append("Sequence=").AppendLine(ˣ.ToString());ɏ.Append("Demand=").AppendLine(ˤ(ɻ));ɏ.Append("Dampeners=").
AppendLine(Ȍ.ToString());ɏ.Append("Cruise=").AppendLine(Ȕ.ToString());ɏ.Append("CruiseTargetSpeed=").AppendLine(ȝ.ToString("R"));ɏ
.Append("GearIndex=").AppendLine(Ȣ.ToString());ɏ.Append("GearCount=").AppendLine(ȟ.Ƞ.Count.ToString());ɏ.Append(
"GearFraction=").AppendLine(ʝ.ToString("R"));ɏ.Append("LevelWithGravity=").AppendLine((Ȕ&&ȟ.Ȱ).ToString());ɡ.CustomData=ˬ(ɡ.CustomData,
ˡ,ɏ.ToString());}void ː(){if(ˑ==null){return;}ˠ(ˑ);ˑ=null;}void ˠ(IMyShipController Ͱ){if(Ͱ==null||Ͱ.Closed){return;}
string ͱ;if(!Ͳ(Ͱ.CustomData,ˡ,"MasterProgrammableBlockId",out ͱ)){return;}long ͳ;if(!long.TryParse(ͱ,out ͳ)||ͳ!=Me.EntityId){
return;}Ͱ.CustomData=ʹ(Ͱ.CustomData,ˡ);}void ͼ(IMyShipController Ͷ){long ͷ=Ͷ!=null?Ͷ.EntityId:0;for(int Į=0;Į<ͺ.Count;Į++){
IMyShipController Ͱ=ͺ[Į];if(Ͱ==null||Ͱ.EntityId==ͷ){continue;}ˠ(Ͱ);}for(int Į=0;Į<ͻ.Count;Į++){IMyShipController Ͱ=ͻ[Į];if(Ͱ==null||Ͱ.
EntityId==ͷ){continue;}ˠ(Ͱ);}}void Ά(){IMyShipController ͽ=Ȏ==ƅ.Ƃ&&ɡ!=null&&ɡ.IsUnderControl?ɡ:null;ͼ(ͽ);if(ͽ==null){ˑ=null;}
else{ˑ=ͽ;}}void ɣ(){for(int Į=0;Į<ͻ.Count;Į++){IMyShipController Ͱ=ͻ[Į];if(Ͱ==null||Ͱ.Closed||!Ͱ.IsUnderControl){continue;}Ƶ
Ƿ;if(!Έ(Ͱ,out Ƿ)){continue;}Ή(Ƿ);return;}}void Ί(){for(int Į=0;Į<ͻ.Count;Į++){IMyShipController Ͱ=ͻ[Į];if(Ͱ==null||Ͱ.
Closed||!Ͱ.IsUnderControl){continue;}if(ɱ.Ƭ!=0&&Ͱ.EntityId!=ɱ.Ƭ){continue;}Ƶ Ƿ;if(!Έ(Ͱ,out Ƿ)){continue;}if(ɱ.ƫ!=0&&Ƿ.ƫ!=ɱ.ƫ){
continue;}Ή(Ƿ);return;}}void Ή(Ƶ Ƿ){bool Ύ=Ƿ.ƭ!=ɦ||Ƿ.ƫ!=Ό||Ƿ.Ƭ!=ɱ.Ƭ;if(Ύ){ɦ=Ƿ.ƭ;Ό=Ƿ.ƫ;ɥ=0;ɤ=true;ɧ=true;}ɱ.ƙ(Ƿ);if(Ȏ==ƅ.ƃ){ɞ();ɝ
();}}bool Έ(IMyShipController Ͱ,out Ƶ Ƿ){Ƿ=null;if(Ͱ==null||Ͱ.Closed||!Ͱ.IsUnderControl){return false;}string Ώ;if(!Ͳ(Ͱ.
CustomData,ˡ,"Version",out Ώ)||string.IsNullOrWhiteSpace(Ώ)){return false;}if(ΐ(Ώ)!=ΐ(ˢ)){return false;}string Α;string Β;string Γ
;string Δ;if(!Ͳ(Ͱ.CustomData,ˡ,"MasterProgrammableBlockId",out Α)||!Ͳ(Ͱ.CustomData,ˡ,"ControllerId",out Β)||!Ͳ(Ͱ.
CustomData,ˡ,"Sequence",out Γ)||!Ͳ(Ͱ.CustomData,ˡ,"Demand",out Δ)){return false;}long ͱ;long Ε;long Ζ;Vector3D ɲ;if(!long.TryParse
(Α,out ͱ)||!long.TryParse(Β,out Ε)||!long.TryParse(Γ,out Ζ)||!Η(Δ,out ɲ)){return false;}if(ͱ==Me.EntityId||Ε!=Ͱ.EntityId)
{return false;}string Θ;string Ι;string Κ;string Λ;string Μ;string Ν;string Ξ;Ͳ(Ͱ.CustomData,ˡ,"Dampeners",out Θ);Ͳ(Ͱ.
CustomData,ˡ,"Cruise",out Ι);Ͳ(Ͱ.CustomData,ˡ,"CruiseTargetSpeed",out Κ);Ͳ(Ͱ.CustomData,ˡ,"GearIndex",out Λ);Ͳ(Ͱ.CustomData,ˡ,
"GearCount",out Μ);Ͳ(Ͱ.CustomData,ˡ,"GearFraction",out Ν);Ͳ(Ͱ.CustomData,ˡ,"LevelWithGravity",out Ξ);bool Ο=true;bool Π;bool Ρ;
double Σ;double Τ;int Υ;int Φ;bool Χ;if(bool.TryParse(Θ,out Χ)){Ο=Χ;}bool.TryParse(Ι,out Π);bool.TryParse(Ξ,out Ρ);double.
TryParse(Κ,out Σ);double.TryParse(Ν,out Τ);int.TryParse(Λ,out Υ);int.TryParse(Μ,out Φ);Ƿ=new Ƶ{ƫ=ͱ,Ƭ=Ε,ƭ=Ζ,Ʈ=n.Ŭ(ɲ,1),Ư=Ο,Ɖ=Π,Ʊ=
Σ,Ƴ=Math.Max(0,Υ),ƴ=Math.Max(0,Φ),Ʋ=MathHelper.Clamp(Τ,0,1),ư=Ρ};return true;}void Ψ(){ɧ=false;ɤ=false;ɥ=0;ɦ=long.
MinValue;Ό=0;ɱ.ĺ();}static int ΐ(string Ω){if(string.IsNullOrWhiteSpace(Ω)){return-1;}int Ϊ=Ω.IndexOf('.');string Ϋ=Ϊ>=0?Ω.
Substring(0,Ϊ):Ω;int ά;return int.TryParse(Ϋ,out ά)?ά:-1;}static int δ(string ȼ,string έ){if(string.IsNullOrEmpty(ȼ)){return-1;}
string ή="["+έ+"]";int ί=0;while(ί<ȼ.Length){int ΰ=ȼ.IndexOf(ή,ί,StringComparison.OrdinalIgnoreCase);if(ΰ<0){return-1;}bool α=
ΰ==0||ȼ[ΰ-1]=='\n';int β=ΰ+ή.Length;bool γ=β>=ȼ.Length||ȼ[β]=='\r'||ȼ[β]=='\n';if(α&&γ){return ΰ;}ί=ΰ+1;}return-1;}static
int θ(string ȼ,int ί){while(ί<ȼ.Length){int ε=ȼ.IndexOf('\n',ί);if(ε<0||ε+1>=ȼ.Length){return ȼ.Length;}ε++;int ζ=ε;while(ζ
<ȼ.Length&&(ȼ[ζ]==' '||ȼ[ζ]=='\t'||ȼ[ζ]=='\r')){ζ++;}if(ζ<ȼ.Length&&ȼ[ζ]=='['){int η=ȼ.IndexOf(']',ζ+1);if(η>=0){return ε
;}}ί=ε;}return ȼ.Length;}static bool Ͳ(string ȼ,string έ,string ɐ,out string ğ){ğ=null;int ι=δ(ȼ,έ);if(ι<0){return false;
}int κ=θ(ȼ,ι+έ.Length+2);int λ=ȼ.IndexOf('\n',ι);if(λ<0||λ>=κ){return false;}string ɏ=ȼ.Substring(λ+1,κ-λ-1);string[]μ=ɏ.
Replace("\r",string.Empty).Split('\n');for(int Į=0;Į<μ.Length;Į++){string ν=μ[Į];int Ϊ=ν.IndexOf('=');if(Ϊ<=0){continue;}string
ξ=ν.Substring(0,Ϊ).Trim();if(!ξ.Equals(ɐ,StringComparison.OrdinalIgnoreCase)){continue;}ğ=ν.Substring(Ϊ+1).Trim();return
true;}return false;}static string ˬ(string ȼ,string έ,string ο){ȼ=ȼ??string.Empty;ο=ο.TrimEnd('\r','\n')+"\n";int ι=δ(ȼ,έ);
if(ι<0){if(ȼ.Length==0){return ο;}string Ϊ=ȼ.EndsWith("\n")?string.Empty:"\n";return ȼ+Ϊ+ο;}int κ=θ(ȼ,ι+έ.Length+2);return
ȼ.Substring(0,ι)+ο+ȼ.Substring(κ);}static string ʹ(string ȼ,string έ){if(string.IsNullOrEmpty(ȼ)){return ȼ;}int ι=δ(ȼ,έ);
if(ι<0){return ȼ;}int κ=θ(ȼ,ι+έ.Length+2);string π=ȼ.Substring(0,ι);string β=ȼ.Substring(κ);if(π.EndsWith("\n")&&β.
StartsWith("\n")){β=β.Substring(1);}return π+β;}static string ˤ(Vector3D ǁ){return ǁ.X.ToString("R")+";"+ǁ.Y.ToString("R")+";"+ǁ.Z
.ToString("R");}static bool Η(string Ȧ,out Vector3D ǁ){ǁ=Vector3D.Zero;if(string.IsNullOrWhiteSpace(Ȧ)){return false;}
string[]ρ=Ȧ.Split(';');if(ρ.Length!=3){return false;}double ς;double σ;double τ;if(!double.TryParse(ρ[0],out ς)||!double.
TryParse(ρ[1],out σ)||!double.TryParse(ρ[2],out τ)){return false;}ǁ=new Vector3D(ς,σ,τ);return true;}IMyShipController ˑ;void ɛ(
){IMyShipController υ=ɡ;IMyShipController φ=null;for(int Į=0;Į<ͺ.Count;Į++){IMyShipController Ͱ=ͺ[Į];if(Ͱ==null||Ͱ.Closed
||!Ͱ.IsFunctional||!Ͱ.CanControlShip){continue;}if(Ͱ.IsUnderControl){φ=Ͱ;break;}if(φ==null||Ͱ.IsMainCockpit){φ=Ͱ;}}long χ=
υ!=null?υ.EntityId:0;long ψ=φ!=null?φ.EntityId:0;if(ˑ!=null&&ˑ.EntityId!=ψ){ˠ(ˑ);ˑ=null;}if(χ!=ψ){ɻ=Vector3D.Zero;ω();}ɡ=
φ;ϊ=ɡ==null;ɠ=ȟ.ȭ&&ɡ!=null&&ɡ.IsUnderControl;}void ɨ(){ɛ();if(Ȏ==ƅ.ƃ&&!ɧ){Ȉ=ϋ;}ƅ ό;if(ϊ||Me.CubeGrid.IsStatic||ȇ){ό=ƅ.Ƅ;}
else if(ȟ.Ȯ&&ɧ&&!ɠ){ό=ƅ.ƃ;}else if(ύ||Ȉ){ό=ƅ.Ƅ;}else if(ɠ){ό=ƅ.Ƃ;}else{ό=ƅ.Ɓ;}if(ό==Ȏ){ɝ();return;}ώ(ό);}void ώ(ƅ Ϗ){ƅ ϐ=Ȏ;
if(ϐ==ƅ.Ƃ&&Ϗ!=ƅ.Ƃ){ː();ɻ=Vector3D.Zero;}if(ϐ==ƅ.Ƅ&&Ϗ!=ƅ.Ƅ){ϑ();}if(ϐ==ƅ.ƃ&&Ϗ!=ƅ.ƃ&&!ɧ){Ȉ=ϋ;}Ȏ=Ϗ;if(Ϗ==ƅ.ƃ){ϋ=ϐ==ƅ.Ƅ;Ȉ=
false;}ϒ(ϐ,Ϗ);if(Ϗ==ƅ.Ƅ&&ϐ!=ƅ.Ƅ){Ĉ();}ϓ=true;}void ɢ(){if(ȟ.ȯ){ύ=false;return;}bool ϔ=false;for(int Į=0;Į<ϕ.Count;Į++){if(ϖ(ϕ
[Į])){ϔ=true;break;}}if(!ϔ){for(int Į=0;Į<ϗ.Count;Į++){if(Ϙ(ϗ[Į])){ϔ=true;break;}}}ύ=ϔ;}bool ϖ(ǯ ϙ){IMyShipConnector Ϛ=ϙ.
ǭ;if(Ϛ==null||Ϛ.Closed||Ϛ.Status!=MyShipConnectorStatus.Connected){return false;}IMyShipConnector Ɨ=Ϛ.OtherConnector;if(Ɨ
==null){return false;}ǘ ϛ;if(!Ϝ.TryGetValue(Ɨ.CubeGrid.EntityId,out ϛ)){return Ɨ.CubeGrid.IsStatic;}ǖ Ȝ=ϛ.Ǘ;if(Ȝ==null){
return Ɨ.CubeGrid.IsStatic;}if(Ȝ.ǥ){return true;}if(Ȝ.Ǣ.Count==0){return false;}if(ɠ&&Ȝ.Ǧ){return false;}return true;}bool Ϙ(ǰ
ϝ){IMyLandingGear Ϟ=ϝ.ǭ;if(Ϟ==null||Ϟ.Closed||!Ϟ.IsFunctional){return false;}return Ϟ.IsLocked;}void Ĉ(){ɬ();ɭ();Vector3D
ʔ=ɡ!=null?ɡ.GetNaturalGravity():Vector3D.Zero;Vector3D û=Me.CubeGrid.WorldAABB.Center;for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į]
;if(!İ.P){continue;}İ.j();ˀ(İ,ƌ.Ɗ);}ü.Clear();for(int Į=0;Į<ϟ.Count;Į++){Ò Ī=ϟ[Į];if(!Ī.P){continue;}Ī.Ĉ(ʔ,û);}Ϡ(ϡ);Save(
);}void ϑ(){ʾ(ƌ.Ɗ);Ϣ.Clear();for(int Į=0;Į<ϟ.Count;Į++){Ò Ī=ϟ[Į];Ī.Þ();Ī.Ó();}ü.Clear();ɻ=Vector3D.Zero;Ϡ(ϣ);Save();}void
ϥ(){for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į];Ϥ(İ);if(!İ.P){continue;}İ.j();ˀ(İ,ƌ.Ɗ);}ɭ();Vector3D ʔ=ɡ!=null?ɡ.
GetNaturalGravity():Vector3D.Zero;Vector3D û=Me.CubeGrid.WorldAABB.Center;for(int Į=0;Į<ϟ.Count;Į++){Ò Ī=ϟ[Į];if(!Ī.P){Ī.Ó();continue;}
double ĉ;if(ü.TryGetValue(Ī.Q,out ĉ)){Ī.Ċ(ĉ);}else{Ī.Ĉ(ʔ,û);}Ī.Đ();}}void ɩ(){for(int Į=0;Į<ϟ.Count;Į++){ϟ[Į].Đ();}}void ɬ(){
for(int Į=0;Į<ʆ.Count;Į++){if(ʆ[Į].P){ʆ[Į].j();}}}void ɮ(){ʾ(ƌ.Ɗ);Ϣ.Clear();}void ϧ(long Ϧ,IMyThrust d){ʾ(Ϧ,d,ƌ.Ɗ);}void Ϡ(
List<IMyTimerBlock>Ϩ){for(int Į=0;Į<Ϩ.Count;Į++){IMyTimerBlock ϩ=Ϩ[Į];if(ϩ==null||ϩ.Closed||!ϩ.IsFunctional){continue;}ϩ.
Trigger();}}const string ȿ="Vector Thrust Redux",ˢ="0.2.0",Ɉ="Vector Thrust Redux",ˡ="Vector Thrust Redux Heartbeat",Ϫ=
"VT-Redux:",ϫ="State",Ϭ="Disabled Thrusters",ϭ="Park Rotor Targets",Ϯ="Topology";const double p=1e-8,U=1e-3,ē=1e-4,Ð=1e-4,Ķ=1.0-
1e-6,ϯ=1.0-1e-4,þ=1.0-1e-4,å=0.1,č=1.0,æ=4.0,Ğ=Math.PI,ď=1e-3,ʢ=0.01,µ=0.0075,ˌ=4.0,ˏ=1.5,ŭ=30.0,Ų=1e-3,ŧ=448000.0,Ũ=
33600000.0,ť=4480000.0,Ŧ=201600000.0,è=1.0/120.0,ϰ=0.25;readonly ȹ ȟ=new ȹ();readonly MyIni ϱ=new MyIni();string Ƚ=string.Empty,ϲ=
string.Empty;bool Ȕ,ύ,ϓ;bool Ȍ=true,ȇ,Ȟ,ϳ,ϊ=true,ɠ,ɧ,Ȉ,ϋ,ɤ,ϴ,ϵ=true,Ϸ;int Ȣ,ɥ,ϸ,Ϲ;double ȝ,ʅ;readonly Dictionary<long,Ə>Ϻ=new
Dictionary<long,Ə>();readonly Dictionary<long,double>ü=new Dictionary<long,double>();Ɯ ϻ,ϼ;readonly Dictionary<long,bool>Ϣ=new
Dictionary<long,bool>();ƅ Ȏ=ƅ.ƀ;IMyShipController ɡ;long ˣ;long ɦ=long.MinValue,Ό;Ƶ ɱ=new Ƶ();Vector3D ɼ;Vector3D ʵ,ɻ,ʀ;double Ü,Ͻ
;int Ͼ;string Ǽ=string.Empty;bool Ͽ;readonly Ɩ ʇ=new Ɩ(),ʈ=new Ɩ(),ʉ=new Ɩ();readonly List<IMyShipController>ͺ=new List<
IMyShipController>(),ͻ=new List<IMyShipController>();readonly List<g>ʆ=new List<g>(),Ѐ=new List<g>(),Ё=new List<g>(),Ђ=new List<g>(),ʲ=
new List<g>(),Ѓ=new List<g>(),ʳ=new List<g>(),ʿ=new List<g>();readonly List<Ò>ϟ=new List<Ò>();readonly List<H>ʂ=new List<H>
();readonly List<ŝ>Є=new List<ŝ>();readonly List<ũ>ˁ=new List<ũ>();readonly List<ǯ>ϕ=new List<ǯ>();readonly List<ǰ>ϗ=new
List<ǰ>();readonly List<IMyTimerBlock>ϡ=new List<IMyTimerBlock>(),ϣ=new List<IMyTimerBlock>();readonly List<Ž>Ѕ=new List<Ž>(
);readonly Dictionary<long,ǘ>Ϝ=new Dictionary<long,ǘ>();readonly StringBuilder І=new StringBuilder(),Ї=new StringBuilder(
);IEnumerator<int>Ј;ƽ Љ;public
 Program
(){Љ=new ƽ(this);Њ();Ɏ(true);Runtime.UpdateFrequency=UpdateFrequency.Update1|UpdateFrequency.Update10|UpdateFrequency.
Update100;Ȋ();}public void
 Save
(){ϱ.Clear();ϱ.Set(ϫ,"Cruise",Ȕ);ϱ.Set(ϫ,"Dampeners",Ȍ);ϱ.Set(ϫ,"ManualPark",ȇ);ϱ.Set(ϫ,"Gear",Ȣ);ϱ.Set(ϫ,
"CruiseTargetSpeed",ȝ);ϱ.Set(ϫ,"CruiseTargetInitialized",Ȟ);foreach(KeyValuePair<long,Ə>Ћ in Ϻ){Ə Ȓ=Ћ.Value;string Ȧ=(Ȓ.ƍ?"1":"0")+";"+((
int)Ȓ.Ǝ).ToString();ϱ.Set(Ϭ,Ћ.Key.ToString(),Ȧ);}foreach(KeyValuePair<long,double>Ћ in ü){ϱ.Set(ϭ,Ћ.Key.ToString(),Ћ.Value)
;}if(Ϸ){ϱ.Set(Ϯ,"Count",ϼ.Ɲ);ϱ.Set(Ϯ,"Xor",ϼ.ƞ.ToString());ϱ.Set(Ϯ,"Sum",ϼ.Ɵ.ToString());}else if(ϳ){ϱ.Set(Ϯ,"Count",ϻ.Ɲ)
;ϱ.Set(Ϯ,"Xor",ϻ.ƞ.ToString());ϱ.Set(Ϯ,"Sum",ϻ.Ɵ.ToString());}Storage=ϱ.ToString();}public void
 Main
(string Ǳ,UpdateType Ќ){Љ.ƾ();double Ѝ=Runtime.TimeSinceLastRun.TotalSeconds;if(Ѝ<è){Ѝ=è;}else if(Ѝ>ϰ){Ѝ=ϰ;}Ͻ+=Ѝ;bool Ў=(
Ќ&(UpdateType.Terminal|UpdateType.Trigger|UpdateType.Script))!=0||!string.IsNullOrWhiteSpace(Ǳ);Џ();if(Ў){ɛ();Ǿ(Ǳ);}if((Ќ
&UpdateType.Update100)!=0&&А(ref Ϲ,ȟ.ȸ)){ɚ();}if((Ќ&UpdateType.Update10)!=0&&А(ref ϸ,ȟ.ȷ)){ɫ();}if((Ќ&UpdateType.Update1)
!=0){ˣ++;if(Ȏ==ƅ.ƃ){Ί();}ɨ();if(А(ref Ͼ,ȟ.ȶ)){Ü=MathHelper.Clamp(Ͻ,è,ϰ);Ͻ=0;ʁ(Ü);}if(ȟ.ȭ||ˑ!=null){ˮ();}}if(Ў){ɨ();if(ȟ.ȭ
||ˑ!=null){ˮ();}ϓ=true;}if(ϓ){ɪ(true);ϓ=false;}Љ.ǀ();}static bool А(ref int Б,int В){if(Б<В){Б++;return false;}Б=0;return
true;}void Њ(){if(string.IsNullOrWhiteSpace(Storage)){return;}MyIniParseResult Ⱦ;if(!ϱ.TryParse(Storage,out Ⱦ)){return;}Ȕ=ϱ.
Get(ϫ,"Cruise").ToBoolean(false);Ȍ=ϱ.Get(ϫ,"Dampeners").ToBoolean(true);ȇ=ϱ.Get(ϫ,"ManualPark").ToBoolean(false);Ȣ=Math.Max
(0,ϱ.Get(ϫ,"Gear").ToInt32(0));ȝ=ϱ.Get(ϫ,"CruiseTargetSpeed").ToDouble(0);Ȟ=ϱ.Get(ϫ,"CruiseTargetInitialized").ToBoolean(
false);Г();Д();Е();}void Г(){List<MyIniKey>Ж=new List<MyIniKey>();ϱ.GetKeys(Ϭ,Ж);for(int Į=0;Į<Ж.Count;Į++){MyIniKey ɐ=Ж[Į];
long Ϧ;if(!long.TryParse(ɐ.Name,out Ϧ)){continue;}string Ȧ=ϱ.Get(ɐ).ToString();string[]ρ=Ȧ.Split(';');if(ρ.Length!=2){
continue;}bool З=ρ[0]=="1";int И;if(!int.TryParse(ρ[1],out И)){continue;}ƌ Й=(ƌ)И;if(Й==ƌ.M){continue;}Ϻ[Ϧ]=new Ə{ƍ=З,Ǝ=Й};if((Й
&ƌ.Ɗ)!=0){Ϣ[Ϧ]=З;}}}void Д(){List<MyIniKey>Ж=new List<MyIniKey>();ϱ.GetKeys(ϭ,Ж);for(int Į=0;Į<Ж.Count;Į++){MyIniKey ɐ=Ж[
Į];long Ϧ;if(!long.TryParse(ɐ.Name,out Ϧ)){continue;}double Ȝ=ϱ.Get(ɐ).ToDouble(double.NaN);if(double.IsNaN(Ȝ)||double.
IsInfinity(Ȝ)){continue;}ü[Ϧ]=Ȝ;}}void Е(){long К=ϱ.Get(Ϯ,"Count").ToInt64(-1);string Л=ϱ.Get(Ϯ,"Xor").ToString();string М=ϱ.Get(Ϯ
,"Sum").ToString();ulong Н;ulong О;if(К<0||!ulong.TryParse(Л,out Н)||!ulong.TryParse(М,out О)){return;}ϻ=new Ɯ{Ɲ=К,ƞ=Н,Ɵ=
О};ϳ=true;}IEnumerable<int>Я(П Р){for(int Į=0;Į<Р.С.Count;Į++){IMyShipConnector d=Р.С[Į];ǘ ǟ;if(!Р.Т.TryGetValue(d.
CubeGrid.EntityId,out ǟ)||!ǟ.ǜ){continue;}Р.У.Add(d);F e=Ф(Р.G,d.EntityId);if(!Х(e)){continue;}Р.Ц.Add(new ǯ{ǭ=d,Ǯ=Ч(Р.Ш,d)});
yield return 1;}for(int Į=0;Į<Р.Щ.Count;Į++){IMyLandingGear d=Р.Щ[Į];ǘ ǟ;if(!Р.Т.TryGetValue(d.CubeGrid.EntityId,out ǟ)||!ǟ.ǜ
){continue;}F e=Ф(Р.G,d.EntityId);if(!Х(e)){continue;}Р.Ъ.Add(new ǰ{ǭ=d});yield return 1;}for(int Į=0;Į<Р.Ы.Count;Į++){
IMyTimerBlock ϩ=Р.Ы[Į];if(ϩ.CubeGrid!=Me.CubeGrid){continue;}F e=Ф(Р.G,ϩ.EntityId);if((e&F.Ƈ)!=0){Р.Ь.Add(ϩ);}if((e&F.ƈ)!=0){Р.Э.Add(
ϩ);}yield return 1;}Ю(Р);}void ъ(П Р){HashSet<long>а=new HashSet<long>();HashSet<long>б=new HashSet<long>();HashSet<long>
в=new HashSet<long>();for(int Į=0;Į<Р.ĥ.Count;Į++){а.Add(Р.ĥ[Į].Q);}for(int Į=0;Į<Р.г.Count;Į++){б.Add(Р.г[Į].Q);}for(int
Į=0;Į<Р.д.Count;Į++){в.Add(Р.д[Į].E.EntityId);}for(int Į=0;Į<ʆ.Count;Į++){g е=ʆ[Į];if(а.Contains(е.Q)){continue;}е.À();ϧ(
е.Q,е.E);ʾ(е.Q,е.E,ƌ.Ɖ);ʾ(е.Q,е.E,ƌ.Ƌ);}for(int Į=0;Į<ϟ.Count;Į++){Ò ж=ϟ[Į];if(!б.Contains(ж.Q)){ж.À();}}for(int Į=0;Į<ˁ.
Count;Į++){ũ з=ˁ[Į];if(!в.Contains(з.E.EntityId)){з.À();}}и(ͺ,Р.й);и(ͻ,Р.к);if(ͻ.Count==0){Ψ();}и(ʆ,Р.ĥ);и(Ѐ,Р.л);и(Ё,Р.м);и(
Ђ,Р.н);и(ʲ,Р.о);и(Ѓ,Р.п);и(ʳ,Р.р);и(ʎ,Р.с);и(ϟ,Р.г);и(ʂ,Р.т);и(Є,Р.у);и(ʏ,Р.ф);и(ˁ,Р.д);и(ϕ,Р.Ц);и(ϗ,Р.Ъ);и(х,Р.Ш);и(ц,Р.
У);и(ϡ,Р.Ь);и(ϣ,Р.Э);и(Ѕ,Р.ч);Ϝ.Clear();foreach(KeyValuePair<long,ǘ>Ћ in Р.Т){Ϝ.Add(Ћ.Key,Ћ.Value);}for(int Į=0;Į<ʆ.Count
;Į++){Ϥ(ʆ[Į]);}ш(а);ɛ();щ();ɝ();for(int Į=0;Į<ʆ.Count;Į++){if(!ʆ[Į].P){ʆ[Į].j();}}for(int Į=0;Į<ˁ.Count;Į++){if(!ˁ[Į].P){
ˁ[Į].Ū();}}if(Ȏ==ƅ.Ƅ){ϥ();}Ά();ϓ=true;}void Ю(П Р){HashSet<string>ы=new HashSet<string>(StringComparer.Ordinal);List<int>
ь=new List<int>();for(int Į=0;Į<Р.э.Count;Į++){IMyTerminalBlock d=Р.э[Į];ǘ ǟ;if(!Р.Т.TryGetValue(d.CubeGrid.EntityId,out
ǟ)||ǟ.Ǘ!=Р.ю){continue;}F e=Ф(Р.G,d.EntityId);IMyTextPanel я=d as IMyTextPanel;if(я!=null&&(e&F.Ɔ)!=0){ѐ(Р.ч,ы,d,я,0);}
IMyTextSurfaceProvider ё=d as IMyTextSurfaceProvider;if(ё==null||ё.SurfaceCount<=0){continue;}ь.Clear();ђ(d.CustomData,ё.SurfaceCount,ь);if((e
&F.Ɔ)!=0&&ь.Count==0){ь.Add(0);}for(int ΰ=0;ΰ<ь.Count;ΰ++){int ż=ь[ΰ];ѐ(Р.ч,ы,d,ё.GetSurface(ż),ż);}}}void ђ(string ȼ,int
ѓ,List<int>Ĺ){if(string.IsNullOrEmpty(ȼ)){return;}string[]μ=ȼ.Replace("\r",string.Empty).Split('\n');for(int Į=0;Į<μ.
Length;Į++){string ν=μ[Į].Trim();if(!ν.StartsWith(Ϫ,StringComparison.OrdinalIgnoreCase)){continue;}string є=ν.Substring(Ϫ.
Length).Trim();int ΰ;if(!int.TryParse(є,out ΰ)||ΰ<0||ΰ>=ѓ||Ĺ.Contains(ΰ)){continue;}Ĺ.Add(ΰ);}}static void ѐ(List<Ž>Ĺ,HashSet<
string>w,IMyTerminalBlock ź,IMyTextSurface Ż,int ż){string ɐ=ź.EntityId+":"+ż;if(!w.Add(ɐ)){return;}Ĺ.Add(new Ž(ź,Ż,ż));}bool
Х(F e){if((e&F.X)!=0){return false;}return ȟ.Ȭ||(e&F.Z)!=0;}bool і(IMyProgrammableBlock ѕ){if(ѕ==null){return false;}
return δ(ѕ.CustomData,Ɉ)>=0;}bool ї(IMyProgrammableBlock ѕ){string Ȧ;if(!Ͳ(ѕ.CustomData,Ɉ,"CanSlave",out Ȧ)){return true;}bool
ğ;return bool.TryParse(Ȧ,out ğ)?ğ:true;}F љ(string ž){if(string.IsNullOrEmpty(ž)){return F.M;}F Ǹ=F.M;if(ј(ž,ȟ.ȱ)){Ǹ|=F.Z
;}if(ј(ž,ȟ.Ȳ)){Ǹ|=F.X;}if(ј(ž,ȟ.ȳ)){Ǹ|=F.Ɔ;}if(ј(ž,ȟ.ȴ)){Ǹ|=F.Ƈ;}if(ј(ž,ȟ.ȵ)){Ǹ|=F.ƈ;}return Ǹ;}static bool ј(string ž,
string њ){return!string.IsNullOrEmpty(ž)&&!string.IsNullOrEmpty(њ)&&ž.IndexOf(њ,StringComparison.OrdinalIgnoreCase)>=0;}static
void ѝ(Dictionary<long,F>e,long Ϧ,F ћ){if(ћ==F.M){return;}F ќ;e.TryGetValue(Ϧ,out ќ);e[Ϧ]=ќ|ћ;}static F Ф(Dictionary<long,F>
e,long Ϧ){F Ǹ;return e.TryGetValue(Ϧ,out Ǹ)?Ǹ:F.M;}static Ǭ Ч(List<Ǭ>ў,IMyShipConnector Ϛ){for(int Į=0;Į<ў.Count;Į++){if(
ў[Į].ƥ.EntityId==Ϛ.EntityId||ў[Į].Ʀ.EntityId==Ϛ.EntityId){return ў[Į];}}return null;}static void Ѡ(List<IMyCubeGrid>џ,
IMyCubeGrid Ŗ){for(int Į=0;Į<џ.Count;Į++){if(џ[Į].EntityId==Ŗ.EntityId){return;}}џ.Add(Ŗ);}static void и<ѡ>(List<ѡ>Ȝ,List<ѡ>Ѣ){Ȝ.
Clear();Ȝ.AddRange(Ѣ);}IEnumerable<int>ѱ(П Р){Dictionary<long,Ò>ѣ=new Dictionary<long,Ò>();Dictionary<long,IMyMotorStator>Ѥ=
new Dictionary<long,IMyMotorStator>();Dictionary<long,List<g>>ѥ=new Dictionary<long,List<g>>();Dictionary<long,List<g>>Ѧ=
new Dictionary<long,List<g>>();for(int Į=0;Į<Р.ѧ.Count;Į++){IMyMotorStator d=Р.ѧ[Į];ǘ ǟ;if(!Р.Т.TryGetValue(d.CubeGrid.
EntityId,out ǟ)){continue;}F e=Ф(Р.G,d.EntityId);if(ǟ.ǜ){Ò Ī=new Ò(d,this,e,false);ѣ.Add(d.EntityId,Ī);Р.Ѩ.Add(Ī);}else if(ѩ(Р,ǟ
.Ǘ)){Ѥ.Add(d.EntityId,d);}yield return 1;}for(int Į=0;Į<Р.Ѫ.Count;Į++){IMyThrust d=Р.Ѫ[Į];ǘ ǟ;if(!Р.Т.TryGetValue(d.
CubeGrid.EntityId,out ǟ)){continue;}F e=Ф(Р.G,d.EntityId);if(ǟ.ǜ){ѫ(Р,ǟ,d,e,ѣ,ѥ);}else if(ѩ(Р,ǟ.Ǘ)){Ѭ(Р,ǟ,d,e,Ѥ,Ѧ);}else if(ǟ.Ǘ.
Ǥ){g İ=new g(d,this,e,false);Р.р.Add(İ);Р.н.Add(İ);}yield return 1;}ѭ(Р,ѥ);Ѯ(Р,Ѥ,Ѧ);foreach(int Ѱ in ѯ(Р)){yield return Ѱ
;}}void ѫ(П Р,ǘ ǟ,IMyThrust d,F e,Dictionary<long,Ò>Ѳ,Dictionary<long,List<g>>ѳ){Ò ѵ=Ѵ(ǟ,Ѳ);bool Ѷ=ǟ.Ǜ>0;bool ѷ=(e&F.Z)!=
0;bool Ѹ=(e&F.X)!=0;bool ѹ=!Ѹ&&(ȟ.Ȭ?ѷ||Ѷ||ѵ!=null:ѷ);g İ=new g(d,this,e,ѹ);Р.ĥ.Add(İ);if(ѹ){Р.л.Add(İ);}else{Р.о.Add(İ);Р
.н.Add(İ);}if(ѵ==null){if(ѹ){Р.м.Add(İ);}return;}List<g>Ѻ;if(!ѳ.TryGetValue(ѵ.Q,out Ѻ)){Ѻ=new List<g>();ѳ.Add(ѵ.Q,Ѻ);}Ѻ.
Add(İ);}void ѭ(П Р,Dictionary<long,List<g>>ѳ){for(int Į=0;Į<Р.Ѩ.Count;Į++){Ò Ī=Р.Ѩ[Į];List<g>Ѻ;bool ѻ=ѳ.TryGetValue(Ī.Q,out
Ѻ);bool Ѽ=false;if(ѻ){for(int ĵ=0;ĵ<Ѻ.Count;ĵ++){if(Ѻ[ĵ].P){Ѽ=true;break;}}}bool f=!Ī.Y&&(Ī.a||ȟ.Ȭ&&Ѽ);Ī.N(C.O,f);if(!f){
if(ѻ){for(int ĵ=0;ĵ<Ѻ.Count;ĵ++){g İ=Ѻ[ĵ];if(İ.P&&!Р.м.Contains(İ)){Р.м.Add(İ);}}}continue;}Р.г.Add(Ī);if(!Ѽ){if(Ī.a){Ī.Ó(
);}continue;}H ʋ=new H(Ī,this);Р.т.Add(ʋ);for(int ĵ=0;ĵ<Ѻ.Count;ĵ++){g İ=Ѻ[ĵ];if(!İ.P){continue;}İ.I=ʋ;ʋ.ĥ.Add(İ);ǘ ѽ;if(
Р.Т.TryGetValue(İ.E.CubeGrid.EntityId,out ѽ)){Ѿ(ʋ,ѽ,Ī);}}ʋ.ĸ();}}void Ѭ(П Р,ǘ ǟ,IMyThrust d,F e,Dictionary<long,
IMyMotorStator>Ѳ,Dictionary<long,List<g>>ѳ){ǖ ѿ=ǟ.Ǘ;IMyMotorStator ѵ=Ҁ(ǟ,Ѳ);bool ѷ=(e&F.Z)!=0;bool Ѹ=(e&F.X)!=0;bool ҁ=!Ѹ&&ѿ.Ǩ&&(ѿ.ǧ||
ѷ);g İ=new g(d,this,e,false);if(!ҁ){Р.р.Add(İ);Р.н.Add(İ);return;}Р.п.Add(İ);if(ѵ==null){Р.с.Add(İ);return;}List<g>Ѻ;if(!
ѳ.TryGetValue(ѵ.EntityId,out Ѻ)){Ѻ=new List<g>();ѳ.Add(ѵ.EntityId,Ѻ);}Ѻ.Add(İ);}void Ѯ(П Р,Dictionary<long,IMyMotorStator
>Ѳ,Dictionary<long,List<g>>ѳ){foreach(KeyValuePair<long,List<g>>Ћ in ѳ){IMyMotorStator Ҋ;if(!Ѳ.TryGetValue(Ћ.Key,out Ҋ)){
ҋ(Р,Ћ.Value);continue;}ǘ Ҍ;if(!Р.Т.TryGetValue(Ҋ.CubeGrid.EntityId,out Ҍ)){ҋ(Р,Ћ.Value);continue;}F ҍ=Ф(Р.G,Ҋ.EntityId);
bool Ѹ=(ҍ&F.X)!=0;bool ѷ=(ҍ&F.Z)!=0;ǖ ѿ=Ҍ.Ǘ;bool ҁ=!Ѹ&&ѿ.Ǩ&&(ѷ||ѿ.ǧ&&Ћ.Value.Count>0);if(!ҁ||Ҋ.TopGrid==null||!Ҋ.
IsFunctional||!Ҋ.Enabled||Ҋ.RotorLock){ҋ(Р,Ћ.Value);continue;}Ҏ ҏ=new Ҏ(Ҋ);ҏ.ĥ.AddRange(Ћ.Value);Р.ф.Add(ҏ);}}static void ҋ(П Р,List
<g>ʆ){for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į];if(!Р.с.Contains(İ)){Р.с.Add(İ);}}}IEnumerable<int>ѯ(П Р){for(int Į=0;Į<Р.Ґ.
Count;Į++){IMyGyro d=Р.Ґ[Į];ǘ ǟ;if(!Р.Т.TryGetValue(d.CubeGrid.EntityId,out ǟ)||!ǟ.ǜ||!ґ(d)){continue;}F e=Ф(Р.G,d.EntityId);
bool Ѹ=(e&F.X)!=0;if(Ѹ){continue;}bool Ѷ=ǟ.Ǜ>0;bool ѷ=(e&F.Z)!=0;bool Ғ=ȟ.Ȭ?ѷ||Ѷ:ѷ;bool ғ=d.CubeGrid==Me.CubeGrid;if(!Ғ&&!ғ)
{continue;}ũ Ҕ=new ũ(d,this,e,Ғ);Р.д.Add(Ҕ);yield return 1;}}Ò Ѵ(ǘ ǟ,Dictionary<long,Ò>Ѳ){ǘ ҕ=ǟ;while(ҕ!=null&&ҕ.Ǚ!=null)
{ǔ Җ=ҕ.ǚ;IMyMotorStator җ=Җ!=null?Җ.ǝ as IMyMotorStator:null;if(җ!=null&&җ.TopGrid==ҕ.Ǔ){Ò Ī;if(Ѳ.TryGetValue(җ.EntityId,
out Ī)){return Ī;}}ҕ=ҕ.Ǚ;}return null;}IMyMotorStator Ҁ(ǘ ǟ,Dictionary<long,IMyMotorStator>Ѳ){ǘ ҕ=ǟ;while(ҕ!=null&&ҕ.Ǚ!=
null){ǔ Җ=ҕ.ǚ;IMyMotorStator җ=Җ!=null?Җ.ǝ as IMyMotorStator:null;if(җ!=null&&җ.TopGrid==ҕ.Ǔ){IMyMotorStator Ī;if(Ѳ.
TryGetValue(җ.EntityId,out Ī)){return Ī;}}ҕ=ҕ.Ǚ;}return null;}void Ѿ(H ʋ,ǘ ѽ,Ò Ī){ǘ ҕ=ѽ;while(ҕ!=null){Ѡ(ʋ.Ħ,ҕ.Ǔ);if(ҕ.ǚ!=null&&ҕ.ǚ
.ǝ.EntityId==Ī.Q){break;}ҕ=ҕ.Ǚ;}}static bool ѩ(П Р,ǖ ѿ){return ѿ!=null&&ѿ!=Р.ю&&!ѿ.ǜ&&ѿ.Ǥ&&ѿ.ǩ!=null;}bool ґ(IMyGyro Ҕ){
string Ҙ=Ҕ.BlockDefinition.SubtypeId;if(Ҙ.Equals("SmallBlockGyro",StringComparison.OrdinalIgnoreCase)||Ҙ.Equals(
"LargeBlockGyro",StringComparison.OrdinalIgnoreCase)){return true;}return Ҙ.Equals("SmallPrototechGyro",StringComparison.
OrdinalIgnoreCase)||Ҙ.Equals("LargePrototechGyro",StringComparison.OrdinalIgnoreCase)||Ҙ.Equals("SmallPrototechGyroscope",
StringComparison.OrdinalIgnoreCase)||Ҙ.Equals("LargePrototechGyroscope",StringComparison.OrdinalIgnoreCase);}IEnumerable<int>Ҥ(){П Р=new
П();foreach(int Ѱ in ҙ(Р)){yield return Ѱ;}foreach(int Ѱ in Қ(Р)){yield return Ѱ;}қ(Р);foreach(int Ѱ in Ҝ(Р)){yield
return Ѱ;}foreach(int Ѱ in ҝ(Р)){yield return Ѱ;}ǘ ҟ=Ҟ(Р.Т,Me.CubeGrid);Р.ю=ҟ.Ǘ;Ҡ(Р);ҡ(Р,ҟ);Ң(Р);foreach(int Ѱ in ң(Р)){yield
return Ѱ;}foreach(int Ѱ in ѱ(Р)){yield return Ѱ;}foreach(int Ѱ in Я(Р)){yield return Ѱ;}ъ(Р);yield return 1;}IEnumerable<int>ҙ
(П Р){GridTerminalSystem.GetBlocks(Р.э);List<IMyBlockGroup>ҥ=new List<IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(
ҥ);List<IMyTerminalBlock>Ҧ=new List<IMyTerminalBlock>();for(int Į=0;Į<ҥ.Count;Į++){IMyBlockGroup ҧ=ҥ[Į];F Ҩ=љ(ҧ.Name);if(
Ҩ==F.M){continue;}Ҧ.Clear();ҧ.GetBlocks(Ҧ);for(int ĵ=0;ĵ<Ҧ.Count;ĵ++){ѝ(Р.G,Ҧ[ĵ].EntityId,Ҩ);}yield return 1;}for(int Į=0
;Į<Р.э.Count;Į++){IMyTerminalBlock d=Р.э[Į];F ҩ=љ(d.CustomName)|љ(d.CustomData);ѝ(Р.G,d.EntityId,ҩ);Ҟ(Р.Т,d.CubeGrid);
IMyShipController Ͱ=d as IMyShipController;if(Ͱ!=null){Р.Ǣ.Add(Ͱ);}IMyThrust T=d as IMyThrust;if(T!=null){Р.Ѫ.Add(T);}IMyMotorStator Ī=d
as IMyMotorStator;if(Ī!=null){Р.ѧ.Add(Ī);}IMyPistonBase Ҫ=d as IMyPistonBase;if(Ҫ!=null){Р.ҫ.Add(Ҫ);}IMyGyro Ҕ=d as
IMyGyro;if(Ҕ!=null){Р.Ґ.Add(Ҕ);}IMyShipConnector Ϛ=d as IMyShipConnector;if(Ϛ!=null){Р.С.Add(Ϛ);}IMyLandingGear Ϟ=d as
IMyLandingGear;if(Ϟ!=null){Р.Щ.Add(Ϟ);}IMyTimerBlock ϩ=d as IMyTimerBlock;if(ϩ!=null){Р.Ы.Add(ϩ);}IMyProgrammableBlock ѕ=d as
IMyProgrammableBlock;if(ѕ!=null){Р.Ҭ.Add(ѕ);}yield return 1;}Ҟ(Р.Т,Me.CubeGrid);}IEnumerable<int>Қ(П Р){for(int Į=0;Į<Р.ѧ.Count;Į++){
IMyMotorStator Ī=Р.ѧ[Į];if(Ī.TopGrid==null){continue;}ҭ(Р.Т,Ī.CubeGrid,Ī.TopGrid,Ī);yield return 1;}for(int Į=0;Į<Р.ҫ.Count;Į++){
IMyPistonBase Ҫ=Р.ҫ[Į];if(Ҫ.TopGrid==null){continue;}ҭ(Р.Т,Ҫ.CubeGrid,Ҫ.TopGrid,Ҫ);yield return 1;}for(int Į=0;Į<Р.С.Count;Į++){
IMyShipConnector Ɨ=Р.С[Į].OtherConnector;if(Ɨ!=null){Ҟ(Р.Т,Ɨ.CubeGrid);}yield return 1;}}void қ(П Р){List<ǘ>Ү=new List<ǘ>();foreach(
KeyValuePair<long,ǘ>Ћ in Р.Т){ǘ ү=Ћ.Value;if(ү.Ǘ!=null){continue;}ǖ ѿ=new ǖ();Р.Ұ.Add(ѿ);Ү.Clear();Ү.Add(ү);ү.Ǘ=ѿ;for(int ΰ=0;ΰ<Ү.
Count;ΰ++){ǘ ǟ=Ү[ΰ];ѿ.ǡ.Add(ǟ);if(ǟ.Ǔ.IsStatic){ѿ.ǥ=true;}for(int ұ=0;ұ<ǟ.Ǖ.Count;ұ++){ǘ Ҳ=ǟ.Ǖ[ұ].Ǡ(ǟ);if(Ҳ.Ǘ!=null){continue
;}Ҳ.Ǘ=ѿ;Ү.Add(Ҳ);}}}}IEnumerable<int>Ҝ(П Р){for(int Į=0;Į<Р.Ǣ.Count;Į++){IMyShipController Ͱ=Р.Ǣ[Į];ǘ ǟ;if(!Р.Т.
TryGetValue(Ͱ.CubeGrid.EntityId,out ǟ)){continue;}ǟ.Ǘ.Ǣ.Add(Ͱ);if(Ͱ.CubeGrid==Me.CubeGrid){Р.й.Add(Ͱ);}yield return 1;}for(int Į=0;
Į<Р.Ҭ.Count;Į++){IMyProgrammableBlock ѕ=Р.Ҭ[Į];if(!і(ѕ)){continue;}ǘ ǟ;if(!Р.Т.TryGetValue(ѕ.CubeGrid.EntityId,out ǟ)){
continue;}ǖ ѿ=ǟ.Ǘ;ѿ.ǣ.Add(ѕ);bool ҳ=ї(ѕ);if(ҳ){ѿ.Ǧ=true;}if(ѿ.ǩ==null||ѕ.EntityId<ѿ.ǩ.EntityId){ѿ.ǩ=ѕ;ѿ.ǧ=Ҵ(ѕ);ѿ.Ǩ=ҳ;}yield
return 1;}}bool Ҵ(IMyProgrammableBlock ѕ){string Ȧ;if(!Ͳ(ѕ.CustomData,Ɉ,"Greedy",out Ȧ)){return true;}bool ğ;return bool.
TryParse(Ȧ,out ğ)?ğ:true;}IEnumerable<int>ҝ(П Р){HashSet<Ƥ>ҵ=new HashSet<Ƥ>();for(int Į=0;Į<Р.С.Count;Į++){IMyShipConnector Ϛ=Р.
С[Į];IMyShipConnector Ɨ=Ϛ.OtherConnector;if(Ɨ==null){continue;}Ƥ Ћ=new Ƥ(Ϛ.EntityId,Ɨ.EntityId);if(!ҵ.Add(Ћ)){continue;}ǘ
Ҷ;ǘ ҷ;if(!Р.Т.TryGetValue(Ϛ.CubeGrid.EntityId,out Ҷ)||!Р.Т.TryGetValue(Ɨ.CubeGrid.EntityId,out ҷ)){continue;}Р.Ш.Add(new
Ǭ{ƥ=Ϛ,Ʀ=Ɨ,Ǫ=Ҷ,ǫ=ҷ});yield return 1;}}void Ҡ(П Р){ǖ Ҹ=Р.ю;if(Ҹ==null){return;}List<ǖ>Ү=new List<ǖ>();Ҹ.Ǥ=true;Ү.Add(Ҹ);for
(int ΰ=0;ΰ<Ү.Count;ΰ++){ǖ ѿ=Ү[ΰ];for(int Į=0;Į<Р.Ш.Count;Į++){Ǭ Җ=Р.Ш[Į];ǖ Ɨ=null;if(Җ.Ǫ.Ǘ==ѿ){Ɨ=Җ.ǫ.Ǘ;}else if(Җ.ǫ.Ǘ==ѿ)
{Ɨ=Җ.Ǫ.Ǘ;}if(Ɨ==null||Ɨ.Ǥ){continue;}Ɨ.Ǥ=true;Ү.Add(Ɨ);}}}void ҡ(П Р,ǘ ҟ){ǖ Ҹ=Р.ю;if(Ҹ==null){return;}Ҹ.ǜ=true;ҹ(Ҹ,ҟ,0);
bool Һ;do{Һ=false;for(int Į=0;Į<Р.Ш.Count;Į++){Ǭ Җ=Р.Ш[Į];bool һ=Җ.Ǫ.Ǘ.ǜ;bool Ҽ=Җ.ǫ.Ǘ.ǜ;if(һ==Ҽ){continue;}ǘ Ѣ=һ?Җ.Ǫ:Җ.ǫ;ǘ Ȝ
=һ?Җ.ǫ:Җ.Ǫ;ǖ ҽ=Ȝ.Ǘ;if(ҽ.ǣ.Count>0){continue;}if(!ȟ.ȭ){continue;}ҽ.ǜ=true;int Ҿ=Ѣ.Ǜ==int.MaxValue?0:Ѣ.Ǜ;ҹ(ҽ,Ȝ,Ҿ);Һ=true;}}
while(Һ);foreach(KeyValuePair<long,ǘ>Ћ in Р.Т){Ћ.Value.ǜ=Ћ.Value.Ǘ.ǜ;}}void Ң(П Р){for(int Į=0;Į<Р.Ұ.Count;Į++){ǖ ѿ=Р.Ұ[Į];if
(ѿ==Р.ю||ѿ.ǜ||!ѿ.Ǥ||ѿ.ǩ==null){continue;}ǘ ҟ;if(!Р.Т.TryGetValue(ѿ.ǩ.CubeGrid.EntityId,out ҟ)){continue;}ҹ(ѿ,ҟ,0);}}void
ҹ(ǖ ѿ,ǘ ι,int ҿ){List<ǘ>Ү=new List<ǘ>();if(ι.Ǜ>ҿ){ι.Ǜ=ҿ;ι.Ǚ=null;ι.ǚ=null;}Ү.Add(ι);for(int ΰ=0;ΰ<Ү.Count;ΰ++){ǘ ǟ=Ү[ΰ];
for(int Į=0;Į<ǟ.Ǖ.Count;Į++){ǔ Җ=ǟ.Ǖ[Į];ǘ Ҳ=Җ.Ǡ(ǟ);if(Ҳ.Ǘ!=ѿ){continue;}int Ӏ=ǟ.Ǜ+1;if(Ӏ>=Ҳ.Ǜ){continue;}Ҳ.Ǜ=Ӏ;Ҳ.Ǚ=ǟ;Ҳ.ǚ=Җ;
Ү.Add(Ҳ);}}}IEnumerable<int>ң(П Р){for(int Į=0;Į<Р.Ұ.Count;Į++){ǖ ѿ=Р.Ұ[Į];if(!ѿ.Ǥ||ѿ.ǜ||ѿ.ǣ.Count==0){continue;}for(int
ĵ=0;ĵ<ѿ.Ǣ.Count;ĵ++){Р.к.Add(ѿ.Ǣ[ĵ]);}yield return 1;}}static ǘ Ҟ(Dictionary<long,ǘ>Ӂ,IMyCubeGrid Ŗ){ǘ ǟ;if(!Ӂ.
TryGetValue(Ŗ.EntityId,out ǟ)){ǟ=new ǘ(Ŗ);Ӂ.Add(Ŗ.EntityId,ǟ);}return ǟ;}static void ҭ(Dictionary<long,ǘ>Ӂ,IMyCubeGrid ӂ,
IMyCubeGrid Ӄ,IMyTerminalBlock Ǟ){ǘ ǂ=Ҟ(Ӂ,ӂ);ǘ ǃ=Ҟ(Ӂ,Ӄ);ǔ Җ=new ǔ(ǂ,ǃ,Ǟ);ǂ.Ǖ.Add(Җ);ǃ.Ǖ.Add(Җ);}sealed class П{public readonly List
<IMyTerminalBlock>э=new List<IMyTerminalBlock>();public readonly List<IMyShipController>Ǣ=new List<IMyShipController>(),й
=new List<IMyShipController>(),к=new List<IMyShipController>();public readonly List<IMyThrust>Ѫ=new List<IMyThrust>();
public readonly List<IMyMotorStator>ѧ=new List<IMyMotorStator>();public readonly List<IMyPistonBase>ҫ=new List<IMyPistonBase>(
);public readonly List<IMyGyro>Ґ=new List<IMyGyro>();public readonly List<IMyShipConnector>С=new List<IMyShipConnector>()
,У=new List<IMyShipConnector>();public readonly List<IMyLandingGear>Щ=new List<IMyLandingGear>();public readonly List<
IMyTimerBlock>Ы=new List<IMyTimerBlock>(),Ь=new List<IMyTimerBlock>(),Э=new List<IMyTimerBlock>();public readonly List<
IMyProgrammableBlock>Ҭ=new List<IMyProgrammableBlock>();public readonly Dictionary<long,F>G=new Dictionary<long,F>();public readonly
Dictionary<long,ǘ>Т=new Dictionary<long,ǘ>();public readonly List<ǖ>Ұ=new List<ǖ>();public readonly List<Ǭ>Ш=new List<Ǭ>();public
readonly List<g>ĥ=new List<g>(),л=new List<g>(),м=new List<g>(),н=new List<g>(),о=new List<g>(),п=new List<g>(),р=new List<g>(),
с=new List<g>();public readonly List<Ò>Ѩ=new List<Ò>(),г=new List<Ò>();public readonly List<H>т=new List<H>();public
readonly List<Ҏ>ф=new List<Ҏ>();public readonly List<ŝ>у=new List<ŝ>();public readonly List<ũ>д=new List<ũ>();public readonly
List<ǯ>Ц=new List<ǯ>();public readonly List<ǰ>Ъ=new List<ǰ>();public readonly List<Ž>ч=new List<Ž>();public ǖ ю;}sealed
class Ҏ{readonly List<double>Ĥ=new List<double>();public readonly IMyMotorStator Ò;public readonly List<g>ĥ=new List<g>();
public Ҏ(IMyMotorStator Ī){Ò=Ī;}public Vector3D É{get{return Ò.WorldMatrix.Up;}}public double ŉ(Vector3D m){m=n.o(m);if(Ò==
null||Ò.Closed||Ò.TopGrid==null||!Ò.IsFunctional||m.LengthSquared()<=p){return 0;}Ĥ.Clear();ӄ(0);for(int Į=0;Į<ĥ.Count;Į++){
g İ=ĥ[Į];if(!Ӆ(İ)){continue;}double ñ=n.Ē(m,İ.R,É);ӄ(ã(ñ));}double Õ=Ò.Angle;if(Î(Ò.LowerLimitRad)){ӄ(Ò.LowerLimitRad-Õ);
}if(Ï(Ò.UpperLimitRad)){ӄ(Ò.UpperLimitRad-Õ);}double Ŀ=0;double ӆ=double.MaxValue;for(int Į=0;Į<Ĥ.Count;Į++){double Ł=ã(Ĥ
[Į]);double ł=Ľ(m,Ł);double Ӈ=Math.Abs(Ł);if(ł>Ŀ+U||Math.Abs(ł-Ŀ)<=U&&Ӈ<ӆ){Ŀ=ł;ӆ=Ӈ;}}return Ŀ;}void ӄ(double ñ){ñ=ã(ñ);
for(int Į=0;Į<Ĥ.Count;Į++){if(Math.Abs(Ĥ[Į]-ñ)<=ē){return;}}Ĥ.Add(ñ);}double Ľ(Vector3D m,double ñ){double ł=0;for(int Į=0;
Į<ĥ.Count;Į++){g İ=ĥ[Į];if(!Ӆ(İ)){continue;}Vector3D ŗ=n.ë(İ.R,É,-ñ);double q=Vector3D.Dot(ŗ,m);if(q<=0){continue;}ł+=q*İ
.V;}return ł;}static bool Ӆ(g İ){if(İ==null||İ.Y||İ.E==null||İ.E.Closed||!İ.E.IsFunctional){return false;}return İ.V>U;}
double ã(double ø){ø=n.Ø(ø);bool Ĕ=Î(Ò.LowerLimitRad);bool ĕ=Ï(Ò.UpperLimitRad);if(!Ĕ&&!ĕ){return ø;}double Õ=Ò.Angle;double Ė
=double.NaN;double ė=double.MaxValue;for(int Ę=-2;Ę<=2;Ę++){double ę=ø+Ę*MathHelper.TwoPi;double Ě=Õ+ę;if(Ĕ&&Ě<Ò.
LowerLimitRad-ē){continue;}if(ĕ&&Ě>Ò.UpperLimitRad+ē){continue;}double ě=Math.Abs(ę);if(ě<ė){ė=ě;Ė=ę;}}if(!double.IsNaN(Ė)){return Ė;
}double Ĝ=Õ+ø;if(Ĕ){Ĝ=Math.Max(Ĝ,Ò.LowerLimitRad);}if(ĕ){Ĝ=Math.Min(Ĝ,Ò.UpperLimitRad);}return Ĝ-Õ;}static bool Î(double
ğ){return!double.IsNaN(ğ)&&!double.IsInfinity(ğ)&&ğ>-1e20;}static bool Ï(double ğ){return!double.IsNaN(ğ)&&!double.
IsInfinity(ğ)&&ğ<1e20;}}readonly List<Ǭ>х=new List<Ǭ>();readonly List<IMyShipConnector>ц=new List<IMyShipConnector>();readonly
Dictionary<long,long>ӈ=new Dictionary<long,long>();readonly Dictionary<long,bool>Ӊ=new Dictionary<long,bool>();readonly List<Ҏ>ʏ=
new List<Ҏ>();readonly List<g>ʎ=new List<g>();void Ȋ(){ϵ=true;}void Џ(){if(Ј==null){if(!ϵ){return;}ϵ=false;Ј=Ҥ().
GetEnumerator();}int ӊ=Runtime.MaxInstructionCount;int Ӌ=Math.Max(1000,ӊ*3/4);int ӌ=0;while(Ј!=null&&Runtime.CurrentInstructionCount<
Ӌ&&ӌ<512){ӌ++;if(Ј.MoveNext()){continue;}Ј.Dispose();Ј=null;if(ϵ){ϵ=false;Ј=Ҥ().GetEnumerator();}}}static double ʍ(List<g
>Ӎ,Vector3D m){m=n.o(m);if(m.LengthSquared()<=p){return 0;}double ł=0;for(int Į=0;Į<Ӎ.Count;Į++){g İ=Ӎ[Į];if(İ==null||İ.Y
||İ.E==null||İ.E.Closed||!İ.E.IsFunctional){continue;}double q=Vector3D.Dot(İ.R,m);if(q<=0){continue;}ł+=q*İ.V;}return ł;}
bool ʟ{get{return Ȏ==ƅ.ƃ?ɱ.Ư:Ȍ;}}bool ǻ{get{return Ȏ==ƅ.ƃ?ɱ.Ɖ:Ȕ;}}double ʭ{get{return Ȏ==ƅ.ƃ?ɱ.Ʊ:ȝ;}}double ʝ{get{if(Ȏ==ƅ.ƃ)
{return MathHelper.Clamp(ɱ.Ʋ,0,1);}if(ȟ.Ƞ.Count==0){return 0;}return MathHelper.Clamp(ȟ.Ƞ[MathHelper.Clamp(Ȣ,0,ȟ.Ƞ.Count-
1)],0,1);}}void ǽ(string Ǹ,bool ǹ){ϲ=Ǹ??string.Empty;ϴ=ǹ;ϓ=true;}void ӎ(){ϲ=string.Empty;ϴ=false;}void ȍ(bool i){Ȍ=i;ӏ(i)
;}void ӏ(bool i){for(int Į=0;Į<ͺ.Count;Į++){IMyShipController Ͱ=ͺ[Į];if(Ͱ==null||Ͱ.Closed||!Ͱ.IsFunctional){continue;}if(
Ͱ.DampenersOverride==i){continue;}Ͱ.DampenersOverride=i;}}void ɞ(){if(Ȏ==ƅ.ƃ){ӏ(ɱ.Ư);return;}if(!Ͽ){ӏ(Ȍ);return;}if(ɡ==
null||ɡ.Closed){return;}bool Ӑ=ɡ.DampenersOverride;if(Ӑ==Ȍ){return;}Ȍ=Ӑ;ӏ(Ȍ);}void ȕ(bool i){if(i&&!Ȕ){ӑ();}Ȕ=i;if(!Ȕ){ʾ(ƌ.Ɖ
);Ӓ(C.Ɖ);}ɝ();ӓ();}void ȏ(){ȕ(!Ȕ);}void ӑ(){if(ɡ==null||ɡ.Closed){ȝ=0;Ȟ=false;return;}Vector3D ʓ=ɡ.GetShipVelocities().
LinearVelocity;ȝ=Vector3D.Dot(ʓ,ɡ.WorldMatrix.Forward);Ȟ=true;}void ʤ(){if(Ȟ){return;}ӑ();}void Ș(double Ö){ʤ();ȝ+=Ö;ǽ(
"Cruise target: "+ȝ.ToString("0.###")+" m/s",false);}bool b(long Ϧ){Ə Ȓ;if(!Ϻ.TryGetValue(Ϧ,out Ȓ)){return false;}return Ȓ.ƍ&&Ȓ.Ǝ!=ƌ.M;}
bool Ӕ(long Ϧ,out Ə Ȓ){return Ϻ.TryGetValue(Ϧ,out Ȓ);}void ˀ(g İ,ƌ ӕ){if(İ==null){return;}ˀ(İ.E,ӕ);}void ˀ(IMyThrust d,ƌ ӕ){
if(d==null||d.Closed||ӕ==ƌ.M){return;}Ə Ȓ;if(!Ϻ.TryGetValue(d.EntityId,out Ȓ)){Ȓ=new Ə{ƍ=d.Enabled,Ǝ=ƌ.M};Ϻ.Add(d.EntityId
,Ȓ);}Ȓ.Ǝ|=ӕ;if(d.Enabled){d.Enabled=false;}if((Ȓ.Ǝ&ƌ.Ɗ)!=0){Ϣ[d.EntityId]=Ȓ.ƍ;}}void y(g İ){if(İ==null){return;}y(İ.E);}
void y(IMyThrust d){if(d==null||d.Closed){return;}Ə Ȓ;if(!Ϻ.TryGetValue(d.EntityId,out Ȓ)){return;}if(Ȓ.ƍ&&!d.Enabled){d.
Enabled=true;}}void ʾ(g İ,ƌ ӕ){if(İ==null){return;}ʾ(İ.Q,İ.E,ӕ);}void ʾ(long Ϧ,IMyThrust d,ƌ ӕ){Ə Ȓ;if(!Ϻ.TryGetValue(Ϧ,out Ȓ))
{return;}Ȓ.Ǝ&=~ӕ;if((ӕ&ƌ.Ɗ)!=0){Ϣ.Remove(Ϧ);}if(Ȓ.Ǝ!=ƌ.M){if(d!=null&&!d.Closed&&d.Enabled){d.Enabled=false;}return;}if(d
!=null&&!d.Closed){d.Enabled=Ȓ.ƍ;}Ϻ.Remove(Ϧ);}void ʾ(ƌ ӕ){if(Ϻ.Count==0){return;}List<long>Ӗ=new List<long>(Ϻ.Keys);for(
int Į=0;Į<Ӗ.Count;Į++){long Ϧ=Ӗ[Į];IMyThrust d=ӗ(Ϧ);ʾ(Ϧ,d,ӕ);}}IMyThrust ӗ(long Ϧ){for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į];if(İ
.Q==Ϧ){return İ.E;}}return null;}void Ϥ(g İ){if(İ==null){return;}Ə Ȓ;if(!Ϻ.TryGetValue(İ.Q,out Ȓ)){return;}if(Ȓ.Ǝ==ƌ.M){Ϻ
.Remove(İ.Q);return;}if(İ.E.Enabled){İ.E.Enabled=false;}}void ш(HashSet<long>Ә){if(Ϻ.Count==0){return;}List<long>ә=new
List<long>();foreach(KeyValuePair<long,Ə>Ћ in Ϻ){if(!Ә.Contains(Ћ.Key)){ә.Add(Ћ.Key);}}for(int Į=0;Į<ә.Count;Į++){long Ϧ=ә[Į
];Ϻ.Remove(Ϧ);Ϣ.Remove(Ϧ);}}void ɝ(){bool Ӛ=Ȏ==ƅ.ƃ;bool ӛ=ǻ;for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į];bool Ӝ=İ.E.CubeGrid==Me.
CubeGrid;bool ӝ=!İ.Y&&(ȟ.Ȭ||İ.a);bool Ӟ=Ӛ&&Ӝ&&ӝ;bool Ӡ=ӛ&&Ӝ&&ӟ(İ)&&ӝ;İ.N(C.ƃ,Ӟ);İ.N(C.Ɖ,Ӡ);if(!Ӡ){ʾ(İ,ƌ.Ɖ);}}for(int Į=0;Į<ˁ.
Count;Į++){ũ Ҕ=ˁ[Į];bool Ӝ=Ҕ.E.CubeGrid==Me.CubeGrid;bool ӡ=!Ҕ.Y&&(ȟ.Ȭ||Ҕ.a);Ҕ.N(C.ƃ,Ӛ&&Ӝ&&ӡ);}ω();}void Ӓ(C h){for(int Į=0;Į
<ʆ.Count;Į++){ʆ[Į].N(h,false);}for(int Į=0;Į<ˁ.Count;Į++){ˁ[Į].N(h,false);}}bool ӟ(g İ){if(İ==null||ɡ==null||İ.E.CubeGrid
!=Me.CubeGrid){return false;}return Vector3D.Dot(n.o(İ.R),ɡ.WorldMatrix.Backward)>=Ķ;}void ω(){ʿ.Clear();if(ɡ==null){
return;}for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į];if(ӟ(İ)){ʿ.Add(İ);}}ӓ();}void щ(){Ͽ=false;for(int Į=0;Į<ʆ.Count;Į++){g İ=ʆ[Į];if(İ
.E.CubeGrid==Me.CubeGrid){Ͽ=true;break;}}ω();}void ӓ(){Ǽ=string.Empty;if(!ǻ||ʿ.Count==0){return;}if(ȟ.Ȭ){bool Ӣ=true;for(
int Į=0;Į<ʿ.Count;Į++){if(!ʿ[Į].Y){Ӣ=false;break;}}if(Ӣ){Ǽ="Cruise cannot control main-grid "+"reverse thrusters; all are "
+ȟ.Ȳ+".";}return;}bool ӣ=false;for(int Į=0;Į<ʿ.Count;Į++){g İ=ʿ[Į];if(!İ.Y&&İ.a){ӣ=true;break;}}if(!ӣ){Ǽ=
"Cruise cannot control main-grid "+"reverse thrusters; add "+ȟ.ȱ+".";}}void ϒ(ƅ ϐ,ƅ Ϗ){if(ϐ==ƅ.ƃ&&Ϗ!=ƅ.ƃ){Ӓ(C.ƃ);ӏ(Ȍ);}if(ϐ!=ƅ.ƃ&&Ϗ==ƅ.ƃ){ӏ(ɱ.Ư);}ɝ();}
void ɪ(bool Ő){І.Clear();І.AppendLine(ȿ).Append("v").AppendLine(ˢ).AppendLine();І.Append("Mode: ").AppendLine(ǒ(Ȏ));І.Append
("Controller: ").AppendLine(ɡ!=null?ɡ.CustomName:"NONE");Ӥ(І);І.Append("Nacelles: ").AppendLine(ʂ.Count.ToString());І.
Append("Controlled thrust: ").Append((ʅ/1000.0).ToString("0.##")).AppendLine(" kN");І.Append("Capacity F/B: ").Append((ʇ.Ɛ/
1000.0).ToString("0.##")).Append("/").Append((ʇ.Ƒ/1000.0).ToString("0.##")).AppendLine(" kN");І.Append("Residual: ").Append((ʵ
.Length()/1000.0).ToString("0.##")).AppendLine(" kN");І.Append("Gyros: ").AppendLine(ˁ.Count.ToString());if(Ȏ==ƅ.ƃ){І.
Append("Heartbeat age: ").Append(ɥ).AppendLine("/2");}if(!string.IsNullOrEmpty(Ǽ)){І.AppendLine().Append("WARNING: ").
AppendLine(Ǽ);}if(!string.IsNullOrEmpty(ϲ)){І.AppendLine().Append(ϴ?"WARNING: ":"Command: ").AppendLine(ϲ);}І.Append("Runtime: ").
Append(Runtime.LastRunTimeMs.ToString("0.###")).Append(" ms | avg ").Append(Љ.ƺ.ToString("0.###")).Append(" | max ").
AppendLine(Љ.ƻ.ToString("0.###"));І.Append("Instructions: ").Append(Runtime.CurrentInstructionCount).Append("/").AppendLine(
Runtime.MaxInstructionCount.ToString());Echo(І.ToString());if(!Ő&&Ѕ.Count==0){return;}Ї.Clear();Ї.AppendLine(
"VECTOR THRUST REDUX").Append("MODE  ").AppendLine(ǒ(Ȏ).ToUpperInvariant());ӥ(Ї);Ї.Append("VECTORS ").AppendLine(ʂ.Count.ToString()).Append(
"THRUST ").Append((ʅ/1000.0).ToString("0.0")).AppendLine(" kN").Append("ERROR ").Append((ʵ.Length()/1000.0).ToString("0.0")).
AppendLine(" kN");if(!string.IsNullOrEmpty(Ǽ)){Ї.Append("WARN ").AppendLine(Ǽ);}if(!string.IsNullOrEmpty(ϲ)){Ї.Append(ϴ?"WARN ":
"CMD  ").AppendLine(ϲ);}string Ӧ=Ї.ToString();for(int Į=0;Į<Ѕ.Count;Į++){Ѕ[Į].ſ(Ӧ);}}void Ӥ(StringBuilder ɖ){if(Ȏ!=ƅ.ƃ){ɖ.
Append("Dampeners: ").AppendLine(Ȍ?"ON":"OFF");ɖ.Append("Cruise: ").Append(Ȕ?"ON":"OFF");if(Ȟ){ɖ.Append(" @ ").Append(ȝ.
ToString("0.###")).Append(" m/s");}ɖ.AppendLine();ӧ(ɖ);return;}ɖ.Append("Dampeners: local ").Append(Ȍ?"ON":"OFF").Append(
" | master ").Append(ɱ.Ư?"ON":"OFF").Append(" | effective ").AppendLine(ʟ?"ON":"OFF");ɖ.Append("Cruise: local ").Append(Ȕ?"ON":"OFF"
);if(Ȟ){ɖ.Append(" @ ").Append(ȝ.ToString("0.###"));}ɖ.Append(" | master ").Append(ɱ.Ɖ?"ON":"OFF").Append(" @ ").Append(ɱ
.Ʊ.ToString("0.###")).AppendLine(" m/s");ɖ.Append("Gear: local ").Append(Ȣ+1).Append("/").Append(ȟ.Ƞ.Count).Append(
" | master ").Append(ɱ.Ƴ+1).Append("/").Append(ɱ.ƴ).Append(" (").Append((ɱ.Ʋ*100).ToString("0.##")).AppendLine("%)");}void ӥ(
StringBuilder ɖ){if(Ȏ!=ƅ.ƃ){ɖ.Append("DAMP  ").AppendLine(Ȍ?"ON":"OFF").Append("CRUISE ").Append(Ȕ?"ON":"OFF");if(Ȟ){ɖ.Append(" ").
Append(ȝ.ToString("0.##")).Append("m/s");}ɖ.AppendLine().Append("GEAR  ").Append(Ȣ+1).Append("/").AppendLine(ȟ.Ƞ.Count.
ToString());return;}ɖ.Append("DAMP  L:").Append(Ȍ?"ON":"OFF").Append(" M:").AppendLine(ɱ.Ư?"ON":"OFF").Append("CRUISE L:").
Append(Ȕ?"ON":"OFF").Append(" M:").Append(ɱ.Ɖ?"ON":"OFF").Append(" ").Append(ɱ.Ʊ.ToString("0.##")).AppendLine("m/s").Append(
"GEAR  L:").Append(Ȣ+1).Append("/").Append(ȟ.Ƞ.Count).Append(" M:").Append(ɱ.Ƴ+1).Append("/").AppendLine(ɱ.ƴ.ToString());}void ӧ(
StringBuilder ɖ){int ȡ=ȟ.Ƞ.Count;int Ө=ȡ>0?MathHelper.Clamp(Ȣ,0,ȡ-1):0;double ȧ=ȡ>0?ȟ.Ƞ[Ө]*100:0;ɖ.Append("Gear: ").Append(Ө+1).
Append("/").Append(ȡ).Append(" (").Append(ȧ.ToString("0.##")).AppendLine("%)");}readonly List<IMyTerminalBlock>ө=new List<
IMyTerminalBlock>(),Ӫ=new List<IMyTerminalBlock>();readonly List<IMyBlockGroup>ӫ=new List<IMyBlockGroup>();void ə(){Ɯ ӭ=Ӭ();if(!Ϸ){ϼ=ӭ;Ϸ
=true;return;}if(ӭ==ϼ){return;}ϼ=ӭ;Ȋ();}Ɯ Ӭ(){Ɯ ӭ=new Ɯ();ө.Clear();GridTerminalSystem.GetBlocks(ө);for(int Į=0;Į<ө.Count
;Į++){IMyTerminalBlock d=ө[Į];int ӯ=Ӯ(d);if(ӯ==0){continue;}ulong ӱ=Ӱ;ӱ=Ӳ(ӱ,unchecked((ulong)d.EntityId));ӱ=Ӳ(ӱ,unchecked
((ulong)d.CubeGrid.EntityId));ӱ=Ӳ(ӱ,unchecked((ulong)ӯ));ӱ=Ӳ(ӱ,d.CubeGrid.IsStatic?1UL:0UL);F e=љ(d.CustomName)|љ(d.
CustomData);ӱ=Ӳ(ӱ,unchecked((ulong)(int)e));IMyMotorStator Ī=d as IMyMotorStator;if(Ī!=null){ӱ=Ӳ(ӱ,Ī.TopGrid!=null?unchecked((
ulong)Ī.TopGrid.EntityId):0UL);ӱ=Ӳ(ӱ,ӳ(Ī.LowerLimitRad));ӱ=Ӳ(ӱ,ӳ(Ī.UpperLimitRad));}IMyPistonBase Ҫ=d as IMyPistonBase;if(Ҫ!=
null){ӱ=Ӳ(ӱ,Ҫ.TopGrid!=null?unchecked((ulong)Ҫ.TopGrid.EntityId):0UL);}IMyShipConnector Ϛ=d as IMyShipConnector;if(Ϛ!=null){
ӱ=Ӳ(ӱ,Ϛ.OtherConnector!=null?unchecked((ulong)Ϛ.OtherConnector.EntityId):0UL);}IMyProgrammableBlock ѕ=d as
IMyProgrammableBlock;if(ѕ!=null){bool Ӵ=і(ѕ);ӱ=Ӳ(ӱ,Ӵ?1UL:0UL);if(Ӵ){ӱ=Ӳ(ӱ,ї(ѕ)?1UL:0UL);ӱ=Ӳ(ӱ,Ҵ(ѕ)?1UL:0UL);}}ӵ(ref ӭ,ӱ,d.EntityId);}Ӷ(ref ӭ
);return ӭ;}void Ӷ(ref Ɯ ӭ){ӫ.Clear();GridTerminalSystem.GetBlockGroups(ӫ);for(int Į=0;Į<ӫ.Count;Į++){IMyBlockGroup ҧ=ӫ[Į
];F Ҩ=љ(ҧ.Name);if(Ҩ==F.M){continue;}Ӫ.Clear();ҧ.GetBlocks(Ӫ);ulong ӷ=Ӱ;ӷ=Ӳ(ӷ,Ӹ(ҧ.Name));ӷ=Ӳ(ӷ,unchecked((ulong)(int)Ҩ));
ulong ӹ=0;ulong Ӻ=0;for(int ĵ=0;ĵ<Ӫ.Count;ĵ++){ulong ӻ=Ӳ(Ӱ,unchecked((ulong)Ӫ[ĵ].EntityId));ӹ^=Ӽ(ӻ,(int)(Ӫ[ĵ].EntityId&63));Ӻ
+=ӻ*ӽ;}ӷ=Ӳ(ӷ,ӹ);ӷ=Ӳ(ӷ,Ӻ);long Ӿ=unchecked((long)Ӹ(ҧ.Name));ӵ(ref ӭ,ӷ,Ӿ);}}static void ӵ(ref Ɯ ӭ,ulong ӱ,long ӿ){ӭ.Ɲ++;ӭ.ƞ
^=Ӽ(ӱ,(int)(ӿ&63));ӭ.Ɵ+=ӱ*ӽ;}static int Ӯ(IMyTerminalBlock d){if(d is IMyMotorStator){return 1;}if(d is IMyPistonBase){
return 2;}if(d is IMyThrust){return 3;}if(d is IMyGyro){return 4;}if(d is IMyShipController){return 5;}if(d is
IMyShipConnector){return 6;}if(d is IMyLandingGear){return 7;}if(d is IMyProgrammableBlock){return 8;}if(d is IMyTimerBlock){return 9;}
if(d is IMyTextPanel||d is IMyTextSurfaceProvider){return 10;}return 0;}static ulong ӳ(double ğ){if(double.IsNaN(ğ)){
return ulong.MaxValue;}if(double.IsPositiveInfinity(ğ)){return ulong.MaxValue-1;}if(double.IsNegativeInfinity(ğ)){return ulong
.MaxValue-2;}return unchecked((ulong)(uint)ğ.GetHashCode());}static ulong Ӹ(string ž){ulong ơ=Ӱ;if(string.IsNullOrEmpty(ž
)){return ơ;}for(int Į=0;Į<ž.Length;Į++){char Ȫ=char.ToUpperInvariant(ž[Į]);ơ^=Ȫ;ơ*=ӽ;}return ơ;}static ulong Ӳ(ulong ơ,
ulong ğ){ơ^=ğ;ơ*=ӽ;ơ^=ğ>>32;ơ*=ӽ;return ơ;}static ulong Ӽ(ulong ğ,int К){К&=63;if(К==0){return ğ;}return ğ<<К|ğ>>(64-К);}
const ulong Ӱ=14695981039346656037UL,ӽ=1099511628211UL;void ɜ(){HashSet<long>Ԁ=new HashSet<long>();for(int Į=0;Į<ц.Count;Į++)
{IMyShipConnector Ϛ=ц[Į];if(Ϛ==null||Ϛ.Closed){continue;}long ԁ=Ϛ.OtherConnector!=null?Ϛ.OtherConnector.EntityId:0;long Ԃ
;if(!ӈ.TryGetValue(Ϛ.EntityId,out Ԃ)||Ԃ!=ԁ){ӈ[Ϛ.EntityId]=ԁ;Ȋ();}Ԁ.Add(Ϛ.EntityId);}ԃ(Ԁ);HashSet<long>Ԅ=new HashSet<long>
();for(int Į=0;Į<ϗ.Count;Į++){IMyLandingGear ԅ=ϗ[Į].ǭ;if(ԅ==null||ԅ.Closed){continue;}bool Ԇ;if(!Ӊ.TryGetValue(ԅ.EntityId
,out Ԇ)||Ԇ!=ԅ.IsLocked){Ӊ[ԅ.EntityId]=ԅ.IsLocked;Ȋ();}Ԅ.Add(ԅ.EntityId);}ԇ(Ԅ);}void ԃ(HashSet<long>Ԉ){if(ӈ.Count==0){
return;}List<long>ԉ=new List<long>();foreach(KeyValuePair<long,long>Ћ in ӈ){if(!Ԉ.Contains(Ћ.Key)){ԉ.Add(Ћ.Key);}}for(int Į=0;
Į<ԉ.Count;Į++){ӈ.Remove(ԉ[Į]);}}void ԇ(HashSet<long>Ԉ){if(Ӊ.Count==0){return;}List<long>ԉ=new List<long>();foreach(
KeyValuePair<long,bool>Ћ in Ӊ){if(!Ԉ.Contains(Ћ.Key)){ԉ.Add(Ћ.Key);}}for(int Į=0;Į<ԉ.Count;Į++){Ӊ.Remove(ԉ[Į]);}}}
