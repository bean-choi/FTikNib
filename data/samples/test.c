#include <stdio.h>

int add(int a, int b) {
    return a + b;
}

int sub(int a, int b) {
    return a - b;
}

int main() {
    int x = add(3, 4);
    int y = sub(x, 1);
    printf("%d\n", y);
    return 0;
}
