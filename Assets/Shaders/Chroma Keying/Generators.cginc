fixed4 genColorWheel(float t) {
    return fixed4(
        sin(t),
        cos(t),
        sin(t) + cos(t * 0.7),
        1
    );
}

fixed4 genColorWheel(float t, float2 uv) {
    uv *= 3.141 / 2.5;
    return float4(
        (sin(t) * sin(uv.x)) / 2,
        (cos(t) * cos(uv.y)) / 2,
        (sin(t) + cos(t * 0.7) * sin(uv.x) + cos(uv.y * 0.7)) / 1.4,
        1
    );
}