#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// float4x4 view_projection;
sampler TextureSampler : register(s0);
float time;

struct VertexInput {
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};
struct PixelInput {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

// PixelInput SpriteVertexShader(VertexInput v) {
//     PixelInput output;

//     output.Position = mul(v.Position, view_projection);
//     output.Color = v.Color;
//     output.TexCoord = v.TexCoord;
//     return output;
// }
float4 SpritePixelShader(PixelInput p) : SV_TARGET {
	float2 uv = p.TexCoord.xy;
	//uv is from 0-1
    uv.x += sin(uv.y * 20.0 - time * 4.0) * 0.05; 

    float4 diffuse = tex2D(TextureSampler, uv);

	// if (floor(p.Position.y) % 4 == 0) {
	// 	diffuse.rgba = 0;
	// }

    return diffuse * p.Color;
}

technique SpriteBatch {
    pass {
        // VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}