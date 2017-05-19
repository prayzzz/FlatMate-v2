interface ObjectConstructor {
    clone<T>(obj: T): T;
}

Object.clone = function <T>(obj: T): T {
    return JSON.parse(JSON.stringify(obj));
}
