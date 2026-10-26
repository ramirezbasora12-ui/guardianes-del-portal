Shader "Guardianes/ForestWater" {
 Properties {
  [PerRendererData] _MainTex("Sprite",2D)="white"{} _Color("Tint",Color)=(1,1,1,1)
  _StencilComp("Stencil Comparison",Float)=8 _Stencil("Stencil ID",Float)=0 _StencilOp("Stencil Operation",Float)=0 _StencilWriteMask("Stencil Write Mask",Float)=255 _StencilReadMask("Stencil Read Mask",Float)=255 _ColorMask("Color Mask",Float)=15
 }
 SubShader {Tags{"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True"}
 Stencil{Ref[_Stencil] Comp[_StencilComp] Pass[_StencilOp] ReadMask[_StencilReadMask] WriteMask[_StencilWriteMask]}
 Cull Off Lighting Off ZWrite Off ZTest[unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha ColorMask[_ColorMask]
 Pass {CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
 #include "UnityCG.cginc"
 #include "UnityUI.cginc"
 struct appdata{float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
 struct v2f{float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;float4 world:TEXCOORD1;};
 sampler2D _MainTex;fixed4 _Color;float4 _ClipRect;
 v2f vert(appdata v){v2f o;o.world=v.vertex;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
 fixed4 frag(v2f i):SV_Target{
  fixed4 base=tex2D(_MainTex,i.uv);float water=saturate((base.b-base.r)*5)*saturate((base.g-base.r)*4);
  // Restrict distortion to turquoise water and crystals in the original illustration.
  float2 uv=i.uv;uv.x+=sin(uv.y*220+_Time.y*2.4)*.0008*water;uv.y+=sin(uv.x*160-_Time.y*3)*.0005*water;
  fixed4 c=tex2D(_MainTex,uv)*i.color;c.rgb+=water*(sin(_Time.y*2+uv.y*120)*.018+.012);
  float portal=exp(-dot((uv-float2(.91,.85))*float2(22,16),(uv-float2(.91,.85))*float2(22,16)));
  c.rgb+=portal*(.03+.025*sin(_Time.y*2))*float3(.8,.1,1);
 #ifdef UNITY_UI_CLIP_RECT
  c.a*=UnityGet2DClipping(i.world.xy,_ClipRect);
 #endif
  return c;
 }
 ENDCG }
 }
}
