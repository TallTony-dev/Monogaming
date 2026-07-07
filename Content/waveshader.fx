#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 view_projection;
sampler TextureSampler : register(s0);
float time;
float freq;
float amp;
float k; // 2pi/wavelength
float phase;

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

PixelInput SpriteVertexShader(VertexInput v) {
    PixelInput output;

    output.Position = mul(v.Position, view_projection);
    output.Color = v.Color;
    output.TexCoord = v.TexCoord;
    return output;
}
float4 SpritePixelShader(PixelInput p) : SV_TARGET {
	float2 uv = p.TexCoord.xy;
	//uv is from 0-1
    float wave = sin(uv.x * k - time * freq - phase) * amp; 

    float4 diffuse = float4(0,0,0,0);

    uv.y += wave * 0.1f;

    if (uv.y > 0.48 && uv.y < 0.52) {
        diffuse.a = 1;
        float dist = abs(p.TexCoord.y - 0.5);
        
        diffuse.r = (dist + 0.2) * 8;

        diffuse.g = (1/dist) * 0.006;
        if (diffuse.g > 0.4) {
            diffuse.r = 0;
        }
    }

	// if (floor(p.Position.y) % 4 == 0) {
	// 	diffuse.rgba = 0;
	// }

    return diffuse * p.Color;
}

technique SpriteBatch {
    pass {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}