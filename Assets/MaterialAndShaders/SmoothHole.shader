Shader "Unlit/InvertedSmoothHoleNoDistortion"
{
    Properties
    {
        _MainColor("Overlay Color", Color) = (0,0,0,1)
        _HoleCenter("Hole Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _HoleRadius("Hole Radius (UV-based)", Float) = 0.25
        _Smooth("Smoothness (UV-based)", Float) = 0.05
        _AspectRatio("Aspect Ratio (width/height)", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _MainColor;
            float4 _HoleCenter;   // (x,y) in UV space
            float  _HoleRadius;   // radius in UV
            float  _Smooth;       // smoothing in UV
            float  _AspectRatio;  // width / height of the rect

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalized UV
                float2 uv = i.uv;
                
                // Center in UV space
                float2 center = _HoleCenter.xy;

                // Shift uv by center
                float2 uvShifted = uv - center;

                // Scale X or Y based on aspect so circle isn't distorted
                // If rect is wider than tall => aspect > 1 => compress X
                // If rect is taller => aspect < 1 => compress Y
                // Here we'll assume rect is wider => multiply X by aspect
                // If you'd like an alternative, you can invert aspect in some scenarios.
                uvShifted.x *= _AspectRatio;

                // Distance from center after scaling
                float dist = length(uvShifted);

                float edge0 = _HoleRadius - _Smooth;
                float edge1 = _HoleRadius + _Smooth;

                // circle hole => alpha=0 in center, 1 outside
                float alpha = smoothstep(edge0, edge1, dist);

                // Multiply final color alpha
                fixed4 col = _MainColor;
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
