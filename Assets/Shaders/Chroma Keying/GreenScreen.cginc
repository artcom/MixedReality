float _Tolerance;
float _Threshold;

// super barebones simplistic green screen removal
float greenFilter(float3 pixel, float3 targetColor) {
    float remain = length(pixel - targetColor);

    return smoothstep(_Threshold * 0.5, (_Threshold + _Tolerance) * 0.5, remain);
}

// chroma keying

// RGB to YCgCo color space
// http://en.wiki.hancel.net/wiki/YCoCg
float3 rgb2ycgco(float3 col) {
    return float3(
         0.25 * col.r + 0.50 * col.g + 0.25 * col.b,
        -0.25 * col.r + 0.50 * col.g - 0.25 * col.b,
         0.50 * col.r - 0.50 * col.b 
    );
}

// YCgCo to RGB Color Space (inverse)
float3 ycgco2rgb(float3 col) {
    return float3(
        col.x - col.y + col.z,
        col.x + col.y,
        col.x - col.y - col.z
    );
}

float ycgcoRemoval(float3 col, float3 targetColor, float yCgCoMult) {
    // YCGCO Color Space - extract CGreen, COrange
    float2 ycgco = rgb2ycgco(col).yz;
    float2 target = rgb2ycgco(targetColor).yz;
    // Adaptive Threshold over multiple samples - Otsu’s Binarization
    float d = distance(ycgco, target) * yCgCoMult;
    return smoothstep(_Threshold * 10, (_Threshold + _Tolerance) * 10, d);
}

// Color Space Conversions can be found here:
// http://www.poynton.com/PDFs/coloureq.pdf
// http://dougkerr.net/Pumpkin/articles/CIE_XYZ.pdf
// cn -> color normalization
float cnRGB2XYZ(float val) {
    if(val > 0.04045) {
        return pow((val + 0.055) / 1.055, 2.4);
    }
    return val / 12.92;
}

float3 cnRGB2XYZ(float3 rgb) {
    return float3(cnRGB2XYZ(rgb.r), cnRGB2XYZ(rgb.g), cnRGB2XYZ(rgb.b));
}

// http://www.brucelindbloom.com/index.html?Eqn_RGB_XYZ_Matrix.html
// and: http://www.easyrgb.com/en/math.php
float3 sRGB2XYZ(float3 rgb) {
    rgb = cnRGB2XYZ(rgb);

    float3x3 mat = float3x3(
        0.4124564, 0.3575761, 0.1804375,
        0.2126729, 0.7151522, 0.0721750,
        0.0193339, 0.1191920, 0.9503041
    );

    return float3(
        0.4124564 * rgb.r + 0.3575761 * rgb.g + 0.1804375 * rgb.b,
        0.2126729 * rgb.r + 0.7151522 * rgb.g + 0.0721750 * rgb.b,
        0.0193339 * rgb.r + 0.1191920 * rgb.g + 0.9503041 * rgb.b
    );
}

float cnXYZ2LAB(float val) {
    if(val > 0.008856f) {
        return pow(val, 1.0f / 3.0f);
    }
    return 7.787f * val + 0.137931f;
}

float3 cnXYZ2LAB(float3 rgb) {
    return float3(cnXYZ2LAB(rgb.r), cnXYZ2LAB(rgb.g), cnXYZ2LAB(rgb.b));
}

float3 XYZ2LAB(float3 xyz, float3 refCol) {
    xyz = xyz / refCol;
    xyz = cnXYZ2LAB(xyz);

    return float3(
        (116.0f *  xyz.y) - 16.0f,
         500.0f * (xyz.x  - xyz.y),
         200.0f * (xyz.y  - xyz.z)
    );
}

float deltaE_CIE76(float3 lab1, float3 lab2) {
    return sqrt(
        pow(lab2.x - lab1.x, 2) +
        pow(lab2.y - lab1.y, 2) +
        pow(lab2.z - lab1.z, 2)
    );
}

float deltaE_CIE76_sRGB(float3 srgb, float3 ref) {
    float3 refCol = float3(0.95047f, 1.00f, 1.08883f);
    // convert to LAB
    srgb = XYZ2LAB(sRGB2XYZ(srgb), refCol);
    ref = XYZ2LAB(sRGB2XYZ(ref), refCol);
    return deltaE_CIE76(srgb, ref);    
}

float chromaKey(float4 col, float4 targetColor) {
    if(col.a == 0) {
        return 0;
    }
    float d2 = deltaE_CIE76_sRGB(col.xyz, targetColor.xyz) / 100;
    d2 = smoothstep(_Threshold, (_Threshold + _Tolerance), d2); // blend in min/max range
    d2 = pow(d2, 1/1.5f); // gives a bit more leeway on the unity sliders
    return col.a * d2;
}

float3 spillRemoval(float3 rgb, float3 targetColor, float weight) {
    float2 target = rgb2ycgco(targetColor.rgb).yz;
    float3 ycgco = rgb2ycgco(rgb);
    float sub = dot(target, ycgco.yz) / dot(target, target);
    ycgco.yz -= target * (sub + 0.5) * weight;
    return ycgco2rgb(ycgco);
}

float ChromaMin(float2 uv, float4 texelSize, sampler2D tex, float4 targetColor) {
    float4 delta = texelSize.xyxy * float4(-0.5, -0.5, 0.5, 0.5);
    float alpha = chromaKey(tex2D(tex, uv + delta.xy), targetColor);
    alpha = min(alpha, chromaKey(tex2D(tex, uv + delta.zy), targetColor));
    alpha = min(alpha, chromaKey(tex2D(tex, uv + delta.xw), targetColor));
    alpha = min(alpha, chromaKey(tex2D(tex, uv + delta.zw), targetColor));
    return alpha;
}

float4 mixCol(float4 front, float4 median) {
    float alphaRef = median.a; // that's what's left of alpha
    float mixAlpha = alphaRef * (1 - front.a); // that's the alpha left after mixing front and median
    return front * (1 - mixAlpha) + median * mixAlpha;
}