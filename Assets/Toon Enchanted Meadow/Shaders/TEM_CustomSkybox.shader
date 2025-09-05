// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Toon/TEM_CustomSkybox"
{
	Properties
	{
		[Gamma][Header(Cubemap)]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1)
		_Exposure("Exposure", Range( 0 , 8)) = 1
		[NoScaleOffset]_Tex("Cubemap (HDR)", CUBE) = "black" {}
		[Header(Rotation)][Toggle(_ENABLEROTATION_ON)] _EnableRotation("Enable Rotation", Float) = 0
		[IntRange]_Rotation("Rotation", Range( 0 , 360)) = 0
		_RotationSpeed("Rotation Speed", Float) = 1
		[Header(Fog)][Toggle(_ENABLEFOG_ON)] _EnableFog("Enable Fog", Float) = 0
		_FogOpacity("Fog Opacity", Range( 0 , 1)) = 0.5
		_FogHeight("Fog Height", Range( 0 , 1)) = 1
		_FogSmoothness("Fog Smoothness", Range( 0.01 , 1)) = 0.01
		[HideInInspector]_Tex_HDR("DecodeInstructions", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.5
		#pragma shader_feature_local _ENABLEFOG_ON
		#pragma shader_feature_local _ENABLEROTATION_ON
		#define ASE_VERSION 19801
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows nofog vertex:vertexDataFunc 
		struct Input
		{
			float3 vertexToFrag50;
			float3 worldPos;
		};

		uniform half4 _Tex_HDR;
		uniform samplerCUBE _Tex;
		uniform half _Rotation;
		uniform half _RotationSpeed;
		uniform half4 _TintColor;
		uniform half _Exposure;
		uniform half _FogHeight;
		uniform half _FogSmoothness;
		uniform half _FogOpacity;


		inline half3 DecodeHDR57( float4 Data )
		{
			return DecodeHDR(Data, _Tex_HDR);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_positionWS = mul( unity_ObjectToWorld, v.vertex );
			float lerpResult14 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
			half CAMERA_MODE17 = lerpResult14;
			float3 appendResult34 = (float3(ase_positionWS.x , ( ase_positionWS.y * CAMERA_MODE17 ) , ase_positionWS.z));
			float3 normalizeResult39 = normalize( appendResult34 );
			float3 appendResult27 = (float3(cos( radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) ) , 0.0 , ( sin( radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) ) * -1.0 )));
			float3 appendResult29 = (float3(0.0 , CAMERA_MODE17 , 0.0));
			float3 appendResult30 = (float3(sin( radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) ) , 0.0 , cos( radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) )));
			float3 normalizeResult33 = normalize( ase_positionWS );
			#ifdef _ENABLEROTATION_ON
				float3 staticSwitch46 = mul( float3x3(appendResult27, appendResult29, appendResult30), normalizeResult33 );
			#else
				float3 staticSwitch46 = normalizeResult39;
			#endif
			o.vertexToFrag50 = staticSwitch46;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			half4 Data57 = texCUBE( _Tex, i.vertexToFrag50 );
			half3 localDecodeHDR57 = DecodeHDR57( Data57 );
			half4 CUBEMAP65 = ( float4( localDecodeHDR57 , 0.0 ) * unity_ColorSpaceDouble * _TintColor * _Exposure );
			float3 ase_positionWS = i.worldPos;
			float3 normalizeResult35 = normalize( ase_positionWS );
			float lerpResult62 = lerp( saturate( pow( (0.0 + (abs( normalizeResult35.y ) - 0.0) * (1.0 - 0.0) / (_FogHeight - 0.0)) , ( 1.0 - _FogSmoothness ) ) ) , 0.0 , _FogOpacity);
			half FOG_MASK66 = lerpResult62;
			float4 lerpResult70 = lerp( unity_FogColor , CUBEMAP65 , FOG_MASK66);
			#ifdef _ENABLEFOG_ON
				float4 staticSwitch72 = lerpResult70;
			#else
				float4 staticSwitch72 = CUBEMAP65;
			#endif
			o.Emission = staticSwitch72.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.CommentaryNode;1;-2112,-704;Inherit;False;2411;608;Cubemap Coordinates;26;39;37;34;33;32;30;29;28;27;26;25;23;22;21;20;19;18;16;15;12;9;7;5;4;3;2;CUBEMAP;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-2064,-400;Half;False;Property;_RotationSpeed;Rotation Speed;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-2064,-528;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;6;-2112,-1008;Inherit;False;860;219;Switch between Perspective / Orthographic camera;4;14;11;10;8;CAMERA MODE;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2064,-656;Half;False;Property;_Rotation;Rotation;4;1;[IntRange];Create;True;0;0;0;False;0;False;0;102;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1808,-528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OrthoParams;8;-2064,-960;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-1680,-656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1616,-960;Half;False;Constant;_Float7;Float 7;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;11;-1760,-960;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;9;-1552,-656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;14;-1424,-960;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;12;-1392,-400;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1168,-960;Half;False;CAMERA_MODE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1040,-528;Half;False;Constant;_Float26;Float 26;50;0;Create;True;0;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;16;-1040,-592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;24;-2112,-16;Inherit;False;1898;485;Fog Coords on Screen;15;62;56;54;53;52;49;47;45;44;42;41;40;36;35;31;FOG EFFECT;0.4653275,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-848,-592;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-1040,-432;Inherit;False;17;CAMERA_MODE;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1040,-352;Half;False;Constant;_Float27;Float 27;50;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;21;-1040,-208;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;22;-1040,-272;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-400,-256;Inherit;False;17;CAMERA_MODE;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;25;-1040,-656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;26;-400,-432;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;27;-656,-656;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-144,-272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;-656,-464;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;30;-656,-272;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;31;-2064,32;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.MatrixFromVectors;32;-400,-656;Inherit;False;FLOAT3x3;True;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.NormalizeNode;33;-144,-528;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;34;-16,-400;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;35;-1808,32;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;38;368,-560;Inherit;False;394;188;Enable Rotation;1;46;;0,0.7386749,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;112,-576;Inherit;False;2;2;0;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;36;-1616,32;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NormalizeNode;39;144,-400;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;46;416,-496;Float;False;Property;_EnableRotation;Enable Rotation;3;0;Create;True;0;0;0;False;1;Header(Rotation);False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2064,224;Half;False;Property;_FogHeight;Fog Height;8;0;Create;True;0;0;0;False;0;False;1;0.21;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2064,352;Half;False;Property;_FogSmoothness;Fog Smoothness;9;0;Create;True;0;0;0;False;0;False;0.01;0.34;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;44;-1296,32;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1296,160;Half;False;Constant;_Float;Float;55;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-1296,256;Half;False;Constant;_Float0;Float 0;55;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;48;1200,-576;Inherit;False;1115;565;Base;7;73;63;59;58;57;55;51;;0,0.4980392,1,1;0;0
