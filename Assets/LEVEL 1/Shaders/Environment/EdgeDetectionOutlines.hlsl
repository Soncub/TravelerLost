#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

// Points to sample relative to starting point
static float2 sobelSamplePoints[9] = {
    float2(-1, 1), float2(0, 1), float2(1, 1),
    float2(-1, 0), float2(0, 0), float2(1, 1),
    float2(-1, -1), float2(0, -1), float2(1, -1),
};

// Weights for x component
static float sobelMatrix[9] = {
    1, 0, -1,
    2, 0, -2,
    1, 0, -1
};

// Weights for y component
static float sobelMatrix[9] = {
    1, 2, 1,
    0, 0, 0,
    -1, -2, -1
}
#endif