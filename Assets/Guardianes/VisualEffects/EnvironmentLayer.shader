Shader "Guardianes/EnvironmentLayer"
{
    Properties
    {
        [PerRendererData] _MainTex("Illustrated sheet", 2D) = "white" {}
        _Region("UV region", Vector) = (0,0,1,1)
        _Mode("Motion profile", Float) = 0
        _Speed("Speed", Float) = 1
        _Intensity("Distortion", Float) = .006
        _StencilComp("Stencil Comparison",Float)=8
        _Stencil("Stencil ID",Float)=0
        _StencilOp("Stencil Operation",Float)=0
        _StencilWriteMask("Stencil Write Mask",Float)=255
        _StencilReadMask("Stencil Read Mask",Float)=255
        _ColorMask("Color Mask",Float)=15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Stencil { Ref[_Stencil] Comp[_StencilComp] Pass[_StencilOp] ReadMask[_StencilReadMask] WriteMask[_StencilWriteMask] }
        Cull Off Lighting Off ZWrite Off ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; float4 world:TEXCOORD1; fixed4 color:COLOR; };
            sampler2D _MainTex;
            float4 _MainTex_TexelSize, _Region, _ClipRect;
            float _Mode, _Speed, _Intensity, _GuardianesQuality;
            v2f vert(appdata v)
            {
                v2f o; o.world=v.vertex; o.vertex=UnityObjectToClipPos(v.vertex);
                o.uv=v.uv; o.color=v.color; return o;
            }
            fixed4 frag(v2f i):SV_Target
            {
                float2 p=(i.uv-_Region.xy)/_Region.zw;
                float t=_Time.y*_Speed;
                float q=max(.35,_GuardianesQuality);
                float envelope=sin(saturate(p.x)*UNITY_PI)*sin(saturate(p.y)*UNITY_PI);
                float2 uv=p;
                if (_Mode>.5 && _Mode<2.5)
                {
                    uv.x+=sin(p.y*36+t*5)*_Intensity*envelope*q;
                    uv.y+=sin(p.y*48+t*7)*_Intensity*envelope*q;
                }
                else if (_Mode>2.5 && _Mode<3.5)
                    uv.x+=sin(t+p.y*3)*_Intensity*p.y*p.y*envelope*q;
                else if (_Mode>4.5)
                {
                    float2 d=p-.5;
                    uv+=float2(-d.y,d.x)*sin(t*1.5+length(d)*20)*_Intensity*3*envelope*q;
                }
                // Clamp within this sprite's cell; neighbouring effects never bleed into the layer.
                float2 inset=_MainTex_TexelSize.xy*1.5;
                float2 sheet=clamp(_Region.xy+uv*_Region.zw,_Region.xy+inset,_Region.xy+_Region.zw-inset);
                fixed4 c=tex2D(_MainTex,sheet)*i.color;
                if (_Mode>.5 && _Mode<2.5) c.rgb*=1+.07*sin(p.y*75+t*8)*q;
                if (_Mode>3.5) c.rgb*=1+.09*sin(t*2+p.y*4)*q;
                #ifdef UNITY_UI_CLIP_RECT
                c.a*=UnityGet2DClipping(i.world.xy,_ClipRect);
                #endif
                return c;
            }
            ENDCG
        }
    }
}
