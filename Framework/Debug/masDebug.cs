using System.Collections.Generic;
using System;
using Godot;
using System.Data;
using System.Runtime.CompilerServices;


public partial class masDebug : Node
{
    static private Node            Shapes   = new Node();
    static public Node            Billboards = new Node();
    static private CanvasLayer     Canvas   = new CanvasLayer    ();
    static private VBoxContainer   Logs     = new VBoxContainer  ();
    static private ScrollContainer Scroller = new ScrollContainer();


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public override void _Ready()
    {
        AddChild(Shapes);
        AddChild(Billboards);

        AddChild(Canvas);
        Canvas.AddChild(Scroller);
        Scroller.AddChild(Logs);

        Scroller.AnchorRight        = 1;
        Scroller.AnchorBottom       = 1;
        Scroller.VerticalScrollMode = ScrollContainer.ScrollMode.ShowNever;
        Scroller.Position           = new Vector2(10.0f, 10.0f);
        Vector2 ScrollSize          = Scroller.Size;
        ScrollSize.Y               -= 20.0f;
        Scroller.CallDeferred("set_size", ScrollSize);
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // DEBUG TIMEOUT HELPERS
    //////////////////////////////////////////////////////////////////////////////////////////////
    private static async void SetupShapeTimeout(Node InDebugShape, float InDuration)
    {
        await Shapes.ToSignal(Shapes.GetTree().CreateTimer(InDuration), Timer.SignalName.Timeout);
        InDebugShape.QueueFree();
    }

    private static async void SetupBillboardTimeout(Node InBillboard, float InDuration)
    {
        await Billboards.ToSignal(Billboards.GetTree().CreateTimer(InDuration), Timer.SignalName.Timeout);
        InBillboard.QueueFree();
    }

    private static async void SetupLogTimeout(Label InLog, float InDuration)
    {
        await Canvas.ToSignal(Canvas.GetTree().CreateTimer(InDuration), Timer.SignalName.Timeout);
        InLog.QueueFree();
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // DEBUG BILLBOARD TEXT
    //////////////////////////////////////////////////////////////////////////////////////////////
    private static Label3D Internal_CreateBillboardText(String InText, Vector3 InPosition, Color InColor, int InFontSize)
    {
        Label3D label3D = new Label3D();

        label3D.Text           = InText;
        label3D.GlobalPosition = InPosition;
        label3D.Billboard      = BaseMaterial3D.BillboardModeEnum.Enabled;
        label3D.FontSize       = InFontSize;
        
        Billboards.AddChild(label3D);

        return label3D;
    }

    public static void BillboardText(String InText, Vector3 InPosition, Color InColor, float InDuration, int InFontSize = 256)
    {
        Label3D Instance = Internal_CreateBillboardText(InText, InPosition, InColor, InFontSize);
        SetupShapeTimeout(Instance, InDuration);
    }

    public static Node3D CreateBillboardText(String InText, Vector3 InPosition, Color InColor, int InFontSize = 256)
    {
        Node3D FoundInstance = Billboards.GetNode<Node3D>(InText);
        if (FoundInstance != null)
            return FoundInstance;

        Label3D Instance = Internal_CreateBillboardText(InText, InPosition, InColor, InFontSize);
        Instance.Name = InText;

        return Instance;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // DEBUG LINE
    //////////////////////////////////////////////////////////////////////////////////////////////
    private static MeshInstance3D Internal_CreateLineMesh(Vector3 InStart, Vector3 InEnd, Color InColor)
    {
        ImmediateMesh LineMesh = new ImmediateMesh();
        LineMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        LineMesh.SurfaceAddVertex(new Vector3());
        LineMesh.SurfaceAddVertex((InEnd - InStart));
        LineMesh.SurfaceEnd();

        StandardMaterial3D LineMaterial = new StandardMaterial3D();
        LineMaterial.ShadingMode        = BaseMaterial3D.ShadingModeEnum.Unshaded;
        LineMaterial.AlbedoColor        = InColor;

        MeshInstance3D Instance = new MeshInstance3D();
        Instance.Mesh           = LineMesh;
        Instance.GlobalPosition = InStart;
        Instance.SetSurfaceOverrideMaterial(0, LineMaterial);

        Shapes.AddChild(Instance);
        return Instance;
    }

    public static void Line(Vector3 InStart, Vector3 InEnd, Color InColor, float InDuration)
    {
        MeshInstance3D LineInstance = Internal_CreateLineMesh(InStart, InEnd, InColor);
        SetupShapeTimeout(LineInstance, InDuration);
    }

    public static Node3D CreateLine(String InName, Vector3 InStart, Vector3 InEnd, Color InColor)
    {
        Node3D FoundInstance = Shapes.GetNode<Node3D>(InName);
        if (FoundInstance != null)
            return FoundInstance;

        MeshInstance3D Instance = Internal_CreateLineMesh(InStart, InEnd, InColor);
        Instance.Name = InName;

        return Instance;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // DEBUG BOX
    //////////////////////////////////////////////////////////////////////////////////////////////
    private static MeshInstance3D Internal_CreateBoxMesh(Vector3 InPosition, Vector3 InSize, Color InColor)
    {
        Vector3 Edge = InSize * 0.5f;

        ImmediateMesh BoxMesh = new ImmediateMesh();
        BoxMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        //BoxMesh.SurfaceSetColor(InColor);

        Vector3[] corners = 
        {
            new Vector3(-Edge.X, -Edge.Y, -Edge.Z),
            new Vector3( Edge.X, -Edge.Y, -Edge.Z),
            new Vector3( Edge.X,  Edge.Y, -Edge.Z),
            new Vector3(-Edge.X,  Edge.Y, -Edge.Z),
            new Vector3(-Edge.X, -Edge.Y,  Edge.Z),
            new Vector3( Edge.X, -Edge.Y,  Edge.Z),
            new Vector3( Edge.X,  Edge.Y,  Edge.Z),
            new Vector3(-Edge.X,  Edge.Y,  Edge.Z)
        };

        int[,] edges = 
        {
            {0,1},{1,2},{2,3},{3,0},
            {4,5},{5,6},{6,7},{7,4},
            {0,4},{1,5},{2,6},{3,7}
        };

        for (int i = 0; i < edges.GetLength(0); i++)
        {
            BoxMesh.SurfaceAddVertex(corners[edges[i, 0]]);
            BoxMesh.SurfaceAddVertex(corners[edges[i, 1]]);
        }

        BoxMesh.SurfaceEnd();

        StandardMaterial3D LineMaterial = new StandardMaterial3D();
        LineMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        LineMaterial.AlbedoColor = InColor;

        MeshInstance3D Instance = new MeshInstance3D();
        Instance.Mesh           = BoxMesh;
        Instance.GlobalPosition = InPosition;
        Instance.SetSurfaceOverrideMaterial(0, LineMaterial);

        Shapes.AddChild(Instance);
        return Instance;
    }

    public static void Box(Vector3 InPosition, Vector3 InSize, Color InColor, float InDuration)
    {
        MeshInstance3D BoxInstance = Internal_CreateBoxMesh(InPosition, InSize, InColor);
        SetupShapeTimeout(BoxInstance, InDuration);
    }

    public static Node3D CreateBox(String InName, Vector3 InPosition, Vector3 InSize, Color InColor)
    {
        Node3D FoundInstance = Shapes.GetNode<Node3D>(InName);
        if (FoundInstance != null)
            return FoundInstance;

        MeshInstance3D Instance = Internal_CreateBoxMesh(InPosition, InSize, InColor);
        Instance.Name = InName;

        return Instance;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // DEBUG SPHERE
    //////////////////////////////////////////////////////////////////////////////////////////////
    private static MeshInstance3D Internal_CreateSphereMesh(Vector3 InPosition, float InRadius, Color InColor)
    {
        int Slices = 6;
        int Stacks = 6;

        ImmediateMesh SphereMesh = new ImmediateMesh();
        SphereMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

        // latitude rings (stacks)
        float TwoPi = Mathf.Pi * 2f;
        for (int stack = 0; stack <= Stacks; stack++)
        {
            float phi = Mathf.Pi * (stack / (float)Stacks) - Mathf.Pi * 0.5f; // -pi/2..pi/2
            float y = Mathf.Sin(phi) * InRadius;
            float r = Mathf.Cos(phi) * InRadius;

            for (int slice = 0; slice < Slices; slice++)
            {
                float theta0 = (slice / (float)Slices) * TwoPi;
                float theta1 = ((slice + 1) / (float)Slices) * TwoPi;

                Vector3 v0 = new Vector3(Mathf.Cos(theta0) * r, y, Mathf.Sin(theta0) * r);
                Vector3 v1 = new Vector3(Mathf.Cos(theta1) * r, y, Mathf.Sin(theta1) * r);

                SphereMesh.SurfaceAddVertex(v0);
                SphereMesh.SurfaceAddVertex(v1);
            }
        }

        // longitude rings (slices)
        for (int slice = 0; slice < Slices; slice++)
        {
            float theta = (slice / (float)Slices) * TwoPi;
            for (int stack = 0; stack < Stacks; stack++)
            {
                float phi0 = Mathf.Pi * (stack / (float)Stacks) - Mathf.Pi * 0.5f;
                float phi1 = Mathf.Pi * ((stack + 1) / (float)Stacks) - Mathf.Pi * 0.5f;

                Vector3 v0 = new Vector3(Mathf.Cos(theta) * Mathf.Cos(phi0) * InRadius, Mathf.Sin(phi0) * InRadius, Mathf.Sin(theta) * Mathf.Cos(phi0) * InRadius);
                Vector3 v1 = new Vector3(Mathf.Cos(theta) * Mathf.Cos(phi1) * InRadius, Mathf.Sin(phi1) * InRadius, Mathf.Sin(theta) * Mathf.Cos(phi1) * InRadius);

                SphereMesh.SurfaceAddVertex(v0);
                SphereMesh.SurfaceAddVertex(v1);
            }
        }

        SphereMesh.SurfaceEnd();

        StandardMaterial3D LineMaterial = new StandardMaterial3D();
        LineMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        LineMaterial.AlbedoColor = InColor;

        MeshInstance3D Instance = new MeshInstance3D();
        Instance.Mesh           = SphereMesh;
        Instance.Transform      = new Transform3D(Basis.Identity, InPosition);
        Instance.SetSurfaceOverrideMaterial(0, LineMaterial);

        Shapes.AddChild(Instance);
        return Instance;
    }

    public static void Sphere(Vector3 InPosition, float InRadius, Color InColor, float InDuration)
    {
        MeshInstance3D Instance = Internal_CreateSphereMesh(InPosition, InRadius, InColor);
        SetupShapeTimeout(Instance, InDuration);
    }

    public static Node3D CreateSphere(String InName, Vector3 InPosition, float InRadius, Color InColor)
    {
        Node3D FoundInstance = Shapes.GetNode<Node3D>(InName);
        if (FoundInstance != null)
            return FoundInstance;

        MeshInstance3D Instance = Internal_CreateSphereMesh(InPosition, InRadius, InColor);
        Instance.Name = InName;

        return Instance;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // DEBUG CAPSULE
    //////////////////////////////////////////////////////////////////////////////////////////////
    private static MeshInstance3D Internal_CreateCapsuleMesh(Vector3 InPosition, float InRadius, float InHeight, Color InColor)
    {
        // clamp sensible defaults
        int Slices = 16;
        int Stacks = 8;

        // cylinder half-length (distance from center to hemisphere base)
        float halfCyl = Math.Max(0f, InHeight * 0.5f);

        ImmediateMesh CapsuleMesh = new ImmediateMesh();
        CapsuleMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        CapsuleMesh.SurfaceSetColor(InColor);

        float twoPi = Mathf.Pi * 2f;

        // 1) horizontal rings along the cylinder and hemispheres (latitude-like)
        // Draw rings at several Y positions: from top hemisphere down through cylinder to bottom hemisphere
        int rings = Stacks * 2 + 1; // stacks for top hemisphere, stacks for bottom, plus middle ring
        for (int r = 0; r <= rings; r++)
        {
            // t from 0..1 across full capsule height (top pole -> bottom pole)
            float t = r / (float)rings;
            // map t to polar angle phi (-pi/2 .. +pi/2) across hemispheres and cylinder
            // compute y and ring radius depending on whether we're in hemisphere or cylinder region
            float y;
            float ringRadius;

            // normalized position along capsule centerline where 0 = top pole, 1 = bottom pole
            float pos = t;

            // top hemisphere region (0 .. stacks/(rings))
            float topBoundary = (float)Stacks / rings;
            float bottomBoundary = 1f - topBoundary;

            if (pos <= topBoundary)
            {
                // top hemisphere: map pos 0..topBoundary to phi 0..pi/2
                float local = pos / topBoundary; // 0..1
                float phi = local * (Mathf.Pi * 0.5f); // 0..pi/2
                y = halfCyl + InRadius * Mathf.Cos(phi); // from top pole down to hemisphere base
                ringRadius = InRadius * Mathf.Sin(phi);
            }
            else if (pos >= bottomBoundary)
            {
                // bottom hemisphere: map pos bottomBoundary..1 to phi pi/2..pi
                float local = (pos - bottomBoundary) / topBoundary; // 0..1
                float phi = (Mathf.Pi * 0.5f) + local * (Mathf.Pi * 0.5f); // pi/2..pi
                y = -halfCyl + InRadius * Mathf.Cos(phi); // from hemisphere base down to bottom pole
                ringRadius = InRadius * Mathf.Sin(phi);
            }
            else
            {
                // cylinder region: constant ring radius, y linearly between hemisphere bases
                float local = (pos - topBoundary) / (bottomBoundary - topBoundary); // 0..1
                y = halfCyl - local * (2f * halfCyl); // from +halfCyl down to -halfCyl
                ringRadius = InRadius;
            }

            // create ring around Y = y
            for (int s = 0; s < Slices; s++)
            {
                float a0 = (s / (float)Slices) * twoPi;
                float a1 = ((s + 1) / (float)Slices) * twoPi;
                Vector3 v0 = new Vector3(Mathf.Cos(a0) * ringRadius, y, Mathf.Sin(a0) * ringRadius);
                Vector3 v1 = new Vector3(Mathf.Cos(a1) * ringRadius, y, Mathf.Sin(a1) * ringRadius);
                CapsuleMesh.SurfaceAddVertex(v0);
                CapsuleMesh.SurfaceAddVertex(v1);
            }
        }

        // 2) vertical meridian lines (longitude-like) — draw a few meridians to show profile
        int meridians = Math.Max(3, Slices / 4); // a few vertical lines
        for (int m = 0; m < meridians; m++)
        {
            float theta = (m / (float)meridians) * twoPi;
            for (int st = 0; st < Stacks * 2 + 1; st++)
            {
                // sample along capsule from top pole to bottom pole
                float t0 = st / (float)(Stacks * 2 + 1);
                float t1 = (st + 1) / (float)(Stacks * 2 + 1);

                // helper to compute point on capsule centerline for parameter t (0..1)
                Vector3 PointOnCapsule(float tt)
                {
                    float pos2 = tt;
                    float topB = (float)Stacks / (Stacks * 2 + 1);
                    float bottomB = 1f - topB;

                    if (pos2 <= topB)
                    {
                        float local = pos2 / topB;
                        float phi = local * (Mathf.Pi * 0.5f);
                        float yy = halfCyl + InRadius * Mathf.Cos(phi);
                        float rr = InRadius * Mathf.Sin(phi);
                        return new Vector3(Mathf.Cos(theta) * rr, yy, Mathf.Sin(theta) * rr);
                    }
                    else if (pos2 >= bottomB)
                    {
                        float local = (pos2 - bottomB) / topB;
                        float phi = (Mathf.Pi * 0.5f) + local * (Mathf.Pi * 0.5f);
                        float yy = -halfCyl + InRadius * Mathf.Cos(phi);
                        float rr = InRadius * Mathf.Sin(phi);
                        return new Vector3(Mathf.Cos(theta) * rr, yy, Mathf.Sin(theta) * rr);
                    }
                    else
                    {
                        float local = (pos2 - topB) / (bottomB - topB);
                        float yy = halfCyl - local * (2f * halfCyl);
                        float rr = InRadius;
                        return new Vector3(Mathf.Cos(theta) * rr, yy, Mathf.Sin(theta) * rr);
                    }
                }

                Vector3 p0 = PointOnCapsule(t0);
                Vector3 p1 = PointOnCapsule(t1);
                CapsuleMesh.SurfaceAddVertex(p0);
                CapsuleMesh.SurfaceAddVertex(p1);
            }
        }

        CapsuleMesh.SurfaceEnd();

        StandardMaterial3D LineMaterial = new StandardMaterial3D();
        LineMaterial.ShadingMode        = BaseMaterial3D.ShadingModeEnum.Unshaded;
        LineMaterial.AlbedoColor        = InColor;

        MeshInstance3D Instance = new MeshInstance3D();
        Instance.Mesh           = CapsuleMesh;
        Instance.Transform      = new Transform3D(Basis.Identity, InPosition);
        Instance.SetSurfaceOverrideMaterial(0, LineMaterial);

        Shapes.AddChild(Instance);
        return Instance;
    }

    public static void Capsule(Vector3 InPosition, float InRadius, float InHeight, Color InColor, float InDuration)
    {
        MeshInstance3D Instance = Internal_CreateCapsuleMesh(InPosition, InRadius, InHeight, InColor);
        SetupShapeTimeout(Instance, InDuration);
    }

    public static Node3D CreateCapsule(String InName, Vector3 InPosition, float InRadius, float InHeight, Color InColor)
    {
        Node3D FoundInstance = Shapes.GetNode<Node3D>(InName);
        if (FoundInstance != null)
            return FoundInstance;

        MeshInstance3D Instance = Internal_CreateCapsuleMesh(InPosition, InRadius, InHeight, InColor);
        Instance.Name = InName;

        return Instance;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // DEBUG ARROW
    //////////////////////////////////////////////////////////////////////////////////////////////
    private static MeshInstance3D Internal_CreateArrowMesh(Vector3 from, Vector3 to, Color InColor, float thickness = 0.02f)
    {
        Vector3 dir   = (to - from).Normalized();
        Vector3 Start = new Vector3(0, 0, 0);
        Vector3 End   = dir * (to - from).Length();

        ImmediateMesh im = new ImmediateMesh();
        // Compute arrow head
        Vector3 right = dir.Cross(Vector3.Up);
        if (right.Length() < 0.001f)
            right = dir.Cross(Vector3.Right);
        right = right.Normalized();
    
        float angle = Mathf.DegToRad(25f);
        float arrowSize = (to - from).Length() * 0.25f;
        Vector3 back = -dir;
        
        Vector3 headRight = End + (back + right * Mathf.Tan(angle)).Normalized() * arrowSize;
        Vector3 headLeft  = End + (back - right * Mathf.Tan(angle)).Normalized() * arrowSize;
    
        // Start drawing
        im.ClearSurfaces();
        im.SurfaceBegin(Mesh.PrimitiveType.Lines);
    
        im.SurfaceSetColor(InColor);
    
        // Main line
        im.SurfaceAddVertex(Start);
        im.SurfaceAddVertex(End);
    
        // Right head
        im.SurfaceAddVertex(End);
        im.SurfaceAddVertex(headRight);
    
        // Left head
        im.SurfaceAddVertex(End);
        im.SurfaceAddVertex(headLeft);
    
        im.SurfaceEnd();
    
        StandardMaterial3D LineMaterial = new StandardMaterial3D();
        LineMaterial.ShadingMode        = BaseMaterial3D.ShadingModeEnum.Unshaded;
        LineMaterial.AlbedoColor        = InColor;

        // Create the MeshInstance3D
        MeshInstance3D arrow = new MeshInstance3D();
        arrow.Mesh = im;
        arrow.GlobalPosition = from;
        arrow.SetSurfaceOverrideMaterial(0, LineMaterial);

        Shapes.AddChild(arrow);

        return arrow;
    }



    public static void Arrow(Vector3 InPosition, Vector3 InTarget, Color InColor, float InDuration)
    {
        MeshInstance3D Instance = Internal_CreateArrowMesh(InPosition, InTarget, InColor);
        SetupShapeTimeout(Instance, InDuration);
    }

    public static Node3D CreateArrow(String InName, Vector3 InStart, Vector3 InEnd, Color InColor)
    {
        Node3D FoundInstance = Shapes.GetNode<Node3D>(InName);
        if (FoundInstance != null)
            return FoundInstance;

        MeshInstance3D Instance = Internal_CreateArrowMesh(InStart, InEnd, InColor);
        Instance.Name = InName;

        return Instance;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // DEBUG LOG
    //////////////////////////////////////////////////////////////////////////////////////////////
    private static async void Internal_UpdateLogScroller()
    {
        await Logs.ToSignal(Logs.GetTree(), SceneTree.SignalName.ProcessFrame);
        Logs.AddThemeConstantOverride("separation", 1);
        Scroller.ScrollVertical = (int)Scroller.GetVScrollBar().MaxValue;
    }

    private static Label Internal_CreateLog(String InText, Color InColor)
    {
        Label label = new Label();
        label.Text  = InText;
        label.AddThemeColorOverride("font_color", InColor);
        label.AddThemeFontSizeOverride("font_size", 12);
        Logs.AddChild(label);

        Internal_UpdateLogScroller();
        return label;
    }

    public static void LogError(String InText, float InDuration = 10f, [CallerFilePath] String InFilePath = "", [CallerLineNumber] int InLine = 0)
    {
        Label label = Internal_CreateLog($"[ERROR] {InText} (File: {InFilePath}, Line: {InLine})", Colors.Red);
        SetupLogTimeout(label, InDuration);
    }

    public static void LogWarning(String InText, float InDuration = 10f, [CallerFilePath] String InFilePath = "", [CallerLineNumber] int InLine = 0)
    {
        Label label = Internal_CreateLog($"[WARNING] {InText} (File: {InFilePath}, Line: {InLine})", Colors.Yellow);
        SetupLogTimeout(label, InDuration);
    }

    public static void Log(String InText, Color InColor, float InDuration = 10f)
    {
        Label label = Internal_CreateLog(InText, InColor);
        SetupLogTimeout(label, InDuration);
    }
}