Node;AmplifyShaderEditor.VertexToFragmentNode;50;864,-496;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;47;-1040,352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;49;-1104,32;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;1248,-528;Inherit;True;Property;_Tex;Cubemap (HDR);2;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;8ea5fa8e069d8d946a1c0da7f077ba94;True;0;False;black;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.PowerNode;52;-848,32;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorSpaceDouble;55;1632,-448;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;58;1632,-96;Half;False;Property;_Exposure;Exposure;1;0;Create;True;0;0;0;False;0;False;1;0.2;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;59;1632,-272;Half;False;Property;_TintColor;Tint Color;0;1;[Gamma];Create;True;0;0;0;False;1;Header(Cubemap);False;0.5,0.5,0.5,1;1,1,1,1;False;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SaturateNode;54;-656,32;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-848,160;Half;False;Constant;_Float1;Float 1;55;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-864,256;Half;False;Property;_FogOpacity;Fog Opacity;7;0;Create;True;0;0;0;False;0;False;0.5;0.13;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;57;1632,-528;Half;False;DecodeHDR(Data, _Tex_HDR);3;Create;1;True;Data;FLOAT4;0,0,0,0;In;;Float;False;DecodeHDR;True;False;0;;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;2144,-528;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;62;-400,32;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;64;-2119.035,-1435.965;Inherit;False;618;357;;4;70;69;68;67;FINAL COLOR;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-160,32;Half;False;FOG_MASK;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;2368,-528;Half;False;CUBEMAP;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;-2069.035,-1193.965;Inherit;False;66;FOG_MASK;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;68;-2069.035,-1385.965;Inherit;False;unity_FogColor;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;-2069.035,-1273.965;Inherit;False;65;CUBEMAP;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;70;-1685.035,-1385.965;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;72;-1397.035,-1289.965;Float;False;Property;_EnableFog;Enable Fog;6;0;Create;True;0;0;0;False;1;Header(Fog);False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;73;1248,-272;Half;False;Property;_Tex_HDR;DecodeInstructions;10;1;[HideInInspector];Create;False;0;0;0;True;0;False;0,0,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;95;-848,-1328;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;Toon/TEM_CustomSkybox;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;3;0
WireConnection;5;1;2;0
WireConnection;7;0;4;0
WireConnection;7;1;5;0
WireConnection;11;0;8;2
WireConnection;11;1;8;1
WireConnection;9;0;7;0
WireConnection;14;0;10;0
WireConnection;14;1;11;0
WireConnection;14;2;8;4
WireConnection;12;0;9;0
WireConnection;17;0;14;0
WireConnection;16;0;12;0
WireConnection;18;0;16;0
WireConnection;18;1;15;0
WireConnection;21;0;12;0
WireConnection;22;0;12;0
WireConnection;25;0;12;0
WireConnection;27;0;25;0
WireConnection;27;1;20;0
WireConnection;27;2;18;0
WireConnection;28;0;26;2
WireConnection;28;1;23;0
WireConnection;29;0;20;0
WireConnection;29;1;19;0
WireConnection;29;2;20;0
WireConnection;30;0;22;0
WireConnection;30;1;20;0
WireConnection;30;2;21;0
WireConnection;32;0;27;0
WireConnection;32;1;29;0
WireConnection;32;2;30;0
WireConnection;33;0;26;0
WireConnection;34;0;26;1
WireConnection;34;1;28;0
WireConnection;34;2;26;3
WireConnection;35;0;31;0
WireConnection;37;0;32;0
WireConnection;37;1;33;0
WireConnection;36;0;35;0
WireConnection;39;0;34;0
WireConnection;46;1;39;0
WireConnection;46;0;37;0
WireConnection;44;0;36;1
WireConnection;50;0;46;0
WireConnection;47;0;41;0
WireConnection;49;0;44;0
WireConnection;49;1;45;0
WireConnection;49;2;40;0
WireConnection;49;3;45;0
WireConnection;49;4;42;0
WireConnection;51;1;50;0
WireConnection;52;0;49;0
WireConnection;52;1;47;0
WireConnection;54;0;52;0
WireConnection;57;0;51;0
WireConnection;63;0;57;0
WireConnection;63;1;55;0
WireConnection;63;2;59;0
WireConnection;63;3;58;0
WireConnection;62;0;54;0
WireConnection;62;1;56;0
WireConnection;62;2;53;0
WireConnection;66;0;62;0
WireConnection;65;0;63;0
WireConnection;70;0;68;0
WireConnection;70;1;69;0
WireConnection;70;2;67;0
WireConnection;72;1;69;0
WireConnection;72;0;70;0
WireConnection;95;2;72;0
ASEEND*/
//CHKSM=DDB185C560BDF47C4FFCE0F6E7B7E7AE88F6BF04