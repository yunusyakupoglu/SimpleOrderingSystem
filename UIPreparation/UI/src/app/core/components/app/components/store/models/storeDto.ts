export class StoreDto {
    id?: number;
    productName?: string;
    createdUserId?: number;
    createdDate?: (Date | any);
    lastUpdatedUserId?: number;
    lastUpdatedDate?: (Date | any);
    status: boolean;
    isDeleted: boolean;
    productId?: number;
    stock?: number;
    isReady: boolean;

}