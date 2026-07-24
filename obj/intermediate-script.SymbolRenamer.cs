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

    sealed class g
    {
        readonly Program A;

        float B =
            float.NaN;

        C D;

        public readonly IMyThrust E;
        public readonly F G;

        public H I;

        public bool J;

        public double K;

        public C L
        {
            get
            {
                return D;
            }
        }

        public bool P
        {
            get
            {
                return D !=
                       C.M;
            }

            set
            {
                N(
                    C.O,
                    value);
            }
        }

        public long Q
        {
            get
            {
                return E.EntityId;
            }
        }

        public Vector3D R
        {
            get
            {
                return E
                    .WorldMatrix
                    .Backward;
            }
        }

        public Vector3D S
        {
            get
            {
                return E
                    .WorldMatrix
                    .Forward;
            }
        }

        public double V
        {
            get
            {
                if (E == null ||
                    E.Closed ||
                    !E.IsFunctional)
                {
                    return 0;
                }

                double T =
                    E
                        .MaxEffectiveThrust;

                return T >
                       U
                    ? T
                    : 0;
            }
        }

        public Vector3D W
        {
            get
            {
                if (E == null ||
                    E.Closed ||
                    !E.IsFunctional)
                {
                    return Vector3D.Zero;
                }

                return R *
                       E.CurrentThrust;
            }
        }

        public bool Y
        {
            get
            {
                return (G &
                        F.X) != 0;
            }
        }

        public bool a
        {
            get
            {
                return (G &
                        F.Z) != 0;
            }
        }

        public bool c
        {
            get
            {
                if (E == null ||
                    E.Closed ||
                    !E.IsFunctional ||
                    Y ||
                    V <=
                        U)
                {
                    return false;
                }

                return E.Enabled ||
                       A
                           .b(
                               Q);
            }
        }

        public g(
            IMyThrust d,
            Program A,
            F e,
            bool f)
        {
            E = d;
            this.A = A;
            G = e;

            if (f &&
                !Y)
            {
                D =
                    C.O;
            }
        }

        public void N(
            C h,
            bool i)
        {
            if (Y)
            {
                D =
                    C.M;

                return;
            }

            if (i)
            {
                D |= h;
                return;
            }

            D &= ~h;

            if (D ==
                C.M)
            {
                j();
            }
        }

        public bool k(
            C h)
        {
            return (D &
                    h) != 0;
        }

        public void l()
        {
            K = 0;
        }

        public double r(
            Vector3D m)
        {
            if (!c)
            {
                return 0;
            }

            m =
                n.o(
                    m);

            if (m
                    .LengthSquared() <=
                p)
            {
                return 0;
            }

            double q =
                Vector3D.Dot(
                    R,
                    m);

            return q > 0
                ? q *
                  V
                : 0;
        }

        public double x(
            ref Vector3D s)
        {
            if (!P ||
                !c)
            {
                return 0;
            }

            Vector3D t =
                n.o(
                    R);

            double u =
                Vector3D.Dot(
                    s,
                    t);

            if (u <=
                U)
            {
                return 0;
            }

            double v =
                V -
                K;

            if (v <=
                U)
            {
                return 0;
            }

            double w =
                Math.Min(
                    u,
                    v);

            K +=
                w;

            s -=
                t * w;

            return w;
        }

        public void º()
        {
            if (!P ||
                E == null ||
                E.Closed)
            {
                return;
            }

            if (K >
                U)
            {
                A
                    .y(
                        this);
            }

            double z =
                V;

            float ª =
                z >
                        U
                    ? (float)
                        MathHelper.Clamp(
                            K /
                            z,
                            0,
                            1)
                    : 0;

            if (!float.IsNaN(
                    B) &&
                Math.Abs(
                    ª -
                    B) <
                        µ &&
                Math.Abs(
                    E
                        .ThrustOverridePercentage -
                    ª) <
                        µ)
            {
                return;
            }

            E
                .ThrustOverridePercentage =
                ª;

            B =
                ª;
        }

        public void j()
        {
            K = 0;

            if (E == null ||
                E.Closed)
            {
                return;
            }

            if (Math.Abs(
                    E
                        .ThrustOverridePercentage) >
                U)
            {
                E
                    .ThrustOverridePercentage = 0;
            }

            B = 0;
        }

        public void À()
        {
            j();

            D =
                C.M;
        }
    }


    sealed class Ò
    {
        readonly Program A;

        double Á =
            double.NaN,Â =
            double.NaN,Ã,Ä;
        double Å;
        bool Æ,Ç;

        C D;

        public readonly IMyMotorStator E;
        public readonly F G;

        public H I;

        public C L
        {
            get
            {
                return D;
            }
        }

        public bool P
        {
            get
            {
                return D !=
                       C.M;
            }

            set
            {
                N(
                    C.O,
                    value);
            }
        }

        public long Q
        {
            get
            {
                return E.EntityId;
            }
        }

        public bool Y
        {
            get
            {
                return (G &
                        F.X) != 0;
            }
        }

        public bool a
        {
            get
            {
                return (G &
                        F.Z) != 0;
            }
        }

        public bool È
        {
            get
            {
                return E
                    .BlockDefinition
                    .SubtypeId
                    .IndexOf(
"Hinge",
                        StringComparison
                            .OrdinalIgnoreCase) >= 0;
            }
        }

        public Vector3D É
        {
            get
            {
                return E
                    .WorldMatrix
                    .Up;
            }
        }

        public double Ê
        {
            get
            {
                return Ã;
            }
        }

        public double Ë
        {
            get
            {
                return Å;
            }
        }

        public bool Ñ
        {
            get
            {
                if (E == null ||
                    E.Closed ||
                    E.Top == null ||
                    !E.IsFunctional ||
                    !E.Enabled ||
                    E.RotorLock)
                {
                    return false;
                }

                double Ì =
                    E.LowerLimitRad;

                double Í =
                    E.UpperLimitRad;

                return !Î(
                           Ì) ||
                       !Ï(
                           Í) ||
                       Math.Abs(
                           Í -
                           Ì) >
                       Ð;
            }
        }

        public Ò(
            IMyMotorStator d,
            Program A,
            F e,
            bool f)
        {
            E = d;
            this.A = A;
            G = e;

            if (f &&
                !Y)
            {
                D =
                    C.O;
            }
        }

        public void N(
            C h,
            bool i)
        {
            if (Y)
            {
                D =
                    C.M;

                Ó();
                return;
            }

            if (i)
            {
                D |= h;
                return;
            }

            D &= ~h;

            if (D ==
                C.M)
            {
                Ó();
            }
        }

        public bool k(
            C h)
        {
            return (D &
                    h) != 0;
        }

        public void Ù(
            double Ô)
        {
            if (E == null ||
                E.Closed)
            {
                Ã = 0;
                Â =
                    double.NaN;

                return;
            }

            double Õ =
                E.Angle;

            if (double.IsNaN(
                    Â) ||
                Ô <=
                    p)
            {
                Ã = 0;
            }
            else
            {
                double Ö =
                    Õ -
                    Â;

                Ö =
                    n.Ø(
                        Ö);

                Ã =
                    Ö / Ô;
            }

            Â =
                Õ;
        }

        public double Û(
            Vector3D Ú)
        {
            return Û(
                Ú,
                1,
                A.Ü);
        }

        public double Û(
            Vector3D Ú,
            double Ý,
            double Ô)
        {
            if (!P ||
                !Ñ ||
                I == null ||
                Ú
                    .LengthSquared() <=
                p)
            {
                Ó();
                return 0;
            }

            Þ();
            Ù(Ô);

            Vector3D à =
                I
                    .ß;

            double â =
                á(
                    Ú,
                    à);

            double ä =
                ã(
                    â);

            Ý =
                MathHelper.Clamp(
                    Ý,
                    0,
                    1);

            double ç =
                å +
                (æ -
                 å) *
                Ý;

            double é =
                ä -
                Ã *
                Math.Max(
                    Ô,
                    è);

            if (ä *
                é <= 0)
            {
                é = 0;
            }

            ê(
                é *
                ç);

            Vector3D ì =
                n.ë(
                    à,
                    É,
                    -ä);

            return n.í(
                ì,
                Ú);
        }

        public void ò(
            î ï,
            double Ý,
            double Ô)
        {
            if (ï == null ||
                ï.I !=
                    I ||
                !P ||
                !Ñ)
            {
                Ó();
                return;
            }

            Þ();
            Ù(Ô);

            double ñ =
                ï.ð;

            Ý =
                MathHelper.Clamp(
                    Ý,
                    0,
                    1);

            double ç =
                å +
                (æ -
                 å) *
                Ý;

            double é =
                ñ -
                Ã *
                Math.Max(
                    Ô,
                    è);

            if (ñ *
                é <= 0)
            {
                é = 0;
            }

            ê(
                é *
                ç);
        }

        public bool ô(
            Vector3D Ú,
            out double ñ,
            out double ó)
        {
            ñ = 0;
            ó = 0;

            if (!Ñ ||
                I == null)
            {
                return false;
            }

            Vector3D à =
                I
                    .ß;

            if (à
                    .LengthSquared() <=
                    p ||
                Ú
                    .LengthSquared() <=
                    p)
            {
                return false;
            }

            double â =
                á(
                    Ú,
                    à);

            ñ =
                ã(
                    â);

            Vector3D ì =
                n.ë(
                    à,
                    É,
                    -ñ);

            ó =
                n.í(
                    ì,
                    Ú);

            return true;
        }

        public double ö(
            Vector3D Ú,
            Vector3D õ)
        {
            return ã(
                á(
                    Ú,
                    õ));
        }

        public double ù(
            double ø)
        {
            return ã(
                ø);
        }

        public void Ĉ(
            Vector3D ú,
            Vector3D û)
        {
            Æ = false;
            Ç = false;

            if (!P ||
                !Ñ ||
                I == null)
            {
                Ó();
                Ç = true;

                A
                    .ü
                    .Remove(Q);

                return;
            }

            double ñ;
            double ó;

            if (ú
                    .LengthSquared() >
                p)
            {
                Vector3D ý =
                    -n.o(
                        ú);

                if (ô(
                        ý,
                        out ñ,
                        out ó) &&
                    ó >=
                        þ)
                {
                    ÿ(
                        ñ);

                    return;
                }
            }

            Vector3D ā =
                I
                    .Ā();

            Vector3D Ă =
                E.GetPosition();

            Vector3D ă =
                ā -
                Ă;

            Vector3D Ą =
                û -
                Ă;

            Vector3D Ć =
                n.ą(
                    ă,
                    É);

            Vector3D ć =
                n.ą(
                    Ą,
                    É);

            if (Ć
                    .LengthSquared() <=
                    p ||
                ć
                    .LengthSquared() <=
                    p)
            {
                ÿ(
                    0);

                return;
            }

            ñ =
                á(
                    ć,
                    Ć);

            ñ =
                ã(
                    ñ);

            ÿ(
                ñ);
        }

        public void Ċ(
            double ĉ)
        {
            Ä =
                ĉ;

            if (Î(
                    E.LowerLimitRad))
            {
                Ä =
                    Math.Max(
                        Ä,
                        E
                            .LowerLimitRad);
            }

            if (Ï(
                    E.UpperLimitRad))
            {
                Ä =
                    Math.Min(
                        Ä,
                        E
                            .UpperLimitRad);
            }

            Æ = true;
            Ç = false;
        }

        public void Đ()
        {
            if (!Æ ||
                !P ||
                !Ñ)
            {
                Ó();
                Ç = true;
                return;
            }

            double ċ =
                Ä -
                E.Angle;

            if (!Č())
            {
                ċ =
                    n.Ø(
                        ċ);
            }

            double Ď =
                ċ *
                č;

            Ç =
                Math.Abs(
                    Ď) <=
                ď;

            ê(
                Ç
                    ? 0
                    : Ď);
        }

        public void Þ()
        {
            Æ = false;
            Ç = false;
        }

        public void Ó()
        {
            ê(0);
        }

        public void À()
        {
            Ó();

            D =
                C.M;
        }

        double á(
            Vector3D m,
            Vector3D đ)
        {
            double ñ =
                n.Ē(
                    m,
                    đ,
                    É);

            if (Math.Abs(
                    Math.Abs(
                        ñ) -
                    Math.PI) <=
                ē)
            {
                if (Å != 0)
                {
                    ñ =
                        Math.Abs(
                            ñ) *
                        Å;
                }
                else
                {
                    ñ =
                        Math.Abs(
                            ñ);
                }
            }

            return ñ;
        }

        void ÿ(
            double ñ)
        {
            Ä =
                E.Angle +
                ñ;

            if (Î(
                    E.LowerLimitRad))
            {
                Ä =
                    Math.Max(
                        Ä,
                        E
                            .LowerLimitRad);
            }

            if (Ï(
                    E.UpperLimitRad))
            {
                Ä =
                    Math.Min(
                        Ä,
                        E
                            .UpperLimitRad);
            }

            Æ = true;
            Ç = false;

            A
                .ü[
                    Q] =
                Ä;
        }

        double ã(
            double ø)
        {
            ø =
                n.Ø(
                    ø);

            bool Ĕ =
                Î(
                    E.LowerLimitRad);

            bool ĕ =
                Ï(
                    E.UpperLimitRad);

            if (!Ĕ &&
                !ĕ)
            {
                return ø;
            }

            double Õ =
                E.Angle;

            double Ė =
                double.NaN;

            double ė =
                double.MaxValue;

            for (int Ę = -2;
                Ę <= 2;
                Ę++)
            {
                double ę =
                    ø +
                    Ę *
                    MathHelper.TwoPi;

                double Ě =
                    Õ +
                    ę;

                if (Ĕ &&
                    Ě <
                        E
                            .LowerLimitRad -
                        ē)
                {
                    continue;
                }

                if (ĕ &&
                    Ě >
                        E
                            .UpperLimitRad +
                        ē)
                {
                    continue;
                }

                double ě =
                    Math.Abs(
                        ę);

                if (ě <
                    ė)
                {
                    ė =
                        ě;

                    Ė =
                        ę;
                }
            }

            if (!double.IsNaN(
                    Ė))
            {
                return Ė;
            }

            double Ĝ =
                Õ +
                ø;

            if (Ĕ)
            {
                Ĝ =
                    Math.Max(
                        Ĝ,
                        E
                            .LowerLimitRad);
            }

            if (ĕ)
            {
                Ĝ =
                    Math.Min(
                        Ĝ,
                        E
                            .UpperLimitRad);
            }

            return Ĝ -
                   Õ;
        }

        void ê(
            double ĝ)
        {
            if (E == null ||
                E.Closed)
            {
                return;
            }

            ĝ =
                MathHelper.Clamp(
                    ĝ,
                    -Ğ,
                    Ğ);

            if (Math.Abs(
                    ĝ) <=
                ď)
            {
                ĝ = 0;
            }

            if (ĝ != 0)
            {
                Å =
                    Math.Sign(
                        ĝ);
            }

            if (!double.IsNaN(
                    Á) &&
                Math.Abs(
                    Á -
                    ĝ) <
                        ď &&
                Math.Abs(
                    E
                        .TargetVelocityRad -
                    ĝ) <
                        ď)
            {
                return;
            }

            E.TargetVelocityRad =
                (float)ĝ;

            Á =
                ĝ;
        }

        bool Č()
        {
            return Î(
                       E
                           .LowerLimitRad) ||
                   Ï(
                       E
                           .UpperLimitRad);
        }

        static bool Î(
            double ğ)
        {
            return !double.IsNaN(
                       ğ) &&
                   !double.IsInfinity(
                       ğ) &&
                   ğ > -1e20;
        }

        static bool Ï(
            double ğ)
        {
            return !double.IsNaN(
                       ğ) &&
                   !double.IsInfinity(
                       ğ) &&
                   ğ < 1e20;
        }
    }


    sealed class H
    {
        sealed class Ģ
        {
            public Vector3D
                Ġ;

            public double
                ġ;
        }

        readonly Program A;

        readonly List<Ģ>
            ģ =
                new List<Ģ>();

        readonly List<double>
            Ĥ =
                new List<double>();

        public readonly Ò Ò;

        public readonly List<g> ĥ =
            new List<g>();

        public readonly List<IMyCubeGrid> Ħ =
            new List<IMyCubeGrid>();

        public Vector3D ħ =
            Vector3D.Zero,Ĩ =
            Vector3D.Zero;

        public double ĩ;

        public H(
            Ò Ī,
            Program A)
        {
            Ò = Ī;
            this.A = A;

            Ī.I = this;
        }

        public Vector3D É
        {
            get
            {
                return Ò.É;
            }
        }

        public Vector3D ß
        {
            get
            {
                IMyCubeGrid ī =
                    Ò.E.TopGrid;

                if (ī == null ||
                    ħ
                        .LengthSquared() <=
                    p)
                {
                    return Vector3D.Zero;
                }

                Vector3D ĭ =
                    n
                        .Ĭ(
                            ħ,
                            ī.WorldMatrix);

                return -n.o(
                    ĭ);
            }
        }

        public void ĸ()
        {
            ģ.Clear();

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                ĥ[Į]
                    .J =
                    false;
            }

            IMyCubeGrid ī =
                Ò.E.TopGrid;

            if (ī == null)
            {
                ħ =
                    Vector3D.Zero;

                ĩ = 0;
                return;
            }

            MatrixD į =
                ī.WorldMatrix;

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                g İ =
                    ĥ[Į];

                if (!İ.P ||
                    !İ.c)
                {
                    continue;
                }

                double ı =
                    İ
                        .V;

                if (ı <=
                    U)
                {
                    continue;
                }

                Vector3D ĳ =
                    n.o(
                        n
                            .Ĳ(
                                İ
                                    .S,
                                į));

                Ģ Ĵ =
                    null;

                for (int ĵ = 0;
                    ĵ <
                        ģ.Count;
                    ĵ++)
                {
                    if (Vector3D.Dot(
                            ģ[ĵ]
                                .Ġ,
                            ĳ) >=
                        Ķ)
                    {
                        Ĵ =
                            ģ[ĵ];

                        break;
                    }
                }

                if (Ĵ == null)
                {
                    Ĵ =
                        new Ģ
                        {
                            Ġ =
                                ĳ
                        };

                    ģ.Add(
                        Ĵ);
                }

                Ĵ.ġ +=
                    ı;
            }

            Ģ ķ =
                null;

            for (int Į = 0;
                Į <
                    ģ.Count;
                Į++)
            {
                if (ķ == null ||
                    ģ[Į]
                        .ġ >
                    ķ
                        .ġ)
                {
                    ķ =
                        ģ[Į];
                }
            }

            if (ķ == null)
            {
                ħ =
                    Vector3D.Zero;

                ĩ = 0;
                return;
            }

            ħ =
                ķ
                    .Ġ;

            ĩ =
                ķ
                    .ġ;

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                g İ =
                    ĥ[Į];

                if (!İ.P ||
                    !İ.c)
                {
                    continue;
                }

                Vector3D ĳ =
                    n.o(
                        n
                            .Ĳ(
                                İ
                                    .S,
                                į));

                İ
                    .J =
                    Vector3D.Dot(
                        ĳ,
                        ħ) >=
                    Ķ;
            }
        }

        public bool ň(
            Vector3D m,
            î Ĺ)
        {
            if (Ĺ == null)
            {
                return false;
            }

            Ĺ.ĺ();
            Ĺ.I = this;

            m =
                n.o(
                    m);

            if (!Ò.P ||
                !Ò.Ñ ||
                m
                    .LengthSquared() <=
                p)
            {
                return false;
            }

            Ĥ.Clear();

            Ļ(0);

            Vector3D ļ =
                ß;

            if (ļ
                    .LengthSquared() >
                p)
            {
                Ļ(
                    Ò
                        .ö(
                            m,
                            ļ));
            }

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                g İ =
                    ĥ[Į];

                if (!İ.P ||
                    !İ.c)
                {
                    continue;
                }

                Ļ(
                    Ò
                        .ö(
                            m,
                            İ
                                .R));
            }

            double Õ =
                Ò.E.Angle;

            double Ì =
                Ò.E
                    .LowerLimitRad;

            double Í =
                Ò.E
                    .UpperLimitRad;

            if (!double.IsNaN(Ì) &&
                !double.IsInfinity(Ì) &&
                Ì > -1e20)
            {
                Ļ(
                    Ì -
                    Õ);
            }

            if (!double.IsNaN(Í) &&
                !double.IsInfinity(Í) &&
                Í < 1e20)
            {
                Ļ(
                    Í -
                    Õ);
            }

            double ľ =
                Ľ(
                    m,
                    0);

            double Ŀ =
                -1;

            double ŀ = 0;

            for (int Į = 0;
                Į <
                    Ĥ.Count;
                Į++)
            {
                double Ł =
                    Ò.ù(
                        Ĥ[Į]);

                double ł =
                    Ľ(
                        m,
                        Ł);

                if (ł >
                    Ŀ +
                    U)
                {
                    Ŀ =
                        ł;

                    ŀ =
                        Ł;

                    continue;
                }

                if (Math.Abs(
                        ł -
                        Ŀ) >
                    U)
                {
                    continue;
                }

                double Ń =
                    Math.Abs(Ł);

                double ė =
                    Math.Abs(ŀ);

                if (Ń <
                    ė -
                    ē)
                {
                    ŀ =
                        Ł;

                    continue;
                }

                if (Math.Abs(
                        Ń -
                        ė) <=
                        ē &&
                    Ò
                        .Ë != 0 &&
                    Math.Sign(Ł) ==
                        Ò
                            .Ë)
                {
                    ŀ =
                        Ł;
                }
            }

            if (Ŀ <=
                U)
            {
                return false;
            }

            Ĺ.ð =
                ŀ;

            Ĺ
                .ń =
                Ŀ;

            Ĺ
                .Ņ =
                ľ;

            Vector3D ņ =
                n.ë(
                    ļ,
                    É,
                    -ŀ);

            Ĺ.Ň =
                n.í(
                    ņ,
                    m);

            return true;
        }

        public double ŉ(
            Vector3D m)
        {
            î ï =
                new î();

            return ň(
                       m,
                       ï)
                ? ï
                    .ń
                : 0;
        }

        public double Ŋ(
            Vector3D Ú)
        {
            Ĩ =
                Ú;

            if (Ú
                    .LengthSquared() <=
                p)
            {
                Ò.Ó();
                return 0;
            }

            î ï =
                new î();

            if (!ň(
                    Ú,
                    ï))
            {
                Ò.Ó();
                return 0;
            }

            double Ý =
                ï
                        .ń >
                    U
                    ? MathHelper.Clamp(
                        Ú
                            .Length() /
                        ï
                            .ń,
                        0,
                        1)
                    : 0;

            Ò.ò(
                ï,
                Ý,
                A.Ü);

            return ï.Ň;
        }

        public void Ŋ(
            î ï,
            double ŋ,
            double Ô)
        {
            if (ï == null ||
                ï.I != this ||
                ï
                        .ń <=
                    U)
            {
                Ĩ =
                    Vector3D.Zero;

                Ò.Ó();
                return;
            }

            double Ý =
                MathHelper.Clamp(
                    ŋ /
                    ï
                        .ń,
                    0,
                    1);

            Ò.ò(
                ï,
                Ý,
                Ô);
        }

        public double Œ(
            ref Vector3D s,
            Vector3D Ō,
            ref Vector3D ō)
        {
            double Ŏ = 0;

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                g İ =
                    ĥ[Į];

                if (!İ
                        .J)
                {
                    continue;
                }

                double ŏ =
                    İ
                        .x(
                            ref s);

                if (ŏ <=
                    U)
                {
                    continue;
                }

                Vector3D Ő =
                    İ
                        .R *
                    ŏ;

                Vector3D ő =
                    İ
                        .E
                        .GetPosition() -
                    Ō;

                ō +=
                    Vector3D.Cross(
                        ő,
                        Ő);

                Ŏ +=
                    ŏ;
            }

            return Ŏ;
        }

        public double œ(
            ref Vector3D s,
            Vector3D Ō,
            ref Vector3D ō)
        {
            double Ŏ = 0;

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                g İ =
                    ĥ[Į];

                if (İ
                        .J)
                {
                    continue;
                }

                double ŏ =
                    İ
                        .x(
                            ref s);

                if (ŏ <=
                    U)
                {
                    continue;
                }

                Vector3D Ő =
                    İ
                        .R *
                    ŏ;

                Vector3D ő =
                    İ
                        .E
                        .GetPosition() -
                    Ō;

                ō +=
                    Vector3D.Cross(
                        ő,
                        Ő);

                Ŏ +=
                    ŏ;
            }

            return Ŏ;
        }

        public Vector3D Ā()
        {
            if (Ħ.Count == 0)
            {
                return Ò
                           .E
                           .TopGrid != null
                    ? Ò
                        .E
                        .TopGrid
                        .WorldAABB
                        .Center
                    : Ò
                        .E
                        .GetPosition();
            }

            Vector3D Ŕ =
                Vector3D.Zero;

            int ŕ = 0;

            for (int Į = 0;
                Į < Ħ.Count;
                Į++)
            {
                IMyCubeGrid Ŗ =
                    Ħ[Į];

                if (Ŗ == null ||
                    Ŗ.Closed)
                {
                    continue;
                }

                Ŕ +=
                    Ŗ.WorldAABB.Center;

                ŕ++;
            }

            return ŕ > 0
                ? Ŕ /
                  ŕ
                : Ò
                    .E
                    .GetPosition();
        }

        void Ļ(
            double ñ)
        {
            ñ =
                Ò.ù(
                    ñ);

            for (int Į = 0;
                Į <
                    Ĥ.Count;
                Į++)
            {
                if (Math.Abs(
                        Ĥ[Į] -
                        ñ) <=
                    ē)
                {
                    return;
                }
            }

            Ĥ.Add(
                ñ);
        }

        double Ľ(
            Vector3D m,
            double ñ)
        {
            double ł = 0;

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                g İ =
                    ĥ[Į];

                if (!İ.P ||
                    !İ.c)
                {
                    continue;
                }

                Vector3D ŗ =
                    n.ë(
                        İ
                            .R,
                        É,
                        -ñ);

                double q =
                    Vector3D.Dot(
                        ŗ,
                        m);

                if (q <= 0)
                {
                    continue;
                }

                ł +=
                    q *
                    İ
                        .V;
            }

            return ł;
        }
    }

    sealed class ŝ
    {
        public readonly List<H> Ř =
            new List<H>();

        public Vector3D É
        {
            get
            {
                return Ř.Count > 0
                    ? n.o(
                        Ř[0]
                            .É)
                    : Vector3D.Zero;
            }
        }

        public double ř
        {
            get
            {
                double ł = 0;

                for (int Į = 0;
                    Į < Ř.Count;
                    Į++)
                {
                    ł +=
                        Ř[Į]
                            .ĩ;
                }

                return ł;
            }
        }

        public Vector3D Ś(
            Vector3D Ő)
        {
            return n.ą(
                Ő,
                É);
        }

        public double Ŝ(
            Vector3D s)
        {
            Vector3D ś =
                Ś(
                    s);

            if (ś
                    .LengthSquared() <=
                p)
            {
                return 0;
            }

            return Math.Min(
                ś.Length(),
                ř);
        }
    }


    sealed class ũ
    {
        readonly Program A;

        bool Ş;

        float ş =
            float.NaN,Š =
            float.NaN,š =
            float.NaN;

        C D;

        public readonly IMyGyro E;
        public readonly F G;

        public readonly double Ţ;

        public C L
        {
            get
            {
                return D;
            }
        }

        public bool P
        {
            get
            {
                return D !=
                       C.M;
            }

            set
            {
                N(
                    C.O,
                    value);
            }
        }

        public bool Y
        {
            get
            {
                return (G &
                        F.X) != 0;
            }
        }

        public bool a
        {
            get
            {
                return (G &
                        F.Z) != 0;
            }
        }

        public double ř
        {
            get
            {
                if (!P ||
                    E == null ||
                    E.Closed ||
                    !E.IsFunctional ||
                    !E.Enabled)
                {
                    return 0;
                }

                return Ţ *
                       MathHelper.Clamp(
                           E.GyroPower,
                           0,
                           1);
            }
        }

        public ũ(
            IMyGyro d,
            Program A,
            F e,
            bool f)
        {
            E = d;
            this.A = A;
            G = e;

            if (f &&
                !Y)
            {
                D =
                    C.O;
            }

            bool ţ =
                d
                    .CubeGrid
                    .GridSizeEnum ==
                VRage.Game
                    .MyCubeSize.Small;

            bool Ť =
                d
                    .BlockDefinition
                    .SubtypeId
                    .IndexOf(
"Prototech",
                        StringComparison
                            .OrdinalIgnoreCase) >= 0;

            if (Ť)
            {
                Ţ =
                    ţ
                        ? ť
                        : Ŧ;
            }
            else
            {
                Ţ =
                    ţ
                        ? ŧ
                        : Ũ;
            }
        }

        public void N(
            C h,
            bool i)
        {
            if (Y)
            {
                D =
                    C.M;

                Ū();
                return;
            }

            if (i)
            {
                D |= h;
                return;
            }

            D &= ~h;

            if (D ==
                C.M)
            {
                Ū();
            }
        }

        public bool k(
            C h)
        {
            return (D &
                    h) != 0;
        }

        public void Ŵ(
            Vector3D ū)
        {
            if (!P ||
                E == null ||
                E.Closed ||
                ř <=
                    p)
            {
                Ū();
                return;
            }

            ū =
                n.Ŭ(
                    ū,
                    ŭ);

            Vector3D Ů =
                n
                    .Ĳ(
                        ū,
                        E.WorldMatrix);

            float ů =
                (float)Ů.X;

            float Ű =
                (float)Ů.Y;

            float ű =
                (float)Ů.Z;

            bool ų =
                Ů
                    .LengthSquared() >
                Ų *
                Ų;

            if (!ų)
            {
                Ū();
                return;
            }

            if (!Ş ||
                Math.Abs(
                    ů -
                    ş) >
                Ų)
            {
                E.Pitch =
                    ů;

                ş =
                    ů;
            }

            if (!Ş ||
                Math.Abs(
                    Ű -
                    Š) >
                Ų)
            {
                E.Yaw =
                    Ű;

                Š =
                    Ű;
            }

            if (!Ş ||
                Math.Abs(
                    ű -
                    š) >
                Ų)
            {
                E.Roll =
                    ű;

                š =
                    ű;
            }

            if (!Ş ||
                !E.GyroOverride)
            {
                E.GyroOverride =
                    true;
            }

            Ş = true;
        }

        public void Ū()
        {
            if (E == null ||
                E.Closed)
            {
                return;
            }

            if (E.GyroOverride)
            {
                E.GyroOverride =
                    false;
            }

            if (Math.Abs(
                    E.Pitch) >
                Ų)
            {
                E.Pitch = 0;
            }

            if (Math.Abs(
                    E.Yaw) >
                Ų)
            {
                E.Yaw = 0;
            }

            if (Math.Abs(
                    E.Roll) >
                Ų)
            {
                E.Roll = 0;
            }

            Ş = false;
            ş = 0;
            Š = 0;
            š = 0;
        }

        public void À()
        {
            Ū();

            D =
                C.M;
        }
    }


    sealed class Ž
    {
        public readonly IMyTerminalBlock ŵ;
        public readonly IMyTextSurface Ŷ;
        public readonly int ŷ;

        bool Ÿ;

        public string Ź
        {
            get
            {
                return ŵ.EntityId +
":"+
                       ŷ;
            }
        }

        public Ž(
            IMyTerminalBlock ź,
            IMyTextSurface Ż,
            int ż)
        {
            ŵ = ź;
            Ŷ = Ż;
            ŷ =
                ż;
        }

        public void ſ(
            string ž)
        {
            if (ŵ == null ||
                ŵ.Closed ||
                Ŷ == null)
            {
                return;
            }

            if (!Ÿ)
            {
                Ŷ.ContentType =
                    VRage.Game.GUI
                        .TextPanel
                        .ContentType
                        .TEXT_AND_IMAGE;

                Ŷ.Font =
"Monospace";

                Ŷ.FontSize =
                    0.8f;

                Ŷ.Alignment =
                    VRage.Game.GUI
                        .TextPanel
                        .TextAlignment
                        .LEFT;

                Ÿ = true;
            }

            Ŷ.WriteText(
                ž,
                false);
        }
    }
    enum ƅ
    {
        ƀ,
        Ɓ,
        Ƃ,
        ƃ,
        Ƅ
    }

    [Flags]
    enum F
    {
        M = 0,
        Z = 1,
        X = 2,
        Ɔ = 4,
        Ƈ = 8,
        ƈ = 16
    }

    [Flags]
    enum C
    {
        M = 0,
        O = 1,
        Ɖ = 2,
        ƃ = 4
    }

    [Flags]
    enum ƌ
    {
        M = 0,
        Ɗ = 1,
        Ɖ = 2,
        Ƌ = 4
    }

    sealed class Ə
    {
        public bool ƍ;
        public ƌ Ǝ;
    }

    sealed class Ɩ
    {
        public double Ɛ,Ƒ,ƒ,Ɠ,Ɣ,ƕ;

        public void ĺ()
        {
            Ɛ = 0;
            Ƒ = 0;
            ƒ = 0;
            Ɠ = 0;
            Ɣ = 0;
            ƕ = 0;
        }

        public void Ƙ(
            Ɩ Ɨ)
        {
            if (Ɨ == null)
            {
                return;
            }

            Ɛ += Ɨ.Ɛ;
            Ƒ += Ɨ.Ƒ;
            ƒ += Ɨ.ƒ;
            Ɠ += Ɨ.Ɠ;
            Ɣ += Ɨ.Ɣ;
            ƕ += Ɨ.ƕ;
        }

        public void ƙ(
            Ɩ Ɨ)
        {
            if (Ɨ == null)
            {
                ĺ();
                return;
            }

            Ɛ = Ɨ.Ɛ;
            Ƒ = Ɨ.Ƒ;
            ƒ = Ɨ.ƒ;
            Ɠ = Ɨ.Ɠ;
            Ɣ = Ɨ.Ɣ;
            ƕ = Ɨ.ƕ;
        }

        public double ƛ(
            Vector3D ƚ)
        {
            double ł = 0;

            if (ƚ.X > 0)
            {
                ł +=
                    Ɠ *
                    ƚ.X;
            }
            else if (ƚ.X < 0)
            {
                ł +=
                    ƒ *
                    -ƚ.X;
            }

            if (ƚ.Y > 0)
            {
                ł +=
                    Ɣ *
                    ƚ.Y;
            }
            else if (ƚ.Y < 0)
            {
                ł +=
                    ƕ *
                    -ƚ.Y;
            }

            if (ƚ.Z > 0)
            {
                ł +=
                    Ƒ *
                    ƚ.Z;
            }
            else if (ƚ.Z < 0)
            {
                ł +=
                    Ɛ *
                    -ƚ.Z;
            }

            return ł;
        }
    }

    sealed class î
    {
        public H I;
        public double ð,ń,Ņ,Ň;

        public void ĺ()
        {
            I = null;
            ð = 0;
            ń = 0;
            Ņ = 0;
            Ň = 0;
        }
    }

    struct Ɯ :
        IEquatable<Ɯ>
    {
        public long Ɲ;
        public ulong ƞ,Ɵ;

        public bool Equals(
            Ɯ Ɨ)
        {
            return Ɲ ==
                       Ɨ.Ɲ &&
                   ƞ ==
                       Ɨ.ƞ &&
                   Ɵ ==
                       Ɨ.Ɵ;
        }

        public override bool Equals(
            object Ơ)
        {
            return Ơ is
                       Ɯ &&
                   Equals(
                       (Ɯ)
                           Ơ);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int ơ =
                    (int)Ɲ ^
                    (int)(Ɲ >> 32);

                ơ =
                    ơ * 397 ^
                    (int)ƞ ^
                    (int)(ƞ >> 32);

                ơ =
                    ơ * 397 ^
                    (int)Ɵ ^
                    (int)(Ɵ >> 32);

                return ơ;
            }
        }

        public static bool operator ==(
            Ɯ Ƣ,
            Ɯ ƣ)
        {
            return Ƣ.Equals(ƣ);
        }

        public static bool operator !=(
            Ɯ Ƣ,
            Ɯ ƣ)
        {
            return !Ƣ.Equals(ƣ);
        }
    }

    struct Ƥ :
        IEquatable<Ƥ>
    {
        public readonly long ƥ,Ʀ;

        public Ƥ(
            long Ƨ,
            long ƨ)
        {
            ƥ = Math.Min(
                Ƨ,
                ƨ);

            Ʀ = Math.Max(
                Ƨ,
                ƨ);
        }

        public bool Equals(
            Ƥ Ɨ)
        {
            return ƥ == Ɨ.ƥ &&
                   Ʀ == Ɨ.Ʀ;
        }

        public override bool Equals(
            object Ơ)
        {
            return Ơ is
                       Ƥ &&
                   Equals(
                       (Ƥ)
                           Ơ);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int Ʃ =
                    (int)ƥ ^
                    (int)(ƥ >> 32);

                int ƪ =
                    (int)Ʀ ^
                    (int)(Ʀ >> 32);

                return Ʃ * 397 ^
                       ƪ;
            }
        }
    }

    sealed class Ƶ
    {
        public long ƫ,Ƭ,ƭ;

        public Vector3D Ʈ;

        public bool Ư,Ɖ,ư;
        public double Ʊ,Ʋ;

        public int Ƴ,ƴ;

        public void ƙ(
            Ƶ Ɨ)
        {
            ƫ =
                Ɨ.ƫ;

            Ƭ =
                Ɨ.Ƭ;

            ƭ =
                Ɨ.ƭ;

            Ʈ =
                Ɨ.Ʈ;

            Ư =
                Ɨ.Ư;

            Ɖ =
                Ɨ.Ɖ;

            Ʊ =
                Ɨ.Ʊ;

            Ƴ =
                Ɨ.Ƴ;

            ƴ =
                Ɨ.ƴ;

            Ʋ =
                Ɨ.Ʋ;

            ư =
                Ɨ.ư;
        }

        public void ĺ()
        {
            ƫ = 0;
            Ƭ = 0;
            ƭ = 0;

            Ʈ =
                Vector3D.Zero;

            Ư = true;
            Ɖ = false;
            Ʊ = 0;

            Ƴ = 0;
            ƴ = 0;
            Ʋ = 0;

            ư = false;
        }
    }

    sealed class ƽ
    {
        readonly Program A;

        double ƶ;
        double Ʒ,Ƹ;
        int ƹ;

        public double ƺ
        {
            get
            {
                return ƶ;
            }
        }

        public double ƻ
        {
            get
            {
                return Ʒ;
            }
        }

        public double Ƽ
        {
            get
            {
                return Ƹ;
            }
        }

        public ƽ(
            Program A)
        {
            this.A = A;
        }

        public void ƾ()
        {
            Ƹ =
                A.Runtime
                    .LastRunTimeMs;
        }

        public void ǀ()
        {
            double ƿ =
                A.Runtime
                    .LastRunTimeMs;

            ƹ++;

            if (ƹ == 1)
            {
                ƶ =
                    ƿ;

                Ʒ =
                    ƿ;

                return;
            }

            ƶ +=
                (ƿ -
                 ƶ) *
                0.05;

            if (ƿ >
                Ʒ)
            {
                Ʒ =
                    ƿ;
            }
            else if (ƹ %
                     600 == 0)
            {
                Ʒ =
                    ƶ;
            }
        }
    }

    public static class n
    {

public static Vector3D o(
            Vector3D ǁ)
        {
            if (Vector3D.IsZero(
                    ǁ))
            {
                return Vector3D.Zero;
            }

            if (Vector3D.IsUnit(
                    ref ǁ))
            {
                return ǁ;
            }

            return Vector3D.Normalize(
                ǁ);
        }

public static Vector3D ą(
            Vector3D ǂ,
            Vector3D ǃ)
        {
            double Ǆ =
                ǃ.LengthSquared();

            if (ǂ.LengthSquared() <=
                    p ||
                Ǆ <=
                    p)
            {
                return Vector3D.Zero;
            }

            return ǂ -
                   Vector3D.Dot(ǂ, ǃ) /
                   Ǆ *
                   ǃ;
        }

public static Vector3D ǅ(
            Vector3D ǂ,
            Vector3D ǃ)
        {
            double Ǆ =
                ǃ.LengthSquared();

            if (ǂ.LengthSquared() <=
                    p ||
                Ǆ <=
                    p)
            {
                return Vector3D.Zero;
            }

            return Vector3D.Dot(ǂ, ǃ) /
                   Ǆ *
                   ǃ;
        }

        public static double í(
            Vector3D ǂ,
            Vector3D ǃ)
        {
            double Ǆ =
                Math.Sqrt(
                    ǂ.LengthSquared() *
                    ǃ.LengthSquared());

            if (Ǆ <=
                p)
            {
                return 0;
            }

            return MathHelper.Clamp(
                Vector3D.Dot(ǂ, ǃ) /
                Ǆ,
                -1,
                1);
        }

        public static Vector3D Ŭ(
            Vector3D ǁ,
            double ǆ)
        {
            if (ǆ <= 0)
            {
                return Vector3D.Zero;
            }

            double Ǉ =
                ǁ.LengthSquared();

            double ǈ =
                ǆ *
                ǆ;

            if (Ǉ <=
                ǈ)
            {
                return ǁ;
            }

            if (Ǉ <=
                p)
            {
                return Vector3D.Zero;
            }

            return ǁ *
                   (ǆ /
                    Math.Sqrt(
                        Ǉ));
        }

        public static double Ø(
            double ǉ)
        {
            while (ǉ >
                   Math.PI)
            {
                ǉ -=
                    MathHelper.TwoPi;
            }

            while (ǉ <
                   -Math.PI)
            {
                ǉ +=
                    MathHelper.TwoPi;
            }

            return ǉ;
        }

        public static Vector3D ë(
            Vector3D ǁ,
            Vector3D Ǌ,
            double ǉ)
        {
            Ǌ =
                o(Ǌ);

            if (Ǌ.LengthSquared() <=
                p)
            {
                return ǁ;
            }

            double ǋ =
                Math.Cos(ǉ);

            double ǌ =
                Math.Sin(ǉ);

            return ǁ * ǋ +
                   Vector3D.Cross(
                       Ǌ,
                       ǁ) *
                   ǌ +
                   Ǌ *
                   Vector3D.Dot(
                       Ǌ,
                       ǁ) *
                   (1.0 - ǋ);
        }

public static double Ē(
            Vector3D m,
            Vector3D đ,
            Vector3D Ǎ)
        {
            Vector3D ǎ =
                ą(
                    m,
                    Ǎ);

            Vector3D Ǐ =
                ą(
                    đ,
                    Ǎ);

            if (ǎ
                    .LengthSquared() <=
                    p ||
                Ǐ
                    .LengthSquared() <=
                    p)
            {
                return 0;
            }

            ǎ =
                o(
                    ǎ);

            Ǐ =
                o(
                    Ǐ);

            Ǎ =
                o(
                    Ǎ);

            return Math.Atan2(
                Vector3D.Dot(
                    Ǎ,
                    Vector3D.Cross(
                        ǎ,
                        Ǐ)),
                Vector3D.Dot(
                    ǎ,
                    Ǐ));
        }

        public static Vector3D Ĳ(
            Vector3D ǐ,
            MatrixD Ǒ)
        {
            return Vector3D.TransformNormal(
                ǐ,
                MatrixD.Transpose(
                    Ǒ));
        }

        public static Vector3D Ĭ(
            Vector3D ƚ,
            MatrixD Ǒ)
        {
            return Vector3D.TransformNormal(
                ƚ,
                Ǒ);
        }
    }

    static string ǒ(
        ƅ ğ)
    {
        switch (ğ)
        {
            case ƅ.ƀ:
                return "Initializing";

            case ƅ.Ɓ:
                return "Active";

            case ƅ.Ƃ:
                return "Master";

            case ƅ.ƃ:
                return "Slave";

            case ƅ.Ƅ:
                return "Parked";

            default:
                return "Unknown";
        }
    }

    sealed class ǘ
    {
        public readonly IMyCubeGrid Ǔ;

        public readonly List<ǔ> Ǖ =
            new List<ǔ>();

        public ǖ Ǘ;
        public ǘ Ǚ;
        public ǔ ǚ;

        public int Ǜ =
            int.MaxValue;

        public bool ǜ;

        public ǘ(
            IMyCubeGrid Ŗ)
        {
            Ǔ = Ŗ;
        }
    }

    sealed class ǔ
    {
        public readonly ǘ ƥ,Ʀ;

        public readonly IMyTerminalBlock ǝ;

        public ǔ(
            ǘ ǂ,
            ǘ ǃ,
            IMyTerminalBlock Ǟ)
        {
            ƥ = ǂ;
            Ʀ = ǃ;
            ǝ = Ǟ;
        }

        public ǘ Ǡ(
            ǘ ǟ)
        {
            return ǟ == ƥ
                ? Ʀ
                : ƥ;
        }
    }

    sealed class ǖ
    {
        public readonly List<ǘ> ǡ =
            new List<ǘ>();

        public readonly List<IMyShipController> Ǣ =
            new List<IMyShipController>();

        public readonly List<IMyProgrammableBlock> ǣ =
            new List<IMyProgrammableBlock>();

        public bool ǜ,Ǥ,ǥ,Ǧ,ǧ = true,Ǩ = true;

        public IMyProgrammableBlock ǩ;
    }

    sealed class Ǭ
    {
        public IMyShipConnector ƥ,Ʀ;

        public ǘ Ǫ,ǫ;
    }

    sealed class ǯ
    {
        public IMyShipConnector ǭ;
        public Ǭ Ǯ;
    }

    sealed class ǰ
    {
        public IMyLandingGear ǭ;
    }

    void Ǿ(
        string Ǳ)
    {
        if (string.IsNullOrWhiteSpace(
                Ǳ))
        {
            return;
        }

        string[] ǲ =
            Ǳ.Split(
                new[]
                {
                    ';',
                    '\n',
                    '\r'
                },
                StringSplitOptions
                    .RemoveEmptyEntries);

        bool ǳ =
            false;

        bool Ǵ =
            false;

        StringBuilder ǵ =
            new StringBuilder();

        for (int Į = 0;
            Į < ǲ.Length;
            Į++)
        {
            string Ƿ =
                Ƕ(
                    ǲ[Į]);

            if (Ƿ.Length == 0)
            {
                continue;
            }

            ǳ =
                true;

            string Ǹ;
            bool ǹ;

            if (!Ǻ(
                    Ƿ,
                    out Ǹ,
                    out ǹ))
            {
                Ǹ =
"Unknown command: \""+
                    Ƿ +
"\"";

                ǹ =
                    true;
            }

            if (ǻ &&
                !string.IsNullOrEmpty(
                    Ǽ))
            {
                if (Ǹ.Length > 0)
                {
                    Ǹ +=
"\n";
                }

                Ǹ +=
                    Ǽ;

                ǹ =
                    true;
            }

            if (ǵ.Length > 0)
            {
                ǵ.AppendLine();
            }

            ǵ.Append(Ǹ);

            Ǵ |=
                ǹ;
        }

        if (!ǳ)
        {
            return;
        }

        ǽ(
            ǵ.ToString(),
            Ǵ);

        Save();
    }

    bool Ǻ(
        string Ƿ,
        out string Ǹ,
        out bool ǹ)
    {
        Ǹ =
            string.Empty;

        ǹ =
            false;

        string[] ǿ =
            Ƿ.Split(
                new[] { ' ' },
                StringSplitOptions
                    .RemoveEmptyEntries);

        if (ǿ.Length == 0)
        {
            return false;
        }

        int Ȁ;

        if (ȁ(
                ǿ,
                out Ȁ))
        {
            return Ȃ(
                ǿ,
                Ȁ,
                out Ǹ,
                out ǹ);
        }

        if (ȃ(
                ǿ,
                out Ȁ))
        {
            return Ȅ(
                ǿ,
                Ȁ,
                out Ǹ,
                out ǹ);
        }

        if (ȅ(
                ǿ,
                out Ȁ))
        {
            return Ȇ(
                ǿ,
                Ȁ,
                out Ǹ,
                out ǹ);
        }

        if (ǿ[0] ==
"unpark"||
            ǿ[0] ==
"undock")
        {
            ȇ =
                false;

            Ȉ =
                false;

            Ǹ =
"Parking: OFF";

            return true;
        }

        if (ǿ[0] ==
"gear")
        {
            return ȉ(
                ǿ,
                1,
                out Ǹ,
                out ǹ);
        }

        if (ǿ[0] ==
"rescan"||
            ǿ[0] ==
"scan"||
            ǿ[0] ==
"refresh")
        {
            if (ǿ.Length != 1)
            {
                Ǹ =
"Invalid rescan command: \""+
                    Ƿ +
"\"";

                ǹ =
                    true;

                return true;
            }

            Ȋ();

            Ǹ =
"Deep rescan requested.";

            return true;
        }

        return false;
    }

    bool Ȃ(
        string[] ǿ,
        int Ȁ,
        out string Ǹ,
        out bool ǹ)
    {
        Ǹ =
            string.Empty;

        ǹ =
            false;

        bool i;

        if (!ȋ(
                ǿ,
                Ȁ,
                Ȍ,
                out i))
        {
            Ǹ =
"Expected dampeners "+
"on, off, or toggle.";

            ǹ =
                true;

            return true;
        }

        ȍ(
            i);

        Ǹ =
"Local dampeners: "+
            (Ȍ
                ? "ON"                : "OFF");

        if (Ȏ ==
            ƅ.ƃ)
        {
            Ǹ +=
" (master remains effective while slaved)";
        }

        return true;
    }

    bool Ȅ(
        string[] ǿ,
        int Ȁ,
        out string Ǹ,
        out bool ǹ)
    {
        Ǹ =
            string.Empty;

        ǹ =
            false;

        if (Ȁ >=
            ǿ.Length)
        {
            ȏ();

            Ǹ =
                Ȑ();

            return true;
        }

        string ȑ =
            ǿ[Ȁ];

        bool Ȓ;

        if (ȓ(
                ȑ,
                Ȕ,
                out Ȓ))
        {
            if (Ȁ + 1 !=
                ǿ.Length)
            {
                Ǹ =
"Unexpected text after "+
"Cruise state.";

                ǹ =
                    true;

                return true;
            }

            ȕ(
                Ȓ);

            Ǹ =
                Ȑ();

            return true;
        }

        double Ȗ;

        if (ȗ(
                ǿ,
                Ȁ,
                out Ȗ))
        {
            Ș(
                Ȗ);

            Ǹ =
                Ȑ();

            return true;
        }

        bool ș =
            ȑ ==
"increase"||
            ȑ ==
"increment"||
            ȑ ==
"up"||
            ȑ ==
"add"||
            ȑ ==
"faster";

        bool Ț =
            ȑ ==
"decrease"||
            ȑ ==
"decrement"||
            ȑ ==
"down"||
            ȑ ==
"subtract"||
            ȑ ==
"slower";

        if (ș ||
            Ț)
        {
            if (Ȁ + 2 !=
                ǿ.Length)
            {
                Ǹ =
"Cruise adjustment requires "+
"one speed value.";

                ǹ =
                    true;

                return true;
            }

            double ț;

            if (!double.TryParse(
                    ǿ[
                        Ȁ + 1],
                    out ț))
            {
                Ǹ =
"Invalid Cruise speed: \""+
                    ǿ[
                        Ȁ + 1] +
"\"";

                ǹ =
                    true;

                return true;
            }

            ț =
                Math.Abs(ț);

            Ș(
                ș
                    ? ț
                    : -ț);

            Ǹ =
                Ȑ();

            return true;
        }

        if (ȑ ==
"target"||
            ȑ ==
"speed"||
            ȑ ==
"set")
        {
            if (Ȁ + 2 !=
                ǿ.Length)
            {
                Ǹ =
"Cruise target requires "+
"one speed value.";

                ǹ =
                    true;

                return true;
            }

            double Ȝ;

            if (!double.TryParse(
                    ǿ[
                        Ȁ + 1],
                    out Ȝ))
            {
                Ǹ =
"Invalid Cruise target: \""+
                    ǿ[
                        Ȁ + 1] +
"\"";

                ǹ =
                    true;

                return true;
            }

            ȝ =
                Ȝ;

            Ȟ =
                true;

            Ǹ =
                Ȑ();

            return true;
        }

        Ǹ =
"Expected Cruise on, off, toggle, "+
"+value, -value, increase value, "+
"decrease value, or target value.";

        ǹ =
            true;

        return true;
    }

    bool Ȇ(
        string[] ǿ,
        int Ȁ,
        out string Ǹ,
        out bool ǹ)
    {
        Ǹ =
            string.Empty;

        ǹ =
            false;

        bool i;

        if (!ȋ(
                ǿ,
                Ȁ,
                ȇ,
                out i))
        {
            Ǹ =
"Expected parking "+
"on, off, or toggle.";

            ǹ =
                true;

            return true;
        }

        ȇ =
            i;

        Ȉ =
            false;

        Ǹ =
"Parking: "+
            (ȇ
                ? "ON"                : "OFF");

        return true;
    }

    bool ȉ(
        string[] ǿ,
        int Ȁ,
        out string Ǹ,
        out bool ǹ)
    {
        Ǹ =
            string.Empty;

        ǹ =
            false;

        int ȡ =
            ȟ
                .Ƞ.Count;

        if (ȡ <= 0)
        {
            Ǹ =
"No gears are configured.";

            ǹ =
                true;

            return true;
        }

        if (Ȁ >=
            ǿ.Length)
        {
            Ȣ =
                (Ȣ + 1) %
                ȡ;

            Ǹ =
                ȣ();

            return true;
        }

        if (Ȁ + 1 !=
            ǿ.Length)
        {
            Ǹ =
"Gear accepts one argument.";

            ǹ =
                true;

            return true;
        }

        string ȑ =
            ǿ[Ȁ];

        if (ȑ ==
"next"||
            ȑ ==
"up"||
            ȑ ==
"increase"||
            ȑ ==
"increment")
        {
            Ȣ =
                (Ȣ + 1) %
                ȡ;

            Ǹ =
                ȣ();

            return true;
        }

        if (ȑ ==
"previous"||
            ȑ ==
"prev"||
            ȑ ==
"down"||
            ȑ ==
"decrease"||
            ȑ ==
"decrement")
        {
            Ȣ--;

            if (Ȣ < 0)
            {
                Ȣ =
                    ȡ - 1;
            }

            Ǹ =
                ȣ();

            return true;
        }

        int Ȥ;

        if (!int.TryParse(
                ȑ,
                out Ȥ))
        {
            Ǹ =
"Invalid gear: \""+
                ȑ +
"\"";

            ǹ =
                true;

            return true;
        }

        if (Ȥ < 1 ||
            Ȥ > ȡ)
        {
            Ǹ =
"Gear must be between 1 and "+
                ȡ +
".";

            ǹ =
                true;

            return true;
        }

        Ȣ =
            Ȥ - 1;

        Ǹ =
            ȣ();

        return true;
    }

    static bool ȁ(
        string[] ǿ,
        out int Ȁ)
    {
        Ȁ = 1;

        if (ǿ[0] ==
"dampeners"||
            ǿ[0] ==
"dampener"||
            ǿ[0] ==
"dampers"||
            ǿ[0] ==
"damper"||
            ǿ[0] ==
"damping"||
            ǿ[0] ==
"dampening")
        {
            return true;
        }

        if (ǿ.Length >= 2 &&
            ǿ[0] ==
"inertia"&&
            (ǿ[1] ==
"dampeners"||
             ǿ[1] ==
"dampener"||
             ǿ[1] ==
"damping"))
        {
            Ȁ = 2;
            return true;
        }

        return false;
    }

    static bool ȃ(
        string[] ǿ,
        out int Ȁ)
    {
        Ȁ = 1;

        if (ǿ.Length >= 2 &&
            ǿ[0] ==
"cruise"&&
            ǿ[1] ==
"control")
        {
            Ȁ = 2;
            return true;
        }

        return ǿ[0] ==
"cruise"||
            ǿ[0] ==
"cruisecontrol";
    }

    static bool ȅ(
        string[] ǿ,
        out int Ȁ)
    {
        Ȁ = 1;

        return ǿ[0] ==
"park"||
               ǿ[0] ==
"parking";
    }

    static bool ȋ(
        string[] ǿ,
        int Ȁ,
        bool ȥ,
        out bool Ǹ)
    {
        if (Ȁ >=
            ǿ.Length)
        {
            Ǹ =
                !ȥ;

            return true;
        }

        if (Ȁ + 1 !=
            ǿ.Length)
        {
            Ǹ =
                ȥ;

            return false;
        }

        return ȓ(
            ǿ[Ȁ],
            ȥ,
            out Ǹ);
    }

    static bool ȓ(
        string ȑ,
        bool ȥ,
        out bool Ǹ)
    {
        if (ȑ ==
"on"||
            ȑ ==
"enable"||
            ȑ ==
"enabled"||
            ȑ ==
"start")
        {
            Ǹ = true;
            return true;
        }

        if (ȑ ==
"off"||
            ȑ ==
"disable"||
            ȑ ==
"disabled"||
            ȑ ==
"stop")
        {
            Ǹ = false;
            return true;
        }

        if (ȑ ==
"toggle"||
            ȑ ==
"switch")
        {
            Ǹ =
                !ȥ;

            return true;
        }

        Ǹ =
            ȥ;

        return false;
    }

    static bool ȗ(
        string[] ǿ,
        int Ȁ,
        out double Ö)
    {
        Ö = 0;

        if (Ȁ >=
            ǿ.Length)
        {
            return false;
        }

        if (Ȁ + 1 ==
            ǿ.Length)
        {
            string Ȧ =
                ǿ[Ȁ];

            if (Ȧ.Length < 2 ||
                Ȧ[0] != '+' &&
                Ȧ[0] != '-')
            {
                return false;
            }

            return double.TryParse(
                Ȧ,
                out Ö);
        }

        if (Ȁ + 2 ==
                ǿ.Length &&
            (ǿ[Ȁ] ==
"+"||
             ǿ[Ȁ] ==
"-"))
        {
            double ț;

            if (!double.TryParse(
                    ǿ[
                        Ȁ + 1],
                    out ț))
            {
                return false;
            }

            Ö =
                ǿ[Ȁ] ==
"+"                    ? Math.Abs(ț)
                    : -Math.Abs(ț);

            return true;
        }

        return false;
    }

    string Ȑ()
    {
        string Ǹ =
"Local Cruise: "+
            (Ȕ
                ? "ON"                : "OFF");

        if (Ȟ)
        {
            Ǹ +=
" @ "+
                ȝ
                    .ToString("0.###") +
" m/s";
        }

        if (Ȏ ==
            ƅ.ƃ)
        {
            Ǹ +=
" (master remains effective while slaved)";
        }

        return Ǹ;
    }

    string ȣ()
    {
        int ȡ =
            ȟ
                .Ƞ.Count;

        double ȧ =
            ȡ > 0
                ? ȟ
                    .Ƞ[
                        MathHelper.Clamp(
                            Ȣ,
                            0,
                            ȡ - 1)] *
                  100
                : 0;

        string Ǹ =
"Local gear: "+
            (Ȣ + 1) +
"/"+
            ȡ +
" ("+
            ȧ
                .ToString("0.##") +
"%)";

        if (Ȏ ==
            ƅ.ƃ)
        {
            Ǹ +=
" (master remains effective while slaved)";
        }

        return Ǹ;
    }

    static string Ƕ(
        string Ƿ)
    {
        if (string.IsNullOrWhiteSpace(
                Ƿ))
        {
            return string.Empty;
        }

        Ƿ =
            Ƿ
                .Trim()
                .ToLowerInvariant();

        StringBuilder Ȩ =
            new StringBuilder(
                Ƿ.Length);

        bool ȩ =
            false;

        for (int Į = 0;
            Į < Ƿ.Length;
            Į++)
        {
            char Ȫ =
                Ƿ[Į];

            bool ȫ =
                char.IsWhiteSpace(
                    Ȫ);

            if (ȫ)
            {
                if (!ȩ &&
                    Ȩ.Length > 0)
                {
                    Ȩ.Append(' ');
                }

                ȩ =
                    true;

                continue;
            }

            Ȩ.Append(
                Ȫ);

            ȩ =
                false;
        }

        if (Ȩ.Length > 0 &&
            Ȩ[
                Ȩ.Length - 1] ==
            ' ')
        {
            Ȩ.Length--;
        }

        return Ȩ.ToString();
    }
    sealed class ȹ
    {
        public bool Ȭ = true,ȭ = true,Ȯ = true,ȯ,Ȱ = true;
        public readonly List<double> Ƞ = new List<double>
        {
            0.15,
            0.50,
            1.00
        };

        public string ȱ = "[VT-use]",Ȳ = "[VT-ignore]",ȳ = "[VT-status]",ȴ = "[VT-park]",ȵ = "[VT-unpark]";

        public int ȶ,ȷ,ȸ;
    }

    readonly MyIni Ⱥ = new MyIni();

    bool Ɏ(bool Ȼ)
    {
        string ȼ = Me.CustomData ?? string.Empty;

        if (!Ȼ && ȼ == Ƚ)
        {
            return false;
        }

        Ⱥ.Clear();

        MyIniParseResult Ⱦ;

        if (!Ⱥ.TryParse(ȼ, out Ⱦ))
        {
            Echo(
                ȿ +
"\n\nCustom Data could not be parsed as INI:\n"+
                Ⱦ);

            Ƚ = ȼ;
            return false;
        }

        bool ɀ = ȟ.Ȭ;
        bool Ɂ = ȟ.ȭ;
        bool ɂ = ȟ.Ȯ;

        string Ƀ = ȟ.ȱ;
        string Ʉ = ȟ.Ȳ;
        string Ʌ = ȟ.ȳ;
        string Ɇ = ȟ.ȴ;
        string ɇ = ȟ.ȵ;


        ȟ.Ȭ = Ⱥ
            .Get(Ɉ, "Greedy")
            .ToBoolean(ȟ.Ȭ);

        ȟ.ȭ = Ⱥ
            .Get(Ɉ, "CanMaster")
            .ToBoolean(ȟ.ȭ);

        ȟ.Ȯ = Ⱥ
            .Get(Ɉ, "CanSlave")
            .ToBoolean(ȟ.Ȯ);


        ȟ.ȯ = Ⱥ
            .Get("Parking", "ParkOnlyByCommand")
            .ToBoolean(ȟ.ȯ);


        ȟ.Ȱ = Ⱥ
            .Get("Flight", "CruiseLevelsWithGravity")
            .ToBoolean(ȟ.Ȱ);

        ɉ(
            Ⱥ.Get("Flight", "GearPercentages")
                .ToString("15; 50; 100"));

        if (Ȣ >= ȟ.Ƞ.Count)
        {
            Ȣ = ȟ.Ƞ.Count - 1;
        }


        ȟ.ȱ = Ɋ(
"Tags",
"Use",
            ȟ.ȱ);

        ȟ.Ȳ = Ɋ(
"Tags",
"Ignore",
            ȟ.Ȳ);

        ȟ.ȳ = Ɋ(
"Tags",
"Status",
            ȟ.ȳ);

        ȟ.ȴ = Ɋ(
"Tags",
"ParkTimer",
            ȟ.ȴ);

        ȟ.ȵ = Ɋ(
"Tags",
"UnparkTimer",
            ȟ.ȵ);


        ȟ.ȶ = Math.Max(
            0,
            Ⱥ
                .Get("Performance", "Update1Skip")
                .ToInt32(ȟ.ȶ));

        ȟ.ȷ = Math.Max(
            0,
            Ⱥ
                .Get("Performance", "Update10Skip")
                .ToInt32(ȟ.ȷ));

        ȟ.ȸ = Math.Max(
            0,
            Ⱥ
                .Get("Performance", "Update100Skip")
                .ToInt32(ȟ.ȸ));

        ɋ();

        string Ɍ = Ⱥ.ToString();

        if (Ɍ != Me.CustomData)
        {
            Me.CustomData = Ɍ;
        }

        Ƚ = Me.CustomData;

        bool ɍ =
            ɀ != ȟ.Ȭ ||
            Ɂ != ȟ.ȭ ||
            ɂ != ȟ.Ȯ ||
            !Ƀ.Equals(ȟ.ȱ, StringComparison.OrdinalIgnoreCase) ||
            !Ʉ.Equals(ȟ.Ȳ, StringComparison.OrdinalIgnoreCase) ||
            !Ʌ.Equals(ȟ.ȳ, StringComparison.OrdinalIgnoreCase) ||
            !Ɇ.Equals(ȟ.ȴ, StringComparison.OrdinalIgnoreCase) ||
            !ɇ.Equals(ȟ.ȵ, StringComparison.OrdinalIgnoreCase);

        if (!Ȼ && ɍ)
        {
            Ȋ();
        }

        return true;
    }

    string Ɋ(
        string ɏ,
        string ɐ,
        string ɑ)
    {
        string ğ = Ⱥ
            .Get(ɏ, ɐ)
            .ToString(ɑ)
            .Trim();

        return ğ.Length == 0 ? ɑ : ğ;
    }

    void ɉ(
        string ɒ)
    {
        string[] ɓ =
            ɒ.Split(
                new[] { ';' },
                StringSplitOptions
                    .RemoveEmptyEntries);

        List<double> ɔ =
            new List<double>();

        for (int Į = 0;
            Į < ɓ.Length;
            Į++)
        {
            double ȧ;

            if (!double.TryParse(
                    ɓ[Į].Trim(),
                    out ȧ))
            {
                continue;
            }

            if (ȧ > 0)
            {
                ɔ.Add(
                    ȧ /
                    100.0);
            }
        }

        if (ɔ.Count == 0)
        {
            return;
        }

        ȟ.Ƞ.Clear();

        ȟ.Ƞ.AddRange(
            ɔ);
    }

    void ɋ()
    {
        Ⱥ.Set(Ɉ, "Greedy", ȟ.Ȭ);
        Ⱥ.Set(Ɉ, "CanMaster", ȟ.ȭ);
        Ⱥ.Set(Ɉ, "CanSlave", ȟ.Ȯ);

        Ⱥ.SetSectionComment(
            Ɉ,
" Vector Thrust Redux ownership and coordination.\n"+
" Greedy controls eligible mechanical-subgrid blocks unless ignored.\n"+
" Main-grid player thrusters and gyros remain read-only unless explicitly tagged.");

        Ⱥ.Set(
"Parking",
"ParkOnlyByCommand",
            ȟ.ȯ);

        Ⱥ.Set(
"Flight",
"CruiseLevelsWithGravity",
            ȟ.Ȱ);

        Ⱥ.Set(
"Flight",
"GearPercentages",
            ɕ());

        Ⱥ.Set("Tags", "Use", ȟ.ȱ);
        Ⱥ.Set("Tags", "Ignore", ȟ.Ȳ);
        Ⱥ.Set("Tags", "Status", ȟ.ȳ);
        Ⱥ.Set("Tags", "ParkTimer", ȟ.ȴ);
        Ⱥ.Set("Tags", "UnparkTimer", ȟ.ȵ);

        Ⱥ.SetComment(
"Tags",
"Use",
" Tag may appear in a block name, group name, or block Custom Data.");

        Ⱥ.SetComment(
"Tags",
"Ignore",
" Ignore always prevents Redux from modifying the block.");

        Ⱥ.Set(
"Performance",
"Update1Skip",
            ȟ.ȶ);

        Ⱥ.Set(
"Performance",
"Update10Skip",
            ȟ.ȷ);

        Ⱥ.Set(
"Performance",
"Update100Skip",
            ȟ.ȸ);

        Ⱥ.SetSectionComment(
"Performance",
" Number of matching update intervals skipped between executions.\n"+
" Heartbeat publication is never skipped.");
    }

    string ɕ()
    {
        StringBuilder ɖ = new StringBuilder();

        for (int Į = 0; Į < ȟ.Ƞ.Count; Į++)
        {
            if (Į > 0)
            {
                ɖ.Append("; ");
            }

            ɖ.Append(
                (ȟ.Ƞ[Į] * 100.0)
                    .ToString("0.########"));
        }

        return ɖ.ToString();
    }
    readonly HashSet<H> ɗ =
        new HashSet<H>(),ɘ =
        new HashSet<H>();


    void ɚ()
    {
        Ɏ(false);

        ə();
    }

    void ɫ()
    {
        ɛ();
        ɜ();

        ɝ();
        ɞ();
        ɟ();

        ɠ =
            ȟ.ȭ &&
            ɡ != null &&
            ɡ
                .IsUnderControl;

        ɢ();

        if (ȟ.Ȯ &&
            !ɠ &&
            !ȇ)
        {
            ɣ();
        }

        if (ɤ)
        {
            ɥ = 0;
        }
        else if (ɦ !=
                 long.MinValue)
        {
            ɥ++;
        }

        ɧ =
            ɦ !=
                long.MinValue &&
            ɥ < 2;

        ɤ =
            false;

        ɨ();

        if (Ȏ ==
            ƅ.Ƅ)
        {
            ɩ();
        }

        ɪ(false);
    }

    void ʁ(
        double Ô)
    {
        ɛ();

        if (Ȏ ==
                ƅ.Ƅ ||
            Ȏ ==
                ƅ.ƀ ||
            ɡ == null)
        {
            ɬ();
            ɭ();
            return;
        }

        ɮ();
        ɝ();
        ɞ();
        ɟ();

        Vector3D ɯ =
            ɡ.CenterOfMass;

        Vector3D ɰ;

        if (Ȏ ==
            ƅ.ƃ)
        {
            Vector3D ɲ =
                n.Ŭ(
                    ɱ
                        .Ʈ,
                    1);

            Vector3D t =
                n.o(
                    ɲ);

            double Ý =
                ɲ.Length();

            double ɴ =
                ɳ(
                    t);

            ɰ =
                t *
                Ý *
                ɴ;
        }
        else
        {
            Vector3D ɶ =
                ɵ(
                    Ô);

            ɶ -=
                ɷ();

            if (Ȏ ==
                ƅ.Ƃ)
            {
                Vector3D t =
                    n.o(
                        ɶ);

                double ɴ =
                    ɳ(
                        t);

                double ɹ =
                    ɸ(
                        t);

                double ɺ =
                    ɴ +
                    ɹ;

                double Ý =
                    ɺ >
                            U
                        ? MathHelper.Clamp(
                            ɶ
                                .Length() /
                            ɺ,
                            0,
                            1)
                        : 0;

                ɻ =
                    t *
                    Ý;

                ɰ =
                    t *
                    Ý *
                    ɴ;
            }
            else
            {
                ɻ =
                    Vector3D.Zero;

                ɰ =
                    ɶ;
            }
        }

        ɼ =
            ɰ;

        ɽ(
            ɰ,
            ɯ,
            Ô);

        bool ɾ =
            Ȏ ==
                    ƅ.ƃ
                ? ɱ
                    .ư
                : ǻ &&
                  ȟ
                      .Ȱ;

        ɿ(
            ʀ,
            ɾ);
    }

    void ɟ()
    {
        for (int Į = 0;
            Į < ʂ.Count;
            Į++)
        {
            ʂ[Į]
                .ĸ();
        }

        ʃ();
        ʄ();
    }

    void ʃ()
    {
        ʅ = 0;

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            if (!İ.P ||
                !İ.c)
            {
                continue;
            }

            ʅ +=
                İ
                    .V;
        }
    }

    void ʄ()
    {
        ʇ.ĺ();
        ʈ.ĺ();
        ʉ.ĺ();

        if (ɡ ==
            null)
        {
            return;
        }

        MatrixD ʊ =
            ɡ
                .WorldMatrix;

        ʇ.Ɛ =
            ɳ(
                ʊ.Forward);

        ʇ.Ƒ =
            ɳ(
                ʊ.Backward);

        ʇ.ƒ =
            ɳ(
                ʊ.Left);

        ʇ.Ɠ =
            ɳ(
                ʊ.Right);

        ʇ.Ɣ =
            ɳ(
                ʊ.Up);

        ʇ.ƕ =
            ɳ(
                ʊ.Down);

        ʈ.Ɛ =
            ɸ(
                ʊ.Forward);

        ʈ.Ƒ =
            ɸ(
                ʊ.Backward);

        ʈ.ƒ =
            ɸ(
                ʊ.Left);

        ʈ.Ɠ =
            ɸ(
                ʊ.Right);

        ʈ.Ɣ =
            ɸ(
                ʊ.Up);

        ʈ.ƕ =
            ɸ(
                ʊ.Down);

        ʉ.ƙ(
            ʇ);

        ʉ.Ƙ(
            ʈ);
    }

    double ɳ(
        Vector3D m)
    {
        m =
            n.o(
                m);

        if (m
                .LengthSquared() <=
            p)
        {
            return 0;
        }

        double ł = 0;

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            if (!İ.P ||
                !İ.c)
            {
                continue;
            }

            if (İ.I != null &&
                İ.I
                    .Ò
                    .P)
            {
                continue;
            }

            ł +=
                İ
                    .r(
                        m);
        }

        for (int Į = 0;
            Į < ʂ.Count;
            Į++)
        {
            H ʋ =
                ʂ[Į];

            if (!ʋ
                    .Ò
                    .P)
            {
                continue;
            }

            ł +=
                ʋ
                    .ŉ(
                        m);
        }

        return ł;
    }

    double ʌ(
        Vector3D m)
    {
        double ł =
            ɳ(
                m);

        if (Ȏ ==
            ƅ.Ƃ)
        {
            ł +=
                ɸ(
                    m);
        }

        return ł;
    }

    double ɸ(
        Vector3D m)
    {
        m =
            n.o(
                m);

        if (m
                .LengthSquared() <=
            p)
        {
            return 0;
        }

        double ł =
            ʍ(
                ʎ,
                m);

        for (int Į = 0;
            Į < ʏ.Count;
            Į++)
        {
            ł +=
                ʏ[Į]
                    .ŉ(
                        m);
        }

        return ł;
    }


    Vector3D ɵ(
        double Ô)
    {
        MyShipMass ʐ =
            ɡ
                .CalculateShipMass();

        double ʑ =
            ʐ.PhysicalMass;

        if (ʑ <=
            U)
        {
            return Vector3D.Zero;
        }

        MyShipVelocities ʒ =
            ɡ
                .GetShipVelocities();

        Vector3D ʓ =
            ʒ.LinearVelocity;

        Vector3D ʔ =
            ɡ
                .GetNaturalGravity();

        Vector3 ʕ =
            ɡ
                .MoveIndicator;

        Vector3D ʖ;

        if (ǻ)
        {
            ʖ =
                ʗ(
                    ʕ,
                    ʓ,
                    ʑ,
                    Ô);
        }
        else
        {
            ʖ =
                ʘ(
                    ʕ,
                    ʓ,
                    ʑ,
                    Ô);
        }

        return ʑ *
               (ʖ -
                ʔ);
    }

    Vector3D ʘ(
        Vector3 ʕ,
        Vector3D ʓ,
        double ʑ,
        double Ô)
    {
        Vector3D ʙ =
            Vector3D.TransformNormal(
                ʕ,
                ɡ
                    .WorldMatrix);

        Vector3D ʚ =
            n.o(
                ʙ);

        bool ʛ =
            ʚ
                .LengthSquared() >
            p;

        Vector3D ʖ =
            Vector3D.Zero;

        if (ʛ)
        {
            double ʜ =
                ʌ(
                    ʚ);

            double ʞ =
                ʜ /
                ʑ *
                ʝ;

            ʖ =
                ʚ *
                ʞ;
        }

        if (!ʟ)
        {
            return ʠ(
                ʖ,
                ʑ);
        }

        Vector3D ʡ =
            ʓ;

        if (ʡ.Length() <=
            ʢ)
        {
            ʡ =
                Vector3D.Zero;
        }

        if (ʛ)
        {
            double ʣ =
                Vector3D.Dot(
                    ʡ,
                    ʚ);

            if (ʣ > 0)
            {
                ʡ -=
                    ʚ *
                    ʣ;
            }
        }

        if (ʡ
                .LengthSquared() >
            p)
        {
            ʖ +=
                -ʡ /
                Math.Max(
                    Ô,
                    è);
        }

        return ʠ(
            ʖ,
            ʑ);
    }

    Vector3D ʗ(
        Vector3 ʕ,
        Vector3D ʓ,
        double ʑ,
        double Ô)
    {
        if (Ȏ !=
            ƅ.ƃ)
        {
            ʤ();

            double ʥ =
                -ʕ.Z;

            if (Math.Abs(
                    ʥ) >
                p)
            {
                Vector3D ʦ =
                    ʥ >= 0
                        ? ɡ
                            .WorldMatrix
                            .Forward
                        : ɡ
                            .WorldMatrix
                            .Backward;

                double ł =
                    ʌ(
                        ʦ);

                double ʧ =
                    ł /
                    ʑ *
                    ʝ;

                ȝ +=
                    ʥ *
                    ʧ *
                    Ô;
            }
        }

        Vector3 ʨ =
            ʕ;

        ʨ.Z = 0;

        Vector3D ʩ =
            Vector3D.TransformNormal(
                ʨ,
                ɡ
                    .WorldMatrix);

        Vector3D ʪ =
            n.o(
                ʩ);

        Vector3D ʖ =
            Vector3D.Zero;

        if (ʪ
                .LengthSquared() >
            p)
        {
            double ʫ =
                ʌ(
                    ʪ);

            ʖ +=
                ʪ *
                (ʫ /
                 ʑ *
                 ʝ);
        }

        Vector3D ʬ =
            ɡ
                .WorldMatrix
                .Forward;

        Vector3D ʮ =
            ʬ *
            ʭ;

        Vector3D ʯ =
            ʮ -
            ʓ;

        if (!ʟ)
        {
            ʯ =
                n.ǅ(
                    ʯ,
                    ʬ);
        }

        if (ʯ.Length() <=
            ʢ)
        {
            ʯ =
                Vector3D.Zero;
        }

        if (ʯ
                .LengthSquared() >
            p)
        {
            ʖ +=
                ʯ /
                Math.Max(
                    Ô,
                    è);
        }

        return ʠ(
            ʖ,
            ʑ);
    }

    Vector3D ʠ(
        Vector3D ʞ,
        double ʑ)
    {
        double ʰ =
            ʞ.Length();

        if (ʰ <=
                p ||
            ʑ <=
                U)
        {
            return Vector3D.Zero;
        }

        Vector3D t =
            ʞ /
            ʰ;

        double ʱ =
            ʌ(
                t) /
            ʑ;

        return n.Ŭ(
            ʞ,
            ʱ);
    }

    Vector3D ɷ()
    {
        Vector3D Ő =
            Vector3D.Zero;

        for (int Į = 0;
            Į < ʲ.Count;
            Į++)
        {
            g İ =
                ʲ[Į];

            if (!İ.P)
            {
                Ő +=
                    İ.W;
            }
        }

        if (Ȏ ==
            ƅ.Ƃ)
        {
            for (int Į = 0;
                Į < ʳ.Count;
                Į++)
            {
                g İ =
                    ʳ[Į];

                if (!İ.P)
                {
                    Ő +=
                        İ.W;
                }
            }
        }

        return Ő;
    }

    Vector3D ʴ()
    {
        return ɷ();
    }


    void ɽ(
        Vector3D ɰ,
        Vector3D ɯ,
        double Ô)
    {
        ʵ =
            ɰ;

        ʀ =
            Vector3D.Zero;

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            if (ʆ[Į].P)
            {
                ʆ[Į]
                    .l();
            }
        }

        ɗ.Clear();
        ɘ.Clear();

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            if (!İ.P ||
                İ.I != null &&
                İ.I
                    .Ò
                    .P)
            {
                continue;
            }

            ʶ(
                İ,
                ref ʵ,
                ɯ,
                ref ʀ);
        }

        for (int Į = 0;
            Į < ʂ.Count;
            Į++)
        {
            H ʋ =
                ʂ[Į];

            if (!ʋ
                    .Ò
                    .P)
            {
                continue;
            }

            ʋ.œ(
                ref ʵ,
                ɯ,
                ref ʀ);

            ɘ.Add(
                ʋ);
        }

        while (ɘ.Count >
                   0 &&
               ʵ
                   .LengthSquared() >
               U *
               U)
        {
            Vector3D m =
                n.o(
                    ʵ);

            H ʷ =
                null;

            î ʸ =
                null;

            foreach (
                H ʋ
                in ɘ)
            {
                î ï =
                    new î();

                if (!ʋ.ň(
                        m,
                        ï))
                {
                    continue;
                }

                bool ʹ =
                    ʸ == null ||
                    ï
                        .ń >
                    ʸ
                        .ń +
                    U;

                bool ʺ =
                    ʸ != null &&
                    Math.Abs(
                        ï
                            .ń -
                        ʸ
                            .ń) <=
                    U;

                bool ʻ =
                    ʺ &&
                    ʷ != null &&
                    ʋ
                        .Ò
                        .Q <
                    ʷ
                        .Ò
                        .Q;

                if (ʹ ||
                    ʻ)
                {
                    ʷ =
                        ʋ;

                    ʸ =
                        ï;
                }
            }

            if (ʷ == null)
            {
                break;
            }

            ɘ.Remove(
                ʷ);

            double ʼ =
                Vector3D.Dot(
                    ʵ,
                    m);

            ʷ.Ŋ(
                ʸ,
                ʼ,
                Ô);

            ɗ.Add(
                ʷ);

            ʷ.Œ(
                ref ʵ,
                ɯ,
                ref ʀ);
        }

        for (int Į = 0;
            Į < ʂ.Count;
            Į++)
        {
            H ʋ =
                ʂ[Į];

            if (!ʋ
                    .Ò
                    .P)
            {
                continue;
            }

            if (!ɗ.Contains(
                    ʋ))
            {
                ʋ.Ò.Ó();
            }
        }

        ʽ();

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            if (ʆ[Į].P)
            {
                ʆ[Į]
                    .º();
            }
        }
    }

    void ʶ(
        g İ,
        ref Vector3D s,
        Vector3D ɯ,
        ref Vector3D ō)
    {
        double ŏ =
            İ
                .x(
                    ref s);

        if (ŏ <=
            U)
        {
            return;
        }

        Vector3D Ő =
            İ
                .R *
            ŏ;

        Vector3D ő =
            İ
                .E
                .GetPosition() -
            ɯ;

        ō +=
            Vector3D.Cross(
                ő,
                Ő);
    }

    void ʽ()
    {
        if (!ǻ)
        {
            ʾ(
                ƌ.Ɖ);

            return;
        }

        for (int Į = 0;
            Į <
                ʿ.Count;
            Į++)
        {
            g İ =
                ʿ[Į];

            if (!İ.P)
            {
                continue;
            }

            if (İ
                    .K >
                U)
            {
                ʾ(
                    İ,
                    ƌ.Ɖ);

                y(
                    İ);

                continue;
            }

            ˀ(
                İ,
                ƌ.Ɖ);
        }
    }


    void ɿ(
        Vector3D ō,
        bool ɾ)
    {
        if (ˁ.Count == 0 ||
            ɡ == null)
        {
            return;
        }

        double ˆ = 0;

        for (int Į = 0;
            Į < ˁ.Count;
            Į++)
        {
            ˆ +=
                ˁ[Į]
                    .ř;
        }

        if (ˆ <=
            p)
        {
            ɭ();
            return;
        }

        Vector3D ˇ =
            -ō /
            ˆ *
            ŭ;

        if (ɾ)
        {
            Vector3D ʔ =
                ɡ
                    .GetNaturalGravity();

            if (ʔ.LengthSquared() >
                p)
            {
                Vector3D ˈ =
                    -n.o(
                        ʔ);

                Vector3D ˉ =
                    ɡ
                        .WorldMatrix
                        .Up;

                Vector3D ˊ =
                    Vector3D.Cross(
                        ˉ,
                        ˈ);

                double q =
                    MathHelper.Clamp(
                        Vector3D.Dot(
                            ˉ,
                            ˈ),
                        -1,
                        1);

                double ˋ =
                    Math.Atan2(
                        ˊ.Length(),
                        q);

                if (ˊ
                        .LengthSquared() >
                    p)
                {
                    ˊ =
                        n.o(
                            ˊ);

                    ˇ +=
                        ˊ *
                        ˋ *
                        ˌ;
                }

                Vector3D ˍ =
                    ɡ
                        .GetShipVelocities()
                        .AngularVelocity;

                Vector3D ˎ =
                    n.ą(
                        ˍ,
                        ˈ);

                ˇ -=
                    ˎ *
                    ˏ;
            }
        }

        if (ˇ
                .LengthSquared() <=
            Ų *
            Ų)
        {
            ɭ();
            return;
        }

        for (int Į = 0;
            Į < ˁ.Count;
            Į++)
        {
            ˁ[Į]
                .Ŵ(
                    ˇ);
        }
    }

    void ɭ()
    {
        for (int Į = 0;
            Į < ˁ.Count;
            Į++)
        {
            ˁ[Į]
                .Ū();
        }
    }

    void ˮ()
    {
        if (!ȟ.ȭ)
        {
            ː();
            return;
        }

        if (Ȏ !=
                ƅ.Ƃ ||
            ɡ == null ||
            !ɡ
                .IsUnderControl)
        {
            ː();
            return;
        }

        if (ˑ != null &&
            ˑ.EntityId !=
                ɡ.EntityId)
        {
            ˠ(
                ˑ);
        }

        ˑ =
            ɡ;

        StringBuilder ɏ =
            new StringBuilder();

        ɏ
            .Append('[')
            .Append(ˡ)
            .AppendLine("]");

        ɏ
            .Append("Version=")
            .AppendLine(
                ˢ);

        ɏ
            .Append(
"MasterProgrammableBlockId=")
            .AppendLine(
                Me.EntityId.ToString());

        ɏ
            .Append("ControllerId=")
            .AppendLine(
                ɡ
                    .EntityId
                    .ToString());

        ɏ
            .Append("Sequence=")
            .AppendLine(
                ˣ
                    .ToString());

        ɏ
            .Append("Demand=")
            .AppendLine(
                ˤ(
                    ɻ));

        ɏ
            .Append("Dampeners=")
            .AppendLine(
                Ȍ
                    .ToString());

        ɏ
            .Append("Cruise=")
            .AppendLine(
                Ȕ.ToString());

        ɏ
            .Append(
"CruiseTargetSpeed=")
            .AppendLine(
                ȝ
                    .ToString("R"));

        ɏ
            .Append("GearIndex=")
            .AppendLine(
                Ȣ
                    .ToString());

        ɏ
            .Append("GearCount=")
            .AppendLine(
                ȟ
                    .Ƞ
                    .Count
                    .ToString());

        ɏ
            .Append("GearFraction=")
            .AppendLine(
                ʝ
                    .ToString("R"));

        ɏ
            .Append(
"LevelWithGravity=")
            .AppendLine(
                (Ȕ &&
                 ȟ
                     .Ȱ)
                .ToString());

        ɡ.CustomData =
            ˬ(
                ɡ
                    .CustomData,
                ˡ,
                ɏ.ToString());
    }

    void ː()
    {
        if (ˑ ==
            null)
        {
            return;
        }

        ˠ(
            ˑ);

        ˑ =
            null;
    }

    void ˠ(
        IMyShipController Ͱ)
    {
        if (Ͱ == null ||
            Ͱ.Closed)
        {
            return;
        }

        string ͱ;

        if (!Ͳ(
                Ͱ.CustomData,
                ˡ,
"MasterProgrammableBlockId",
                out ͱ))
        {
            return;
        }

        long ͳ;

        if (!long.TryParse(
                ͱ,
                out ͳ) ||
            ͳ !=
                Me.EntityId)
        {
            return;
        }

        Ͱ.CustomData =
            ʹ(
                Ͱ.CustomData,
                ˡ);
    }

    void ͼ(
        IMyShipController Ͷ)
    {
        long ͷ =
            Ͷ !=
                    null
                ? Ͷ
                    .EntityId
                : 0;

        for (int Į = 0;
            Į < ͺ.Count;
            Į++)
        {
            IMyShipController Ͱ =
                ͺ[Į];

            if (Ͱ == null ||
                Ͱ.EntityId ==
                    ͷ)
            {
                continue;
            }

            ˠ(
                Ͱ);
        }

        for (int Į = 0;
            Į <
                ͻ
                    .Count;
            Į++)
        {
            IMyShipController Ͱ =
                ͻ[Į];

            if (Ͱ == null ||
                Ͱ.EntityId ==
                    ͷ)
            {
                continue;
            }

            ˠ(
                Ͱ);
        }
    }

    void Ά()
    {
        IMyShipController ͽ =
            Ȏ ==
                    ƅ.Ƃ &&
                ɡ !=
                    null &&
                ɡ
                    .IsUnderControl
                ? ɡ
                : null;

        ͼ(
            ͽ);

        if (ͽ == null)
        {
            ˑ =
                null;
        }
        else
        {
            ˑ =
                ͽ;
        }
    }


    void ɣ()
    {
        for (int Į = 0;
            Į <
                ͻ
                    .Count;
            Į++)
        {
            IMyShipController Ͱ =
                ͻ[Į];

            if (Ͱ == null ||
                Ͱ.Closed ||
                !Ͱ.IsUnderControl)
            {
                continue;
            }

            Ƶ Ƿ;

            if (!Έ(
                    Ͱ,
                    out Ƿ))
            {
                continue;
            }

            Ή(
                Ƿ);

            return;
        }
    }

    void Ί()
    {
        for (int Į = 0;
            Į <
                ͻ
                    .Count;
            Į++)
        {
            IMyShipController Ͱ =
                ͻ[Į];

            if (Ͱ == null ||
                Ͱ.Closed ||
                !Ͱ.IsUnderControl)
            {
                continue;
            }

            if (ɱ
                    .Ƭ != 0 &&
                Ͱ.EntityId !=
                    ɱ
                        .Ƭ)
            {
                continue;
            }

            Ƶ Ƿ;

            if (!Έ(
                    Ͱ,
                    out Ƿ))
            {
                continue;
            }

            if (ɱ
                    .ƫ !=
                    0 &&
                Ƿ
                    .ƫ !=
                ɱ
                    .ƫ)
            {
                continue;
            }

            Ή(
                Ƿ);

            return;
        }
    }

    void Ή(
        Ƶ Ƿ)
    {
        bool Ύ =
            Ƿ.ƭ !=
                ɦ ||
            Ƿ
                .ƫ !=
                Ό ||
            Ƿ.Ƭ !=
                ɱ
                    .Ƭ;

        if (Ύ)
        {
            ɦ =
                Ƿ.ƭ;

            Ό =
                Ƿ
                    .ƫ;

            ɥ = 0;

            ɤ =
                true;

            ɧ =
                true;
        }

        ɱ.ƙ(
            Ƿ);

        if (Ȏ ==
            ƅ.ƃ)
        {
            ɞ();
            ɝ();
        }
    }

    bool Έ(
        IMyShipController Ͱ,
        out Ƶ Ƿ)
    {
        Ƿ =
            null;

        if (Ͱ == null ||
            Ͱ.Closed ||
            !Ͱ.IsUnderControl)
        {
            return false;
        }

        string Ώ;

        if (!Ͳ(
                Ͱ.CustomData,
                ˡ,
"Version",
                out Ώ) ||
            string.IsNullOrWhiteSpace(Ώ))
        {
            return false;
        }

        if (ΐ(Ώ) != ΐ(ˢ))
        {
            return false;
        }
            
        string Α;
        string Β;
        string Γ;
        string Δ;

        if (!Ͳ(
                Ͱ.CustomData,
                ˡ,
"MasterProgrammableBlockId",
                out Α) ||
            !Ͳ(
                Ͱ.CustomData,
                ˡ,
"ControllerId",
                out Β) ||
            !Ͳ(
                Ͱ.CustomData,
                ˡ,
"Sequence",
                out Γ) ||
            !Ͳ(
                Ͱ.CustomData,
                ˡ,
"Demand",
                out Δ))
        {
            return false;
        }

        long ͱ;
        long Ε;
        long Ζ;
        Vector3D ɲ;

        if (!long.TryParse(
                Α,
                out ͱ) ||
            !long.TryParse(
                Β,
                out Ε) ||
            !long.TryParse(
                Γ,
                out Ζ) ||
            !Η(
                Δ,
                out ɲ))
        {
            return false;
        }

        if (ͱ ==
                Me.EntityId ||
            Ε !=
                Ͱ.EntityId)
        {
            return false;
        }

        string Θ;
        string Ι;
        string Κ;
        string Λ;
        string Μ;
        string Ν;
        string Ξ;

        Ͳ(
            Ͱ.CustomData,
            ˡ,
"Dampeners",
            out Θ);

        Ͳ(
            Ͱ.CustomData,
            ˡ,
"Cruise",
            out Ι);

        Ͳ(
            Ͱ.CustomData,
            ˡ,
"CruiseTargetSpeed",
            out Κ);

        Ͳ(
            Ͱ.CustomData,
            ˡ,
"GearIndex",
            out Λ);

        Ͳ(
            Ͱ.CustomData,
            ˡ,
"GearCount",
            out Μ);

        Ͳ(
            Ͱ.CustomData,
            ˡ,
"GearFraction",
            out Ν);

        Ͳ(
            Ͱ.CustomData,
            ˡ,
"LevelWithGravity",
            out Ξ);

        bool Ο =
            true;

        bool Π;
        bool Ρ;

        double Σ;
        double Τ;

        int Υ;
        int Φ;

        bool Χ;

        if (bool.TryParse(
                Θ,
                out Χ))
        {
            Ο =
                Χ;
        }

        bool.TryParse(
            Ι,
            out Π);

        bool.TryParse(
            Ξ,
            out Ρ);

        double.TryParse(
            Κ,
            out Σ);

        double.TryParse(
            Ν,
            out Τ);

        int.TryParse(
            Λ,
            out Υ);

        int.TryParse(
            Μ,
            out Φ);

        Ƿ =
            new Ƶ
            {
                ƫ =
                    ͱ,

                Ƭ =
                    Ε,

                ƭ =
                    Ζ,

                Ʈ =
                    n.Ŭ(
                        ɲ,
                        1),

                Ư =
                    Ο,

                Ɖ =
                    Π,

                Ʊ =
                    Σ,

                Ƴ =
                    Math.Max(
                        0,
                        Υ),

                ƴ =
                    Math.Max(
                        0,
                        Φ),

                Ʋ =
                    MathHelper.Clamp(
                        Τ,
                        0,
                        1),

                ư =
                    Ρ
            };

        return true;
    }

    void Ψ()
    {
        ɧ =
            false;

        ɤ =
            false;

        ɥ = 0;

        ɦ =
            long.MinValue;

        Ό =
            0;

        ɱ.ĺ();
    }

    static int ΐ(string Ω)
    {
        if (string.IsNullOrWhiteSpace(Ω))
        {
            return -1;
        }

        int Ϊ = Ω.IndexOf('.');

        string Ϋ =
            Ϊ >= 0
                ? Ω.Substring(0, Ϊ)
                : Ω;

        int ά;

        return int.TryParse(Ϋ, out ά)
            ? ά
            : -1;
    }


    static int δ(
        string ȼ,
        string έ)
    {
        if (string.IsNullOrEmpty(
                ȼ))
        {
            return -1;
        }

        string ή =
"["+
            έ +
"]";

        int ί = 0;

        while (ί <
               ȼ.Length)
        {
            int ΰ =
                ȼ.IndexOf(
                    ή,
                    ί,
                    StringComparison
                        .OrdinalIgnoreCase);

            if (ΰ < 0)
            {
                return -1;
            }

            bool α =
                ΰ == 0 ||
                ȼ[
                    ΰ - 1] ==
                '\n';

            int β =
                ΰ +
                ή.Length;

            bool γ =
                β >=
                    ȼ.Length ||
                ȼ[β] ==
                    '\r' ||
                ȼ[β] ==
                    '\n';

            if (α &&
                γ)
            {
                return ΰ;
            }

            ί =
                ΰ + 1;
        }

        return -1;
    }

    static int θ(
        string ȼ,
        int ί)
    {
        while (ί <
               ȼ.Length)
        {
            int ε =
                ȼ.IndexOf(
                    '\n',
                    ί);

            if (ε < 0 ||
                ε + 1 >=
                    ȼ.Length)
            {
                return ȼ.Length;
            }

            ε++;

            int ζ =
                ε;

            while (ζ <
                       ȼ.Length &&
                   (ȼ[ζ] ==
                        ' ' ||
                    ȼ[ζ] ==
                        '\t' ||
                    ȼ[ζ] ==
                        '\r'))
            {
                ζ++;
            }

            if (ζ <
                    ȼ.Length &&
                ȼ[ζ] ==
                    '[')
            {
                int η =
                    ȼ.IndexOf(
                        ']',
                        ζ + 1);

                if (η >= 0)
                {
                    return ε;
                }
            }

            ί =
                ε;
        }

        return ȼ.Length;
    }

    static bool Ͳ(
        string ȼ,
        string έ,
        string ɐ,
        out string ğ)
    {
        ğ =
            null;

        int ι =
            δ(
                ȼ,
                έ);

        if (ι < 0)
        {
            return false;
        }

        int κ =
            θ(
                ȼ,
                ι +
                έ.Length +
                2);

        int λ =
            ȼ.IndexOf(
                '\n',
                ι);

        if (λ < 0 ||
            λ >= κ)
        {
            return false;
        }

        string ɏ =
            ȼ.Substring(
                λ + 1,
                κ -
                λ -
                1);

        string[] μ =
            ɏ
                .Replace(
"\r",
                    string.Empty)
                .Split('\n');

        for (int Į = 0;
            Į < μ.Length;
            Į++)
        {
            string ν =
                μ[Į];

            int Ϊ =
                ν.IndexOf('=');

            if (Ϊ <= 0)
            {
                continue;
            }

            string ξ =
                ν
                    .Substring(
                        0,
                        Ϊ)
                    .Trim();

            if (!ξ.Equals(
                    ɐ,
                    StringComparison
                        .OrdinalIgnoreCase))
            {
                continue;
            }

            ğ =
                ν
                    .Substring(
                        Ϊ + 1)
                    .Trim();

            return true;
        }

        return false;
    }

    static string ˬ(
        string ȼ,
        string έ,
        string ο)
    {
        ȼ =
            ȼ ??
            string.Empty;

        ο =
            ο.TrimEnd(
                '\r',
                '\n') +
"\n";

        int ι =
            δ(
                ȼ,
                έ);

        if (ι < 0)
        {
            if (ȼ.Length == 0)
            {
                return ο;
            }

            string Ϊ =
                ȼ.EndsWith(
"\n")
                    ? string.Empty
                    : "\n";

            return ȼ +
                   Ϊ +
                   ο;
        }

        int κ =
            θ(
                ȼ,
                ι +
                έ.Length +
                2);

        return ȼ.Substring(
                   0,
                   ι) +
               ο +
               ȼ.Substring(
                   κ);
    }

    static string ʹ(
        string ȼ,
        string έ)
    {
        if (string.IsNullOrEmpty(
                ȼ))
        {
            return ȼ;
        }

        int ι =
            δ(
                ȼ,
                έ);

        if (ι < 0)
        {
            return ȼ;
        }

        int κ =
            θ(
                ȼ,
                ι +
                έ.Length +
                2);

        string π =
            ȼ.Substring(
                0,
                ι);

        string β =
            ȼ.Substring(
                κ);

        if (π.EndsWith(
"\n") &&
            β.StartsWith(
"\n"))
        {
            β =
                β.Substring(1);
        }

        return π +
               β;
    }

    static string ˤ(
        Vector3D ǁ)
    {
        return ǁ.X
                   .ToString("R") +
";"+
               ǁ.Y
                   .ToString("R") +
";"+
               ǁ.Z
                   .ToString("R");
    }

    static bool Η(
        string Ȧ,
        out Vector3D ǁ)
    {
        ǁ =
            Vector3D.Zero;

        if (string.IsNullOrWhiteSpace(
                Ȧ))
        {
            return false;
        }

        string[] ρ =
            Ȧ.Split(';');

        if (ρ.Length !=
            3)
        {
            return false;
        }

        double ς;
        double σ;
        double τ;

        if (!double.TryParse(
                ρ[0],
                out ς) ||
            !double.TryParse(
                ρ[1],
                out σ) ||
            !double.TryParse(
                ρ[2],
                out τ))
        {
            return false;
        }

        ǁ =
            new Vector3D(
                ς,
                σ,
                τ);

        return true;
    }
    IMyShipController ˑ;


    void ɛ()
    {
        IMyShipController υ =
            ɡ;

        IMyShipController φ =
            null;

        for (int Į = 0;
            Į < ͺ.Count;
            Į++)
        {
            IMyShipController Ͱ =
                ͺ[Į];

            if (Ͱ == null ||
                Ͱ.Closed ||
                !Ͱ.IsFunctional ||
                !Ͱ.CanControlShip)
            {
                continue;
            }

            if (Ͱ.IsUnderControl)
            {
                φ =
                    Ͱ;

                break;
            }

            if (φ == null ||
                Ͱ.IsMainCockpit)
            {
                φ =
                    Ͱ;
            }
        }

        long χ =
            υ != null
                ? υ
                    .EntityId
                : 0;

        long ψ =
            φ != null
                ? φ.EntityId
                : 0;

        if (ˑ != null &&
            ˑ.EntityId !=
                ψ)
        {
            ˠ(
                ˑ);

            ˑ =
                null;
        }

        if (χ !=
            ψ)
        {
            ɻ =
                Vector3D.Zero;

            ω();
        }

        ɡ =
            φ;

        ϊ =
            ɡ ==
            null;

        ɠ =
            ȟ.ȭ &&
            ɡ !=
                null &&
            ɡ
                .IsUnderControl;
    }


    void ɨ()
    {
        ɛ();

        if (Ȏ ==
                ƅ.ƃ &&
            !ɧ)
        {
            Ȉ =
                ϋ;
        }

        ƅ ό;

        if (ϊ ||
            Me.CubeGrid.IsStatic ||
            ȇ)
        {
            ό =
                ƅ.Ƅ;
        }
        else if (ȟ.Ȯ &&
                 ɧ &&
                 !ɠ)
        {
            ό =
                ƅ.ƃ;
        }
        else if (ύ ||
                 Ȉ)
        {
            ό =
                ƅ.Ƅ;
        }
        else if (ɠ)
        {
            ό =
                ƅ.Ƃ;
        }
        else
        {
            ό =
                ƅ.Ɓ;
        }

        if (ό ==
            Ȏ)
        {
            ɝ();
            return;
        }

        ώ(
            ό);
    }

    void ώ(
        ƅ Ϗ)
    {
        ƅ ϐ =
            Ȏ;

        if (ϐ ==
                ƅ.Ƃ &&
            Ϗ !=
                ƅ.Ƃ)
        {
            ː();
            ɻ =
                Vector3D.Zero;
        }

        if (ϐ ==
                ƅ.Ƅ &&
            Ϗ !=
                ƅ.Ƅ)
        {
            ϑ();
        }

        if (ϐ ==
                ƅ.ƃ &&
            Ϗ !=
                ƅ.ƃ &&
            !ɧ)
        {
            Ȉ =
                ϋ;
        }

        Ȏ =
            Ϗ;

        if (Ϗ ==
            ƅ.ƃ)
        {
            ϋ =
                ϐ ==
                ƅ.Ƅ;

            Ȉ =
                false;
        }

        ϒ(
            ϐ,
            Ϗ);

        if (Ϗ ==
                ƅ.Ƅ &&
            ϐ !=
                ƅ.Ƅ)
        {
            Ĉ();
        }

        ϓ =
            true;
    }


    void ɢ()
    {
        if (ȟ.ȯ)
        {
            ύ =
                false;

            return;
        }

        bool ϔ =
            false;

        for (int Į = 0;
            Į < ϕ.Count;
            Į++)
        {
            if (ϖ(
                    ϕ[Į]))
            {
                ϔ =
                    true;

                break;
            }
        }

        if (!ϔ)
        {
            for (int Į = 0;
                Į <
                    ϗ.Count;
                Į++)
            {
                if (Ϙ(
                        ϗ[Į]))
                {
                    ϔ =
                        true;

                    break;
                }
            }
        }

        ύ =
            ϔ;
    }

    bool ϖ(
        ǯ ϙ)
    {
        IMyShipConnector Ϛ =
            ϙ.ǭ;

        if (Ϛ == null ||
            Ϛ.Closed ||
            Ϛ.Status !=
                MyShipConnectorStatus
                    .Connected)
        {
            return false;
        }

        IMyShipConnector Ɨ =
            Ϛ.OtherConnector;

        if (Ɨ == null)
        {
            return false;
        }

        ǘ ϛ;

        if (!Ϝ.TryGetValue(
                Ɨ.CubeGrid.EntityId,
                out ϛ))
        {
            return Ɨ
                .CubeGrid
                .IsStatic;
        }

        ǖ Ȝ =
            ϛ.Ǘ;

        if (Ȝ == null)
        {
            return Ɨ
                .CubeGrid
                .IsStatic;
        }

        if (Ȝ.ǥ)
        {
            return true;
        }

        if (Ȝ.Ǣ.Count ==
            0)
        {
            return false;
        }

        if (ɠ &&
            Ȝ.Ǧ)
        {
            return false;
        }

        return true;
    }

    bool Ϙ(
        ǰ ϝ)
    {
        IMyLandingGear Ϟ =
            ϝ.ǭ;

        if (Ϟ == null ||
            Ϟ.Closed ||
            !Ϟ.IsFunctional)
        {
            return false;
        }

        return Ϟ.IsLocked;
    }


    void Ĉ()
    {
        ɬ();
        ɭ();

        Vector3D ʔ =
            ɡ !=
                    null
                ? ɡ
                    .GetNaturalGravity()
                : Vector3D.Zero;

        Vector3D û =
            Me.CubeGrid
                .WorldAABB
                .Center;

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            if (!İ.P)
            {
                continue;
            }

            İ.j();

            ˀ(
                İ,
                ƌ.Ɗ);
        }

        ü.Clear();

        for (int Į = 0;
            Į < ϟ.Count;
            Į++)
        {
            Ò Ī =
                ϟ[Į];

            if (!Ī.P)
            {
                continue;
            }

            Ī.Ĉ(
                ʔ,
                û);
        }

        Ϡ(
            ϡ);
            
        Save();
    }

    void ϑ()
    {
        ʾ(
            ƌ.Ɗ);

        Ϣ.Clear();

        for (int Į = 0;
            Į < ϟ.Count;
            Į++)
        {
            Ò Ī =
                ϟ[Į];

            Ī.Þ();
            Ī.Ó();
        }

        ü.Clear();

        ɻ =
            Vector3D.Zero;

        Ϡ(
            ϣ);
            
        Save();
    }

    void ϥ()
    {
        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            Ϥ(
                İ);

            if (!İ.P)
            {
                continue;
            }

            İ.j();

            ˀ(
                İ,
                ƌ.Ɗ);
        }

        ɭ();

        Vector3D ʔ =
            ɡ !=
                    null
                ? ɡ
                    .GetNaturalGravity()
                : Vector3D.Zero;

        Vector3D û =
            Me.CubeGrid
                .WorldAABB
                .Center;

        for (int Į = 0;
            Į < ϟ.Count;
            Į++)
        {
            Ò Ī =
                ϟ[Į];

            if (!Ī.P)
            {
                Ī.Ó();
                continue;
            }

            double ĉ;

            if (ü
                    .TryGetValue(
                        Ī.Q,
                        out ĉ))
            {
                Ī.Ċ(
                    ĉ);
            }
            else
            {
                Ī.Ĉ(
                    ʔ,
                    û);
            }

            Ī.Đ();
        }
    }

    void ɩ()
    {
        for (int Į = 0;
            Į < ϟ.Count;
            Į++)
        {
            ϟ[Į]
                .Đ();
        }
    }

    void ɬ()
    {
        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            if (ʆ[Į].P)
            {
                ʆ[Į]
                    .j();
            }
        }
    }

    void ɮ()
    {
        ʾ(
            ƌ.Ɗ);

        Ϣ.Clear();
    }

    void ϧ(
        long Ϧ,
        IMyThrust d)
    {
        ʾ(
            Ϧ,
            d,
            ƌ.Ɗ);
    }

    void Ϡ(
        List<IMyTimerBlock> Ϩ)
    {
        for (int Į = 0;
            Į < Ϩ.Count;
            Į++)
        {
            IMyTimerBlock ϩ =
                Ϩ[Į];

            if (ϩ == null ||
                ϩ.Closed ||
                !ϩ.IsFunctional)
            {
                continue;
            }

            ϩ.Trigger();
        }
    }

    const string ȿ = "Vector Thrust Redux",ˢ = "0.2.0",Ɉ = "Vector Thrust Redux",ˡ = "Vector Thrust Redux Heartbeat",Ϫ = "VT-Redux:",ϫ = "State",Ϭ = "Disabled Thrusters",ϭ = "Park Rotor Targets",Ϯ = "Topology";


    const double p = 1e-8,U = 1e-3,ē = 1e-4,Ð = 1e-4,Ķ = 1.0 - 1e-6,ϯ = 1.0 - 1e-4,þ = 1.0 - 1e-4,å = 0.1,č = 1.0,æ = 4.0,Ğ = Math.PI,ď = 1e-3,ʢ = 0.01,µ = 0.0075,ˌ = 4.0,ˏ = 1.5,ŭ = 30.0,Ų = 1e-3,ŧ = 448000.0,Ũ = 33600000.0,ť = 4480000.0,Ŧ = 201600000.0,è = 1.0 / 120.0,ϰ = 0.25;


    readonly ȹ ȟ = new ȹ();
    readonly MyIni ϱ = new MyIni();

    string Ƚ = string.Empty,ϲ = string.Empty;

    bool Ȕ,ύ,ϓ;
    bool Ȍ = true,ȇ,Ȟ,ϳ,ϊ = true,ɠ,ɧ,Ȉ,ϋ,ɤ,ϴ,ϵ = true,Ϸ;
    int Ȣ,ɥ,ϸ,Ϲ;

    double ȝ,ʅ;

    readonly Dictionary<long, Ə> Ϻ =
        new Dictionary<long, Ə>();

    readonly Dictionary<long, double> ü =
        new Dictionary<long, double>();

    Ɯ ϻ,ϼ;

    readonly Dictionary<long, bool> Ϣ =
        new Dictionary<long, bool>();


    ƅ Ȏ = ƅ.ƀ;

    IMyShipController ɡ;

    long ˣ;
    long ɦ = long.MinValue,Ό;

    Ƶ ɱ = new Ƶ();

    Vector3D ɼ;
    Vector3D ʵ,ɻ,ʀ;
    double Ü,Ͻ;

    int Ͼ;
    string Ǽ = string.Empty;

    bool Ͽ;


    readonly Ɩ ʇ =
        new Ɩ(),ʈ =
        new Ɩ(),ʉ =
        new Ɩ();


    readonly List<IMyShipController> ͺ =
        new List<IMyShipController>(),ͻ =
        new List<IMyShipController>();

    readonly List<g> ʆ =
        new List<g>(),Ѐ =
        new List<g>(),Ё =
        new List<g>(),Ђ =
        new List<g>(),ʲ =
        new List<g>(),Ѓ =
        new List<g>(),ʳ =
        new List<g>(),ʿ =
        new List<g>();

    readonly List<Ò> ϟ =
        new List<Ò>();

    readonly List<H> ʂ =
        new List<H>();

    readonly List<ŝ> Є =
        new List<ŝ>();

    readonly List<ũ> ˁ =
        new List<ũ>();

    readonly List<ǯ> ϕ =
        new List<ǯ>();

    readonly List<ǰ> ϗ =
        new List<ǰ>();

    readonly List<IMyTimerBlock> ϡ =
        new List<IMyTimerBlock>(),ϣ =
        new List<IMyTimerBlock>();

    readonly List<Ž> Ѕ =
        new List<Ž>();

    readonly Dictionary<long, ǘ> Ϝ =
        new Dictionary<long, ǘ>();

    readonly StringBuilder І =
        new StringBuilder(),Ї =
        new StringBuilder();

    IEnumerator<int> Ј;

    ƽ Љ;

    public Program()
    {
        Љ =
            new ƽ(this);

        Њ();
        Ɏ(true);

        Runtime.UpdateFrequency =
            UpdateFrequency.Update1 |
            UpdateFrequency.Update10 |
            UpdateFrequency.Update100;

        Ȋ();
    }

    public void Save()
    {
        ϱ.Clear();

        ϱ.Set(
            ϫ,
"Cruise",
            Ȕ);

        ϱ.Set(
            ϫ,
"Dampeners",
            Ȍ);

        ϱ.Set(
            ϫ,
"ManualPark",
            ȇ);

        ϱ.Set(
            ϫ,
"Gear",
            Ȣ);

        ϱ.Set(
            ϫ,
"CruiseTargetSpeed",
            ȝ);

        ϱ.Set(
            ϫ,
"CruiseTargetInitialized",
            Ȟ);

        foreach (
            KeyValuePair<long, Ə> Ћ
            in Ϻ)
        {
            Ə Ȓ =
                Ћ.Value;

            string Ȧ =
                (Ȓ.ƍ
                    ? "1"                    : "0") +
";"+
                ((int)Ȓ.Ǝ).ToString();

            ϱ.Set(
                Ϭ,
                Ћ.Key.ToString(),
                Ȧ);
        }

        foreach (
            KeyValuePair<long, double> Ћ
            in ü)
        {
            ϱ.Set(
                ϭ,
                Ћ.Key.ToString(),
                Ћ.Value);
        }

        if (Ϸ)
        {
            ϱ.Set(
                Ϯ,
"Count",
                ϼ.Ɲ);

            ϱ.Set(
                Ϯ,
"Xor",
                ϼ.ƞ.ToString());

            ϱ.Set(
                Ϯ,
"Sum",
                ϼ.Ɵ.ToString());
        }
        else if (ϳ)
        {
            ϱ.Set(
                Ϯ,
"Count",
                ϻ.Ɲ);

            ϱ.Set(
                Ϯ,
"Xor",
                ϻ.ƞ.ToString());

            ϱ.Set(
                Ϯ,
"Sum",
                ϻ.Ɵ.ToString());
        }

        Storage =
            ϱ.ToString();
    }

    public void Main(
        string Ǳ,
        UpdateType Ќ)
    {
        Љ.ƾ();

        double Ѝ =
            Runtime.TimeSinceLastRun.TotalSeconds;

        if (Ѝ <
            è)
        {
            Ѝ =
                è;
        }
        else if (Ѝ >
                 ϰ)
        {
            Ѝ =
                ϰ;
        }

        Ͻ +=
            Ѝ;

        bool Ў =
            (Ќ &
             (UpdateType.Terminal |
              UpdateType.Trigger |
              UpdateType.Script)) != 0 ||
            !string.IsNullOrWhiteSpace(
                Ǳ);

        Џ();

        if (Ў)
        {
            ɛ();
            Ǿ(Ǳ);
        }

        if ((Ќ &
             UpdateType.Update100) != 0 &&
            А(
                ref Ϲ,
                ȟ.ȸ))
        {
            ɚ();
        }

        if ((Ќ &
             UpdateType.Update10) != 0 &&
            А(
                ref ϸ,
                ȟ.ȷ))
        {
            ɫ();
        }

        if ((Ќ &
             UpdateType.Update1) != 0)
        {
            ˣ++;

            if (Ȏ ==
                ƅ.ƃ)
            {
                Ί();
            }

            ɨ();

            if (А(
                    ref Ͼ,
                    ȟ.ȶ))
            {
                Ü =
                    MathHelper.Clamp(
                        Ͻ,
                        è,
                        ϰ);

                Ͻ = 0;

                ʁ(
                    Ü);
            }

            if (ȟ.ȭ ||
                ˑ != null)
            {
                ˮ();
            }
        }

        if (Ў)
        {
            ɨ();

            if (ȟ.ȭ ||
                ˑ != null)
            {
                ˮ();
            }

            ϓ = true;
        }

        if (ϓ)
        {
            ɪ(true);
            ϓ = false;
        }

        Љ.ǀ();
    }

    static bool А(
        ref int Б,
        int В)
    {
        if (Б <
            В)
        {
            Б++;
            return false;
        }

        Б = 0;
        return true;
    }

    void Њ()
    {
        if (string.IsNullOrWhiteSpace(
                Storage))
        {
            return;
        }

        MyIniParseResult Ⱦ;

        if (!ϱ.TryParse(
                Storage,
                out Ⱦ))
        {
            return;
        }

        Ȕ =
            ϱ
                .Get(
                    ϫ,
"Cruise")
                .ToBoolean(false);

        Ȍ =
            ϱ
                .Get(
                    ϫ,
"Dampeners")
                .ToBoolean(true);

        ȇ =
            ϱ
                .Get(
                    ϫ,
"ManualPark")
                .ToBoolean(false);

        Ȣ =
            Math.Max(
                0,
                ϱ
                    .Get(
                        ϫ,
"Gear")
                    .ToInt32(0));

        ȝ =
            ϱ
                .Get(
                    ϫ,
"CruiseTargetSpeed")
                .ToDouble(0);

        Ȟ =
            ϱ
                .Get(
                    ϫ,
"CruiseTargetInitialized")
                .ToBoolean(false);

        Г();
        Д();
        Е();
    }

    void Г()
    {
        List<MyIniKey> Ж =
            new List<MyIniKey>();

        ϱ.GetKeys(
            Ϭ,
            Ж);

        for (int Į = 0;
            Į < Ж.Count;
            Į++)
        {
            MyIniKey ɐ =
                Ж[Į];

            long Ϧ;

            if (!long.TryParse(
                    ɐ.Name,
                    out Ϧ))
            {
                continue;
            }

            string Ȧ =
                ϱ
                    .Get(ɐ)
                    .ToString();

            string[] ρ =
                Ȧ.Split(';');

            if (ρ.Length != 2)
            {
                continue;
            }

            bool З =
                ρ[0] == "1";

            int И;

            if (!int.TryParse(
                    ρ[1],
                    out И))
            {
                continue;
            }

            ƌ Й =
                (ƌ)
                    И;

            if (Й ==
                ƌ.M)
            {
                continue;
            }

            Ϻ[
                Ϧ] =
                new Ə
                {
                    ƍ =
                        З,
                    Ǝ = Й
                };

            if ((Й &
                 ƌ.Ɗ) != 0)
            {
                Ϣ[
                    Ϧ] =
                    З;
            }
        }
    }

    void Д()
    {
        List<MyIniKey> Ж =
            new List<MyIniKey>();

        ϱ.GetKeys(
            ϭ,
            Ж);

        for (int Į = 0;
            Į < Ж.Count;
            Į++)
        {
            MyIniKey ɐ =
                Ж[Į];

            long Ϧ;

            if (!long.TryParse(
                    ɐ.Name,
                    out Ϧ))
            {
                continue;
            }

            double Ȝ =
                ϱ
                    .Get(ɐ)
                    .ToDouble(
                        double.NaN);

            if (double.IsNaN(Ȝ) ||
                double.IsInfinity(Ȝ))
            {
                continue;
            }

            ü[
                Ϧ] =
                Ȝ;
        }
    }

    void Е()
    {
        long К =
            ϱ
                .Get(
                    Ϯ,
"Count")
                .ToInt64(-1);

        string Л =
            ϱ
                .Get(
                    Ϯ,
"Xor")
                .ToString();

        string М =
            ϱ
                .Get(
                    Ϯ,
"Sum")
                .ToString();

        ulong Н;
        ulong О;

        if (К < 0 ||
            !ulong.TryParse(
                Л,
                out Н) ||
            !ulong.TryParse(
                М,
                out О))
        {
            return;
        }

        ϻ =
            new Ɯ
            {
                Ɲ = К,
                ƞ = Н,
                Ɵ = О
            };

        ϳ =
            true;
    }
    IEnumerable<int> Я(
        П Р)
    {

        for (int Į = 0;
            Į <
                Р.С.Count;
            Į++)
        {
            IMyShipConnector d =
                Р.С[Į];

            ǘ ǟ;

            if (!Р
                    .Т
                    .TryGetValue(
                        d
                            .CubeGrid
                            .EntityId,
                        out ǟ) ||
                !ǟ.ǜ)
            {
                continue;
            }

            Р
                .У
                .Add(d);

            F e =
                Ф(
                    Р.G,
                    d.EntityId);

            if (!Х(
                    e))
            {
                continue;
            }

            Р
                .Ц
                .Add(
                    new ǯ
                    {
                        ǭ =
                            d,

                        Ǯ =
                            Ч(
                                Р
                                    .Ш,
                                d)
                    });

            yield return 1;
        }


        for (int Į = 0;
            Į <
                Р
                    .Щ
                    .Count;
            Į++)
        {
            IMyLandingGear d =
                Р
                    .Щ[Į];

            ǘ ǟ;

            if (!Р
                    .Т
                    .TryGetValue(
                        d
                            .CubeGrid
                            .EntityId,
                        out ǟ) ||
                !ǟ.ǜ)
            {
                continue;
            }

            F e =
                Ф(
                    Р.G,
                    d.EntityId);

            if (!Х(
                    e))
            {
                continue;
            }

            Р
                .Ъ
                .Add(
                    new ǰ
                    {
                        ǭ =
                            d
                    });

            yield return 1;
        }


        for (int Į = 0;
            Į <
                Р.Ы.Count;
            Į++)
        {
            IMyTimerBlock ϩ =
                Р.Ы[Į];

            if (ϩ.CubeGrid !=
                Me.CubeGrid)
            {
                continue;
            }

            F e =
                Ф(
                    Р.G,
                    ϩ.EntityId);

            if ((e &
                 F.Ƈ) != 0)
            {
                Р.Ь.Add(
                    ϩ);
            }

            if ((e &
                 F.ƈ) !=
                0)
            {
                Р
                    .Э
                    .Add(ϩ);
            }

            yield return 1;
        }

        Ю(
            Р);
    }


    void ы(
        П Р)
    {
        HashSet<long> а =
            new HashSet<long>();

        HashSet<long> б =
            new HashSet<long>();

        HashSet<long> в =
            new HashSet<long>();

        for (int Į = 0;
            Į <
                Р.ĥ.Count;
            Į++)
        {
            а.Add(
                Р
                    .ĥ[Į]
                    .Q);
        }

        for (int Į = 0;
            Į <
                Р
                    .г
                    .Count;
            Į++)
        {
            б.Add(
                Р
                    .г[Į]
                    .Q);
        }

        for (int Į = 0;
            Į <
                Р.д.Count;
            Į++)
        {
            в.Add(
                Р
                    .д[Į]
                    .E
                    .EntityId);
        }

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g е =
                ʆ[Į];

            if (а.Contains(
                    е.Q))
            {
                continue;
            }

            е.À();

            ϧ(
                е.Q,
                е.E);

            ʾ(
                е.Q,
                е.E,
                ƌ
                    .Ɖ);

            ʾ(
                е.Q,
                е.E,
                ƌ
                    .Ƌ);
        }

        for (int Į = 0;
            Į < ϟ.Count;
            Į++)
        {
            Ò ж =
                ϟ[Į];

            if (!б
                    .Contains(
                        ж.Q))
            {
                ж.À();
            }
        }

        for (int Į = 0;
            Į < ˁ.Count;
            Į++)
        {
            ũ з =
                ˁ[Į];

            if (!в.Contains(
                    з
                        .E
                        .EntityId))
            {
                з.À();
            }
        }

        и(
            ͺ,
            Р.й);

        и(
            ͻ,
            Р.к);

        if (ͻ.Count == 0)
        {
            Ψ();
        }

        и(
            ʆ,
            Р.ĥ);

        и(
            Ѐ,
            Р.л);

        и(
            Ё,
            Р.м);

        и(
            Ђ,
            Р.н);

        и(
            ʲ,
            Р.о);

        и(
            Ѓ,
            Р.п);

        и(
            ʳ,
            Р.р);

        и(
            ʎ,
            Р.с);

        и(
            ϟ,
            Р.г);

        и(
            ʂ,
            Р.т);

        и(
            Є,
            Р.у);

        и(
            ʏ,
            Р.ф);

        и(
            ˁ,
            Р.д);

        и(
            ϕ,
            Р.Ц);

        и(
            ϗ,
            Р.Ъ);

        и(
            х,
            Р.Ш);

        и(
            ц,
            Р.У);

        и(
            ϡ,
            Р.Ь);

        и(
            ϣ,
            Р.Э);

        и(
            Ѕ,
            Р.ч);

        Ϝ.Clear();

        foreach (
            KeyValuePair<long, ǘ> Ћ
            in Р.Т)
        {
            Ϝ.Add(
                Ћ.Key,
                Ћ.Value);
        }

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            Ϥ(
                ʆ[Į]);
        }

        ш(
            а);

        ɛ();
        щ();
        ɝ();

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            if (!ʆ[Į].P)
            {
                ʆ[Į]
                    .j();
            }
        }

        for (int Į = 0;
            Į < ˁ.Count;
            Į++)
        {
            if (!ˁ[Į]
                    .P)
            {
                ˁ[Į]
                    .Ū();
            }
        }

        if (Ȏ ==
            ƅ.Ƅ)
        {
            ϥ();
        }

        Ά();

        ϼ =
            ъ();

        Ϸ =
            true;

        ϓ =
            true;
    }


    void Ю(
        П Р)
    {
        HashSet<string> ь =
            new HashSet<string>(
                StringComparer.Ordinal);

        List<int> э =
            new List<int>();

        for (int Į = 0;
            Į < Р.ю.Count;
            Į++)
        {
            IMyTerminalBlock d =
                Р.ю[Į];

            ǘ ǟ;

            if (!Р
                    .Т
                    .TryGetValue(
                        d
                            .CubeGrid
                            .EntityId,
                        out ǟ) ||
                ǟ.Ǘ !=
                    Р.я)
            {
                continue;
            }

            F e =
                Ф(
                    Р.G,
                    d.EntityId);

            IMyTextPanel ѐ =
                d as
                    IMyTextPanel;

            if (ѐ != null &&
                (e &
                 F.Ɔ) != 0)
            {
                ё(
                    Р
                        .ч,
                    ь,
                    d,
                    ѐ,
                    0);
            }

            IMyTextSurfaceProvider ђ =
                d as
                    IMyTextSurfaceProvider;

            if (ђ == null ||
                ђ.SurfaceCount <= 0)
            {
                continue;
            }

            э.Clear();

            ѓ(
                d.CustomData,
                ђ.SurfaceCount,
                э);

            if ((e &
                 F.Ɔ) != 0 &&
                э.Count ==
                    0)
            {
                э.Add(
                    0);
            }

            for (int ΰ = 0;
                ΰ <
                    э.Count;
                ΰ++)
            {
                int ż =
                    э[ΰ];

                ё(
                    Р
                        .ч,
                    ь,
                    d,
                    ђ.GetSurface(
                        ż),
                    ż);
            }
        }
    }

    void ѓ(
        string ȼ,
        int є,
        List<int> Ĺ)
    {
        if (string.IsNullOrEmpty(
                ȼ))
        {
            return;
        }

        string[] μ =
            ȼ
                .Replace(
"\r",
                    string.Empty)
                .Split('\n');

        for (int Į = 0;
            Į < μ.Length;
            Į++)
        {
            string ν =
                μ[Į].Trim();

            if (!ν.StartsWith(
                    Ϫ,
                    StringComparison
                        .OrdinalIgnoreCase))
            {
                continue;
            }

            string ѕ =
                ν
                    .Substring(
                        Ϫ
                            .Length)
                    .Trim();

            int ΰ;

            if (!int.TryParse(
                    ѕ,
                    out ΰ) ||
                ΰ < 0 ||
                ΰ >= є ||
                Ĺ.Contains(
                    ΰ))
            {
                continue;
            }

            Ĺ.Add(
                ΰ);
        }
    }

    static void ё(
        List<Ž> Ĺ,
        HashSet<string> w,
        IMyTerminalBlock ź,
        IMyTextSurface Ż,
        int ż)
    {
        string ɐ =
            ź.EntityId +
":"+
            ż;

        if (!w.Add(ɐ))
        {
            return;
        }

        Ĺ.Add(
            new Ž(
                ź,
                Ż,
                ż));
    }


    bool Х(
        F e)
    {
        if ((e &
             F.X) != 0)
        {
            return false;
        }

        return ȟ.Ȭ ||
               (e &
                F.Z) != 0;
    }

    bool ї(
        IMyProgrammableBlock і)
    {
        if (і ==
            null)
        {
            return false;
        }

        return δ(
                   і
                       .CustomData,
                   Ɉ) >=
               0;
    }

    bool ј(
        IMyProgrammableBlock і)
    {
        string Ȧ;

        if (!Ͳ(
                і.CustomData,
                Ɉ,
"CanSlave",
                out Ȧ))
        {
            return true;
        }

        bool ğ;

        return bool.TryParse(
                   Ȧ,
                   out ğ)
            ? ğ
            : true;
    }

    F њ(
        string ž)
    {
        if (string.IsNullOrEmpty(
                ž))
        {
            return F.M;
        }

        F Ǹ =
            F.M;

        if (љ(
                ž,
                ȟ.ȱ))
        {
            Ǹ |=
                F.Z;
        }

        if (љ(
                ž,
                ȟ.Ȳ))
        {
            Ǹ |=
                F.X;
        }

        if (љ(
                ž,
                ȟ.ȳ))
        {
            Ǹ |=
                F.Ɔ;
        }

        if (љ(
                ž,
                ȟ.ȴ))
        {
            Ǹ |=
                F.Ƈ;
        }

        if (љ(
                ž,
                ȟ.ȵ))
        {
            Ǹ |=
                F.ƈ;
        }

        return Ǹ;
    }

    static bool љ(
        string ž,
        string ћ)
    {
        return !string.IsNullOrEmpty(
                   ž) &&
               !string.IsNullOrEmpty(
                   ћ) &&
               ž.IndexOf(
                   ћ,
                   StringComparison
                       .OrdinalIgnoreCase) >=
               0;
    }

    static void ў(
        Dictionary<long, F> e,
        long Ϧ,
        F ќ)
    {
        if (ќ ==
            F.M)
        {
            return;
        }

        F ѝ;

        e.TryGetValue(
            Ϧ,
            out ѝ);

        e[Ϧ] =
            ѝ |
            ќ;
    }

    static F Ф(
        Dictionary<long, F> e,
        long Ϧ)
    {
        F Ǹ;

        return e.TryGetValue(
                   Ϧ,
                   out Ǹ)
            ? Ǹ
            : F.M;
    }


    static Ǭ Ч(
        List<Ǭ> џ,
        IMyShipConnector Ϛ)
    {
        for (int Į = 0;
            Į < џ.Count;
            Į++)
        {
            if (џ[Į]
                        .ƥ
                        .EntityId ==
                    Ϛ.EntityId ||
                џ[Į]
                        .Ʀ
                        .EntityId ==
                    Ϛ.EntityId)
            {
                return џ[Į];
            }
        }

        return null;
    }

    static void ѡ(
        List<IMyCubeGrid> Ѡ,
        IMyCubeGrid Ŗ)
    {
        for (int Į = 0;
            Į < Ѡ.Count;
            Į++)
        {
            if (Ѡ[Į]
                    .EntityId ==
                Ŗ.EntityId)
            {
                return;
            }
        }

        Ѡ.Add(
            Ŗ);
    }

    static void и<Ѣ>(
        List<Ѣ> Ȝ,
        List<Ѣ> ѣ)
    {
        Ȝ.Clear();
        Ȝ.AddRange(
            ѣ);
    }
    IEnumerable<int> Ѳ(
        П Р)
    {
        Dictionary<long, Ò> Ѥ =
            new Dictionary<long, Ò>();

        Dictionary<long, IMyMotorStator> ѥ =
            new Dictionary<long, IMyMotorStator>();

        Dictionary<long, List<g>> Ѧ =
            new Dictionary<long, List<g>>();

        Dictionary<long, List<g>> ѧ =
            new Dictionary<long, List<g>>();


        for (int Į = 0;
            Į <
                Р.Ѩ.Count;
            Į++)
        {
            IMyMotorStator d =
                Р.Ѩ[Į];

            ǘ ǟ;

            if (!Р
                    .Т
                    .TryGetValue(
                        d
                            .CubeGrid
                            .EntityId,
                        out ǟ))
            {
                continue;
            }

            F e =
                Ф(
                    Р.G,
                    d.EntityId);

            if (ǟ
                .ǜ)
            {
                Ò Ī =
                    new Ò(
                        d,
                        this,
                        e,
                        false);

                Ѥ.Add(
                    d.EntityId,
                    Ī);

                Р
                    .ѩ
                    .Add(Ī);
            }
            else if (Ѫ(
                         Р,
                         ǟ.Ǘ))
            {
                ѥ.Add(
                    d.EntityId,
                    d);
            }

            yield return 1;
        }


        for (int Į = 0;
            Į <
                Р
                    .ѫ
                    .Count;
            Į++)
        {
            IMyThrust d =
                Р.ѫ[Į];

            ǘ ǟ;

            if (!Р
                    .Т
                    .TryGetValue(
                        d
                            .CubeGrid
                            .EntityId,
                        out ǟ))
            {
                continue;
            }

            F e =
                Ф(
                    Р.G,
                    d.EntityId);

            if (ǟ.ǜ)
            {
                Ѭ(
                    Р,
                    ǟ,
                    d,
                    e,
                    Ѥ,
                    Ѧ);
            }
            else if (Ѫ(
                         Р,
                         ǟ.Ǘ))
            {
                ѭ(
                    Р,
                    ǟ,
                    d,
                    e,
                    ѥ,
                    ѧ);
            }
            else if (ǟ
                         .Ǘ
                         .Ǥ)
            {
                g İ =
                    new g(
                        d,
                        this,
                        e,
                        false);

                Р
                    .р
                    .Add(İ);

                Р
                    .н
                    .Add(İ);
            }

            yield return 1;
        }

        Ѯ(
            Р,
            Ѧ);

        ѯ(
            Р,
            ѥ,
            ѧ);

        foreach (int ѱ in
            Ѱ(
                Р))
        {
            yield return ѱ;
        }
    }

    void Ѭ(
        П Р,
        ǘ ǟ,
        IMyThrust d,
        F e,
        Dictionary<long, Ò> ѳ,
        Dictionary<long, List<g>> Ѵ)
    {
        Ò Ѷ =
            ѵ(
                ǟ,
                ѳ);

        bool ѷ =
            ǟ.Ǜ >
            0;

        bool Ѹ =
            (e &
             F.Z) != 0;

        bool ѹ =
            (e &
             F.X) != 0;

        bool Ѻ =
            !ѹ &&
            (ȟ.Ȭ
                ? Ѹ ||
                  ѷ ||
                  Ѷ != null
                : Ѹ);

        g İ =
            new g(
                d,
                this,
                e,
                Ѻ);

        Р.ĥ.Add(
            İ);

        if (Ѻ)
        {
            Р
                .л
                .Add(İ);
        }
        else
        {
            Р
                .о
                .Add(İ);

            Р
                .н
                .Add(İ);
        }

        if (Ѷ == null)
        {
            if (Ѻ)
            {
                Р
                    .м
                    .Add(İ);
            }

            return;
        }

        List<g> ѻ;

        if (!Ѵ
                .TryGetValue(
                    Ѷ.Q,
                    out ѻ))
        {
            ѻ =
                new List<g>();

            Ѵ.Add(
                Ѷ.Q,
                ѻ);
        }

        ѻ.Add(
            İ);
    }

    void Ѯ(
        П Р,
        Dictionary<long, List<g>> Ѵ)
    {
        for (int Į = 0;
            Į <
                Р
                    .ѩ
                    .Count;
            Į++)
        {
            Ò Ī =
                Р
                    .ѩ[Į];

            List<g> ѻ;

            bool Ѽ =
                Ѵ
                    .TryGetValue(
                        Ī.Q,
                        out ѻ);

            bool ѽ =
                false;

            if (Ѽ)
            {
                for (int ĵ = 0;
                    ĵ < ѻ.Count;
                    ĵ++)
                {
                    if (ѻ[ĵ]
                        .P)
                    {
                        ѽ =
                            true;

                        break;
                    }
                }
            }

            bool f =
                !Ī.Y &&
                (Ī.a ||
                 ȟ.Ȭ &&
                 ѽ);

            Ī.N(
                C.O,
                f);

            if (!f)
            {
                if (Ѽ)
                {
                    for (int ĵ = 0;
                        ĵ < ѻ.Count;
                        ĵ++)
                    {
                        g İ =
                            ѻ[ĵ];

                        if (İ.P &&
                            !Р
                                .м
                                .Contains(
                                    İ))
                        {
                            Р
                                .м
                                .Add(
                                    İ);
                        }
                    }
                }

                continue;
            }

            Р
                .г
                .Add(Ī);

            if (!ѽ)
            {
                continue;
            }

            H ʋ =
                new H(
                    Ī,
                    this);

            Р
                .т
                .Add(ʋ);

            for (int ĵ = 0;
                ĵ < ѻ.Count;
                ĵ++)
            {
                g İ =
                    ѻ[ĵ];

                if (!İ.P)
                {
                    continue;
                }

                İ.I =
                    ʋ;

                ʋ.ĥ.Add(
                    İ);

                ǘ Ѿ;

                if (Р
                        .Т
                        .TryGetValue(
                            İ
                                .E
                                .CubeGrid
                                .EntityId,
                            out Ѿ))
                {
                    ѿ(
                        ʋ,
                        Ѿ,
                        Ī);
                }
            }

            ʋ
                .ĸ();
        }
    }

    void ѭ(
        П Р,
        ǘ ǟ,
        IMyThrust d,
        F e,
        Dictionary<long, IMyMotorStator> ѳ,
        Dictionary<long, List<g>> Ѵ)
    {
        ǖ Ҁ =
            ǟ.Ǘ;

        IMyMotorStator Ѷ =
            ҁ(
                ǟ,
                ѳ);

        bool Ѹ =
            (e &
             F.Z) != 0;

        bool ѹ =
            (e &
             F.X) != 0;

        bool Ҋ =
            !ѹ &&
            Ҁ.Ǩ &&
            (Ҁ.ǧ ||
             Ѹ);

        g İ =
            new g(
                d,
                this,
                e,
                false);

        if (!Ҋ)
        {
            Р
                .р
                .Add(İ);

            Р
                .н
                .Add(İ);

            return;
        }

        Р
            .п
            .Add(İ);

        if (Ѷ == null)
        {
            Р
                .с
                .Add(İ);

            return;
        }

        List<g> ѻ;

        if (!Ѵ
                .TryGetValue(
                    Ѷ.EntityId,
                    out ѻ))
        {
            ѻ =
                new List<g>();

            Ѵ.Add(
                Ѷ.EntityId,
                ѻ);
        }

        ѻ.Add(
            İ);
    }

    void ѯ(
        П Р,
        Dictionary<long, IMyMotorStator> ѳ,
        Dictionary<long, List<g>> Ѵ)
    {
        foreach (
            KeyValuePair<long, List<g>> Ћ
            in Ѵ)
        {
            IMyMotorStator ҋ;

            if (!ѳ.TryGetValue(
                    Ћ.Key,
                    out ҋ))
            {
                Ҍ(
                    Р,
                    Ћ.Value);

                continue;
            }

            ǘ ҍ;

            if (!Р
                    .Т
                    .TryGetValue(
                        ҋ
                            .CubeGrid
                            .EntityId,
                        out ҍ))
            {
                Ҍ(
                    Р,
                    Ћ.Value);

                continue;
            }

            F Ҏ =
                Ф(
                    Р.G,
                    ҋ.EntityId);

            bool ѹ =
                (Ҏ &
                 F.X) != 0;

            bool Ѹ =
                (Ҏ &
                 F.Z) != 0;

            ǖ Ҁ =
                ҍ.Ǘ;

            bool Ҋ =
                !ѹ &&
                Ҁ.Ǩ &&
                (Ѹ ||
                 Ҁ.ǧ &&
                 Ћ.Value.Count > 0);

            if (!Ҋ ||
                ҋ.TopGrid == null ||
                !ҋ.IsFunctional ||
                !ҋ.Enabled ||
                ҋ.RotorLock)
            {
                Ҍ(
                    Р,
                    Ћ.Value);

                continue;
            }

            ҏ Ґ =
                new ҏ(
                    ҋ);

            Ґ.ĥ.AddRange(
                Ћ.Value);

            Р
                .ф
                .Add(Ґ);
        }
    }

    static void Ҍ(
        П Р,
        List<g> ʆ)
    {
        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            if (!Р
                    .с
                    .Contains(
                        İ))
            {
                Р
                    .с
                    .Add(İ);
            }
        }
    }

    IEnumerable<int> Ѱ(
        П Р)
    {
        for (int Į = 0;
            Į <
                Р.ґ.Count;
            Į++)
        {
            IMyGyro d =
                Р.ґ[Į];

            ǘ ǟ;

            if (!Р
                    .Т
                    .TryGetValue(
                        d
                            .CubeGrid
                            .EntityId,
                        out ǟ) ||
                !ǟ.ǜ ||
                !Ғ(
                    d))
            {
                continue;
            }

            F e =
                Ф(
                    Р.G,
                    d.EntityId);

            bool ѹ =
                (e &
                 F.X) != 0;

            if (ѹ)
            {
                continue;
            }

            bool ѷ =
                ǟ.Ǜ >
                0;

            bool Ѹ =
                (e &
                 F.Z) != 0;

            bool ғ =
                ȟ.Ȭ
                    ? Ѹ ||
                      ѷ
                    : Ѹ;

            bool Ҕ =
                d.CubeGrid ==
                Me.CubeGrid;

            if (!ғ &&
                !Ҕ)
            {
                continue;
            }

            ũ ҕ =
                new ũ(
                    d,
                    this,
                    e,
                    ғ);

            Р.д.Add(
                ҕ);

            yield return 1;
        }
    }

    Ò ѵ(
        ǘ ǟ,
        Dictionary<long, Ò> ѳ)
    {
        ǘ Җ =
            ǟ;

        while (Җ != null &&
               Җ.Ǚ != null)
        {
            ǔ җ =
                Җ.ǚ;

            IMyMotorStator Ҙ =
                җ != null
                    ? җ.ǝ as
                        IMyMotorStator
                    : null;

            if (Ҙ != null &&
                Ҙ.TopGrid ==
                    Җ.Ǔ)
            {
                Ò Ī;

                if (ѳ.TryGetValue(
                        Ҙ.EntityId,
                        out Ī))
                {
                    return Ī;
                }
            }

            Җ =
                Җ.Ǚ;
        }

        return null;
    }

    IMyMotorStator ҁ(
        ǘ ǟ,
        Dictionary<long, IMyMotorStator> ѳ)
    {
        ǘ Җ =
            ǟ;

        while (Җ != null &&
               Җ.Ǚ != null)
        {
            ǔ җ =
                Җ.ǚ;

            IMyMotorStator Ҙ =
                җ != null
                    ? җ.ǝ as
                        IMyMotorStator
                    : null;

            if (Ҙ != null &&
                Ҙ.TopGrid ==
                    Җ.Ǔ)
            {
                IMyMotorStator Ī;

                if (ѳ.TryGetValue(
                        Ҙ.EntityId,
                        out Ī))
                {
                    return Ī;
                }
            }

            Җ =
                Җ.Ǚ;
        }

        return null;
    }

    void ѿ(
        H ʋ,
        ǘ Ѿ,
        Ò Ī)
    {
        ǘ Җ =
            Ѿ;

        while (Җ != null)
        {
            ѡ(
                ʋ.Ħ,
                Җ.Ǔ);

            if (Җ.ǚ !=
                    null &&
                Җ.ǚ
                    .ǝ
                    .EntityId ==
                Ī.Q)
            {
                break;
            }

            Җ =
                Җ.Ǚ;
        }
    }

    static bool Ѫ(
        П Р,
        ǖ Ҁ)
    {
        return Ҁ != null &&
               Ҁ !=
                   Р.я &&
               !Ҁ
                   .ǜ &&
               Ҁ
                   .Ǥ &&
               Ҁ
                   .ǩ !=
                   null;
    }

    bool Ғ(
        IMyGyro ҕ)
    {
        string ҙ =
            ҕ
                .BlockDefinition
                .SubtypeId;

        if (ҙ.Equals(
"SmallBlockGyro",
                StringComparison
                    .OrdinalIgnoreCase) ||
            ҙ.Equals(
"LargeBlockGyro",
                StringComparison
                    .OrdinalIgnoreCase))
        {
            return true;
        }

        return ҙ.Equals(
"SmallPrototechGyro",
                   StringComparison
                       .OrdinalIgnoreCase) ||
               ҙ.Equals(
"LargePrototechGyro",
                   StringComparison
                       .OrdinalIgnoreCase) ||
               ҙ.Equals(
"SmallPrototechGyroscope",
                   StringComparison
                       .OrdinalIgnoreCase) ||
               ҙ.Equals(
"LargePrototechGyroscope",
                   StringComparison
                       .OrdinalIgnoreCase);
    }
    IEnumerable<int> ҥ()
    {
        П Р =
            new П();

        foreach (int ѱ in
            Қ(
                Р))
        {
            yield return ѱ;
        }

        foreach (int ѱ in
            қ(
                Р))
        {
            yield return ѱ;
        }

        Ҝ(
            Р);

        foreach (int ѱ in
            ҝ(
                Р))
        {
            yield return ѱ;
        }

        foreach (int ѱ in
            Ҟ(
                Р))
        {
            yield return ѱ;
        }

        ǘ Ҡ =
            ҟ(
                Р.Т,
                Me.CubeGrid);

        Р.я =
            Ҡ.Ǘ;

        ҡ(
            Р);

        Ң(
            Р,
            Ҡ);

        ң(
            Р);

        foreach (int ѱ in
            Ҥ(
                Р))
        {
            yield return ѱ;
        }

        foreach (int ѱ in
            Ѳ(
                Р))
        {
            yield return ѱ;
        }

        foreach (int ѱ in
            Я(
                Р))
        {
            yield return ѱ;
        }

        ы(
            Р);

        yield return 1;
    }


    IEnumerable<int> Қ(
        П Р)
    {
        GridTerminalSystem.GetBlocks(
            Р.ю);

        List<IMyBlockGroup> Ҧ =
            new List<IMyBlockGroup>();

        GridTerminalSystem.GetBlockGroups(
            Ҧ);

        List<IMyTerminalBlock> ҧ =
            new List<IMyTerminalBlock>();

        for (int Į = 0;
            Į < Ҧ.Count;
            Į++)
        {
            IMyBlockGroup Ҩ =
                Ҧ[Į];

            F ҩ =
                њ(
                    Ҩ.Name);

            if (ҩ ==
                F.M)
            {
                continue;
            }

            ҧ.Clear();

            Ҩ.GetBlocks(
                ҧ);

            for (int ĵ = 0;
                ĵ < ҧ.Count;
                ĵ++)
            {
                ў(
                    Р.G,
                    ҧ[ĵ]
                        .EntityId,
                    ҩ);
            }

            yield return 1;
        }

        for (int Į = 0;
            Į < Р.ю.Count;
            Į++)
        {
            IMyTerminalBlock d =
                Р.ю[Į];

            F Ҫ =
                њ(
                    d.CustomName) |
                њ(
                    d.CustomData);

            ў(
                Р.G,
                d.EntityId,
                Ҫ);

            ҟ(
                Р.Т,
                d.CubeGrid);

            IMyShipController Ͱ =
                d as
                    IMyShipController;

            if (Ͱ != null)
            {
                Р.Ǣ.Add(
                    Ͱ);
            }

            IMyThrust T =
                d as
                    IMyThrust;

            if (T != null)
            {
                Р.ѫ.Add(
                    T);
            }

            IMyMotorStator Ī =
                d as
                    IMyMotorStator;

            if (Ī != null)
            {
                Р.Ѩ.Add(
                    Ī);
            }

            IMyPistonBase ҫ =
                d as
                    IMyPistonBase;

            if (ҫ != null)
            {
                Р.Ҭ.Add(
                    ҫ);
            }

            IMyGyro ҕ =
                d as
                    IMyGyro;

            if (ҕ != null)
            {
                Р.ґ.Add(
                    ҕ);
            }

            IMyShipConnector Ϛ =
                d as
                    IMyShipConnector;

            if (Ϛ != null)
            {
                Р.С.Add(
                    Ϛ);
            }

            IMyLandingGear Ϟ =
                d as
                    IMyLandingGear;

            if (Ϟ != null)
            {
                Р
                    .Щ
                    .Add(
                        Ϟ);
            }

            IMyTimerBlock ϩ =
                d as
                    IMyTimerBlock;

            if (ϩ != null)
            {
                Р.Ы.Add(
                    ϩ);
            }

            IMyProgrammableBlock і =
                d as
                    IMyProgrammableBlock;

            if (і != null)
            {
                Р
                    .ҭ
                    .Add(
                        і);
            }

            yield return 1;
        }

        ҟ(
            Р.Т,
            Me.CubeGrid);
    }


    IEnumerable<int> қ(
        П Р)
    {
        for (int Į = 0;
            Į <
                Р.Ѩ.Count;
            Į++)
        {
            IMyMotorStator Ī =
                Р.Ѩ[Į];

            if (Ī.TopGrid ==
                null)
            {
                continue;
            }

            Ү(
                Р.Т,
                Ī.CubeGrid,
                Ī.TopGrid,
                Ī);

            yield return 1;
        }

        for (int Į = 0;
            Į <
                Р.Ҭ.Count;
            Į++)
        {
            IMyPistonBase ҫ =
                Р.Ҭ[Į];

            if (ҫ.TopGrid ==
                null)
            {
                continue;
            }

            Ү(
                Р.Т,
                ҫ.CubeGrid,
                ҫ.TopGrid,
                ҫ);

            yield return 1;
        }

        for (int Į = 0;
            Į <
                Р.С.Count;
            Į++)
        {
            IMyShipConnector Ɨ =
                Р
                    .С[Į]
                    .OtherConnector;

            if (Ɨ != null)
            {
                ҟ(
                    Р.Т,
                    Ɨ.CubeGrid);
            }

            yield return 1;
        }
    }

    void Ҝ(
        П Р)
    {
        List<ǘ> ү =
            new List<ǘ>();

        foreach (
            KeyValuePair<long, ǘ> Ћ
            in Р.Т)
        {
            ǘ Ұ =
                Ћ.Value;

            if (Ұ.Ǘ !=
                null)
            {
                continue;
            }

            ǖ Ҁ =
                new ǖ();

            Р.ұ.Add(
                Ҁ);

            ү.Clear();
            ү.Add(Ұ);

            Ұ.Ǘ =
                Ҁ;

            for (int ΰ = 0;
                ΰ < ү.Count;
                ΰ++)
            {
                ǘ ǟ =
                    ү[ΰ];

                Ҁ.ǡ.Add(
                    ǟ);

                if (ǟ.Ǔ.IsStatic)
                {
                    Ҁ.ǥ =
                        true;
                }

                for (int Ҳ = 0;
                    Ҳ <
                        ǟ
                            .Ǖ
                            .Count;
                    Ҳ++)
                {
                    ǘ ҳ =
                        ǟ
                            .Ǖ[
                                Ҳ]
                            .Ǡ(ǟ);

                    if (ҳ.Ǘ !=
                        null)
                    {
                        continue;
                    }

                    ҳ.Ǘ =
                        Ҁ;

                    ү.Add(
                        ҳ);
                }
            }
        }
    }


    IEnumerable<int> ҝ(
        П Р)
    {
        for (int Į = 0;
            Į <
                Р.Ǣ.Count;
            Į++)
        {
            IMyShipController Ͱ =
                Р.Ǣ[Į];

            ǘ ǟ;

            if (!Р
                    .Т
                    .TryGetValue(
                        Ͱ
                            .CubeGrid
                            .EntityId,
                        out ǟ))
            {
                continue;
            }

            ǟ.Ǘ
                .Ǣ
                .Add(Ͱ);

            if (Ͱ.CubeGrid ==
                Me.CubeGrid)
            {
                Р
                    .й
                    .Add(Ͱ);
            }

            yield return 1;
        }

        for (int Į = 0;
            Į <
                Р
                    .ҭ
                    .Count;
            Į++)
        {
            IMyProgrammableBlock і =
                Р
                    .ҭ[Į];

            if (!ї(
                    і))
            {
                continue;
            }

            ǘ ǟ;

            if (!Р
                    .Т
                    .TryGetValue(
                        і
                            .CubeGrid
                            .EntityId,
                        out ǟ))
            {
                continue;
            }

            ǖ Ҁ =
                ǟ.Ǘ;

            Ҁ
                .ǣ
                .Add(
                    і);

            bool Ҵ =
                ј(
                    і);

            if (Ҵ)
            {
                Ҁ
                    .Ǧ =
                    true;
            }

            if (Ҁ
                    .ǩ ==
                    null ||
                і
                    .EntityId <
                Ҁ
                    .ǩ
                    .EntityId)
            {
                Ҁ
                    .ǩ =
                    і;

                Ҁ.ǧ =
                    ҵ(
                        і);

                Ҁ.Ǩ =
                    Ҵ;
            }

            yield return 1;
        }
    }

    bool ҵ(
        IMyProgrammableBlock і)
    {
        string Ȧ;

        if (!Ͳ(
                і.CustomData,
                Ɉ,
"Greedy",
                out Ȧ))
        {
            return true;
        }

        bool ğ;

        return bool.TryParse(
                   Ȧ,
                   out ğ)
            ? ğ
            : true;
    }


    IEnumerable<int> Ҟ(
        П Р)
    {
        HashSet<Ƥ> Ҷ =
            new HashSet<Ƥ>();

        for (int Į = 0;
            Į <
                Р.С.Count;
            Į++)
        {
            IMyShipConnector Ϛ =
                Р.С[Į];

            IMyShipConnector Ɨ =
                Ϛ.OtherConnector;

            if (Ɨ == null)
            {
                continue;
            }

            Ƥ Ћ =
                new Ƥ(
                    Ϛ.EntityId,
                    Ɨ.EntityId);

            if (!Ҷ.Add(
                    Ћ))
            {
                continue;
            }

            ǘ ҷ;
            ǘ Ҹ;

            if (!Р
                    .Т
                    .TryGetValue(
                        Ϛ
                            .CubeGrid
                            .EntityId,
                        out ҷ) ||
                !Р
                    .Т
                    .TryGetValue(
                        Ɨ
                            .CubeGrid
                            .EntityId,
                        out Ҹ))
            {
                continue;
            }

            Р
                .Ш
                .Add(
                    new Ǭ
                    {
                        ƥ =
                            Ϛ,

                        Ʀ =
                            Ɨ,

                        Ǫ =
                            ҷ,

                        ǫ =
                            Ҹ
                    });

            yield return 1;
        }
    }

    void ҡ(
        П Р)
    {
        ǖ ҹ =
            Р.я;

        if (ҹ == null)
        {
            return;
        }

        List<ǖ> ү =
            new List<ǖ>();

        ҹ.Ǥ =
            true;

        ү.Add(ҹ);

        for (int ΰ = 0;
            ΰ < ү.Count;
            ΰ++)
        {
            ǖ Ҁ =
                ү[ΰ];

            for (int Į = 0;
                Į <
                    Р
                        .Ш
                        .Count;
                Į++)
            {
                Ǭ җ =
                    Р
                        .Ш[Į];

                ǖ Ɨ =
                    null;

                if (җ.Ǫ.Ǘ ==
                    Ҁ)
                {
                    Ɨ =
                        җ.ǫ.Ǘ;
                }
                else if (җ.ǫ.Ǘ ==
                         Ҁ)
                {
                    Ɨ =
                        җ.Ǫ.Ǘ;
                }

                if (Ɨ == null ||
                    Ɨ
                        .Ǥ)
                {
                    continue;
                }

                Ɨ
                    .Ǥ =
                    true;

                ү.Add(Ɨ);
            }
        }
    }

    void Ң(
        П Р,
        ǘ Ҡ)
    {
        ǖ ҹ =
            Р.я;

        if (ҹ == null)
        {
            return;
        }

        ҹ.ǜ =
            true;

        Һ(
            ҹ,
            Ҡ,
            0);

        bool һ;

        do
        {
            һ =
                false;

            for (int Į = 0;
                Į <
                    Р
                        .Ш
                        .Count;
                Į++)
            {
                Ǭ җ =
                    Р
                        .Ш[Į];

                bool Ҽ =
                    җ
                        .Ǫ
                        .Ǘ
                        .ǜ;

                bool ҽ =
                    җ
                        .ǫ
                        .Ǘ
                        .ǜ;

                if (Ҽ ==
                    ҽ)
                {
                    continue;
                }

                ǘ ѣ =
                    Ҽ
                        ? җ.Ǫ
                        : җ.ǫ;

                ǘ Ȝ =
                    Ҽ
                        ? җ.ǫ
                        : җ.Ǫ;

                ǖ Ҿ =
                    Ȝ.Ǘ;

                if (Ҿ
                        .ǣ
                        .Count > 0)
                {
                    continue;
                }

                if (!ȟ.ȭ)
                {
                    continue;
                }

                Ҿ
                    .ǜ =
                    true;

                int ҿ =
                    ѣ.Ǜ ==
                        int.MaxValue
                        ? 0
                        : ѣ.Ǜ;

                Һ(
                    Ҿ,
                    Ȝ,
                    ҿ);

                һ =
                    true;
            }
        }
        while (һ);

        foreach (
            KeyValuePair<long, ǘ> Ћ
            in Р.Т)
        {
            Ћ.Value
                .ǜ =
                Ћ.Value
                    .Ǘ
                    .ǜ;
        }
    }

    void ң(
        П Р)
    {
        for (int Į = 0;
            Į <
                Р.ұ.Count;
            Į++)
        {
            ǖ Ҁ =
                Р.ұ[Į];

            if (Ҁ ==
                    Р.я ||
                Ҁ.ǜ ||
                !Ҁ
                    .Ǥ ||
                Ҁ
                    .ǩ ==
                    null)
            {
                continue;
            }

            ǘ Ҡ;

            if (!Р
                    .Т
                    .TryGetValue(
                        Ҁ
                            .ǩ
                            .CubeGrid
                            .EntityId,
                        out Ҡ))
            {
                continue;
            }

            Һ(
                Ҁ,
                Ҡ,
                0);
        }
    }

    void Һ(
        ǖ Ҁ,
        ǘ ι,
        int Ӏ)
    {
        List<ǘ> ү =
            new List<ǘ>();

        if (ι.Ǜ >
            Ӏ)
        {
            ι.Ǜ =
                Ӏ;

            ι.Ǚ =
                null;

            ι.ǚ =
                null;
        }

        ү.Add(ι);

        for (int ΰ = 0;
            ΰ < ү.Count;
            ΰ++)
        {
            ǘ ǟ =
                ү[ΰ];

            for (int Į = 0;
                Į <
                    ǟ
                        .Ǖ
                        .Count;
                Į++)
            {
                ǔ җ =
                    ǟ
                        .Ǖ[Į];

                ǘ ҳ =
                    җ.Ǡ(ǟ);

                if (ҳ.Ǘ !=
                    Ҁ)
                {
                    continue;
                }

                int Ӂ =
                    ǟ.Ǜ + 1;

                if (Ӂ >=
                    ҳ.Ǜ)
                {
                    continue;
                }

                ҳ.Ǜ =
                    Ӂ;

                ҳ.Ǚ =
                    ǟ;

                ҳ.ǚ =
                    җ;

                ү.Add(
                    ҳ);
            }
        }
    }

    IEnumerable<int> Ҥ(
        П Р)
    {
        for (int Į = 0;
            Į <
                Р.ұ.Count;
            Į++)
        {
            ǖ Ҁ =
                Р.ұ[Į];

            if (!Ҁ
                    .Ǥ ||
                Ҁ
                    .ǜ ||
                Ҁ
                    .ǣ
                    .Count == 0)
            {
                continue;
            }

            for (int ĵ = 0;
                ĵ <
                    Ҁ
                        .Ǣ
                        .Count;
                ĵ++)
            {
                Р
                    .к
                    .Add(
                        Ҁ
                            .Ǣ[ĵ]);
            }

            yield return 1;
        }
    }


    static ǘ ҟ(
        Dictionary<long, ǘ> ӂ,
        IMyCubeGrid Ŗ)
    {
        ǘ ǟ;

        if (!ӂ.TryGetValue(
                Ŗ.EntityId,
                out ǟ))
        {
            ǟ =
                new ǘ(Ŗ);

            ӂ.Add(
                Ŗ.EntityId,
                ǟ);
        }

        return ǟ;
    }

    static void Ү(
        Dictionary<long, ǘ> ӂ,
        IMyCubeGrid Ӄ,
        IMyCubeGrid ӄ,
        IMyTerminalBlock Ǟ)
    {
        ǘ ǂ =
            ҟ(
                ӂ,
                Ӄ);

        ǘ ǃ =
            ҟ(
                ӂ,
                ӄ);

        ǔ җ =
            new ǔ(
                ǂ,
                ǃ,
                Ǟ);

        ǂ.Ǖ.Add(
            җ);

        ǃ.Ǖ.Add(
            җ);
    }
    sealed class П
    {
        public readonly List<IMyTerminalBlock> ю =
            new List<IMyTerminalBlock>();

        public readonly List<IMyShipController> Ǣ =
            new List<IMyShipController>(),й =
            new List<IMyShipController>(),к =
            new List<IMyShipController>();

        public readonly List<IMyThrust> ѫ =
            new List<IMyThrust>();

        public readonly List<IMyMotorStator> Ѩ =
            new List<IMyMotorStator>();

        public readonly List<IMyPistonBase> Ҭ =
            new List<IMyPistonBase>();

        public readonly List<IMyGyro> ґ =
            new List<IMyGyro>();

        public readonly List<IMyShipConnector> С =
            new List<IMyShipConnector>(),У =
            new List<IMyShipConnector>();

        public readonly List<IMyLandingGear> Щ =
            new List<IMyLandingGear>();

        public readonly List<IMyTimerBlock> Ы =
            new List<IMyTimerBlock>(),Ь =
            new List<IMyTimerBlock>(),Э =
            new List<IMyTimerBlock>();

        public readonly List<IMyProgrammableBlock> ҭ =
            new List<IMyProgrammableBlock>();

        public readonly Dictionary<long, F> G =
            new Dictionary<long, F>();

        public readonly Dictionary<long, ǘ> Т =
            new Dictionary<long, ǘ>();

        public readonly List<ǖ> ұ =
            new List<ǖ>();

        public readonly List<Ǭ> Ш =
            new List<Ǭ>();

        public readonly List<g> ĥ =
            new List<g>(),л =
            new List<g>(),м =
            new List<g>(),н =
            new List<g>(),о =
            new List<g>(),п =
            new List<g>(),р =
            new List<g>(),с =
            new List<g>();

        public readonly List<Ò> ѩ =
            new List<Ò>(),г =
            new List<Ò>();

        public readonly List<H> т =
            new List<H>();

        public readonly List<ҏ> ф =
            new List<ҏ>();

        public readonly List<ŝ> у =
            new List<ŝ>();

        public readonly List<ũ> д =
            new List<ũ>();

        public readonly List<ǯ> Ц =
            new List<ǯ>();

        public readonly List<ǰ> Ъ =
            new List<ǰ>();

        public readonly List<Ž> ч =
            new List<Ž>();

        public ǖ я;
    }

    sealed class ҏ
    {
        readonly List<double> Ĥ =
            new List<double>();

        public readonly IMyMotorStator Ò;

        public readonly List<g> ĥ =
            new List<g>();

        public ҏ(
            IMyMotorStator Ī)
        {
            Ò = Ī;
        }

        public Vector3D É
        {
            get
            {
                return Ò
                    .WorldMatrix
                    .Up;
            }
        }

        public double ŉ(
            Vector3D m)
        {
            m =
                n.o(
                    m);

            if (Ò == null ||
                Ò.Closed ||
                Ò.TopGrid == null ||
                !Ò.IsFunctional ||
                m
                    .LengthSquared() <=
                p)
            {
                return 0;
            }

            Ĥ.Clear();

            Ӆ(0);

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                g İ =
                    ĥ[Į];

                if (!ӆ(
                        İ))
                {
                    continue;
                }

                double ñ =
                    n.Ē(
                        m,
                        İ
                            .R,
                        É);

                Ӆ(
                    ã(
                        ñ));
            }

            double Õ =
                Ò.Angle;

            if (Î(
                    Ò.LowerLimitRad))
            {
                Ӆ(
                    Ò.LowerLimitRad -
                    Õ);
            }

            if (Ï(
                    Ò.UpperLimitRad))
            {
                Ӆ(
                    Ò.UpperLimitRad -
                    Õ);
            }

            double Ŀ = 0;
            double Ӈ =
                double.MaxValue;

            for (int Į = 0;
                Į <
                    Ĥ.Count;
                Į++)
            {
                double Ł =
                    ã(
                        Ĥ[Į]);

                double ł =
                    Ľ(
                        m,
                        Ł);

                double ӈ =
                    Math.Abs(Ł);

                if (ł >
                        Ŀ +
                        U ||
                    Math.Abs(
                        ł -
                        Ŀ) <=
                        U &&
                    ӈ <
                        Ӈ)
                {
                    Ŀ =
                        ł;

                    Ӈ =
                        ӈ;
                }
            }

            return Ŀ;
        }

        void Ӆ(
            double ñ)
        {
            ñ =
                ã(
                    ñ);

            for (int Į = 0;
                Į <
                    Ĥ.Count;
                Į++)
            {
                if (Math.Abs(
                        Ĥ[Į] -
                        ñ) <=
                    ē)
                {
                    return;
                }
            }

            Ĥ.Add(
                ñ);
        }

        double Ľ(
            Vector3D m,
            double ñ)
        {
            double ł = 0;

            for (int Į = 0;
                Į < ĥ.Count;
                Į++)
            {
                g İ =
                    ĥ[Į];

                if (!ӆ(
                        İ))
                {
                    continue;
                }

                Vector3D ŗ =
                    n.ë(
                        İ
                            .R,
                        É,
                        -ñ);

                double q =
                    Vector3D.Dot(
                        ŗ,
                        m);

                if (q <= 0)
                {
                    continue;
                }

                ł +=
                    q *
                    İ
                        .V;
            }

            return ł;
        }

        static bool ӆ(
            g İ)
        {
            if (İ == null ||
                İ.Y ||
                İ.E == null ||
                İ.E.Closed ||
                !İ
                    .E
                    .IsFunctional)
            {
                return false;
            }

            return İ
                       .V >
                   U;
        }

        double ã(
            double ø)
        {
            ø =
                n.Ø(
                    ø);

            bool Ĕ =
                Î(
                    Ò.LowerLimitRad);

            bool ĕ =
                Ï(
                    Ò.UpperLimitRad);

            if (!Ĕ &&
                !ĕ)
            {
                return ø;
            }

            double Õ =
                Ò.Angle;

            double Ė =
                double.NaN;

            double ė =
                double.MaxValue;

            for (int Ę = -2;
                Ę <= 2;
                Ę++)
            {
                double ę =
                    ø +
                    Ę *
                    MathHelper.TwoPi;

                double Ě =
                    Õ +
                    ę;

                if (Ĕ &&
                    Ě <
                        Ò.LowerLimitRad -
                        ē)
                {
                    continue;
                }

                if (ĕ &&
                    Ě >
                        Ò.UpperLimitRad +
                        ē)
                {
                    continue;
                }

                double ě =
                    Math.Abs(
                        ę);

                if (ě <
                    ė)
                {
                    ė =
                        ě;

                    Ė =
                        ę;
                }
            }

            if (!double.IsNaN(
                    Ė))
            {
                return Ė;
            }

            double Ĝ =
                Õ +
                ø;

            if (Ĕ)
            {
                Ĝ =
                    Math.Max(
                        Ĝ,
                        Ò.LowerLimitRad);
            }

            if (ĕ)
            {
                Ĝ =
                    Math.Min(
                        Ĝ,
                        Ò.UpperLimitRad);
            }

            return Ĝ -
                   Õ;
        }

        static bool Î(
            double ğ)
        {
            return !double.IsNaN(
                       ğ) &&
                   !double.IsInfinity(
                       ğ) &&
                   ğ > -1e20;
        }

        static bool Ï(
            double ğ)
        {
            return !double.IsNaN(
                       ğ) &&
                   !double.IsInfinity(
                       ğ) &&
                   ğ < 1e20;
        }
    }

    readonly List<Ǭ> х =
        new List<Ǭ>();

    readonly List<IMyShipConnector> ц =
        new List<IMyShipConnector>();

    readonly Dictionary<long, long> Ӊ =
        new Dictionary<long, long>();

    readonly Dictionary<long, bool> ӊ =
        new Dictionary<long, bool>();

    readonly List<ҏ> ʏ =
        new List<ҏ>();

    readonly List<g> ʎ =
        new List<g>();

    void Ȋ()
    {
        ϵ =
            true;
    }

    void Џ()
    {
        if (Ј == null)
        {
            if (!ϵ)
            {
                return;
            }

            ϵ =
                false;

            Ј =
                ҥ()
                    .GetEnumerator();
        }

        int Ӌ =
            Runtime.MaxInstructionCount;

        int ӌ =
            Math.Max(
                1000,
                Ӌ *
                3 /
                4);

        int Ӎ = 0;

        while (Ј !=
                   null &&
               Runtime
                   .CurrentInstructionCount <
                   ӌ &&
               Ӎ < 512)
        {
            Ӎ++;

            if (Ј
                .MoveNext())
            {
                continue;
            }

            Ј.Dispose();

            Ј =
                null;

            if (ϵ)
            {
                ϵ =
                    false;

                Ј =
                    ҥ()
                        .GetEnumerator();
            }
        }
    }

    static double ʍ(
        List<g> ӎ,
        Vector3D m)
    {
        m =
            n.o(
                m);

        if (m
                .LengthSquared() <=
            p)
        {
            return 0;
        }

        double ł = 0;

        for (int Į = 0;
            Į <
                ӎ.Count;
            Į++)
        {
            g İ =
                ӎ[Į];

            if (İ == null ||
                İ.Y ||
                İ.E == null ||
                İ.E.Closed ||
                !İ
                    .E
                    .IsFunctional)
            {
                continue;
            }

            double q =
                Vector3D.Dot(
                    İ
                        .R,
                    m);

            if (q <= 0)
            {
                continue;
            }

            ł +=
                q *
                İ
                    .V;
        }

        return ł;
    }

    bool ʟ
    {
        get
        {
            return Ȏ ==
                    ƅ.ƃ
                ? ɱ
                    .Ư
                : Ȍ;
        }
    }

    bool ǻ
    {
        get
        {
            return Ȏ ==
                    ƅ.ƃ
                ? ɱ
                    .Ɖ
                : Ȕ;
        }
    }

    double ʭ
    {
        get
        {
            return Ȏ ==
                    ƅ.ƃ
                ? ɱ
                    .Ʊ
                : ȝ;
        }
    }

    double ʝ
    {
        get
        {
            if (Ȏ ==
                ƅ.ƃ)
            {
                return MathHelper.Clamp(
                    ɱ
                        .Ʋ,
                    0,
                    1);
            }

            if (ȟ
                    .Ƞ.Count == 0)
            {
                return 0;
            }

            return MathHelper.Clamp(
                ȟ.Ƞ[
                    MathHelper.Clamp(
                        Ȣ,
                        0,
                        ȟ
                            .Ƞ
                            .Count -
                        1)],
                0,
                1);
        }
    }


    void ǽ(
        string Ǹ,
        bool ǹ)
    {
        ϲ =
            Ǹ ?? string.Empty;

        ϴ =
            ǹ;

        ϓ =
            true;
    }

    void ӏ()
    {
        ϲ =
            string.Empty;

        ϴ =
            false;
    }


    void ȍ(
        bool i)
    {
        Ȍ =
            i;

        Ӑ(
            i);
    }

    void Ӑ(
        bool i)
    {
        for (int Į = 0;
            Į < ͺ.Count;
            Į++)
        {
            IMyShipController Ͱ =
                ͺ[Į];

            if (Ͱ == null ||
                Ͱ.Closed ||
                !Ͱ.IsFunctional)
            {
                continue;
            }

            if (Ͱ
                    .DampenersOverride ==
                i)
            {
                continue;
            }

            Ͱ
                .DampenersOverride =
                i;
        }
    }

    void ɞ()
    {
        if (Ȏ ==
            ƅ.ƃ)
        {
            Ӑ(
                ɱ
                    .Ư);

            return;
        }

        if (!Ͽ)
        {
            Ӑ(
                Ȍ);

            return;
        }

        if (ɡ ==
                null ||
            ɡ.Closed)
        {
            return;
        }

        bool ӑ =
            ɡ
                .DampenersOverride;

        if (ӑ ==
            Ȍ)
        {
            return;
        }

        Ȍ =
            ӑ;

        Ӑ(
            Ȍ);
    }

    void ȕ(
        bool i)
    {
        if (i &&
            !Ȕ)
        {
            Ӓ();
        }

        Ȕ = i;

        if (!Ȕ)
        {
            ʾ(
                ƌ.Ɖ);

            ӓ(
                C.Ɖ);
        }

        ɝ();
        Ӕ();
    }

    void ȏ()
    {
        ȕ(
            !Ȕ);
    }

    void Ӓ()
    {
        if (ɡ ==
                null ||
            ɡ.Closed)
        {
            ȝ = 0;
            Ȟ =
                false;

            return;
        }

        Vector3D ʓ =
            ɡ
                .GetShipVelocities()
                .LinearVelocity;

        ȝ =
            Vector3D.Dot(
                ʓ,
                ɡ
                    .WorldMatrix
                    .Forward);

        Ȟ =
            true;
    }

    void ʤ()
    {
        if (Ȟ)
        {
            return;
        }

        Ӓ();
    }

    void Ș(
        double Ö)
    {
        ʤ();

        ȝ +=
            Ö;

        ǽ(
"Cruise target: "+
            ȝ
                .ToString("0.###") +
" m/s",
            false);
    }


    bool b(
        long Ϧ)
    {
        Ə Ȓ;

        if (!Ϻ
                .TryGetValue(
                    Ϧ,
                    out Ȓ))
        {
            return false;
        }

        return Ȓ.ƍ &&
               Ȓ.Ǝ !=
                   ƌ.M;
    }

    bool ӕ(
        long Ϧ,
        out Ə Ȓ)
    {
        return Ϻ
            .TryGetValue(
                Ϧ,
                out Ȓ);
    }

    void ˀ(
        g İ,
        ƌ Ӗ)
    {
        if (İ == null)
        {
            return;
        }

        ˀ(
            İ.E,
            Ӗ);
    }

    void ˀ(
        IMyThrust d,
        ƌ Ӗ)
    {
        if (d == null ||
            d.Closed ||
            Ӗ ==
                ƌ.M)
        {
            return;
        }

        Ə Ȓ;

        if (!Ϻ
                .TryGetValue(
                    d.EntityId,
                    out Ȓ))
        {
            Ȓ =
                new Ə
                {
                    ƍ =
                        d.Enabled,
                    Ǝ =
                        ƌ.M
                };

            Ϻ.Add(
                d.EntityId,
                Ȓ);
        }

        Ȓ.Ǝ |=
            Ӗ;

        if (d.Enabled)
        {
            d.Enabled =
                false;
        }

        if ((Ȓ.Ǝ &
             ƌ.Ɗ) != 0)
        {
            Ϣ[
                d.EntityId] =
                Ȓ.ƍ;
        }
    }

    void y(
        g İ)
    {
        if (İ == null)
        {
            return;
        }

        y(
            İ.E);
    }

    void y(
        IMyThrust d)
    {
        if (d == null ||
            d.Closed)
        {
            return;
        }

        Ə Ȓ;

        if (!Ϻ
                .TryGetValue(
                    d.EntityId,
                    out Ȓ))
        {
            return;
        }

        if (Ȓ.ƍ &&
            !d.Enabled)
        {
            d.Enabled =
                true;
        }
    }

    void ʾ(
        g İ,
        ƌ Ӗ)
    {
        if (İ == null)
        {
            return;
        }

        ʾ(
            İ.Q,
            İ.E,
            Ӗ);
    }

    void ʾ(
        long Ϧ,
        IMyThrust d,
        ƌ Ӗ)
    {
        Ə Ȓ;

        if (!Ϻ
                .TryGetValue(
                    Ϧ,
                    out Ȓ))
        {
            return;
        }

        Ȓ.Ǝ &=
            ~Ӗ;

        if ((Ӗ &
             ƌ.Ɗ) != 0)
        {
            Ϣ.Remove(
                Ϧ);
        }

        if (Ȓ.Ǝ !=
            ƌ.M)
        {
            if (d != null &&
                !d.Closed &&
                d.Enabled)
            {
                d.Enabled =
                    false;
            }

            return;
        }

        if (d != null &&
            !d.Closed)
        {
            d.Enabled =
                Ȓ.ƍ;
        }

        Ϻ.Remove(
            Ϧ);
    }

    void ʾ(
        ƌ Ӗ)
    {
        if (Ϻ.Count ==
            0)
        {
            return;
        }

        List<long> ӗ =
            new List<long>(
                Ϻ.Keys);

        for (int Į = 0;
            Į < ӗ.Count;
            Į++)
        {
            long Ϧ =
                ӗ[Į];

            IMyThrust d =
                Ә(
                    Ϧ);

            ʾ(
                Ϧ,
                d,
                Ӗ);
        }
    }

    IMyThrust Ә(
        long Ϧ)
    {
        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            if (İ.Q ==
                Ϧ)
            {
                return İ.E;
            }
        }

        return null;
    }

    void Ϥ(
        g İ)
    {
        if (İ == null)
        {
            return;
        }

        Ə Ȓ;

        if (!Ϻ
                .TryGetValue(
                    İ.Q,
                    out Ȓ))
        {
            return;
        }

        if (Ȓ.Ǝ ==
            ƌ.M)
        {
            Ϻ.Remove(
                İ.Q);

            return;
        }

        if (İ.E.Enabled)
        {
            İ.E.Enabled =
                false;
        }
    }

    void ш(
        HashSet<long> ә)
    {
        if (Ϻ.Count ==
            0)
        {
            return;
        }

        List<long> Ӛ =
            new List<long>();

        foreach (
            KeyValuePair<long, Ə> Ћ
            in Ϻ)
        {
            if (!ә.Contains(
                    Ћ.Key))
            {
                Ӛ.Add(
                    Ћ.Key);
            }
        }

        for (int Į = 0;
            Į < Ӛ.Count;
            Į++)
        {
            long Ϧ =
                Ӛ[Į];

            Ϻ.Remove(
                Ϧ);

            Ϣ.Remove(
                Ϧ);
        }
    }


    void ɝ()
    {
        bool ӛ =
            Ȏ ==
            ƅ.ƃ;

        bool Ӝ =
            ǻ;

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            bool ӝ =
                İ
                    .E
                    .CubeGrid ==
                Me.CubeGrid;

            bool Ӟ =
                !İ.Y &&
                (ȟ.Ȭ ||
                 İ
                     .a);

            bool ӟ =
                ӛ &&
                ӝ &&
                Ӟ;

            bool ӡ =
                Ӝ &&
                ӝ &&
                Ӡ(
                    İ) &&
                Ӟ;

            İ.N(
                C.ƃ,
                ӟ);

            İ.N(
                C.Ɖ,
                ӡ);

            if (!ӡ)
            {
                ʾ(
                    İ,
                    ƌ
                        .Ɖ);
            }
        }

        for (int Į = 0;
            Į < ˁ.Count;
            Į++)
        {
            ũ ҕ =
                ˁ[Į];

            bool ӝ =
                ҕ
                    .E
                    .CubeGrid ==
                Me.CubeGrid;

            bool Ӣ =
                !ҕ.Y &&
                (ȟ.Ȭ ||
                 ҕ.a);

            ҕ.N(
                C.ƃ,
                ӛ &&
                ӝ &&
                Ӣ);
        }

        ω();
    }

    void ӓ(
        C h)
    {
        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            ʆ[Į]
                .N(
                    h,
                    false);
        }

        for (int Į = 0;
            Į < ˁ.Count;
            Į++)
        {
            ˁ[Į]
                .N(
                    h,
                    false);
        }
    }

    bool Ӡ(
        g İ)
    {
        if (İ == null ||
            ɡ ==
                null ||
            İ
                .E
                .CubeGrid !=
            Me.CubeGrid)
        {
            return false;
        }

        return Vector3D.Dot(
                   n.o(
                       İ
                           .R),
                   ɡ
                       .WorldMatrix
                       .Backward) >=
               Ķ;
    }

    void ω()
    {
        ʿ.Clear();

        if (ɡ ==
            null)
        {
            return;
        }

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            if (Ӡ(
                    İ))
            {
                ʿ.Add(
                    İ);
            }
        }

        Ӕ();
    }

    void щ()
    {
        Ͽ =
            false;

        for (int Į = 0;
            Į < ʆ.Count;
            Į++)
        {
            g İ =
                ʆ[Į];

            if (İ.E.CubeGrid ==
                Me.CubeGrid)
            {
                Ͽ =
                    true;

                break;
            }
        }

        ω();
    }

    void Ӕ()
    {
        Ǽ =
            string.Empty;

        if (!ǻ ||
            ʿ.Count ==
                0)
        {
            return;
        }

        if (ȟ.Ȭ)
        {
            bool ӣ =
                true;

            for (int Į = 0;
                Į <
                    ʿ
                        .Count;
                Į++)
            {
                if (!ʿ[Į]
                        .Y)
                {
                    ӣ =
                        false;

                    break;
                }
            }

            if (ӣ)
            {
                Ǽ =
"Cruise cannot control main-grid "+
"reverse thrusters; all are "+
                    ȟ.Ȳ +
".";
            }

            return;
        }

        bool Ӥ =
            false;

        for (int Į = 0;
            Į <
                ʿ
                    .Count;
            Į++)
        {
            g İ =
                ʿ[Į];

            if (!İ.Y &&
                İ.a)
            {
                Ӥ =
                    true;

                break;
            }
        }

        if (!Ӥ)
        {
            Ǽ =
"Cruise cannot control main-grid "+
"reverse thrusters; add "+
                ȟ.ȱ +
".";
        }
    }


    void ϒ(
        ƅ ϐ,
        ƅ Ϗ)
    {
        if (ϐ ==
                ƅ.ƃ &&
            Ϗ !=
                ƅ.ƃ)
        {
            ӓ(
                C.ƃ);

            Ӑ(
                Ȍ);
        }

        if (ϐ !=
                ƅ.ƃ &&
            Ϗ ==
                ƅ.ƃ)
        {
            Ӑ(
                ɱ
                    .Ư);
        }

        ɝ();
    }

    void ɪ(
        bool Ő)
    {
        І.Clear();

        І
            .AppendLine(
                ȿ)
            .Append("v")
            .AppendLine(
                ˢ)
            .AppendLine();

        І
            .Append("Mode: ")
            .AppendLine(
                ǒ(
                    Ȏ));

        І
            .Append("Controller: ")
            .AppendLine(
                ɡ !=
                        null
                    ? ɡ
                        .CustomName
                    : "NONE");

        ӥ(
            І);

        І
            .Append("Nacelles: ")
            .AppendLine(
                ʂ
                    .Count
                    .ToString());

        І
            .Append(
"Controlled thrust: ")
            .Append(
                (ʅ /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        І
            .Append(
"Capacity F/B: ")
            .Append(
                (ʇ
                     .Ɛ /
                 1000.0)
                .ToString("0.##"))
            .Append("/")
            .Append(
                (ʇ
                     .Ƒ /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        І
            .Append("Residual: ")
            .Append(
                (ʵ
                     .Length() /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        І
            .Append("Gyros: ")
            .AppendLine(
                ˁ
                    .Count
                    .ToString());

        if (Ȏ ==
            ƅ.ƃ)
        {
            І
                .Append(
"Heartbeat age: ")
                .Append(
                    ɥ)
                .AppendLine("/2");
        }

        if (!string.IsNullOrEmpty(
                Ǽ))
        {
            І
                .AppendLine()
                .Append("WARNING: ")
                .AppendLine(
                    Ǽ);
        }

        if (!string.IsNullOrEmpty(
                ϲ))
        {
            І
                .AppendLine()
                .Append(
                    ϴ
                        ? "WARNING: "                        : "Command: ")
                .AppendLine(
                    ϲ);
        }

        І
            .Append("Runtime: ")
            .Append(
                Runtime
                    .LastRunTimeMs
                    .ToString("0.###"))
            .Append(" ms | avg ")
            .Append(
                Љ
                    .ƺ
                    .ToString("0.###"))
            .Append(" | max ")
            .AppendLine(
                Љ
                    .ƻ
                    .ToString("0.###"));

        І
            .Append("Instructions: ")
            .Append(
                Runtime
                    .CurrentInstructionCount)
            .Append("/")
            .AppendLine(
                Runtime
                    .MaxInstructionCount
                    .ToString());

        Echo(
            І.ToString());

        if (!Ő &&
            Ѕ.Count == 0)
        {
            return;
        }

        Ї.Clear();

        Ї
            .AppendLine(
"VECTOR THRUST REDUX")
            .Append("MODE  ")
            .AppendLine(
                ǒ(Ȏ)
                    .ToUpperInvariant());

        Ӧ(
            Ї);

        Ї
            .Append("VECTORS ")
            .AppendLine(
                ʂ
                    .Count
                    .ToString())
            .Append("THRUST ")
            .Append(
                (ʅ /
                 1000.0)
                .ToString("0.0"))
            .AppendLine(" kN")
            .Append("ERROR ")
            .Append(
                (ʵ
                     .Length() /
                 1000.0)
                .ToString("0.0"))
            .AppendLine(" kN");

        if (!string.IsNullOrEmpty(
                Ǽ))
        {
            Ї
                .Append("WARN ")
                .AppendLine(
                    Ǽ);
        }
            
        if (!string.IsNullOrEmpty(
                ϲ))
        {
            Ї
                .Append(
                    ϴ
                        ? "WARN "                        : "CMD  ")
                .AppendLine(
                    ϲ);
        }

        string ӧ =
            Ї.ToString();

        for (int Į = 0;
            Į < Ѕ.Count;
            Į++)
        {
            Ѕ[Į]
                .ſ(ӧ);
        }
    }

    void ӥ(
        StringBuilder ɖ)
    {
        if (Ȏ !=
            ƅ.ƃ)
        {
            ɖ
                .Append("Dampeners: ")
                .AppendLine(
                    Ȍ
                        ? "ON"                        : "OFF");

            ɖ
                .Append("Cruise: ")
                .Append(
                    Ȕ
                        ? "ON"                        : "OFF");

            if (Ȟ)
            {
                ɖ
                    .Append(" @ ")
                    .Append(
                        ȝ
                            .ToString(
"0.###"))
                    .Append(" m/s");
            }

            ɖ.AppendLine();

            Ө(
                ɖ);

            return;
        }

        ɖ
            .Append("Dampeners: local ")
            .Append(
                Ȍ
                    ? "ON"                    : "OFF")
            .Append(" | master ")
            .Append(
                ɱ
                    .Ư
                    ? "ON"                    : "OFF")
            .Append(" | effective ")
            .AppendLine(
                ʟ
                    ? "ON"                    : "OFF");

        ɖ
            .Append("Cruise: local ")
            .Append(
                Ȕ
                    ? "ON"                    : "OFF");

        if (Ȟ)
        {
            ɖ
                .Append(" @ ")
                .Append(
                    ȝ
                        .ToString("0.###"));
        }

        ɖ
            .Append(" | master ")
            .Append(
                ɱ
                    .Ɖ
                    ? "ON"                    : "OFF")
            .Append(" @ ")
            .Append(
                ɱ
                    .Ʊ
                    .ToString("0.###"))
            .AppendLine(" m/s");

        ɖ
            .Append("Gear: local ")
            .Append(
                Ȣ + 1)
            .Append("/")
            .Append(
                ȟ
                    .Ƞ
                    .Count)
            .Append(" | master ")
            .Append(
                ɱ
                    .Ƴ +
                1)
            .Append("/")
            .Append(
                ɱ
                    .ƴ)
            .Append(" (")
            .Append(
                (ɱ
                     .Ʋ *
                 100)
                .ToString("0.##"))
            .AppendLine("%)");
    }

    void Ӧ(
        StringBuilder ɖ)
    {
        if (Ȏ !=
            ƅ.ƃ)
        {
            ɖ
                .Append("DAMP  ")
                .AppendLine(
                    Ȍ
                        ? "ON"                        : "OFF")
                .Append("CRUISE ")
                .Append(
                    Ȕ
                        ? "ON"                        : "OFF");

            if (Ȟ)
            {
                ɖ
                    .Append(" ")
                    .Append(
                        ȝ
                            .ToString("0.##"))
                    .Append("m/s");
            }

            ɖ
                .AppendLine()
                .Append("GEAR  ")
                .Append(
                    Ȣ + 1)
                .Append("/")
                .AppendLine(
                    ȟ
                        .Ƞ
                        .Count
                        .ToString());

            return;
        }

        ɖ
            .Append("DAMP  L:")
            .Append(
                Ȍ
                    ? "ON"                    : "OFF")
            .Append(" M:")
            .AppendLine(
                ɱ
                        .Ư
                    ? "ON"                    : "OFF")
            .Append("CRUISE L:")
            .Append(
                Ȕ
                    ? "ON"                    : "OFF")
            .Append(" M:")
            .Append(
                ɱ
                        .Ɖ
                    ? "ON"                    : "OFF")
            .Append(" ")
            .Append(
                ɱ
                    .Ʊ
                    .ToString("0.##"))
            .AppendLine("m/s")
            .Append("GEAR  L:")
            .Append(
                Ȣ + 1)
            .Append("/")
            .Append(
                ȟ
                    .Ƞ
                    .Count)
            .Append(" M:")
            .Append(
                ɱ
                    .Ƴ +
                1)
            .Append("/")
            .AppendLine(
                ɱ
                    .ƴ
                    .ToString());
    }

    void Ө(
        StringBuilder ɖ)
    {
        int ȡ =
            ȟ
                .Ƞ
                .Count;

        int ө =
            ȡ > 0
                ? MathHelper.Clamp(
                    Ȣ,
                    0,
                    ȡ - 1)
                : 0;

        double ȧ =
            ȡ > 0
                ? ȟ
                    .Ƞ[
                        ө] *
                  100
                : 0;

        ɖ
            .Append("Gear: ")
            .Append(
                ө + 1)
            .Append("/")
            .Append(
                ȡ)
            .Append(" (")
            .Append(
                ȧ
                    .ToString("0.##"))
            .AppendLine("%)");
    }
    readonly List<IMyTerminalBlock> Ӫ =
        new List<IMyTerminalBlock>(),ӫ =
        new List<IMyTerminalBlock>();

    readonly List<IMyBlockGroup> Ӭ =
        new List<IMyBlockGroup>();


    void ə()
    {
        Ɯ ӭ =
            ъ();

        if (!Ϸ)
        {
            ϼ =
                ӭ;

            Ϸ =
                true;

            return;
        }

        if (ӭ ==
            ϼ)
        {
            return;
        }

        ϼ =
            ӭ;

        Ȋ();
    }

    Ɯ ъ()
    {
        Ɯ ӭ =
            new Ɯ();

        Ӫ.Clear();

        GridTerminalSystem.GetBlocks(
            Ӫ);

        for (int Į = 0;
            Į <
                Ӫ
                    .Count;
            Į++)
        {
            IMyTerminalBlock d =
                Ӫ[Į];

            int ӯ =
                Ӯ(
                    d);

            if (ӯ == 0)
            {
                continue;
            }

            ulong ӱ =
                Ӱ;

            ӱ =
                Ӳ(
                    ӱ,
                    unchecked(
                        (ulong)
                            d.EntityId));

            ӱ =
                Ӳ(
                    ӱ,
                    unchecked(
                        (ulong)
                            d
                                .CubeGrid
                                .EntityId));

            ӱ =
                Ӳ(
                    ӱ,
                    unchecked(
                        (ulong)ӯ));

            ӱ =
                Ӳ(
                    ӱ,
                    d
                        .CubeGrid
                        .IsStatic
                        ? 1UL
                        : 0UL);

            F e =
                њ(
                    d.CustomName) |
                њ(
                    d.CustomData);

            ӱ =
                Ӳ(
                    ӱ,
                    unchecked(
                        (ulong)(int)e));

            IMyMotorStator Ī =
                d as
                    IMyMotorStator;

            if (Ī != null)
            {
                ӱ =
                    Ӳ(
                        ӱ,
                        Ī.TopGrid !=
                                null
                            ? unchecked(
                                (ulong)
                                    Ī
                                        .TopGrid
                                        .EntityId)
                            : 0UL);

                ӱ =
                    Ӳ(
                        ӱ,
                        ӳ(
                            Ī
                                .LowerLimitRad));

                ӱ =
                    Ӳ(
                        ӱ,
                        ӳ(
                            Ī
                                .UpperLimitRad));
            }

            IMyPistonBase ҫ =
                d as
                    IMyPistonBase;

            if (ҫ != null)
            {
                ӱ =
                    Ӳ(
                        ӱ,
                        ҫ.TopGrid !=
                                null
                            ? unchecked(
                                (ulong)
                                    ҫ
                                        .TopGrid
                                        .EntityId)
                            : 0UL);
            }

            IMyShipConnector Ϛ =
                d as
                    IMyShipConnector;

            if (Ϛ != null)
            {
                ӱ =
                    Ӳ(
                        ӱ,
                        Ϛ
                                .OtherConnector !=
                            null
                            ? unchecked(
                                (ulong)
                                    Ϛ
                                        .OtherConnector
                                        .EntityId)
                            : 0UL);
            }

            IMyProgrammableBlock і =
                d as
                    IMyProgrammableBlock;

            if (і !=
                null)
            {
                bool Ӵ =
                    ї(
                        і);

                ӱ =
                    Ӳ(
                        ӱ,
                        Ӵ
                            ? 1UL
                            : 0UL);

                if (Ӵ)
                {
                    ӱ =
                        Ӳ(
                            ӱ,
                            ј(
                                і)
                                ? 1UL
                                : 0UL);

                    ӱ =
                        Ӳ(
                            ӱ,
                            ҵ(
                                і)
                                ? 1UL
                                : 0UL);
                }
            }

            ӵ(
                ref ӭ,
                ӱ,
                d.EntityId);
        }

        Ӷ(
            ref ӭ);

        return ӭ;
    }

    void Ӷ(
        ref Ɯ ӭ)
    {
        Ӭ.Clear();

        GridTerminalSystem.GetBlockGroups(
            Ӭ);

        for (int Į = 0;
            Į <
                Ӭ
                    .Count;
            Į++)
        {
            IMyBlockGroup Ҩ =
                Ӭ[Į];

            F ҩ =
                њ(
                    Ҩ.Name);

            if (ҩ ==
                F.M)
            {
                continue;
            }

            ӫ.Clear();

            Ҩ.GetBlocks(
                ӫ);

            ulong ӷ =
                Ӱ;

            ӷ =
                Ӳ(
                    ӷ,
                    Ӹ(
                        Ҩ.Name));

            ӷ =
                Ӳ(
                    ӷ,
                    unchecked(
                        (ulong)
                            (int)ҩ));

            ulong ӹ = 0;
            ulong Ӻ = 0;

            for (int ĵ = 0;
                ĵ <
                    ӫ
                        .Count;
                ĵ++)
            {
                ulong ӻ =
                    Ӳ(
                        Ӱ,
                        unchecked(
                            (ulong)
                                ӫ[ĵ]
                                    .EntityId));

                ӹ ^=
                    Ӽ(
                        ӻ,
                        (int)(
                            ӫ[ĵ]
                                .EntityId &
                            63));

                Ӻ +=
                    ӻ *
                    ӽ;
            }

            ӷ =
                Ӳ(
                    ӷ,
                    ӹ);

            ӷ =
                Ӳ(
                    ӷ,
                    Ӻ);

            long Ӿ =
                unchecked(
                    (long)
                        Ӹ(
                            Ҩ.Name));

            ӵ(
                ref ӭ,
                ӷ,
                Ӿ);
        }
    }

    static void ӵ(
        ref Ɯ ӭ,
        ulong ӱ,
        long ӿ)
    {
        ӭ.Ɲ++;

        ӭ.ƞ ^=
            Ӽ(
                ӱ,
                (int)(ӿ & 63));

        ӭ.Ɵ +=
            ӱ *
            ӽ;
    }

    static int Ӯ(
        IMyTerminalBlock d)
    {
        if (d is
            IMyMotorStator)
        {
            return 1;
        }

        if (d is
            IMyPistonBase)
        {
            return 2;
        }

        if (d is
            IMyThrust)
        {
            return 3;
        }

        if (d is
            IMyGyro)
        {
            return 4;
        }

        if (d is
            IMyShipController)
        {
            return 5;
        }

        if (d is
            IMyShipConnector)
        {
            return 6;
        }

        if (d is
            IMyLandingGear)
        {
            return 7;
        }

        if (d is
            IMyProgrammableBlock)
        {
            return 8;
        }

        if (d is
            IMyTimerBlock)
        {
            return 9;
        }

        if (d is
                IMyTextPanel ||
            d is
                IMyTextSurfaceProvider)
        {
            return 10;
        }

        return 0;
    }

    static ulong ӳ(
        double ğ)
    {
        if (double.IsNaN(ğ))
        {
            return ulong.MaxValue;
        }

        if (double.IsPositiveInfinity(
                ğ))
        {
            return ulong.MaxValue -
                1;
        }

        if (double.IsNegativeInfinity(
                ğ))
        {
            return ulong.MaxValue -
                2;
        }

        return unchecked(
            (ulong)(uint)
                ğ.GetHashCode());
    }

    static ulong Ӹ(
        string ž)
    {
        ulong ơ =
            Ӱ;

        if (string.IsNullOrEmpty(
                ž))
        {
            return ơ;
        }

        for (int Į = 0;
            Į < ž.Length;
            Į++)
        {
            char Ȫ =
                char.ToUpperInvariant(
                    ž[Į]);

            ơ ^=
                Ȫ;

            ơ *=
                ӽ;
        }

        return ơ;
    }

    static ulong Ӳ(
        ulong ơ,
        ulong ğ)
    {
        ơ ^=
            ğ;

        ơ *=
            ӽ;

        ơ ^=
            ğ >> 32;

        ơ *=
            ӽ;

        return ơ;
    }

    static ulong Ӽ(
        ulong ğ,
        int К)
    {
        К &=
            63;

        if (К == 0)
        {
            return ğ;
        }

        return ğ << К |
               ğ >>
               (64 - К);
    }

    const ulong Ӱ =
        14695981039346656037UL,ӽ =
        1099511628211UL;


    void ɜ()
    {
        HashSet<long> Ԁ =
            new HashSet<long>();

        for (int Į = 0;
            Į < ц.Count;
            Į++)
        {
            IMyShipConnector Ϛ =
                ц[Į];

            if (Ϛ == null ||
                Ϛ.Closed)
            {
                continue;
            }

            long ԁ =
                Ϛ
                        .OtherConnector !=
                    null
                    ? Ϛ
                        .OtherConnector
                        .EntityId
                    : 0;

            long Ԃ;

            if (!Ӊ
                    .TryGetValue(
                        Ϛ.EntityId,
                        out Ԃ) ||
                Ԃ !=
                    ԁ)
            {
                Ӊ[
                    Ϛ.EntityId] =
                    ԁ;

                Ȋ();
            }

            Ԁ.Add(
                Ϛ.EntityId);
        }

        ԃ(
            Ԁ);

        HashSet<long> Ԅ =
            new HashSet<long>();

        for (int Į = 0;
            Į < ϗ.Count;
            Į++)
        {
            IMyLandingGear ԅ =
                ϗ[Į]
                    .ǭ;

            if (ԅ == null ||
                ԅ.Closed)
            {
                continue;
            }

            bool Ԇ;

            if (!ӊ
                    .TryGetValue(
                        ԅ.EntityId,
                        out Ԇ) ||
                Ԇ !=
                    ԅ.IsLocked)
            {
                ӊ[
                    ԅ.EntityId] =
                    ԅ.IsLocked;

                Ȋ();
            }

            Ԅ.Add(
                ԅ.EntityId);
        }

        ԇ(
            Ԅ);
    }

    void ԃ(
        HashSet<long> Ԉ)
    {
        if (Ӊ.Count ==
            0)
        {
            return;
        }

        List<long> ԉ =
            new List<long>();

        foreach (
            KeyValuePair<long, long> Ћ
            in Ӊ)
        {
            if (!Ԉ.Contains(
                    Ћ.Key))
            {
                ԉ.Add(
                    Ћ.Key);
            }
        }

        for (int Į = 0;
            Į < ԉ.Count;
            Į++)
        {
            Ӊ.Remove(
                ԉ[Į]);
        }
    }

    void ԇ(
        HashSet<long> Ԉ)
    {
        if (ӊ.Count ==
            0)
        {
            return;
        }

        List<long> ԉ =
            new List<long>();

        foreach (
            KeyValuePair<long, bool> Ћ
            in ӊ)
        {
            if (!Ԉ.Contains(
                    Ћ.Key))
            {
                ԉ.Add(
                    Ћ.Key);
            }
        }

        for (int Į = 0;
            Į < ԉ.Count;
            Į++)
        {
            ӊ.Remove(
                ԉ[Į]);
        }
    }
}
