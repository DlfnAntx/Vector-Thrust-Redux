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

                if (!İ.c)
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

    sealed class Ʈ
    {
        public long Ƥ,ƥ,Ʀ;

        public Vector3D Ƨ;

        public bool ƨ,Ɖ,Ʃ;
        public double ƪ,ƫ;

        public int Ƭ,ƭ;

        public void ƙ(
            Ʈ Ɨ)
        {
            Ƥ =
                Ɨ.Ƥ;

            ƥ =
                Ɨ.ƥ;

            Ʀ =
                Ɨ.Ʀ;

            Ƨ =
                Ɨ.Ƨ;

            ƨ =
                Ɨ.ƨ;

            Ɖ =
                Ɨ.Ɖ;

            ƪ =
                Ɨ.ƪ;

            Ƭ =
                Ɨ.Ƭ;

            ƭ =
                Ɨ.ƭ;

            ƫ =
                Ɨ.ƫ;

            Ʃ =
                Ɨ.Ʃ;
        }

        public void ĺ()
        {
            Ƥ = 0;
            ƥ = 0;
            Ʀ = 0;

            Ƨ =
                Vector3D.Zero;

            ƨ = true;
            Ɖ = false;
            ƪ = 0;

            Ƭ = 0;
            ƭ = 0;
            ƫ = 0;

            Ʃ = false;
        }
    }

    sealed class ƶ
    {
        readonly Program A;

        double Ư;
        double ư,Ʊ;
        int Ʋ;

        public double Ƴ
        {
            get
            {
                return Ư;
            }
        }

        public double ƴ
        {
            get
            {
                return ư;
            }
        }

        public double Ƶ
        {
            get
            {
                return Ʊ;
            }
        }

        public ƶ(
            Program A)
        {
            this.A = A;
        }

        public void Ʒ()
        {
            Ʊ =
                A.Runtime
                    .LastRunTimeMs;
        }

        public void ƹ()
        {
            double Ƹ =
                A.Runtime
                    .LastRunTimeMs;

            Ʋ++;

            if (Ʋ == 1)
            {
                Ư =
                    Ƹ;

                ư =
                    Ƹ;

                return;
            }

            Ư +=
                (Ƹ -
                 Ư) *
                0.05;

            if (Ƹ >
                ư)
            {
                ư =
                    Ƹ;
            }
            else if (Ʋ %
                     600 == 0)
            {
                ư =
                    Ư;
            }
        }
    }

    public static class n
    {

public static Vector3D o(
            Vector3D ƺ)
        {
            if (Vector3D.IsZero(
                    ƺ))
            {
                return Vector3D.Zero;
            }

            if (Vector3D.IsUnit(
                    ref ƺ))
            {
                return ƺ;
            }

            return Vector3D.Normalize(
                ƺ);
        }

public static Vector3D ą(
            Vector3D ƻ,
            Vector3D Ƽ)
        {
            double ƽ =
                Ƽ.LengthSquared();

            if (ƻ.LengthSquared() <=
                    p ||
                ƽ <=
                    p)
            {
                return Vector3D.Zero;
            }

            return ƻ -
                   Vector3D.Dot(ƻ, Ƽ) /
                   ƽ *
                   Ƽ;
        }

public static Vector3D ƾ(
            Vector3D ƻ,
            Vector3D Ƽ)
        {
            double ƽ =
                Ƽ.LengthSquared();

            if (ƻ.LengthSquared() <=
                    p ||
                ƽ <=
                    p)
            {
                return Vector3D.Zero;
            }

            return Vector3D.Dot(ƻ, Ƽ) /
                   ƽ *
                   Ƽ;
        }

        public static double í(
            Vector3D ƻ,
            Vector3D Ƽ)
        {
            double ƽ =
                Math.Sqrt(
                    ƻ.LengthSquared() *
                    Ƽ.LengthSquared());

            if (ƽ <=
                p)
            {
                return 0;
            }

            return MathHelper.Clamp(
                Vector3D.Dot(ƻ, Ƽ) /
                ƽ,
                -1,
                1);
        }

        public static Vector3D Ŭ(
            Vector3D ƺ,
            double ƿ)
        {
            if (ƿ <= 0)
            {
                return Vector3D.Zero;
            }

            double ǀ =
                ƺ.LengthSquared();

            double ǁ =
                ƿ *
                ƿ;

            if (ǀ <=
                ǁ)
            {
                return ƺ;
            }

            if (ǀ <=
                p)
            {
                return Vector3D.Zero;
            }

            return ƺ *
                   (ƿ /
                    Math.Sqrt(
                        ǀ));
        }

        public static double Ø(
            double ǂ)
        {
            while (ǂ >
                   Math.PI)
            {
                ǂ -=
                    MathHelper.TwoPi;
            }

            while (ǂ <
                   -Math.PI)
            {
                ǂ +=
                    MathHelper.TwoPi;
            }

            return ǂ;
        }

        public static Vector3D ë(
            Vector3D ƺ,
            Vector3D ǃ,
            double ǂ)
        {
            ǃ =
                o(ǃ);

            if (ǃ.LengthSquared() <=
                p)
            {
                return ƺ;
            }

            double Ǆ =
                Math.Cos(ǂ);

            double ǅ =
                Math.Sin(ǂ);

            return ƺ * Ǆ +
                   Vector3D.Cross(
                       ǃ,
                       ƺ) *
                   ǅ +
                   ǃ *
                   Vector3D.Dot(
                       ǃ,
                       ƺ) *
                   (1.0 - Ǆ);
        }

public static double Ē(
            Vector3D m,
            Vector3D đ,
            Vector3D ǆ)
        {
            Vector3D Ǉ =
                ą(
                    m,
                    ǆ);

            Vector3D ǈ =
                ą(
                    đ,
                    ǆ);

            if (Ǉ
                    .LengthSquared() <=
                    p ||
                ǈ
                    .LengthSquared() <=
                    p)
            {
                return 0;
            }

            Ǉ =
                o(
                    Ǉ);

            ǈ =
                o(
                    ǈ);

            ǆ =
                o(
                    ǆ);

            return Math.Atan2(
                Vector3D.Dot(
                    ǆ,
                    Vector3D.Cross(
                        Ǉ,
                        ǈ)),
                Vector3D.Dot(
                    Ǉ,
                    ǈ));
        }

        public static Vector3D Ĳ(
            Vector3D ǉ,
            MatrixD Ǌ)
        {
            return Vector3D.TransformNormal(
                ǉ,
                MatrixD.Transpose(
                    Ǌ));
        }

        public static Vector3D Ĭ(
            Vector3D ƚ,
            MatrixD Ǌ)
        {
            return Vector3D.TransformNormal(
                ƚ,
                Ǌ);
        }
    }

    static string ǋ(
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

    sealed class Ǒ
    {
        public readonly IMyCubeGrid ǌ;

        public readonly List<Ǎ> ǎ =
            new List<Ǎ>();

        public Ǐ ǐ;
        public Ǒ ǒ;
        public Ǎ Ǔ;

        public int ǔ =
            int.MaxValue;

        public bool Ǖ;

        public Ǒ(
            IMyCubeGrid Ŗ)
        {
            ǌ = Ŗ;
        }
    }

    sealed class Ǎ
    {
        public readonly Ǒ ǖ,Ǘ;

        public readonly IMyTerminalBlock ǘ;

        public Ǎ(
            Ǒ ƻ,
            Ǒ Ƽ,
            IMyTerminalBlock Ǚ)
        {
            ǖ = ƻ;
            Ǘ = Ƽ;
            ǘ = Ǚ;
        }

        public Ǒ Ǜ(
            Ǒ ǚ)
        {
            return ǚ == ǖ
                ? Ǘ
                : ǖ;
        }
    }

    sealed class Ǐ
    {
        public readonly List<Ǒ> ǜ =
            new List<Ǒ>();

        public readonly List<IMyShipController> ǝ =
            new List<IMyShipController>();

        public readonly List<IMyProgrammableBlock> Ǟ =
            new List<IMyProgrammableBlock>();

        public bool Ǖ,ǟ,Ǡ,ǡ,Ǣ = true,ǣ = true;

        public IMyProgrammableBlock Ǥ;
    }

    sealed class ǧ
    {
        public IMyShipConnector ǖ,Ǘ;

        public Ǒ ǥ,Ǧ;
    }

    sealed class Ǫ
    {
        public IMyShipConnector Ǩ;
        public ǧ ǩ;
    }

    sealed class ǫ
    {
        public IMyLandingGear Ǩ;
    }
    sealed class Ǻ
    {
        public bool Ǭ = true,ǭ = true,Ǯ = true,ǯ,ǰ = true;
        public readonly List<double> Ǳ = new List<double>
        {
            0.15,
            0.50,
            1.00
        };

        public string ǲ = "[VT-use]",ǳ = "[VT-ignore]",Ǵ = "[VT-status]",ǵ = "[VT-park]",Ƕ = "[VT-unpark]";

        public int Ƿ,Ǹ,ǹ;
    }

    readonly MyIni ǻ = new MyIni();

    bool Ȓ(bool Ǽ)
    {
        string ǽ = Me.CustomData ?? string.Empty;

        if (!Ǽ && ǽ == Ǿ)
        {
            return false;
        }

        ǻ.Clear();

        MyIniParseResult ǿ;

        if (!ǻ.TryParse(ǽ, out ǿ))
        {
            Echo(
                Ȁ +
"\n\nCustom Data could not be parsed as INI:\n"+
                ǿ);

            Ǿ = ǽ;
            return false;
        }

        bool Ȃ = ȁ.Ǭ;
        bool ȃ = ȁ.ǭ;
        bool Ȅ = ȁ.Ǯ;

        string ȅ = ȁ.ǲ;
        string Ȇ = ȁ.ǳ;
        string ȇ = ȁ.Ǵ;
        string Ȉ = ȁ.ǵ;
        string ȉ = ȁ.Ƕ;


        ȁ.Ǭ = ǻ
            .Get(Ȋ, "Greedy")
            .ToBoolean(ȁ.Ǭ);

        ȁ.ǭ = ǻ
            .Get(Ȋ, "CanMaster")
            .ToBoolean(ȁ.ǭ);

        ȁ.Ǯ = ǻ
            .Get(Ȋ, "CanSlave")
            .ToBoolean(ȁ.Ǯ);


        ȁ.ǯ = ǻ
            .Get("Parking", "ParkOnlyByCommand")
            .ToBoolean(ȁ.ǯ);


        ȁ.ǰ = ǻ
            .Get("Flight", "CruiseLevelsWithGravity")
            .ToBoolean(ȁ.ǰ);

        ȋ(
            ǻ.Get("Flight", "GearPercentages")
                .ToString("15; 50; 100"));

        if (Ȍ >= ȁ.Ǳ.Count)
        {
            Ȍ = ȁ.Ǳ.Count - 1;
        }


        ȁ.ǲ = ȍ(
"Tags",
"Use",
            ȁ.ǲ);

        ȁ.ǳ = ȍ(
"Tags",
"Ignore",
            ȁ.ǳ);

        ȁ.Ǵ = ȍ(
"Tags",
"Status",
            ȁ.Ǵ);

        ȁ.ǵ = ȍ(
"Tags",
"ParkTimer",
            ȁ.ǵ);

        ȁ.Ƕ = ȍ(
"Tags",
"UnparkTimer",
            ȁ.Ƕ);


        ȁ.Ƿ = Math.Max(
            0,
            ǻ
                .Get("Performance", "Update1Skip")
                .ToInt32(ȁ.Ƿ));

        ȁ.Ǹ = Math.Max(
            0,
            ǻ
                .Get("Performance", "Update10Skip")
                .ToInt32(ȁ.Ǹ));

        ȁ.ǹ = Math.Max(
            0,
            ǻ
                .Get("Performance", "Update100Skip")
                .ToInt32(ȁ.ǹ));

        Ȏ();

        string ȏ = ǻ.ToString();

        if (ȏ != Me.CustomData)
        {
            Me.CustomData = ȏ;
        }

        Ǿ = Me.CustomData;

        bool Ȑ =
            Ȃ != ȁ.Ǭ ||
            ȃ != ȁ.ǭ ||
            Ȅ != ȁ.Ǯ ||
            !ȅ.Equals(ȁ.ǲ, StringComparison.OrdinalIgnoreCase) ||
            !Ȇ.Equals(ȁ.ǳ, StringComparison.OrdinalIgnoreCase) ||
            !ȇ.Equals(ȁ.Ǵ, StringComparison.OrdinalIgnoreCase) ||
            !Ȉ.Equals(ȁ.ǵ, StringComparison.OrdinalIgnoreCase) ||
            !ȉ.Equals(ȁ.Ƕ, StringComparison.OrdinalIgnoreCase);

        if (!Ǽ && Ȑ)
        {
            ȑ();
        }

        return true;
    }

    string ȍ(
        string ȓ,
        string Ȕ,
        string ȕ)
    {
        string ğ = ǻ
            .Get(ȓ, Ȕ)
            .ToString(ȕ)
            .Trim();

        return ğ.Length == 0 ? ȕ : ğ;
    }

    void ȋ(string Ȗ)
    {
        string[] ȗ = Ȗ.Split(
            new[] { ';', ',' },
            StringSplitOptions.RemoveEmptyEntries);

        List<double> Ș = new List<double>();

        for (int Į = 0; Į < ȗ.Length; Į++)
        {
            double ș;

            if (!double.TryParse(
                ȗ[Į].Trim(),
                out ș))
            {
                continue;
            }

            if (ș > 0)
            {
                Ș.Add(ș / 100.0);
            }
        }

        if (Ș.Count == 0)
        {
            return;
        }

        ȁ.Ǳ.Clear();
        ȁ.Ǳ.AddRange(Ș);
    }

    void Ȏ()
    {
        ǻ.Set(Ȋ, "Greedy", ȁ.Ǭ);
        ǻ.Set(Ȋ, "CanMaster", ȁ.ǭ);
        ǻ.Set(Ȋ, "CanSlave", ȁ.Ǯ);

        ǻ.SetSectionComment(
            Ȋ,
" Vector Thrust Redux ownership and coordination.\n"+
" Greedy controls eligible mechanical-subgrid blocks unless ignored.\n"+
" Main-grid player thrusters and gyros remain read-only unless explicitly tagged.");

        ǻ.Set(
"Parking",
"ParkOnlyByCommand",
            ȁ.ǯ);

        ǻ.Set(
"Flight",
"CruiseLevelsWithGravity",
            ȁ.ǰ);

        ǻ.Set(
"Flight",
"GearPercentages",
            Ț());

        ǻ.Set("Tags", "Use", ȁ.ǲ);
        ǻ.Set("Tags", "Ignore", ȁ.ǳ);
        ǻ.Set("Tags", "Status", ȁ.Ǵ);
        ǻ.Set("Tags", "ParkTimer", ȁ.ǵ);
        ǻ.Set("Tags", "UnparkTimer", ȁ.Ƕ);

        ǻ.SetComment(
"Tags",
"Use",
" Tag may appear in a block name, group name, or block Custom Data.");

        ǻ.SetComment(
"Tags",
"Ignore",
" Ignore always prevents Redux from modifying the block.");

        ǻ.Set(
"Performance",
"Update1Skip",
            ȁ.Ƿ);

        ǻ.Set(
"Performance",
"Update10Skip",
            ȁ.Ǹ);

        ǻ.Set(
"Performance",
"Update100Skip",
            ȁ.ǹ);

        ǻ.SetSectionComment(
"Performance",
" Number of matching update intervals skipped between executions.\n"+
" Heartbeat publication is never skipped.");
    }

    string Ț()
    {
        StringBuilder ț = new StringBuilder();

        for (int Į = 0; Į < ȁ.Ǳ.Count; Į++)
        {
            if (Į > 0)
            {
                ț.Append("; ");
            }

            ț.Append(
                (ȁ.Ǳ[Į] * 100.0)
                    .ToString("0.########"));
        }

        return ț.ToString();
    }
    readonly List<ŝ> Ȝ =
        new List<ŝ>();

    readonly HashSet<H> ȝ =
        new HashSet<H>();

    IMyShipController Ȟ;


    void ȟ()
    {
        Ȓ(false);
        ȑ();
    }

    void Ȱ()
    {
        Ƞ();
        ȡ();
        Ȣ();

        ȣ =
            ȁ.ǭ &&
            Ȥ != null &&
            Ȥ.IsUnderControl;

        ȥ();

        if (ȁ.Ǯ &&
            !ȣ &&
            !Ȧ)
        {
            ȧ();
        }

        if (Ȩ)
        {
            ȩ = 0;
        }
        else if (Ȫ !=
                 long.MinValue)
        {
            ȩ++;
        }

        ȫ =
            Ȫ !=
                long.MinValue &&
            ȩ < 2;

        Ȩ = false;

        Ȭ();

        if (ȭ == ƅ.Ƅ)
        {
            Ȯ();
        }

        ȯ(false);
    }

    void ɂ(double Ô)
    {
        Ƞ();

        if (ȭ == ƅ.Ƅ ||
            ȭ == ƅ.ƀ ||
            Ȥ == null)
        {
            ȱ();
            Ȳ();
            return;
        }

        ȳ();
        ȴ();

        Vector3D ȵ =
            Ȥ.CenterOfMass;

        Vector3D ȶ;

        if (ȭ == ƅ.ƃ)
        {
            ȶ =
                ȷ
                    .Ƨ *
                ȸ;

            ȶ -= ȹ();
        }
        else
        {
            ȶ = Ⱥ(
                Ô);

            if (ȸ >
                U)
            {
                Ȼ =
                    n.Ŭ(
                        ȶ /
                        ȸ,
                        1.0);
            }
            else
            {
                Ȼ =
                    Vector3D.Zero;
            }
        }

        ȼ = ȶ;

        Ƚ(
            ȶ,
            ȵ);

        bool ȿ =
            ȭ == ƅ.ƃ
                ? ȷ
                    .Ʃ
                : Ⱦ &&
                  ȁ
                      .ǰ;

        ɀ(
            Ɂ,
            ȿ);
    }

    void Ȣ()
    {
        for (int Į = 0;
            Į < Ƀ.Count;
            Į++)
        {
            Ƀ[Į]
                .ĸ();
        }

        ȴ();
    }

    void ȴ()
    {
        ȸ = 0;

        for (int Į = 0;
            Į < Ʉ.Count;
            Į++)
        {
            g İ =
                Ʉ[Į];

            if (!İ.c)
            {
                continue;
            }

            ȸ +=
                İ.V;
        }
    }


    Vector3D Ⱥ(
        double Ô)
    {
        MyShipMass Ʌ =
            Ȥ
                .CalculateShipMass();

        double Ɇ =
            Ʌ.PhysicalMass;

        if (Ɇ <= U)
        {
            return Vector3D.Zero;
        }

        MyShipVelocities ɇ =
            Ȥ
                .GetShipVelocities();

        Vector3D Ɉ =
            ɇ.LinearVelocity;

        Vector3D ɉ =
            Ȥ
                .GetNaturalGravity();

        Vector3D Ɋ =
            Vector3D.TransformNormal(
                Ȥ
                    .MoveIndicator,
                Ȥ
                    .WorldMatrix);

        bool ɋ =
            Ɋ.LengthSquared() >
            p;

        Vector3D Ɍ =
            n.o(
                Ɋ);

        double ɍ =
            ȸ /
            Ɇ;

        double Ɏ =
            ȁ.Ǳ[
                MathHelper.Clamp(
                    Ȍ,
                    0,
                    ȁ
                        .Ǳ.Count - 1)];

        Vector3D ɏ =
            Ɍ *
            ɍ *
            Ɏ;

        ɐ =
            Ȥ
                .DampenersOverride;

        if (ɐ)
        {
            Vector3D ɑ =
                Ɉ;

            if (ɋ)
            {
                double ɒ =
                    Vector3D.Dot(
                        Ɉ,
                        Ɍ);

                if (ɒ > 0)
                {
                    ɑ -=
                        Ɍ *
                        ɒ;
                }
            }

            if (Ⱦ)
            {
                Vector3D ɓ =
                    Ȥ
                        .WorldMatrix.Forward;

                double ɔ =
                    Vector3D.Dot(
                        ɑ,
                        ɓ);

                if (ɔ > 0)
                {
                    ɑ -=
                        ɓ *
                        ɔ;
                }
            }

            Vector3D ɕ =
                -ɑ /
                Math.Max(
                    Ô,
                    è);

            ɕ =
                n.Ŭ(
                    ɕ,
                    ɍ);

            ɏ +=
                ɕ;
        }

        ɏ =
            n.Ŭ(
                ɏ,
                ɍ);

        Vector3D ɖ =
            Ɇ *
            (ɏ - ɉ);

        return ɖ -
               ȹ();
    }

    Vector3D ȹ()
    {
        Vector3D Ő = Vector3D.Zero;

        for (int Į = 0;
            Į <
                ɗ.Count;
            Į++)
        {
            Ő +=
                ɗ[Į]
                    .W;
        }

        return Ő;
    }


    void Ƚ(
        Vector3D ȶ,
        Vector3D ȵ)
    {
        ɘ = ȶ;
        Ɂ = Vector3D.Zero;

        for (int Į = 0;
            Į < Ʉ.Count;
            Į++)
        {
            Ʉ[Į]
                .l();
        }

        ȝ.Clear();

        for (int Į = 0;
            Į <
                ə.Count;
            Į++)
        {
            ɚ(
                ə[Į],
                ref ɘ,
                ȵ,
                ref Ɂ);
        }

        for (int Į = 0;
            Į < Ƀ.Count;
            Į++)
        {
            Ƀ[Į]
                .œ(
                    ref ɘ,
                    ȵ,
                    ref Ɂ);
        }

        Ȝ.Clear();

        for (int Į = 0;
            Į < ɛ.Count;
            Į++)
        {
            Ȝ.Add(
                ɛ[Į]);
        }

        while (Ȝ.Count > 0 &&
               ɘ
                   .LengthSquared() >
               U * U)
        {
            int ɜ = -1;
            double ɝ = U;

            for (int Į = 0;
                Į <
                    Ȝ.Count;
                Į++)
            {
                double ɞ =
                    Ȝ[Į]
                        .Ŝ(
                            ɘ);

                if (ɞ <= ɝ)
                {
                    continue;
                }

                ɝ = ɞ;
                ɜ = Į;
            }

            if (ɜ < 0)
            {
                break;
            }

            ŝ ɟ =
                Ȝ[
                    ɜ];

            Ȝ.RemoveAt(
                ɜ);

            Vector3D ɠ =
                ɟ.Ś(
                    ɘ);

            if (ɠ
                    .LengthSquared() <=
                p)
            {
                continue;
            }

            for (int Į = 0;
                Į < ɟ.Ř.Count;
                Į++)
            {
                H ɡ =
                    ɟ.Ř[Į];

                ɡ.Ŋ(ɠ);
                ȝ.Add(ɡ);
            }

            for (int Į = 0;
                Į < ɟ.Ř.Count;
                Į++)
            {
                ɟ.Ř[Į]
                    .Œ(
                        ref ɘ,
                        ȵ,
                        ref Ɂ);
            }
        }

        for (int Į = 0;
            Į < Ƀ.Count;
            Į++)
        {
            H ɡ =
                Ƀ[Į];

            if (!ȝ.Contains(
                    ɡ))
            {
                ɡ.Ŋ(
                    Vector3D.Zero);
            }
        }

        for (int Į = 0;
            Į < Ʉ.Count;
            Į++)
        {
            Ʉ[Į]
                .º();
        }
    }

    void ɚ(
        g İ,
        ref Vector3D s,
        Vector3D ȵ,
        ref Vector3D ō)
    {
        double ŏ =
            İ.x(
                ref s);

        if (ŏ <= U)
        {
            return;
        }

        Vector3D Ő =
            İ.R *
            ŏ;

        Vector3D ő =
            İ.E.GetPosition() -
            ȵ;

        ō +=
            Vector3D.Cross(ő, Ő);
    }


    void ɀ(
        Vector3D ō,
        bool ȿ)
    {
        if (ɢ.Count == 0 ||
            Ȥ == null)
        {
            return;
        }

        double ɣ = 0;

        for (int Į = 0;
            Į < ɢ.Count;
            Į++)
        {
            ɣ +=
                ɢ[Į]
                    .ř;
        }

        if (ɣ <=
            p)
        {
            Ȳ();
            return;
        }

        Vector3D ɤ =
            -ō /
            ɣ *
            ŭ;

        if (ȿ)
        {
            Vector3D ɉ =
                Ȥ
                    .GetNaturalGravity();

            if (ɉ.LengthSquared() >
                p)
            {
                Vector3D ɥ =
                    -n.o(
                        ɉ);

                Vector3D ɦ =
                    Ȥ
                        .WorldMatrix.Up;

                Vector3D ɧ =
                    Vector3D.Cross(
                        ɦ,
                        ɥ);

                double q =
                    MathHelper.Clamp(
                        Vector3D.Dot(
                            ɦ,
                            ɥ),
                        -1,
                        1);

                double ɨ =
                    Math.Atan2(
                        ɧ.Length(),
                        q);

                if (ɧ
                        .LengthSquared() >
                    p)
                {
                    ɧ =
                        n.o(
                            ɧ);

                    ɤ +=
                        ɧ *
                        ɨ *
                        ɩ;
                }

                Vector3D ɪ =
                    Ȥ
                        .GetShipVelocities()
                        .AngularVelocity;

                Vector3D ɫ =
                    n.ą(
                        ɪ,
                        ɥ);

                ɤ -=
                    ɫ *
                    ɬ;
            }
        }

        if (ɤ.LengthSquared() <=
            Ų *
            Ų)
        {
            Ȳ();
            return;
        }

        for (int Į = 0;
            Į < ɢ.Count;
            Į++)
        {
            ɢ[Į]
                .Ŵ(
                    ɤ);
        }
    }

    void Ȳ()
    {
        for (int Į = 0;
            Į < ɢ.Count;
            Į++)
        {
            ɢ[Į]
                .Ū();
        }
    }


    void Ƞ()
    {
        IMyShipController ɭ = null;

        for (int Į = 0;
            Į < ɮ.Count;
            Į++)
        {
            IMyShipController ɯ =
                ɮ[Į];

            if (ɯ == null ||
                ɯ.Closed ||
                !ɯ.IsFunctional ||
                !ɯ.CanControlShip)
            {
                continue;
            }

            if (ɯ.IsUnderControl)
            {
                ɭ = ɯ;
                break;
            }

            if (ɭ == null ||
                ɯ.IsMainCockpit)
            {
                ɭ = ɯ;
            }
        }

        Ȥ = ɭ;
        ɰ =
            Ȥ == null;

        ȣ =
            ȁ.ǭ &&
            Ȥ != null &&
            Ȥ.IsUnderControl;
    }

    void Ȭ()
    {
        Ƞ();

        if (ȭ == ƅ.ƃ &&
            !ȫ)
        {
            ɱ =
                ɲ;
        }

        ƅ ɳ;

        if (ɰ ||
            Me.CubeGrid.IsStatic ||
            Ȧ)
        {
            ɳ =
                ƅ.Ƅ;
        }
        else if (ȁ.Ǯ &&
                 ȫ &&
                 !ȣ)
        {
            ɳ =
                ƅ.ƃ;
        }
        else if (ɴ ||
                 ɱ)
        {
            ɳ =
                ƅ.Ƅ;
        }
        else if (ȣ)
        {
            ɳ =
                ƅ.Ƃ;
        }
        else
        {
            ɳ =
                ƅ.Ɓ;
        }

        if (ɳ == ȭ)
        {
            return;
        }

        ɵ(ɳ);
    }

    void ɵ(
        ƅ ɶ)
    {
        ƅ ɷ = ȭ;

        if (ɷ ==
                ƅ.Ƅ &&
            ɶ !=
                ƅ.Ƅ)
        {
            ɸ();
        }

        if (ɷ ==
                ƅ.ƃ &&
            ɶ !=
                ƅ.ƃ &&
            !ȫ)
        {
            ɱ =
                ɲ;
        }

        ȭ = ɶ;

        if (ɶ ==
            ƅ.ƃ)
        {
            ɲ =
                ɷ ==
                ƅ.Ƅ;

            ɱ = false;
        }

        if (ɶ ==
                ƅ.Ƅ &&
            ɷ !=
                ƅ.Ƅ)
        {
            ɹ();
            Ĉ();
        }

        ɺ = true;
    }


    void ȥ()
    {
        if (ȁ.ǯ)
        {
            ɴ = false;
            return;
        }

        bool ɻ = false;

        for (int Į = 0;
            Į < ɼ.Count;
            Į++)
        {
            if (ɽ(
                    ɼ[Į]))
            {
                ɻ = true;
                break;
            }
        }

        if (!ɻ)
        {
            for (int Į = 0;
                Į < ɾ.Count;
                Į++)
            {
                if (ɿ(
                        ɾ[Į]))
                {
                    ɻ = true;
                    break;
                }
            }
        }

        ɴ = ɻ;
    }

    bool ɽ(
        Ǫ ʀ)
    {
        IMyShipConnector ʁ =
            ʀ.Ǩ;

        if (ʁ == null ||
            ʁ.Closed ||
            ʁ.Status !=
                MyShipConnectorStatus.Connected)
        {
            return false;
        }

        IMyShipConnector Ɨ =
            ʁ.OtherConnector;

        if (Ɨ == null)
        {
            return false;
        }

        Ǒ ʂ;

        if (!ʃ.TryGetValue(
                Ɨ.CubeGrid.EntityId,
                out ʂ))
        {
            return Ɨ.CubeGrid.IsStatic;
        }

        Ǐ ʄ =
            ʂ.ǐ;

        if (ʄ == null)
        {
            return Ɨ.CubeGrid.IsStatic;
        }

        if (ʄ.Ǡ)
        {
            return true;
        }

        if (ʄ.ǝ.Count == 0)
        {
            return false;
        }

        if (ȣ &&
            ʄ.ǡ)
        {
            return false;
        }

        return true;
    }

    bool ɿ(
        ǫ ʅ)
    {
        IMyLandingGear ʆ =
            ʅ.Ǩ;

        if (ʆ == null ||
            ʆ.Closed ||
            !ʆ.IsFunctional)
        {
            return false;
        }

        return ʆ.IsLocked;
    }

    void Ĉ()
    {
        ȱ();
        Ȳ();

        Vector3D ɉ =
            Ȥ != null
                ? Ȥ
                    .GetNaturalGravity()
                : Vector3D.Zero;

        Vector3D û =
            Me.CubeGrid.WorldAABB.Center;

        for (int Į = 0;
            Į < Ʉ.Count;
            Į++)
        {
            g İ =
                Ʉ[Į];

            long ʇ =
                İ.Q;

            if (!ʈ
                    .ContainsKey(ʇ))
            {
                ʈ.Add(
                    ʇ,
                    İ.E.Enabled);
            }

            İ.j();
            İ.E.Enabled = false;
        }

        for (int Į = 0;
            Į < ʉ.Count;
            Į++)
        {
            ʉ[Į]
                .Ĉ(
                    ɉ,
                    û);
        }

        ʊ(ʋ);
    }

    void ɸ()
    {
        ȳ();

        for (int Į = 0;
            Į < ʉ.Count;
            Į++)
        {
            ʉ[Į]
                .Þ();
        }

        ʊ(ʌ);
    }

    void ʍ()
    {
        for (int Į = 0;
            Į < Ʉ.Count;
            Į++)
        {
            g İ =
                Ʉ[Į];

            if (!ʈ
                    .ContainsKey(
                        İ.Q))
            {
                ʈ.Add(
                    İ.Q,
                    İ.E.Enabled);
            }

            İ.j();
            İ.E.Enabled = false;
        }

        Ȳ();

        for (int Į = 0;
            Į < ʉ.Count;
            Į++)
        {
            if (Math.Abs(
                    ʉ[Į]
                        .E
                        .TargetVelocityRad) >
                ď)
            {
                ʉ[Į]
                    .E
                    .TargetVelocityRad = 0;
            }
        }
    }

    void Ȯ()
    {
        for (int Į = 0;
            Į < ʉ.Count;
            Į++)
        {
            ʉ[Į]
                .Đ();
        }
    }

    void ȱ()
    {
        for (int Į = 0;
            Į < Ʉ.Count;
            Į++)
        {
            Ʉ[Į]
                .j();
        }
    }

    void ȳ()
    {
        if (ʈ.Count == 0)
        {
            return;
        }

        for (int Į = 0;
            Į < Ʉ.Count;
            Į++)
        {
            g İ =
                Ʉ[Į];

            bool ʎ;

            if (!ʈ
                    .TryGetValue(
                        İ.Q,
                        out ʎ))
            {
                continue;
            }

            İ.E.Enabled =
                ʎ;
        }

        ʈ.Clear();
    }

    void ʏ(
        long ʇ,
        IMyThrust d)
    {
        bool ʎ;

        if (!ʈ
                .TryGetValue(
                    ʇ,
                    out ʎ))
        {
            return;
        }

        if (d != null &&
            !d.Closed)
        {
            d.Enabled = ʎ;
        }

        ʈ.Remove(
            ʇ);
    }

    bool ʐ(
        long ʇ)
    {
        return ʈ
            .ContainsKey(ʇ);
    }

    void ʊ(
        List<IMyTimerBlock> ʑ)
    {
        for (int Į = 0;
            Į < ʑ.Count;
            Į++)
        {
            IMyTimerBlock ʒ =
                ʑ[Į];

            if (ʒ == null ||
                ʒ.Closed ||
                !ʒ.IsFunctional)
            {
                continue;
            }

            ʒ.Trigger();
        }
    }


    void ʖ(string ʓ)
    {
        if (string.IsNullOrWhiteSpace(ʓ))
        {
            return;
        }

        string[] ʔ =
            ʓ.ToLowerInvariant()
                .Split(
                    new[] { ';', '\n', '\r' },
                    StringSplitOptions
                        .RemoveEmptyEntries);

        for (int Į = 0;
            Į < ʔ.Length;
            Į++)
        {
            string ʕ =
                ʔ[Į].Trim();

            if (ʕ == "park")
            {
                Ȧ =
                    !Ȧ;

                ɱ = false;
            }
            else if (ʕ == "park on")
            {
                Ȧ = true;
                ɱ = false;
            }
            else if (ʕ == "park off"||
                     ʕ == "unpark")
            {
                Ȧ = false;
                ɱ = false;
            }
            else if (ʕ == "cruise")
            {
                Ⱦ = !Ⱦ;
            }
            else if (ʕ == "cruise on")
            {
                Ⱦ = true;
            }
            else if (ʕ == "cruise off")
            {
                Ⱦ = false;
            }
            else if (ʕ == "dampeners")
            {
                ɐ =
                    !ɐ;

                if (Ȥ != null)
                {
                    Ȥ
                        .DampenersOverride =
                        ɐ;
                }
            }
            else if (ʕ == "gear")
            {
                Ȍ++;

                if (Ȍ >=
                    ȁ
                        .Ǳ.Count)
                {
                    Ȍ = 0;
                }
            }
            else if (ʕ == "rescan")
            {
                ȑ();
            }
        }

        Save();
    }


    void ȡ()
    {
        HashSet<long> ʗ =
            new HashSet<long>();

        for (int Į = 0;
            Į < ʘ.Count;
            Į++)
        {
            IMyShipConnector ʁ =
                ʘ[Į];

            if (ʁ == null ||
                ʁ.Closed)
            {
                continue;
            }

            long ʙ =
                ʁ.OtherConnector != null
                    ? ʁ
                        .OtherConnector
                        .EntityId
                    : 0;

            long ʚ;

            if (!ʛ
                    .TryGetValue(
                        ʁ.EntityId,
                        out ʚ) ||
                ʚ != ʙ)
            {
                ʛ[
                    ʁ.EntityId] =
                    ʙ;

                ȑ();
            }

            ʗ.Add(
                ʁ.EntityId);
        }

        for (int Į = 0;
            Į < ɾ.Count;
            Į++)
        {
            IMyLandingGear ʜ =
                ɾ[Į].Ǩ;

            if (ʜ == null ||
                ʜ.Closed)
            {
                continue;
            }

            bool ʝ;

            if (!ʞ
                    .TryGetValue(
                        ʜ.EntityId,
                        out ʝ) ||
                ʝ != ʜ.IsLocked)
            {
                ʞ[
                    ʜ.EntityId] =
                    ʜ.IsLocked;

                ȑ();
            }
        }
    }


    void ʥ()
    {
        if (ȭ != ƅ.Ƃ ||
            Ȥ == null)
        {
            ɹ();
            return;
        }

        if (Ȟ != null &&
            Ȟ.EntityId !=
                Ȥ.EntityId)
        {
            ʟ(
                Ȟ);
        }

        Ȟ =
            Ȥ;

        StringBuilder ȓ =
            new StringBuilder();

        ȓ.Append('[')
            .Append(ʠ)
            .AppendLine("]");

        ȓ.Append("Version=")
            .AppendLine(ʡ);

        ȓ.Append("MasterProgrammableBlockId=")
            .AppendLine(Me.EntityId.ToString());

        ȓ.Append("ControllerId=")
            .AppendLine(
                Ȥ.EntityId
                    .ToString());

        ȓ.Append("Sequence=")
            .AppendLine(
                ʢ.ToString());

        ȓ.Append("Demand=")
            .AppendLine(
                ʣ(
                    Ȼ));

        ȓ.Append("Cruise=")
            .AppendLine(
                Ⱦ.ToString());

        ȓ.Append("LevelWithGravity=")
            .AppendLine(
                (Ⱦ &&
                ȁ
                    .ǰ)
                .ToString());

        Ȥ.CustomData =
            ʤ(
                Ȥ.CustomData,
                ʠ,
                ȓ.ToString());
    }

    void ɹ()
    {
        if (Ȟ == null)
        {
            return;
        }

        ʟ(
            Ȟ);

        Ȟ = null;
    }

    void ʟ(
        IMyShipController ɯ)
    {
        if (ɯ == null ||
            ɯ.Closed)
        {
            return;
        }

        string ʦ;

        if (!ʧ(
                ɯ.CustomData,
                ʠ,
"MasterProgrammableBlockId",
                out ʦ))
        {
            return;
        }

        long ʨ;

        if (!long.TryParse(
                ʦ,
                out ʨ) ||
            ʨ != Me.EntityId)
        {
            return;
        }

        ɯ.CustomData =
            ʩ(
                ɯ.CustomData,
                ʠ);
    }

    void ȧ()
    {
        for (int Į = 0;
            Į <
                ʪ.Count;
            Į++)
        {
            Ʈ ʕ;

            if (!ʫ(
                    ʪ[Į],
                    out ʕ))
            {
                continue;
            }

            ʬ(ʕ);
            return;
        }
    }

    void ʭ()
    {
        for (int Į = 0;
            Į <
                ʪ.Count;
            Į++)
        {
            IMyShipController ɯ =
                ʪ[Į];

            if (ȷ
                    .ƥ != 0 &&
                ɯ.EntityId !=
                    ȷ
                        .ƥ)
            {
                continue;
            }

            Ʈ ʕ;

            if (!ʫ(
                    ɯ,
                    out ʕ))
            {
                continue;
            }

            if (ȷ
                    .Ƥ != 0 &&
                ʕ
                    .Ƥ !=
                ȷ
                    .Ƥ)
            {
                continue;
            }

            ʬ(ʕ);
            return;
        }
    }

    void ʬ(
        Ʈ ʕ)
    {
        if (ʕ.Ʀ !=
                Ȫ ||
            ʕ
                .Ƥ !=
            ʮ)
        {
            Ȫ =
                ʕ.Ʀ;

            ʮ =
                ʕ
                    .Ƥ;

            ȩ = 0;
            Ȩ =
                true;
            ȫ = true;
        }

        ȷ.ƙ(
            ʕ);
    }

    bool ʫ(
        IMyShipController ɯ,
        out Ʈ ʕ)
    {
        ʕ = null;

        if (ɯ == null ||
            ɯ.Closed)
        {
            return false;
        }

        string ʯ;
        string ʰ;
        string ʱ;
        string ʲ;
        string ʳ;
        string ʴ;

        if (!ʧ(
                ɯ.CustomData,
                ʠ,
"MasterProgrammableBlockId",
                out ʯ) ||
            !ʧ(
                ɯ.CustomData,
                ʠ,
"ControllerId",
                out ʰ) ||
            !ʧ(
                ɯ.CustomData,
                ʠ,
"Sequence",
                out ʱ) ||
            !ʧ(
                ɯ.CustomData,
                ʠ,
"Demand",
                out ʲ))
        {
            return false;
        }

        long ʦ;
        long ʵ;
        long ʶ;
        Vector3D ʷ;

        if (!long.TryParse(
                ʯ,
                out ʦ) ||
            !long.TryParse(
                ʰ,
                out ʵ) ||
            !long.TryParse(
                ʱ,
                out ʶ) ||
            !ʸ(
                ʲ,
                out ʷ))
        {
            return false;
        }

        ʧ(
            ɯ.CustomData,
            ʠ,
"Cruise",
            out ʳ);

        ʧ(
            ɯ.CustomData,
            ʠ,
"LevelWithGravity",
            out ʴ);

        bool ʹ;
        bool ʺ;

        bool.TryParse(
            ʳ,
            out ʹ);

        bool.TryParse(
            ʴ,
            out ʺ);

        ʕ = new Ʈ
        {
            Ƥ =
                ʦ,
            ƥ = ʵ,
            Ʀ = ʶ,
            Ƨ =
                n.Ŭ(
                    ʷ,
                    1),
            Ɖ = ʹ,
            Ʃ =
                ʺ
        };

        return true;
    }


    static int ˆ(
        string ǽ,
        string ʻ)
    {
        if (string.IsNullOrEmpty(ǽ))
        {
            return -1;
        }

        string ʼ =
"["+ ʻ + "]";

        int ʽ = 0;

        while (ʽ <
               ǽ.Length)
        {
            int ʾ = ǽ.IndexOf(
                ʼ,
                ʽ,
                StringComparison.OrdinalIgnoreCase);

            if (ʾ < 0)
            {
                return -1;
            }

            bool ʿ =
                ʾ == 0 ||
                ǽ[ʾ - 1] == '\n';

            int ˀ = ʾ + ʼ.Length;

            bool ˁ =
                ˀ >= ǽ.Length ||
                ǽ[ˀ] == '\r' ||
                ǽ[ˀ] == '\n';

            if (ʿ && ˁ)
            {
                return ʾ;
            }

            ʽ = ʾ + 1;
        }

        return -1;
    }

    static int ˊ(
        string ǽ,
        int ʽ)
    {
        while (ʽ <
               ǽ.Length)
        {
            int ˇ =
                ǽ.IndexOf(
                    '\n',
                    ʽ);

            if (ˇ < 0 ||
                ˇ + 1 >=
                    ǽ.Length)
            {
                return ǽ.Length;
            }

            ˇ++;

            int ˈ = ˇ;

            while (ˈ <
                       ǽ.Length &&
                   (ǽ[ˈ] == ' ' ||
                    ǽ[ˈ] == '\t' ||
                    ǽ[ˈ] == '\r'))
            {
                ˈ++;
            }

            if (ˈ <
                    ǽ.Length &&
                ǽ[ˈ] == '[')
            {
                int ˉ =
                    ǽ.IndexOf(
                        ']',
                        ˈ + 1);

                if (ˉ >= 0)
                {
                    return ˇ;
                }
            }

            ʽ = ˇ;
        }

        return ǽ.Length;
    }

    static bool ʧ(
        string ǽ,
        string ʻ,
        string Ȕ,
        out string ğ)
    {
        ğ = null;

        int ˋ =
            ˆ(
                ǽ,
                ʻ);

        if (ˋ < 0)
        {
            return false;
        }

        int ˌ =
            ˊ(
                ǽ,
                ˋ +
                ʻ.Length + 2);

        int ˍ =
            ǽ.IndexOf(
                '\n',
                ˋ);

        if (ˍ < 0 ||
            ˍ >= ˌ)
        {
            return false;
        }

        string ȓ =
            ǽ.Substring(
                ˍ + 1,
                ˌ - ˍ - 1);

        string[] ˎ =
            ȓ.Replace(
"\r",
                    string.Empty)
                .Split('\n');

        for (int Į = 0;
            Į < ˎ.Length;
            Į++)
        {
            string ˏ = ˎ[Į];

            int ː =
                ˏ.IndexOf('=');

            if (ː <= 0)
            {
                continue;
            }

            string ˑ =
                ˏ.Substring(
                        0,
                        ː)
                    .Trim();

            if (!ˑ.Equals(
                    Ȕ,
                    StringComparison
                        .OrdinalIgnoreCase))
            {
                continue;
            }

            ğ =
                ˏ.Substring(
                        ː + 1)
                    .Trim();

            return true;
        }

        return false;
    }

    static string ʤ(
        string ǽ,
        string ʻ,
        string ˠ)
    {
        ǽ =
            ǽ ?? string.Empty;

        ˠ =
            ˠ.TrimEnd(
                '\r',
                '\n') +
"\n";

        int ˋ =
            ˆ(
                ǽ,
                ʻ);

        if (ˋ < 0)
        {
            if (ǽ.Length == 0)
            {
                return ˠ;
            }

            string ː =
                ǽ.EndsWith("\n")
                    ? string.Empty
                    : "\n";

            return ǽ +
                   ː +
                   ˠ;
        }

        int ˌ =
            ˊ(
                ǽ,
                ˋ +
                ʻ.Length + 2);

        return ǽ.Substring(0, ˋ) +
               ˠ +
               ǽ.Substring(ˌ);
    }

    static string ʩ(
        string ǽ,
        string ʻ)
    {
        if (string.IsNullOrEmpty(ǽ))
        {
            return ǽ;
        }

        int ˋ =
            ˆ(
                ǽ,
                ʻ);

        if (ˋ < 0)
        {
            return ǽ;
        }

        int ˌ =
            ˊ(
                ǽ,
                ˋ +
                ʻ.Length + 2);

        string ˡ =
            ǽ.Substring(0, ˋ);

        string ˀ =
            ǽ.Substring(ˌ);

        if (ˡ.EndsWith("\n") &&
            ˀ.StartsWith("\n"))
        {
            ˀ = ˀ.Substring(1);
        }

        return ˡ + ˀ;
    }

    static string ʣ(
        Vector3D ƺ)
    {
        return ƺ.X.ToString("R") +
";"+
            ƺ.Y.ToString("R") +
";"+
            ƺ.Z.ToString("R");
    }

    static bool ʸ(
        string ˢ,
        out Vector3D ƺ)
    {
        ƺ = Vector3D.Zero;

        if (string.IsNullOrWhiteSpace(
                ˢ))
        {
            return false;
        }

        string[] ˣ =
            ˢ.Split(';');

        if (ˣ.Length != 3)
        {
            return false;
        }

        double ˤ;
        double ˬ;
        double ˮ;

        if (!double.TryParse(
                ˣ[0],
                out ˤ) ||
            !double.TryParse(
                ˣ[1],
                out ˬ) ||
            !double.TryParse(
                ˣ[2],
                out ˮ))
        {
            return false;
        }

        ƺ = new Vector3D(ˤ, ˬ, ˮ);
        return true;
    }


    void ȯ(bool Ő)
    {
        Ͱ.Clear();

        Ͱ
            .AppendLine(Ȁ)
            .Append("v")
            .AppendLine(ʡ)
            .AppendLine();

        Ͱ
            .Append("Mode: ")
            .AppendLine(ȭ.ToString());

        Ͱ
            .Append("Controller: ")
            .AppendLine(
                Ȥ != null
                    ? Ȥ
                        .CustomName
                    : "NONE");

        Ͱ
            .Append("Dampeners: ")
            .AppendLine(
                ɐ
                    ? "ON"                    : "OFF");

        Ͱ
            .Append("Cruise: ")
            .AppendLine(
                Ⱦ
                    ? "ON"                    : "OFF");

        Ͱ
            .Append("Gear: ")
            .Append(Ȍ + 1)
            .Append("/")
            .Append(ȁ
                .Ǳ.Count)
            .Append(" (")
            .Append(
                (ȁ
                    .Ǳ[
                        MathHelper.Clamp(
                            Ȍ,
                            0,
                            ȁ
                                .Ǳ
                                .Count - 1)] *
                 100)
                .ToString("0.##"))
            .AppendLine("%)");

        Ͱ
            .Append("Nacelles: ")
            .AppendLine(
                Ƀ.Count
                    .ToString());

        Ͱ
            .Append("Controlled thrust: ")
            .Append(
                (ȸ /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        Ͱ
            .Append("Residual: ")
            .Append(
                (ɘ.Length() /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        Ͱ
            .Append("Gyros: ")
            .AppendLine(
                ɢ.Count
                    .ToString());

        if (ȭ == ƅ.ƃ)
        {
            Ͱ
                .Append("Heartbeat age: ")
                .Append(
                    ȩ)
                .AppendLine("/2");
        }

        Ͱ
            .Append("Runtime: ")
            .Append(
                Runtime.LastRunTimeMs
                    .ToString("0.###"))
            .Append(" ms | avg ")
            .Append(
                ͱ
                    .Ƴ
                    .ToString("0.###"))
            .Append(" | max ")
            .AppendLine(
                ͱ
                    .ƴ
                    .ToString("0.###"));

        Ͱ
            .Append("Instructions: ")
            .Append(Runtime
                .CurrentInstructionCount)
            .Append("/")
            .AppendLine(Runtime
                .MaxInstructionCount
                .ToString());

        Echo(Ͱ.ToString());

        if (!Ő &&
            Ͳ.Count == 0)
        {
            return;
        }

        ͳ.Clear();

        ͳ
            .AppendLine("VECTOR THRUST REDUX")
            .Append("MODE  ")
            .AppendLine(
                ȭ.ToString()
                    .ToUpperInvariant())
            .Append("DAMP  ")
            .AppendLine(
                ɐ
                    ? "ON"                    : "OFF")
            .Append("CRUISE ")
            .AppendLine(
                Ⱦ
                    ? "ON"                    : "OFF")
            .Append("GEAR  ")
            .Append(Ȍ + 1)
            .Append("/")
            .AppendLine(
                ȁ
                    .Ǳ.Count
                    .ToString())
            .Append("VECTORS ")
            .AppendLine(
                Ƀ.Count
                    .ToString())
            .Append("THRUST ")
            .Append(
                (ȸ /
                 1000.0)
                .ToString("0.0"))
            .AppendLine(" kN")
            .Append("ERROR ")
            .Append(
                (ɘ.Length() /
                 1000.0)
                .ToString("0.0"))
            .AppendLine(" kN");

        for (int Į = 0;
            Į < Ͳ.Count;
            Į++)
        {
            Ͳ[Į]
                .ſ(
                    ͳ.ToString());
        }
    }

    const string Ȁ = "Vector Thrust Redux",ʡ = "0.2.0",Ȋ = "Vector Thrust Redux",ʠ = "Vector Thrust Redux Heartbeat",ʹ = "VT-Redux:",Ͷ = "State",ͷ = "Disabled Thrusters",ͺ = "Park Rotor Targets",ͻ = "Topology";


    const double p = 1e-8,U = 1e-3,ē = 1e-4,Ð = 1e-4,Ķ = 1.0 - 1e-6,ͼ = 1.0 - 1e-4,þ = 1.0 - 1e-4,å = 0.1,č = 1.0,æ = 4.0,Ğ = Math.PI,ď = 1e-3,ͽ = 0.01,µ = 0.0075,ɩ = 4.0,ɬ = 1.5,ŭ = 30.0,Ų = 1e-3,ŧ = 448000.0,Ũ = 33600000.0,ť = 4480000.0,Ŧ = 201600000.0,è = 1.0 / 120.0,Ά = 0.25;


    readonly Ǻ ȁ = new Ǻ();
    readonly MyIni Έ = new MyIni();

    string Ǿ = string.Empty,Ή = string.Empty;

    bool Ⱦ,ɴ,ɺ;
    bool ɐ = true,Ȧ,Ί,Ό,ɰ = true,ȣ,ȫ,ɱ,ɲ,Ȩ,Ύ,Ώ = true,ΐ;
    int Ȍ,ȩ,Α,Β;

    double Γ,ȸ;

    readonly Dictionary<long, Ə> Δ =
        new Dictionary<long, Ə>();

    readonly Dictionary<long, double> ü =
        new Dictionary<long, double>();

    Ɯ Ε,Ζ;

    readonly Dictionary<long, bool> ʈ =
        new Dictionary<long, bool>();


    ƅ ȭ = ƅ.ƀ;

    IMyShipController Ȥ;

    long ʢ;
    long Ȫ = long.MinValue,ʮ;

    Ʈ ȷ = new Ʈ();

    Vector3D ȼ;
    Vector3D ɘ,Ȼ,Ɂ;
    double Ü,Η;

    int Θ;

    bool Ι;


    readonly Ɩ Κ =
        new Ɩ(),Λ =
        new Ɩ(),Μ =
        new Ɩ();


    readonly List<IMyShipController> ɮ =
        new List<IMyShipController>(),ʪ =
        new List<IMyShipController>();

    readonly List<g> Ν =
        new List<g>(),Ʉ =
        new List<g>(),ə =
        new List<g>(),ɗ =
        new List<g>(),Ξ =
        new List<g>(),Ο =
        new List<g>(),Π =
        new List<g>(),Ρ =
        new List<g>();

    readonly List<Ò> ʉ =
        new List<Ò>();

    readonly List<H> Ƀ =
        new List<H>();

    readonly List<ŝ> ɛ =
        new List<ŝ>();

    readonly List<ũ> ɢ =
        new List<ũ>();

    readonly List<Ǫ> ɼ =
        new List<Ǫ>();

    readonly List<ǫ> ɾ =
        new List<ǫ>();

    readonly List<IMyTimerBlock> ʋ =
        new List<IMyTimerBlock>(),ʌ =
        new List<IMyTimerBlock>();

    readonly List<Ž> Ͳ =
        new List<Ž>();

    readonly Dictionary<long, Ǒ> ʃ =
        new Dictionary<long, Ǒ>();

    readonly StringBuilder Ͱ =
        new StringBuilder(),ͳ =
        new StringBuilder();

    IEnumerator<int> Σ;

    ƶ ͱ;

    public Program()
    {
        ͱ =
            new ƶ(this);

        Τ();
        Ȓ(true);

        Runtime.UpdateFrequency =
            UpdateFrequency.Update1 |
            UpdateFrequency.Update10 |
            UpdateFrequency.Update100;

        ȑ();
    }

    public void Save()
    {
        Έ.Clear();

        Έ.Set(
            Ͷ,
"Cruise",
            Ⱦ);

        Έ.Set(
            Ͷ,
"Dampeners",
            ɐ);

        Έ.Set(
            Ͷ,
"ManualPark",
            Ȧ);

        Έ.Set(
            Ͷ,
"Gear",
            Ȍ);

        Έ.Set(
            Ͷ,
"CruiseTargetSpeed",
            Γ);

        Έ.Set(
            Ͷ,
"CruiseTargetInitialized",
            Ί);

        foreach (
            KeyValuePair<long, Ə> Υ
            in Δ)
        {
            Ə Φ =
                Υ.Value;

            string ˢ =
                (Φ.ƍ
                    ? "1"                    : "0") +
";"+
                ((int)Φ.Ǝ).ToString();

            Έ.Set(
                ͷ,
                Υ.Key.ToString(),
                ˢ);
        }

        foreach (
            KeyValuePair<long, double> Υ
            in ü)
        {
            Έ.Set(
                ͺ,
                Υ.Key.ToString(),
                Υ.Value);
        }

        if (ΐ)
        {
            Έ.Set(
                ͻ,
"Count",
                Ζ.Ɲ);

            Έ.Set(
                ͻ,
"Xor",
                Ζ.ƞ.ToString());

            Έ.Set(
                ͻ,
"Sum",
                Ζ.Ɵ.ToString());
        }
        else if (Ό)
        {
            Έ.Set(
                ͻ,
"Count",
                Ε.Ɲ);

            Έ.Set(
                ͻ,
"Xor",
                Ε.ƞ.ToString());

            Έ.Set(
                ͻ,
"Sum",
                Ε.Ɵ.ToString());
        }

        Storage =
            Έ.ToString();
    }

    public void Main(
        string ʓ,
        UpdateType Χ)
    {
        ͱ.Ʒ();

        double Ψ =
            Runtime.TimeSinceLastRun.TotalSeconds;

        if (Ψ <
            è)
        {
            Ψ =
                è;
        }
        else if (Ψ >
                 Ά)
        {
            Ψ =
                Ά;
        }

        Η +=
            Ψ;

        bool Ω =
            (Χ &
             (UpdateType.Terminal |
              UpdateType.Trigger |
              UpdateType.Script)) != 0 ||
            !string.IsNullOrWhiteSpace(
                ʓ);

        Ϊ();

        if (Ω)
        {
            Ƞ();
            ʖ(ʓ);
        }

        if ((Χ &
             UpdateType.Update100) != 0 &&
            Ϋ(
                ref Β,
                ȁ.ǹ))
        {
            ȟ();
        }

        if ((Χ &
             UpdateType.Update10) != 0 &&
            Ϋ(
                ref Α,
                ȁ.Ǹ))
        {
            Ȱ();
        }

        if ((Χ &
             UpdateType.Update1) != 0)
        {
            ʢ++;

            if (ȭ ==
                ƅ.ƃ)
            {
                ʭ();
            }

            Ȭ();

            if (Ϋ(
                    ref Θ,
                    ȁ.Ƿ))
            {
                Ü =
                    MathHelper.Clamp(
                        Η,
                        è,
                        Ά);

                Η = 0;

                ɂ(
                    Ü);
            }

            if (ȁ.ǭ ||
                Ȟ != null)
            {
                ʥ();
            }
        }

        if (Ω)
        {
            Ȭ();

            if (ȁ.ǭ ||
                Ȟ != null)
            {
                ʥ();
            }

            ɺ = true;
        }

        if (ɺ)
        {
            ȯ(true);
            ɺ = false;
        }

        ͱ.ƹ();
    }

    static bool Ϋ(
        ref int ά,
        int έ)
    {
        if (ά <
            έ)
        {
            ά++;
            return false;
        }

        ά = 0;
        return true;
    }

    void Τ()
    {
        if (string.IsNullOrWhiteSpace(
                Storage))
        {
            return;
        }

        MyIniParseResult ǿ;

        if (!Έ.TryParse(
                Storage,
                out ǿ))
        {
            return;
        }

        Ⱦ =
            Έ
                .Get(
                    Ͷ,
"Cruise")
                .ToBoolean(false);

        ɐ =
            Έ
                .Get(
                    Ͷ,
"Dampeners")
                .ToBoolean(true);

        Ȧ =
            Έ
                .Get(
                    Ͷ,
"ManualPark")
                .ToBoolean(false);

        Ȍ =
            Math.Max(
                0,
                Έ
                    .Get(
                        Ͷ,
"Gear")
                    .ToInt32(0));

        Γ =
            Έ
                .Get(
                    Ͷ,
"CruiseTargetSpeed")
                .ToDouble(0);

        Ί =
            Έ
                .Get(
                    Ͷ,
"CruiseTargetInitialized")
                .ToBoolean(false);

        ή();
        ί();
        ΰ();
    }

    void ή()
    {
        List<MyIniKey> α =
            new List<MyIniKey>();

        Έ.GetKeys(
            ͷ,
            α);

        for (int Į = 0;
            Į < α.Count;
            Į++)
        {
            MyIniKey Ȕ =
                α[Į];

            long ʇ;

            if (!long.TryParse(
                    Ȕ.Name,
                    out ʇ))
            {
                continue;
            }

            string ˢ =
                Έ
                    .Get(Ȕ)
                    .ToString();

            string[] ˣ =
                ˢ.Split(';');

            if (ˣ.Length != 2)
            {
                continue;
            }

            bool β =
                ˣ[0] == "1";

            int γ;

            if (!int.TryParse(
                    ˣ[1],
                    out γ))
            {
                continue;
            }

            ƌ δ =
                (ƌ)
                    γ;

            if (δ ==
                ƌ.M)
            {
                continue;
            }

            Δ[
                ʇ] =
                new Ə
                {
                    ƍ =
                        β,
                    Ǝ = δ
                };

            if ((δ &
                 ƌ.Ɗ) != 0)
            {
                ʈ[
                    ʇ] =
                    β;
            }
        }
    }

    void ί()
    {
        List<MyIniKey> α =
            new List<MyIniKey>();

        Έ.GetKeys(
            ͺ,
            α);

        for (int Į = 0;
            Į < α.Count;
            Į++)
        {
            MyIniKey Ȕ =
                α[Į];

            long ʇ;

            if (!long.TryParse(
                    Ȕ.Name,
                    out ʇ))
            {
                continue;
            }

            double ʄ =
                Έ
                    .Get(Ȕ)
                    .ToDouble(
                        double.NaN);

            if (double.IsNaN(ʄ) ||
                double.IsInfinity(ʄ))
            {
                continue;
            }

            ü[
                ʇ] =
                ʄ;
        }
    }

    void ΰ()
    {
        long ε =
            Έ
                .Get(
                    ͻ,
"Count")
                .ToInt64(-1);

        string ζ =
            Έ
                .Get(
                    ͻ,
"Xor")
                .ToString();

        string η =
            Έ
                .Get(
                    ͻ,
"Sum")
                .ToString();

        ulong θ;
        ulong ι;

        if (ε < 0 ||
            !ulong.TryParse(
                ζ,
                out θ) ||
            !ulong.TryParse(
                η,
                out ι))
        {
            return;
        }

        Ε =
            new Ɯ
            {
                Ɲ = ε,
                ƞ = θ,
                Ɵ = ι
            };

        Ό =
            true;
    }
    sealed class ϖ
    {
        public readonly List<IMyTerminalBlock> κ =
            new List<IMyTerminalBlock>();

        public readonly List<IMyShipController> ǝ =
            new List<IMyShipController>(),λ =
            new List<IMyShipController>(),μ =
            new List<IMyShipController>();

        public readonly List<IMyThrust> ν =
            new List<IMyThrust>();

        public readonly List<IMyMotorStator> ξ =
            new List<IMyMotorStator>();

        public readonly List<IMyPistonBase> ο =
            new List<IMyPistonBase>();

        public readonly List<IMyGyro> π =
            new List<IMyGyro>();

        public readonly List<IMyShipConnector> ρ =
            new List<IMyShipConnector>(),ς =
            new List<IMyShipConnector>();

        public readonly List<IMyLandingGear> σ =
            new List<IMyLandingGear>();

        public readonly List<IMyTimerBlock> τ =
            new List<IMyTimerBlock>(),υ =
            new List<IMyTimerBlock>(),φ =
            new List<IMyTimerBlock>();

        public readonly List<IMyProgrammableBlock> χ =
            new List<IMyProgrammableBlock>();

        public readonly Dictionary<long, F> G =
            new Dictionary<long, F>();

        public readonly Dictionary<long, Ǒ> ψ =
            new Dictionary<long, Ǒ>();

        public readonly List<Ǐ> ω =
            new List<Ǐ>();

        public readonly List<ǧ> ϊ =
            new List<ǧ>();

        public readonly List<g> ĥ =
            new List<g>(),ϋ =
            new List<g>(),ό =
            new List<g>(),ύ =
            new List<g>();

        public readonly List<Ò> ώ =
            new List<Ò>();

        public readonly List<H> Ϗ =
            new List<H>();

        public readonly List<ŝ> ϐ =
            new List<ŝ>();

        public readonly List<ũ> ϑ =
            new List<ũ>();

        public readonly List<Ǫ> ϒ =
            new List<Ǫ>();

        public readonly List<ǫ> ϓ =
            new List<ǫ>();

        public readonly List<Ž> ϔ =
            new List<Ž>();

        public Ǐ ϕ;
    }

    readonly List<ǧ> ϗ =
        new List<ǧ>();

    readonly List<IMyShipConnector> ʘ =
        new List<IMyShipConnector>();

    readonly Dictionary<long, long> ʛ =
        new Dictionary<long, long>();

    readonly Dictionary<long, bool> ʞ =
        new Dictionary<long, bool>();

    void ȑ()
    {
        Ώ = true;
    }

    void Ϊ()
    {
        if (Σ == null)
        {
            if (!Ώ)
            {
                return;
            }

            Ώ = false;
            Σ = Ϙ().GetEnumerator();
        }

        int ϙ = Runtime.MaxInstructionCount;
        int Ϛ =
            Math.Max(1000, ϙ * 3 / 4);

        int ϛ = 0;

        while (Σ != null &&
               Runtime.CurrentInstructionCount <
                   Ϛ &&
               ϛ < 512)
        {
            ϛ++;

            if (Σ.MoveNext())
            {
                continue;
            }

            Σ.Dispose();
            Σ = null;

            if (Ώ)
            {
                Ώ = false;
                Σ =
                    Ϙ().GetEnumerator();
            }
        }
    }

    IEnumerable<int> Ϙ()
    {
        ϖ Ϝ = new ϖ();


        GridTerminalSystem.GetBlocks(Ϝ.κ);

        List<IMyBlockGroup> ϝ =
            new List<IMyBlockGroup>();

        GridTerminalSystem.GetBlockGroups(ϝ);

        List<IMyTerminalBlock> Ϟ =
            new List<IMyTerminalBlock>();

        for (int Į = 0; Į < ϝ.Count; Į++)
        {
            IMyBlockGroup ɟ = ϝ[Į];

            F Ϡ =
                ϟ(ɟ.Name);

            if (Ϡ == F.M)
            {
                continue;
            }

            Ϟ.Clear();
            ɟ.GetBlocks(Ϟ);

            for (int ĵ = 0;
                ĵ < Ϟ.Count;
                ĵ++)
            {
                ϡ(
                    Ϝ.G,
                    Ϟ[ĵ].EntityId,
                    Ϡ);
            }

            yield return 1;
        }

        for (int Į = 0;
            Į < Ϝ.κ.Count;
            Į++)
        {
            IMyTerminalBlock d =
                Ϝ.κ[Į];

            F Ϣ =
                ϟ(d.CustomName) |
                ϟ(d.CustomData);

            ϡ(
                Ϝ.G,
                d.EntityId,
                Ϣ);

            ϣ(
                Ϝ.ψ,
                d.CubeGrid);

            IMyShipController ɯ =
                d as IMyShipController;

            if (ɯ != null)
            {
                Ϝ.ǝ.Add(ɯ);
            }

            IMyThrust T =
                d as IMyThrust;

            if (T != null)
            {
                Ϝ.ν.Add(T);
            }

            IMyMotorStator Ī =
                d as IMyMotorStator;

            if (Ī != null)
            {
                Ϝ.ξ.Add(Ī);
            }

            IMyPistonBase Ϥ =
                d as IMyPistonBase;

            if (Ϥ != null)
            {
                Ϝ.ο.Add(Ϥ);
            }

            IMyGyro ϥ =
                d as IMyGyro;

            if (ϥ != null)
            {
                Ϝ.π.Add(ϥ);
            }

            IMyShipConnector ʁ =
                d as IMyShipConnector;

            if (ʁ != null)
            {
                Ϝ.ρ.Add(ʁ);
            }

            IMyLandingGear ʆ =
                d as IMyLandingGear;

            if (ʆ != null)
            {
                Ϝ.σ.Add(
                    ʆ);
            }

            IMyTimerBlock ʒ =
                d as IMyTimerBlock;

            if (ʒ != null)
            {
                Ϝ.τ.Add(ʒ);
            }

            IMyProgrammableBlock Ϧ =
                d as IMyProgrammableBlock;

            if (Ϧ != null)
            {
                Ϝ.χ.Add(
                    Ϧ);
            }

            yield return 1;
        }

        Ǒ ϧ =
            ϣ(
                Ϝ.ψ,
                Me.CubeGrid);


        for (int Į = 0;
            Į < Ϝ.ξ.Count;
            Į++)
        {
            IMyMotorStator Ī =
                Ϝ.ξ[Į];

            if (Ī.TopGrid == null)
            {
                continue;
            }

            Ϩ(
                Ϝ.ψ,
                Ī.CubeGrid,
                Ī.TopGrid,
                Ī);

            yield return 1;
        }

        for (int Į = 0;
            Į < Ϝ.ο.Count;
            Į++)
        {
            IMyPistonBase Ϥ =
                Ϝ.ο[Į];

            if (Ϥ.TopGrid == null)
            {
                continue;
            }

            Ϩ(
                Ϝ.ψ,
                Ϥ.CubeGrid,
                Ϥ.TopGrid,
                Ϥ);

            yield return 1;
        }

        for (int Į = 0;
            Į < Ϝ.ρ.Count;
            Į++)
        {
            IMyShipConnector Ɨ =
                Ϝ.ρ[Į]
                    .OtherConnector;

            if (Ɨ != null)
            {
                ϣ(
                    Ϝ.ψ,
                    Ɨ.CubeGrid);
            }

            yield return 1;
        }

        ϩ(Ϝ);


        for (int Į = 0;
            Į < Ϝ.ǝ.Count;
            Į++)
        {
            IMyShipController ɯ =
                Ϝ.ǝ[Į];

            Ǒ ǚ;

            if (!Ϝ.ψ.TryGetValue(
                    ɯ.CubeGrid.EntityId,
                    out ǚ))
            {
                continue;
            }

            ǚ.ǐ.ǝ.Add(ɯ);

            if (ɯ.CubeGrid == Me.CubeGrid)
            {
                Ϝ.λ.Add(
                    ɯ);
            }

            yield return 1;
        }

        for (int Į = 0;
            Į < Ϝ.χ.Count;
            Į++)
        {
            IMyProgrammableBlock Ϧ =
                Ϝ.χ[Į];

            if (!Ϫ(
                    Ϧ))
            {
                continue;
            }

            Ǒ ǚ;

            if (!Ϝ.ψ.TryGetValue(
                    Ϧ
                        .CubeGrid.EntityId,
                    out ǚ))
            {
                continue;
            }

            ǚ.ǐ
                .Ǟ
                .Add(Ϧ);

            if (ϫ(
                    Ϧ))
            {
                ǚ.ǐ
                    .ǡ = true;
            }

            yield return 1;
        }


        HashSet<long> Ϭ =
            new HashSet<long>();

        for (int Į = 0;
            Į < Ϝ.ρ.Count;
            Į++)
        {
            IMyShipConnector ʁ =
                Ϝ.ρ[Į];

            IMyShipConnector Ɨ =
                ʁ.OtherConnector;

            if (Ɨ == null)
            {
                continue;
            }

            long ϭ = Math.Min(
                ʁ.EntityId,
                Ɨ.EntityId);

            long Ϯ = Math.Max(
                ʁ.EntityId,
                Ɨ.EntityId);

            long ϯ =
                unchecked(ϭ * 397L ^ Ϯ);

            if (!Ϭ.Add(ϯ))
            {
                continue;
            }

            Ǒ ϰ;
            Ǒ ϱ;

            if (!Ϝ.ψ.TryGetValue(
                    ʁ.CubeGrid.EntityId,
                    out ϰ) ||
                !Ϝ.ψ.TryGetValue(
                    Ɨ.CubeGrid.EntityId,
                    out ϱ))
            {
                continue;
            }

            Ϝ.ϊ.Add(
                new ǧ
                {
                    ǖ = ʁ,
                    Ǘ = Ɨ,
                    ǥ = ϰ,
                    Ǧ = ϱ
                });

            yield return 1;
        }

        Ϝ.ϕ =
            ϧ.ǐ;

        ϲ(Ϝ);
        ϳ(
            Ϝ,
            ϧ);


        for (int Į = 0;
            Į < Ϝ.ω.Count;
            Į++)
        {
            Ǐ ϴ =
                Ϝ.ω[Į];

            if (!ϴ
                    .ǟ ||
                ϴ.Ǖ ||
                ϴ
                    .Ǟ.Count == 0)
            {
                continue;
            }

            for (int ĵ = 0;
                ĵ < ϴ.ǝ.Count;
                ĵ++)
            {
                Ϝ.μ.Add(
                    ϴ.ǝ[ĵ]);
            }

            yield return 1;
        }


        Dictionary<long, Ò> ϵ =
            new Dictionary<long, Ò>();

        for (int Į = 0;
            Į < Ϝ.ξ.Count;
            Į++)
        {
            IMyMotorStator d =
                Ϝ.ξ[Į];

            Ǒ ǚ;

            if (!Ϝ.ψ.TryGetValue(
                    d.CubeGrid.EntityId,
                    out ǚ) ||
                !ǚ.Ǖ)
            {
                continue;
            }

            F e =
                Ϸ(
                    Ϝ.G,
                    d.EntityId);

            bool f =
                ϸ(e);

            Ò Ī = new Ò(
                d,
                this,
                e,
                f);

            ϵ.Add(
                d.EntityId,
                Ī);

            if (f)
            {
                Ϝ.ώ.Add(
                    Ī);
            }

            yield return 1;
        }

        Dictionary<long, H> Ϲ =
            new Dictionary<long, H>();


        for (int Į = 0;
            Į < Ϝ.ν.Count;
            Į++)
        {
            IMyThrust d =
                Ϝ.ν[Į];

            Ǒ ǚ;

            if (!Ϝ.ψ.TryGetValue(
                    d.CubeGrid.EntityId,
                    out ǚ))
            {
                continue;
            }

            bool Ϻ =
                ǚ.Ǖ ||
                ǚ.ǐ
                    .ǟ;

            if (!Ϻ)
            {
                continue;
            }

            F e =
                Ϸ(
                    Ϝ.G,
                    d.EntityId);

            Ò ϼ =
                ϻ(
                    ǚ,
                    ϵ);

            bool Ͻ =
                ǚ.Ǖ &&
                ǚ.ǔ > 0;

            bool f =
                Ͼ(
                    e,
                    ǚ.Ǖ,
                    Ͻ,
                    ϼ);

            g İ = new g(
                d,
                this,
                e,
                f);

            Ϝ.ĥ.Add(İ);

            if (!f)
            {
                Ϝ
                    .ύ
                    .Add(İ);

                yield return 1;
                continue;
            }

            Ϝ.ϋ.Add(
                İ);

            if (ϼ == null ||
                !ϼ.P)
            {
                Ϝ
                    .ό
                    .Add(İ);

                yield return 1;
                continue;
            }

            H ɡ;

            if (!Ϲ.TryGetValue(
                    ϼ.Q,
                    out ɡ))
            {
                ɡ = new H(
                    ϼ,
                    this);

                Ϲ.Add(
                    ϼ.Q,
                    ɡ);

                Ϝ.Ϗ.Add(
                    ɡ);
            }

            İ.I = ɡ;
            ɡ.ĥ.Add(İ);

            Ͽ(
                ɡ,
                ǚ,
                ϼ);

            yield return 1;
        }

        for (int Į = 0;
            Į < Ϝ.ώ.Count;
            Į++)
        {
            Ò Ī =
                Ϝ.ώ[Į];

            if (Ī.I == null &&
                Math.Abs(
                    Ī.E
                        .TargetVelocityRad) >
                    ď)
            {
                Ī.E
                    .TargetVelocityRad = 0;
            }

            yield return 1;
        }

        for (int Į = 0;
            Į < Ϝ.Ϗ.Count;
            Į++)
        {
            Ϝ.Ϗ[Į]
                .ĸ();

            Ѐ(
                Ϝ.ϐ,
                Ϝ.Ϗ[Į]);

            yield return 1;
        }


        for (int Į = 0;
            Į < Ϝ.π.Count;
            Į++)
        {
            IMyGyro d =
                Ϝ.π[Į];

            Ǒ ǚ;

            if (!Ϝ.ψ.TryGetValue(
                    d.CubeGrid.EntityId,
                    out ǚ) ||
                !ǚ.Ǖ ||
                !Ё(d))
            {
                continue;
            }

            F e =
                Ϸ(
                    Ϝ.G,
                    d.EntityId);

            bool f =
                Ђ(
                    e,
                    ǚ.ǔ > 0);

            if (!f)
            {
                continue;
            }

            Ϝ.ϑ.Add(
                new ũ(
                    d,
                    this,
                    e,
                    true));

            yield return 1;
        }


        for (int Į = 0;
            Į < Ϝ.ρ.Count;
            Į++)
        {
            IMyShipConnector d =
                Ϝ.ρ[Į];

            Ǒ ǚ;

            if (!Ϝ.ψ.TryGetValue(
                    d.CubeGrid.EntityId,
                    out ǚ) ||
                !ǚ.Ǖ)
            {
                continue;
            }

            Ϝ.ς.Add(
                d);

            F e =
                Ϸ(
                    Ϝ.G,
                    d.EntityId);

            if (!Ѓ(e))
            {
                continue;
            }

            Ϝ.ϒ.Add(
                new Ǫ
                {
                    Ǩ = d,
                    ǩ = Є(
                        Ϝ.ϊ,
                        d)
                });

            yield return 1;
        }

        for (int Į = 0;
            Į < Ϝ.σ.Count;
            Į++)
        {
            IMyLandingGear d =
                Ϝ.σ[Į];

            Ǒ ǚ;

            if (!Ϝ.ψ.TryGetValue(
                    d.CubeGrid.EntityId,
                    out ǚ) ||
                !ǚ.Ǖ)
            {
                continue;
            }

            F e =
                Ϸ(
                    Ϝ.G,
                    d.EntityId);

            if (!Ѓ(e))
            {
                continue;
            }

            Ϝ.ϓ.Add(
                new ǫ
                {
                    Ǩ = d
                });

            yield return 1;
        }


        for (int Į = 0;
            Į < Ϝ.τ.Count;
            Į++)
        {
            IMyTimerBlock ʒ =
                Ϝ.τ[Į];

            if (ʒ.CubeGrid != Me.CubeGrid)
            {
                continue;
            }

            F e =
                Ϸ(
                    Ϝ.G,
                    ʒ.EntityId);

            if ((e & F.Ƈ) != 0)
            {
                Ϝ.υ.Add(ʒ);
            }

            if ((e & F.ƈ) != 0)
            {
                Ϝ.φ.Add(ʒ);
            }

            yield return 1;
        }

        Ѕ(Ϝ);

        І(Ϝ);

        yield return 1;
    }

    void ϩ(
        ϖ Ϝ)
    {
        List<Ǒ> Ї =
            new List<Ǒ>();

        foreach (KeyValuePair<long, Ǒ> Υ
            in Ϝ.ψ)
        {
            Ǒ Ј = Υ.Value;

            if (Ј.ǐ != null)
            {
                continue;
            }

            Ǐ ϴ =
                new Ǐ();

            Ϝ.ω.Add(ϴ);

            Ї.Clear();
            Ї.Add(Ј);
            Ј.ǐ = ϴ;

            for (int ʾ = 0;
                ʾ < Ї.Count;
                ʾ++)
            {
                Ǒ ǚ = Ї[ʾ];

                ϴ.ǜ.Add(ǚ);

                if (ǚ.ǌ.IsStatic)
                {
                    ϴ.Ǡ = true;
                }

                for (int Љ = 0;
                    Љ <
                        ǚ.ǎ.Count;
                    Љ++)
                {
                    Ǒ Њ =
                        ǚ.ǎ[
                            Љ]
                        .Ǜ(ǚ);

                    if (Њ.ǐ != null)
                    {
                        continue;
                    }

                    Њ.ǐ = ϴ;
                    Ї.Add(Њ);
                }
            }
        }
    }

    void ϲ(
        ϖ Ϝ)
    {
        Ǐ Ћ =
            Ϝ.ϕ;

        if (Ћ == null)
        {
            return;
        }

        List<Ǐ> Ї =
            new List<Ǐ>();

        Ћ.ǟ = true;
        Ї.Add(Ћ);

        for (int ʾ = 0;
            ʾ < Ї.Count;
            ʾ++)
        {
            Ǐ ϴ =
                Ї[ʾ];

            for (int Į = 0;
                Į < Ϝ
                    .ϊ.Count;
                Į++)
            {
                ǧ Ќ =
                    Ϝ.ϊ[Į];

                Ǐ Ɨ = null;

                if (Ќ.ǥ.ǐ ==
                    ϴ)
                {
                    Ɨ = Ќ.Ǧ.ǐ;
                }
                else if (Ќ.Ǧ.ǐ ==
                         ϴ)
                {
                    Ɨ = Ќ.ǥ.ǐ;
                }

                if (Ɨ == null ||
                    Ɨ
                        .ǟ)
                {
                    continue;
                }

                Ɨ.ǟ =
                    true;

                Ї.Add(Ɨ);
            }
        }
    }

    void ϳ(
        ϖ Ϝ,
        Ǒ ϧ)
    {
        Ǐ Ћ =
            Ϝ.ϕ;

        if (Ћ == null)
        {
            return;
        }

        Ћ.Ǖ = true;

        Ѝ(
            Ћ,
            ϧ,
            0);

        bool Ў;

        do
        {
            Ў = false;

            for (int Į = 0;
                Į < Ϝ
                    .ϊ.Count;
                Į++)
            {
                ǧ Ќ =
                    Ϝ.ϊ[Į];

                bool Џ =
                    Ќ.ǥ.ǐ
                        .Ǖ;

                bool А =
                    Ќ.Ǧ.ǐ
                        .Ǖ;

                if (Џ == А)
                {
                    continue;
                }

                Ǒ Б =
                    Џ
                        ? Ќ.ǥ
                        : Ќ.Ǧ;

                Ǒ ʄ =
                    Џ
                        ? Ќ.Ǧ
                        : Ќ.ǥ;

                Ǐ В =
                    ʄ.ǐ;

                if (В
                        .Ǟ
                        .Count > 0)
                {
                    continue;
                }

                if (!ȁ.ǭ)
                {
                    continue;
                }

                В
                    .Ǖ = true;

                int Г =
                    Б.ǔ == int.MaxValue
                        ? 0
                        : Б.ǔ;

                Ѝ(
                    В,
                    ʄ,
                    Г);

                Ў = true;
            }
        }
        while (Ў);

        foreach (KeyValuePair<long, Ǒ> Υ
            in Ϝ.ψ)
        {
            Υ.Value.Ǖ =
                Υ.Value.ǐ
                    .Ǖ;
        }
    }

    void Ѝ(
        Ǐ ϴ,
        Ǒ ˋ,
        int Д)
    {
        List<Ǒ> Ї =
            new List<Ǒ>();

        if (ˋ.ǔ > Д)
        {
            ˋ.ǔ = Д;
            ˋ.ǒ = null;
            ˋ.Ǔ = null;
        }

        Ї.Add(ˋ);

        for (int ʾ = 0;
            ʾ < Ї.Count;
            ʾ++)
        {
            Ǒ ǚ = Ї[ʾ];

            for (int Į = 0;
                Į < ǚ.ǎ.Count;
                Į++)
            {
                Ǎ Ќ =
                    ǚ.ǎ[Į];

                Ǒ Њ =
                    Ќ.Ǜ(ǚ);

                if (Њ.ǐ !=
                    ϴ)
                {
                    continue;
                }

                int Е =
                    ǚ.ǔ + 1;

                if (Е >=
                    Њ.ǔ)
                {
                    continue;
                }

                Њ.ǔ = Е;
                Њ.ǒ = ǚ;
                Њ.Ǔ = Ќ;

                Ї.Add(Њ);
            }
        }
    }

    Ò ϻ(
        Ǒ ǚ,
        Dictionary<long, Ò> ϵ)
    {
        Ǒ Ж = ǚ;

        while (Ж != null &&
               Ж.ǒ != null)
        {
            Ǎ Ќ =
                Ж.Ǔ;

            IMyMotorStator З =
                Ќ != null
                    ? Ќ.ǘ
                        as IMyMotorStator
                    : null;

            if (З != null &&
                З.TopGrid ==
                    Ж.ǌ)
            {
                Ò Ī;

                if (ϵ.TryGetValue(
                        З.EntityId,
                        out Ī) &&
                    Ī.Ñ)
                {
                    return Ī.P
                        ? Ī
                        : null;
                }
            }

            Ж = Ж.ǒ;
        }

        return null;
    }

    void Ͽ(
        H ɡ,
        Ǒ И,
        Ò Ī)
    {
        Ǒ Ж = И;

        while (Ж != null)
        {
            Й(
                ɡ.Ħ,
                Ж.ǌ);

            if (Ж.Ǔ != null &&
                Ж.Ǔ.ǘ
                    .EntityId ==
                Ī.Q)
            {
                break;
            }

            Ж = Ж.ǒ;
        }
    }

    void Ѐ(
        List<ŝ> ϝ,
        H ɡ)
    {
        Vector3D ǃ =
            n.o(
                ɡ.É);

        for (int Į = 0;
            Į < ϝ.Count;
            Į++)
        {
            Vector3D К =
                ϝ[Į].É;

            if (Math.Abs(
                    Vector3D.Dot(
                        ǃ,
                        К)) <
                ͼ)
            {
                continue;
            }

            ϝ[Į].Ř.Add(ɡ);
            return;
        }

        ŝ ɟ =
            new ŝ();

        ɟ.Ř.Add(ɡ);
        ϝ.Add(ɟ);
    }

    void Ѕ(
        ϖ Ϝ)
    {
        HashSet<string> Л =
            new HashSet<string>(
                StringComparer.Ordinal);

        List<int> М =
            new List<int>();

        for (int Į = 0;
            Į < Ϝ.κ.Count;
            Į++)
        {
            IMyTerminalBlock d =
                Ϝ.κ[Į];

            Ǒ ǚ;

            if (!Ϝ.ψ.TryGetValue(
                    d.CubeGrid.EntityId,
                    out ǚ) ||
                ǚ.ǐ !=
                    Ϝ.ϕ)
            {
                continue;
            }

            F e =
                Ϸ(
                    Ϝ.G,
                    d.EntityId);

            IMyTextPanel Н =
                d as IMyTextPanel;

            if (Н != null &&
                (e & F.Ɔ) != 0)
            {
                О(
                    Ϝ.ϔ,
                    Л,
                    d,
                    Н,
                    0);
            }

            IMyTextSurfaceProvider П =
                d as IMyTextSurfaceProvider;

            if (П == null ||
                П.SurfaceCount <= 0)
            {
                continue;
            }

            М.Clear();

            Р(
                d.CustomData,
                П.SurfaceCount,
                М);

            if ((e & F.Ɔ) != 0 &&
                М.Count == 0)
            {
                М.Add(0);
            }

            for (int ʾ = 0;
                ʾ < М.Count;
                ʾ++)
            {
                int ż =
                    М[ʾ];

                О(
                    Ϝ.ϔ,
                    Л,
                    d,
                    П.GetSurface(
                        ż),
                    ż);
            }
        }
    }

    void І(
        ϖ Ϝ)
    {
        HashSet<long> С =
            new HashSet<long>();

        HashSet<long> Т =
            new HashSet<long>();

        HashSet<long> У =
            new HashSet<long>();

        for (int Į = 0;
            Į < Ϝ
                .ϋ.Count;
            Į++)
        {
            С.Add(
                Ϝ.ϋ[Į]
                    .Q);
        }

        for (int Į = 0;
            Į < Ϝ
                .ώ.Count;
            Į++)
        {
            Т.Add(
                Ϝ.ώ[Į]
                    .Q);
        }

        for (int Į = 0;
            Į < Ϝ
                .ϑ.Count;
            Į++)
        {
            У.Add(
                Ϝ.ϑ[Į]
                    .E.EntityId);
        }

        for (int Į = 0;
            Į < Ʉ.Count;
            Į++)
        {
            g Ф =
                Ʉ[Į];

            if (!С.Contains(
                    Ф.Q))
            {
                Ф.À();
                ʏ(
                    Ф.Q,
                    Ф.E);
            }
        }

        for (int Į = 0;
            Į < ʉ.Count;
            Į++)
        {
            Ò Х =
                ʉ[Į];

            if (!Т.Contains(
                    Х.Q))
            {
                Х.À();
            }
        }

        for (int Į = 0;
            Į < ɢ.Count;
            Į++)
        {
            ũ Ц =
                ɢ[Į];

            if (!У.Contains(
                    Ц.E.EntityId))
            {
                Ц.À();
            }
        }

        Ч(
            ɮ,
            Ϝ.λ);

        Ч(
            ʪ,
            Ϝ.μ);

        Ч(
            Ν,
            Ϝ.ĥ);

        Ч(
            Ʉ,
            Ϝ.ϋ);

        Ч(
            ə,
            Ϝ.ό);

        Ч(
            ɗ,
            Ϝ.ύ);

        Ч(
            ʉ,
            Ϝ.ώ);

        Ч(
            Ƀ,
            Ϝ.Ϗ);

        Ч(
            ɛ,
            Ϝ.ϐ);

        Ч(
            ɢ,
            Ϝ.ϑ);

        Ч(
            ɼ,
            Ϝ.ϒ);

        Ч(
            ɾ,
            Ϝ.ϓ);

        Ч(
            ϗ,
            Ϝ.ϊ);

        Ч(
            ʘ,
            Ϝ.ς);

        Ч(
            ʋ,
            Ϝ.υ);

        Ч(
            ʌ,
            Ϝ.φ);

        Ч(
            Ͳ,
            Ϝ.ϔ);

        ʃ.Clear();

        foreach (KeyValuePair<long, Ǒ> Υ
            in Ϝ.ψ)
        {
            ʃ.Add(Υ.Key, Υ.Value);
        }

        Ƞ();

        if (ȭ == ƅ.Ƅ)
        {
            ʍ();
        }

        ɺ = true;
    }

    F ϟ(string ž)
    {
        if (string.IsNullOrEmpty(ž))
        {
            return F.M;
        }

        F Ш = F.M;

        if (Щ(
                ž,
                ȁ.ǲ))
        {
            Ш |= F.Z;
        }

        if (Щ(
                ž,
                ȁ.ǳ))
        {
            Ш |= F.X;
        }

        if (Щ(
                ž,
                ȁ.Ǵ))
        {
            Ш |= F.Ɔ;
        }

        if (Щ(
                ž,
                ȁ.ǵ))
        {
            Ш |= F.Ƈ;
        }

        if (Щ(
                ž,
                ȁ.Ƕ))
        {
            Ш |= F.ƈ;
        }

        return Ш;
    }

    bool ϸ(
        F e)
    {
        if ((e & F.X) != 0)
        {
            return false;
        }

        return ȁ.Ǭ ||
               (e & F.Z) != 0;
    }

    bool Ͼ(
        F e,
        bool Ъ,
        bool Ͻ,
        Ò ϼ)
    {
        if (!Ъ ||
            (e & F.X) != 0)
        {
            return false;
        }

        bool Ы =
            (e & F.Z) != 0;

        if (!ȁ.Ǭ)
        {
            return Ы;
        }

        return Ы ||
               Ͻ ||
               ϼ != null;
    }

    bool Ђ(
        F e,
        bool Ͻ)
    {
        if ((e & F.X) != 0)
        {
            return false;
        }

        bool Ы =
            (e & F.Z) != 0;

        if (!ȁ.Ǭ)
        {
            return Ы;
        }

        return Ы ||
               Ͻ;
    }

    bool Ѓ(
        F e)
    {
        if ((e & F.X) != 0)
        {
            return false;
        }

        return ȁ.Ǭ ||
               (e & F.Z) != 0;
    }

    bool Ё(IMyGyro ϥ)
    {
        string Ь =
            ϥ.BlockDefinition.SubtypeId;

        if (Ь.Equals(
"SmallBlockGyro",
                StringComparison.OrdinalIgnoreCase) ||
            Ь.Equals(
"LargeBlockGyro",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return Ь.Equals(
"SmallPrototechGyro",
                   StringComparison.OrdinalIgnoreCase) ||
               Ь.Equals(
"LargePrototechGyro",
                   StringComparison.OrdinalIgnoreCase) ||
               Ь.Equals(
"SmallPrototechGyroscope",
                   StringComparison.OrdinalIgnoreCase) ||
               Ь.Equals(
"LargePrototechGyroscope",
                   StringComparison.OrdinalIgnoreCase);
    }

    bool Ϫ(
        IMyProgrammableBlock Ϧ)
    {
        if (Ϧ == null)
        {
            return false;
        }

        return ˆ(
                   Ϧ.CustomData,
                   Ȋ) >= 0;
    }

    bool ϫ(
        IMyProgrammableBlock Ϧ)
    {
        string ˢ;

        if (!ʧ(
                Ϧ.CustomData,
                Ȋ,
"CanSlave",
                out ˢ))
        {
            return true;
        }

        bool ğ;

        return bool.TryParse(
                   ˢ,
                   out ğ)
            ? ğ
            : true;
    }

    void Р(
        string ǽ,
        int Э,
        List<int> Ĺ)
    {
        if (string.IsNullOrEmpty(ǽ))
        {
            return;
        }

        string[] ˎ = ǽ.Replace(
"\r",
                string.Empty)
            .Split('\n');

        for (int Į = 0;
            Į < ˎ.Length;
            Į++)
        {
            string ˏ = ˎ[Į].Trim();

            if (!ˏ.StartsWith(
                    ʹ,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string Ю =
                ˏ.Substring(
                    ʹ.Length)
                .Trim();

            int ʾ;

            if (!int.TryParse(
                    Ю,
                    out ʾ) ||
                ʾ < 0 ||
                ʾ >= Э ||
                Ĺ.Contains(ʾ))
            {
                continue;
            }

            Ĺ.Add(ʾ);
        }
    }

    static void О(
        List<Ž> Ĺ,
        HashSet<string> w,
        IMyTerminalBlock ź,
        IMyTextSurface Ż,
        int ż)
    {
        string Ȕ =
            ź.EntityId +
":"+
            ż;

        if (!w.Add(Ȕ))
        {
            return;
        }

        Ĺ.Add(
            new Ž(
                ź,
                Ż,
                ż));
    }

    static void Й(
        List<IMyCubeGrid> Я,
        IMyCubeGrid Ŗ)
    {
        for (int Į = 0;
            Į < Я.Count;
            Į++)
        {
            if (Я[Į].EntityId ==
                Ŗ.EntityId)
            {
                return;
            }
        }

        Я.Add(Ŗ);
    }

    static ǧ Є(
        List<ǧ> а,
        IMyShipConnector ʁ)
    {
        for (int Į = 0;
            Į < а.Count;
            Į++)
        {
            if (а[Į].ǖ.EntityId ==
                    ʁ.EntityId ||
                а[Į].Ǘ.EntityId ==
                    ʁ.EntityId)
            {
                return а[Į];
            }
        }

        return null;
    }

    static Ǒ ϣ(
        Dictionary<long, Ǒ> б,
        IMyCubeGrid Ŗ)
    {
        Ǒ ǚ;

        if (!б.TryGetValue(
                Ŗ.EntityId,
                out ǚ))
        {
            ǚ = new Ǒ(Ŗ);
            б.Add(Ŗ.EntityId, ǚ);
        }

        return ǚ;
    }

    static void Ϩ(
        Dictionary<long, Ǒ> б,
        IMyCubeGrid в,
        IMyCubeGrid г,
        IMyTerminalBlock Ǚ)
    {
        Ǒ ƻ =
            ϣ(б, в);

        Ǒ Ƽ =
            ϣ(б, г);

        Ǎ Ќ =
            new Ǎ(ƻ, Ƽ, Ǚ);

        ƻ.ǎ.Add(Ќ);
        Ƽ.ǎ.Add(Ќ);
    }

    static void ϡ(
        Dictionary<long, F> e,
        long ʇ,
        F д)
    {
        if (д ==
            F.M)
        {
            return;
        }

        F е;

        e.TryGetValue(
            ʇ,
            out е);

        e[ʇ] =
            е | д;
    }

    static F Ϸ(
        Dictionary<long, F> e,
        long ʇ)
    {
        F Ш;

        return e.TryGetValue(
                   ʇ,
                   out Ш)
            ? Ш
            : F.M;
    }

    static bool Щ(
        string ž,
        string ж)
    {
        return !string.IsNullOrEmpty(ž) &&
               !string.IsNullOrEmpty(ж) &&
               ž.IndexOf(
                   ж,
                   StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static void Ч<з>(
        List<з> ʄ,
        List<з> Б)
    {
        ʄ.Clear();
        ʄ.AddRange(Б);
    }

    bool и
    {
        get
        {
            return ȭ ==
                    ƅ.ƃ
                ? ȷ
                    .ƨ
                : ɐ;
        }
    }

    bool й
    {
        get
        {
            return ȭ ==
                    ƅ.ƃ
                ? ȷ
                    .Ɖ
                : Ⱦ;
        }
    }

    double к
    {
        get
        {
            return ȭ ==
                    ƅ.ƃ
                ? ȷ
                    .ƪ
                : Γ;
        }
    }

    double л
    {
        get
        {
            if (ȭ ==
                ƅ.ƃ)
            {
                return MathHelper.Clamp(
                    ȷ
                        .ƫ,
                    0,
                    1);
            }

            if (ȁ
                    .Ǳ.Count == 0)
            {
                return 0;
            }

            return MathHelper.Clamp(
                ȁ.Ǳ[
                    MathHelper.Clamp(
                        Ȍ,
                        0,
                        ȁ
                            .Ǳ
                            .Count -
                        1)],
                0,
                1);
        }
    }


    void н(
        string Ш,
        bool м)
    {
        Ή =
            Ш ?? string.Empty;

        Ύ =
            м;

        ɺ =
            true;
    }

    void о()
    {
        Ή =
            string.Empty;

        Ύ =
            false;
    }


    void р(
        bool i)
    {
        ɐ =
            i;

        п(
            i);
    }

    void п(
        bool i)
    {
        for (int Į = 0;
            Į < ɮ.Count;
            Į++)
        {
            IMyShipController ɯ =
                ɮ[Į];

            if (ɯ == null ||
                ɯ.Closed ||
                !ɯ.IsFunctional)
            {
                continue;
            }

            if (ɯ
                    .DampenersOverride ==
                i)
            {
                continue;
            }

            ɯ
                .DampenersOverride =
                i;
        }
    }

    void т()
    {
        if (ȭ ==
            ƅ.ƃ)
        {
            п(
                ȷ
                    .ƨ);

            return;
        }

        if (!Ι)
        {
            п(
                ɐ);

            return;
        }

        if (Ȥ ==
                null ||
            Ȥ.Closed)
        {
            return;
        }

        bool с =
            Ȥ
                .DampenersOverride;

        if (с ==
            ɐ)
        {
            return;
        }

        ɐ =
            с;

        п(
            ɐ);
    }

    void ш(
        bool i)
    {
        if (i &&
            !Ⱦ)
        {
            у();
        }

        Ⱦ = i;

        if (!Ⱦ)
        {
            ф(
                ƌ.Ɖ);

            х(
                C.Ɖ);
        }

        ц();
        ч();
    }

    void щ()
    {
        ш(
            !Ⱦ);
    }

    void у()
    {
        if (Ȥ ==
                null ||
            Ȥ.Closed)
        {
            Γ = 0;
            Ί =
                false;

            return;
        }

        Vector3D Ɉ =
            Ȥ
                .GetShipVelocities()
                .LinearVelocity;

        Γ =
            Vector3D.Dot(
                Ɉ,
                Ȥ
                    .WorldMatrix
                    .Forward);

        Ί =
            true;
    }

    void ъ()
    {
        if (Ί)
        {
            return;
        }

        у();
    }

    void ы(
        double Ö)
    {
        ъ();

        Γ +=
            Ö;

        н(
"Cruise target: "+
            Γ
                .ToString("0.###") +
" m/s",
            false);
    }


    bool b(
        long ʇ)
    {
        Ə Φ;

        if (!Δ
                .TryGetValue(
                    ʇ,
                    out Φ))
        {
            return false;
        }

        return Φ.ƍ &&
               Φ.Ǝ !=
                   ƌ.M;
    }

    bool ь(
        long ʇ,
        out Ə Φ)
    {
        return Δ
            .TryGetValue(
                ʇ,
                out Φ);
    }

    void ю(
        g İ,
        ƌ э)
    {
        if (İ == null)
        {
            return;
        }

        ю(
            İ.E,
            э);
    }

    void ю(
        IMyThrust d,
        ƌ э)
    {
        if (d == null ||
            d.Closed ||
            э ==
                ƌ.M)
        {
            return;
        }

        Ə Φ;

        if (!Δ
                .TryGetValue(
                    d.EntityId,
                    out Φ))
        {
            Φ =
                new Ə
                {
                    ƍ =
                        d.Enabled,
                    Ǝ =
                        ƌ.M
                };

            Δ.Add(
                d.EntityId,
                Φ);
        }

        Φ.Ǝ |=
            э;

        if (d.Enabled)
        {
            d.Enabled =
                false;
        }

        if ((Φ.Ǝ &
             ƌ.Ɗ) != 0)
        {
            ʈ[
                d.EntityId] =
                Φ.ƍ;
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

        Ə Φ;

        if (!Δ
                .TryGetValue(
                    d.EntityId,
                    out Φ))
        {
            return;
        }

        if (Φ.ƍ &&
            !d.Enabled)
        {
            d.Enabled =
                true;
        }
    }

    void ф(
        g İ,
        ƌ э)
    {
        if (İ == null)
        {
            return;
        }

        ф(
            İ.Q,
            İ.E,
            э);
    }

    void ф(
        long ʇ,
        IMyThrust d,
        ƌ э)
    {
        Ə Φ;

        if (!Δ
                .TryGetValue(
                    ʇ,
                    out Φ))
        {
            return;
        }

        Φ.Ǝ &=
            ~э;

        if ((э &
             ƌ.Ɗ) != 0)
        {
            ʈ.Remove(
                ʇ);
        }

        if (Φ.Ǝ !=
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
                Φ.ƍ;
        }

        Δ.Remove(
            ʇ);
    }

    void ф(
        ƌ э)
    {
        if (Δ.Count ==
            0)
        {
            return;
        }

        List<long> я =
            new List<long>(
                Δ.Keys);

        for (int Į = 0;
            Į < я.Count;
            Į++)
        {
            long ʇ =
                я[Į];

            IMyThrust d =
                ѐ(
                    ʇ);

            ф(
                ʇ,
                d,
                э);
        }
    }

    IMyThrust ѐ(
        long ʇ)
    {
        for (int Į = 0;
            Į < Ν.Count;
            Į++)
        {
            g İ =
                Ν[Į];

            if (İ.Q ==
                ʇ)
            {
                return İ.E;
            }
        }

        return null;
    }

    void ё(
        g İ)
    {
        if (İ == null)
        {
            return;
        }

        Ə Φ;

        if (!Δ
                .TryGetValue(
                    İ.Q,
                    out Φ))
        {
            return;
        }

        if (Φ.Ǝ ==
            ƌ.M)
        {
            Δ.Remove(
                İ.Q);

            return;
        }

        if (İ.E.Enabled)
        {
            İ.E.Enabled =
                false;
        }
    }

    void є(
        HashSet<long> ђ)
    {
        if (Δ.Count ==
            0)
        {
            return;
        }

        List<long> ѓ =
            new List<long>();

        foreach (
            KeyValuePair<long, Ə> Υ
            in Δ)
        {
            if (!ђ.Contains(
                    Υ.Key))
            {
                ѓ.Add(
                    Υ.Key);
            }
        }

        for (int Į = 0;
            Į < ѓ.Count;
            Į++)
        {
            long ʇ =
                ѓ[Į];

            Δ.Remove(
                ʇ);

            ʈ.Remove(
                ʇ);
        }
    }


    void ц()
    {
        bool ѕ =
            ȭ ==
            ƅ.ƃ;

        bool і =
            й;

        for (int Į = 0;
            Į < Ν.Count;
            Į++)
        {
            g İ =
                Ν[Į];

            bool ї =
                İ
                    .E
                    .CubeGrid ==
                Me.CubeGrid;

            bool ј =
                !İ.Y &&
                (ȁ.Ǭ ||
                 İ
                     .a);

            bool љ =
                ѕ &&
                ї &&
                ј;

            bool ћ =
                і &&
                ї &&
                њ(
                    İ) &&
                ј;

            İ.N(
                C.ƃ,
                љ);

            İ.N(
                C.Ɖ,
                ћ);

            if (!ћ)
            {
                ф(
                    İ,
                    ƌ
                        .Ɖ);
            }
        }

        for (int Į = 0;
            Į < ɢ.Count;
            Į++)
        {
            ũ ϥ =
                ɢ[Į];

            bool ї =
                ϥ
                    .E
                    .CubeGrid ==
                Me.CubeGrid;

            bool ќ =
                !ϥ.Y &&
                (ȁ.Ǭ ||
                 ϥ.a);

            ϥ.N(
                C.ƃ,
                ѕ &&
                ї &&
                ќ);
        }

        ѝ();
    }

    void х(
        C h)
    {
        for (int Į = 0;
            Į < Ν.Count;
            Į++)
        {
            Ν[Į]
                .N(
                    h,
                    false);
        }

        for (int Į = 0;
            Į < ɢ.Count;
            Į++)
        {
            ɢ[Į]
                .N(
                    h,
                    false);
        }
    }

    bool њ(
        g İ)
    {
        if (İ == null ||
            Ȥ ==
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
                   Ȥ
                       .WorldMatrix
                       .Backward) >=
               Ķ;
    }

    void ѝ()
    {
        Ρ.Clear();

        if (Ȥ ==
            null)
        {
            return;
        }

        for (int Į = 0;
            Į < Ν.Count;
            Į++)
        {
            g İ =
                Ν[Į];

            if (њ(
                    İ))
            {
                Ρ.Add(
                    İ);
            }
        }
    }

    void ў()
    {
        Ι =
            false;

        for (int Į = 0;
            Į < Ν.Count;
            Į++)
        {
            g İ =
                Ν[Į];

            if (İ.E.CubeGrid ==
                Me.CubeGrid)
            {
                Ι =
                    true;

                break;
            }
        }

        ѝ();
    }

    void ч()
    {
        if (!й ||
            Ρ.Count ==
                0)
        {
            return;
        }

        if (ȁ.Ǭ)
        {
            bool џ =
                true;

            for (int Į = 0;
                Į <
                    Ρ
                        .Count;
                Į++)
            {
                if (!Ρ[Į]
                        .Y)
                {
                    џ =
                        false;

                    break;
                }
            }

            if (џ)
            {
                н(
"WARNING: Cruise cannot control "+
"main-grid reverse thrusters; "+
"all are "+
                    ȁ.ǳ +
".",
                    true);
            }

            return;
        }

        bool Ѡ =
            false;

        for (int Į = 0;
            Į <
                Ρ
                    .Count;
            Į++)
        {
            g İ =
                Ρ[Į];

            if (!İ.Y &&
                İ.a)
            {
                Ѡ =
                    true;

                break;
            }
        }

        if (!Ѡ)
        {
            н(
"WARNING: Cruise cannot control "+
"main-grid reverse thrusters; "+
"add "+
                ȁ.ǲ +
".",
                true);
        }
    }


    void ѡ(
        ƅ ɷ,
        ƅ ɶ)
    {
        if (ɷ ==
                ƅ.ƃ &&
            ɶ !=
                ƅ.ƃ)
        {
            х(
                C.ƃ);

            п(
                ɐ);
        }

        if (ɷ !=
                ƅ.ƃ &&
            ɶ ==
                ƅ.ƃ)
        {
            п(
                ȷ
                    .ƨ);
        }

        ц();
    }
}
