#include <stdio.h>

static int add(int a, int b) {
    return a + b;
}

static int sub(int a, int b) {
    return a - b;
}

static int mul(int a, int b) {
    return a * b;
}

int main() {
    int x = add(3, 4);
    int y = sub(x, 1);
    int z = mul(y, 2);
    printf("%d\n", z);
    return 0;
}