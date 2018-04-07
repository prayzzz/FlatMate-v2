interface IKeyValuePair<K, V> {
    key: K;
    value: V;
}

export default class Dictionary<K, V> {
    private readonly entries: Array<IKeyValuePair<K, V>>;

    constructor() {
        this.entries = [];
    }

    public set(key: K, value: V) {
        const entry = this.entries.filter(entry => entry.key == key);
        if (entry.length == 0) {
            this.entries.push({key: key, value: value});
        } else {
            entry[0].value = value;
        }
    }

    public get(key: K): V | null {
        const entry = this.entries.filter(entry => entry.key == key);
        if (entry.length == 0) {
            return null;
        } else {
            return entry[0].value;
        }
    }

    public clear() {
        while (this.entries.length != 0) {
            this.entries.pop();
        }
    }

    public remove(key: K) {
        const entry = this.entries.filter(entry => entry.key != key);
        if (entry.length == 1) {
            const index = this.entries.indexOf(entry[0]);
            if (index > -1) {
                this.entries.splice(index, 1);
            }
        }
    }
}