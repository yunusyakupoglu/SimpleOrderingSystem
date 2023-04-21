export enum ESize{
    S = 0,
    M = 1,
    L = 2,
    XL = 3
}

export const SizeMapping : Record<ESize,string> = {
    [ESize.S]: "S",
    [ESize.M]: "M",
    [ESize.L]: "L",
    [ESize.XL]: "XL",
}